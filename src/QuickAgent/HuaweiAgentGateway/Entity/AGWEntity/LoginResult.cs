using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaweiAgentGateway.AgentGatewayEntity;

namespace HuaweiAgentGateway
{
    public class LoginResult:ServiceResponseBase
    {
        public string vdnid { get; set; }

        public string workno { get; set; }

        public string mediatype { get; set; }

        public string loginTime { get; set; }

        public string isForceChange { get; set; }
    }
}
