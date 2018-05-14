
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

namespace HuaweiAgentGateway
{
    public class OCXBusiness : IBusiness
    {
        #region  属性

        /// <summary>
        /// 内部呼叫
        /// </summary>
        private static int INTER_CALL = 6;

        /// <summary>
        /// 两方内部求助呼叫
        /// </summary>
        private static int INTER_TWO_HELP = 51;

        /// <summary>
        /// 三方内部求助呼叫
        /// </summary>
        private static int INTER_THREE_HELP = 52;

        private static int DEFAULT_PROGID = 48;

        private CccX.CccCtrlX device = null;

        private const int OCX_MEDIA_TYPE = 5;

        public bool ExistHold
        {
            set { }
            get
            {
                var lstHold = GetHoldList();
                return (lstHold != null && lstHold.Count > 0);
            }
        }
        private static string DEFAULT_AGENTSTATUS = "0";

        /// <summary>
        /// 会场的唯一标识（conference 相关功能用得到）
        /// </summary>
        private int _confID = 0;

        /// <summary>
        /// 工号
        /// </summary>
        public int WorkNo { set; get; }

        public bool IsInitialVoiceOCX { set; get; }

        public int ProgID { set; get; }

        public int TimeOut { set; get; }

        public string SpecError { set; get; }

        public int AgentType { set; get; }

        public int ReleaseType = 2; // 默认短通


        /// <summary>
        /// 备用地址
        /// </summary>
        public string BackCcsIP { set; get; }

        private readonly static Dictionary<int, string> m_AgentStatusTrans = new Dictionary<int, string>()
        {
            {0,"2"},
            {1,"4"},
            {2,"0"},
            {3,"0"},
            {4,"0"},
            {5,"7"},
            {6,"5"},
            {7,"3"},
            {8,"8"},
            {9,"0"},
            {10,"0"},
        };

        private IBusinessEvents businessEvent;
        /// <summary>
        /// 签入AGW的参数
        /// </summary>
        public HuaweiAgentGateway.AgentGatewayEntity.SignInParam Info { get; set; }

        public CccX.CccCtrlX Device
        {
            get
            {
                if (device == null)
                {
                    throw new NullReferenceException("Device");
                }
                return device;
            }
        }

        #endregion

        #region  构造函数，初始化函数

        public OCXBusiness()
        {
            try
            {
                if (device == null)
                    device = new CccX.CccCtrlX();
            }
            catch (COMException e)
            {
                SpecError = e.Message;
            }
        }

        public int Initial()
        {
            try
            {
                if (device == null)
                {
                    device = new CccX.CccCtrlX();
                }
                if (Info != null)
                {
                    device.MainCcsIP = Info.ip;
                    device.CcsID = 22;
                    device.WorkNo = WorkNo;
                    device.Password = Info.password;
                    device.MyID = (ProgID > 39 &&  ProgID < 51) ? ProgID : DEFAULT_PROGID;
                    device.TimeOut = TimeOut * 1000;   // 配置界面设置的超时时间单位是秒,ocx 属性是毫秒
                    device.AutoAnswer = Info.autoanswer;
                    device.AutoRelease = true;
                    device.BackCcsIP = BackCcsIP;
                    device.AutoReleasePhoneCallEx = ReleaseType;
                }
                return device.Initial();
            }
            catch (COMException exc)
            {
                SpecError = exc.Message;
                return -2;
            }
        }

        #endregion

        #region  事件绑定，取绑定

