using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.AgentGatewayEntity
{
    public class AGWVDNInfo
    {
        public string description { set; get; }

        public int id { set; get; }

        public string inno { set; get; }

        public int vdnid { set; get; }

        public int mediatype { set; get; }
    }

    /// <summary>
    /// work gourp info
    /// </summary>
    public class AgwWorkGroupDetail
    {
        /// <summary>
        /// gourp name
        /// </summary>
        public string name {set;get;}

        /// <summary>
        /// gourp id
        /// </summary>
        public int id {set;get;}

        /// <summary>
        /// monitor no
        /// </summary>
        public int monitorno {set;get;}

        /// <summary>
        /// monitor name
        /// </summary>
        public string monitorname { set; get; }
    }
}
