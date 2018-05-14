using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaweiAgentGateway;
using QuickAgent.View;
using QuickAgent.Constants;

namespace QuickAgent
{
    public class MainWindowCallBack : IBusinessEvents
    {

        private static MainWindow mainWindow = null;
        private static MainWindowCallBack _instance = null;
        public static MainWindowCallBack Instance(MainWindow main)
        {
            if (null == _instance)
            {
                _instance = new MainWindowCallBack();
                mainWindow = main;
            }
            return _instance;
        }

        public void DetachEvents()
        {
            BusinessAdapter.GetBusinessInstance().DetachEvents(this);
        }

        public void AnotherInService(string info)
        {
            
        }

        public void SignIn()
        {
            mainWindow.SignIn();
        }

        public void OnPhoneAlerting()
        {
            mainWindow.OnPhoneAlerting();
        }

        public void OnWaitAnswer(string callerNum, string calledNum)
        {
            mainWindow.OnWaitAnswer(callerNum, calledNum);
        }

        public void OnTalking(string data)
        {
            mainWindow.OnTalking(data);
        }

        public void ReleaseRequest()
        {
            mainWindow.ReleaseRequest();
        }

        public void OnCallRelease()
        {
            mainWindow.OnCallRelease();
        }

        public void OnNoAnswerFromCti()
        {
            mainWindow.OnNoAnswerFromCti();
        }

        public void OnHold()
        {
            mainWindow.OnHold();
        }

        public void OnSetWork()
        {
            mainWindow.OnSetWork();
        }

        public void OnCancelWork()
        {
            mainWindow.OnCancelWork();
        }

        public void OnSetReady()
        {
            mainWindow.StartIdle();
        }

        public void OnSetNotReady()
        {
            mainWindow.StartBusy();
        }

        public void OnCancelNotReady()
        {
            mainWindow.StartIdle();
        }

        public void OnTripartiteCall()
        {
            mainWindow.TripartiteCall();
        }

        public void OnReleaseResponse()
        {
            mainWindow.OnReleaseResponse();
        }

        public void OnSetRest()
        {
            mainWindow.StartRest();
        }

        public void OnRestTimeOut()
        {
            mainWindow.RestOverTime();
        }

        public void OnCancelRest()
        {
            mainWindow.OnCancelRest();
        }

        public void PreviewCallOut(string info)
        {
            mainWindow.PreviewCallOut(info);
        }

        public void AbandonCall(string info)
        {
            mainWindow.AbandonCall(info);
        }

        public void PhoneOffhook()
        {
            mainWindow.PhoneOffhook();
        }

        #region MCP Events

        #region  signIn/signOut

        public void SignInExSuccess(int mediaServer)
        {
            mainWindow.axcontrol_OnSignInExSuccess(mediaServer);
        }

        public void SignInExFailure(int mediaServer)
        {
            mainWindow.axcontrol_OnSignInExFailure(mediaServer);
        }

        public void SignOutExSuccess(int mediaServer)
        {
            mainWindow.axcontrol_OnSignOutExSuccess(mediaServer);
        }

        public void SignOutExFailure(int mediaServer)
        {
            mainWindow.axcontrol_OnSignInExFailure(mediaServer);
        }

        public void ForceOut(int usSuccess)
        {
            mainWindow.axcontrol_OnForceOut(usSuccess);
        }

        #endregion

        #region  inner Help

        public void InternalHelpSuccess(int helpMode)
        {
            mainWindow.InternalHelpEventHandle(false, Constant.InterHelp, true, MainWindow.AgentStatus.InternaHelp, new CommonText().StateInfo_InnHelpSuc);
        }

        public void InternalHelpFailure(int helpMode)
        {
            mainWindow.InnerHelpFailed();
            mainWindow.InternalHelpEventHandle(false, Constant.InterHelp, false, MainWindow.AgentStatus.Talking, new CommonText().StateInfo_InnHelpFal);
        }

        public void InternalHelpRefused()
        {
            mainWindow.InternalHelpEventHandle(true, Constant.InterHelpRefuse, false, MainWindow.AgentStatus.Talking, new CommonText().StateInfo_InnHelpRef);
        }

        public void InternalHelpArrived()
        {
            mainWindow.CommonEventHandle(Constant.InterHelpArrive, true, string.Empty, true, new CommonText().StateInfo_InnHelpArr);
        }

        #endregion

        #region  mute/UnMute

        public void BeginMuteUserSuccess()
        {
            mainWindow.axcontrol_OnBeginMuteUserSuccess();
        }

        public void BeginMuteUserFailure()
        {
            mainWindow.axcontrol_OnBeginMuteUserFailure();
        }

