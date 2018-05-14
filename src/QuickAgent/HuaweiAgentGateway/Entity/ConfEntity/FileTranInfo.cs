using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.ConfEntity
{
    public class FileTranInfo
    {
        public string fileHandle { set; get; }

        public string fileName { set; get; }

        public string fileProgress { set; get; }

        public string fileSize { set; get; }

        public string fileStatus { set; get; }

        public string resultCode { set; get; }

        public string resultDesc { set; get; }

        public string senderId { set; get; }
    }
}
