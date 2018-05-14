using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class CallInfoBeforeAnswer
    {
        public string callfeature { get; set; }

        public string callid { get; set; }

        public string caller { get; set; }

        public string called { get; set; }

        public string callskill { get; set; }

        public string callskillid { get; set; }

        public string orgicallednum { get; set; }

        public string calldata { get; set; }

        public string begintime { get; set; }

        public string endtime { get; set; }

        public string userPriority { get; set; }

        public string trunkNo { get; set; }

        public string logontimes { get; set; }
    }
}
