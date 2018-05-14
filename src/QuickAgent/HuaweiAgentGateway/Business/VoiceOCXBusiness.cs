using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaweiAgentGateway.Entity.VoiceEntity;

namespace HuaweiAgentGateway
{
    /// <summary>
    /// Voice.OCX 类
    /// </summary>
    public class VoiceOCXBusiness
    {
        #region  属性

        /// <summary>
        /// voice instance
        /// </summary>
        private VoiceLib.Voice _voice { set; get; }

        public bool IsInitialSuc { private set; get; }
        public bool IsDispose
        {
            set{}
            get { return _voice == null; }
        }

        #endregion

        #region  构造函数

        public VoiceOCXBusiness()
        {
            if (null == _voice)
            {
                _voice = new VoiceLib.Voice();
            }
        }

        #endregion

        #region  其他方法

        public void AttachEvent(IBusinessEvents businessEvent)
        {
            if (null == _voice)
            {
                return;
            }
            _voice.VoiceTalkAlertingEvent += businessEvent.VoiceTalkAlertingEvent;
            _voice.VoiceTalkConnectedEvent += businessEvent.VoiceTalkConnectEvent;
            _voice.VoiceRegisterResultEvent += businessEvent.VoiceRegisterResultEvent;
            _voice.VoiceInitResultEvent += this.InitialResult;
            _voice.VoiceTalkReleaseEvent += businessEvent.VoiceTalkDisconnectEvent;
        }

        public void DetachEvents(IBusinessEvents businessEvent)
        {
            if (null == _voice)
            {
                return;
            }
            _voice.VoiceTalkAlertingEvent -= businessEvent.VoiceTalkAlertingEvent;
            _voice.VoiceTalkConnectedEvent -= businessEvent.VoiceTalkConnectEvent;
            _voice.VoiceRegisterResultEvent -= businessEvent.VoiceRegisterResultEvent;
            _voice.VoiceInitResultEvent -= this.InitialResult;
            _voice.VoiceTalkReleaseEvent -= businessEvent.VoiceTalkDisconnectEvent;
        }

        public string SetConfig(string key, string value)
        {
            if (null == _voice) return AGWErrorCode.Empty;
            return _voice.SetConfig(key, value) + "";
        }

        public string Call(string callee, bool isVideo)
        {
            if (null == _voice) return AGWErrorCode.Empty;
            var info = Utils.JsonUtil.ToJson(new VoiceCallData() { callee = callee, isVideo = isVideo });
            return _voice.CallEx(info) + "";
        }

        public string SetVideoWindow(string localHwd, string remoteHwd)
        {
            if (_voice == null) return AGWErrorCode.Empty;
            var info = Utils.JsonUtil.ToJson(new VideoWindowParm() { localVideoWindow = localHwd, remoteVideoWindow = remoteHwd });
            return _voice.SetVideoWindow(info) + "";
        }

        /// <summary>
        /// 设置本地的IP地址、SIP和Audio端口号，并设置音频端口范围
        /// 用户输入Audio端口值，则端口自动生成范围为[Audio端口值，Audio端口值+10]
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="sipPort"></param>
        /// <param name="audioPort"></param>
        /// <returns></returns>
        public string SetLocalInfo(string ip, string sipPort, string audioPort)
        {
            if (_voice == null) return AGWErrorCode.Empty;
            return _voice.SetLocalInfo(ip, sipPort, audioPort) + "";
        }

        /// <summary>
        /// 设置SIP服务器信息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="serIp"></param>
        /// <param name="backSerIp"></param>
        /// <returns></returns>
        public string SetSipServerInfo(string ip, string serIp, string backSerIp)
        {
            if (_voice == null) return AGWErrorCode.Empty;
            return _voice.SetSipServerInfo(ip, serIp, backSerIp) + "";
        }

        /// <summary>
        /// 电话号码注册
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <param name="pwd"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string Register(string phoneNum, string pwd, string mode)
        {
            if (_voice == null) return AGWErrorCode.Empty;
            return _voice.Register(phoneNum, pwd, mode) + "";
        }

        /// <summary>
        /// 电话号码注销
        /// </summary>
        /// <returns></returns>
        public string Deregister()
        {
            if (_voice == null) return AGWErrorCode.Empty;
            return _voice.Deregister() + "";
        }

        /// <summary>
        /// 接听来电
        /// </summary>
        /// <param name="callId"></param>
        /// <param name="isVideo"></param>
        /// <returns></returns>
        public string AnswerCall(string callId, bool isVideo)
        {
            if (_voice == null) return AGWErrorCode.Empty;
            var info = Utils.JsonUtil.ToJson(new VoiceAnswerParm() { callid = callId, isVideo = isVideo });
            return _voice.AnswerEx(info) + "";
        }

        public string Answer(string callId)
        {
            if (_voice == null) return AGWErrorCode.Empty;
            return _voice.Answer(callId) + "";
        }

        /// <summary>
        /// snap shot
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string SnapShot(string info)
        {
            if (null == _voice)
            {
                return AGWErrorCode.Empty;
            }
            return _voice.SnapShot(info) + "";
        }

        public string GetDeviceVideo()
        {
            if (null == _voice)
            {
                return AGWErrorCode.Empty;
            }
            return _voice.GetVideoDevices();
        }

        public void DisposeVoice()
        {
            if (_voice != null)
            {
                _voice = null;
                IsDispose = true;
            }
        }

        #endregion

        #region  本地事件

        private void InitialResult(string result)
        {
            var res = Utils.JsonUtil.DeJsonEx<Entity.VoiceEntity.VoiceResult>(result);
            IsInitialSuc = (res != null && res.resultCode == 0);
        }

        #endregion
    }
}
