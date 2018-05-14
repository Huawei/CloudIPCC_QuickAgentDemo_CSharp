using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.VoiceEntity
{
    public class SipInfo
    {
        public string ip { set; get; }

        public string port { set; get; }
    }


    public class VideoWindowParm
    {
        public string localVideoWindow { set; get; }

        public string remoteVideoWindow { set; get; }
    }
}
