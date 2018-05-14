using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.ConfEntity
{
    public class MessageInfo
    {
        public string data { set; get; }

        public int dataLen { set; get; }

        public int msgId { set; get; }

        public int senderId { set; get; }
    }
}
