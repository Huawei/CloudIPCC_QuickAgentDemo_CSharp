using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class AGWTransferData
    {
        public int devicetype { get; set; }

        public string address { get; set; }

        public int mode { get; set; }

        public string callappdata { get; set; }

        public string caller { get; set; }

        public int mediaability { get; set; }
    }


    public class AGWNotifyBullet
    {
        public int targettype { set; get; }

        public string bulletindata { set; get; }

        public string targetname { set; get; }
    }


    public class AGWSendNote 
    {
        public List<int> agentIds { set; get; }

        public string content { set; get; }

        public AGWSendNote() { }

        public AGWSendNote(int id, string msg)
        {
            if (null == agentIds)
                agentIds = new List<int>();
            agentIds.Clear();
            agentIds.Add(id);
            this.content = msg;
        }
    }
}
