using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.AgentGatewayEntity
{
    public class ReqMultiMediaConfParm
    {
        public string callid { set; get; }

        public string addressinfo { set; get; }
    }


    class JoinMultiMediaConfResParm
    {
        public int confid { set; get; }

        public int confresult { set; get; }

        public int cause { set; get; }
    }


    class EndMultiMediaConfParm
    {
        public int confid { set; get; }
    }


    class QryMultiMediaConfParm
    {
        public string addressinfo { set; get; }
    }


    public class ConfInviteParm
    {
 
    }
}