        public void EndMuteUserSuccess()
        {
            mainWindow.axcontrol_OnEndMuteUserSuccess();
        }

        public void EndMuteUserFailure()
        {
            mainWindow.axcontrol_OnEndMuteUserFailure();
        }

        #endregion

        #region  hold/UnHold

        public void GetHoldFailure()
        {
            mainWindow.axcontrol_OnGetHoldFailure();
        }

        public void GetHoldSuccess()
        {
            mainWindow.axcontrol_OnGetHoldSuccess();
        }

        public void HoldFailure()
        {
            mainWindow.axcontrol_OnHoldFailure();
        }

        public void HoldSuccess()
        {
            mainWindow.OnHold();
        }

        public void GetHoldSuccTalk()
        {
            mainWindow.axcontrol_OnGetHoldSuccTalk();
        }

        #endregion

        #region  rest/cancel rest

        public void StartRest()
        {
            mainWindow.StartRest();
        }

        public void RestExSuccess()
        {
            mainWindow.axcontrol_OnRestExSuccess();
        }

        public void RestExFailure()
        {
            mainWindow.axcontrol_OnRestExFailure();
        }

        public void RestTimeOut(int time)
        {
            mainWindow.RestOverTime();
        }

        public void CancelRestFailure()
        {
            mainWindow.axcontrol_OnCancelRestFailure();
        }

        public void CancelRestSuccess()
        {
            mainWindow.axcontrol_OnCancelRestSuccess();
        }

        #endregion

        #region  inner Call

        public void CallInnerSuccess()
        {
            mainWindow.axcontrol_OnCallInnerSuccess();
        }

        public void CallInnerFailure()
        {
            mainWindow.axcontrol_OnCallInnerFailure();
        }

        public void CallInnerSuccTalk()
        {
            mainWindow.axcontrol_OnCallInnerSuccTalk();
        }

        #endregion

        #region  call out

        public void CallOutSuccess()
        {
            mainWindow.axcontrol_OnCallOutSuccess();
        }

        public void CallOutFailure(int errCode)
        {
            mainWindow.axcontrol_OnCallOutFailure(errCode);
        }

        public void CallOutSuccTalkEx(int ulTime)
        {
            mainWindow.axcontrol_OnCallOutSuccTalkEx(ulTime);
        }

        #endregion

        #region  send Message

        public void SendMessageSuccess(int sendToWorkNo)
        {
            mainWindow.SendMsgSucc(sendToWorkNo);
        }

        public void SendMessageFailure(int sendToWorkNo)
        {
            mainWindow.SendMsgFailure(sendToWorkNo);
        }

        public void ReceiveMessageEx(int agentId, string content)
        {
            mainWindow.ReceiveMsgOrBullet(agentId, content, Constant.ReceNote);
        }

        #endregion

        #region  Transfer Call

        public void TransInnerFailure()
        {
            mainWindow.axcontrol_OnTransInnerFailure();
        }

        public void TransInnerSuccess()
        {
            mainWindow.axcontrol_OnTransInnerSuccess();
        }

        public void RedirectToOtherFailure()
        {
            mainWindow.axcontrol_OnRedirectToOtherFailure();
        }

        public void RedirectToOtherSuccess()
        {
            mainWindow.axcontrol_OnRedirectToOtherSuccess();
        }

        public void RedirectToAutoSuccess()
        {
            mainWindow.axcontrol_OnRedirectToAutoSuccess();
        }

        public void RedirectToAutoFailure()
        {
            mainWindow.axcontrol_OnRedirectToAutoFailure();
        }

        #endregion

        #region  connect Hold

        public void IsTalkingChanged()
        {
            mainWindow.axcontrol_OnIsTalkingChanged();
        }

        public void ConnectHoldSuccess()
        {
            mainWindow.axcontrol_OnConnectHoldSuccess();
        }

        public void ConnectHoldFailure()
        {
            mainWindow.axcontrol_OnConnectHoldFailure();
        }

        #endregion

        #region  ConfJoin

        public void ConfJoinSuccess()
        {
            mainWindow.axcontrol_OnConfJoinSuccess();
        }

        public void ConfJoinSuccTalk()
        {
            mainWindow.TripartiteCall();
        }

        public void ConfJoinFailure()
        {
            mainWindow.axcontrol_OnConfJoinFailure();
        }

        public void CallerCalledInfoArrived(int MediaType, string Caller, string Called)
        {
            mainWindow.axcontrol_OnCallerCalledInfoArrived(MediaType, Caller, Called);
        }

