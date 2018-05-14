using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class AGWCallData
    {
        public string caller { get; set; }

        public string called { get; set; }

        public int skillid { get; set; }

        public string callappdata { get; set; }

        public long callcontrolid { get; set; }

        public int mediaability { get; set; }
    }


    public class AGWCallInfo
    {
        public int callfeature {set;get;}

        public string callid {set;get;}

        public string caller {set;get;}

        public string callskill {set;get;}

        public int callskillid {set;get;}

        public string orgicallednum {set;get;}

        public string calldata {set;get;}

        public string called {set;get;}

        public object begintime{set;get;}

        public object endtime { set; get; }
    }


    public class AGWCallInfoParm
    {
        public string called { set; get; }

        public string caller { set; get; }

        public string callid { set; get; }

        public string type { set; get; }

        public string feature { set; get; }

        public string mediaAbility { set; get; }
    }


    public class AgwPreviewCallOutParm
    {
        public string caller {set;get;}

        public string called {set;get;}

        public int skillid {set;get;}

        public string callappdata {set;get;}

        public string callcontrolid {set;get;}

        public int mediaability{ set; get; }
    }


    public class AgwEventPreview
    {
        public string controlid { set; get; }

        public string dialeddigits { set; get; }
    }


    public class AbandonCallParm
    {
        public string caller {set;get;}

        public string callData {set;get;}

        public string callId { set; get; }
    }
}
