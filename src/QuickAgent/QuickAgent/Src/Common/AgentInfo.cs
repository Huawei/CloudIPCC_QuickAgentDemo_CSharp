using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuickAgent.Common
{
    /// <summary>
    /// 座席信息
    /// Voice.OCX 接口不能传入 null 值，因此密码默认为空
    /// </summary>
    public class AgentInfo
    {
        public string AgentId { get; set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        // voice.ocx
        public string VoicePassword = string.Empty;
    }


    /// <summary>
    /// log parm that shown on textchat window
    /// </summary>
    public class LogShowParm
    {
        /// <summary>
        /// current time
        /// </summary>
        public string LogTime { set; get; }

        /// <summary>
        /// agentgateway event type
        /// </summary>
        public string LogType { set; get; }

        /// <summary>
        /// agentgateway event info
        /// </summary>
        public string LogInfo { set; get; }
    }
}
