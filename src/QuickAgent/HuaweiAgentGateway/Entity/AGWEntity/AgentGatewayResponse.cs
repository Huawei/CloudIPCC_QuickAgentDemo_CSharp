using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.AgentGatewayEntity
{
    public class AgentGatewayResponse
    {
        public string message { get; set; }

        public string retcode { get; set; }
    }


    public class AgentGatewayResponse<T>
    {
        public string message { get; set; }

        public string retcode { get; set; }

        public T result { get; set; }
    }


    public class AGWEventDataRes<T>
    {
        public string message { set; get; }

        public string retcode { set; get; }

        public T @event { set; get; }
    }


    public class AGWEventData<T>
    {
        public string eventType { set; get; }

        public int workNo { set; get; }

        public T content { set; get; }
    }


    public class AGWNotesData
    {
        public string sender {set;get;}

        public string receivetime {set;get;}

        public string sendername {set;get;}

        public string timestring {set;get;}

        public string context { set; get; }
    }


    public class AGWWorkGroupData
    {
        public int id { set; get; }

        public string name { set; get; }

        public int monitorno { set; get; }

        public string monitorname { set; get; }
    }


    [Serializable]
    public class AgentGatewayException : Exception
    {
        public string ErrorCode { get; set; }

        public string ErrorMsg { get; set; }
    }
}