        public bool AttachEvent(IBusinessEvents businessEvent)
        {
            this.businessEvent = businessEvent;
            if (device == null)
            {
                return false;
            }

            try
            {
                device.OnReceiveAgentStateInfo += businessEvent.OnReceiveAgentStateInfo;
                // Phone Alerting
                device.OnPhoneStatusNotify += businessEvent.PhoneStatusNotify;
                // Busy/Free events
                device.OnSayBusySuccess += businessEvent.SayBusySuccess;
                device.OnSayBusyFailure += businessEvent.SayBusyFailure;
                device.OnSayFreeSuccess += businessEvent.SayFreeSuccess;
                device.OnSayFreeFailure += businessEvent.SayFreeFailure;
                device.OnStartBusy += businessEvent.StartBusy;
                device.OnForceBusy += businessEvent.ForceBusy;
                device.OnForceIdle += businessEvent.ForceIdle;
                // Answer/Release events
                device.OnLongNoAnswerEx += device_OnLongNoAnswerEx;
                device.OnHoldCallRelease += businessEvent.HoldCallRelease;
                device.OnForceRelease += businessEvent.ForceRelease;
                device.OnAnswerSuccess += this.AnswerSuccess;//For voice call
                device.OnAnswerRequestEx3 += this.AnswerRequestEx3;//for media call other than voice
                device.OnAnswerExSuccess += this.AnswerExSuccess;
                device.OnAnswerFailure += businessEvent.AnswerFailure;
                device.OnAnswerExFailure += businessEvent.AnswerExFailure;
                device.OnRequestReleaseEx += businessEvent.RequestReleaseEx;
                device.OnReleaseExSuccess += businessEvent.ReleaseExSuccess;
                device.OnReleaseExFailure += businessEvent.ReleaseExFailure;
                // Login/Logout events 
                device.OnSignInExSuccess += businessEvent.SignInExSuccess;
                device.OnSignInExFailure += businessEvent.SignInExFailure;
                device.OnSignOutExSuccess += businessEvent.SignOutExSuccess;
                device.OnSignOutExFailure += businessEvent.SignOutExFailure;
                device.OnForceOutEx += businessEvent.ForceOut;
                // Internal help events
                device.OnInternalHelpSuccess += businessEvent.InternalHelpSuccess;
                device.OnInternalHelpFailure += businessEvent.InternalHelpFailure;
                device.OnInternalHelpRefused += businessEvent.InternalHelpRefused;
                device.OnInternalHelpArrived += businessEvent.InternalHelpArrived;
                // Mute/UnMute events
                device.OnBeginMuteUserSuccess += businessEvent.BeginMuteUserSuccess;
                device.OnBeginMuteUserFailure += businessEvent.BeginMuteUserFailure;
                device.OnEndMuteUserSuccess += businessEvent.EndMuteUserSuccess;
                device.OnEndMuteUserFailure += businessEvent.EndMuteUserFailure;
                // Hold/Unhold events
                device.OnGetHoldSuccTalk += businessEvent.GetHoldSuccTalk;
                device.OnHoldSuccess += businessEvent.HoldSuccess;
                device.OnHoldFailure += businessEvent.HoldFailure;
                device.OnGetHoldSuccess += businessEvent.GetHoldSuccess;
                device.OnGetHoldFailure += businessEvent.GetHoldFailure;
                // Rest or Break
                device.OnStartRest += businessEvent.StartRest;
                device.OnRestTimeOut += this.RestTimeOut;
                device.OnRestExSuccess += businessEvent.RestExSuccess;
                device.OnRestExFailure += businessEvent.RestExFailure;
                device.OnCancelRestSuccess += businessEvent.OnCancelRest;
                device.OnCancelRestFailure += businessEvent.CancelRestFailure;
                // InnerCall
                device.OnCallInnerFailure += businessEvent.CallInnerFailure;
                device.OnCallInnerSuccess += businessEvent.CallInnerSuccess;
                device.OnCallInnerSuccTalk += businessEvent.CallInnerSuccTalk;
                // CallOut
                device.OnCallOutFailure += businessEvent.CallOutFailure;
                device.OnCallOutSuccess += businessEvent.CallOutSuccess;
                device.OnCallOutSuccTalkEx += businessEvent.CallOutSuccTalkEx;
                // Internal Help
                device.OnInternalHelpSuccess += businessEvent.InternalHelpSuccess;
                device.OnInternalHelpFailure += businessEvent.InternalHelpFailure;
                // NotifyBullet
                device.OnReceiveBulletinMsgEx += businessEvent.ReceiveBulletinMsgEx;
                // Send Message
                device.OnSendMessageFailure += businessEvent.SendMessageFailure;
                device.OnSendMessageSuccess += businessEvent.SendMessageSuccess;
                device.OnReceiveMessageEx += businessEvent.ReceiveMessageEx;
                // Transfer Call
                device.OnTransInnerFailure += businessEvent.TransInnerFailure;
                device.OnTransInnerSuccess += businessEvent.TransInnerSuccess;
                device.OnRedirectToOtherFailure += businessEvent.RedirectToOtherFailure;
                device.OnRedirectToOtherSuccess += businessEvent.RedirectToOtherSuccess;
                device.OnRedirectToAutoSuccess += businessEvent.RedirectToAutoSuccess;
                device.OnRedirectToAutoFailure += businessEvent.RedirectToAutoFailure;
                // Connect Hold
                device.OnIsTalkingChanged += businessEvent.IsTalkingChanged;
                device.OnConnectHoldSuccess += businessEvent.ConnectHoldSuccess;
                device.OnConnectHoldFailure += businessEvent.ConnectHoldFailure;
                // ConfJoin
                device.OnConfJoinSuccess += businessEvent.ConfJoinSuccess;
                device.OnConfJoinSuccTalk += businessEvent.ConfJoinSuccTalk;
                device.OnConfJoinFailure += businessEvent.ConfJoinFailure;
                device.OnCallerCalledInfoArrived += businessEvent.CallerCalledInfoArrived;
                device.OnDelCallInConf += businessEvent.DelCallInConf;
                // TransOut
                device.OnTransOutSuccess += businessEvent.TransOutSuccess;
                device.OnTransOutFailure += businessEvent.TransOutFailure;
                device.OnTransOutFailTalk += businessEvent.TransOutFailTalk;
                device.OnTransOutSuccTalk += businessEvent.TransOutSuccTalk;
                //  电话会议
                device.OnRequestAgentConfFailed += this.RequestConfFail;
                device.OnRequestAgentConfSuccess += this.RequestConfSucc;

                return true;
            }
            catch
            {
                return false;
            }
        }

        void device_OnLongNoAnswerEx()
        {
            businessEvent.OnNoAnswerFromCti();
        }

