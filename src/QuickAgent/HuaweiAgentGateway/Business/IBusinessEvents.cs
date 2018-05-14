using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public interface IBusinessEvents
    {
        //座席签入成功
        void SignIn();

        //话机振铃
        void OnPhoneAlerting();

        //等待应答
        void OnWaitAnswer(string callerNum, string calledNum);

        //通话中
        void OnTalking(string data);

        void ReleaseRequest();

        //客户退出呼叫
        void OnCallRelease();

        //CTI平台久不应答
        void OnNoAnswerFromCti();

        //呼叫保持
        void OnHold();

        //进入工作态
        void OnSetWork();

        //退出工作态
        void OnCancelWork();

        //示闲
        void OnSetReady();

        //示忙
        void OnSetNotReady();

        //示闲
        void OnCancelNotReady();

        //三方通话
        void OnTripartiteCall();

        //释放响应事件
        void OnReleaseResponse();

        //请求休息成功
        void OnSetRest();

        //已经超过休息时间
        void OnRestTimeOut();

        //取消休息成功
        void OnCancelRest();

        //释放资源
        void DetachEvents();

        void PreviewCallOut(string info);

        void AbandonCall(string info);

        void PhoneOffhook();    

        #region events used for ocx

        #region  signIn/signOut

        void SignInExSuccess(int mediaServer);

        void SignInExFailure(int mediaServer);

        void SignOutExSuccess(int mediaServer);

        void SignOutExFailure(int mediaServer);

        void ForceOut(int usSuccess);

        #endregion

        #region  inner Help

        void InternalHelpSuccess(int helpMode);

        void InternalHelpFailure(int helpMode);

        void InternalHelpRefused();

        void InternalHelpArrived();

        #endregion

        #region  mute/UnMute

        void BeginMuteUserSuccess();

        void BeginMuteUserFailure();

        void EndMuteUserSuccess();

        void EndMuteUserFailure();

        #endregion

        #region  hold/UnHold

        void GetHoldFailure();

        void GetHoldSuccess();

        void HoldFailure();

        void HoldSuccess();

        void GetHoldSuccTalk();

        #endregion

        #region  rest/cancel rest

        void RestExSuccess();

        void StartRest();

        void RestExFailure();

        void CancelRestFailure();

        #endregion

        #region  inner Call

        void CallInnerSuccess();

        void CallInnerFailure();

        void CallInnerSuccTalk();

        #endregion

        #region  call out

        void CallOutSuccess();

        void CallOutFailure(int errCode);

        void CallOutSuccTalkEx(int ulTime);

        #endregion

        #region  send Message

        void SendMessageSuccess(int sendToWorkNo);

        void SendMessageFailure(int sendToWorkNo);

        void ReceiveMessageEx(int agentId, string content);

        #endregion

        #region  Transfer Call

        void TransInnerFailure();

        void TransInnerSuccess();

        void RedirectToOtherFailure();

        void RedirectToOtherSuccess();

        void RedirectToAutoSuccess();

        void RedirectToAutoFailure();

        #endregion

        #region  connect Hold

        void IsTalkingChanged();

        void ConnectHoldSuccess();

        void ConnectHoldFailure();

        #endregion

        #region  ConfJoin

        void ConfJoinSuccess();

        void ConfJoinSuccTalk();

        void ConfJoinFailure();

        void CallerCalledInfoArrived(int MediaType, string Caller, string Called);

        void DelCallInConf(string CallerNo, string CalledNo);

        #endregion

        #region  receive NotifyBullet

        void ReceiveBulletinMsgEx(int id, string msg);

        #endregion

        #region  trans out

        void TransOutSuccess(int mode);

        void TransOutFailure(int mode);

        void TransOutFailTalk(int mode);

        void TransOutSuccTalk(int mode);

        #endregion

        #region busy/free

        void SayBusySuccess();

        void SayBusyFailure();

        void SayFreeSuccess();

        void SayFreeFailure();

        void StartBusy();

        void ForceBusy();

        void ForceIdle();

        #endregion

        #region answer/release

        void OnReceiveAgentStateInfo(int AgentState, int BusyFlg, int RestFlg);

        void HoldCallRelease(int cause);

        void AnswerFailure();

        void AnswerExFailure(int mediaType);

        void RequestReleaseEx(int mediaType);

        void ReleaseExSuccess(int mediaType);

        void ReleaseExFailure(int mediaType);

        void ForceRelease();

        void PhoneStatusNotify(int status);

        #endregion

        #region  Voice.ocx Events

        void VoiceTalkAlertingEvent(string sResult);

        void VoiceTalkConnectEvent(string result);

        void VoiceTalkDisconnectEvent(string info);

        void VoiceRegisterResultEvent(string result);

        #endregion

        #endregion

        #region  电话会议事件

        /// <summary>
        /// 申请会议成功
        /// </summary>
        void RequestAgentConfSucc();

        /// <summary>
        /// 申请会议失败
        /// </summary>
        void RequestAgentConfFail();

        /// <summary>
        /// 加入会议成功
        /// </summary>
        void JoinAgentConf();

        /// <summary>
        /// 与会者变更
        /// </summary>
        void AgentConfParticipantChanged();

        /// <summary>
        /// 退出会议
        /// </summary>
        void LeaveAgentConf();

        #endregion
    }
}
