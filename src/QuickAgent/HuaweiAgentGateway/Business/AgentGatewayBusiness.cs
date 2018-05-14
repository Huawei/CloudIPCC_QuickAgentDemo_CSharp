using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaweiAgentGateway.AgentGatewayEntity;
using HuaweiAgentGateway.Utils;
using HuaweiAgentGateway.Entity;
using HuaweiAgentGateway.Entity.AgentGatewayEntity;
using HuaweiAgentGateway.Entity.AGWEntity;
using System.Net;

namespace HuaweiAgentGateway
{
    public class AgentGatewayBusiness : IBusiness
    {
        #region  属性

        public string ErrorMessage { set; get; }

        public bool IsForceChange { set; get; }

        /// <summary>
        /// 相关联的IBusinessEvents类主要是回调的时候上抛函数都在这个类里面
        /// </summary>
        internal static IBusinessEvents RelatedBusinessEvents = null;

        /// <summary>
        /// 签入AGW的参数
        /// </summary>
        public SignInParam Info { get; set; }

        /// <summary>
        /// 当前成功登陆的工号（登出的时候会赋空）
        /// </summary>
        public string WorkNo { get; set; }

        /// <summary>
        /// 是否自动进入工作态
        /// </summary>
        private bool _autoEnterWork { get; set; }

        /// <summary>
        /// 当前登陆的坐席的VDN号码--在AGW签入成功的时候返回的
        /// </summary>
        private int _vdnID { get; set; }

        private string m_phoneAlertCallId = String.Empty;//用于话机振铃时，判断当前呼叫是否外呼或内部呼叫

        /// <summary>
        /// AGW 的服务基地址
        /// </summary>
        public string BaseUri { get { return AgentGatewayHelper.BaseUri; } set { AgentGatewayHelper.BaseUri = value; } }

        /// <summary>
        /// AGW 的服务基地址
        /// </summary>
        public string BackupUrl { get { return AgentGatewayHelper.BackupUrl; } set { AgentGatewayHelper.BackupUrl = value; } }

        /// <summary>
        /// 备用地址
        /// </summary>
        public string BackBaseUrl { set; get; }

        /// <summary>
        /// "内部呼叫" 的呼叫类型数值
        /// </summary>
        private const int INNERCALL_TYPE = 6;

        /// <summary>
        /// 两方内部求助呼叫
        /// </summary>
        private const int INTER_TWO_HELP = 51;

        /// <summary>
        /// 三方内部求助呼叫
        /// </summary>
        private const int INTER_THREE_HELP = 52;

        /// <summary>
        /// 是否存在保持的列表
        /// </summary>
        public bool ExistHold { get; set; }

        [FlagsAttribute]
        private enum QC_Type
        {
            Insert,
            Supervise,
            Intercept,
            ReqWhisper,
            StopWhisper,
            SwitchWhisper,
            StopListenAndInert,
            ForceOut
        }

        #endregion

        #region  请求的地址

        #region  Voice Call

        /// <summary>
        /// preview call out
        /// {0}: agentid
        /// </summary>
        private const string URL_PREVIEW_CALLOUT = "/voicecall/{0}/previewcallout";

        /// <summary>
        /// snatch pickup
        /// {0}: agentid
        /// {1}: destWorkNo
        /// </summary>
        private const string URL_SNATCHPICKUP = "/voicecall/{0}/snatchpickup?destWorkNo={1}";

        /// <summary>
        /// phone pick up(used for phonelinkage)
        /// {0}: agentid
        /// </summary>
        private const string URL_PHONEPICKUP = "/voicecall/{0}/phonepickup";

        #endregion

        #region   Text Chat

        /// <summary>
        /// send msg without call
        /// {0}：workno
        /// </summary>
        private const string URL_TC_SENDMSGWITHOUTCALL = "/textchat/{0}/sendmessagewithoutcall";

        /// <summary>
        /// 文字交谈应答
        /// {0}：workno
        /// {1}：callid
        /// </summary>
        private const string URL_TXTCHATANS = "/textchat/{0}/answer/{1}";

        /// <summary>
        /// 发送文字消息
        /// {0}：workno
        /// </summary>
        private const string URL_TXTMSGSEND = "/textchat/{0}/chatmessage";

        /// <summary>
        /// 内部文字交谈
        /// {0}：workno
        /// {1}：destno
        /// </summary>
        private const string URL_INTERNALTXTCHAT = "/textchat/{0}/internalcall/{1}";

        /// <summary>
        /// 转移文字交谈
        /// {0}：workno
        /// </summary>
        private const string URL_TRANSTXTCHAT = "/textchat/{0}/transfer";

        /// <summary>
        /// 关闭文字交谈
        /// {0}：workno
        /// {1}：callID
        /// </summary>
        private const string URL_CLOSETXTCHAT = "/textchat/{0}/drop/{1}";

        /// <summary>
        /// 拒绝文字交谈
        /// {0}：workno
        /// {1}：callID
        /// </summary>
        private const string URL_REJECTTXTCHAT = "/textchat/{0}/reject/{1}";

        /// <summary>
        /// 确认接收文字消息
        /// {0}：workNo
        /// </summary>
        private const string URL_MSGCONFIRM = "/textchat/{0}/confirmmessage";

        /// <summary>
        /// 富媒体文件上传
        /// {0}：workno
        /// {1}：callId
        /// {2}：isNeedConfirm
        /// {3}：chatId
        /// {4}：msgType
        /// {5}：remark
        /// </summary>
        private const string URL_UPLOADMEDIAFILE = "/textchat/{0}/mediafile?callId={1}&isNeedConfirm={2}&chatId={3}&msgType={4}&remark={5}";

        /// <summary>
        /// 富媒体文件下载
        /// {0}：workno
        /// {1}：filePath
        /// </summary>
        private const string URL_DOWNLOADMEDIAFILE = "/textchat/{0}/mediafile?filePath={1}";

        /// <summary>
        /// qry text chat call content
        /// {0}：workno
        /// {1}：callId
        /// {2}：pageNo
        /// {3}：pageSize
        /// </summary>
        private const string URL_QRYCALLCONTENT = "/textchat/{0}/callcontent?callId={1}&pageNo={2}&pageSize={3}";

        /// <summary>
        /// qry call session
        /// {0}：workno
        /// </summary>
        private const string URL_QRYCALLSESSION = "/textchat/{0}/callsession";

        #endregion

        #region  Quality Control

