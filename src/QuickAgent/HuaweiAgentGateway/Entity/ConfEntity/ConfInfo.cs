using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.ConfEntity
{
    public class ConfInfo
    {
        public int confId { set; get; }

        public int resultCode { set; get; }

        public string resultDesc { set; get; }
    }


    public class BaseResponse
    {
        public string resultCode { set; get; }

        public string resultDesc { set; get; }
    }


    public class PresenterInfo : BaseResponse
    {
        public bool isSelf { set; get; }

        public string userId { set; get; }
    }


    public class AgwConfRes<T>
    {
        public string message { set; get; }

        public string retcode { set; get; }

        public AgwEvent<T> @event { set; get; }
    }


    public class AgwEvent<T>
    {
        public string eventType { set; get; }

        public string workNo { set; get; }

        public T content { set; get; }
    }


    public class AgwConfInfoParm
    {
        public string vdnid { set; get; }

        public string addresstype { set; get; }

        public int confid { set; get; }

        public string confinfo { set; get; }

        public string address { set; get; }
    }


    public class AgwConfStatusInfo
    {
        public string vdnid { set; get; }

        public string addresstype { set; get; }

        public string cause { set; get; }

        public string confid { set; get; }

        public string confstate { set; get; }
    }


    public class AgwConfEnd
    {
        public string confid { set; get; }
    }
}
