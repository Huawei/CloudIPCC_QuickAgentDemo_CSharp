using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public class AgwCommonRes<T>
    {
        public string retcode { set; get; }

        public string message { set; get; }

        public T @event { set; get; }
    }


    public class AgentEventResult<T>
    {
        public string eventType { get; set; }

        public string workNo { get; set; }

        public T content { get; set; }
    }


    public class AgentEventData : EventArgs
    {
        public string CallBackFunctionName { get; set; }

        public bool IsSuccess { get; set; }

        public string Data { get; set; }
    }
}
