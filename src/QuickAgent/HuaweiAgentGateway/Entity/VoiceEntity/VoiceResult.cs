using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HuaweiAgentGateway.Utils;

namespace HuaweiAgentGateway.Entity.VoiceEntity
{
    public class VoiceResult
    {
        public int resultCode { set; get; }

        public string resultDesc { set; get; }
    }


    public class VoiceRegisterResult : VoiceResult
    {
        public string telephone { set; get; }

        public string name { set; get; }
    }


    public class VoiceSnapShot
    {
        public string deviceIndex { set; get; }

        public string width { set; get; }

        public string height { set; get; }

        public string filePath { set; get; }
    }


    public class DeviceItem
    {
        public string deviceIndex { set; get; }

        public string deviceName { set; get; }
    }


    public class DeviceList
    {
        public int deviceNum { set; get; }

        public int resultCode { set; get; }

        public object deviceList { set; get; }


        public List<DeviceItem> GetDeviceList()
        {
            if (null == deviceList || 0 == deviceNum || 0 != resultCode)
            {
                return null;
            }

            return JsonUtil.DeJson<List<DeviceItem>>(deviceList.ToString());
        }
    }
}
