using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuaweiAgentGateway.Business
{
    /// <summary>
    /// Conference.ocx 事件接口
    /// </summary>
    public interface IConfBusinessEvents
    {
        void ShareFileCloseEvent(string info);

        void ShareFileCurrentPageEvent(string info);

        void ShareFileOpenEvent(string info);

        void ShareFileLoadingProgressEvent(string info);

        void ConfNetworkQosNotifyEvent(string info);

        void ShareScreenWndSizeEvent(string info);

        void OperationPrivilegeStateEvent(string info);

        void OperationPrivilegeRequestEvent(string info);

        void ShareScreenStateNotifyEvent(string info);

        void SharingOwnerNotifyEvent(string info);

        void StartShareScreenResultEvent(string info);

        void FileArrivedEvent(string info);

        void FileTranOverEvent(string info);

        void FileTranProgressEvent(string info);

        void MessageArrivedEvent(string info);

        void ConfVideoFlowWarningEvent(string info);

        void ConfVideoNotifyEvent(string info);

        void ConfVideoReconnectedEvent();

        void ConfVideoDisconnectedEvent();

        void VideoSwitchEvent(string info);

        void ConfRemainingTimeEvent(string info);

        void ConfNetWorkStatusEvent(string info);

        void LoadComponentFailedEvent(string info);

        void ConfNetWorkReconnectedEvent();

        void ConfNetWorkDisconnectedEvent();

        void MemberLeaveConfEvent(string info);

        void MemberEnterConfEvent(string info);

        void ConfInitResultEvent(string info);

        void JoinConfResultEvent(string info);

        void TerminateConfResultEvent(string info);
    }
}
