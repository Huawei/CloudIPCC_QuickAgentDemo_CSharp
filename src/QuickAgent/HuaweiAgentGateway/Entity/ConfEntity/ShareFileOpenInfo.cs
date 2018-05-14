using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.ConfEntity
{
    public class ShareFileOpenInfo
    {
        public string resultCode { set; get; }

        public string resultDesc { set; get; }

        public string fileName { set; get; }

        public string owner { set; get; }

        public string fileId { set; get; }
    }


    public class ShareFileCloseInfo
    {
        public string fileId { set; get; }

        public string resultCode { set; get; }

        public string resultDesc { set; get; }
    }


    public class OperPriReqParm
    {
        public string userId { set; get; }

        public string privilegeType { set; get; }
    }


    public class ShareScreenStateData
    {
        public string state { set; get; }

        public string shareType { set; get; }
    }
}
