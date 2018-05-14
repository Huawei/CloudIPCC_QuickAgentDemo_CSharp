using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Entity.AgentGatewayEntity
{
    class RegAgentConfParm
    {
        public int memberNum { set; get; }

        public string confCallerNo { set; get; }

        public int time { set; get; }

        public int promptTime { set; get; }

        public bool beepTone { set; get; }

        public int playMode { set; get; }

        public string voicePath { set; get; }
    }


    class PlayVoiceParm
    {
        public int playType { set; get; }

        public int playMode { set; get; }

        public string voicePath { set; get; }
    }


    class ModifyConfResParm
    {
        public int modifyType { set; get; }

        public int modifyNum { set; get; }
    }
}
