using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickAgent.Src.Common;

namespace QuickAgent
{
    public class CommonText
    {
        public string StateInfo_Idle = LanguageResource.FindResourceMessageByKey("agentIdle");
        public string StateInfo_Busy = LanguageResource.FindResourceMessageByKey("agentBusy");
        public string StateInfo_ForceBusy = LanguageResource.FindResourceMessageByKey("agentForceBusy");
        public string StateInfo_Leave = LanguageResource.FindResourceMessageByKey("agentSignOut");
        public string StateInfo_Ringing = LanguageResource.FindResourceMessageByKey("phoneAlerting");
        public string StateInfo_WaitAnswer = LanguageResource.FindResourceMessageByKey("agentWaitAnswer");
        public string StateInfo_Recording = LanguageResource.FindResourceMessageByKey("agentRecord");
        public string StateInfo_Talking = LanguageResource.FindResourceMessageByKey("agentConver");
        public string StateInfo_Work = LanguageResource.FindResourceMessageByKey("agentWork");
        public string StateInfo_WaitRest = LanguageResource.FindResourceMessageByKey("agentWaitRest");
        public string StateInfo_EndBusy = LanguageResource.FindResourceMessageByKey("agentCancelBusy");
        public string StateInfo_EndRest = LanguageResource.FindResourceMessageByKey("agentEndRest");
        public string StateInfo_InnHelpSuc = LanguageResource.FindResourceMessageByKey("innerhelp_succ");
        public string StateInfo_InnHelpFal = LanguageResource.FindResourceMessageByKey("innerhelp_failed");
        public string StateInfo_InnHelpRef = LanguageResource.FindResourceMessageByKey("innerhelp_refused");
        public string StateInfo_InnHelpArr = LanguageResource.FindResourceMessageByKey("innerhelp_arrive");
        public string StateInfo_SendMsgSuc = LanguageResource.FindResourceMessageByKey("msgsend_succ");
        public string StateInfo_SendMsgFal = LanguageResource.FindResourceMessageByKey("msgsend_failed");
    }
}
