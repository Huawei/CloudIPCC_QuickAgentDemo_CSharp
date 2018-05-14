using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class SignInParam
    {
        public string password { get; set; }

        public string phonenum { get; set; }

        public bool autoanswer { get; set; }

        public bool autoenteridle { get; set; }

        public bool releasephone { get; set; }

        public int agenttype { get; set; }

        public int status { get; set; }

        public string ip { get; set; }

        public string entryIp { get; set; }

        public bool phonelinkage { get; set; }

        public bool checkInMailm { get; set; }

        public string vcPhoneNumber { get; set; }
    }
}