        /// <summary>
        /// quality control insert
        /// {0}：agentid
        /// {1}：workNo
        /// </summary>
        private const string URL_QC_INSERT = "/qualitycontrol/{0}/addinsert/{1}";

        /// <summary>
        /// quality contrl supervise
        /// {0}：agentid
        /// {1}：workNo
        /// </summary>
        private const string URL_QC_SUPERVISE = "/qualitycontrol/{0}/addsupervise/{1}";

        /// <summary>
        /// quality contrl intercept
        /// {0}：agentid
        /// {1}：workNo
        /// </summary>
        private const string URL_QC_INTERCEPT = "/qualitycontrol/{0}/intercept/{1}";

        /// <summary>
        /// quality control request whisper
        /// {0}：agentid
        /// {1}：whisperagentid
        /// </summary>
        private const string URL_QC_REQWHISPER = "/qualitycontrol/{0}/requestwhisperagent?whisperagentid={1}";

        /// <summary>
        /// quality control stop whisper
        /// {0}：agentid
        /// {1}：whisperagentid
        /// </summary>
        private const string URL_QC_STOPWHISPER = "/qualitycontrol/{0}/requeststopwhisperagent?whisperagentid={1}";

        /// <summary>
        /// quality control stop listen and insert
        /// {0}：agentid
        /// {1}：workno
        /// </summary>
        private const string URL_QC_STOPINSERTLISTEN = "/qualitycontrol/{0}/{1}";

        /// <summary>
        /// quality control whisper switch
        /// {0}：agentid
        /// {1}：whisperagentid
        /// {2}: switchtype
        /// </summary>
        private const string URL_QC_WHISPERSWITCH = "/qualitycontrol/{0}/requestswitchinsertwhisperagent?whisperagentid={1}&switchtype={2}";

        /// <summary>
        /// quality control force out
        /// {0}：agentid
        /// {1}：whisperagentid
        /// </summary>
        private const string URL_QC_FORCEOUT = "/qualitycontrol/{0}/forcelogout/{1}";

        /// <summary>
        /// start record screen
        /// {1}: agentID
        /// </summary>
        private const string URL_RECORDSCREENSTART = "/recordplay/{0}/startrecordscreen";

        /// <summary>
        /// stop record screen
        /// {1}: agentID
        /// </summary>
        private const string URL_RECORDSCREENSTOP = "/recordplay/{0}/stoprecordscreen";

        #endregion

        #region Qry Info

        /// <summary>
        /// qry agent sign skills
        /// {0}: agent id
        /// {1}: work no
        /// </summary>
        private const string URL_SIGNSKILLS = "/queuedevice/{0}/agentskill/{1}";

        /// <summary>
        /// an agent is censor
        /// {0}: agent id
        /// </summary>
        private const string URL_ISCENSOR = "/agentgroup/{0}/iscensor";

        /// <summary>
        /// work group which current agent in
        /// {0}: agent id
        /// {1}: workno
        /// </summary>
        private const string URL_CURRENTGROUP = "/agentgroup/{0}/groupbyagent/{1}";

        /// <summary>
        /// whether an agnet has snatch pickup right
        /// {0}: agent id
        /// </summary>
        private const string URL_HASPICKUPRIGHT = "/agentgroup/{0}/ispickup";

        /// <summary>
        /// get record token
        /// {0}: agent id
        /// </summary>
        /// </summary>
        private const string URL_GETRECORDTOKEN = "/recordplay/{0}/getconnecttoken";

        #endregion

        #endregion

        /// <summary>
        /// AGW初始化--当前设置回调的错误次数为0
        /// </summary>
        /// <returns></returns>
        public int Initial()
        {
            AgentGatewayHelper.callBackFailedTimes = 0;
            AgentGatewayHelper.callNoRightTimes = 0;
            return 0;
        }

        /// <summary>
        /// 挂载事件
        /// </summary>
        /// <param name="huaweiDevice">回调所在的Device的实例</param>
        /// <returns></returns>
        public bool AttachEvent(IBusinessEvents businessEvents)
        {
            RelatedBusinessEvents = businessEvents;
            AgentGatewayHelper.CallBackEvent += ProcessAgentGatewayCallBack;
            AgentGatewayHelper.StartCallBackAndSetInfo(WorkNo);
            return true;
        }

        public bool DetachEvents(IBusinessEvents businessEvents)
        {
            AgentGatewayHelper.StopCallBackAndClearInfo();
            RelatedBusinessEvents = null;
            this.WorkNo = string.Empty;
            AgentGatewayHelper.CallBackEvent -= ProcessAgentGatewayCallBack;
            return true;
        }

