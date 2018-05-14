using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.AgentGatewayEntity
{
    public class AGWAgentInfo
    {
        public string workno { set; get; }

        public string name { set; get; }

        public string status { set; get; }

        public string ctiStatus { set; get; }

        public string groupid { set; get; }

        public string groupname { set; get; }

        public object skilllist { set; get; }

        public string mediatype { set; get; }

        public int vdnid { set; get; }

        public string phonenumber { set; get; }

        public long currentstatetime { set; get; }

        public long logindate { set; get; }

        public int inMultimediaConf { set; get; }
    }
}
