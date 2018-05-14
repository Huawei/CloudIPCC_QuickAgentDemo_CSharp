using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class AGWSkill
    {
        public string name { get; set; }

        public string id { get; set; }

        public string mediatype { get; set; }
    }


    public class AGWVdnSkills
    {
        public string name {set;get;}

        public int id {set;get;}

        public int mediatype {set;get;}

        public int serviceType {set;get;}

        public int realFlag { set; get; }
    }
}