        /// <summary>
        /// 处理AGW的回调主要是将AGW的回调跟上抛回调方法之间建立关系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data">AGW回调函数中的数据</param>
        private void ProcessAgentGatewayCallBack(object sender, AgentEventData data)
        {
            if (null == RelatedBusinessEvents || null == data || null == data.Data)
            {
                return;
            }

            data.Data = data.Data.Replace("\\u0000", ""); //有的是unicode编码然后结尾的时候会有很多\\u0000需要清楚
            //真正处理回调函数的地方
            if (data.CallBackFunctionName == "Force_Logout")
            {
                RelatedBusinessEvents.ForceOut(0);
            }
            else if (data.CallBackFunctionName == "AgentOther_PhoneAlerting")
            {
                //话机振铃
                RelatedBusinessEvents.OnPhoneAlerting();
            }
            else if (data.CallBackFunctionName == "AgentEvent_Customer_Alerting")
            {
                //话机振铃
                if (ExistHold)
                {
                    RelatedBusinessEvents.OnPhoneAlerting();
                }
            }
            else if (data.CallBackFunctionName == "AgentEvent_Ringing")
            {
                //等待应答
                string caller = string.Empty;
                string called = string.Empty;
                var jObject = AgentGatewayHelper.Parse(data.Data);
                var @event = jObject["event"];
                if (@event != null)
                {
                    var @content = @event["content"];

                    caller = @content["caller"].ToString();
                    called = @content["called"].ToString();
                }
                else
                {
                    //事件不存在
                }
                RelatedBusinessEvents.OnWaitAnswer(caller, called);
            }
            else if (data.CallBackFunctionName == "AgentEvent_Call_Out_Fail")
            {
                RelatedBusinessEvents.CallOutFailure(0);
            }
            else if (data.CallBackFunctionName == "AgentEvent_ReleaseRequest" || data.CallBackFunctionName == "AgentOther_PhoneRelease")
            {
                RelatedBusinessEvents.ReleaseRequest();
            }
            else if (data.CallBackFunctionName == "AgentEvent_Talking")
            {
                //通话中
                RelatedBusinessEvents.OnTalking(data.Data);
            }
            else if (data.CallBackFunctionName == "AgentEvent_Call_Release")
            {
                //客户退出呼叫
                RelatedBusinessEvents.OnCallRelease();
            }
            else if (data.CallBackFunctionName == "AgentEvent_No_Answer_From_Cti")
            {
                //CTI平台久不应答
                RelatedBusinessEvents.OnNoAnswerFromCti();
            }
            else if (data.CallBackFunctionName == "AgentEvent_Hold")
            {
                //呼叫保持
                RelatedBusinessEvents.OnHold();
            }
            else if (data.CallBackFunctionName == "AgentState_SetWork_Success" || data.CallBackFunctionName == "AgentState_Work")
            {
                //进入工作态
                RelatedBusinessEvents.OnSetWork();
            }
            else if (data.CallBackFunctionName == "AgentState_CancelWork_Success")
            {
                //退出工作态
                RelatedBusinessEvents.OnCancelWork();
            }
            else if (data.CallBackFunctionName == "AgentState_Ready")
            {
                //示闲
                RelatedBusinessEvents.OnSetReady();
            }
            else if (data.CallBackFunctionName == "AgentState_SetNotReady_Success")
            {
                //示忙
                RelatedBusinessEvents.OnSetNotReady();
            }
            else if (data.CallBackFunctionName == "AgentState_CancelNotReady_Success")
            {
                //示闲
                RelatedBusinessEvents.OnCancelNotReady();
            }
            else if (data.CallBackFunctionName == "AgentEvent_Conference")
            {
                //三方通话
                RelatedBusinessEvents.OnTripartiteCall();
            }
            else if (data.CallBackFunctionName == "AgentEvent_ReleaseResponse")
            {
                //释放响应事件
                var lstCall = GetDeviceCallLst();
                if (null == lstCall || lstCall.Count == 0) return;

                RelatedBusinessEvents.OnReleaseResponse();
            }
            else if (data.CallBackFunctionName == "AgentState_SetRest_Success")
            {
                //请求休息成功
                RelatedBusinessEvents.OnSetRest();
            }
            else if (data.CallBackFunctionName == "AgentState_Rest_Timeout")
            {
                //已经超过休息时间
                RelatedBusinessEvents.OnRestTimeOut();
            }
            else if (data.CallBackFunctionName == "AgentState_CancelRest_Success")
            {
                //取消休息成功
                RelatedBusinessEvents.OnCancelRest();
            }
            else if (data.CallBackFunctionName == "AgentOther_Note")
            {
                // 收到便签
                var sendDt = JsonUtil.DeJson<AGWEventDataRes<AGWEventData<AGWNotesData>>>(data.Data);
                int workNo = 0;
                Int32.TryParse(sendDt.@event.content.sender, out workNo);
                RelatedBusinessEvents.ReceiveMessageEx(workNo, sendDt.@event.content.context);
            }
            else if (data.CallBackFunctionName == "AgentOther_Bulletin")
            {
                // 收到公告
                var notifyDt = JsonUtil.DeJson<AGWEventDataRes<AGWEventData<AGWNotesData>>>(data.Data);
                int workNo = 0;
                Int32.TryParse(notifyDt.@event.content.sender, out workNo);
                RelatedBusinessEvents.ReceiveBulletinMsgEx(workNo, notifyDt.@event.content.context);
            }
            else if (data.CallBackFunctionName == "AgentEvent_Auto_Answer")
            {
                // 自动应答
                //RelatedBusinessEvents.OnTalking(data.Data);
            }
            else if (data.CallBackFunctionName == "AgentEvent_Preview")
            {
                RelatedBusinessEvents.PreviewCallOut(data.Data);
            }
            else if (data.CallBackFunctionName == "AgentEvent_AbandonCall")
            {
                RelatedBusinessEvents.AbandonCall(data.Data);
            }
            else if (data.CallBackFunctionName == "AgentOther_PhoneOffhook")
            {
                RelatedBusinessEvents.PhoneOffhook();
            }
            else if (data.CallBackFunctionName == "AgentEvent_Consult_Fail")
            {
                RelatedBusinessEvents.InternalHelpFailure(0);
            }
        }

        public string SignInEx(string MediaServer, string TelNum, bool isForceLogin)
        {
            try
            {
                var retcode = Login(WorkNo, Info, isForceLogin);
                return retcode;
            }
            catch (WebException e)
            {
                ErrorMessage = e.Message;
                var res = LoginAgain(WorkNo, Info, isForceLogin);
                return res;
            }
        }