        public void DelCallInConf(string CallerNo, string CalledNo)
        {
            mainWindow.axcontrol_OnDelCallInConf(CallerNo, CalledNo);
        }

        #endregion

        #region  receive NotifyBullet

        public void ReceiveBulletinMsgEx(int id, string msg)
        {
            mainWindow.ReceiveMsgOrBullet(id, msg, Constant.ReceBullet);
        }

        #endregion

        #region  trans out

        public void TransOutSuccess(int mode)
        {
            mainWindow.axcontrol_OnTransOutSuccess(mode);
        }

        public void TransOutFailure(int mode)
        {
            mainWindow.axcontrol_OnTransOutFailure(mode);
        }

        public void TransOutFailTalk(int mode)
        {
            mainWindow.axcontrol_OnTransOutFailTalk(mode);
        }

        public void TransOutSuccTalk(int mode)
        {
            mainWindow.axcontrol_OnTransOutSuccTalk(mode);
        }

        #endregion

        #region busy/free

        public void SayBusySuccess()
        {
            mainWindow.axcontrol_OnSayBusySuccess();
        }

        public void SayBusyFailure()
        {
            mainWindow.axcontrol_OnSayBusyFailure();
        }

        public void SayFreeSuccess()
        {
            mainWindow.axcontrol_OnSayFreeSuccess();
        }
        public void SayFreeFailure()
        {
            mainWindow.axcontrol_OnSayFreeFailure();
        }

        public void StartBusy()
        {
            mainWindow.StartBusy();
        }

        public void ForceBusy()
        {
            mainWindow.StartBusy(Constant.ForceBusy);
        }

        public void ForceIdle()
        {
            mainWindow.StartIdle(Constant.ForceBusy);
        }

        #endregion

        #region answer/release

        //public void AnswerSuccess()
        //{
        //    mainWindow.axcontrol_OnAnswerSuccess();
        //}

        //public void AnswerExSuccess(int mediaType)
        //{
        //    mainWindow.axcontrol_OnAnswerExSuccess(mediaType);
        //}

        public void OnReceiveAgentStateInfo(int state, int busyFlg, int restFlg)
        {
            mainWindow.OnReceiveAgentStateInfo(state, busyFlg, restFlg);
        }

        public void HoldCallRelease(int cause)
        {
            mainWindow.axcontrol_HoldCallRelease();
        }

        public void ForceRelease()
        {
            mainWindow.axcontrol_OnForceRelease();
        }

        public void AnswerFailure()
        {
            mainWindow.axcontrol_OnAnswerFailure();
        }

        public void AnswerExFailure(int mediaType)
        {
            mainWindow.axcontrol_OnAnswerExFailure(mediaType);
        }

        public void RequestReleaseEx(int mediaType)
        {
            mainWindow.axcontrol_OnRequestReleaseEx(mediaType);
        }

        public void ReleaseExSuccess(int mediaType)
        {
            mainWindow.axcontrol_OnReleaseExSuccess(mediaType);
        }

        public void ReleaseExFailure(int mediaType)
        {
            mainWindow.axcontrol_OnReleaseExSuccess(mediaType);
        }

        public void PhoneStatusNotify(int status)
        {
            mainWindow.axcontrol_OnPhoneStatusNotify(status);
        }

        #endregion

        #endregion

        #region  Voice.OCX Events

        /// <summary>
        /// 振铃
        /// </summary>
        /// <param name="sResult"></param>
        public void VoiceTalkAlertingEvent(string sResult)
        {
            mainWindow.VoiceTalk(sResult);
        }

        /// <summary>
        /// 建立通话
        /// </summary>
        /// <param name="sResult"></param>
        public void VoiceTalkConnectEvent(string sResult)
        {
            mainWindow.VoiceTalking(sResult);
        }

        public void VoiceRegisterResultEvent(string result)
        {
            mainWindow.VoiceRegisterResult(result);
        }

        public void VoiceTalkDisconnectEvent(string info)
        {
            mainWindow.VoiceTalkDisconnected(info);
        }

        #endregion

        #region  电话会议事件

        /// <summary>
        /// 申请会议成功
        /// </summary>
        public void RequestAgentConfSucc()
        {

        }

        /// <summary>
        /// 申请会议失败
        /// </summary>
        public void RequestAgentConfFail()
        {

        }

        /// <summary>
        /// 加入会议成功
        /// </summary>
        public void JoinAgentConf()
        {

        }

        /// <summary>
        /// 与会者变更
        /// </summary>
        public void AgentConfParticipantChanged()
        {

        }

        /// <summary>
        /// 退出会议
        /// </summary>
        public void LeaveAgentConf()
        {

        }

        #endregion
    }
}
