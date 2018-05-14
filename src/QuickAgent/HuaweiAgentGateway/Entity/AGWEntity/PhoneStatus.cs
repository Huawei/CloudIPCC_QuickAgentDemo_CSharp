using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public enum PhoneStatus
    {
        AGENT_PHONE_ALERTING = 0,           

        AGENT_PHONE_CALLEDOFFHOOK = 1,    

        AGENT_PHONE_RELEASE = 2,          

        AGENT_PHONE_CTI_AUTO_RELEASE = 3,   
    }
}