        private string LoginAgain(string workNo, SignInParam info, bool isForceLogin)
        {
            try
            {
                var retcode = Login(WorkNo, Info, isForceLogin);
                return retcode;
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }

        private string Login(string workNo, SignInParam info, bool isForceLogin)
        {
            AgentGatewayResponse<OnlineAgentResult> result = null;

            if (isForceLogin)
            {
                result = AgentGatewayHelper.OnlineAgentForceLogin(WorkNo, Info);
            }
            else
            {
                result = AgentGatewayHelper.OnlineAgent(WorkNo, Info);
            }

            if (result == null)
            {
                WorkNo = string.Empty;
                IsForceChange = false;
                return AGWErrorCode.Empty;
            }
            if (result.retcode == AGWErrorCode.OK)
            {
                if (result.result != null)
                {
                    int vdn = 0;
                    Int32.TryParse(result.result.vdnid, out vdn);
                    this._vdnID = vdn;
                    IsForceChange = string.Compare(result.result.isForceChange, "true", true) == 0;
                }
            }
            else
            {
                WorkNo = string.Empty;
            }
            return result.retcode;
        }

        public string SignOutEx()
        {
            var result = AgentGatewayHelper.Logout(WorkNo);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            if (result.retcode == AGWErrorCode.OK)
            {
                WorkNo = string.Empty;
            }
            IsForceChange = false;
            return result.retcode;
        }

        public string CancelRest()
        {
            var result = AgentGatewayHelper.CancelRest(WorkNo);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string RestEx(int RestTime, int usCause)
        {
            var result = AgentGatewayHelper.Rest(WorkNo, RestTime, usCause);

            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string SayBusy()
        {
            var url = string.Format(AgentGatewayUri.SAYBUSY_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string SayFree()
        {
            var url = string.Format(AgentGatewayUri.CANCELBUSY_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<AGWCallInfo>>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// 外呼
        /// </summary>
        public string CallOutEx3(int MediaType, string Caller, string Called, string Pilot,
            int Mode, int SkillID, string Param, int mediaAbility, int chekcMode)
        {
            var response = AgentGatewayHelper.CallOut(WorkNo,
                new AGWCallData { called = Called, caller = Caller, skillid = SkillID, mediaability = mediaAbility });

            if (null == response)
            {
                return AGWErrorCode.Empty;
            }
            this.m_phoneAlertCallId = response.result == null ? string.Empty : response.result.ToString();
            return response.retcode;
        }

        public string InternalHelp(int MediaType, string DestWorkNo, int Mode, int DeviceType)
        {
            AGWInnerHelp data = new AGWInnerHelp();
            data.dstaddress = DestWorkNo;
            data.devicetype = DeviceType;
            data.mode = Mode;
            var result = AgentGatewayHelper.InnerHelp(WorkNo, data);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string CallInnerEx(string DestWorkNo, int MediaType)
        {
            var response = AgentGatewayHelper.InnerCall(WorkNo, new AGWCallData { called = DestWorkNo.ToString() });
            this.m_phoneAlertCallId = response == null ? string.Empty : response.result;
            if (response == null)
            {
                return AGWErrorCode.Empty;
            }
            return response.retcode;
        }

        public string ConnectHoldEx(string CallID)
        {
            AgentGatewayResponse response = AgentGatewayHelper.ConnectHold(WorkNo, CallID);
            if (response == null)
            {
                return AGWErrorCode.Empty;
            }
            return response.retcode;
        }

        public string ConfJoinEx(string CallID)
        {
            AGWConfJoin data = new AGWConfJoin();
            data.callid = CallID;
            data.callappdata = string.Empty;
            AgentGatewayResponse response = AgentGatewayHelper.ConfJoin(WorkNo, data);
            if (response == null)
            {
                return AGWErrorCode.Empty;
            }
            return response.retcode;
        }

        public string GetHoldEx(string CallID)
        {
            var result = AgentGatewayHelper.GetHold(WorkNo, CallID);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string HoldEx()
        {
            var url = string.Format(AgentGatewayUri.HOLD_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// 取消静音
        /// </summary>
        /// <returns></returns>
        public string EndMuteUserEx()
        {
            var url = string.Format(AgentGatewayUri.EndMute_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// 静音
        /// </summary>
        /// <returns></returns>       
        public string BeginMuteUserEx()
        {
            var url = string.Format(AgentGatewayUri.BeginMute_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string TransOutEx(int MediaType, string Caller, string Called, int Flag, int Mode, string Pilot)
        {
            AGWTransferData data = new AGWTransferData();
            data.devicetype = 5;//外部号码
            data.address = Called;

            data.mode = Flag;
            data.caller = Caller;
            var result = AgentGatewayHelper.Transfer(WorkNo, data);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string TransInnerEx(int MediaType, int TransType, string Destination, int DeviceType)
        {
            AGWTransferData data = new AGWTransferData();
            data.devicetype = DeviceType;
            data.address = Destination;
            data.mode = TransType;
            AgentGatewayResponse result = AgentGatewayHelper.Transfer(WorkNo, data);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string AnswerEx(int MediaType)
        {
            var url = string.Format(AgentGatewayUri.ANSWER_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string SetWork()
        {
            var url = string.Format(AgentGatewayUri.WORK_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string AgentEnterIdle()
        {
            var url = string.Format(AgentGatewayUri.CANCELWORK_URI, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string ReleaseCall(int MediaType)
        {
            return AgentGatewayHelper.Release(WorkNo);
        }

        public TAgentTypeForOcx GetAgentType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 技能重设（默认签入所有技能）
        /// </summary>
        /// <param name="sWorkNo"></param>
        /// <returns></returns>
        public string ResetSkillEx(string sWorkNo)
        {
            var result = AgentGatewayHelper.ResetSkill(sWorkNo, true, null, false);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        /// <summary>
        /// reset skills()
        /// </summary>
        /// <param name="isAutoConfig">auto reset skills</param>
        /// <param name="skills">null if is auto config else list skills id</param>
        /// <param name="isPhonelinkage">phonelinkage</param>
        /// <returns></returns>
        public string ResetSkillEx(bool isAutoConfig, List<int> skills, bool isPhonelinkage)
        {
            var result = AgentGatewayHelper.ResetSkill(WorkNo, isAutoConfig, skills, isPhonelinkage);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        /// <summary>
        /// 查询座席技能
        /// </summary>
        /// <returns></returns>
        public List<SkillInfo> GetAgentSkills()
        {
            AgentGatewayResponse<List<SkillInfo>> result = AgentGatewayHelper.AgentSkills(WorkNo);
            if (result != null && AGWErrorCode.OK.Equals(result.retcode) && result.result != null)
            {
                return result.result;
            }
            return null;
        }

        /// <summary>
        /// qry skills that have been sign in
        /// </summary>
        /// <returns></returns>
        public List<SkillInfo> GetAgentSignedSkills()
        {
            var url = string.Format(URL_SIGNSKILLS, WorkNo, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<List<SkillInfo>>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return (res == null || !AGWErrorCode.OK.Equals(res.retcode)) ? null : res.result;
        }

        public List<SkillInfo> GetVDNSkillInfo()
        {
            AgentGatewayResponse<List<AGWSkill>> result = AgentGatewayHelper.QeueDeviceAgentVDNSkills(WorkNo);
            if (null == result || null == result.result || !AGWErrorCode.OK.Equals(result.retcode))
            {
                return null;
            }

            List<SkillInfo> queriedSkillInfoList = new List<SkillInfo>();
            foreach (AGWSkill tempSkillInfo in result.result)
            {
                SkillInfo queriedSkillInfo = new SkillInfo();
                queriedSkillInfo.id = tempSkillInfo.id;
                queriedSkillInfo.name = tempSkillInfo.name;
                queriedSkillInfo.mediatype = tempSkillInfo.mediatype;
                queriedSkillInfoList.Add(queriedSkillInfo);
            }
            return queriedSkillInfoList;
        }

        public List<IvrInfo> GetVDNIvrInfo()
        {
            AgentGatewayResponse<List<AGWIVRInfo>> result = AgentGatewayHelper.QeueDeviceIVRInfo(WorkNo);
            if (null == result || null == result.result || !AGWErrorCode.OK.Equals(result.retcode))
            {
                return null;
            }

            List<IvrInfo> queriedIVRInfoList = new List<IvrInfo>();
            foreach (AGWIVRInfo tempivrinfo in result.result)
            {
                IvrInfo queriedIvrInfo = new IvrInfo();

                queriedIvrInfo.ServiceNo = tempivrinfo.serviceNo;
                queriedIvrInfo.InNo = tempivrinfo.access;
                queriedIvrInfo.Description = tempivrinfo.description;
                queriedIvrInfo.Id = tempivrinfo.id;
                queriedIVRInfoList.Add(queriedIvrInfo);
            }
            return queriedIVRInfoList;
        }

        public List<AccessCode> GetVDNAccessCodeInfo()
        {
            AgentGatewayResponse<List<AGWAccessCode>> result = AgentGatewayHelper.QueryVDNAccessCodeInfo(WorkNo);
            if (null == result || null == result.result || !AGWErrorCode.OK.Equals(result.retcode))
            {
                return null;
            }

            List<AccessCode> queriedAccessCodeList = new List<AccessCode>();
            foreach (AGWAccessCode tempAccessCodeinfo in result.result)
            {
                AccessCode queriedAccessCodeInfo = new AccessCode();

                queriedAccessCodeInfo.SystemAccessCode = tempAccessCodeinfo.inno;
                queriedAccessCodeInfo.AccessCodeDesc = tempAccessCodeinfo.description;
                queriedAccessCodeInfo.MediaType = tempAccessCodeinfo.mediatype;
                queriedAccessCodeList.Add(queriedAccessCodeInfo);
            }
            return queriedAccessCodeList;
        }

        /// <summary>
        /// 查询保持的列表
        /// </summary>
        /// <returns></returns>
        public List<HoldListData> GetHoldList()
        {
            AgentGatewayResponse<List<HoldListData>> result = AgentGatewayHelper.HoldList(WorkNo);
            ExistHold = false;
            if (result != null)
            {
                if (result.retcode.Equals(AGWErrorCode.OK) && result.result != null && result.result.Count > 0)
                {
                    ExistHold = true;
                    return result.result;
                }
            }
            return null;
        }

        public List<AgentStateInfo> GetAllAgentStatusInfo()
        {
            AgentGatewayResponse<List<AgentStateInfo>> result = AgentGatewayHelper.AllAgentStatus(WorkNo);
            if (null == result || null == result.result || !AGWErrorCode.OK.Equals(result.retcode))
            {
                return null;
            }
            return result.result;
        }

        /// <summary>
        /// 查询工作组
        /// </summary>
        /// <returns></returns>
        public List<WorkGroupData> GetWorkGroupData()
        {
            AgentGatewayResponse<List<AGWWorkGroupData>> result = AgentGatewayHelper.QueryVDNWorkGroup(WorkNo);
            if (result == null || result.result == null) return null;
            var lst = new List<WorkGroupData>();
            foreach (var item in result.result)
            {
                lst.Add(new WorkGroupData() { WorkGroupName = item.name });
            }
            return lst;
        }

        public string AgentStatus()
        {
            string status = string.Empty;
            AgentGatewayResponse<AGWAgentStatus> result = AgentGatewayHelper.AgentStatus(WorkNo);
            if (result != null)
            {
                if (result.retcode.Equals(AGWErrorCode.OK) && result.result != null)
                {
                    return result.result.agentState;
                }
                else if (!result.retcode.Equals(AGWErrorCode.OK))
                {
                    return result.retcode;
                }
            }
            return null;
        }

        /// <summary>
        /// 发布公告
        /// </summary>
        public string NotifyBulletin(int type, string name, string msg)
        {
            var data = new AGWNotifyBullet() { targettype = type, bulletindata = msg, targetname = name };
            var url = string.Format(AgentGatewayUri.NotifyBullet, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// 发送便签
        /// </summary>
        public string SendNote(int agentId, string msg)
        {
            var data = new AGWSendNote(agentId, msg);
            var url = string.Format(AgentGatewayUri.SendNote, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// 取消求助（取消转移）
        /// </summary>
        /// <returns></returns>
        public string CancelHelp()
        {
            var url = string.Format(AgentGatewayUri.CancelTrans, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, null);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        public string SetData(string id, string msg, int mediaType)
        {
            var url = string.Format(AgentGatewayUri.SetData, WorkNo, msg, id);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string QryData(string callid)
        {
            var url = string.Format(AgentGatewayUri.QryData, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<string>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return res == null ? AGWErrorCode.Empty : res.result;
        }

        /// <summary>
        /// 二次拨号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string SecondDial(string number)
        {
            var result = AgentGatewayHelper.SecondDial(WorkNo, number);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            return result.retcode;
        }

        /// <summary>
        /// 设置是否进入工作态
        /// </summary>
        /// <remarks>
        /// 调用的 AGW 函数是 "是否进入空闲态"，因此 bool 参数要转换
        /// </remarks>
        /// <returns></returns>
        public string SetDeviceAutoEnterWork(bool flag)
        {
            var result = AgentGatewayHelper.SetDeviceAutoEnterWork(WorkNo, !flag);
            if (result == null)
            {
                return AGWErrorCode.Empty;
            }
            if (result.retcode == AGWErrorCode.OK)
            {
                _autoEnterWork = flag;
            }

            return result.retcode;
        }

        #region  电话会议

        /// <summary>
        /// 申请电话会议
        /// </summary>
        /// <param name="num"></param>
        /// <param name="caller"></param>
        /// <param name="time"></param>
        /// <param name="promptTime"></param>
        /// <param name="beepTone"></param>
        /// <param name="mode"></param>
        /// <param name="voicePath"></param>
        /// <returns></returns>
        public string RequestAgentConf(int num, string caller, int time, int promptTime, bool beepTone, int mode, string voicePath)
        {
            var data = new RegAgentConfParm()
            {
                memberNum = num,
                confCallerNo = caller,
                time = time,
                beepTone = beepTone,
                playMode = mode,
                promptTime = promptTime,
                voicePath = voicePath
            };
            var url = string.Format(AgentGatewayUri.RequestAgentConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 座席应答会议
        /// </summary>
        /// <returns></returns>
        public string AnswerAgentConf()
        {
            var url = string.Format(AgentGatewayUri.AnswerAgentConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 座席拒接会议
        /// </summary>
        /// <returns></returns>
        public string RejectAgentConf()
        {
            var url = string.Format(AgentGatewayUri.RejectAgentConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.DELETE, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 退出电话会议
        /// </summary>
        /// <returns></returns>
        public string LeaveAgentConf()
        {
            var url = string.Format(AgentGatewayUri.LeaveAgentConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.DELETE, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 释放电话会议
        /// </summary>
        /// <returns></returns>
        public string ReleaseAgentConf()
        {
            var url = string.Format(AgentGatewayUri.LeaveAgentConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.DELETE, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 主席申请延长会议时间
        /// </summary>
        /// <param name="time">需要增加的时间(分钟：1～1440,默认 60)</param>
        /// <returns></returns>
        public string ProlongTimeAgentConf(int time)
        {
            var url = string.Format(AgentGatewayUri.ProlongTime, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 转移主席权限
        /// </summary>
        /// <param name="targetWorkNo">目标座席</param>
        /// <returns></returns>
        public string ApplyToShiftPresidentAgentConf(int targetWorkNo)
        {
            var url = string.Format(AgentGatewayUri.ChangePresident, WorkNo, targetWorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 电话会议放音
        /// </summary>
        /// <param name="type">放音类型（0：播放会议前景;1：播放会议背景音）</param>
        /// <param name="mode">放音模式（1：播放VP提示音;2：播放指定文件音;4：播放SIG音）</param>
        /// <param name="voicePath">提示音编码或者文件绝对路径名</param>
        /// <returns></returns>
        public string PlayVoiceAgentConf(int type, int mode, string voicePath)
        {
            var data = new PlayVoiceParm() { playType = type, playMode = mode, voicePath = voicePath };
            var url = string.Format(AgentGatewayUri.PlayVoiceConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 停止电话会议放音
        /// </summary>
        /// <returns></returns>
        public string StopVoiceAgentConf()
        {
            var url = string.Format(AgentGatewayUri.StopVoiceConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 改变会议资源方数
        /// </summary>
        /// <param name="type">变更类型(0：增加;1：减少)</param>
        /// <param name="num">申请变更的数目（2-118）</param>
        /// <returns></returns>
        public string ModifyConfResAgentConf(int type, int num)
        {
            var url = string.Format(AgentGatewayUri.StopVoiceConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        #endregion

        #region  多媒体会议

        /// <summary>
        /// 申请多媒体会议
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="num"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public string RequestMultimediaConf(string callID, int num, string info)
        {
            var data = new ReqMultiMediaConfParm() { callid = callID, addressinfo = info };
            var url = string.Format(AgentGatewayUri.ReqMultiMediaConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 邀请加入多媒体会议
        /// </summary>
        /// <param name="callID"></param>
        /// <param name="num"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public string InviteJoinMultimediaConf(int callID, int num, string info)
        {
            var data = new ReqMultiMediaConfParm() { callid = callID + "", addressinfo = info };
            var url = string.Format(AgentGatewayUri.InvMultiMediaConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.PUT, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 上报加入多媒体会议结果
        /// </summary>
        /// <param name="confID"></param>
        /// <param name="res"></param>
        /// <param name="cause"></param>
        /// <returns></returns>
        public string JoinMultimediaConfResponse(int confID, int res, int cause)
        {
            var data = new JoinMultiMediaConfResParm() { confid = confID, confresult = res, cause = cause };
            var url = string.Format(AgentGatewayUri.JoinConfRes, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 结束多媒体会议
        /// </summary>
        /// <param name="confID">会议 ID</param>
        /// <returns></returns>
        public string StopMultimediaConf(int confID)
        {
            var data = new EndMultiMediaConfParm() { confid = confID };
            var url = string.Format(AgentGatewayUri.EndMultiMediaConf, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.DELETE, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        /// <summary>
        /// 查询多媒体会议状态
        /// </summary>
        /// <param name="partNum">查询的与会人个数</param>
        /// <param name="info">查询到的多媒体会议状态信息列表</param>
        /// <returns></returns>
        public string QueryMultimediaConfState(int partNum, string info)
        {
            var data = new QryMultiMediaConfParm() { addressinfo = info };
            var url = string.Format(AgentGatewayUri.QryConfState, WorkNo);
            var result = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return result == null ? AGWErrorCode.Empty : result.retcode;
        }

        #endregion

        #region   Textchat

        /// <summary>
        /// send message without call
        /// </summary>
        /// <param name="skillId">skillID</param>
        /// <param name="chatId">chatID</param>
        /// <param name="recv">receiver</param>
        /// <param name="msg">msg content</param>
        /// <param name="ip">jauIP</param>
        /// <param name="port">jauPort</param>
        /// <returns>send result</returns>
        public string SendMsgWithoutCall(string skillId, string chatId, string recv, string msg, string ip, string port)
        {
            var url = string.Format(URL_TC_SENDMSGWITHOUTCALL, WorkNo);
            var data = new TcSmsParm() { skillId = skillId, chatId = chatId, chatReceiver = recv, content = msg, jauIP = ip, jauPort = port };
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return null == res ? string.Empty : res.retcode;
        }

        /// <summary>
        /// answer text chat
        /// </summary>
        /// <param name="callID">callID</param>
        /// <returns>answer result</returns>
        public string AnswerTextChat(string callID)
        {
            var url = string.Format(URL_TXTCHATANS, WorkNo, callID);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// send message
        /// </summary>
        /// <param name="callID">callID</param>
        /// <param name="content">message content</param>
        /// <returns>send result</returns>
        public string TextChatSendMsg(string callID, string content)
        {
            var url = string.Format(URL_TXTMSGSEND, WorkNo);
            var data = new TCSendMsgParm() { callId = callID, content = content };
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, data);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// inner textchat
        /// </summary>
        /// <param name="destNo">dest workNo</param>
        /// <returns>chat result</returns>
        public string InnerTextChat(string destNo)
        {
            var url = string.Format(URL_INTERNALTXTCHAT, WorkNo, destNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// transfer textchat
        /// </summary>
        /// <param name="addressType">address type</param>
        /// <param name="destAddr">dest address</param>
        /// <param name="callId">callID</param>
        /// <param name="content">chat content</param>
        /// <param name="mode">transfer mode</param>
        /// <returns>transfer result</returns>
        public string TransTextChat(int addressType, string destAddr, string callId, string content, int mode)
        {
            var data = new TCTransParm() { addesstype = addressType, destaddr = destAddr, callid = callId, attachdata = content, mode = mode };
            var url = string.Format(URL_TRANSTXTCHAT, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// close textchat
        /// </summary>
        /// <param name="callID">callID</param>
        /// <returns>close result</returns>
        public string CloseTextChat(string callID)
        {
            var url = string.Format(URL_CLOSETXTCHAT, WorkNo, callID);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.DELETE, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// reject textchat
        /// </summary>
        /// <param name="callID">callID</param>
        /// <returns>reject result</returns>
        public string RejectTextChat(string callID)
        {
            var url = string.Format(URL_REJECTTXTCHAT, WorkNo, callID);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.DELETE, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// file upload
        /// </summary>
        /// <param name="callID">callID</param>
        /// <param name="filePath">file path</param>
        /// <returns>upload result</returns>
        public string UploadMediaFile(string callID, string filePath)
        {
            var url = string.Format(URL_UPLOADMEDIAFILE, WorkNo, callID, "false", "1", "1", "fileupload");
            return AgentGatewayHelper.HttpUploadFile(url, filePath);
        }

        /// <summary>
        /// download file
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>download byts[] content</returns>
        public List<byte> DownloadMediaFile(string filePath)
        {
            var url = string.Format(URL_DOWNLOADMEDIAFILE, WorkNo, filePath);
            return AgentGatewayHelper.DownloadFileStream(url, AgentGatewayHelper.HttpMethod.GET, null);
        }

        #endregion

        #region 查询类方法

        /// <summary>
        /// qry current agent record token
        /// </summary>
        /// <returns>qry result</returns>
        public string QryCurrentAgentRecordToken()
        {
            var url = string.Format(URL_GETRECORDTOKEN, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<RecordTokenParm>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return (null == res || null == res.result) ? string.Empty : res.result.token;
        }

        /// <summary>
        /// qry call session
        /// </summary>
        /// <param name="data">qry data</param>
        /// <returns>qry result</returns>
        public AgentGatewayResponse<TcCallSessionRes> QryCallSession(object data)
        {
            var url = string.Format(URL_QRYCALLSESSION, WorkNo);
            return AgentGatewayHelper.CallFunction<AgentGatewayResponse<TcCallSessionRes>>(url, AgentGatewayHelper.HttpMethod.POST, data);
        }

        /// <summary>
        /// qry call content
        /// </summary>
        /// <param name="callID">callID</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page no</param>
        /// <returns>qry result</returns>
        public AgentGatewayResponse<TcCallContentRes> QryCallContent(string callID, string pageSize, string pageNo)
        {
            var url = string.Format(URL_QRYCALLCONTENT, WorkNo, callID, pageNo, pageSize);
            return AgentGatewayHelper.CallFunction<AgentGatewayResponse<TcCallContentRes>>(url, AgentGatewayHelper.HttpMethod.GET, null);
        }

        /// <summary>
        /// get current workgroup detail
        /// </summary>
        /// <returns>group detail</returns>
        public AgwWorkGroupDetail GetCurrentAgentWorkGroupDetail()
        {
            var url = string.Format(URL_CURRENTGROUP, WorkNo, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<AgwWorkGroupDetail>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return res == null ? null : res.result;
        }

        /// <summary>
        /// get agent workgroup detail
        /// </summary>
        /// <param name="workNo">target workNo</param>
        /// <returns>group detail</returns>
        public AgwWorkGroupDetail GetAgentWorkGroupDetail(string workNo)
        {
            var url = string.Format(URL_CURRENTGROUP, WorkNo, workNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<AgwWorkGroupDetail>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return res == null ? null : res.result;
        }

        public bool IsCensor()
        {
            var url = string.Format(URL_ISCENSOR, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            if (AGWErrorCode.OK.Equals(res.retcode) && null != res.result)
            {
                return string.Compare("true", res.result.ToString(), true) == 0;
            }
            return false;
        }

        /// <summary>
        /// 当前呼叫是否是内部呼叫或者内部求助
        /// </summary>
        /// <returns></returns>
        public bool IsInnerCallOrInnerHelp()
        {
            var url = string.Format(AgentGatewayUri.IsInnerCall, WorkNo, false);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<AGWCallInfo>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            if (null == res || null == res.result) return false;
            return (res.result.callfeature == INNERCALL_TYPE || res.result.callfeature == INTER_TWO_HELP || res.result.callfeature == INTER_THREE_HELP);
        }

        public bool HasAgentInGroup(int groupID, int monitorID)
        {
            var url = string.Format(AgentGatewayUri.AgentOnGroup, WorkNo, groupID);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<List<AGWAgentInfo>>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return (res != null && res.result != null && res.result.Count > 0);
        }

        /// <summary>
        /// 查询座席正在处理的呼叫列表
        /// </summary>
        /// <returns></returns>
        public List<CallInfo> GetDeviceCallLst()
        {
            var lstCallIDsRes = AgentGatewayHelper.GetCallIDs(WorkNo);
            if (lstCallIDsRes == null || null == lstCallIDsRes.result)
            {
                return null;
            }
            var lstCallInfo = new List<CallInfo>();
            foreach (var item in lstCallIDsRes.result)
            {
                var infoRes = AgentGatewayHelper.GetCallInfoByID(WorkNo, item);
                if (null == infoRes || null == infoRes.result) continue;
                lstCallInfo.Add(new CallInfo() { Called = infoRes.result.called, Caller = infoRes.result.caller, CallId = infoRes.result.callid });
            }

            return lstCallInfo;
        }

        /// <summary>
        /// 获取指定VDN技能队列信息
        /// </summary>
        /// <returns></returns>
        public List<AGWVdnSkills> QrySkillsOnAgentVDN()
        {
            var url = string.Format(AgentGatewayUri.QrySkillsOnVDN, WorkNo, _vdnID);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<List<AGWVdnSkills>>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return (res != null && res.result != null) ? res.result : null; ;
        }

        /// <summary>
        /// 查询是否自动进入工作态
        /// </summary>
        /// <returns></returns>
        public bool GetDeviceAutoEnterWork()
        {
            return _autoEnterWork;
        }

        /// <summary>
        /// 获取座席所属VDN号
        /// </summary>
        /// <returns></returns>
        public string GetVDNID()
        {
            var url = string.Format(AgentGatewayUri.QryVDNID, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<List<AGWVDNInfo>>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return (res == null || res.result == null || res.result.Count == 0) ? AGWErrorCode.Empty : res.result[0].vdnid + "";
        }

        #endregion

        #region  其他方法

        /// <summary>
        /// chekc https certification
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool DoCertificate(string url)
        {
            if (url.StartsWith("https") && !string.IsNullOrEmpty(url))
            {
                AgentGatewayHelper.InitCheckValid();
                var reqUrl = string.Format(AgentGatewayUri.IsInnerCall, WorkNo, false);
                var fullUrl = url + reqUrl;
                AgentGatewayHelper.CallFunctionForCheckCertificate<AgentGatewayResponse<AGWCallInfo>>(fullUrl, AgentGatewayHelper.HttpMethod.GET);
                return AgentGatewayHelper.IsCertificationValid();
            }
            return true;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        public string ModifyAccountPwd(string workNo, string oldPwd, string newPwd)
        {
            var url = string.Format(AgentGatewayUri.ModifyPwd, workNo);
            var data = new AGWPwd() { oldPassword = oldPwd, newPassword = newPwd };
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse>(url, AgentGatewayHelper.HttpMethod.POST, data);
            return res == null ? string.Empty : res.retcode;
        }

        /// <summary>
        /// 获取呼叫特征
        /// </summary>
        public string GetCallFeature()
        {
            var url = string.Format(AgentGatewayUri.IsInnerCall, WorkNo, false);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<AGWCallInfo>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            if (null == res || null == res.result) return AGWErrorCode.Empty;
            return res.result.callfeature + "";
        }

        #endregion

        #region  Quality Control Methods

        /// <summary>
        /// quality control force logout
        /// </summary>
        /// <param name="destNo">dest agentno</param>
        /// <returns>force result</returns>
        public string ForceLogout(string destNo)
        {
            return QC_CommonMethod(QC_Type.ForceOut, new string[] { WorkNo, destNo });
        }

        /// <summary>
        /// quality control insert
        /// </summary>
        /// <param name="destNo">dest agentno</param>
        /// <returns>insert result</returns>
        public string AddInsert(string destNo)
        {
            return QC_CommonMethod(QC_Type.Insert, new string[] { WorkNo, destNo });
        }

        public string AddSupervise(string destNo)
        {
            return QC_CommonMethod(QC_Type.Supervise, new string[] { WorkNo, destNo });
        }

        public string AddIntercept(string destNo)
        {
            return QC_CommonMethod(QC_Type.Intercept, new string[] { WorkNo, destNo });
        }

        public string ReqWhisper(string destNo)
        {
            return QC_CommonMethod(QC_Type.ReqWhisper, new string[] { WorkNo, destNo });
        }

        public string StopWhisper(string destNo)
        {
            return QC_CommonMethod(QC_Type.StopWhisper, new string[] { WorkNo, destNo });
        }

        public string SwitchWhisper(string destNo, string type)
        {
            return QC_CommonMethod(QC_Type.SwitchWhisper, new string[] { WorkNo, destNo, type });
        }

        public string StopInsertListen(string destNo)
        {
            return QC_CommonMethod(QC_Type.StopListenAndInert, new string[] { WorkNo, destNo });
        }

        private string QC_CommonMethod(QC_Type type, string[] args)
        {
            try
            {
                var url = string.Empty;
                var method = AgentGatewayHelper.HttpMethod.PUT;
                switch (type)
                {
                    case QC_Type.Insert:
                        url = URL_QC_INSERT;
                        break;
                    case QC_Type.Intercept:
                        url = URL_QC_INTERCEPT;
                        break;
                    case QC_Type.ReqWhisper:
                        url = URL_QC_REQWHISPER;
                        method = AgentGatewayHelper.HttpMethod.POST;
                        break;
                    case QC_Type.StopWhisper:
                        url = URL_QC_STOPWHISPER;
                        method = AgentGatewayHelper.HttpMethod.POST;
                        break;
                    case QC_Type.StopListenAndInert:
                        url = URL_QC_STOPINSERTLISTEN;
                        method = AgentGatewayHelper.HttpMethod.DELETE;
                        break;
                    case QC_Type.Supervise:
                        url = URL_QC_SUPERVISE;
                        break;
                    case QC_Type.SwitchWhisper:
                        url = URL_QC_WHISPERSWITCH;
                        method = AgentGatewayHelper.HttpMethod.POST;
                        break;
                    case QC_Type.ForceOut:
                        url = URL_QC_FORCEOUT;
                        method = AgentGatewayHelper.HttpMethod.POST;
                        break;
                    default:
                        return AGWErrorCode.SpecErr;
                }
                var fullUrl = string.Format(url, args);
                var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(fullUrl, method, null);
                return res == null ? AGWErrorCode.Empty : res.retcode;
            }
            catch (WebException exc)
            {
                ErrorMessage = exc.Message; 
                return AGWErrorCode.SpecErr;
            }
        }

        /// <summary>
        /// start record screen
        /// </summary>
        /// <returns>start record result</returns>
        public string StartRecordScreen()
        {
            var url = string.Format(URL_RECORDSCREENSTART, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// stop record screen
        /// </summary>
        /// <returns>stop result</returns>
        public string StopRecordScreen()
        {
            var url = string.Format(URL_RECORDSCREENSTOP, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.DELETE, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        #endregion

        #region  Voice Call Methods

        /// <summary>
        /// phone pickup (used for phonelinkage)
        /// </summary>
        /// <returns>pick up result</returns>
        public string PhonePickUp()
        {
            var url = string.Format(URL_PHONEPICKUP, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        /// <summary>
        /// preview callout
        /// </summary>
        /// <param name="controlid">controlid</param>
        /// <param name="called">called</param>
        /// <returns>call result</returns>
        public string PreviewCallOut(string controlid, string called)
        {
            var url = string.Format(URL_PREVIEW_CALLOUT, WorkNo);
            var data = JsonUtil.ToJson(new AgwPreviewCallOutParm() { called = called, callcontrolid = controlid });
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, data);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public string SnatchPickUp(string destWorkNo)
        {
            var url = string.Format(URL_SNATCHPICKUP, WorkNo, destWorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.PUT, null);
            return res == null ? AGWErrorCode.Empty : res.retcode;
        }

        public bool HasPickUpRight()
        {
            var url = string.Format(URL_HASPICKUPRIGHT, WorkNo);
            var res = AgentGatewayHelper.CallFunction<AgentGatewayResponse<object>>(url, AgentGatewayHelper.HttpMethod.GET, null);
            return res != null && res.result != null && string.Compare("true", res.result.ToString(), true) == 0;
        }

        #endregion
    }
}