        public bool DetachEvents(IBusinessEvents businessEvent)
        {
            if (device == null)
            {
                return false;
            }
            try
            {
                device.OnReceiveAgentStateInfo -= businessEvent.OnReceiveAgentStateInfo;
                // Phone Alerting
                device.OnPhoneStatusNotify -= businessEvent.PhoneStatusNotify;
                // Busy/Free events
                device.OnSayBusySuccess -= businessEvent.SayBusySuccess;
                device.OnSayBusyFailure -= businessEvent.SayBusyFailure;
                device.OnSayFreeSuccess -= businessEvent.SayFreeSuccess;
                device.OnSayFreeFailure -= businessEvent.SayFreeFailure;
                device.OnStartBusy -= businessEvent.StartBusy;
                device.OnForceBusy -= businessEvent.ForceBusy;
                device.OnForceIdle -= businessEvent.ForceIdle;
                // Answer/Release events
                device.OnLongNoAnswerEx -= device_OnLongNoAnswerEx;
                device.OnHoldCallRelease -= businessEvent.HoldCallRelease;
                device.OnForceRelease -= businessEvent.ForceRelease;
                device.OnAnswerSuccess -= this.OnTalking;//For voice call
                device.OnAnswerRequestEx3 -= this.AnswerRequestEx3;//for media call other than voice
                device.OnAnswerExSuccess -= this.AnswerExSuccess;
                device.OnAnswerFailure -= businessEvent.AnswerFailure;
                device.OnAnswerExFailure -= businessEvent.AnswerExFailure;
                device.OnRequestReleaseEx -= businessEvent.RequestReleaseEx;
                device.OnReleaseExSuccess -= businessEvent.ReleaseExSuccess;
                device.OnReleaseExFailure -= businessEvent.ReleaseExFailure;
                // Login/Logout events 
                device.OnSignInExSuccess -= businessEvent.SignInExSuccess;
                device.OnSignInExFailure -= businessEvent.SignInExFailure;
                device.OnSignOutExSuccess -= businessEvent.SignOutExSuccess;
                device.OnSignOutExFailure -= businessEvent.SignOutExFailure;
                device.OnForceOutEx -= businessEvent.ForceOut;
                // Internal help events
                device.OnInternalHelpSuccess -= businessEvent.InternalHelpSuccess;
                device.OnInternalHelpFailure -= businessEvent.InternalHelpFailure;
                device.OnInternalHelpRefused -= businessEvent.InternalHelpRefused;
                device.OnInternalHelpArrived -= businessEvent.InternalHelpArrived;
                // Mute/UnMute events
                device.OnBeginMuteUserSuccess -= businessEvent.BeginMuteUserSuccess;
                device.OnBeginMuteUserFailure -= businessEvent.BeginMuteUserFailure;
                device.OnEndMuteUserSuccess -= businessEvent.EndMuteUserSuccess;
                device.OnEndMuteUserFailure -= businessEvent.EndMuteUserFailure;
                // Hold/Unhold events
                device.OnGetHoldSuccTalk -= businessEvent.GetHoldSuccTalk;
                device.OnHoldSuccess -= businessEvent.HoldSuccess;
                device.OnHoldFailure -= businessEvent.HoldFailure;
                device.OnGetHoldSuccess -= businessEvent.GetHoldSuccess;
                device.OnGetHoldFailure -= businessEvent.GetHoldFailure;
                // Rest or Break
                device.OnStartRest -= businessEvent.StartRest;
                device.OnRestTimeOut -= this.RestTimeOut;
                device.OnRestExSuccess -= businessEvent.RestExSuccess;
                device.OnRestExFailure -= businessEvent.RestExFailure;
                device.OnCancelRestSuccess -= businessEvent.OnCancelRest;
                device.OnCancelRestFailure -= businessEvent.CancelRestFailure;
                // InnerCall
                device.OnCallInnerFailure -= businessEvent.CallInnerFailure;
                device.OnCallInnerSuccess -= businessEvent.CallInnerSuccess;
                device.OnCallInnerSuccTalk -= businessEvent.CallInnerSuccTalk;
                // CallOut
                device.OnCallOutFailure -= businessEvent.CallOutFailure;
                device.OnCallOutSuccess -= businessEvent.CallOutSuccess;
                device.OnCallOutSuccTalkEx -= businessEvent.CallOutSuccTalkEx;
                // Internal Help
                device.OnInternalHelpSuccess -= businessEvent.InternalHelpSuccess;
                device.OnInternalHelpFailure -= businessEvent.InternalHelpFailure;
                // NotifyBullet
                device.OnReceiveBulletinMsgEx -= businessEvent.ReceiveBulletinMsgEx;
                // Send Message
                device.OnSendMessageFailure -= businessEvent.SendMessageFailure;
                device.OnSendMessageSuccess -= businessEvent.SendMessageSuccess;
                device.OnReceiveMessageEx -= businessEvent.ReceiveMessageEx;
                // Transfer Call
                device.OnTransInnerFailure -= businessEvent.TransInnerFailure;
                device.OnTransInnerSuccess -= businessEvent.TransInnerSuccess;
                device.OnRedirectToOtherFailure -= businessEvent.RedirectToOtherFailure;
                device.OnRedirectToOtherSuccess -= businessEvent.RedirectToOtherSuccess;
                device.OnRedirectToAutoSuccess -= businessEvent.RedirectToAutoSuccess;
                device.OnRedirectToAutoFailure -= businessEvent.RedirectToAutoFailure;
                // Connect Hold
                device.OnIsTalkingChanged -= businessEvent.IsTalkingChanged;
                device.OnConnectHoldSuccess -= businessEvent.ConnectHoldSuccess;
                device.OnConnectHoldFailure -= businessEvent.ConnectHoldFailure;
                // ConfJoin
                device.OnConfJoinSuccess -= businessEvent.OnTripartiteCall;
                device.OnConfJoinSuccTalk -= businessEvent.ConfJoinSuccTalk;
                device.OnConfJoinFailure -= businessEvent.ConfJoinFailure;
                device.OnCallerCalledInfoArrived -= businessEvent.CallerCalledInfoArrived;
                device.OnDelCallInConf -= businessEvent.DelCallInConf;
                // TransOut
                device.OnTransOutSuccess -= businessEvent.TransOutSuccess;
                device.OnTransOutFailure -= businessEvent.TransOutFailure;
                device.OnTransOutFailTalk -= businessEvent.TransOutFailTalk;
                device.OnTransOutSuccTalk -= businessEvent.TransOutSuccTalk;

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region  本地方法（屏蔽事件多余参数，调用 businessEvent 中统一的方法）

        private void RequestConfFail(int errCode)
        {
            var errMsg = device.GetPromptByErrorCode(errCode);
            businessEvent.RequestAgentConfFail();
        }

        private void RequestConfSucc(int confID)
        {
            this._confID = confID;
            businessEvent.RequestAgentConfSucc();
        }

        private void OnTalking()
        {
            var data = GetCallInfo();
            businessEvent.OnTalking(data);
        }

        private void AnswerSuccess()
        {
            var data = GetCallInfo();
            businessEvent.OnTalking(data);
        }

        private void AnswerExSuccess(int mediaType)
        {
            var data = GetCallInfo();
            businessEvent.OnTalking(data);
        }

        private void AnswerRequestEx3(int ulTime, int usDsn, int ucHandle, int ucServer, int MediaType, int CCBIdx, int CallerAddresType, int CalledAddressType, string CallerAddress, string Calledaddress)
        {
            businessEvent.OnWaitAnswer(CallerAddress, Calledaddress);
        }

        private void RestTimeOut(int time)
        {
            businessEvent.OnRestTimeOut();
        }

        #endregion

        #region  内部方法

        /// <summary>
        /// 获取主被叫信息
        /// </summary>
        /// <returns></returns>
        public string GetCallInfo()
        {
            var data = string.Empty;
            if (Device.GetCallSuccess)
            {
                var objData = new Dictionary<string, CallContent>() 
                { 
                    {"event", new CallContent() { content = new content() {
                        caller = Device.GetCallerNo(),
                        called = Device.GetCalledNo() }}}
                };
                data = Newtonsoft.Json.JsonConvert.SerializeObject(objData);
            }

            return data;
        }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        /// <returns></returns>
        public string GetErrMsg(string code)
        {
            if (device == null)
            {
                return "OCX Not Initial";
            }
            int errCode = -1;
            if (Int32.TryParse(code, out errCode) && errCode != -1)
            {
                return device.GetPromptByErrorCode(errCode);
            }
            return "Undefined Error:" + code;
        }

        #endregion

        #region  对外方法

        public string ModifyAccountPwd(string workNo, string oldPwd, string newPwd)
        {
            if (null == device) return AGWErrorCode.Empty;
            var msg = string.Empty;
            return device.ModifyAgentPwd(oldPwd, newPwd) + "";
        }

        public bool IsInnerCallOrInnerHelp()
        {
            if (null == device || !device.GetCallSuccess) return false;
            return (device.CallFeature == INTER_CALL || device.CallFeature == INTER_TWO_HELP || device.CallFeature == INTER_THREE_HELP);
        }

        /// <summary>
        /// 查询班组内是否有座席
        /// </summary>
        public bool HasAgentInGroup(int groupID, int monitorID)
        {
            if (device == null) return false;
            if (device.QueryAgentsByWorkGroup(monitorID) != 0) return false;
            return device.AgentNum > 0;
        }

        /// <summary>
        /// 获取座席所属VDN号
        /// </summary>
        /// <returns></returns>
        public string GetVDNID()
        {
            if (Device == null) return string.Empty;
            return Device.VDNID + "";
        }

        /// <summary>
        /// 获取当前设备自动进入工作态属性
        /// </summary>
        /// <returns></returns>
        public bool GetDeviceAutoEnterWork()
        {
            if (Device == null) return false;
            return Device.FreeStatus == CccX.TxFreeStatus.InWorking;
        }

        /// <summary>
        /// 设置座席在释放来话等情况下是否自动进入空闲态（Idle）或工作态（Work）
        /// FreeStatus：自动进入空闲状态（InIdle）还是自动进入工作状态（InWorking）
        /// </summary>
        /// <param name="autoIdle"></param>
        /// <returns></returns>
        public string SetDeviceAutoEnterWork(bool autoIdle)
        {
            if (Device == null) return "-1";
            var res = Device.SetAgentAutoEnterIdle(!autoIdle) + "";
            var pro = Device.FreeStatus;
            return res;
        }

        /// <summary>
        /// 二次拨号
        /// </summary>
        /// <param name="phoneNo"></param>
        /// <returns></returns>
        public string SecondDial(string phoneNo)
        {
            if (Device == null) return "-1";
            return Device.AgentSendDTMF(phoneNo) + "";
        }

        /// <summary>
        /// 座席是否正在处理呼叫
        /// </summary>
        /// <returns></returns>
        public bool IsDeviceTalking()
        {
            if (Device == null) return false;
            return device.IsTalking;
        }

        /// <summary>
        /// 设备是否静音
        /// </summary>
        /// <returns></returns>
        public bool IsDeviceMute()
        {
            if (Device == null) return false;
            return Device.IsMute;
        }

        /// <summary>
        /// 设置自动应答
        /// </summary>
        /// <param name="isAutoAnswer"></param>
        public void SetAutoAnswer(bool isAutoAnswer)
        {
            if (Device == null) return;
            Device.AutoAnswer = isAutoAnswer;
        }

        /// <summary>
        /// 获取当前座席技能列表
        /// </summary>
        /// <returns></returns>
        public List<SkillInfo> GetAgentSkills()
        {
            if (Device == null) return null;
            var lstSkill = new List<SkillInfo>();
            int qryRes = Device.QueryAgentSkillsEx(Device.WorkNo);
            if (qryRes != 0) return null;
            int skillnum = Device.AgentSkillExNum;

            for (int i = 0; i < skillnum; i++)
            {
                if (Device.GetAgentSkillExByIdx(i) != 0) return null;
                if (!Device.AgentSkillEx_IsConfiged) continue;
                if (Device.QueryAcdSkillEx(Device.AgentSkillEx_SkillID) != 0) return null;
                lstSkill.Add(new SkillInfo() { id = Device.AgentSkillEx_SkillID + "", name = Device.AcdSkillDesc });
            }

            return lstSkill;
        }

        /// <summary>
        /// 获取 VDN 技能列表（对 mediatype 过滤，当前只取 mediatype = 5 的技能）
        /// </summary>
        /// <returns></returns>
        public List<SkillInfo> GetVDNSkillInfo()
        {
            if (Device == null) return null;
            var lstSkill = new List<SkillInfo>();
            int res = device.QueryAcdID();
            if (res != 0) return null;
            if (device.AcdNumber == 0) return null;
            int num = device.AcdNumber;

            for (int i = 0; i < num; i++)
            {
                if (device.GetAcdIDByIdx(i) != 0) return null;
                if (device.QueryAcdSkillEx(device.AcdID) != 0) return null;
                if (device.QuerySkillInfoBySkillIdEx(device.AcdID) != 0) return null;
                if (device.SkillInfoEx_MediaType != OCX_MEDIA_TYPE) continue;
                lstSkill.Add(new SkillInfo() { id = device.AcdID + "", name = device.AcdSkillDesc, mediatype = device.SkillInfoEx_MediaType + "" });
            }

            return lstSkill;
        }

        /// <summary>
        /// 查询 IVR 信息
        /// </summary>
        /// <returns></returns>
        public List<IvrInfo> GetVDNIvrInfo()
        {
            if (Device == null) return null;
            var lstIvrInfo = new List<IvrInfo>();

            if (Device.QueryIvrID() == 0)
            {
                int num = Device.IvrNumber;
                for (int i = 0; i < num; i++)
                {
                    if (Device.QueryIvrDescriptionByID(i) == 0)
                    {
                        if (string.IsNullOrEmpty(Device.IvrInfo_InNo) && string.IsNullOrEmpty(Device.IvrInfo_Description)
                            && Device.IvrID == 0) continue;
                        lstIvrInfo.Add(new IvrInfo()
                        {
                            Id = Device.IvrID + "",
                            ServiceNo = Device.IvrInfo_ServiceNo + "",
                            InNo = Device.IvrInfo_InNo,
                            Description = Device.IvrInfo_Description
                        });
                    }
                }
            }

            return lstIvrInfo;
        }

        public List<AccessCode> GetVDNAccessCodeInfo()
        {
            return null;
        }

        /// <summary>
        /// 获取座席保持的电话列表
        /// </summary>
        /// <returns></returns>
        public List<HoldListData> GetHoldList()
        {
            if (Device == null) return null;
            if (Device.QueryHoldListEx() != 0) return null;
            var lstHold = new List<HoldListData>();
            int num = Device.CallIDNum;
            if (num == 0) return null;

            for (int i = 0; i < num; i++)
            {
                int callId = Device.GetCallIDByIdx(i);
                if (callId == 0 || Device.QueryCallInfoEx2(callId) != 0) break;
                lstHold.Add(new HoldListData()
                {
                    callfeature = Device.CallFeature + "",
                    callid = callId + "",
                    callskill = Device.CallSkill,
                    caller = Device.CallInfoEx2_Ani + "",
                    called = Device.CallInfoEx2_DialedNumber + "",
                    orgicallednum = Device.CallInfoEx2_OrgCalledNo + "",
                    calldata = Device.CallInfoEx2_CallData
                });
            }

            return lstHold;
        }

        /// <summary>
        /// 获取座席状态        
        /// </summary>
        /// <remarks>输出的座席状态要转换成通用的状态</remarks>
        /// <returns></returns>
        public List<AgentStateInfo> GetAllAgentStatusInfo()
        {
            if (Device == null) return null;
            var lstAgent = new List<AgentStateInfo>();
            int res = Device.QueryTotalAgentDynamicInfoEx3();
            var qryAgent = Device.QueryTotalAgentInfoEx3();

            if (res == 0)
            {
                int num = Device.AgentNum;
                for (int i = 0; i < num; i++)
                {
                    int status = Device.GetDynaStateByIdx(i);
                    var workNo = Device.GetDynaWorkNoByIdx(i);
                    var qryRes = Device.QueryAgentEx(workNo);
                    var gourpId = Device.GetGroupIDByIdx(i);
                    var groupDic = GetWorkGroupData();

                    // 查找班组数据字典匹配班组名称
                    var groupName = string.Empty;
                    if (groupDic != null && groupDic.Count > 0)
                    {
                        foreach (var item in groupDic)
                        {
                            if (item.WorkGourpId == gourpId)
                            {
                                groupName = item.WorkGroupName;
                            }
                        }
                    }
                    //

                    lstAgent.Add(new AgentStateInfo()
                    {
                        workno = Device.GetDynaWorkNoByIdx(i) + "",
                        status = m_AgentStatusTrans.ContainsKey(status) ? m_AgentStatusTrans[status] : DEFAULT_AGENTSTATUS,
                        name = Device.AgentName,
                        groupName = groupName
                    });
                }
            }
            return lstAgent;
        }


        public TAgentTypeForOcx GetAgentType()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 签入
        /// </summary>
        /// <param name="MediaServer"></param>
        /// <param name="TelNum"></param>
        /// <param name="isForceLogin"></param>
        /// <returns></returns>
        public string SignInEx(string MediaServer, string TelNum, bool isForceLogin)
        {
            return Device.SignInEx(MediaServer, AgentType, TelNum) + "";
        }

        /// <summary>
        /// 签出
        /// </summary>
        /// <returns></returns>
        public string SignOutEx()
        {
            return Device.SignOutEx() + "";
        }

        /// <summary>
        /// 取消休息
        /// </summary>
        /// <returns></returns>
        public string CancelRest()
        {
            return Device.CancelRest() + "";
        }

        /// <summary>
        /// 休息
        /// </summary>
        /// <param name="RestTime"></param>
        /// <param name="usCause"></param>
        /// <returns></returns>
        public string RestEx(int RestTime, int usCause)
        {
            return Device.RestEx(RestTime, usCause) + "";
        }

        /// <summary>
        /// 示忙
        /// </summary>
        /// <returns></returns>
        public string SayBusy()
        {
            return Device.SayBusy() + "";
        }

        /// <summary>
        /// 示闲
        /// </summary>
        /// <returns></returns>
        public string SayFree()
        {
            return Device.SayFree() + "";
        }

        /// <summary>
        /// 内部帮助
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="DestWorkNo"></param>
        /// <param name="Mode"></param>
        /// <param name="DeviceType"></param>
        /// <returns></returns>
        public string InternalHelp(int MediaType, string DestWorkNo, int Mode, int DeviceType)
        {
            if (DeviceType == 1)
            {
                int workNo = 0;
                return Device.InternalHelpEx2(MediaType, Int32.Parse(DestWorkNo), Mode, ref workNo) + "";
            }
            else if (DeviceType == 2)
            {
                return Device.InternalHelpEx(MediaType, Int32.Parse(DestWorkNo), Mode) + "";
            }

            return ConstData.ICD_ERROR + "";
        }

        /// <summary>
        /// 内部呼叫
        /// </summary>
        /// <param name="DestWorkNo"></param>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        public string CallInnerEx(string DestWorkNo, int MediaType)
        {
            int workNo = 0;
            if (Int32.TryParse(DestWorkNo, out workNo))
            {
                return Device.CallInnerEx(workNo, MediaType) + "";
            }
            return ConstData.ICD_ERROR + "";
        }

        /// <summary>
        /// 保持连接
        /// </summary>
        /// <param name="CallID"></param>
        /// <returns></returns>
        public string ConnectHoldEx(string CallID)
        {
            int callId = 0;
            if (Int32.TryParse(CallID, out callId))
            {
                return Device.ConnectHoldEx(callId) + "";
            }
            return ConstData.ICD_ERROR + "";
        }

        /// <summary>
        /// 三方通话
        /// </summary>
        /// <param name="CallID"></param>
        /// <returns></returns>
        public string ConfJoinEx(string CallID)
        {
            int callId = 0;
            if (Int32.TryParse(CallID, out callId))
            {
                return Device.ConfJoinEx(callId) + "";
            }
            return ConstData.ICD_ERROR + "";
        }

        /// <summary>
        /// 取保持
        /// </summary>
        /// <param name="CallID"></param>
        /// <returns></returns>
        public string GetHoldEx(string CallID)
        {
            int callId = 0;
            if (Int32.TryParse(CallID, out callId))
            {
                return Device.GetHoldEx(callId) + "";
            }
            return ConstData.ICD_ERROR + "";
        }

        /// <summary>
        /// 保持
        /// </summary>
        /// <returns></returns>
        public string HoldEx()
        {
            return Device.HoldEx() + "";
        }

        /// <summary>
        /// 结束静音
        /// </summary>
        /// <returns></returns>
        public string EndMuteUserEx()
        {
            return Device.EndMuteUserEx() + "";
        }

        /// <summary>
        /// 静音
        /// </summary>
        /// <returns></returns>
        public string BeginMuteUserEx()
        {
            return Device.BeginMuteUserEx() + "";
        }

        /// <summary>
        /// 呼叫转移
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="TransType"></param>
        /// <param name="Destination"></param>
        /// <param name="DeviceType"></param>
        /// <returns></returns>
        public string TransInnerEx(int MediaType, int TransType, string Destination, int DeviceType)
        {
            if (DeviceType == 1)
            {
                return Device.TransToQueueEx(MediaType, TransType, Int32.Parse(Destination)) + "";
            }
            else if (DeviceType == 2)
            {
                return Device.TransToAgent(MediaType, TransType, Int32.Parse(Destination)) + "";
            }
            else if (DeviceType == 3)
            {
                return Device.TransToIVR(MediaType, TransType, Destination) + "";
            }
            else if (DeviceType == 4)
            {
                return Device.TransToAccess(MediaType, Destination, TransType) + "";
            }

            return ConstData.ICD_ERROR + "";
        }

        /// <summary>
        /// 应答
        /// </summary>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        public string AnswerEx(int MediaType)
        {
            if (Device.AutoAnswer) return "0";
            //if (!CanReleaseOnHoldStatus()) return "0";
            return Device.AnswerEx(MediaType) + "";
        }

        /// <summary>
        /// 进入工作状态
        /// </summary>
        /// <returns></returns>
        public string SetWork()
        {
            if (Device.FreeStatus == CccX.TxFreeStatus.InWorking) return "-1";
            return Device.AgentEnterWork() + "";
        }

        /// <summary>
        /// 退出工作状态
        /// </summary>
        /// <returns></returns>
        public string AgentEnterIdle()
        {
            if (null == Device) return AGWErrorCode.Empty;
            return Device.AgentEnterIdle() + "";
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        public string ReleaseCall(int MediaType)
        {
            if (Device == null) return "-1";
            if (Device.QueryCallIDOnAgentEx(Device.WorkNo) != 0) return "-1";
            if (Device.CallIDNum == 0) return "0";
            return Device.ReleaseCallEx(MediaType) + "";
        }

        /// <summary>
        /// 技能重设（默认选择所有技能）
        /// </summary>
        /// <param name="WorkNo"></param>
        /// <returns></returns>
        public string ResetSkillEx(string WorkNo)
        {
            Device.ResetSkillEx_Begin();
            var lstSkill = GetAgentSkills();
            if (lstSkill != null && lstSkill.Count > 0)
            {
                foreach (var skill in lstSkill)
                {
                    Device.ResetSkillEx_AddSkillID(Int32.Parse(skill.id));
                }
            }

            return Device.ResetSkillEx(Int32.Parse(WorkNo)) + "";
        }

        /// <summary>
        /// 技能重设
        /// </summary>
        /// <param name="WorkNo"></param>
        /// <param name="Skills"></param>
        /// <returns></returns>
        public string ResetSkillEx(string WorkNo, List<SkillInfo> Skills)
        {
            if (Skills == null || Skills.Count == 0) return "-1";
            Device.ResetSkillEx_Begin();
            foreach (var skill in Skills)
            {
                var addRes = Device.ResetSkillEx_AddSkillID(Int32.Parse(skill.id));
            }
            var resetRes = Device.ResetSkillEx(Int32.Parse(WorkNo)) + "";

            //  检测技能重设情况
            foreach (var skill in Skills)
            {
                var res = Device.ResetSkillEx_GetResult(Int32.Parse(skill.id));
            }
            //
            return resetRes;
        }

        /// <summary>
        /// 转出
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="Caller"></param>
        /// <param name="Called"></param>
        /// <param name="Flag"></param>
        /// <param name="Mode"></param>
        /// <param name="Pilot"></param>
        /// <returns></returns>
        public string TransOutEx(int MediaType, string Caller, string Called, int Flag, int Mode, string Pilot)
        {
            if (Device != null)
            {
                return Device.TransOutEx2(MediaType, Caller, Called, Flag, Mode, Pilot) + "";
            }
            return "-1";
        }

        /// <summary>
        /// 外呼
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="Caller"></param>
        /// <param name="Called"></param>
        /// <param name="Pilot"></param>
        /// <param name="Mode"></param>
        /// <param name="SkillID"></param>
        /// <param name="Param"></param>
        /// <param name="mediaAbility"></param>
        /// <param name="checkMode"></param>
        /// <returns></returns>
        public string CallOutEx3(int MediaType, string Caller, string Called, string Pilot, int Mode, int SkillID, string Param, int mediaAbility, int checkMode)
        {
            if (Device == null) return "-1";
            return Device.CallOutWithMediaAbility(MediaType, mediaAbility, Caller, Called, Pilot, Mode, SkillID, Param, checkMode) + "";
        }

        /// <summary>
        /// 获取座席状态
        /// </summary>
        /// <returns></returns>
        public string AgentStatus()
        {
            if (Device == null) return "-1";
            if (Device.QueryAgentStatusEx(WorkNo) == 0)
            {
                return Device.AgentInfoEx_CurState + "";
            }

            return "-1";
        }

        /// <summary>
        /// 发送便签
        /// MCP下发送的内容：发送者工号+英文冒号＋通知文本
        /// </summary>
        /// <param name="agentNo">目标座席工号</param>
        /// <param name="msg">要发送的消息</param>
        /// <returns></returns>
        public string SendNote(int agentNo, string msg)
        {
            if (Device == null) return "-1";
            return Device.CccSendMessageEx(agentNo, this.WorkNo + ":" + msg) + "";
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="mediaType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string SetData(string data, string msg, int mediaType = OCX_MEDIA_TYPE)
        {
            if (Device == null) return "-1";
            return Device.SetCallDataEx(mediaType, msg) + "";
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="callId"></param>
        /// <returns></returns>
        public string QryData(string callId)
        {
            if (Device == null) return "-1";
            return Device.QueryCallDataEx(OCX_MEDIA_TYPE) + "";
        }

        /// <summary>
        /// 查询座席正在处理的呼叫列表
        /// </summary>
        /// <returns></returns>
        public List<CallInfo> GetDeviceCallLst()
        {
            if (Device == null) return null;
            if (Device.QueryCallIDOnAgentEx(this.WorkNo) != 0) return null;
            int num = Device.CallIDNum;
            var lstCall = new List<CallInfo>();

            for (int i = 0; i < num; i++)
            {
                var callId = Device.GetCallIDByIdx(i);
                if (callId == 0 || Device.QueryCallInfoEx2(callId) != 0) return null;
                lstCall.Add(new CallInfo()
                {
                    Caller = Device.CallInfoEx2_Ani,
                    Called = Device.CallInfoEx2_DialedNumber,
                    CallId = callId + "",
                    MediaType = Device.GetMediaTypeDescription(Device.CallInfoEx2_MediaType),
                });
            }

            return lstCall;
        }

        /// <summary>
        /// 查询班组信息
        /// </summary>
        /// <returns></returns>
        public List<WorkGroupData> GetWorkGroupData()
        {
            if (Device == null) return null;
            if (Device.QueryTotalWorkGroup() != 0) return null;
            int num = Device.WorkGroupNum;
            var lstWorkGroup = new List<WorkGroupData>();
            for (int i = 0; i < num; i++)
            {
                if (Device.GetWorkGroupInfoByIdx(i) != 0) return null;
                lstWorkGroup.Add(new WorkGroupData() { WorkGroupName = Device.WorkGroupInfo_Name, WorkGourpId = Device.WorkGroupInfo_ID, MonitorNo = Device.WorkGroupInfo_MonitorNo });
            }

            return lstWorkGroup;
        }

        /// <summary>
        /// 发布公告
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parm"></param>
        /// <param name="msg"></param>
        /// <remarks>格式为：发送者工号＋英文冒号＋ 公告文本</remarks>
        /// <returns></returns>
        public string NotifyBulletin(int type, string parm, string msg)
        {
            if (Device == null) return "-1";
            return Device.NotifyBulletinEx(type, parm, Device.WorkNo + ":" + msg) + "";
        }

        /// <summary>
        /// 取消求助
        /// </summary>
        /// <returns></returns>
        public string CancelHelp()
        {
            var lstCall = GetDeviceCallLst();
            var lstHold = GetHoldList();
            if (lstCall == null || lstHold == null) return "-1";

            var id = 0;
            var lstCallId = lstHold.Select(item => item.callid).ToList();
            foreach (var item in lstCall)
            {
                if (lstCallId.Contains(item.CallId)) continue;
                id = Int32.Parse(item.CallId);
            }

            return Device.ReleaseCallByCallIDEx(id) + "";
        }

        #region  会议

        /// <summary>
        /// 申请电话会议
        /// </summary>
        /// <param name="num">会场能够容纳的与会人数</param>
        /// <param name="caller">会议外呼用户时显示的主叫号码</param>
        /// <param name="time">会议申请时长(分钟：4～1440,默认 60)</param>
        /// <param name="promptTime">会议结束前提示时长(分钟：3～1440,默认 3)</param>
        /// <param name="beepTone">有用户加入、退出会议时是否播放提示音</param>
        /// <param name="mode">放音模式(1：播放VP提示音;2：播放指定文件音)</param>
        /// <param name="voicePath">提示音编码或者文件绝对路径名</param>
        /// <returns></returns>
        public string RequestAgentConf(int num, string caller, int time, int promptTime, bool beepTone, int mode, string voicePath)
        {
            var msg = device.GetPromptByErrorCode(2220); ;
            if (Device == null) return "-1";
            return Device.RequestAgentConf(num, caller, time, promptTime, beepTone, mode, voicePath) + "";
        }

        /// <summary>
        /// 座席应答会议
        /// </summary>
        /// <returns></returns>
        public string AnswerAgentConf()
        {
            if (Device == null) return "-1";
            return Device.AnswerAgentConf(_confID) + "";
        }

        /// <summary>
        /// 座席拒接会议
        /// </summary>
        /// <returns></returns>
        public string RejectAgentConf()
        {
            if (Device == null) return "-1";
            return Device.RejectAgentConf(_confID) + "";
        }

        /// <summary>
        /// 退出电话会议
        /// </summary>
        /// <returns></returns>
        public string LeaveAgentConf()
        {
            if (Device == null) return "-1";
            return Device.RequestLeaveAgentConf(_confID) + "";
        }

        /// <summary>
        /// 释放电话会议
        /// </summary>
        /// <returns></returns>
        public string ReleaseAgentConf()
        {
            if (Device == null) return "-1";
            return Device.ReleaseAgentConf(_confID) + "";
        }

        /// <summary>
        /// 保持与会者（参数有疑问）
        /// </summary>
        /// <returns></returns>
        public string HoldParticipantAgentConf()
        {
            if (Device == null) return "-1";
            return Device.HoldParticipantAgentConf(_confID, 1, 1, "") + "";
        }

        /// <summary>
        /// 取保持与会者（参数有疑问）
        /// </summary>
        /// <returns></returns>
        public string UnHoldParticipantAgentConf()
        {
            if (Device == null) return "-1";
            return Device.UnHoldParticipantAgentConf(1, 1) + "";
        }

        /// <summary>
        /// 主席申请延长会议时间
        /// </summary>
        /// <param name="time">需要增加的时间(分钟：1～1440,默认 60)</param>
        /// <returns></returns>
        public string ProlongTimeAgentConf(int time)
        {
            if (Device == null) return "-1";
            int realTime = 0;
            return Device.ProlongTimeAgentConf(_confID, time, ref realTime) + "";
        }

        /// <summary>
        /// 转移主席权限
        /// </summary>
        /// <param name="workNo">目标座席</param>
        /// <returns></returns>
        public string ApplyToShiftPresidentAgentConf(int workNo)
        {
            if (Device == null) return "-1";
            return Device.ApplyToShiftPresidentAgentConf(_confID, workNo) + "";
        }

        /// <summary>
        /// 批量邀请与会者（参数有疑问）
        /// </summary>
        /// <returns></returns>
        public string BatchAddParticipantAgentConf()
        {
            if (Device == null) return "-1";
            return Device.BatchAddParticipantAgentConf(_confID, true, 1, "", 1, 1) + "";
        }

        /// <summary>
        /// 批量删除与会者（参数有疑问）
        /// </summary>
        /// <returns></returns>
        public string BatchRemoveParticipantAgentConf()
        {
            if (Device == null) return "-1";
            int aa = 0;
            return Device.BatchRemoveParticipantAgentConfEx(1, ref aa, "") + "";
        }

        /// <summary>
        /// 修改与会者属性（参数有疑问）
        /// </summary>
        /// <returns></returns>
        public string ChangeParticipantModeAgentConf()
        {
            if (Device == null) return "-1";
            return Device.ChangeParticipantModeAgentConfEx(1, "", 1) + "";
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
            if (Device == null) return "-1";
            int errCode = 0;
            return Device.PlayVoiceAgentConf(_confID, type, mode, voicePath, ref errCode) + "";
        }

        /// <summary>
        /// 停止电话会议放音
        /// </summary>
        /// <returns></returns>
        public string StopVoiceAgentConf()
        {
            if (Device == null) return "-1";
            int errCode = 0;
            return Device.StopVoiceAgentConf(_confID, ref errCode) + "";
        }

        /// <summary>
        /// 电话会议通道放音（参数存在疑问）
        /// </summary>
        /// <returns></returns>
        public string PlayVoiceToParticipantAgentConf()
        {
            if (Device == null) return "-1";
            int aa = 1;
            return Device.PlayVoiceToParticipantAgentConf(1, 1, 1, "", ref aa) + "";
        }

        /// <summary>
        /// 停止电话会议通道放音（参数存在疑问）
        /// </summary>
        /// <returns></returns>
        public string StopVoiceToParticipantAgentConf()
        {
            if (Device == null) return "-1";
            int code = 0;
            return Device.StopVoiceToParticipantAgentConf(1, 1, ref code) + "";
        }

        /// <summary>
        /// 改变会议资源方数
        /// </summary>
        /// <param name="type">变更类型(0：增加;1：减少)</param>
        /// <param name="num">申请变更的数目（2-118）</param>
        /// <returns></returns>
        public string ModifyConfResAgentConf(int type, int num)
        {
            if (Device == null) return "-1";
            return Device.ModifyConfResAgentConf(_confID, type, num) + "";
        }

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
            if (Device == null) return AGWErrorCode.Empty;
            var err = Device.GetPromptByErrorCode(65535);
            return Device.RequestMultimediaConfEx(callID, num, info) + "";
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
            if (Device == null) return AGWErrorCode.Empty;
            return Device.InviteJoinMultimediaConfEx(callID, num, info) + "";
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
            if (Device == null) return AGWErrorCode.Empty;
            return Device.JoinMultimediaConfResponseEx(confID, res, cause) + "";
        }

        /// <summary>
        /// 结束多媒体会议
        /// </summary>
        /// <param name="confID">会议 ID</param>
        /// <returns></returns>
        public string StopMultimediaConf(int confID)
        {
            if (Device == null) return AGWErrorCode.Empty;
            return Device.StopMultimediaConf(confID) + "";
        }

        /// <summary>
        /// 查询多媒体会议状态
        /// </summary>
        /// <param name="partNum">查询的与会人个数</param>
        /// <param name="info">查询到的多媒体会议状态信息列表</param>
        /// <returns></returns>
        public string QueryMultimediaConfState(int partNum, string info)
        {
            if (Device == null) return AGWErrorCode.Empty;
            return Device.QueryMultimediaConfStateEx(partNum, info) + "";
        }

        #endregion

        #endregion

        #endregion
    }

    public class CallContent
    {
        public content content { set; get; }
    }

    public class content
    {
        public string caller { set; get; }
        public string called { set; get; }
    }
}
