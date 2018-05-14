using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.ConfEntity
{
    public class LocalVideo
    {
        public int resultCode { set; get; }

        public string resultDesc { set; get; }

        public int deviceCount { set; get; }

        public List<VideoDetail> devicesInfo { set; get; }
    }


    public class VideoDetail
    {
        public int deviceId { set; get; }

        public string name { set; get; }
    }


    public class VideoParm
    {
        public string X { set; get; }

        public string Y { set; get; }

        public string frameRate { set; get; }
    }


    public class VideoSupDev 
    {
        public int capbilityCount { set; get; }

        public string resultCode { set; get; }

        public string resultDesc { set; get; }

        public List<VideoParm> SupportVideoParam { set; get; }
    }


    public class VideoCurDev : VideoParm
    {
        public string resultCode { set; get; }

        public string resultDesc { set; get; }
    }
}
