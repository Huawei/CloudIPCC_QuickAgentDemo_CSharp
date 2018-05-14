using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.VoiceEntity
{
    public class VoiceOCXAlertData
    {
        public string callid { set; get; }

        public string caller { set; get; }

        public string callType { set; get; }

        public bool isVideo { set; get; }
    }


    public class VoiceTalkingInfo
    {
        public string callee { set; get; }

        public string caller { set; get; }

        public string callidd { set; get; }
    }


    public class VoiceTalkReleaseParm
    {
        public string callid {set;get;}

        public string caller {set;get;}

        public string callee {set;get;}

        public bool isVideo {set;get;}

        public string resultCode {set;get;}

        public string resultDesc { set; get; }
    }


    public class VoiceCallData
    {
        public string callee { set; get; }

        public bool isVideo { set; get; }
    }
}
