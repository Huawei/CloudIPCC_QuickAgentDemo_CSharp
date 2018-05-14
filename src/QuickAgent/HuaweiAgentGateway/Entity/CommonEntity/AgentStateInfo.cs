using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public class AgentStateInfo
    {
        public string workno { get; set; }

        public string status { get; set; }

        public string name { get; set; }

        public string groupName { set; get; }

        /// <summary>
        /// used for agw only
        /// </summary>
        public string ctiStatus { set; get; }
    }
}
