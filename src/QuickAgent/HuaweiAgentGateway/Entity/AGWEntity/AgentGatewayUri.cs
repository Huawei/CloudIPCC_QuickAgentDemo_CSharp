using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HuaweiAgentGateway
{
    public class AgentGatewayUri
    {
        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string ONLINEAGENT_URI = "/onlineagent/{0}";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string OnlineAgentForceLogin_URI = "/onlineagent/{0}/forcelogin";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string SAYBUSY_URI = "/onlineagent/{0}/saybusy";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string AGENTEVENT_URI = "/agentevent/{0}";

        public static string ANSWER_URI = "/voicecall/{0}/answer";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string RELEASE_URI = "/voicecall/{0}/release";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string SAYFREE_URI = "/onlineagent/{0}/sayfree";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string CANCELBUSY_URI = "/onlineagent/{0}/cancelbusy";

        /// <summary>
        /// 重置技能队列
        /// </summary>
        public static string RESETSKILL_URI = "/onlineagent/{0}/resetskill/{1}?skillid={2}&phonelinkage={3}";

        public static string FORCELOGOUT_URI = "/onlineagent/{0}/forcelogout";

        /// <summary>
        /// {0}:AgentID
        /// {1}:restTime
        /// {2}:restReason
        /// </summary>
        public static string REST_URI = "/onlineagent/{0}/rest/{1}/{2}";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string CANCELREST_URI = "/onlineagent/{0}/cancelrest";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string CallOut_URI = "/voicecall/{0}/callout";

        /// <summary>
        /// 查询应答前呼叫信息
        /// </summary>
        public static string CallDataCallInfoBeforeAnswer_URI = "/calldata/{0}/callinfobeforeanswer";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string PhonePickup_URI = "/voicecall/{0}/phonepickup";

        /// <summary>
        /// 查询呼叫信息
        /// {0}:座席工号
        /// </summary>
        public static string CallDataCallInfo_URI = "/calldata/{0}/callinfo";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string BeginMute_URI = "/voicecall/{0}/beginmute";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string EndMute_URI = "/voicecall/{0}/endmute";

        /// <summary>
        ///  {0}:AgentID
        /// </summary>
        public static string HOLD_URI = "/voicecall/{0}/hold";

        /// <summary>
        /// {0}:AgentID
        /// {1}:callid
        /// </summary>
        public static string GETHOLD_URI = "/voicecall/{0}/gethold?callid={1}";

        /// <summary>
        /// 查询保持呼叫信息
        /// </summary>
        public static string HOLDLIST_URI = "/calldata/{0}/holdlist";

        public static string CANCELWORK_URI = "/onlineagent/{0}/cancelwork";

        public static string WORK_URI = "/onlineagent/{0}/work";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string CallInner_URI = "/voicecall/{0}/callinner";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string AgentSkills_URI = "/onlineagent/{0}/agentskills";

        public static string AgentStatusInMyVdn_URI = "/agentgroup/{0}/allagentstatus";

        /// <summary>
        /// 获取VDN接入码信息
        /// {0}:座席工号
        /// </summary>
        public static string QueueDevice_VDNAccessCodeInfo_URI = "/queuedevice/{0}/innoinfo";


        /// <summary>
        /// 获取座席所在VDN的IVR信息
        /// {0}:座席工号
        /// </summary>
        public static string QueueDevice_IVRInfo_URI = "/queuedevice/{0}/ivrinfo";

        /// <summary>
        /// 获取座席所在VDN的技能队列
        /// {0}:座席工号
        /// </summary>
        public static string QueueDevice_AgentVDNSkills_URI = "/queuedevice/{0}/agentvdnskill";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string Transfer_URI = "/voicecall/{0}/transfer";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string AgentStatus_URI = "/onlineagent/{0}/agentstatus";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string InnerHelp_URI = "/voicecall/{0}/innerhelp";

        /// <summary>
        /// {0}:AgentID
        /// </summary>
        public static string ConfJoin_URI = "/voicecall/{0}/confjoin";

        /// <summary>
        /// {0}:AgentID
        /// {1}:callid
        /// </summary>
        public static string ConnectHold_URI = "/voicecall/{0}/connecthold/{1}";

        /// <summary>
        /// 发布公告
        /// {0}:AgentID
        /// </summary>
        public static string NotifyBullet = "/onlineagent/{0}/notifybulletin";

        /// <summary>
        /// 发送便签
        /// 0}:AgentID
        /// </summary>
        public static string SendNote = "/onlineagent/{0}/sendnoteletex";

        /// <summary>
        /// 设置数据
        /// {0}:AgentID
        /// {1}:CallData
        /// {2}:CallId
        /// </summary>
        public static string SetData = "/calldata/{0}?calldata={1}&callid={2}";

        /// <summary>
        /// 查询数据
        /// {0}:AgentID
        /// </summary>
        public static string QryData = "/calldata/{0}/appdata";

        /// <summary>
        /// 二次拨号
        /// {0}:AgentID
        /// {1}:Number
        /// </summary>
        public static string SecDial = "/voicecall/{0}/seconddial/{1}";

        /// <summary>
        /// 查询班组信息
        /// {0}:AgentID
        /// </summary>
        public static string QryWorkGroup = "/agentgroup/{0}/grouponvdn";

        /// <summary>
        /// 根据 callid 获取呼叫信息
        /// {0}:AgentID
        /// {1}:CallID
        /// </summary>
        public static string GetCallInfoById = "/calldata/{0}/callinfobycallid/{1}";

        /// <summary>
        /// 获取所有的 callid 信息
        /// {0}:AgentID
        /// </summary>
        public static string GetCallIDs = "/calldata/{0}/allcallid";

        /// <summary>
        /// 设置是否进入空闲态
        /// {0}:AgentID
        /// {1}:为是否进入空闲态标志位（true为自动进入空闲态，其他非true字符串为进入整理态）。
        /// </summary>
        public static string SetAutoEnterIdle = "/onlineagent/{0}/autoenteridle/{1}";

        /// <summary>
        /// 取消转移
        /// {0}:AgentID
        /// </summary>
        public static string CancelTrans = "/voicecall/{0}/canceltransfer";

        #region  电话会议相关 url

        /// <summary>
        /// 申请电话会议
        /// 0}:AgentID
        /// </summary>
        public static string RequestAgentConf = "/agentconf/{0}/requestagentconf";

        /// <summary>
        /// 座席应答会议
        /// {0}:AgentID
        /// </summary>
        public static string AnswerAgentConf = "/agentconf/{0}/answeragentconf";

        /// <summary>
        /// 座席拒接会议
        /// {0}:AgentID
        /// </summary>
        public static string RejectAgentConf = "/agentconf/{0}/rejectagentconf";

        /// <summary>
        /// 退出电话会议
        /// {0}:AgentID
        /// </summary>
        public static string LeaveAgentConf = "/agentconf/{0}/requestleaveagentconf";

        /// <summary>
        /// 释放电话会议
        /// {0}:AgentID
        /// </summary>
        public static string ReleaseAgentConf = "/agentconf/{0}/releaseagentconf";

        /// <summary>
        /// 主席申请延长会议时间
        /// {0}:AgentID
        /// {1}:延长的会议时长
        /// </summary>
        public static string ProlongTime = "/agentconf/{0}/prolongtime?prolongTime={1}";

        /// <summary>
        /// 转移主席权限
        /// {0}:AgentID
        /// {1}:目标座席
        /// </summary>
        public static string ChangePresident = "/agentconf/{0}/applytoshiftpresident?destWorkNo={1}";

        /// <summary>
        /// 电话会议放音
        /// {0}:AgentID
        /// </summary>
        public static string PlayVoiceConf = "/agentconf/{0}/playvoicetoconf";

        /// <summary>
        /// 停止电话会议放音
        /// {0}:AgentID
        /// </summary>
        public static string StopVoiceConf = "/agentconf/{0}/stopvoicetoconf";

        /// <summary>
        /// 改变会议资源方数
        /// {0}:AgentID
        /// </summary>
        public static string ModifyConfRes = "/agentconf/{0}/modifyconfresource";

        #endregion

        #region  多媒体会议 url

        /// <summary>
        /// 申请多媒体会议
        /// {0}:AgentID
        /// </summary>
        public static string ReqMultiMediaConf = "/multimediaconf/{0}/requestconf";

        /// <summary>
        /// 邀请加入多媒体会议
        /// {0}:AgentID
        /// </summary>
        public static string InvMultiMediaConf = "/multimediaconf/{0}/inviteconf";

        /// <summary>
        /// 上报加入多媒体会议结果
        /// {0}:AgentID
        /// </summary>
        public static string JoinConfRes = "/multimediaconf/{0}/joinconfresult";

        /// <summary>
        /// 结束多媒体会议
        /// {0}:AgentID
        /// </summary>
        public static string EndMultiMediaConf = "/multimediaconf/{0}/stopconf";

        /// <summary>
        /// 查询多媒体会议状态
        /// {0}:AgentID
        /// </summary>
        public static string QryConfState = "/multimediaconf/{0}/getconfstate";

        #endregion

        #region  查询类 url

        /// <summary>
        /// 查询 VDN ID
        /// {0}:AgentID
        /// </summary>
        public static string QryVDNID = "/queuedevice/{0}/innoinfo";

        /// <summary>
        /// 查询指定班组的座席信息
        /// {0}:AgentID
        /// {1}:GroupID
        /// </summary>
        public static string AgentOnGroup = "/agentgroup/{0}/agentongroupex?groupid={1}";

        /// <summary>
        /// 查询当前呼叫是否是内部呼叫
        /// {0}:AgentID
        /// {1}:表示当前座席无呼叫时，是否不查询上一通呼叫信息（boolean型）。
        /// </summary>
        public static string IsInnerCall = "/calldata/{0}/callinfo?isNoContainLastCall={1}";

        /// <summary>
        /// 获取指定VDN技能队列信息
        /// {0}:AgentID
        /// {1}:VDNID
        /// </summary>
        public static string QrySkillsOnVDN = "/queuedevice/{0}/vdnskill/{1}";

        #endregion

        #region  其他 url

        /// <summary>
        /// 修改密码
        /// {0}:AgentID
        /// </summary>
        public static string ModifyPwd = "/onlineagent/{0}/modifyaccountpwdex";

        #endregion
    }
}
