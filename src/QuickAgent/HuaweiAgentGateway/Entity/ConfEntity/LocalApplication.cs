using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.ConfEntity
{
    public class LocalApplication
    {
        public int resultCode { set; get; }

        public string resultDesc { set; get; }

        public int count { set; get; }

        public List<AppDetail> appInfoList { set; get; }
    }


    public class AppDetail
    {
        public int hwnd { set; get; }

        public int sharing { set; get; }

        public string title { set; get; }
    }
}
