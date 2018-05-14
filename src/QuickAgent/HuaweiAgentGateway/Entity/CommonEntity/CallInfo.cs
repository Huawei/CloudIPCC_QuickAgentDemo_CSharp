using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public class CallInfo
    {
        public string Caller { set; get; }

        public string Called { set; get; }

        public string CallId { set; get; }

        public string MediaType { set; get; }
    }
}
