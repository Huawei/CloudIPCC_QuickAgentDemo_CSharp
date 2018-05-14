using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;
using System.Threading;
using HuaweiAgentGateway.AgentGatewayEntity;
using HuaweiAgentGateway;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security;
using Microsoft.Win32;
using Newtonsoft.Json;
using HuaweiAgentGateway.Utils;
using HuaweiAgentGateway.Entity.AGWEntity;

namespace HuaweiAgentGateway
{
    public class AgentGatewayHelper
    {
        #region  属性

        private static List<string> lst_cookie { set; get; }
        private static string guid = string.Empty;
        private static string tmpGuid = string.Empty;
        public static string BaseUri { get; set; }
        public static string BackupUrl { set; get; }
        public static string SpecError = string.Empty;

        public static bool _isSslNoError { get; set; }

        #endregion

        public enum HttpMethod
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        /// <summary>
        /// 增加request请求头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="header"></param>
        /// <param name="value"></param>
        public static void Add(HttpWebRequest request, HttpRequestHeader header, string value)
        {
            //CodeDEX:安全整改
            Add(request, header.ToString(), value);
        }

        /// <summary>
        /// 增加request请求头
        /// </summary>
        /// <param name="request"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void Add(HttpWebRequest request, string name, string value)
        {
            //CodeDEX:安全整改
            Cookie cookie = new Cookie(name, value);
            request.Headers.Add(cookie.ToString().Replace(name + "=", name + ":"));
        }

        private static T CallService<T>(string uri, HttpMethod httpMethod)
        {
            return CallService<T>(uri, httpMethod, null);
        }

        private static T CallService<T>(string uri, HttpMethod httpMethod, object requestData)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                SetSecurityProtocolByUrl(uri);
                request.Timeout = 20000;
                request.KeepAlive = false;
                if (requestData != null)
                {
                    request.ContentType = @"application/json; charset=UTF-8";
                }
                request.Method = httpMethod.ToString();

                if (!string.IsNullOrEmpty(guid))
                {
                    Add(request, "guid", guid);
                }
                if (lst_cookie != null && lst_cookie.Count > 0)
                {
                    foreach (var cookie in lst_cookie)
                    {
                        Add(request, HttpRequestHeader.Cookie, cookie);
                    }
                }

