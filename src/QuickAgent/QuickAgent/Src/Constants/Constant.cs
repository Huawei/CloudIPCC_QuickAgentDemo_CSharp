using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace QuickAgent.Constants
{
    public static class Constant
    {
        #region 窗口信息
        public const double MainWindowTop = 0;
        public const double MainWindowLeft = 0;
        public const WindowStyle MainWindowWindowStyle = WindowStyle.None;
        public const ResizeMode MainWindowResizeMode = ResizeMode.NoResize;
        #endregion

        public const string LogLevelInfo = "INFO";
        public const string LogLevelDebug = "DEBUG";

        //按钮样式路径
        public const string ButtonStyle = "/QuickAgent;component/Src/Style/ButtonStyle.xaml";

        //皮肤资源路径
        public const string RedStyle = "/QuickAgent;component/Src/Style/RedStyle.xaml";
        public const string BlueStyle = "/QuickAgent;component/Src/Style/BlueStyle.xaml";
        public const string YellowStyle = "/QuickAgent;component/Src/Style/YellowStyle.xaml";
        public const string GreenStyle = "/QuickAgent;component/Src/Style/GreenStyle.xaml";

        public const int LogInputMaxLength = 1000;

        //选择皮肤
        public const string SetRedStyle = "red";
        public const string SetBlueStyle = "blue";
        public const string SetYellowStyle = "yellow";
        public const string SetGreenStyle = "green";

        //提示框内容
        public const string HoldCall = "holdCall";

        public const string PhoneState_AgentHungUp = "2";
        public const string PhoneState_PlatformHungUp = "3";

        //座席状态
        public const string AgentStatus_SignOut = "2";

        //座席ID与状态之间插入的空格数
        public const int SpaceNum = 35;

        public const string TransferProtocolHttps = "https";

        #region  日志相关

        //  初始化
        public const string Initial = "Initial";
        //  进入工作态、退出工作态
        public const string EnterWork = "EnterWork";
        public const string ExitWork = "ExitWork";

        public const string Answer = "Answer";
        //  设置数据、查询数据
        public const string SetCallData = "SetCallData";
        public const string QryCallData = "QryCallData";
        //  呼叫转移
        public const string CallTrans = "CallTrans";
        //  转出
        public const string TransOut = "TransOut";
        public const string TransOutTalk = "TransOutTalk";
        //  静音、取消静音
        public const string Mute = "Mute";
        public const string EndMute = "EndMute";
        //  保持、取保持
        public const string Hold = "RequestHold";
        public const string CancelHold = "GetHold";
        public const string GetHoldTalk = "GetHoldTalk";
        //
        public const string TriCall = "TripartiteCall";
        //  内部呼叫
        public const string InterCall = "InternalCall";
        public const string InterCallTalk = "InternalCallTalk";
        //  内部求助、求助被拒、求助到达
        public const string InterHelp = "InternalHelp";
        public const string InterHelpRefuse = "InternalHelpRefused";
        public const string InterHelpArrive = "InternalHelpArrived";
        //  发送便签、发布公告、收到便签、收到公告
        public const string NotifyBulletin = "NotifyBulletin";
        public const string SendNote = "SendNote";
        public const string ReceNote = "ReceiveNote";
        public const string ReceBullet = "ReceiveBulletin";
        //  外呼
        public const string CallOut = "CallOut";
        public const string CallOutTalk = "CallOutTalk";
        //  示忙、示闲
        public const string SayFree = "SayFree";
        public const string SayBusy = "SayBusy";
        //  休息、取消休息、开始休息、休息超时
        public const string Rest = "Rest";
        public const string RestStart = "StartRest";
        public const string CancelRest = "CancelRest";
        public const string RestOverTime = "RestTimeOut";
        //  签入、签出、强制签出
        public const string SignIn = "SignIn";
        public const string SignOut = "SignOut";
        public const string ForceOut = "ForceOut";

        public const string Release = "ReleaseCall";
        public const string HoldCallRelease = "HoldCallRelease";
        public const string RequestRelease = "RequestRelease";
        public const string ForceRelease = "ForceRelease";

        public const string StartBusy = "StartBusy";

        public const string ForceBusy = "ForceBusy";

        public const string ForceIdle = "ForceIdle";

        //  三方通话
        public const string ConfJoin = "ConfJoin";
        //public const string ConfJoinTalk = "ConfJoinTalk";
        public const string CallerCalledInfoArrived = "CallerCalledInfoArrived";
        public const string DelCallInConf = "DelCallInConf";
        //  连接保持
        public const string ConnHold = "ConnectHold";
        //  座席状态
        public const string GetAgentStatus = "GetAgentStatus";
        public const string TalkChange = "TalkingChanged";
        public const string PhoneStatusNotify = "PhoneStatusNotify";
        public const string PhoneAlerting = "PhoneAlerting";
        public const string PhoneRinging = "PhoneRinging";
        public const string PhoneTalking = "PhoneTalking";
        public const string PhoneNoAnswer = "NoAnswer";
        //  技能重设
        public const string ResetSkill = "Reset Skills";

        // Voice.OCX
        public const string SetSipSerInfo = "SetSipServerInfo";
        public const string SetLocalInfo = "SetLocalInfo";
        public const string VoiceReg = "VoicePhoneRegister";

        #endregion
    }
}
