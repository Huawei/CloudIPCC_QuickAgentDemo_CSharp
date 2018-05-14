using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway
{
    public interface IBusiness
    {
        #region  初始化相关

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        int Initial();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="businessCallBack"></param>
        /// <returns></returns>
        bool AttachEvent(IBusinessEvents businessCallBack);

        /// <summary>
        /// 事件解注册
        /// </summary>
        /// <param name="businessCallBack"></param>
        /// <returns></returns>
        bool DetachEvents(IBusinessEvents businessCallBack);

        /// <summary>
        /// 签入
        /// </summary>
        /// <param name="MediaServer"></param>
        /// <param name="TelNum"></param>
        /// <param name="isForceLogin"></param>
        /// <returns></returns>
        string SignInEx(string MediaServer, string TelNum, bool isForceLogin);

        /// <summary>
        /// 签出
        /// </summary>
        /// <returns></returns>
        string SignOutEx();

        #endregion

        #region  基本操作

        /// <summary>
        /// 取消休息
        /// </summary>
        /// <returns></returns>
        string CancelRest();

        /// <summary>
        /// 休息
        /// </summary>
        /// <param name="RestTime"></param>
        /// <param name="usCause"></param>
        /// <returns></returns>
        string RestEx(int RestTime, int usCause);

        /// <summary>
        /// 示忙
        /// </summary>
        /// <returns></returns>
        string SayBusy();

        /// <summary>
        /// 示闲
        /// </summary>
        /// <returns></returns>
        string SayFree();

        /// <summary>
        /// 外呼
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="Caller"></param>
        /// <param name="Called"></param>
        /// <param name="Pilot"></param>
        /// <param name="Mode"></param>
        /// <param name="SkillID"></param>
        /// <param name="Param"></param>
        /// <param name="mediaAbility"></param>
        /// <param name="checkMode"></param>
        /// <returns></returns>
        string CallOutEx3(int MediaType, string Caller, string Called, string Pilot, int Mode, int SkillID, string Param, int mediaAbility, int checkMode);

        /// <summary>
        /// 内部求助
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="DestWorkNo"></param>
        /// <param name="Mode"></param>
        /// <param name="DeviceType"></param>
        /// <returns></returns>
        string InternalHelp(int MediaType, string DestWorkNo, int Mode, int DeviceType);

        /// <summary>
        /// 内部呼叫
        /// </summary>
        /// <param name="DestWorkNo"></param>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        string CallInnerEx(string DestWorkNo, int MediaType);

        /// <summary>
        /// 连接保持
        /// </summary>
        /// <param name="CallID"></param>
        /// <returns></returns>
        string ConnectHoldEx(string CallID);

        /// <summary>
        /// 三方通话
        /// </summary>
        /// <param name="CallID"></param>
        /// <returns></returns>
        string ConfJoinEx(string CallID);

        /// <summary>
        /// 取保持
        /// </summary>
        /// <param name="CallID"></param>
        /// <returns></returns>
        string GetHoldEx(string CallID);

        /// <summary>
        /// 保持
        /// </summary>
        /// <returns></returns>
        string HoldEx();

        /// <summary>
        /// 取消静音
        /// </summary>
        /// <returns></returns>
        string EndMuteUserEx();

        /// <summary>
        /// 开始静音
        /// </summary>
        /// <returns></returns>
        string BeginMuteUserEx();

        /// <summary>
        /// 应答
        /// </summary>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        string AnswerEx(int MediaType);

        /// <summary>
        /// 进入工作态
        /// </summary>
        /// <returns></returns>
        string SetWork();

        /// <summary>
        /// 进入空闲态
        /// </summary>
        /// <returns></returns>
        string AgentEnterIdle();

        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="MediaType"></param>
        /// <returns></returns>
        string ReleaseCall(int MediaType);

        /// <summary>
        /// 技能重设
        /// </summary>
        string ResetSkillEx(string WorkNo);

        /// <summary>
        /// 座席当前状态
        /// </summary>
        /// <returns></returns>
        string AgentStatus();

        /// <summary>
        /// 是否存在保持的电话
        /// </summary>
        bool ExistHold { get; set; }

        /// <summary>
        /// 发布公告
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        string NotifyBulletin(int type, string name, string msg);

        /// <summary>
        /// 二次拨号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        string SecondDial(string number);

        /// <summary>
        /// 发送便签
        /// </summary>
        /// <param name="agentId">目标座席工号</param>
        /// <param name="msg">内容</param>
        /// <returns></returns>
        string SendNote(int agentId, string msg);

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="id">callId</param>
        /// <param name="msg">要设置的数据内容</param>
        /// <param name="mediaType">媒体类型（用于 OCX）</param>
        /// <returns></returns>
        string SetData(string id, string msg, int mediaType);

        /// <summary>
        /// 设置是否进入工作态
        /// </summary>
        /// <returns></returns>
        string SetDeviceAutoEnterWork(bool flag);

        /// <summary>
        /// 取消求助
        /// </summary>
        /// <returns></returns>
        string CancelHelp();

        /// <summary>
        /// 修改密码
        /// </summary>
        string ModifyAccountPwd(string workNo, string oldPwd, string newPwd);

        #endregion

        #region  查询相关

        /// <summary>
        /// 当前呼叫是否是内部呼叫
        /// </summary>
        /// <returns></returns>
        bool IsInnerCallOrInnerHelp();

        /// <summary>
        /// 班组下是否有座席
        /// </summary>
        /// <param name="groupID">班组 ID</param>
        /// <param name="monitorID">班长工号</param>
        /// <returns></returns>
        bool HasAgentInGroup(int groupID, int monitorID);

        /// <summary>
        /// 座席是否自动进入工作态
        /// </summary>
        /// <returns></returns>
        bool GetDeviceAutoEnterWork();

        /// <summary>
        /// 获取座席 VDN ID
        /// </summary>
        /// <returns></returns>
        string GetVDNID();

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="callid"></param>
        /// <returns></returns>
        string QryData(string callid);

        /// <summary>
        /// 查询工作组
        /// </summary>
        /// <returns></returns>
        List<WorkGroupData> GetWorkGroupData();

        /// <summary>
        /// 查询座席正在处理的呼叫列表
        /// </summary>
        /// <returns></returns>
        List<CallInfo> GetDeviceCallLst();

        /// <summary>
        /// 获取座席类型（暂时没用到）
        /// </summary>
        /// <returns></returns>
        TAgentTypeForOcx GetAgentType();

        List<SkillInfo> GetAgentSkills();

        List<SkillInfo> GetVDNSkillInfo();

        List<IvrInfo> GetVDNIvrInfo();

        List<AccessCode> GetVDNAccessCodeInfo();

        List<HoldListData> GetHoldList();

        List<AgentStateInfo> GetAllAgentStatusInfo();

        #endregion

        #region  转移相关

        /// <summary>
        /// 呼叫转出
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="Caller"></param>
        /// <param name="Called"></param>
        /// <param name="Flag"></param>
        /// <param name="Mode"></param>
        /// <param name="Pilot"></param>
        /// <returns></returns>
        string TransOutEx(int MediaType, string Caller, string Called, int Flag, int Mode, string Pilot);

        /// <summary>
        /// 呼叫转移
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="TransType"></param>
        /// <param name="Destination"></param>
        /// <param name="DeviceType"></param>
        /// <returns></returns>
        string TransInnerEx(int MediaType, int TransType, string Destination, int DeviceType);

        #endregion

        #region  电话会议相关

        /// <summary>
        /// 申请电话会议
        /// </summary>
        /// <param name="num"></param>
        /// <param name="caller"></param>
        /// <param name="time"></param>
        /// <param name="promptTime"></param>
        /// <param name="beepTone"></param>
        /// <param name="mode"></param>
        /// <param name="voicePath"></param>
        /// <returns></returns>
        string RequestAgentConf(int num, string caller, int time, int promptTime, bool beepTone, int mode, string voicePath);

        /// <summary>
        /// 座席应答会议
        /// </summary>
        /// <returns></returns>
        string AnswerAgentConf();

        /// <summary>
        /// 座席拒接会议
        /// </summary>
        /// <returns></returns>
        string RejectAgentConf();

        /// <summary>
        /// 退出电话会议
        /// </summary>
        /// <returns></returns>
        string LeaveAgentConf();

        /// <summary>
        /// 释放电话会议
        /// </summary>
        /// <returns></returns>
        string ReleaseAgentConf();

        /// <summary>
        /// 主席申请延长会议时间
        /// </summary>
        /// <param name="time">需要增加的时间(分钟：1～1440,默认 60)</param>
        /// <returns></returns>
        string ProlongTimeAgentConf(int time);

        /// <summary>
        /// 转移主席权限
        /// </summary>
        /// <param name="workNo">目标座席</param>
        /// <returns></returns>
        string ApplyToShiftPresidentAgentConf(int workNo);

        /// <summary>
        /// 电话会议放音
        /// </summary>
        /// <param name="type">放音类型（0：播放会议前景;1：播放会议背景音）</param>
        /// <param name="mode">放音模式（1：播放VP提示音;2：播放指定文件音;4：播放SIG音）</param>
        /// <param name="voicePath">提示音编码或者文件绝对路径名</param>
        /// <returns></returns>
        string PlayVoiceAgentConf(int type, int mode, string voicePath);

        /// <summary>
        /// 停止电话会议放音
        /// </summary>
        /// <returns></returns>
        string StopVoiceAgentConf();

        /// <summary>
        /// 改变会议资源方数
        /// </summary>
        /// <param name="type">变更类型(0：增加;1：减少)</param>
        /// <param name="num">申请变更的数目（2-118）</param>
        /// <returns></returns>
        string ModifyConfResAgentConf(int type, int num);

        #endregion

        #region  多媒体会议

        /// <summary>
        /// 申请多媒体会议
        /// </summary>
        /// <param name="callID">桌面共享依附的callid字符串格式</param>
        /// <param name="num">会议方数</param>
        /// <param name="info">会议参与方信息，只支持座席</param>
        /// <returns></returns>
        string RequestMultimediaConf(string callID, int num, string info);

        /// <summary>
        /// 邀请加入多媒体会议
        /// </summary>
        /// <param name="confID">会议ID</param>
        /// <param name="num">会议方数</param>
        /// <param name="info">待邀请的参与方信息，只支持座席</param>
        /// <returns></returns>
        string InviteJoinMultimediaConf(int confID, int num, string info);

        /// <summary>
        /// 上报加入多媒体会议结果
        /// </summary>
        /// <param name="confID">会议ID</param>
        /// <param name="res">客户端加入多媒体会议结果</param>
        /// <param name="cause">失败时表示失败原因码</param>
        /// <returns></returns>
        string JoinMultimediaConfResponse(int confID, int res, int cause);

        /// <summary>
        /// 结束多媒体会议
        /// </summary>
        /// <param name="confID">会议 ID</param>
        /// <returns></returns>
        string StopMultimediaConf(int confID);

        /// <summary>
        /// 查询多媒体会议状态
        /// </summary>
        /// <param name="partNum">查询的与会人个数</param>
        /// <param name="info">查询到的多媒体会议状态信息列表</param>
        /// <returns></returns>
        string QueryMultimediaConfState(int partNum, string info);

        #endregion
    }
}
