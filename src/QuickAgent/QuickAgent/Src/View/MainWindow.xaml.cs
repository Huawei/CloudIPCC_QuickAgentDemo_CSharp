#define COMMERCAIL_USE

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using QuickAgent.ViewModel;
using System.Threading;
using HuaweiAgentGateway.AgentGatewayEntity;
using HuaweiAgentGateway;
using QuickAgent.Common;
using QuickAgent.Src.Common;
using QuickAgent.Src.View;
using QuickAgent.Constants;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Windows.Forms.Integration;
using HuaweiAgentGateway.Entity.VoiceEntity;
using HuaweiAgentGateway.Utils;
using HuaweiAgentGateway.Entity.AGWEntity;
using System.Windows.Interop;
using QuickAgent.Src.UserControl;
using QuickAgent.Src.View.Others;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security;

namespace QuickAgent.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Single

        private const string OCX_REST_STATUS = "4";
        private const string OCX_STATUSERROR = "1104";
        private const string TITLE = "QuickAgent";
        private const int OCX_MEDIA_TYPE = 5;
        private const string OCX_MEDIA_SERVER = "TFF";
        private const string DEFAULT_VOICEREGMODE = "0";
        private const int MCP_RESETPWD = 548;
        private const string LOCAL_IP = "127.0.0.1";
        private AgentInfo m_agentInfo = null;
        private IBusinessEvents m_mainCallBack = null;
        private List<AgentStateInfo> m_agentStateInfoList = null;
        private List<IvrInfo> ivrInfoList = null;
        private List<SkillInfo> skillInfoList = null;
        private List<HoldListData> holdinfoList = null;
        private List<AccessCode> accessCodeList = null;
        private Dictionary<string, string> _settings = null;
        private AgentStatus m_status = AgentStatus.SignOut;
        private AgentBaseStatus m_baseStatus = AgentBaseStatus.Idle;
        private string _callID = string.Empty;
        private string m_restTime = string.Empty;
        private List<SkillInfo> m_mySkillList;

        public static string ErrorMessage { set; get; }

        /// <summary>
        /// demo version
        /// current type: C80,C10
        /// used for ui limit and voice domain set
        /// </summary>
        public DemoVersion Version { set; get; }

        /// <summary>
        /// agent type
        /// current type: pc_phone,pc_phone_video
        /// </summary>
        private AgentType _agentType { set; get; }

        public static string VoicePwd { set; get; }

        private static MainWindow _instance = null;

        private VoiceOCXBusiness _voiceOCX { set; get; }

        #region  视频呼叫属性

        private VideoWinCtrl m_videoLocalWindow { set; get; }

        private VideoWinCtrl m_videoRemoteWindow { set; get; }

        private const double DEFAULT_VIDEOWIN_HEIGHT = 300.0;

        private const double DEFAULT_VIDEOWIN_WIDTH = 300.0;

        private const double DEFAULT_VIDEOWIN_LEFT = 285.0;

        #endregion

        public bool IsOpenMessge { get; set; }//判断消息框是否存在，false为不存在
        public static MainWindow Instance()
        {
            if (null == _instance)
            {
                _instance = new MainWindow();
            }
            return _instance;
        }

        public string RestTime
        {
            get { return m_restTime; }
            set { m_restTime = value; }
        }

        /// <summary>
        /// 座席条信息
        /// </summary>
        public AgentInfo AgentInfo
        {
            get { return m_agentInfo; }
        }

        /// <summary>
        /// 座席条当前状态
        /// </summary>
        public AgentStatus Status
        {
            get { return m_status; }
            set { m_status = value; }
        }

        public AgentStatus TalkStatus { set; get; }

        public AgentBaseStatus BaseStatus
        {
            get { return m_baseStatus; }
            set { m_baseStatus = value; }
        }

        #endregion

        #region 初始化

        public MainWindow()
        {
            CheckLanguageFileAndSet();
            var hasStarted = HasQuickAgentStarted();
            if (hasStarted)
            {
                Environment.Exit(0);
            }
            if (_instance == null)
            {
                _instance = this;
            }
            InitMainWindowSize();
            InitializeComponent();
            GetCurrentVersion();
            this.DataContext = MainWindowVM.GetInstance();
            m_mainCallBack = MainWindowCallBack.Instance(this);
        }

        #endregion

        #region Method

        /// <summary>
        /// 获取同一VDN座席的状态信息
        /// </summary>
        /// <returns></returns>
        public List<AgentStateInfo> GetAgentStateInfoList()
        {
            List<AgentStateInfo> tempList;
            if (m_agentStateInfoList != null && m_agentStateInfoList.Count > 0)
            {
                tempList = new List<AgentStateInfo>();
                for (int i = 0; i < m_agentStateInfoList.Count; ++i)
                {
                    AgentStateInfo info = m_agentStateInfoList[i];
                    AgentStateInfo newInfo = new AgentStateInfo { workno = info.workno, status = info.status, name = info.name, groupName = info.groupName };
                    tempList.Add(newInfo);
                }
            }
            else
            {
                tempList = null;
            }
            return tempList;
        }

        /// <summary>
        /// 获取座席当前保持的所有呼叫信息
        /// </summary>
        /// <returns></returns>
        public List<HoldListData> GetHoldInfoList()
        {
            List<HoldListData> tempList;
            if (holdinfoList != null && holdinfoList.Count > 0)
            {
                tempList = new List<HoldListData>();
                for (int i = 0; i < holdinfoList.Count; ++i)
                {
                    HoldListData info = holdinfoList[i];
                    HoldListData newInfo = new HoldListData
                    {
                        callfeature = info.callfeature,
                        callid = info.callid,
                        caller = info.caller,
                        calldata = info.calldata,
                        begintime = info.begintime,
                        called = info.called,
                        callskill = info.callskill,
                        endtime = info.endtime,
                        orgicallednum = info.orgicallednum
                    };
                    tempList.Add(newInfo);
                }
            }
            else
            {
                tempList = null;
            }
            return tempList;
        }

        /// <summary>
        /// 获取IVR信息
        /// </summary>
        /// <returns></returns>
        public List<IvrInfo> GetIvrInfoList()
        {
            List<IvrInfo> tempList;
            if (ivrInfoList != null && ivrInfoList.Count > 0)
            {
                tempList = new List<IvrInfo>();
                for (int i = 0; i < ivrInfoList.Count; ++i)
                {
                    IvrInfo info = ivrInfoList[i];
                    IvrInfo newInfo = new IvrInfo { InNo = info.InNo, Id = info.Id, Description = info.Description, ServiceNo = info.ServiceNo };
                    tempList.Add(newInfo);
                }
            }
            else
            {
                tempList = null;
            }
            return tempList;
        }

        /// <summary>
        /// 获取技能队列信息
        /// </summary>
        /// <returns></returns>
        public List<SkillInfo> GetSkillInfoList()
        {
            List<SkillInfo> tempList;
            if (skillInfoList.Count > 0)
            {
                tempList = new List<SkillInfo>();
                for (int i = 0; i < skillInfoList.Count; ++i)
                {
                    SkillInfo info = skillInfoList[i];
                    SkillInfo newInfo = new SkillInfo { id = info.id, name = info.name, mediatype = info.mediatype };
                    tempList.Add(newInfo);
                }
            }
            else
            {
                tempList = null;
            }
            return tempList;
        }

        /// <summary>
        /// 获取系统接入码信息
        /// </summary>
        /// <returns></returns>
        public List<AccessCode> GetAccessCodeList()
        {
            List<AccessCode> tempList;
            if (accessCodeList.Count > 0)
            {
                tempList = new List<AccessCode>();
                for (int i = 0; i < accessCodeList.Count; ++i)
                {
                    AccessCode info = accessCodeList[i];
                    AccessCode newInfo = new AccessCode { SystemAccessCode = info.SystemAccessCode, AccessCodeDesc = info.AccessCodeDesc, MediaType = info.MediaType };
                    tempList.Add(newInfo);
                }
            }
            else
            {
                tempList = null;
            }
            return tempList;
        }

        private void OnMainWindowLoad(object sender, RoutedEventArgs e)
        {
            LanguageResource.ChangeWindowLanguage(this);
            ChangeBtnUIByAgentStatus();
            var config = ConfigHelper.Load();
            m_agentStateInfoList = new List<AgentStateInfo>();
            if (config != null)
            {
                this.m_agentInfo = config.AgentInfo;
                this._settings = config.Settings;
            }
            AppBarFunctions.SetAppBar(this, ABEdge.Top);
        }

        private void OnMainWindowClosed(object sender, EventArgs e)
        {
            try
            {
                _instance = null;
                if (_voiceOCX != null)
                {
                    _voiceOCX.DisposeVoice();
                    _voiceOCX = null;
                }
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "MainWindowClose", exc.Message);
            }
        }

        public void Talking(string data, string caller = "", string called = "")
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM main = MainWindowVM.GetInstance();
                main.AgentState = new CommonText().StateInfo_Talking;
                main.AgentStateIcon = ImageResource.AgentStateSource_HandleCall;
                ChangeBtnUIByAgentStatus();

                if (!string.IsNullOrEmpty(caller) && !string.IsNullOrEmpty(called))
                {
                    main.CallerNumber = caller;
                    main.CalledNumber = called;
                }

                var jObject = Parse(data);
                var @event = jObject["event"];
                if (@event != null)
                {
                    var @content = @event["content"];

                    main.CallerNumber = @content["caller"].ToString();
                    main.CalledNumber = @content["called"].ToString();
                }
            }), null);
        }

        public void VoiceTalking(string info)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                m_status = AgentStatus.Talking;
                MainWindowVM main = MainWindowVM.GetInstance();
                main.AgentState = new CommonText().StateInfo_Talking;
                main.AgentStateIcon = ImageResource.AgentStateSource_HandleCall;
                ChangeBtnUIByAgentStatus();

                var ret = HuaweiAgentGateway.Utils.JsonUtil.DeJsonEx<VoiceTalkingInfo>(info);
                if (null == ret) return;
                main.CallerNumber = ret.caller;
                main.CalledNumber = ret.callee;
            }), null);
        }

        /// <summary>
        /// 一般状态（用于 mcp.ocx 保持下的释放）
        /// </summary>
        /// <param name="stateIcon"></param>
        public void IdleOnHold()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM main = MainWindowVM.GetInstance();

                main.AgentState = new CommonText().StateInfo_Idle;
                main.AgentStateIcon = ImageResource.AgentStateSource_Busy;
                main.CurAgentStateIcon = ImageResource.AgentStateSource_Idle;
                main.CalledNumber = string.Empty;
                main.CallerNumber = string.Empty;
                ChangeBtnUIByAgentStatus();
            }), null);
        }

        private void QueryVdnInfo()
        {
            //查询座席签入技能信息
            QueryAgentSkills();

            //查询VDN所有IVR信息
            QueryVDNIvrInfo();

            //查询VDN技能队列信息
            QueryVDNSkillInfo();

            //查询VDN接入码信息
            QueryVDNAccessCodeInfo();
        }

        public void PhoneRelease(string data)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM main = MainWindowVM.GetInstance();

                var jObject = AgentGatewayHelper.Parse(data);
                var @event = jObject["event"];
                if (@event != null)
                {
                    var @content = @event["content"];
                    string phoneState = @content["phoneState"].ToString();

                    if (phoneState.Equals(Constant.PhoneState_PlatformHungUp))
                    {
                        main.AgentState = new CommonText().StateInfo_Idle;
                        main.AgentStateIcon = ImageResource.AgentStateSource_Idle;
                        main.CurAgentStateIcon = ImageResource.AgentStateSource_Idle;
                        m_status = AgentStatus.Idle;
                    }
                    else if (phoneState.Equals(Constant.PhoneState_AgentHungUp))
                    {
                        main.AgentState = new CommonText().StateInfo_Busy;
                        main.AgentStateIcon = ImageResource.AgentStateSource_Idle;
                        main.CurAgentStateIcon = ImageResource.AgentStateSource_Busy;
                        m_status = AgentStatus.Busy;
                    }
                    ChangeBtnUIByAgentStatus();
                }
                else
                {
                    //事件不存在
                }
            }), null);
        }

        #endregion

        #region  枚举类型

        /// <summary>
        /// 座席状态
        /// </summary>
        [FlagsAttribute]
        public enum AgentStatus
        {
            SignIn,
            SignOut,
            Alerting,
            WaitAnswer,
            Talking,
            TalkingOnMute,
            Idle,
            Busy,
            Hold,
            Work,
            Rest,
            WaitRest,
            TripartiteCall,
            InternaCall,
            InternaHelp,
            CallOutWithHold,
        }

        [FlagsAttribute]
        public enum AgentBaseStatus
        {
            Idle,
            Busy
        }

        [FlagsAttribute]
        public enum TalkingStatus
        {
            Default,
            InnerCall,
            InnerHelp,
            CallOutWithHold
        }

        [FlagsAttribute]
        public enum DemoVersion
        {
            C10,
            C80
        }

        [FlagsAttribute]
        private enum AgentType
        {
            PC_PHONE,
            PC_PHONE_VIDEO
        }

        #endregion

        #region UIEvents

        public void SignIn()
        {
            m_status = AgentStatus.SignIn;
            ChangeOtherUIByStatus(AgentStatus.SignIn);
            QueryVdnInfo();
        }

        public void OnPhoneAlerting()
        {
            //话机振铃
            Log4NetHelper.BaseLog(Constant.PhoneAlerting);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                m_status = AgentStatus.Alerting;
                ChangeOtherUIByStatus(AgentStatus.Alerting);
            }), null);
        }

        public void OnWaitAnswer(string callerNum, string calledNum)
        {
            //等待应答
            Log4NetHelper.BaseLog(Constant.PhoneRinging);
            m_status = AgentStatus.WaitAnswer;
            ChangeOtherUIByStatus(AgentStatus.WaitAnswer, callerNum, calledNum);
        }

        public void ReleaseRequest()
        {
            var agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            AgwAgentStatus(agentStatus);
        }

        private void AgwAgentStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return;
            }
            switch (status)
            {
                case "1":
                    m_status = AgentStatus.SignIn;
                    break;
                case "2":
                    m_status = AgentStatus.SignOut;
                    break;
                case "3":
                    m_status = AgentStatus.Busy;
                    break;
                case "4":
                case "6":
                    m_status = AgentStatus.Idle;
                    break;
                case "7":
                    var holdLst = BusinessAdapter.GetBusinessInstance().GetHoldList();
                    if (null == holdinfoList || 0 == holdLst.Count)
                    {
                        m_status = AgentStatus.Talking;
                    }
                    else
                    {
                        m_status = AgentStatus.Hold;
                    }
                    break;
                case "8":
                    m_status = AgentStatus.Rest;
                    break;
                default:
                    break;
            }
            ChangeOtherUIByStatus(m_status);
        }

        public void OnTalking(string data)
        {
            Log4NetHelper.BaseLog(Constant.PhoneTalking);
            if (BusinessAdapter.GetBusinessInstance().IsInnerCallOrInnerHelp())
            {
                m_status = AgentStatus.InternaCall;
            }
            else if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
            {
                if (TalkStatus == AgentStatus.InternaHelp)
                    m_status = AgentStatus.InternaHelp;
                else if (TalkStatus == AgentStatus.InternaCall)
                    m_status = AgentStatus.InternaCall;
                else if (TalkStatus == AgentStatus.CallOutWithHold)
                    m_status = AgentStatus.CallOutWithHold;
                else
                {
                    m_status = AgentStatus.Talking;
                }
                TalkStatus = AgentStatus.Idle;
            }
            else if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
            {
                var lstTalkState = new List<AgentStatus>() { AgentStatus.Talking, AgentStatus.TalkingOnMute, AgentStatus.TripartiteCall, AgentStatus.CallOutWithHold, AgentStatus.InternaCall, AgentStatus.InternaHelp };
                if (!lstTalkState.Contains(m_status))
                    m_status = AgentStatus.Talking;
            }

            Talking(data);
            var info = JsonUtil.DeJsonEx<AGWEventDataRes<AGWEventData<AGWCallInfoParm>>>(data);
            if (null == info || null == info.@event || null == info.@event.content) return;
        }

        public void OnCallRelease()
        {
            //客户退出呼叫
            SetDefaultMuteStatus();
            Log4NetHelper.BaseLog(Constant.Release);
            QueryHoldList();
            if (holdinfoList != null && holdinfoList.Count > 0)
            {
                m_status = AgentStatus.Hold;
                ChangeOtherUIByStatus(AgentStatus.Hold);
            }
            TalkStatus = AgentStatus.Idle;
        }

        public void OnNoAnswerFromCti()
        {
            Log4NetHelper.BaseLog(Constant.PhoneNoAnswer);
            MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("mainwindow_noanswer");
        }

        public void OnSetWork()
        {
            m_status = AgentStatus.Work;
            ChangeOtherUIByStatus(AgentStatus.Work);
        }

        public void OnCancelWork()
        {
            QueryAgentStatus();
        }

        public void OnReleaseResponse()
        {
            //释放响应事件
            Log4NetHelper.BaseLog(Constant.Release);
            QueryHoldList();
            if (holdinfoList != null && holdinfoList.Count > 0)
            {
                m_status = AgentStatus.Hold;
                ChangeOtherUIByStatus(AgentStatus.Hold);
            }
        }

        public void OnCancelRest()
        {
            if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
            {
                string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
                ChangeUIByAgentState(agentStatus);
                return;
            }
            QueryAgentStatus();
        }

        #endregion

        #region  按钮功能

        /// <summary>
        /// 签入
        /// </summary>
        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var isVoiceIniSucc = false;
            var useVoice = (_settings != null && _settings.ContainsKey("UseVoice")) ? string.Compare(_settings["UseVoice"], "true", true) == 0 : false;
            if (useVoice)
                isVoiceIniSucc = VoiceInitial();
            var account = (_settings != null && _settings.ContainsKey("VoiceAccount")) ? _settings["VoiceAccount"] : string.Empty;
            if (null == m_agentInfo)
            {
                m_agentInfo = new Common.AgentInfo();
            }
            if (_settings != null)
            {
                m_agentInfo.AgentId = _settings.ContainsKey("WorkNo") ? _settings["WorkNo"] : string.Empty;
                m_agentInfo.PhoneNumber = _settings.ContainsKey("PhoneNo") ? _settings["PhoneNo"] : string.Empty;
            }
            if (_settings == null)
            {
                MessageBox.Show("please set config and save.");
                return;
            }

            var logWin = new LoginWindow(m_agentInfo, useVoice, account);
            logWin.ShowDialog();
            if (logWin.IsConfirm)
            {
                m_agentInfo.AgentId = logWin.WorkNo;
                m_agentInfo.Password = logWin.Password;
                m_agentInfo.PhoneNumber = logWin.PhoneNo;
                Login(m_agentInfo.AgentId, m_agentInfo.Password, m_agentInfo.PhoneNumber, useVoice, isVoiceIniSucc);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        private void OnExitClick(object sender, RoutedEventArgs e)
        {
            if (m_status != AgentStatus.SignOut)
            {
                string result = BusinessAdapter.GetBusinessInstance().SignOutEx();
                Log4NetHelper.ActionLog("Common", "Vc_SignOut", result);

                if (result.Equals(AGWErrorCode.OK))
                {
                    m_status = AgentStatus.SignOut;
                    ChangeOtherUIByStatus(AgentStatus.SignOut);
                }
            }
            this.Close();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 释放
        /// </summary>
        /// <remarks>
        /// ignore release error when current is mcp and error code is 1104 and current status is rest
        /// </remarks>
        private void OnReleaseClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            string result = BusinessAdapter.GetBusinessInstance().ReleaseCall(OCX_MEDIA_TYPE);
            Log4NetHelper.ActionLog("Common", "Vc_ReleaseCall", result);
            if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX && OCX_STATUSERROR == result)
            {
                var status = BusinessAdapter.GetBusinessInstance().AgentStatus();
                if (OCX_REST_STATUS == status)
                {
                    return;
                }
            }
            if (!result.Equals(AGWErrorCode.OK))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("releaseCallFailed"));
            }
        }

        /// <summary>
        /// 退出工作状态
        /// </summary>
        private void OnExitWorkClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }
            if (m_status == AgentStatus.Hold) return;
            string result = BusinessAdapter.GetBusinessInstance().AgentEnterIdle();
            Log4NetHelper.ActionLog("Common", "Vc_ExitWork", result);

            if (result != null)
            {
                if (!result.Equals(AGWErrorCode.OK))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("exitWorkFailed"));
                    return;
                }
                if (result.Equals(AGWErrorCode.OK) && BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                {
                    StartIdle();
                }
            }
        }

        /// <summary>
        /// 进入工作状态
        /// MCP 下进入工作态结果只能依据返回值进行判断
        /// </summary>
        private void OnEnterWorkClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }
            if (m_status == AgentStatus.Hold)
            {
                return;
            }
            string result = BusinessAdapter.GetBusinessInstance().SetWork();
            Log4NetHelper.ActionLog("Common", "Vc_SetWork", result);

            if (result != null)
            {
                if (!result.Equals(AGWErrorCode.OK))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("enterWorkFailed"));
                }
                if (result.Equals(AGWErrorCode.OK) && BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                {
                    OnSetWork();
                }
            }
        }

        /// <summary>
        /// 应答
        /// </summary>
        private void OnAnswerClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }
            string result = BusinessAdapter.GetBusinessInstance().AnswerEx(OCX_MEDIA_TYPE);
            Log4NetHelper.ActionLog("Common", "Vc_Answer", result);

            if (result != null && !result.Equals(AGWErrorCode.OK))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("answerFailed"));
                return;
            }
            if (null != result && AGWErrorCode.OK.Equals(result))  // if answer succeed, then disable "answer" button
            {
                ChangeBtnStatus(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true);
            }
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        private void OnSetDataClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            var lstCallInfo = BusinessAdapter.GetBusinessInstance().GetDeviceCallLst();
            var setDataWin = new SetCallDataWindow(lstCallInfo);
            setDataWin.ShowDialog();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        private void OnQueryDataClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            var lstCallInfo = BusinessAdapter.GetBusinessInstance().GetDeviceCallLst();
            var qryDataWin = new QryCallDataWindow(lstCallInfo);
            qryDataWin.ShowDialog();
        }

        /// <summary>
        /// 呼叫转移
        /// </summary>
        private void OnCallTransformClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            QueryAllAgentStatusInfo();
            var win = new CallTransferWindow();
            win.ShowDialog();
        }

        /// <summary>
        /// 转出
        /// </summary>
        private void OnTransformOutClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            var win = new TransferOutWindow();
            win.ShowDialog();
        }

        /// <summary>
        /// 静音/取消静音
        /// </summary>
        private void OnNoSoundClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            bool isMute = false;
            if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
            {
                var business = (OCXBusiness)BusinessAdapter.GetBusinessInstance();
                if (business != null)
                {
                    isMute = business.IsDeviceMute();
                    MainWindowVM.GetInstance().IsMute = isMute;
                }
            }
            else
            {
                isMute = MainWindowVM.GetInstance().IsMute;
            }

            if (isMute)
            {
                string result = BusinessAdapter.GetBusinessInstance().EndMuteUserEx();
                Log4NetHelper.ActionLog("Common", "Vc_EndMute", result);

                if (null == result)
                {
                    return;
                }
                if (!AGWErrorCode.OK.Equals(result))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("cancelMuteFailed"));
                    return;
                }
                NoSoundButton.Focusable = false;
                MainWindowVM.GetInstance().IsMute = false;
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    m_status = AgentStatus.Talking;
                    ChangeBtnUIByAgentStatus();
                }
            }
            else
            {
                string result = BusinessAdapter.GetBusinessInstance().BeginMuteUserEx();
                Log4NetHelper.ActionLog("Common", "Vc_Mute", result);

                if (null == result)
                {
                    return;
                }
                if (!AGWErrorCode.OK.Equals(result))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("muteFailed"));
                    return;
                }
                NoSoundButton.Focusable = true;
                MainWindowVM.GetInstance().IsMute = true;
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    m_status = AgentStatus.TalkingOnMute;
                    ChangeBtnUIByAgentStatus();
                }
            }
        }

        /// <summary>
        /// 保持
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHoldClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            PublicTooltip tooltip = PublicTooltip.GetInstance();
            tooltip.Owner = this;
            tooltip.LoadMessage("holdCall");
            bool? isHold = tooltip.ShowDialog();
            if (isHold != null)
            {
                if (isHold == true)
                {
                    string result = BusinessAdapter.GetBusinessInstance().HoldEx();
                    Log4NetHelper.ActionLog("Common", "Vc_Hold", result);

                    if (!result.Equals(AGWErrorCode.OK))
                    {
                        MessageBox.Show(LanguageResource.FindResourceMessageByKey("requestHoldFailed"));
                    }
                }
            }
        }

        /// <summary>
        /// 取保持(先查询是否存在保持的列表)
        /// </summary>
        private void OnUnHoldClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            if (QueryHoldList())
            {
                var holdDt = holdinfoList.Select(item =>
                    new CallInfo()
                    {
                        Called = item.called,
                        Caller = item.caller,
                        MediaType = LanguageResource.FindResourceMessageByKey("normalVoiceCall"),
                        CallId = item.callid
                    }).ToList();
                var holdWin = new GetHoldWindow(holdDt);
                holdWin.ShowDialog();
            }
            else
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("canNotGetHoldInfo"));
            }
        }

        /// <summary>
        /// 三方通话
        /// </summary>
        private void OnTripartiteCallClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            if (QueryHoldList())
            {
                HoldListWindow holdListWindow = HoldListWindow.Instance(HoldListWindow.OperateType.Tripartite);
                holdListWindow.Owner = this;
                holdListWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("canNotGetHoldInfo"));
            }
        }

        /// <summary>
        /// 连接保持
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectHoldClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            if (QueryHoldList())
            {
                HoldListWindow holdListWindow = HoldListWindow.Instance(HoldListWindow.OperateType.ConnectHold);
                holdListWindow.Owner = this;
                holdListWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("canNotGetHoldInfo"));
            }
        }

        /// <summary>
        /// 内部呼叫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInnerCallClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            try
            {
                QueryAllAgentStatusInfo();
                var innCallWin = new InnerCall();
                innCallWin.ShowDialog();
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "InnerCallClick", exc.Message);
            }
        }

        public void QueryAllAgentStatusInfo()
        {
            if (m_agentStateInfoList != null)
            {
                m_agentStateInfoList.Clear();
            }

            List<AgentStateInfo> result = BusinessAdapter.GetBusinessInstance().GetAllAgentStatusInfo();
            if (null == result || result.Count == 0)
            {
                return;
            }
            List<AgentStateInfo> tempList = result;
            for (int i = 0; i < tempList.Count; ++i)
            {
                AgentStateInfo info = tempList[i];
                if (!info.status.Equals(Constant.AgentStatus_SignOut))
                {
                    m_agentStateInfoList.Add(info);
                }
            }
        }

        /// <summary>
        /// 内部帮助
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInternalRequestHelpClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            QueryAllAgentStatusInfo();
            InnerConsultWindow.Instance().Owner = this;
            InnerConsultWindow.Instance().ShowDialog();
        }

        /// <summary>
        /// 发布公告
        /// </summary>
        private void OnAnnouncementClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            var lstWorkGroup = BusinessAdapter.GetBusinessInstance().GetWorkGroupData();
            var notifyWin = new NotifyBullet(m_mySkillList, lstWorkGroup);
            notifyWin.ShowDialog();
            if (notifyWin.DialogResult != null && notifyWin.DialogResult.Value)
            {
                var msg = notifyWin.Msg;
                var parm = notifyWin.Parm;
                var type = notifyWin.MessageType == MsgType.Skill ? 1 : 0;
                var result = BusinessAdapter.GetBusinessInstance().NotifyBulletin(type, parm, msg);
                Log4NetHelper.ActionLog("Common", "Vc_NotifyBulletin", result);

                if (!result.Equals(AGWErrorCode.OK))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("sendnote_fail"));
                }
                else
                {
                    MessageBox.Show(new CommonText().StateInfo_SendMsgSuc);
                }
            }
        }

        /// <summary>
        /// 发送便签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDispatchNoteClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            SendMsgWindow msgWindow = new SendMsgWindow();
            msgWindow.ShowDialog();
            if (msgWindow.IsConfirm)
            {
                string res = BusinessAdapter.GetBusinessInstance().SendNote(Int32.Parse(msgWindow.WorkNo), msgWindow.TxtContent);
                Log4NetHelper.ActionLog("Common", "Vc_SendNote", res);

                if (!res.Equals(AGWErrorCode.OK))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("sendnote_fail"));
                }
                else
                {
                    if (BusinessAdapter.CurrentBusinessType == HuaweiAgentGateway.BusinessType.AgentGateway)
                        MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_SendMsgSuc;
                }
            }
        }

        /// <summary>
        /// 录制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRecordClick(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 呼出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCallOutClick(object sender, RoutedEventArgs e)
        {
            if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway && ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            CallOutWindow callOutWindow = CallOutWindow.GetInstance();
            callOutWindow.Owner = this;
            bool? result = callOutWindow.ShowDialog();
            if (result != null && result.Value)
            {
                string callOutResult = BusinessAdapter.GetBusinessInstance().CallOutEx3(OCX_MEDIA_TYPE, CallOutWindow.CallerNumber, CallOutWindow.CalledNumber, "", 0, CallOutWindow.SkillID, "",
                    CallOutWindow.MediaAbility, CallOutWindow.CallCheck);
                Log4NetHelper.ActionLog("Common", "Vc_CallOut", callOutResult);

                if (!AGWErrorCode.OK.Equals(callOutResult))
                {
                    MessageBoxForErr(callOutResult);
                    return;
                }
                if (1 == CallOutWindow.MediaAbility && _voiceOCX != null)
                {
                    CheckAndShowVideoWindows(true, true, false);
                }
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    var lstHold = BusinessAdapter.GetBusinessInstance().GetHoldList();
                    TalkStatus = (lstHold != null && lstHold.Count > 0) ? AgentStatus.CallOutWithHold : AgentStatus.Talking;
                }
            }
        }

        /// <summary>
        /// 示忙/示闲
        /// </summary>
        private void OnBusyStateClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            string result = string.Empty;
            if (m_baseStatus == AgentBaseStatus.Busy)
            {
                result = BusinessAdapter.GetBusinessInstance().SayFree();
                Log4NetHelper.ActionLog("Common", "Vc_SayFree", result);

                if (result.Equals(AGWErrorCode.OK))
                {
                    Btn_Busy.Focusable = false;
                    MainWindowVM.GetInstance().CurAgentStateIcon = ImageResource.AgentStateSource_Idle;
                    m_baseStatus = AgentBaseStatus.Idle;
                    Btn_Busy.ToolTip = LanguageResource.FindResourceMessageByKey("busy");
                }
                else
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("setIdleFailed"));
                }
            }
            else if (m_baseStatus == AgentBaseStatus.Idle)
            {
                result = BusinessAdapter.GetBusinessInstance().SayBusy();
                Log4NetHelper.ActionLog("Common", "Vc_SayBusy", result);

                if (result.Equals(AGWErrorCode.OK))
                {
                    Btn_Busy.Focusable = true;
                    Btn_Busy.Focus();
                    MainWindowVM.GetInstance().CurAgentStateIcon = ImageResource.AgentStateSource_Busy;
                    m_baseStatus = AgentBaseStatus.Busy;
                    Btn_Busy.ToolTip = LanguageResource.FindResourceMessageByKey("free");
                }
                else
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("setBusyFailed"));
                }
            }
        }

        /// <summary>
        /// 休息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRestStateClick(object sender, RoutedEventArgs e)
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                SetActionDisableForTxtMsg();
                return;
            }

            if (Btn_Rest.Focusable)
            {
                string result = BusinessAdapter.GetBusinessInstance().CancelRest();
                Log4NetHelper.ActionLog("Common", "Vc_CancelRest", result);

                if (result != null)
                {
                    Btn_Busy.IsEnabled = true;
                    if (result.Equals(AGWErrorCode.OK))
                    {
                        Btn_Rest.Focusable = false;
                        Btn_Rest.ToolTip = LanguageResource.FindResourceMessageByKey("rest");
                    }
                    else
                    {
                        MessageBox.Show(LanguageResource.FindResourceMessageByKey("cancelRestFailed"));
                    }
                }
            }
            else
            {
                var win = new RestWindow();
                win.ShowDialog();
                Btn_Busy.IsEnabled = !win.IsRestSuccess;  // false 
                Btn_Rest.Focusable = win.IsRestSuccess;   // true
                if (win.IsRestSuccess)
                {
                    Btn_Rest.Focus();
                    Btn_Rest.ToolTip = LanguageResource.FindResourceMessageByKey("agentEndRest");
                }
            }
        }

        /// <summary>
        /// 配置
        /// </summary>
        private void OnConfigClick(object sender, RoutedEventArgs e)
        {
            var configWindow = new ConfigWindow(_settings, m_agentInfo);
            configWindow.ShowDialog();
            var workNo = (_settings != null && _settings.ContainsKey("WorkNo")) ? _settings["WorkNo"] : string.Empty;
            var phoneNo = (_settings != null && _settings.ContainsKey("PhoneNo")) ? _settings["PhoneNo"] : string.Empty;

            if (configWindow.DialogResult != null && configWindow.DialogResult.Value)
            {
                this._settings = configWindow.Settings;
                ConfigHelper config = new ConfigHelper();
                config.Settings = this._settings;
                this._settings.Add("WorkNo", workNo);
                this._settings.Add("PhoneNo", phoneNo);
                config.Save();
            }
        }

        /// <summary>
        /// 签出
        /// </summary>
        private void OnLogOutClick(object sender, RoutedEventArgs e)
        {
            string result = BusinessAdapter.GetBusinessInstance().SignOutEx();
            Log4NetHelper.ActionLog("Common", "Vc_SignOut", result);

            if (result.Equals(AGWErrorCode.OK))
            {
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    m_status = AgentStatus.SignOut;
                    ChangeOtherUIByStatus(AgentStatus.SignOut);
                }
                return;
            }
            MessageBoxForErr(result);
        }

        #endregion

        /// <summary>
        /// 座席登陆（初始化，签入，技能重设）
        /// </summary>
        private void Login(string workNo, string password, string phoneNumber, bool isUseVoice, bool isVoiceIniSucc)
        {
            try
            {
                #region  初始化(读取配置文件获取登陆类型 -> 加载数据 -> 初始化)

                string strBusinessType = _settings.ContainsKey("HuaweiBusinessType") ? _settings["HuaweiBusinessType"] : "Agent";
                var type = string.Compare("Agent", strBusinessType, true) == 0 ? BusinessType.AgentGateway : BusinessType.OCX;
                BusinessAdapter.CurrentBusinessType = type;
                BusinessAdapter.AddBusinessInstance(type);
                IBusiness business = BusinessAdapter.GetBusinessInstance();
                int agentType = 4; // 默认值：PC+PHONE座席
                if (_settings.ContainsKey("agentType"))
                    Int32.TryParse(_settings["agentType"], out agentType);

                var isLongCall = false;
                if (_settings.ContainsKey("LongCall"))
                {
                    isLongCall = string.Compare(_settings["LongCall"], "true", true) == 0;
                }

                SignInParam signInParam = new SignInParam
                {
                    password = password,
                    phonenum = phoneNumber,
                    status = 4,
                    agenttype = agentType,
                    releasephone = !isLongCall,
                    autoanswer = string.Compare(_settings["HuaweiIsAutoAnswer"], "true", true) == 0,
                };
                var ip1 = DomainToIP(GetSettingDictStrValue("HuaweiServerUrl"));
                var ip2 = DomainToIP(GetSettingDictStrValue("HuaweiBackupAddress"));
                if (business.GetType() == typeof(AgentGatewayBusiness))
                {
                    ((AgentGatewayBusiness)business).Info = signInParam;
                    ((AgentGatewayBusiness)business).WorkNo = workNo;
                    var port1 = GetSettingDictStrValue("tbSerPort");
                    var port2 = GetSettingDictStrValue("tbSerPort1");
                    var useSSL = string.Compare(GetSettingDictStrValue("UseSsl"), "true", true);
                    var head = useSSL == 0 ? "https://" : "http://";
                    var agw_main_url = string.Concat(head, ip1, ":", port1, "/agentgateway/resource");
                    var agw_back_url = string.Empty;
                    if (!string.IsNullOrEmpty(port2) && !string.IsNullOrEmpty(ip2))
                        agw_back_url = string.Concat(head, ip2, ":", port2, "/agentgateway/resource");
                    ((AgentGatewayBusiness)business).BaseUri = agw_main_url;
                    ((AgentGatewayBusiness)business).BackupUrl = agw_back_url;
                }
                else if (business.GetType() == typeof(OCXBusiness))
                {
                    int no = 0;
                    if (Int32.TryParse(workNo, out no))
                    {
                        ((OCXBusiness)business).WorkNo = no;
                    }
                    ((OCXBusiness)business).ReleaseType = isLongCall ? 1 : 2;
                    signInParam.ip = ip1;
                    ((OCXBusiness)business).AgentType = agentType;
                    ((OCXBusiness)business).BackCcsIP = ip2;
                    ((OCXBusiness)business).Info = signInParam;
                    ((OCXBusiness)business).ProgID = GetSettingDictIntValue("ProgID");
                    ((OCXBusiness)business).TimeOut = GetSettingDictIntValue("HuaweiOutTime");
                }

                // check https certification
                var isCheckCerti = string.Compare(GetSettingDictStrValue("ChkCerti"), "true", true) == 0;
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway && isCheckCerti)
                {
                    var mainUrl = ((AgentGatewayBusiness)business).BaseUri;
                    var backUrl = ((AgentGatewayBusiness)business).BackupUrl;

                    var chkMain = ((AgentGatewayBusiness)business).DoCertificate(mainUrl);
                    var chkBack = ((AgentGatewayBusiness)business).DoCertificate(backUrl);
                    if (!(chkMain & chkBack))
                    {
                        var select = MessageBox.Show(LanguageResource.FindResourceMessageByKey("main_other_certificaterr"), "", MessageBoxButton.YesNo);
                        if (select == MessageBoxResult.No)//如果点击“确定”按钮
                        {
                            return;
                        }
                    }
                }

                int res = business.Initial();
                Log4NetHelper.ActionLog("Common", "Vc_initial", res + "");
                if (res != 0)
                {
                    if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX && res == MCP_RESETPWD)
                    {
                        bool modifySuc = ModifyPwd(workNo);
                        return;
                    }
                    else
                    {
                        MessageBoxForErr(res + "");
                        Log4NetHelper.BaseLog("Initial Failed");
                        return;
                    }
                }
                if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                {
                    business.AttachEvent(m_mainCallBack);
                }

                #endregion

                #region  技能重设

                if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                {
                    var phoneNo = m_agentInfo == null ? string.Empty : m_agentInfo.PhoneNumber;
                    var lstSkills = BusinessAdapter.GetBusinessInstance().GetAgentSkills();
                    var skillWin = new AgentRegisterWindow(lstSkills, phoneNo);
                    skillWin.ShowDialog();
                    if (skillWin.DialogResult == null || !skillWin.DialogResult.Value)
                    {
                        Log4NetHelper.BaseLog("Cancel Reset Skills");
                        return;
                    }

                    var resetRes = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).ResetSkillEx(AgentInfo.AgentId, skillWin.lstSelectSkills);
                    Log4NetHelper.ActionLog("Common", "Vc_ResetSkills", resetRes);
                    if (!resetRes.Equals(AGWErrorCode.OK))
                    {
                        MessageBoxForErr(resetRes);
                    }
                }

                #endregion

                #region 签入

                var voiceRegRes = AGWErrorCode.OK;
                if (isUseVoice && isVoiceIniSucc)
                {
                    voiceRegRes = VoiceRegister();
                }
                string result = business.SignInEx(OCX_MEDIA_SERVER, AgentInfo.PhoneNumber, true);
                Log4NetHelper.ActionLog("Common", "Vc_SignIn", result);

                if (result.Equals(AGWErrorCode.OK) && BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    business.AttachEvent(m_mainCallBack);
                    this.SignIn();
                    if (((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).IsForceChange)
                    {
                        bool modifySuc = ModifyPwd(workNo);
                        modifySuc = true;
                        if (modifySuc)
                        {
                            BusinessAdapter.GetBusinessInstance().SignOutEx();
                            m_status = AgentStatus.SignOut;
                            ChangeOtherUIByStatus(AgentStatus.SignOut);
                            return;
                        }
                    }
                }

                if (result.Equals(AGWErrorCode.OK))
                {
                    if (isUseVoice && !string.Equals(voiceRegRes, AGWErrorCode.OK))
                    {
                        MessageBox.Show("Voice Register Failed. " + voiceRegRes);
                    }
                    if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                    {
                        bool autoAsnwer = string.Compare("true", _settings["HuaweiIsAutoAnswer"], true) == 0;
                        ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).SetAutoAnswer(autoAsnwer);
                    }

                    var config = new ConfigHelper();
                    DictUtil.AddKey<string, string>(_settings, "WorkNo", workNo);
                    DictUtil.AddKey<string, string>(_settings, "PhoneNo", phoneNumber);
                    config.Settings = this._settings;
                    config.Save();
                    var autoEnterWork = (_settings.ContainsKey("HuaweiIsAutoAnswer") && string.Compare(_settings["HuaweiAutoEnterWork"], "true", true) == 0);
                    var setRes = BusinessAdapter.GetBusinessInstance().SetDeviceAutoEnterWork(autoEnterWork);
                    Log4NetHelper.BaseLog("Auto Enter Work: " + setRes);
                    if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                    {
                        return;
                    }
                    string restSkillResult = business.ResetSkillEx(AgentInfo.AgentId);
                    Log4NetHelper.ActionLog("Common", "Vc_ResetSkills", restSkillResult);
                    if (restSkillResult.Equals("0"))
                    {
                        return;
                    }
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("resetSkillFailed"));
                    return;
                }
                MessageBoxForErr(result);
                business.DetachEvents(m_mainCallBack);

                #endregion
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                Log4NetHelper.ExcepLog("Common", "Sign", exc.Message);
            }
        }

        private void btnStart_Initialized(object sender, EventArgs e)
        {
            //设置右键菜单为空
            btnStart.ContextMenu = null;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //目标
            ctxtStartMenu.PlacementTarget = btnStart;

            //位置
            ctxtStartMenu.Placement = PlacementMode.Top;

            //显示菜单
            ctxtStartMenu.IsOpen = true;
        }

        public void ReceiveMsgOrBullet(int agentID, string msg, string type)
        {
            var msgType = string.Empty;
            if (string.Compare(Constant.ReceBullet, type, true) == 0)
            {
                msgType = LanguageResource.FindResourceMessageByKey("msgview_bullet");
            }
            else
            {
                msgType = LanguageResource.FindResourceMessageByKey("msgview_note");
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var msgInfo = new HuaweiAgentGateway.MessageInfo() { WorkNo = agentID + "", Data = msg, MsgTime = DateTime.Now + "", MsgType = msgType };
                foreach (Window win in Application.Current.Windows)
                {
                    if (string.Equals(win.Name, "MessageViewWindow", StringComparison.OrdinalIgnoreCase))
                    {
                        win.Close();
                    }
                }
                var msgWindow = new MsgViewWindow();
                msgWindow.AddMsgInfo(msgInfo);
                msgWindow.Show();
            }));
        }

        /// <summary>
        /// preview callout event
        /// </summary>
        /// <param name="info">user info</param>
        /// <remarks>
        /// 1,dejson info
        /// 2,choose whether call out or not
        /// 3,preview callout
        /// </remarks>
        public void PreviewCallOut(string info)
        {
            Log4NetHelper.BaseLog("[Event_Preview]");
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var ret = JsonUtil.DeJsonEx<TCRes<AgwEventPreview>>(info);
                if (null == ret || null == ret.@event || null == ret.@event.content)
                {
                    Log4NetHelper.BaseLog("preview dejson failed.");
                    return;
                }

                var called = ret.@event.content.dialeddigits;
                var id = ret.@event.content.controlid;
                var msg = LanguageResource.FindResourceMessageByKey("main_previewcallout") + Environment.NewLine +
                    LanguageResource.FindResourceMessageByKey("main_usernumber") + ": " + called;
                if (AutoClosedMsgBox.Show(msg, "", 10000, MsgBoxStyle.OKCancel) != AutoClosedMsgBox.MSG_OK)
                {
                    Log4NetHelper.BaseLog("[does not comfirm preview callout]");
                    return;
                }

                var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).PreviewCallOut(id, called);
                Log4NetHelper.BaseLog("[preview_callout] Rslt:" + res);
                var text = AGWErrorCode.OK.Equals(res) ? "preview callout succ" : "preview callout failed";
                MainWindowVM main = MainWindowVM.GetInstance();
                main.AgentState = text;
            }), null);
        }

        /// <summary>
        /// Abandon Call
        /// </summary>
        /// <param name="info">call info</param>
        /// <remarks>
        /// 1, show messagebox
        /// 2, call out(called , caller, skillid, mediaability;called must not be empty)
        /// </remarks>
        public void AbandonCall(string info)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var ret = JsonUtil.DeJsonEx<TCRes<AbandonCallParm>>(info);
                if (null == ret || null == ret.@event || null == ret.@event.content)
                {
                    Log4NetHelper.BaseLog("AbandonCall event dejson failed.");
                    return;
                }
                if (null == BusinessAdapter.GetBusinessInstance() || BusinessType.OCX == BusinessAdapter.CurrentBusinessType)
                {
                    Log4NetHelper.BaseLog("no agw instance.");
                    return;
                }

                var caller = ret.@event.content.caller;
                var msg = LanguageResource.FindResourceMessageByKey("main_abandoncall") + Environment.NewLine +
                    LanguageResource.FindResourceMessageByKey("main_usernumber") + ": " + caller;
                AutoClosedMsgBox.Show(msg, "", 10000, MsgBoxStyle.OK);

                var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).CallOutEx3(0, "", caller, "", 0, 0, "", 0, 0);  // need caller only,others is not needed
                Log4NetHelper.ActionLog("Agw", "Abandon_CallOut", res);
            }), null);
        }

        /// <summary>
        /// phone off hook status
        /// </summary>
        public void PhoneOffhook()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM main = MainWindowVM.GetInstance();
                if (null == main)
                {
                    return;
                }
                main.AgentState = LanguageResource.FindResourceMessageByKey("main_phoneoffhook");
            }), null);
        }

        #region  MCP OCX Event Handler

        #region Answer/Release

        public void axcontrol_OnAnswerFailure()
        {
            Log4NetHelper.BaseLog("on answer failure");
        }

        public void axcontrol_OnForceRelease()
        {
            Log4NetHelper.BaseLog(Constant.ForceRelease);

            var lstHold = BusinessAdapter.GetBusinessInstance().GetHoldList();   // 如果存在保持列表,那么状态为保持下的空闲态
            if (lstHold != null && lstHold.Count > 0)
            {
                IdleOnHold();
                return;
            }

            m_status = AgentStatus.Idle;
            ChangeOtherUIByStatus(AgentStatus.Idle);
            var autoWork = BusinessAdapter.GetBusinessInstance().GetDeviceAutoEnterWork();
            Log4NetHelper.BaseLog("Get Agent AutoEnterWork Res: " + autoWork);
            if (autoWork)  // 如果设置了自动进入工作态,那么释放通话后座席进入工作态
            {
                OnSetWork();
            }
        }

        public void axcontrol_OnAnswerExFailure(int mediaType)
        {
            Log4NetHelper.BaseLog("on answerex failure");
        }

        public void axcontrol_OnRequestReleaseEx(int mediaType)
        {
            Log4NetHelper.BaseLog(Constant.RequestRelease);
            BusinessAdapter.GetBusinessInstance().ReleaseCall(OCX_MEDIA_TYPE);
        }

        public void axcontrol_OnReleaseExSuccess(int mediaType)
        {
            Log4NetHelper.BaseLog("on releaseex success");
            SetDefaultMuteStatus();
            // 释放时，如果存在保持列表，那么左边的绿灯变红
            var lstHold = BusinessAdapter.GetBusinessInstance().GetHoldList();
            if (lstHold != null && lstHold.Count > 0)
            {
                m_status = AgentStatus.Hold;
                IdleOnHold();
                return;
            }
            //
            m_status = AgentStatus.Idle;
            ChangeOtherUIByStatus(AgentStatus.Idle);
            var autoWork = BusinessAdapter.GetBusinessInstance().GetDeviceAutoEnterWork();
            Log4NetHelper.BaseLog("Get Agent AutoEnterWork Res: " + autoWork);
            // 如果设置了 "自动进入工作态"，那么释放后自动进入工作状态
            if (autoWork)
            {
                OnSetWork();
            }
        }

        public void axcontrol_OnReleaseExFailure(int mediaType)
        {
            Log4NetHelper.BaseLog("on releaseex failure");
        }

        public void axcontrol_OnPhoneStatusNotify(int status)
        {
            var main = MainWindowVM.GetInstance();
            if (status == 0)
            {
                m_status = AgentStatus.Alerting;
                ChangeOtherUIByStatus(AgentStatus.Alerting);
            }
            else if (status == 1)
            {
                m_status = AgentStatus.WaitAnswer;
                ChangeOtherUIByStatus(AgentStatus.WaitAnswer, string.Empty, string.Empty);
            }
            else if (status == 2)
            {
                m_status = AgentStatus.Idle;
                ChangeOtherUIByStatus(AgentStatus.Idle);
            }
            else if (status == 3)
            {
                //StartIdle();
            }
        }

        #endregion

        #region Busy/Free

        public void axcontrol_OnSayBusySuccess()
        {
            Log4NetHelper.BaseLog("on saybusy success");
        }

        public void axcontrol_OnSayBusyFailure()
        {
            Log4NetHelper.BaseLog("on saybusy failure");
        }

        public void axcontrol_OnSayFreeSuccess()
        {
            Log4NetHelper.BaseLog("on sayfree success");
            MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_EndBusy;
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            if (NoSoundButton.Focusable)
            {
                m_status = AgentStatus.TalkingOnMute;
                var data = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetCallInfo();
                Talking(data);
                return;
            }
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnSayFreeFailure()
        {
            Log4NetHelper.BaseLog("on sayfree failure");
        }

        public void axcontrol_HoldCallRelease()
        {
            Log4NetHelper.BaseLog(Constant.HoldCallRelease);
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        private void ChangeUIByAgentState(string status)
        {
            switch (status)
            {
                case "1":
                    m_status = AgentStatus.Idle;
                    ChangeOtherUIByStatus(AgentStatus.Idle);
                    break;
                case "5":
                    // 保持通话下的状态
                    var lstHold = BusinessAdapter.GetBusinessInstance().GetHoldList();
                    if (lstHold != null && lstHold.Count > 0)
                    {
                        IdleOnHold();
                        return;
                    }
                    m_status = AgentStatus.Talking;
                    var data = string.Empty;
                    if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                    {
                        data = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetCallInfo();
                    }
                    Talking(data);
                    break;
                case "6":
                    m_status = AgentStatus.Work;
                    ChangeOtherUIByStatus(AgentStatus.Work);
                    break;
                case "7":
                    m_status = AgentStatus.Busy;
                    ChangeOtherUIByStatus(AgentStatus.Busy);
                    break;
                case "8":
                    m_status = AgentStatus.Rest;
                    ChangeOtherUIByStatus(AgentStatus.Rest);
                    break;
                default:
                    ChangeOtherUIByStatus(AgentStatus.Idle);
                    break;
            }
        }

        #endregion

        #region Login/Logout

        public void OnReceiveAgentStateInfo(int agentState, int BusyFlg, int RestFlg)
        {
            if (agentState == 0)
            {
                MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_Idle;
            }
        }

        public void axcontrol_OnSignInExSuccess(int mediaServer)
        {
            SignIn();
        }

        public void axcontrol_OnSignInExFailure(int mediaServer)
        {
            Log4NetHelper.BaseLog("on signin failure");
        }

        public void axcontrol_OnSignOutExSuccess(int mediaServer)
        {
            Log4NetHelper.BaseLog("on signout success");
            m_status = AgentStatus.SignOut;
            ChangeOtherUIByStatus(AgentStatus.SignOut);
        }

        public void axcontrol_OnSignOutExFailure(int mediaServer)
        {
            Log4NetHelper.BaseLog("on signout failure");
        }

        public void axcontrol_OnForceOut(int usSuccess)
        {
            Log4NetHelper.BaseLog(Constant.ForceOut);
            m_status = AgentStatus.SignOut;
            ChangeOtherUIByStatus(AgentStatus.SignOut);
            MessageBox.Show(LanguageResource.FindResourceMessageByKey("main_forcelogout"));
        }

        #endregion

        #region Mute/UnMute

        public void axcontrol_OnBeginMuteUserSuccess()
        {
            Log4NetHelper.BaseLog("OCX_BeginMuteUserSucc");
            m_status = AgentStatus.TalkingOnMute;
            ChangeBtnUIByAgentStatus();
        }

        public void axcontrol_OnBeginMuteUserFailure()
        {
            Log4NetHelper.BaseLog("OCX_BeginMuteUserFailure");
        }

        public void axcontrol_OnEndMuteUserSuccess()
        {
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnEndMuteUserFailure()
        {
            Log4NetHelper.BaseLog("OCX_EndMuteUserFailure");
        }

        #endregion

        #region Hold/Unhold

        public void axcontrol_OnGetHoldFailure()
        {
            Log4NetHelper.BaseLog("OnGetHoldFailure");
        }

        public void axcontrol_OnGetHoldSuccess()
        {
            Log4NetHelper.BaseLog("OnGetHoldSuccess");
        }

        public void axcontrol_OnHoldFailure()
        {
            Log4NetHelper.BaseLog("OnHoldFailure");
        }

        public void axcontrol_OnGetHoldSuccTalk()
        {
            Log4NetHelper.BaseLog(Constant.GetHoldTalk);
            ChangeBarUI(AgentStatus.Talking, "", "");
        }

        #endregion

        #region  Rest/CancelRest

        /// <summary>
        /// 休息调用成功
        /// </summary>
        public void axcontrol_OnRestExSuccess()
        {
            Log4NetHelper.BaseLog("OnRestExSuccess");
            MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_WaitRest;
        }

        public void axcontrol_OnRestExFailure()
        {
            Log4NetHelper.BaseLog("OnRestExFailure");
        }

        public void axcontrol_OnCancelRestFailure()
        {
            Log4NetHelper.BaseLog("OnCancelRestFailure");
        }

        public void axcontrol_OnCancelRestSuccess()
        {
            Log4NetHelper.BaseLog("OnCancelRestSuccess");
            MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_EndRest;
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        #endregion

        #region  InnerCall

        public void axcontrol_OnCallInnerSuccess()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("mainwindow_innercallsuc");
            }), null);
            Log4NetHelper.BaseLog("OnCallInnerSuccess");
        }

        public void axcontrol_OnCallInnerFailure()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("mainwindow_innercallfal");
            }), null);
            Log4NetHelper.BaseLog("OnCallInnerFailure");
        }

        public void axcontrol_OnCallInnerSuccTalk()
        {
            Log4NetHelper.BaseLog("OnCallInnerSuccTalk");
            m_status = AgentStatus.InternaCall;
            var data = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetCallInfo();
            Talking(data);
        }

        #endregion

        #region  CallOut

        public void axcontrol_OnCallOutSuccess()
        {
            Log4NetHelper.BaseLog("OnCallOutSuccess");
        }

        public void axcontrol_OnCallOutFailure(int errCode)
        {
            TalkStatus = AgentStatus.Idle;
            Log4NetHelper.BaseLog("OnCallOutFailure");
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("calloutFailed");
            }));
            Thread.Sleep(350);

            //try
            //{       
            //    var status = BusinessAdapter.GetBusinessInstance().AgentStatus();
            //    ChangeUIByAgentState(status);
            //}
            //catch (Exception exc)
            //{
            //    Log4NetHelper.ExcepLog("Sys", "OnCallOutFailureEvent", exc.Message);
            //}
        }

        public void axcontrol_OnCallOutSuccTalkEx(int ulTime)
        {
            Log4NetHelper.BaseLog("OnCallOutSuccTalkEx");
            var lstHold = BusinessAdapter.GetBusinessInstance().GetHoldList();
            m_status = (lstHold != null && lstHold.Count > 0) ? AgentStatus.CallOutWithHold : AgentStatus.Talking;
            var data = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetCallInfo();
            Talking(data);
        }

        #endregion

        #region  Transfer Call

        public void axcontrol_OnTransInnerFailure()
        {
            Log4NetHelper.BaseLog("OnTransInnerFailure");
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnTransInnerSuccess()
        {
            Log4NetHelper.BaseLog("OnTransInnerSuccess");
        }

        public void axcontrol_OnRedirectToOtherFailure()
        {
            Log4NetHelper.BaseLog("OnRedirectToOtherFailure");
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnRedirectToOtherSuccess()
        {
            Log4NetHelper.BaseLog("OnRedirectToOtherSuccess");
        }

        public void axcontrol_OnRedirectToAutoSuccess()
        {
            Log4NetHelper.BaseLog("OnRedirectToAutoSuccess");
        }

        public void axcontrol_OnRedirectToAutoFailure()
        {
            Log4NetHelper.BaseLog("OnRedirectToAutoFailure");
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        #endregion

        #region  Connect Hold

        public void axcontrol_OnIsTalkingChanged()
        {
            Log4NetHelper.BaseLog(Constant.TalkChange);
        }

        public void axcontrol_OnConnectHoldSuccess()
        {
            Log4NetHelper.BaseLog("OnConnectHoldSuccess");
            m_status = AgentStatus.Idle;
            ChangeOtherUIByStatus(AgentStatus.Idle);
        }

        public void axcontrol_OnConnectHoldFailure()
        {
            Log4NetHelper.BaseLog("OnConnectHoldFailure");
        }

        #endregion

        #region  ConfJoin

        public void axcontrol_OnConfJoinSuccess()
        {
            Log4NetHelper.BaseLog("OnConfJoinSuccess");
        }

        public void axcontrol_OnConfJoinFailure()
        {
            Log4NetHelper.BaseLog("OnConfJoinFailure");
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnCallerCalledInfoArrived(int MediaType, string Caller, string Called)
        {
            Log4NetHelper.BaseLog(Constant.CallerCalledInfoArrived);
            MainWindowVM.GetInstance().CalledNumber = Called;
            MainWindowVM.GetInstance().CallerNumber = Caller;
        }

        public void axcontrol_OnDelCallInConf(string CallerNo, string CalledNo)
        {
            Log4NetHelper.BaseLog(Constant.DelCallInConf);
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        #endregion

        #region  TransOut

        public void axcontrol_OnTransOutSuccess(int mode)
        {
            Log4NetHelper.BaseLog("OnTransOutSuccess");
            StartIdle();
        }

        public void axcontrol_OnTransOutFailure(int mode)
        {
            Log4NetHelper.BaseLog("OnTransOutFailure");
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnTransOutFailTalk(int mode)
        {
            Log4NetHelper.BaseLog("OnTransOutFailTalk");
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            ChangeUIByAgentState(agentStatus);
        }

        public void axcontrol_OnTransOutSuccTalk(int mode)
        {
            Log4NetHelper.BaseLog("OnTransOutSuccTalk");
            m_status = AgentStatus.Talking;
            var data = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetCallInfo();
            Talking(data);
        }

        public void InternalHelpEventHandle(bool isBaseLog, string funcName, bool isSuc, AgentStatus status, string text)
        {
            m_status = status;
            //Talking("");
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindowVM.GetInstance().AgentState = text;
            }), null);
        }

        public void InnerHelpFailed()
        {
            TalkStatus = AgentStatus.Idle;
        }

        #endregion

        #region  Change Bar UI

        /// <summary>
        /// 改变 ccbar 界面状态， ocx 专用
        /// </summary>
        private void ChangeBarUI(AgentStatus status, string caller = "", string called = "")
        {
            m_status = status;
            switch (status)
            {
                case AgentStatus.SignIn:
                    SignIn();
                    break;
                case AgentStatus.SignOut:
                    ChangeOtherUIByStatus(AgentStatus.SignOut);
                    break;
                case AgentStatus.Idle:
                    ChangeOtherUIByStatus(AgentStatus.Idle);
                    break;
                case AgentStatus.Work:
                    ChangeOtherUIByStatus(AgentStatus.Work);
                    break;
                case AgentStatus.Hold:
                    ChangeOtherUIByStatus(AgentStatus.Hold);
                    break;
                case AgentStatus.Alerting:
                    ChangeOtherUIByStatus(AgentStatus.Alerting);
                    break;
                case AgentStatus.Busy:
                    ChangeOtherUIByStatus(AgentStatus.Busy);
                    break;
                case AgentStatus.Rest:
                    ChangeOtherUIByStatus(AgentStatus.Rest);
                    break;
                case AgentStatus.Talking:
                case AgentStatus.InternaHelp:
                case AgentStatus.InternaCall:
                    var data = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetCallInfo();
                    Talking(data);
                    break;
                case AgentStatus.TripartiteCall:
                    ChangeOtherUIByStatus(AgentStatus.TripartiteCall);
                    break;
                case AgentStatus.WaitAnswer:
                    break;
                default:
                    ChangeOtherUIByStatus(AgentStatus.Idle);
                    break;
            }
        }

        #endregion

        #endregion

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
            catch (JsonSerializationException exc)
            {
                ErrorMessage = exc.Message;
                Log4NetHelper.BaseLog("json parse failed");
            }
            return new JObject();
        }

        #region  Voice.OCX 方法与事件

        /// <summary>
        /// Voice.OCX 初始化,加载事件
        /// </summary>
        private bool VoiceInitial()
        {
            try
            {
                var useVoice = (_settings != null && _settings.ContainsKey("UseVoice")) ? string.Compare(_settings["UseVoice"], "true", true) == 0 : false;
                if (null == _voiceOCX && useVoice)
                {
                    _voiceOCX = new VoiceOCXBusiness();
                    _voiceOCX.AttachEvent(m_mainCallBack);
                }
                return true;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Voice Initial failed : " + exc.Message);
                Log4NetHelper.BaseLog(exc.Message);
                return false;
            }
        }

        /// <summary>
        /// Voice.OCX 振铃事件
        /// </summary>
        /// <remarks>
        /// 1,振铃事件中调用 answer 方法
        /// 2,商用下屏蔽视频能力
        /// </remarks>
        public void VoiceTalk(string sResult)
        {
            m_status = AgentStatus.Alerting;
            ChangeOtherUIByStatus(AgentStatus.Alerting);
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<VoiceOCXAlertData>(sResult);
            if (data == null || string.IsNullOrEmpty(data.callid))
            {
                return;
            }

            var answerRes = string.Empty;
            answerRes = _voiceOCX.Answer(data.callid);
            Log4NetHelper.BaseLog("voice_answerex " + answerRes);
        }

        /// <summary>
        /// Voice.OCX 话机号码注册结果事件（只在注册失败的情况下提示）
        /// </summary>
        /// <param name="result"></param>
        public void VoiceRegisterResult(string result)
        {
            var res = HuaweiAgentGateway.Utils.JsonUtil.DeJsonEx<VoiceRegisterResult>(result);
            if (res.resultCode != 0)
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("voice_registerfail") + " " + res.resultDesc);
            }
        }

        public void VoiceTalkDisconnected(string info)
        {
            QueryAgentStatus();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<VoiceTalkReleaseParm>(info);
            if (data == null) return;
            if (data.isVideo)
            {
                CloseVideoWindows(false);
            }
        }

        /// <summary>
        /// Voice.ocx 建立通话
        /// </summary>
        /// <param name="result"></param>
        public void VoiceConnect(string result)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<VoiceOCXConnectData>(result);
            if (data == null) return;

            var objData = new Dictionary<string, CallContent>()
                {
                    {"event", new CallContent() { content = new content() {
                        caller = data.caller,
                        called = data.callee }}}
                };
            var callData = Newtonsoft.Json.JsonConvert.SerializeObject(objData);
            m_status = AgentStatus.Talking;
            Talking(callData);
        }

        /// <summary>
        /// Voice.OCX 注册
        /// </summary>
        private string VoiceRegister()
        {
            var setAecRes = _voiceOCX.SetConfig("CALL_D_CFG_AUDIO_AGC", "1");
            Log4NetHelper.ActionLog("Voice", "AudioAgc", setAecRes);
            var setSesRes = _voiceOCX.SetConfig("CALL_D_CFG_SIP_SESSIONTIMER_ENABLE", "1");
            Log4NetHelper.ActionLog("Voice", "SessTimeEnable", setSesRes);
            var setRegTimeOut = _voiceOCX.SetConfig("CALL_D_CFG_SIP_REGIST_TIMEOUT", "3600");
            Log4NetHelper.ActionLog("Voice", "RegistTimeout", setRegTimeOut);
            var setCodeRes = _voiceOCX.SetConfig("CALL_D_CFG_AUDIO_CODEC", "[{\"code\":\"112\"},{\"code\":\"98\"},{\"code\":\"18\"},{\"code\":\"8\"},{\"code\":\"0\"}]");

            var backSerIP = string.Empty;
            var mainIp = DomainToIP(GetSettingDictStrValue("SipIP"));
            var mainPort = GetSettingDictStrValue("SipPort");
            var backIp = DomainToIP(GetSettingDictStrValue("SipIP2"));
            var backPort = GetSettingDictStrValue("SipPort2");
            if (!string.IsNullOrEmpty(backIp) && !string.IsNullOrEmpty(backPort))
            {
                var lstSip = new List<SipInfo>() { new SipInfo() { ip = backIp, port = backPort } };
                backSerIP = HuaweiAgentGateway.Utils.JsonUtil.ToJson(lstSip);
            }

            var domain = GetSettingDictStrValue("Domain");
            var mainSipIp = mainIp;
            if (!string.IsNullOrEmpty(domain))
            {
                mainSipIp = mainSipIp + "|" + domain;
            }

            var sipRes = _voiceOCX.SetSipServerInfo(mainSipIp, mainPort, backSerIP);
            Log4NetHelper.ActionLog("Voice", "SetSipServer", sipRes);
            var locRes = _voiceOCX.SetLocalInfo(LOCAL_IP, "5080", "19100");
            Log4NetHelper.ActionLog("Voice", "SetLocalInfo", locRes);
            var mainAccount = GetSettingDictStrValue("VoiceAccount");
            var regRes = _voiceOCX.Register(mainAccount, VoicePwd, DEFAULT_VOICEREGMODE);
            Log4NetHelper.ActionLog("Voice", "Register", regRes);

            return regRes;
        }

        #endregion

        #region  窗体其他按钮功能（按钮可见性，说明，RecordScreen, SnapShot）

        public void OnInstructionClick(object sender, EventArgs e)
        {
            var insWin = new InstructionWindow();
            insWin.ShowDialog();
        }

        public void OnHelpClick(object sender, EventArgs e)
        {
            try
            {
                var chm = string.Compare("zh-cn", LanguageResource.CurrentLanguage, true) == 0 ? "QuickAgent操作指南.chm" : "QuickAgent Operation Guide.chm";
                var defaultChmPath = new Uri("Resource/Docs/" + chm, UriKind.Relative) + "";
                System.Windows.Forms.Help.ShowHelp(null, defaultChmPath);
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "OpenHelpFile", exc.Message);
            }
        }

        public void OnInstallCertiClick(object sender, EventArgs e)
        {
            var win = new InstallCertification();
            win.ShowDialog();
            if (!win.IsConfirm || string.IsNullOrEmpty(win.Ip) || string.IsNullOrEmpty(win.Port))
            {
                return;
            }

            try
            {
                var url = "https://" + win.Ip + ":" + win.Port + "/agentgateway/resource/queuedevice/123/queryskillstat/";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                SetSecurityProtocolByUrlEx(url);
                request.Timeout = 20000;
                request.KeepAlive = false;
                request.Method = "GET";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "InstallCertification", exc.Message);
            }
            finally
            {
                ServicePointManager.ServerCertificateValidationCallback -= CheckValidationResult;
            }
        }

        private void SetSecurityProtocolByUrlEx(string url)
        {
            try
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)192 | (SecurityProtocolType)768;
            }
            catch (NotSupportedException nsexc)
            {
                Log4NetHelper.ExcepLog("Sys", "SecurityProtocol", nsexc.Message);
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            }
            ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
        }

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            X509Store store = null;
            try
            {

                X509Certificate2 oldCertificate2 = null;
                X509Certificate2 certificate2 = new X509Certificate2(certificate);
                Log4NetHelper.BaseLog("get certification succ");
                store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadWrite);
                Log4NetHelper.BaseLog("open certification store succ");

                foreach (X509Certificate2 myX509Certificate2 in store.Certificates)
                {
                    if (myX509Certificate2.Subject == certificate2.Subject)
                    {
                        oldCertificate2 = myX509Certificate2;
                        Log4NetHelper.BaseLog("exist old certification");
                    }
                }

                if (null != oldCertificate2)
                {
                    store.Remove(oldCertificate2);
                    Log4NetHelper.BaseLog("remove old certification succ");
                }
                store.Add(certificate2);
                Log4NetHelper.BaseLog("add certification succ");
            }
            catch (CryptographicException ex)
            {
                Log4NetHelper.ExcepLog("Sys", "InstallCertification_CryptographicException", ex.ToString());
            }
            catch (SecurityException ex)
            {
                Log4NetHelper.ExcepLog("Sys", "InstallCertification_SecurityException", ex.Message);
            }
            finally
            {
                if (null != store)
                {
                    store.Close();
                }
                store = null;
            }
            return true;
        }

        #endregion

        #region  视频呼叫相关方法

        private void CheckAndShowVideoWindows(bool isShowLocal, bool isShowRemote, bool isHide = false)
        {
            if (null == m_videoLocalWindow)
                m_videoLocalWindow = new VideoWinCtrl(DEFAULT_VIDEOWIN_LEFT, 500, DEFAULT_VIDEOWIN_WIDTH,
                    DEFAULT_VIDEOWIN_HEIGHT, LanguageResource.FindResourceMessageByKey("main_localvideowindow"));
            if (null == m_videoRemoteWindow)
                m_videoRemoteWindow = new VideoWinCtrl(DEFAULT_VIDEOWIN_LEFT, 200, DEFAULT_VIDEOWIN_WIDTH,
                    DEFAULT_VIDEOWIN_HEIGHT, LanguageResource.FindResourceMessageByKey("main_remotevideowindow"));
            if (isShowLocal)
            {
                m_videoLocalWindow.Show();
                if (isHide)
                    m_videoLocalWindow.Hide();
            }
            if (isShowRemote)
            {
                m_videoRemoteWindow.Show();
                if (isHide)
                    m_videoRemoteWindow.Hide();
            }
        }

        private void CloseVideoWindows(bool isForceClose)
        {
            if (m_videoLocalWindow != null)
            {
                m_videoLocalWindow.IsForceClose = isForceClose;
                m_videoLocalWindow.Close();
            }
            if (m_videoRemoteWindow != null)
            {
                m_videoRemoteWindow.IsForceClose = isForceClose;
                m_videoRemoteWindow.Close();
            }
            if (isForceClose)
            {
                m_videoLocalWindow = null;
                m_videoRemoteWindow = null;
            }
        }

        private void LocalVideoWindowClick(object sender, EventArgs e)
        {
            if (null == m_videoLocalWindow)
            {
                m_videoLocalWindow = new VideoWinCtrl(DEFAULT_VIDEOWIN_LEFT, 500, DEFAULT_VIDEOWIN_WIDTH,
                   DEFAULT_VIDEOWIN_HEIGHT, LanguageResource.FindResourceMessageByKey("main_localvideowindow"));
            }
            m_videoLocalWindow.Show();
        }

        private void RemoteVideoWindowClick(object sender, EventArgs e)
        {
            if (null == m_videoRemoteWindow)
            {
                m_videoRemoteWindow = new VideoWinCtrl(DEFAULT_VIDEOWIN_LEFT, 200, DEFAULT_VIDEOWIN_WIDTH,
                  DEFAULT_VIDEOWIN_HEIGHT, LanguageResource.FindResourceMessageByKey("main_remotevideowindow"));
            }
            m_videoRemoteWindow.Show();
        }

        #endregion

        #region  内部方法（错误信息，界面改变）

        /// <summary>
        /// 错误处理: 根据错误码获取错误描述 -> 弹出消息框 
        /// </summary>
        /// <param name="errCode"></param>
        public void MessageBoxForErr(string errCode)
        {
            #region  AGW 错误码集合

            var m_AGWErrDic = new Dictionary<string, string>()
        {
            {"000-001",LanguageResource.FindResourceMessageByKey("000-001")},
            {"000-002",LanguageResource.FindResourceMessageByKey("000-002")},
            {"000-003",LanguageResource.FindResourceMessageByKey("000-003")},
            {"000-004",LanguageResource.FindResourceMessageByKey("000-004")},
            {"100-001",LanguageResource.FindResourceMessageByKey("100-001")},
            {"100-002",LanguageResource.FindResourceMessageByKey("100-002")},
            {"100-003",LanguageResource.FindResourceMessageByKey("100-003")},
            {"100-004",LanguageResource.FindResourceMessageByKey("100-004")},
            {"100-005",LanguageResource.FindResourceMessageByKey("100-005")},
            {"100-006",LanguageResource.FindResourceMessageByKey("100-006")},
            {"100-007",LanguageResource.FindResourceMessageByKey("100-007")},
            {"100-008",LanguageResource.FindResourceMessageByKey("100-008")},
            {"100-009",LanguageResource.FindResourceMessageByKey("100-009")},
            {"100-010",LanguageResource.FindResourceMessageByKey("100-010")},
            {"100-011",LanguageResource.FindResourceMessageByKey("100-011")},
            {"100-012",LanguageResource.FindResourceMessageByKey("100-012")},
            {"100-013",LanguageResource.FindResourceMessageByKey("100-013")},
            {"100-014",LanguageResource.FindResourceMessageByKey("100-014")},
            {"100-015",LanguageResource.FindResourceMessageByKey("100-015")},
            {"100-016",LanguageResource.FindResourceMessageByKey("100-016")},
            {"110-016",LanguageResource.FindResourceMessageByKey("110-016")},
            {"101-001",LanguageResource.FindResourceMessageByKey("101-001")},
            {"101-002",LanguageResource.FindResourceMessageByKey("101-002")},
            {"200-001",LanguageResource.FindResourceMessageByKey("200-001")},
            {"200-002",LanguageResource.FindResourceMessageByKey("200-002")},
            {"200-003",LanguageResource.FindResourceMessageByKey("200-003")},
            {"200-004",LanguageResource.FindResourceMessageByKey("200-004")},
            {"200-005",LanguageResource.FindResourceMessageByKey("200-005")},
            {"200-006",LanguageResource.FindResourceMessageByKey("200-006")},
            {"200-007",LanguageResource.FindResourceMessageByKey("200-007")},
            {"200-008",LanguageResource.FindResourceMessageByKey("200-008")},
            {"200-009",LanguageResource.FindResourceMessageByKey("200-009")},
            {"200-010",LanguageResource.FindResourceMessageByKey("200-010")},
            {"200-011",LanguageResource.FindResourceMessageByKey("200-011")},
            {"200-012",LanguageResource.FindResourceMessageByKey("200-012")},
            {"200-013",LanguageResource.FindResourceMessageByKey("200-013")},
            {"200-014",LanguageResource.FindResourceMessageByKey("200-014")},
            {"200-015",LanguageResource.FindResourceMessageByKey("200-015")},
            {"200-016",LanguageResource.FindResourceMessageByKey("200-016")},
            {"200-017",LanguageResource.FindResourceMessageByKey("200-017")},
            {"200-018",LanguageResource.FindResourceMessageByKey("200-018")},
            {"200-019",LanguageResource.FindResourceMessageByKey("200-019")},
            {"200-020",LanguageResource.FindResourceMessageByKey("200-020")},
            {"200-021",LanguageResource.FindResourceMessageByKey("200-021")},
            {"200-022",LanguageResource.FindResourceMessageByKey("200-022")},
            {"200-023",LanguageResource.FindResourceMessageByKey("200-023")},
            {"200-024",LanguageResource.FindResourceMessageByKey("200-024")},
            {"200-025",LanguageResource.FindResourceMessageByKey("200-025")},
            {"200-026",LanguageResource.FindResourceMessageByKey("200-026")},
            {"210-001",LanguageResource.FindResourceMessageByKey("210-001")},
            {"300-001",LanguageResource.FindResourceMessageByKey("300-001")},
            {"300-002",LanguageResource.FindResourceMessageByKey("300-002")},
            {"300-003",LanguageResource.FindResourceMessageByKey("300-003")},
            {"300-004",LanguageResource.FindResourceMessageByKey("300-004")},
            {"400-001",LanguageResource.FindResourceMessageByKey("400-001")},
            {"400-003",LanguageResource.FindResourceMessageByKey("400-003")},
            {"500-001",LanguageResource.FindResourceMessageByKey("500-001")},
            {"500-002",LanguageResource.FindResourceMessageByKey("500-002")},
            {"500-003",LanguageResource.FindResourceMessageByKey("500-003")},
            {"500-004",LanguageResource.FindResourceMessageByKey("500-004")},
            {"500-005",LanguageResource.FindResourceMessageByKey("500-005")},
            {"500-006",LanguageResource.FindResourceMessageByKey("500-006")},
            {"500-007",LanguageResource.FindResourceMessageByKey("500-007")},
            {"500-008",LanguageResource.FindResourceMessageByKey("500-008")},
            {"500-009",LanguageResource.FindResourceMessageByKey("500-009")},
            {"500-010",LanguageResource.FindResourceMessageByKey("500-010")},
            {"600-001",LanguageResource.FindResourceMessageByKey("600-001")},
            {"600-002",LanguageResource.FindResourceMessageByKey("600-002")},
            {"600-003",LanguageResource.FindResourceMessageByKey("600-003")},
            {"600-004",LanguageResource.FindResourceMessageByKey("600-004")},
            {"600-005",LanguageResource.FindResourceMessageByKey("600-005")},
            {"700-001",LanguageResource.FindResourceMessageByKey("700-001")},
            {"700-002",LanguageResource.FindResourceMessageByKey("700-002")},
            {"700-003",LanguageResource.FindResourceMessageByKey("700-003")},
            {"701-001",LanguageResource.FindResourceMessageByKey("701-001")},
            {"701-002",LanguageResource.FindResourceMessageByKey("701-002")},
            {"701-003",LanguageResource.FindResourceMessageByKey("701-003")},
            {"701-004",LanguageResource.FindResourceMessageByKey("701-004")},
            {"701-005",LanguageResource.FindResourceMessageByKey("701-005")},
            {"701-006",LanguageResource.FindResourceMessageByKey("701-006")},
            {"800-001",LanguageResource.FindResourceMessageByKey("800-001")},
            {"800-002",LanguageResource.FindResourceMessageByKey("800-002")},
            {"800-003",LanguageResource.FindResourceMessageByKey("800-003")},
            {"900-001",LanguageResource.FindResourceMessageByKey("900-001")},
            {"900-002",LanguageResource.FindResourceMessageByKey("900-002")},
            {"900-003",LanguageResource.FindResourceMessageByKey("900-003")},
            {"900-004",LanguageResource.FindResourceMessageByKey("900-004")},
            {"900-005",LanguageResource.FindResourceMessageByKey("900-005")},
            {"900-006",LanguageResource.FindResourceMessageByKey("900-006")},
            {"900-007",LanguageResource.FindResourceMessageByKey("900-007")},
            {"900-008",LanguageResource.FindResourceMessageByKey("900-008")},
            {"900-009",LanguageResource.FindResourceMessageByKey("900-009")},
            {"999-001",LanguageResource.FindResourceMessageByKey("999-001")}
        };

            #endregion

            var errMsg = errCode;
            if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
            {
                errMsg = ((OCXBusiness)BusinessAdapter.GetBusinessInstance()).GetErrMsg(errCode);
            }
            else if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
            {
                if (m_AGWErrDic.ContainsKey(errCode))
                {
                    errMsg = m_AGWErrDic[errCode];
                }
                else
                {
                    errMsg = LanguageResource.FindResourceMessageByKey("main_syserror");
                }
            }
            else
            {
                errMsg = LanguageResource.FindResourceMessageByKey("main_syserror");
            }
            MessageBox.Show(errMsg);
        }

        private string GetSettingDictStrValue(string key)
        {
            if (null == _settings || !_settings.ContainsKey(key))
            {
                return string.Empty;
            }
            return _settings[key];
        }

        private int GetSettingDictIntValue(string key)
        {
            int val = 0;
            if (null == _settings || !_settings.ContainsKey(key)) return val;
            Int32.TryParse(_settings[key], out val);
            return val;
        }

        /// <summary>
        /// 通用事件处理(日志记录)
        /// </summary>
        public void CommonEventHandle(string eventName, bool isSuc, string parm, bool isBaseLog, string showText)
        {
            MainWindowVM main = MainWindowVM.GetInstance();
            main.AgentState = showText;
        }

        public void SendMsgFailure(int sendToWorkNo)
        {
            MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_SendMsgFal;
        }

        public void SendMsgSucc(int sendToWorkNo)
        {
            MainWindowVM.GetInstance().AgentState = new CommonText().StateInfo_SendMsgSuc;
        }

        #region  不同功能下界面显示

        /// <summary>
        /// 休息超时
        /// </summary>
        public void RestOverTime()
        {
            Log4NetHelper.BaseLog(Constant.RestOverTime);
            string tips = LanguageResource.FindResourceMessageByKey("restTimeOut");
            MessageBox.Show(string.Format(tips, m_restTime));
        }

        /// <summary>
        /// 开始休息
        /// </summary>
        public void StartRest()
        {
            Log4NetHelper.BaseLog(Constant.RestStart);
            m_status = AgentStatus.Rest;
            ChangeOtherUIByStatus(AgentStatus.Rest, string.Empty, string.Empty, true);
        }

        /// <summary>
        /// 示忙/强制示忙 (默认是示忙)
        /// </summary>
        public void StartBusy(string funcName = Constant.SayBusy)
        {
            m_status = AgentStatus.Busy;
            m_baseStatus = AgentBaseStatus.Busy;
            ChangeOtherUIByStatus(AgentStatus.Busy);
        }

        /// <summary>
        /// 示闲/强制示闲 （默认是示忙）
        /// </summary>
        public void StartIdle(string funcName = Constant.SayFree)
        {
            m_status = AgentStatus.Idle;
            m_baseStatus = AgentBaseStatus.Idle;
            ChangeOtherUIByStatus(AgentStatus.Idle);
        }

        /// <summary>
        /// 三方通话
        /// </summary>
        public void TripartiteCall()
        {
            m_status = AgentStatus.TripartiteCall;
            ChangeOtherUIByStatus(AgentStatus.TripartiteCall);
        }

        /// <summary>
        /// 保持
        /// </summary>
        public void OnHold()
        {
            m_status = AgentStatus.Hold;
            ChangeOtherUIByStatus(AgentStatus.Hold);
            QueryHoldList();
        }

        #endregion

        #region  查询类方法

        private void QueryAgentStatus()
        {
            string agentStatus = BusinessAdapter.GetBusinessInstance().AgentStatus();
            if (agentStatus != null)
            {
                if (agentStatus.Equals("1") || agentStatus.Equals("4") || agentStatus.Equals("6"))
                {
                    //示闲状态
                    m_status = AgentStatus.Idle;
                    m_baseStatus = AgentBaseStatus.Idle;
                    ChangeOtherUIByStatus(AgentStatus.Idle);
                }
                else if (agentStatus.Equals("2"))
                {
                    //签出状态
                    m_status = AgentStatus.SignOut;
                    ChangeOtherUIByStatus(AgentStatus.SignOut);
                }
                else if (agentStatus.Equals("3"))
                {
                    //示忙状态
                    m_status = AgentStatus.Busy;
                    m_baseStatus = AgentBaseStatus.Busy;
                    ChangeOtherUIByStatus(AgentStatus.Busy);
                }
                else if (agentStatus.Equals("8"))
                {
                    //休息态
                    m_status = AgentStatus.Rest;
                    ChangeOtherUIByStatus(AgentStatus.Rest);
                }
                else
                {
                    Log4NetHelper.BaseLog(Constant.GetAgentStatus + ":" + agentStatus);
                }
            }
        }

        private void QueryAgentSkills()
        {
            if (m_mySkillList != null)
            {
                m_mySkillList.Clear();
            }
            List<SkillInfo> result = BusinessAdapter.GetBusinessInstance().GetAgentSkills();
            if (result != null && result.Count > 0)
            {
                m_mySkillList = result;
            }
        }

        private void QueryVDNIvrInfo()
        {
            if (ivrInfoList != null)
            {
                ivrInfoList.Clear();
            }
            List<IvrInfo> result = BusinessAdapter.GetBusinessInstance().GetVDNIvrInfo();
            if (result != null && result.Count > 0)
            {
                ivrInfoList = result;
            }
        }

        private void QueryVDNSkillInfo()
        {
            if (skillInfoList != null)
            {
                skillInfoList.Clear();
            }
            List<SkillInfo> result = BusinessAdapter.GetBusinessInstance().GetVDNSkillInfo();
            if (result != null && result.Count > 0)
            {
                skillInfoList = result;
            }
        }

        private void QueryVDNAccessCodeInfo()
        {
            if (accessCodeList != null)
            {
                accessCodeList.Clear();
            }
            List<AccessCode> result = BusinessAdapter.GetBusinessInstance().GetVDNAccessCodeInfo();
            if (result != null && result.Count > 0)
            {
                accessCodeList = result;
            }
        }

        private bool QueryHoldList()
        {
            if (holdinfoList != null)
            {
                holdinfoList.Clear();
            }
            List<HoldListData> result = BusinessAdapter.GetBusinessInstance().GetHoldList();
            if (result != null && result.Count > 0)
            {
                holdinfoList = result;
            }
            return BusinessAdapter.GetBusinessInstance().ExistHold;
        }

        #endregion

        #region  其他方法

        /// <summary>
        /// 域名转 ip
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        private string DomainToIP(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return string.Empty;
            }

            try
            {
                IPAddress[] ips;
                ips = Dns.GetHostAddresses(domain);
                if (null == ips || ips.Length == 0)
                {
                    return domain;
                }
                return ips[0].ToString();
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("DomainToIP Failed：" + exc.Message);
                return domain;
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        private bool ModifyPwd(string workNo)
        {
            Log4NetHelper.BaseLog("Start change password.");
            MessageBox.Show(LanguageResource.FindResourceMessageByKey("main_changepwd"));
            var mcpModifyPwd = new ModifyPwdWindow();
            mcpModifyPwd.ShowDialog();
            if (!mcpModifyPwd.DialogResult.Value)
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("main_cancelchangepwd"));
                Log4NetHelper.BaseLog("Cancel Modify Password.");
                return false;
            }
            var mcpModyPwdRes = BusinessAdapter.GetBusinessInstance().ModifyAccountPwd(workNo, mcpModifyPwd.OldPwd, mcpModifyPwd.NewPwd);
            Log4NetHelper.BaseLog("Change Password Res: " + mcpModyPwdRes);
            if (!string.Equals(mcpModyPwdRes, AGWErrorCode.OK))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("main_changepwdfail"));
                return false;
            }
            MessageBox.Show(LanguageResource.FindResourceMessageByKey("main_changepwdsucc"));
            return true;
        }

        /// <summary>
        /// 检测 QuickAgent 是否已经开启（防止多开）
        /// </summary>
        /// <returns></returns>
        private bool HasQuickAgentStarted()
        {
            System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] allProcess = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in allProcess)
            {
                if (process.Id != curProcess.Id)
                {
                    if (process.ProcessName == curProcess.ProcessName)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 初始化界面的模式、尺寸、风格信息
        /// </summary>
        private void InitMainWindowSize()
        {
            this.Height = (double)30;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Top = Constants.Constant.MainWindowTop;
            this.Left = Constants.Constant.MainWindowLeft;

            MainWindowVM.GetInstance().ControlBarHeight = this.Height;
            MainWindowVM.GetInstance().ControlBarWidth = 1024;//this.Width * 24 / 45;
            MainWindowVM.GetInstance().ComponentWidth = 30 / 28f * (this.Height - 2);
            MainWindowVM.GetInstance().ComponentHeight = this.Height - 2;
        }

        #endregion

        #region  界面状态更改

        /// <summary>
        /// 通过座席状态改变其他状态（主叫被叫,状态图标,提示文字）
        /// </summary>
        private void ChangeOtherUIByStatus(AgentStatus status, string caller = "", string called = "", bool isShowMsg = false)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                ChangeBtnUIByAgentStatus();
                MainWindowVM main = MainWindowVM.GetInstance();
                switch (status)
                {
                    case AgentStatus.SignIn:
                        main.IsLogin = true;
                        main.LogoutVisibility = Visibility.Visible;
                        main.LoginVisibility = Visibility.Collapsed;
                        main.JobNumber = m_agentInfo.AgentId + " ; ";
                        tbk_PhoneNumber.Text = m_agentInfo.PhoneNumber;
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Idle, ImageResource.AgentStateSource_Idle, new CommonText().StateInfo_Idle, true);
                        m_status = AgentStatus.Idle;
                        break;
                    case AgentStatus.SignOut:
                        var useVoice = _settings != null && _settings.ContainsKey("UseVoice") ? string.Compare(_settings["UseVoice"], "true", true) == 0 : false;
                        if (useVoice && _voiceOCX != null)
                        {
                            var res = _voiceOCX.Deregister();
                        }
                        DefaultUIStatus();
                        main.LogoutVisibility = Visibility.Collapsed;
                        main.LoginVisibility = Visibility.Visible;
                        main.JobNumber = string.Empty;
                        main.IsLogin = false;
                        m_mainCallBack.DetachEvents();
                        SetDefaultMuteStatus();
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Leave, ImageResource.AgentStateSource_Leave, new CommonText().StateInfo_Leave, true);
                        break;
                    case AgentStatus.Alerting:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_HandleCall, string.Empty, new CommonText().StateInfo_Ringing);
                        break;
                    case AgentStatus.WaitAnswer:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_HandleCall, string.Empty, new CommonText().StateInfo_WaitAnswer);
                        break;
                    case AgentStatus.WaitRest:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Idle, ImageResource.AgentStateSource_Idle, new CommonText().StateInfo_WaitRest);
                        break;
                    case AgentStatus.TripartiteCall:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_HandleCall, string.Empty, LanguageResource.FindResourceMessageByKey("threeCall"));
                        break;
                    case AgentStatus.Talking:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_HandleCall, string.Empty, new CommonText().StateInfo_Talking);
                        break;
                    case AgentStatus.Idle:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Idle, ImageResource.AgentStateSource_Idle, new CommonText().StateInfo_Idle, true);
                        break;
                    case AgentStatus.Busy:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Idle, ImageResource.AgentStateSource_Busy, new CommonText().StateInfo_Busy, true);
                        break;
                    case AgentStatus.Hold:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Busy, string.Empty, new CommonText().StateInfo_Idle, true);
                        break;
                    case AgentStatus.Work:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Busy, string.Empty, new CommonText().StateInfo_Work, true);
                        break;
                    case AgentStatus.Rest:
                        ChangeOtherStatus(caller, called, ImageResource.AgentStateSource_Idle, ImageResource.AgentStateSource_Rest, LanguageResource.FindResourceMessageByKey("agentRest"), true);
                        if (isShowMsg)
                        {
                            string strTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            string tips = LanguageResource.FindResourceMessageByKey("agentStartRest");
                            MessageBox.Show(string.Format(tips, strTime));
                        }
                        break;
                }
            }), null);
        }

        /// <summary>
        /// 设置按钮可见性（非签出状态即可见）
        /// </summary>
        private void CheckBtnVisibility()
        {
            ReleaseButton.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_ExitWork.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_EnterWork.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            AnswerButton.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_SetData.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_QueryData.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_CallTransform.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_TransformOut.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            NoSoundButton.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            HoldButton.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            UnholdButton.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_TripartiteCall.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_ConnectHold.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_InternalCall.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_CallOut.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_Busy.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_Rest.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_InternalRequestHelp.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_Announcement.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
            Btn_DispatchNote.Visibility = m_status == AgentStatus.SignOut ? Visibility.Hidden : Visibility.Visible;
        }

        /// <summary>
        /// 改变按钮状态
        /// </summary>
        private void ChangeBtnStatus(bool release, bool exitwork, bool entWork, bool answer, bool setDt, bool qryDt, bool callTrans, bool transOut, bool mute,
            bool hold, bool unhold, bool triCall, bool connHold, bool innerCall, bool callOut, bool busy, bool rest, bool innerHelp, bool annou, bool note)
        {
            if (!mute)  // 静音按钮不可用时，下压状态如果有则去除
            {
                NoSoundButton.Focusable = false;
            }

            CheckBtnVisibility();
            ReleaseButton.IsEnabled = release;
            Btn_ExitWork.IsEnabled = exitwork;
            Btn_EnterWork.IsEnabled = entWork;
            AnswerButton.IsEnabled = answer;
            Btn_SetData.IsEnabled = setDt;
            Btn_QueryData.IsEnabled = qryDt;
            Btn_CallTransform.IsEnabled = callTrans;
            Btn_TransformOut.IsEnabled = transOut;
            NoSoundButton.IsEnabled = mute;
            HoldButton.IsEnabled = hold;
            UnholdButton.IsEnabled = unhold;
            Btn_TripartiteCall.IsEnabled = triCall;
            Btn_ConnectHold.IsEnabled = connHold;
            Btn_InternalCall.IsEnabled = innerCall;
            Btn_CallOut.IsEnabled = callOut;
            Btn_Busy.IsEnabled = busy;
            Btn_Rest.IsEnabled = rest;
            Btn_InternalRequestHelp.IsEnabled = innerHelp;
            Btn_Announcement.IsEnabled = annou;
            Btn_DispatchNote.IsEnabled = note;
        }

        /// <summary>
        /// 改变其他状态（主叫被叫,状态图标,提示文字）
        /// </summary>
        /// <param name="caller">caller</param>
        /// <param name="called">called</param>
        /// <param name="agentState">agent state</param>
        /// <param name="curAgentState">current agent state</param>
        /// <param name="stateText">state text</param>
        /// <param name="isCallEmpty">has call</param>
        private void ChangeOtherStatus(string caller, string called, string agentState, string curAgentState, string stateText, bool isCallEmpty = false)
        {
            MainWindowVM main = MainWindowVM.GetInstance();
            if (!string.IsNullOrEmpty(stateText))
                main.AgentState = stateText;
            if (!string.IsNullOrEmpty(agentState))
                main.AgentStateIcon = agentState;
            if (!string.IsNullOrEmpty(curAgentState))
                main.CurAgentStateIcon = curAgentState;

            if (isCallEmpty)
            {
                main.CalledNumber = string.Empty;
                main.CallerNumber = string.Empty;
            }
            else
            {
                if (!string.IsNullOrEmpty(called))
                    main.CalledNumber = called;
                if (!string.IsNullOrEmpty(caller))
                    main.CallerNumber = caller;
            }
        }

        /// <summary>
        /// 根据座席状态改变按钮状态
        /// </summary>
        private void ChangeBtnUIByAgentStatus()
        {
            MainWindowVM main = MainWindowVM.GetInstance();
            switch (m_status)
            {
                case AgentStatus.SignIn:
                case AgentStatus.Idle:
                case AgentStatus.Work:
                case AgentStatus.Alerting:
                    ChangeBtnStatus(true, true, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, true, true);
                    break;
                case AgentStatus.SignOut:
                    CheckBtnVisibility();
                    break;
                case AgentStatus.Busy:
                    ChangeBtnStatus(true, true, true, false, false, false, false, false, false, false, false, false, false, true, true, true, false, false, true, true);
                    break;
                case AgentStatus.WaitAnswer:
                    ChangeBtnStatus(true, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, true);
                    break;
                case AgentStatus.Talking:
                    ChangeBtnStatus(true, false, false, false, true, true, true, true, true, true, false, false, false, false, false, true, true, true, true, true);
                    break;
                case AgentStatus.TalkingOnMute:
                    ChangeBtnStatus(true, false, false, false, true, true, false, false, true, false, false, false, false, false, false, true, true, true, true, true);
                    break;
                case AgentStatus.Hold:
                    ChangeBtnStatus(true, true, true, false, false, false, false, false, false, false, true, false, false, true, true, true, true, false, true, true);
                    break;
                case AgentStatus.Rest:
                    ChangeBtnStatus(true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, true);
                    break;
                case AgentStatus.WaitRest:
                    ChangeBtnStatus(true, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, true, false, true, true);
                    break;
                case AgentStatus.TripartiteCall:
                    ChangeBtnStatus(true, false, false, false, true, true, false, false, true, false, false, false, false, false, false, true, true, false, true, true);
                    break;
                case AgentStatus.InternaCall:
                case AgentStatus.InternaHelp:
                    ChangeBtnStatus(true, false, false, false, true, true, false, false, false, false, false, false, false, false, false, true, true, false, true, true);
                    break;
                case AgentStatus.CallOutWithHold:
                    ChangeBtnStatus(true, false, false, false, true, true, false, false, false, false, false, true, true, false, false, true, true, false, true, true);
                    break;
                default:
                    ChangeBtnStatus(true, true, true, false, false, false, false, false, false, false, false, false, false, true, true, true, true, false, true, true);
                    break;
            }
            //MenuItem_BtnSet.IsEnabled = m_status == AgentStatus.SignOut;
            MenuItem_Config.IsEnabled = m_status == AgentStatus.SignOut;
        }

        /// <summary>
        /// 文字交谈下，界面部分按钮提示 "动作不允许"
        /// </summary>
        private void SetActionDisableForTxtMsg()
        {
            MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("main_actiondisable");
        }

        /// <summary>
        /// 默认的 UI 状态
        /// </summary>
        private void DefaultUIStatus()
        {
            Btn_Busy.ToolTip = LanguageResource.FindResourceMessageByKey("busy");
            Btn_Busy.Focusable = false;
            Btn_Rest.ToolTip = LanguageResource.FindResourceMessageByKey("rest");
            Btn_Rest.Focusable = false;
            VoicePwd = string.Empty;
            tbk_PhoneNumber.Text = string.Empty;
            CloseVideoWindows(true);
            var ins = MainWindowVM.GetInstance();
            if (null == ins)
            {
                return;
            }
            ins.IsMute = false;
        }

        #endregion

        #endregion

        #region  UI Limit Methods (business type,demo version)

        /// <summary>
        /// get demo version
        /// </summary>
        /// <remarks>default version is C10</remarks>
        private void GetCurrentVersion()
        {
            Version = DemoVersion.C10;
            try
            {
                var conf = ConfigHelper.Load();
                if (null == conf)
                {
                    conf = new ConfigHelper();
                }
                if (null == conf.Settings)
                {
                    conf.Settings = new Dictionary<string, string>();
                }
                if (!conf.Settings.ContainsKey("Version"))
                {
                    conf.Settings.Add("Version", "C10");
                    conf.Save();
                    return;
                }
                Version = string.Equals("C80", conf.Settings["Version"]) ? DemoVersion.C80 : DemoVersion.C10;
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
        }

        /// <summary>
        /// whether current demo version is C80
        /// </summary>
        /// <returns></returns>
        private bool IsC80Version()
        {
            return Version == DemoVersion.C80;
        }

        /// <summary>
        /// whether current demo business is agentgateway business
        /// </summary>
        /// <returns></returns>
        private bool IsAgwBusiness()
        {
            return BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway;
        }

        /// <summary>
        ///  whether current call type is text chat or not
        /// </summary>
        /// <remarks>limit voice function at text chat.show 'action not allowed'</remarks>
        /// <returns></returns>
        private bool IsCallTypeTextChat()
        {
            if (ComboBox_CallType.SelectedIndex == 1)
            {
                MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("main_actiondisable");
            }
            return ComboBox_CallType.SelectedIndex == 1;
        }

        #endregion

        #region  Other Methods

        /// <summary>
        /// check if language file exists
        /// </summary>
        /// <remarks>if not exists,then show language select window</remarks>
        private void CheckLanguageFileAndSet()
        {
            try
            {
                if (File.Exists(FileInfos.PathConfig))
                {
                    return;
                }

                var languageSelect = new LanguageSelect();
                languageSelect.ShowDialog();
                if (!languageSelect.IsConfirm)
                {
                    this.Close();
                    return;
                }
                FileInfos.SetLanguageAndWriteIni(languageSelect.Culture);
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "WriteLanguageIni", exc.Message);
            }
        }

        /// <summary>
        /// set mute status as default
        /// </summary>
        /// <remarks>use this function in call release and sign out</remarks>
        private void SetDefaultMuteStatus()
        {
            var ins = MainWindowVM.GetInstance();
            if (null == ins)
            {
                return;
            }
            ins.IsMute = false;
        }

        #endregion
    }
}
