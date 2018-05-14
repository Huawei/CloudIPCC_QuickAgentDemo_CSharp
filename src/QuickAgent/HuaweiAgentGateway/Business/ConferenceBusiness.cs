using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Business
{
    /// <summary>
    /// Conference Ocx 业务类
    /// </summary>
    public class ConferenceBusiness
    {
        private AxConferenceLib.AxConference m_Conf = null;
        private IConfBusinessEvents m_BusinessEvent = null;

        /// <summary>
        /// 错误码字典
        /// </summary>
        private static Dictionary<string, string> m_ErrDict = new Dictionary<string, string>()
        {
            {"5000","一般错误"},
            {"5001","分配内存失败"},
            {"5002","没有视频设备"},
            {"5003","不支持的视频参数"},
            {"5004","用户未入会"},
            {"5005","用户已经入会"},
            {"5006","参数错误"},
            {"5007","无效窗口"},
            {"5008","设置窗口Hook失败"},
            {"5009","取消发送文件或接收文件"},
            {"5010","找不到主持人"},
            {"5011","超时"},
            {"5050","未设置窗口句柄"},
            {"5051","未设置显示区大小"},
            {"5052","文档不存在"},
            {"5053","文档未打开"},
            {"5054","当前有文档正在打开"},
            {"5055","当前没有文档正在打开"},
            {"5056","没有所需的页或文档没有页"},
            {"5057","当前文档或页未处于激活态"},
            {"5058","移动超出范围"},
            {"5059","打开的共享文档数量超出范围"},
            {"5060","当前没有文档共享"},
            {"5061","磁盘空间不足"},
            {"5062","状态出错"},
            {"5499","未知错误"},

            {"1","警告"},
            {"2","错误"},
            {"3","失败"},
            {"4","空指针"},
            {"5","调用超时"},
            {"6","参数错误"},
            {"7","内存太小"},
            {"8","xml参数错误"},
            {"9","没有权限"},
            {"10","调用的接口不支持或未实现"},  
            {"11","模块没有加载"},
            {"101","拒绝用户请求"},
            {"102","网络错误"},
            {"103","版本不支持"},
            {"104","通信协议错误"},
            {"105","服务器超出最大连接数"},
            {"106","license过期"},
            {"107","会议已结束"},
            {"109","连接断开"},
            {"111","鉴权时发现该用户不存在"},
            {"112","会议中channel达到阈值"},
            {"126","鉴权失败"},
            {"127","未知错误"},
            {"136","鉴权License超时,不允许入会"},
            {"137","服务器区域不允许,不允许入会"},
            {"138","用户重复入会,不允许入会"},
            {"300","会议正常离会"},
            {"301","用户被踢"},
            {"302","Token清除"},
            {"303","系统异常"},
            {"304","网络错误"},
            {"305","超时"},
            {"306","未知错误"},
            {"307","超过License最大许可"},
            {"308","系统错误"},
            {"309","用户离线"},
            {"400","未初始化"},
            {"401","重复初始化"},
            {"402","重复入会"},
            {"403","未入会"},
            {"404","组件未加载"},
            {"405","数据太大"},
            {"406","发送太快"},
            {"407","会议已结束"},
            {"408","会议已离开"},
            {"409","组件已加载"},
            {"410","组件已卸载"},
            {"411","会议已断开"},
            {"412","参数错误"},
            {"413","会议环境尚未初始化完成"},
            {"414","自己已经是主讲人"},
            {"415","电话未初始化"},
            {"416","加入电话会议失败"},
            {"417","申请内存失败"},
            {"418","参数无效"},
            {"419","相同的会议已经存在"},
            {"420","Paas模式下,错误的会议ID,会议前12位不能有值"},
            {"1101","存在"},
            {"1102","存在"},
            {"1103","存在"},
            {"1104","存在"},
            {"1105","存在"},
            {"1106","存在"},
            {"1107","个有效的CWP文件"},
            {"1109","性不存在"},
            {"1110","经打开"},
            {"1111","初始化"},
            {"1112","口未设置"},
            {"1113","加入"},
            {"1114","离线"},
            {"1115","在打印中"},
            {"1116","面数据正在下载中"},
            {"1117","共享时打开了保存的白板文件,或在白板共享时加载了保存的文档共享文档"},
            {"1118","据没有上传到服务器,并且此时共享者已不在会议中"},
            {"1121","调用失败"},
            {"1122","0页"},
            {"1123","印"},
            {"1124","式不支持"},
            {"1125","ice版本不支持"},
            {"1126","败"},
            {"1127","空"},
            {"1128","式错误"},
            {"1129","时"},
            {"1130","未安装"},
            {"1131","加载线程错误"},
            {"1132","加载错误"},
            {"1133","大页数(200页)"},
            {"1134","错误"},
            {"1135","务出现错误"},
            {"1136","erPoint2003运行中,不能加载"},
            {"1137","限打开"},
            {"1138","没有安装"},
            {"1140","已经存在"},
            {"1141","标注类型不存在"},
            {"1201","误"},
            {"1202","常"},
            {"1203","误"},
            {"1204","误"},
            {"1205","初始化"},
            {"1206","态错误"},
            {"1207","窗口handle"},
            {"1208","享任务失败"},
            {"1209","线"},
            {"1210","文档读写错误"},
            {"1211","锁定"},
            {"1212","数据帧尚未被发送"},
            {"1301","空"},
            {"1302","经被使用"},
            {"1303","在断线中"},
            {"1305","ngine失败"},
            {"1306","有Session"},
            {"1307","有Obj对象"},
            {"1308","有空闲的Table表"},
            {"1309","备时,此设备已经在正在打开的列表中"},
            {"1310","件还没有准备好"},
            {"1311","已经被卸载掉了"},
            {"1312","不在使用范围之内"},
            {"1313","超过设备本身的最大能力范围"},
            {"1314","大的打开视频路数"},
            {"1320","相同的能力已经在使用中"},
            {"1321","件当前是释放状态"},
            {"1322","不到此用户设备的对应Cell"},
            {"1323","大的打开视频路数"},
            {"1324","已经处于暂停状态"},
            {"1325","未处于暂停状态"},
            {"1326","低流时错误(不是Decode方,或是处于暂停状态)"},
            {"1327","频错误(数据Table表值已经不正常)"},
            {"1330","行流控"},
            {"1331","行外部流控"},
            {"1340","备已经在使用中"},
            {"1341","备已经被向导使用中"},
            {"1350","设备不是当前用户的设备"},
            {"1351","文件名太长"},
            {"1352","ender参数出错"},
            {"1353","转参数出错"},
            {"1354","移动版本API错用"},
            {"1355","口"},
            {"1356","设置的能力超出Capture的能力"},
            {"1357","使用过程中(比如Detach时),跟刚才Attach时的窗口不一致"},
            {"1358","时不能使用VIDEOHME,另一个会议正在使用HME中"},
            {"1801","建指定文件"},
            {"1802","被取消"},
            {"1803","件句柄"},
            {"1804","件状态"},
            {"1805","是一个有效文件"},
            {"1806","空"},
            {"1807","传超时"},
            {"1808","经正在下载"},

        };

        private string _fileID = string.Empty;

        public ConferenceBusiness()
        {
        }

        public bool AttachEvent(IConfBusinessEvents businessEvent)
        {
            if (null == m_Conf) return false;
            this.m_BusinessEvent = businessEvent;

            // 会议基础事件
            m_Conf.ConfInitResultEvent += _conf_ConfInitResultEvent;
            m_Conf.JoinConfResultEvent += _conf_JoinConfResultEvent;
            m_Conf.MemberEnterConfEvent += _conf_MemberEnterConfEvent;
            m_Conf.MemberLeaveConfEvent += _conf_MemberLeaveConfEvent;
            m_Conf.ConfNetWorkDisconnectedEvent += _conf_ConfNetWorkDisconnectedEvent;
            m_Conf.ConfNetWorkReconnectedEvent += _conf_ConfNetWorkReconnectedEvent;
            m_Conf.LoadComponentFailedEvent += _conf_LoadComponentFailedEvent;
            m_Conf.ConfNetWorkStatusEvent += _conf_ConfNetWorkStatusEvent;
            m_Conf.ConfRemainingTimeEvent += _conf_ConfRemainingTimeEvent;
            m_Conf.TerminateConfResultEvent += m_Conf_TerminateConfResultEvent;
            //  视频相关事件
            m_Conf.VideoSwitchEvent += _conf_VideoSwitchEvent;
            m_Conf.ConfVideoDisconnectedEvent += _conf_ConfVideoDisconnectedEvent;
            m_Conf.ConfVideoReconnectedEvent += _conf_ConfVideoReconnectedEvent;
            m_Conf.ConfVideoNotifyEvent += _conf_ConfVideoNotifyEvent;
            m_Conf.ConfVideoFlowWarningEvent += _conf_ConfVideoFlowWarningEvent;
            //  IO 相关事件
            m_Conf.MessageArrivedEvent += _conf_MessageArrivedEvent;
            m_Conf.FileTranProgressEvent += _conf_FileTranProgressEvent;
            m_Conf.FileTranOverEvent += _conf_FileTranOverEvent;
            m_Conf.FileArrivedEvent += _conf_FileArrivedEvent;
            //  多媒体协作事件
            m_Conf.StartShareScreenResultEvent += _conf_StartShareScreenResultEvent;
            m_Conf.SharingOwnerNotifyEvent += _conf_SharingOwnerNotifyEvent;
            m_Conf.ShareScreenStateNotifyEvent += _conf_ShareScreenStateNotifyEvent;
            m_Conf.OperationPrivilegeRequestEvent += _conf_OperationPrivilegeRequestEvent;
            m_Conf.OperationPrivilegeStateEvent += _conf_OperationPrivilegeStateEvent;
            m_Conf.ShareScreenWndSizeEvent += _conf_ShareScreenWndSizeEvent;
            //  文档共享事件
            m_Conf.ShareFileLoadingProgressEvent += _conf_ShareFileLoadingProgressEvent;
            m_Conf.ShareFileOpenEvent += _conf_ShareFileOpenEvent;
            m_Conf.ShareFileCurrentPageEvent += _conf_ShareFileCurrentPageEvent;
            m_Conf.ShareFileCloseEvent += _conf_ShareFileCloseEvent;

            return true;
        }

        void m_Conf_TerminateConfResultEvent(object sender, AxConferenceLib._DConferenceEvents_TerminateConfResultEventEvent e)
        {
            m_BusinessEvent.TerminateConfResultEvent(e.sEventInfo);
        }

        void _conf_ShareFileCloseEvent(object sender, AxConferenceLib._DConferenceEvents_ShareFileCloseEventEvent e)
        {
            m_BusinessEvent.ShareFileCloseEvent(e.sEventInfo);
        }

        void _conf_ShareFileCurrentPageEvent(object sender, AxConferenceLib._DConferenceEvents_ShareFileCurrentPageEventEvent e)
        {
            m_BusinessEvent.ShareFileCurrentPageEvent(e.sEventInfo);
        }

        void _conf_ShareFileOpenEvent(object sender, AxConferenceLib._DConferenceEvents_ShareFileOpenEventEvent e)
        {
            m_BusinessEvent.ShareFileOpenEvent(e.sEventInfo);
        }

        void _conf_ShareFileLoadingProgressEvent(object sender, AxConferenceLib._DConferenceEvents_ShareFileLoadingProgressEventEvent e)
        {
            m_BusinessEvent.ShareFileLoadingProgressEvent(e.sEventInfo);
        }

        void _conf_ShareScreenWndSizeEvent(object sender, AxConferenceLib._DConferenceEvents_ShareScreenWndSizeEventEvent e)
        {
            m_BusinessEvent.ShareScreenWndSizeEvent(e.sEventInfo);
        }

        void _conf_OperationPrivilegeStateEvent(object sender, AxConferenceLib._DConferenceEvents_OperationPrivilegeStateEventEvent e)
        {
            m_BusinessEvent.OperationPrivilegeStateEvent(e.sEventInfo);
        }

        void _conf_OperationPrivilegeRequestEvent(object sender, AxConferenceLib._DConferenceEvents_OperationPrivilegeRequestEventEvent e)
        {
            m_BusinessEvent.OperationPrivilegeRequestEvent(e.sEventInfo);
        }

        void _conf_ShareScreenStateNotifyEvent(object sender, AxConferenceLib._DConferenceEvents_ShareScreenStateNotifyEventEvent e)
        {
            m_BusinessEvent.ShareScreenStateNotifyEvent(e.sEventInfo);
        }

        void _conf_SharingOwnerNotifyEvent(object sender, AxConferenceLib._DConferenceEvents_SharingOwnerNotifyEventEvent e)
        {
            m_BusinessEvent.SharingOwnerNotifyEvent(e.sEventInfo);
        }

        void _conf_StartShareScreenResultEvent(object sender, AxConferenceLib._DConferenceEvents_StartShareScreenResultEventEvent e)
        {
            m_BusinessEvent.StartShareScreenResultEvent(e.sEventInfo);
        }

        void _conf_FileArrivedEvent(object sender, AxConferenceLib._DConferenceEvents_FileArrivedEventEvent e)
        {
            m_BusinessEvent.FileArrivedEvent(e.sEventInfo);
        }

        void _conf_FileTranOverEvent(object sender, AxConferenceLib._DConferenceEvents_FileTranOverEventEvent e)
        {
            m_BusinessEvent.FileTranOverEvent(e.sEventInfo);
        }

        void _conf_FileTranProgressEvent(object sender, AxConferenceLib._DConferenceEvents_FileTranProgressEventEvent e)
        {
            m_BusinessEvent.FileTranProgressEvent(e.sEventInfo);
        }

        void _conf_MessageArrivedEvent(object sender, AxConferenceLib._DConferenceEvents_MessageArrivedEventEvent e)
        {
            m_BusinessEvent.MessageArrivedEvent(e.sEventInfo);
        }

        void _conf_ConfVideoFlowWarningEvent(object sender, AxConferenceLib._DConferenceEvents_ConfVideoFlowWarningEventEvent e)
        {
            m_BusinessEvent.ConfVideoFlowWarningEvent(e.sEventInfo);
        }

        void _conf_ConfVideoNotifyEvent(object sender, AxConferenceLib._DConferenceEvents_ConfVideoNotifyEventEvent e)
        {
            m_BusinessEvent.ConfVideoNotifyEvent(e.sEventInfo);
        }

        void _conf_ConfVideoReconnectedEvent(object sender, EventArgs e)
        {
            m_BusinessEvent.ConfVideoReconnectedEvent();
        }

        void _conf_ConfVideoDisconnectedEvent(object sender, EventArgs e)
        {
            var res = string.Empty;
        }

        void _conf_VideoSwitchEvent(object sender, AxConferenceLib._DConferenceEvents_VideoSwitchEventEvent e)
        {
            m_BusinessEvent.VideoSwitchEvent(e.sEventInfo);
        }

        void _conf_ConfRemainingTimeEvent(object sender, AxConferenceLib._DConferenceEvents_ConfRemainingTimeEventEvent e)
        {
            m_BusinessEvent.ConfRemainingTimeEvent(e.sEventInfo);
        }

        void _conf_ConfNetWorkStatusEvent(object sender, AxConferenceLib._DConferenceEvents_ConfNetWorkStatusEventEvent e)
        {
            m_BusinessEvent.ConfNetWorkStatusEvent(e.sEventInfo);
        }

        void _conf_LoadComponentFailedEvent(object sender, AxConferenceLib._DConferenceEvents_LoadComponentFailedEventEvent e)
        {
            m_BusinessEvent.LoadComponentFailedEvent(e.sEventInfo);
        }

        void _conf_ConfNetWorkReconnectedEvent(object sender, EventArgs e)
        {
            var res = string.Empty;
        }

        void _conf_ConfNetWorkDisconnectedEvent(object sender, EventArgs e)
        {
            var res = string.Empty;
        }

        void _conf_MemberLeaveConfEvent(object sender, AxConferenceLib._DConferenceEvents_MemberLeaveConfEventEvent e)
        {
            m_BusinessEvent.MemberLeaveConfEvent(e.sEventInfo);
        }

        void _conf_MemberEnterConfEvent(object sender, AxConferenceLib._DConferenceEvents_MemberEnterConfEventEvent e)
        {
            m_BusinessEvent.MemberEnterConfEvent(e.sEventInfo);
        }

        void _conf_ConfInitResultEvent(object sender, AxConferenceLib._DConferenceEvents_ConfInitResultEventEvent e)
        {
            m_BusinessEvent.ConfInitResultEvent(e.sEventInfo);
        }

        void _conf_JoinConfResultEvent(object sender, AxConferenceLib._DConferenceEvents_JoinConfResultEventEvent e)
        {
            m_BusinessEvent.JoinConfResultEvent(e.sEventInfo);
        }

        public void Initial(AxConferenceLib.AxConference conf)
        {
            this.m_Conf = conf;
        }

        public void Dispose()
        {
            m_Conf = null;
            m_BusinessEvent = null;
        }

        public bool DetachEvents(IConfBusinessEvents businessEvent)
        {
            if (null == m_Conf) return false;
            // 会议基础事件
            m_Conf.ConfInitResultEvent -= _conf_ConfInitResultEvent;
            m_Conf.JoinConfResultEvent -= _conf_JoinConfResultEvent;
            m_Conf.MemberEnterConfEvent -= _conf_MemberEnterConfEvent;
            m_Conf.MemberLeaveConfEvent -= _conf_MemberLeaveConfEvent;
            m_Conf.ConfNetWorkDisconnectedEvent -= _conf_ConfNetWorkDisconnectedEvent;
            m_Conf.ConfNetWorkReconnectedEvent -= _conf_ConfNetWorkReconnectedEvent;
            m_Conf.LoadComponentFailedEvent -= _conf_LoadComponentFailedEvent;
            m_Conf.ConfNetWorkStatusEvent -= _conf_ConfNetWorkStatusEvent;
            m_Conf.ConfRemainingTimeEvent -= _conf_ConfRemainingTimeEvent;
            //_conf.ConfNetworkQosNotifyEvent -= _conf_ConfNetworkQosNotifyEvent;
            //  视频相关事件
            m_Conf.VideoSwitchEvent -= _conf_VideoSwitchEvent;
            m_Conf.ConfVideoDisconnectedEvent -= _conf_ConfVideoDisconnectedEvent;
            m_Conf.ConfVideoReconnectedEvent -= _conf_ConfVideoReconnectedEvent;
            m_Conf.ConfVideoNotifyEvent -= _conf_ConfVideoNotifyEvent;
            m_Conf.ConfVideoFlowWarningEvent -= _conf_ConfVideoFlowWarningEvent;
            //  IO 相关事件
            m_Conf.MessageArrivedEvent -= _conf_MessageArrivedEvent;
            m_Conf.FileTranProgressEvent -= _conf_FileTranProgressEvent;
            m_Conf.FileTranOverEvent -= _conf_FileTranOverEvent;
            m_Conf.FileArrivedEvent -= _conf_FileArrivedEvent;
            //  多媒体协作事件
            m_Conf.StartShareScreenResultEvent -= _conf_StartShareScreenResultEvent;
            m_Conf.SharingOwnerNotifyEvent -= _conf_SharingOwnerNotifyEvent;
            m_Conf.ShareScreenStateNotifyEvent -= _conf_ShareScreenStateNotifyEvent;
            m_Conf.OperationPrivilegeRequestEvent -= _conf_OperationPrivilegeRequestEvent;
            m_Conf.OperationPrivilegeStateEvent -= _conf_OperationPrivilegeStateEvent;
            m_Conf.ShareScreenWndSizeEvent -= _conf_ShareScreenWndSizeEvent;
            //  文档共享事件
            m_Conf.ShareFileLoadingProgressEvent -= _conf_ShareFileLoadingProgressEvent;
            m_Conf.ShareFileOpenEvent -= _conf_ShareFileOpenEvent;
            m_Conf.ShareFileCurrentPageEvent -= _conf_ShareFileCurrentPageEvent;
            m_Conf.ShareFileCloseEvent -= _conf_ShareFileCloseEvent;

            this.m_BusinessEvent = null;
            return true;
        }

        #region  接口方法

        #region  文档共享接口

        /// <summary>
        /// 设置共享文档窗口大小
        /// </summary>
        /// <param name="sWidth"></param>
        /// <param name="sHeight"></param>
        /// <returns></returns>
        public string ShareFileSetDisplaySize(string sWidth, string sHeight)
        {
            if (m_Conf == null) return AGWErrorCode.Empty;
            return m_Conf.ShareFileSetDisplaySize(sWidth, sHeight) + "";
        }

        /// <summary>
        /// 设置共享文档窗口句柄
        /// </summary>
        /// <param name="sHWnd">窗口句柄</param>
        /// <returns></returns>
        public string ShareFileSetDisplayWnd(string sHWnd)
        {
            if (m_Conf == null) return AGWErrorCode.Empty;
            return m_Conf.ShareFileSetDisplayWnd(sHWnd) + "";
        }

        /// <summary>
        /// 发起文档共享
        /// </summary>
        /// <param name="fileName">文档的全路径</param>
        /// <returns></returns>
        public string ShareFileOpen(string fileName)
        {
            if (m_Conf == null) return AGWErrorCode.Empty;
            return m_Conf.ShareFileOpen(fileName) + "";
        }

        /// <summary>
        /// 获取共享文档信息
        /// </summary>
        /// <param name="fileID">共享文档ID</param>
        /// <returns></returns>
        public string GetSharedFileInfo(string fileID)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileGetFileInfo(fileID) + "";
        }

        /// <summary>
        /// 关闭共享文档
        /// </summary>
        /// <param name="fileID"></param>
        /// <returns></returns>
        public string StopShareFile(string fileID)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileClose(fileID) + "";
        }

        /// <summary>
        /// 共享文档跳转到上一页
        /// </summary>
        /// <param name="fileID">文档ID</param>
        /// <param name="sync">是否同步其他人界面</param>
        /// <returns></returns>
        public string ShareFilePrePage(string fileID, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFilePreviousPage(fileID, sync) + "";
        }

        /// <summary>
        /// 共享文档跳转到下一页
        /// </summary>
        /// <param name="fileID">文档ID</param>
        /// <param name="sync">是否同步其他人界面</param>
        /// <returns></returns>
        public string ShareFileNextPage(string fileID, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileNextPage(fileID, sync) + "";
        }

        /// <summary>
        /// 共享文档跳转到指定页
        /// </summary>
        /// <param name="fileID">文档ID</param>
        /// <param name="pageNo">页码，页码从0开始</param>
        /// <param name="sync">是否同步其他人界面</param>
        /// <returns></returns>
        public string ShareFileSpecPage(string fileID, string pageNo, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileSpecifiedPage(fileID, pageNo, sync) + "";
        }

        /// <summary>
        /// 切换共享文档
        /// </summary>
        /// <param name="fileID">共享文档ID</param>
        /// <param name="sync">是否同步其他人界面</param>
        /// <returns></returns>
        public string ShareFileSwitch(string fileID, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileSwitch(fileID, sync) + "";
        }

        /// <summary>
        /// 保存共享文档到本地
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string ShareFileSave(string fileID, string fileName)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileSave(fileID, fileName) + "";
        }

        /// <summary>
        /// 共享文档缩放显示控制
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="zoomType"></param>
        /// <param name="percent"></param>
        /// <param name="sync"></param>
        /// <returns></returns>
        public string ShareFileZoomControl(string fileID, string zoomType, string percent, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileZoomControl(fileID, zoomType, percent, sync) + "";
        }

        /// <summary>
        /// 共享文档抓手移动
        /// </summary>
        /// <param name="fileID">共享文档ID</param>
        /// <param name="opera">操作类型</param>
        /// <param name="pointX">相对页面原点的X坐标</param>
        /// <param name="pointY">相对页面原点的Y坐标</param>
        /// <param name="sync">是否同步其他人界面</param>
        /// <returns></returns>
        public string ShareFileMoveByHand(string fileID, string opera, string pointX, string pointY, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileMoveByHand(fileID, opera, pointX, pointY, sync) + "";
        }

        /// <summary>
        /// 共享文档抓手自动方式
        /// </summary>
        /// <param name="autoMove">是否开启抓手自动移动文档页面</param>
        /// <param name="sync">是否同步其他人界面</param>
        /// <returns></returns>
        public string ShareFileAutoMoveByHand(string autoMove, string sync)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ShareFileAutoMoveByHand(autoMove, sync) + "";
        }

        #endregion

        #region  会议基础接口

        /// <summary>
        /// 加入多媒体会议
        /// </summary>
        /// <param name="confInfo"></param>
        /// <returns></returns>
        public string JoinConf(string confInfo)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.JoinConf(confInfo) + "";
        }

        /// <summary>
        /// 离开多媒体会议
        /// </summary>
        /// <returns></returns>
        public string LeaveConf()
        {
            if (m_Conf == null) return "-1";
            return m_Conf.LeaveConf() + "";
        }

        /// <summary>
        /// 终止多媒体会议
        /// </summary>
        /// <returns></returns>
        public string TerminateConf()
        {
            if (m_Conf == null) return "-1";
            return m_Conf.TerminateConf() + "";
        }

        /// <summary>
        /// 限制数据流量(该接口只能限制上行流量，而不能限制下行流量)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string LimitDataFlowSize(string type, string size)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.LimitDataFlowSize(type, size) + "";
        }

        /// <summary>
        /// 根据错误码获取错误描述
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetErrMsg(string code)
        {
            return m_ErrDict.ContainsKey(code) ? m_ErrDict[code] : "未定义错误:" + code;
        }

        /// <summary>
        /// 获取主讲人Id
        /// </summary>
        /// <returns></returns>
        public string GetPresenter()
        {
            if (m_Conf == null) return AGWErrorCode.Empty;
            return m_Conf.GetPresenter();
        }
        #endregion

        #region  IO 相关接口

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userID">接收方用户Id</param>
        /// <param name="msg">接收方用户Id</param>
        /// <param name="msgID">消息Id</param>
        /// <returns></returns>
        public string SendMsg(string userID, string msg, string msgID)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.SendMessage(userID, msg, msgID) + "";
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="userID">接收方用户Id</param>
        /// <param name="file">本地文件名全路径</param>
        /// <returns></returns>
        public string SendFile(string userID, string file)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.SendFile(userID, file) + "";
        }

        /// <summary>
        /// 取消文件发送
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <returns></returns>
        public string CancelSendFile(string handle)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.CancelSendingFile(handle) + "";
        }

        /// <summary>
        /// 接收文件
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <param name="file">文件名全路径</param>
        /// <returns></returns>
        public string RevFile(string handle, string file)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ReceiveFile(handle, file) + "";
        }

        /// <summary>
        /// 取消接收文件
        /// </summary>
        /// <param name="handle">文件句柄</param>
        /// <returns></returns>
        public string CancelRevFile(string handle)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.CancelReceiveFile(handle) + "";
        }

        #endregion

        #region  视频相关接口

        /// <summary>
        /// 获取本地视频设备
        /// </summary>
        /// <returns></returns>
        public string GetLocalVideoDevices()
        {
            if (m_Conf == null) return "-1";
            return m_Conf.GetLocalVideoDevices();
        }

        /// <summary>
        /// 打开视频设备
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public string OpenVideoDevice(string deviceID)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.OpenVideoDevice(deviceID) + "";
        }

        /// <summary>
        /// 关闭视频设备
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public string CloseVideoDevice(string deviceID)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.CloseVideoDevice(deviceID) + "";
        }

        /// <summary>
        /// 开始绘制视频
        /// </summary>
        /// <param name="userID">要绘制的视频所属用户ID</param>
        /// <param name="deviceID">要绘制的视频设备ID</param>
        /// <param name="handle">视频绘制的窗口句柄</param>
        /// <param name="mode">绘制模式</param>
        /// <returns></returns>
        public string StartDrawVideo(string userID, string deviceID, string handle, string mode)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.StartDrawVideo(userID, deviceID, handle, mode) + "";
        }

        /// <summary>
        /// 停止绘制视频
        /// </summary>
        /// <param name="userID">要停止绘制的视频所属用户ID</param>
        /// <param name="deviceID">要停止绘制的视频设备ID</param>
        /// <param name="handle">视频绘制的窗口句柄</param>
        /// <returns></returns>
        public string StopDrawVideo(string userID, string deviceID, string handle)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.StopDrawVideo(userID, deviceID, handle) + "";
        }

        /// <summary>
        /// 恢复视频
        /// </summary>
        /// <param name="userID">要暂停的视频所属用户ID</param>
        /// <param name="deviceID">要暂停的视频设备ID</param>
        /// <returns></returns>
        public string ResumeVideo(string userID, string deviceID)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.ResumeVideo(userID, deviceID) + "";
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <param name="fileName">照片存放的全路径，照片只支持png格式</param>
        /// <param name="sol">照片分辨率(格式：{"X":"宽","Y":"高"})</param>
        /// <returns></returns>
        public string SnapShot(string deviceID, string fileName, string sol)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.Snapshot(deviceID, fileName, sol) + "";
        }

		/// <summary>
        /// 设置设备当前视频参数
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <param name="parm">视频参数</param>
        /// <returns></returns>
        public string SetVideoCurrentParam(string deviceID, string parm)
        {
            if (m_Conf == null) return AGWErrorCode.Empty;
            return m_Conf.SetVideoCurrentParam(deviceID, parm) + "";
        }

        /// <summary>
        /// 获取设备支持的视频参数
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns></returns>
        public string GetSupportVideoParams(string deviceID)
        {
            if (m_Conf == null) return AGWErrorCode.Empty;
            return m_Conf.GetSupportVideoParams(deviceID);
        }

        /// <summary>
        /// 获取设备当前视频参数
        /// </summary>
        /// <param name="deviceID">设备ID</param>
        /// <returns></returns>
        public string GetVideoCurrentParam(string deviceID)
        {
            if (null == m_Conf) return AGWErrorCode.Empty;
            return m_Conf.GetVideoCurrentParam(deviceID);
        }
        #endregion

        #region  多媒体协作接口

        /// <summary>
        /// 设置显示屏幕共享窗口
        /// </summary>
        /// <param name="handle">窗口句柄</param>
        /// <returns></returns>
        public string SetDisplayShareScreenWnd(string handle)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.SetDisplayShareScreenWnd(handle) + "";
        }

        /// <summary>
        /// 获取本地应用信息列表
        /// </summary>
        /// <returns></returns>
        public string GetApplicationList()
        {
            if (m_Conf == null) return "-1";
            return m_Conf.GetApplicationList();
        }

        /// <summary>
        /// 添加要共享的应用程序
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public string AddApplicationToShare(string handle)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.AddApplicationToShare(handle) + "";
        }

        /// <summary>
        /// 删除已共享的应用程序
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public string DeleteSharedApplication(string handle)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.DeleteSharedApplication(handle) + "";
        }

        /// <summary>
        /// 开启屏幕共享
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string StartShareScreen(string type)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.StartShareScreen(type) + "";
        }

        /// <summary>
        /// 停止屏幕共享
        /// </summary>
        /// <returns></returns>
        public string StopShareScreen()
        {
            if (m_Conf == null) return "-1";
            return m_Conf.StopShareScreen() + "";
        }

        /// <summary>
        /// 设置远程操作权限
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="type"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public string SetOperationPrivilege(string userID, string type, string action)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.SetOperationPrivilege(userID, type, action) + "";
        }

        /// <summary>
        /// 申请远程操作权限
        /// </summary>
        /// <param name="type">权限类型。当前只支持RemoteCtl-远程协助。不区分大小写</param>
        /// <returns></returns>
        public string RequestOperationPrivilege(string type)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.RequestOperationPrivilege(type) + "";
        }

        /// <summary>
        /// 设置共享屏幕显示大小
        /// </summary>
        /// <param name="width">屏幕宽度</param>
        /// <param name="height">屏幕高度</param>
        /// <returns></returns>
        public string SetShareScreenDisplaySize(string width, string height)
        {
            if (m_Conf == null) return "-1";
            return m_Conf.SetShareScreenDisplaySize(width, height) + "";
        }

        #endregion

        #endregion
    }
}
