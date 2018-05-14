using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.VoiceEntity
{
    public class VoiceOCXConnectData
    {
        public string callid { set; get; }

        public string caller { set; get; }

        public string callee { set; get; }

        public bool isVideo { set; get; }
    }


    public class VoiceAnswerParm
    {
        public string callid { set; get; }

        public bool isVideo { set; get; }
    }
}