                if (requestData != null && !string.IsNullOrEmpty(requestData.ToString()))
                {
                    using (Stream rs = request.GetRequestStream())
                    {
                        var btsdata = GetBytes(requestData);
                        rs.Write(btsdata, 0, btsdata.Length);
                        rs.Flush();
                    }
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tmpGuid = response.Headers.Get("Set-GUID");
                    if (!string.IsNullOrEmpty(tmpGuid))
                    {
                        tmpGuid = tmpGuid.Substring(tmpGuid.IndexOf("=") + 1);
                    }
                    SetCookieByResponse(response);
                }
                using (Stream rs = response.GetResponseStream())
                {
                    List<byte> bts = new List<byte>();
                    int tmp;
                    while ((tmp = rs.ReadByte()) >= 0)
                    {
                        bts.Add((byte)tmp);
                    }

                    string strResponse = string.Empty;
                    if (string.Compare("GBK", response.CharacterSet, true) == 0)
                    {
                        strResponse = System.Text.Encoding.GetEncoding("GBK").GetString(bts.ToArray());
                    }
                    else
                    {
                        strResponse = System.Text.UTF8Encoding.UTF8.GetString(bts.ToArray());
                    }
                    var result = HuaweiAgentGateway.Utils.JsonUtil.DeJsonEx<T>(strResponse);
                    return result;
                }
            }
            catch (Exception ex)
            {
                SpecError = ex.Message;
                if (!string.IsNullOrEmpty(BackupUrl) && !string.Equals(BaseUri, BackupUrl))
                {
                    BaseUri = BackupUrl;
                }
                throw ex;
            }
        }

        private static void SetCookieByResponse(HttpWebResponse response)
        {
            if (null == response || null == response.Headers || null == response.Headers.Keys)
            {
                return;
            }
            if (null == lst_cookie)
            {
                lst_cookie = new List<string>();
            }

            int index = 0;
            foreach (var item in response.Headers.Keys)
            {
                if (item.ToString() != "Set-Cookie")
                {
                    index++;
                    continue;
                }
                var ck = response.Headers.Get(index);
                if (!lst_cookie.Contains(ck))
                {
                    lst_cookie.Add(ck);
                }
                index++;
            }
        }

        private static byte[] GetBytes(object input)
        {
            string data = string.Empty;
            if (input.GetType() != typeof(string))
            {
                data = HuaweiAgentGateway.Utils.JsonUtil.ToJson(input);
            }
            else
            {
                data = input.ToString();
            }
            return System.Text.UTF8Encoding.UTF8.GetBytes(data);
        }

        public static EventHandler<AgentEventData> CallBackEvent;
        private static System.Timers.Timer t = new System.Timers.Timer();
        private static string currentWorkNo = string.Empty;
        internal static int callBackFailedTimes = 0;
        internal static int callNoRightTimes = 0;
        private const int maxCallBackFailedTimes = 8;
        private const int maxNoRightMaxTimes = 15;

        static AgentGatewayHelper()
        {
            t.Elapsed += (a, b) =>
            {
                t.Stop();
                if (!string.IsNullOrEmpty(currentWorkNo))
                {
                    //需要内部做的逻辑处理
                    try
                    {
                        AgentEventData eventData = AgentGatewayHelper.GetAgentEvents(currentWorkNo);
                        if (null == eventData)
                        {
                            callBackFailedTimes++;
                            if (callBackFailedTimes >= maxCallBackFailedTimes)
                                ForceOutAndClearInfo();
                        }
                        else
                        {
                            string data = eventData.Data;
                            var jObject = Parse(data);
                            var retcode = jObject["retcode"];
                            if (retcode == null || !eventData.IsSuccess || retcode.ToString() == AGWErrorCode.NoRight || retcode.ToString() == AGWErrorCode.NotLogIn)
                            {
                                callNoRightTimes++;
                                if (callNoRightTimes >= maxNoRightMaxTimes)
                                    ForceOutAndClearInfo();
                            }
                            if (!string.IsNullOrEmpty(eventData.CallBackFunctionName))
                            {
                                callBackFailedTimes = 0;
                                callNoRightTimes = 0;
                                if (CallBackEvent != null)
                                {
                                    new Thread(
                                            () =>
                                            {
                                                CallBackEvent(null, eventData);
                                            }
                                        ).Start();
                                }
                            }
                        }
                    }
                    catch (AgentGatewayException agex)
                    {
                        SpecError = agex.Message;
                        callNoRightTimes++;
                        if (callNoRightTimes >= maxNoRightMaxTimes)
                            ForceOutAndClearInfo();
                    }
                    catch (Exception ex)
                    {
                        SpecError = ex.Message;
                        if (!string.IsNullOrEmpty(BackupUrl) && !string.Equals(BaseUri, BackupUrl))
                        {
                            BaseUri = BackupUrl;
                        }
                        callBackFailedTimes++;
                        if (callBackFailedTimes >= maxCallBackFailedTimes)
                            ForceOutAndClearInfo();
                    }
                }
                t.Start();
            };
            t.Interval = 500;
            t.Start();
        }

        private static void ForceOutAndClearInfo()
        {
            currentWorkNo = string.Empty;
            callBackFailedTimes = 0;
            callNoRightTimes = 0;
            CallBackEvent(null, new AgentEventData() { CallBackFunctionName = "Force_Logout", Data = string.Empty });
        }

        public static void StartCallBackAndSetInfo(string workNo)
        {
            currentWorkNo = workNo;
            lst_cookie = new List<string>();
        }

        public static void StopCallBackAndClearInfo()
        {
            currentWorkNo = string.Empty;
            guid = string.Empty;
            if (null == lst_cookie || lst_cookie.Count == 0)
            {
                return;
            }
            lst_cookie.Clear();
        }

        public static AgentEventData GetAgentEvents(string agentid)
        {
            string uri = BaseUri + string.Format(AgentGatewayUri.AGENTEVENT_URI, agentid);
            SetSecurityProtocolByUrl(uri);
            string strInput = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 20000;
                request.KeepAlive = false;
                request.ContentType = @"application/json";
                request.Method = HttpMethod.GET.ToString();

                if (!string.IsNullOrEmpty(guid))
                {
                    Add(request, "guid", guid);
                }
                if (lst_cookie != null && lst_cookie.Count > 0)
                {
                    foreach (var cookie in lst_cookie)
                    {
                        Add(request, HttpRequestHeader.Cookie, cookie);
                    }
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string tmpGuid = response.Headers.Get("Set-GUID");
                    if (!string.IsNullOrEmpty(tmpGuid))
                    {
                        tmpGuid = tmpGuid.Substring(tmpGuid.IndexOf("=") + 1);
                    }
                }
                using (Stream rs = response.GetResponseStream())
                {
                    List<byte> bts = new List<byte>();
                    int tmp;
                    while ((tmp = rs.ReadByte()) >= 0)
                    {
                        bts.Add((byte)tmp);
                    }
                    strInput = System.Text.UTF8Encoding.UTF8.GetString(bts.ToArray());
                    if (string.Equals("{\"message\":\"\",\"retcode\":\"0\"}", strInput))//屏蔽AGW返回的空的事件
                    {
                        return new AgentEventData() { IsSuccess = true, CallBackFunctionName = null, Data = strInput };
                    }
                    AgentEventData data = new AgentEventData();
                    var jObject = Parse(strInput);
                    var @event = jObject["event"];
                    if (@event == null)
                    {
                        data.IsSuccess = false;
                        data.Data = strInput;
                        data.CallBackFunctionName = null;
                        return data;
                    }
                    else
                    {
                        data.IsSuccess = true;
                        var eventObject = Parse(@event.ToString());
                        var eventType = eventObject["eventType"];
                        data.Data = strInput;
                        data.CallBackFunctionName = eventType.ToString();
                        return data;
                    }
                }
            }
            catch (WebException webexc)
            {
                SpecError = webexc.Message;
                if (!string.IsNullOrEmpty(BackupUrl) && !string.Equals(BaseUri, BackupUrl))
                {
                    BaseUri = BackupUrl;
                }
                return null;
            }
        }

        /// <summary>
        /// 解析json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject Parse(string json)
        {
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    return Newtonsoft.Json.Linq.JObject.Parse(json);
                }
            }
            catch(JsonException exc)
            {
                SpecError = exc.Message;
            }
            return new JObject();
        }

        public static AgentGatewayResponse<OnlineAgentResult> OnlineAgent(string agentid, SignInParam param)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.ONLINEAGENT_URI, agentid);
            AgentGatewayResponse<OnlineAgentResult> result = CallService<AgentGatewayResponse<OnlineAgentResult>>(realUri, HttpMethod.PUT, param);
            if (result.retcode == AGWErrorCode.OK)
            {
                guid = tmpGuid;
            }
            return result;
        }

        public static AgentGatewayResponse<OnlineAgentResult> OnlineAgentForceLogin(string agentid, SignInParam param)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.OnlineAgentForceLogin_URI, agentid);
            AgentGatewayResponse<OnlineAgentResult> result = CallService<AgentGatewayResponse<OnlineAgentResult>>(realUri, HttpMethod.PUT, param);
            if (result.retcode == AGWErrorCode.OK)
            {
                guid = tmpGuid;
            }
            return result;
        }

        public static AgentGatewayResponse ResetSkill(string agentId, bool autoConfig, List<int> skills, bool phonelinkage)
        {
            //写死0这边目前联动的时候用签入里面的联动为准
            string strSkills = string.Empty;
            if (skills != null)
            {
                for (int i = 0; i < skills.Count; ++i)
                {
                    if (i != skills.Count - 1)
                    {
                        strSkills += skills[i] + ";";
                    }
                    else
                    {
                        strSkills += skills[i];
                    }
                }
            }
            string realUri = BaseUri + string.Format(AgentGatewayUri.RESETSKILL_URI, agentId, autoConfig ? "true" : "false", autoConfig ? "" : strSkills, phonelinkage ? 1 : 0);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static AgentGatewayResponse Logout(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.FORCELOGOUT_URI, agentid);
            var res = CallService<AgentGatewayResponse>(realUri, HttpMethod.DELETE);
            return res;
        }

        /// <summary>
        /// AgentGateWay的休息方法
        /// </summary>
        public static AgentGatewayResponse Rest(string agentId, int restTime, int restReason)
        {
            var realUri = BaseUri + string.Format(AgentGatewayUri.REST_URI, agentId, restTime, restReason);
            var result = CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);

            return result;
        }

        /// <summary>
        /// AgentGateWay的取消休息方法
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public static AgentGatewayResponse CancelRest(string agentId)
        {
            var realUri = BaseUri + string.Format(AgentGatewayUri.CANCELREST_URI, agentId);
            var result = CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);
            return result;
        }

        /// <summary>
        /// 外呼
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AgentGatewayResponse<object> CallOut(string agentid, AGWCallData data)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.CallOut_URI, agentid);
            return CallService<AgentGatewayResponse<object>>(realUri, HttpMethod.PUT, data);
        }

        /// <summary>
        /// 内部呼叫
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AgentGatewayResponse<string> InnerCall(string agentid, AGWCallData data)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.CallInner_URI, agentid);
            return CallService<AgentGatewayResponse<string>>(realUri, HttpMethod.PUT, data);
        }

        /// <summary>
        /// AgentGateWay的挂断方法
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public static string Release(string agentId)
        {
            var realUri = BaseUri + string.Format(AgentGatewayUri.RELEASE_URI, agentId);
            var result = CallService<AgentGatewayResponse>(realUri, HttpMethod.DELETE);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            if (result.retcode == "200-017")
            {
                return AGWErrorCode.OK;
            }
            else
            {
                return result.retcode;
            }
        }

        public static AgentGatewayResponse<CallInfoBeforeAnswer> QueryCallInfo(string agentId)
        {
            var realUri = BaseUri + string.Format(AgentGatewayUri.CallDataCallInfo_URI, agentId);
            var result = CallService<AgentGatewayResponse<CallInfoBeforeAnswer>>(realUri, HttpMethod.GET);
            return result;
        }

        public static AgentGatewayResponse GetHold(string agentid, string callid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.GETHOLD_URI, agentid, callid);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);
        }

        public static AgentGatewayResponse<List<HoldListData>> HoldList(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.HOLDLIST_URI, agentid);
            return CallService<AgentGatewayResponse<List<HoldListData>>>(realUri, HttpMethod.GET);
        }

        public static AgentGatewayResponse<List<SkillInfo>> AgentSkills(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.AgentSkills_URI, agentid);
            return CallService<AgentGatewayResponse<List<SkillInfo>>>(realUri, HttpMethod.GET);
        }

        public static AgentGatewayResponse<List<AgentStateInfo>> AllAgentStatus(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.AgentStatusInMyVdn_URI, agentid);
            return CallService<AgentGatewayResponse<List<AgentStateInfo>>>(realUri, HttpMethod.GET);
        }

        public static AgentGatewayResponse<List<AGWAccessCode>> QueryVDNAccessCodeInfo(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.QueueDevice_VDNAccessCodeInfo_URI, agentid);
            return CallService<AgentGatewayResponse<List<AGWAccessCode>>>(realUri, HttpMethod.GET);
        }

        /// <summary>
        /// 查询座席所在 VDN 所有班组信息
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static AgentGatewayResponse<List<AGWWorkGroupData>> QueryVDNWorkGroup(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.QryWorkGroup, agentid);
            return CallService<AgentGatewayResponse<List<AGWWorkGroupData>>>(realUri, HttpMethod.GET);
        }

        /// <summary>
        /// 查询座席所在VDN的IVR信息
        /// </summary>
        /// <param name="agentid">座席工号</param>
        /// <returns>IVR信息列表</returns>
        public static AgentGatewayResponse<List<AGWIVRInfo>> QeueDeviceIVRInfo(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.QueueDevice_IVRInfo_URI, agentid);
            return CallService<AgentGatewayResponse<List<AGWIVRInfo>>>(realUri, HttpMethod.GET);
        }

        /// <summary>
        /// 获取座席所在VDN的技能队列信息
        /// </summary>
        /// <param name="agentid">座席工号</param>
        /// <returns>技能列表</returns>
        public static AgentGatewayResponse<List<AGWSkill>> QeueDeviceAgentVDNSkills(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.QueueDevice_AgentVDNSkills_URI, agentid);
            return CallService<AgentGatewayResponse<List<AGWSkill>>>(realUri, HttpMethod.GET);
        }

        public static AgentGatewayResponse Transfer(string agentid, AGWTransferData data)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.Transfer_URI, agentid);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST, data);
        }

        public static AgentGatewayResponse<AGWAgentStatus> AgentStatus(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.AgentStatus_URI, agentid);
            return CallService<AgentGatewayResponse<AGWAgentStatus>>(realUri, HttpMethod.GET);
        }

        public static AgentGatewayResponse<object> InnerHelp(string agentid, AGWInnerHelp data)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.InnerHelp_URI, agentid);
            return CallService<AgentGatewayResponse<object>>(realUri, HttpMethod.POST, data);
        }

        public static AgentGatewayResponse ConfJoin(string agentid, AGWConfJoin data)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.ConfJoin_URI, agentid);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST, data);
        }

        public static AgentGatewayResponse ConnectHold(string agentid, string callid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.ConnectHold_URI, agentid, callid);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            _isSslNoError = errors == SslPolicyErrors.None;
            return true;
        }

        /// <summary>
        /// 二次拨号
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static AgentGatewayResponse SecondDial(string agentid, string number)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.SecDial, agentid, number);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);
        }

        /// <summary>
        /// 获取所有的 callid
        /// </summary>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public static AgentGatewayResponse<List<string>> GetCallIDs(string agentid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.GetCallIDs, agentid);
            return CallService<AgentGatewayResponse<List<string>>>(realUri, HttpMethod.GET);
        }

        /// <summary>
        /// 根据 callid 获取呼叫信息
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="callid"></param>
        /// <returns></returns>
        public static AgentGatewayResponse<AGWCallInfo> GetCallInfoByID(string agentid, string callid)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.GetCallInfoById, agentid, callid);
            return CallService<AgentGatewayResponse<AGWCallInfo>>(realUri, HttpMethod.GET);
        }

        /// <summary>
        /// 设置是否进入空闲态
        /// </summary>
        /// <param name="agentid"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static AgentGatewayResponse SetDeviceAutoEnterWork(string agentid, bool flag)
        {
            string realUri = BaseUri + string.Format(AgentGatewayUri.SetAutoEnterIdle, agentid, flag);
            return CallService<AgentGatewayResponse>(realUri, HttpMethod.POST);
        }

        #region

        /// <summary>
        /// common requeest function
        /// </summary>
        /// <param name="url">url 后半部分地址（例如二次拨号下 url 就是：/voicecall/工号/seconddial/号码）</param>
        /// <param name="method">http request method</param>
        /// <param name="data">request data</param>
        /// <returns></returns>
        public static T CallFunction<T>(string url, HttpMethod method, object data)
        {
            string reqUrl = BaseUri + url;
            return CallService<T>(reqUrl, method, data);
        }

        /// <summary>
        /// set SecurityProtocol by url.
        /// </summary>
        /// <param name="url">request url</param>
        /// <remarks>use .net 4.5 security protocol as default.if not supported, then use .net 4.0 security protocol</remarks>
        private static void SetSecurityProtocolByUrl(string url)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith("http:"))
            {
                return;
            }
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)192 | (SecurityProtocolType)768;
            }
            catch (NotSupportedException nsexc)
            {
                SpecError = nsexc.Message;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            }
            ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
        }

        private static void SetSecurityProtocolByUrlEx(string url)
        {
            if (string.IsNullOrEmpty(url) || url.StartsWith("http:"))
            {
                return;
            }
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)192 | (SecurityProtocolType)768;
            }
            catch (NotSupportedException nsexc)
            {
                SpecError = nsexc.Message;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            }
            ServicePointManager.ServerCertificateValidationCallback += CheckValidationResult;
        }

        #endregion

        #region  特别方法（文件上传，下载）

        public static string HttpUploadFile(string url, string path)
        {
            var reqUrl = BaseUri + url;
            SetSecurityProtocolByUrl(reqUrl);
            string content = string.Empty;
            HttpWebRequest request = WebRequest.Create(reqUrl) as HttpWebRequest;

            if (!string.IsNullOrEmpty(guid))
            {
                Add(request, "guid", guid);
            }
            if (lst_cookie != null && lst_cookie.Count > 0)
            {
                foreach (var cookie in lst_cookie)
                {
                    Add(request, HttpRequestHeader.Cookie, cookie);
                }
            }
            if (null == request)
            {
                return content;
            }

            request.Method = "POST";
            string boundary = DateTime.Now.Ticks.ToString("X"); // 随机分隔线
            request.ContentType = "multipart/form-data;charset=utf-8;boundary=" + boundary;
            byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "\r\n");
            byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            

            try
            {
                int pos = path.LastIndexOf("\\");
                string fileName = path.Substring(pos + 1);

                //请求头部信息 
                StringBuilder sbHeader = new StringBuilder(string.Format("Content-Disposition:form-data;name=\"file\";filename=\"{0}\"\r\nContent-Type:application/octet-stream\r\n\r\n", fileName));
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sbHeader.ToString());

                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                byte[] bArr = new byte[fs.Length];
                try
                {
                    int fileLen = fs.Read(bArr, 0, bArr.Length);
                    if (fileLen <= 0)
                    {
                        SpecError = "file end";
                    }
                }
                catch (FileNotFoundException fileExce)
                {
                    SpecError = fileExce.Message;
                }
                catch (IOException ioexc)
                {
                    SpecError = ioexc.Message;
                }
                finally
                {
                    fs.Close();
                }

                Stream postStream = request.GetRequestStream();
                if (null == postStream)
                {
                    return content;
                }
                postStream.Write(itemBoundaryBytes, 0, itemBoundaryBytes.Length);
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                postStream.Write(bArr, 0, bArr.Length);
                postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                postStream.Close();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (null == response)
                {
                    return content;
                }
                SetCookieByResponse(response);
                using (Stream rs = response.GetResponseStream())
                {
                    List<byte> bts = new List<byte>();
                    int tmp;
                    while ((tmp = rs.ReadByte()) >= 0)
                    {
                        bts.Add((byte)tmp);
                    }

                    if (string.Compare("GBK", response.CharacterSet, true) == 0)
                    {
                        content = System.Text.Encoding.GetEncoding("GBK").GetString(bts.ToArray());
                    }
                    else
                    {
                        content = System.Text.UTF8Encoding.UTF8.GetString(bts.ToArray());
                    }
                    return content;
                }
            }
            catch (WebException exc)
            {
                SpecError = exc.Message;
                return AGWErrorCode.SpecErr;
            }
        }

        public static List<byte> DownloadFileStream(string uri, HttpMethod httpMethod, object requestData)
        {
            try
            {
                var url = BaseUri + uri;
                SetSecurityProtocolByUrl(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                if (requestData != null)
                {
                    request.ContentType = @"application/json; charset=UTF-8";
                }
                request.Method = httpMethod.ToString();

                if (!string.IsNullOrEmpty(guid))
                {
                    Add(request, "guid", guid);
                }
                if (lst_cookie != null && lst_cookie.Count > 0)
                {
                    foreach (var cookie in lst_cookie)
                    {
                        Add(request, HttpRequestHeader.Cookie, cookie);
                    }
                }

                if (requestData != null && !string.IsNullOrEmpty(requestData.ToString()))
                {
                    using (Stream rs = request.GetRequestStream())
                    {
                        var btsdata = GetBytes(requestData);
                        rs.Write(btsdata, 0, btsdata.Length);
                        rs.Flush();
                    }
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tmpGuid = response.Headers.Get("Set-GUID");
                    if (!string.IsNullOrEmpty(tmpGuid))
                    {
                        tmpGuid = tmpGuid.Substring(tmpGuid.IndexOf("=") + 1);
                    }
                    SetCookieByResponse(response);
                }
                using (Stream rs = response.GetResponseStream())
                {
                    List<byte> bts = new List<byte>();
                    int tmp;
                    while ((tmp = rs.ReadByte()) >= 0)
                    {
                        bts.Add((byte)tmp);
                    }
                    return bts;
                }
            }
            catch (WebException ex)
            {
                SpecError = ex.Message;
                return null;
            }
        }

        #endregion


        #region  Certification Check

        public static T CallFunctionForCheckCertificate<T>(string url, HttpMethod httpMethod)
        {
            T ret = default(T);
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                SetSecurityProtocolByUrlEx(url);

                request.Timeout = 20000;
                request.KeepAlive = false;
                request.Method = httpMethod.ToString();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    tmpGuid = response.Headers.Get("Set-GUID");
                    if (!string.IsNullOrEmpty(tmpGuid))
                    {
                        tmpGuid = tmpGuid.Substring(tmpGuid.IndexOf("=") + 1);
                    }
                    SetCookieByResponse(response);
                }
                using (Stream rs = response.GetResponseStream())
                {
                    List<byte> bts = new List<byte>();
                    int tmp;
                    while ((tmp = rs.ReadByte()) >= 0)
                    {
                        bts.Add((byte)tmp);
                    }

                    string strResponse = string.Empty;
                    if (string.Compare("GBK", response.CharacterSet, true) == 0)
                    {
                        strResponse = System.Text.Encoding.GetEncoding("GBK").GetString(bts.ToArray());
                    }
                    else
                    {
                        strResponse = System.Text.UTF8Encoding.UTF8.GetString(bts.ToArray());
                    }
                    var result = HuaweiAgentGateway.Utils.JsonUtil.DeJsonEx<T>(strResponse);
                    return result;
                }
            }
            catch (WebException exc)
            {
                SpecError = exc.Message;
            }

            return ret;
        }

        public static bool IsCertificationValid()
        {
            return _isSslNoError;
        }

        public static void InitCheckValid()
        {
            _isSslNoError = true;
        }

        #endregion
    }
}
