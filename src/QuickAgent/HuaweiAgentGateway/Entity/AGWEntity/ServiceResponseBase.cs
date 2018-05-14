using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class ServiceResponseBase
    {
        public string message { get; set; }

        public string retcode { get; set; }
    }


    public class ServiceResponseBase<T>
    {
        public string message { get; set; }

        public string retcode { get; set; }

        public T result { get; set; }
    }
}
