

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HuaweiAgentGateway;
using QuickAgent.Common;
using QuickAgent.Src.View.TextChat;
using HuaweiAgentGateway.Entity.AGWEntity;
using HuaweiAgentGateway.AgentGatewayEntity;
using System.IO;
using QuickAgent.Src.Common;
using System.Threading;
using System.Globalization;
using QuickAgent.View;
using System.Windows.Controls.Primitives;
using System.Collections;
using QuickAgent.Constants;
using HuaweiAgentGateway.Business;
using System.Windows.Forms.Integration;
using HuaweiAgentGateway.Utils;
using HuaweiAgentGateway.Entity.ConfEntity;
using System.Windows.Interop;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// Textchat Window
    /// </summary>
    public partial class TextChatWindow : Window, IConfBusinessEvents
    {
        #region  Fields

        #region  Textchat

        /// <summary>
        /// textchat window instance
        /// </summary>
        private static TextChatWindow _chatWindow { set; get; }

        /// <summary>
        /// text chat msg dictionary
        /// key: callID
        /// value: message
        /// </summary>
        public Dictionary<string, string> ChatDictWithMsg { set; get; }

        /// <summary>
        /// current callID
        /// </summary>
        public string CurrentCallID { set; get; }

        /// <summary>
        /// exists textchat
        /// </summary>
        public bool HasTextChat
        {
            get { return lst_callid.Items != null && lst_callid.Items.Count > 0; }
        }

        /// <summary>
        /// is video answer type
        /// </summary>
        public bool IsVideoAnswerType
        {
            get { return chk_answervideotype.IsChecked.Value; }
        }

        /// <summary>
        /// is default answer type
        /// </summary>
        public bool IsDefaultAnswerType
        {
            get { return chk_answerdefaulttype.IsChecked.Value; }
        }

        /// <summary>
        /// sms message format
        /// {0}: sender
        /// {1}: time
        /// {2}: message
        /// </summary>
        private string m_smsMsgFormat = "(SMS_{0}) {1}" + Environment.NewLine + "{2}";

        /// <summary>
        /// common msg succ format
        /// {0}: sender
        /// {1}: time
        /// {2}: message
        /// </summary>
        private string m_msgSuccFormat = "({0}) {1}" + Environment.NewLine + "{2}";

        /// <summary>
        /// common msg fail format
        /// {0}: sender
        /// {1}: time
        /// {2}: message
        /// </summary>
        private string m_msgFailFormat = "({0}) {1}" + Environment.NewLine + "{2}<failed>";

        /// <summary>
        /// event logs
        /// </summary>
        private static List<LogShowParm> m_logInfoParm = new List<LogShowParm>();

        #endregion

        #region  Multimedia Conference Fields

        /// <summary>
        /// value: whether self is in share
        /// </summary>
        private bool _isSelfInShare { set; get; }

        /// <summary>
        /// share screen state notify
        /// </summary>
        private const string SCREENSHARE_START = "Start";

        /// <summary>
        /// share screen state notify
        /// </summary>
        private const string SCREENSHARE_STOP = "Stop";

        /// <summary>
        /// file receive user id
        /// 0: all will receive file except self
        /// </summary>
        private const string FILERECV_USERID = "0";

        /// <summary>
        /// request Operation privilege user's ID
        /// </summary>
        private string _reqPriUserID { set; get; }

        /// <summary>
        /// share window height
        /// </summary>
        private const int WIN_HEIGHT = 500;

        /// <summary>
        /// share window width
        /// </summary>
        private const int WIN_WIDTH = 500;

        /// <summary>
        /// screen share window
        /// </summary>
        private ConfShareWindow _screenShareWin { set; get; }

        /// <summary>
        /// file share view window
        /// </summary>
        private ConfShareWindow _fileShareWin { set; get; }

        /// <summary>
        /// share file ID
        /// </summary>
        private string _shareFileID { set; get; }

        /// <summary>
        /// local apps list
        /// </summary>
        private List<AppDetail> _localApps { set; get; }

        /// <summary>
        /// confers list
        /// </summary>
        private List<string> m_lstConfers = new List<string>();

        /// <summary>
        /// file status: receive
        /// </summary>
        private const string FILE_STATUS_REC = "1";

        /// <summary>
        /// file statuc: send
        /// </summary>
        private const string FILE_STATUS_SEN = "3";

        /// <summary>
        /// receive file handle
        /// </summary>
        private string _recvFileHandle { set; get; }

        /// <summary>
        /// create conf string format
        /// {0}: workNo
        /// </summary>
        private const string REQ_CONF_FORMAT = "9,1,{0};";

        /// <summary>
        /// privilege type: support 'RemoteCtl-' only at now
        /// </summary>
        private const string PRIVILEGE_TYPE = "RemoteCtl";

        /// <summary>
        /// privilege action: ignore case
        /// </summary>
        private List<string> m_privilegeAction = new List<string>() 
        {
            "Add",
            "Delete",
            "Reject",
        };

        /// <summary>
        /// screen share type: ignore case
        /// </summary>
        private List<string> m_screenShareType = new List<string>() 
        {
            "Desktop",
            "Application",
        };

        /// <summary>
        /// file sync
        /// 0: self only
        /// 1: sync to others
        /// </summary>
        private const string FILE_SYNC = "1";

        /// <summary>
        /// conference business instance
        /// </summary>
        private ConferenceBusiness _confBus { set; get; }

        private WindowsFormsHost _formsHost { set; get; }

        private AxConferenceLib.AxConference _axConf { set; get; }

        //private bool _isConfInitSucc { set; get; }

        /// <summary>
        /// conference id
        /// </summary>
        private int _confID { set; get; }

        /// <summary>
        /// conference info
        /// src: AgentEvent_JoinConf
        /// </summary>
        private string _confInfo { set; get; }

        #endregion

        #region  others

        /// <summary>
        /// whether an agnet has snatch pickup right
        /// </summary>
        private bool _hasPickupRight { set; get; }

        /// <summary>
        /// current agent's gourp id
        /// </summary>
        private int _gourpID { set; get; }

        /// <summary>
        /// when qry current agnet's group info failed,then set _gourpID = SPEC_GROUPID
        /// </summary>
        private const int SPEC_GROUPID = -101;

        /// <summary>
        /// log datagrid columns headers
        /// </summary>
        private List<string> m_lstColumnHeader = new List<string>() 
        {
             LanguageResource.FindResourceMessageByKey("otherfunc_logtime"), 
             LanguageResource.FindResourceMessageByKey("otherfunc_logtype"),
             LanguageResource.FindResourceMessageByKey("otherfunc_logcontent"),  
        };

        #endregion

        #endregion

        #region  内部事件与方法

        public TextChatWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            _chatWindow = this;
            GetCurrentAgentInfo();
            InitialConfBusiness();
            ChatDictWithMsg = new Dictionary<string, string>();
            this.ShowInTaskbar = false;

            if (dgrd_loginfo != null && dgrd_loginfo.Columns != null && m_lstColumnHeader.Count == dgrd_loginfo.Columns.Count)
            {
                for (int i = 0; i < dgrd_loginfo.Columns.Count; i++)
                {
                    dgrd_loginfo.Columns[i].Header = m_lstColumnHeader[i];
                }
            }
        }

        #region  TextChat

        /// <summary>
        /// msg send click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_msgsend_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentCallID))
            {
                MessageBox.Show("Please select a call.");
                return;
            }
            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).TextChatSendMsg(CurrentCallID, tb_msgsend.Text);
            Log4NetHelper.ActionLog("Agw", "Tc_SendMsg", res);

            if (string.Equals(res + "", AGWErrorCode.OK))
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    tb_msgsend.Text = string.Empty;
                }), null);
            }

            var data = string.Format(m_msgSuccFormat, "self", DateTime.Now.ToString(), tb_msgsend.Text);
            if (!ChatDictWithMsg.ContainsKey(CurrentCallID))
            {
                return;
            }
            var txt = ChatDictWithMsg[CurrentCallID];
            ChatDictWithMsg[CurrentCallID] = string.IsNullOrEmpty(txt) ? data : txt + Environment.NewLine + data;
            if (lst_callid.SelectedItem != null && lst_callid.SelectedItem.ToString() == CurrentCallID)
            {
                tb_msgshow.Text = ChatDictWithMsg[CurrentCallID];
            }
        }

        /// <summary>
        /// end textchat click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_endchat_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentCallID))
            {
                MessageBox.Show("Please select a call.");
                return;
            }
            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).CloseTextChat(CurrentCallID);
            Log4NetHelper.ActionLog("Agw", "Tc_EndChat", res);
        }

        /// <summary>
        /// inner textchat click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_innerCall_Click(object sender, RoutedEventArgs e)
        {
            var win = new TC_Common();
            win.ShowDialog();
            if (win.IsConfirm)
            {
                var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).InnerTextChat(win.WorkNo);
                Log4NetHelper.ActionLog("Agw", "Tc_InnerChat", res);
            }
        }

        /// <summary>
        /// transfer textchat click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_calltrans_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentCallID))
            {
                MessageBox.Show("Please select a call.");
                return;
            }
            var win = new TC_CallTrans();
            win.ShowDialog();
            if (win.IsConfirm)
            {
                var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).TransTextChat(win.TransAddrType, win.Dest, CurrentCallID, string.Empty, win.TransType);
                Log4NetHelper.ActionLog("Agw", "Tc_TransChat", res);
            }
        }

        /// <summary>
        /// upload click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click</param>
        private void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentCallID))
            {
                MessageBox.Show("Please select a call.");
                return;
            }
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            if (string.IsNullOrEmpty(dlg.FileName))
            {
                return;
            }

            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).UploadMediaFile(CurrentCallID, dlg.FileName);
            var ret = HuaweiAgentGateway.Utils.JsonUtil.DeJsonEx<AgentGatewayResponse<TCUploadRes>>(res);
            if (null == ret)
            {
                return;
            }
            Log4NetHelper.ActionLog("Agw", "Tc_Upload", ret.retcode);
        }

        /// <summary>
        /// download click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_download_Click(object sender, RoutedEventArgs e)
        {
            var filedownloadwin = new TC_FileDownload();
            filedownloadwin.ShowDialog();
            if (!filedownloadwin.IsConfirm)
            {
                return;
            }
            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).DownloadMediaFile(filedownloadwin.FilePath);

            if (null == res)
            {
                return;
            }
            byte[] byteArr = new byte[res.Count];
            for (int i = 0; i < res.Count; i++)
            {
                byteArr[i] = res[i];
            }
            FileStream fs = new FileStream(filedownloadwin.FileName, FileMode.Create, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(byteArr);
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("Download File：" + exc.Message);
            }
            finally
            {
                bw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// qry call content button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_qrycallcontent_Click(object sender, EventArgs e)
        {
            var win = new TC_CallContent();
            win.ShowDialog();
        }

        /// <summary>
        /// qry call session button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_qrycallsession_Click(object sender, EventArgs e)
        {
            var win = new TC_CallSession();
            win.ShowDialog();
        }

        /// <summary>
        /// sms window button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_sms_Click(object sender, EventArgs e)
        {
            var win = new TC_Sms();
            win.ShowDialog();
            if (!win.IsConfirm || string.IsNullOrEmpty(win.Receiver))
            {
                return;
            }
            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).SendMsgWithoutCall(win.SkillID, win.ChatID,
                win.Receiver, win.SmsContent, win.JauIP, win.JauPort);
            Log4NetHelper.ActionLog("Agw", "SendSms", res);

            if (!AGWErrorCode.OK.Equals(res))
            {
                return;
            }
            var msg = string.Format(m_msgSuccFormat, "self", DateTime.Now.ToString(), win.SmsContent);
            if (!ChatDictWithMsg.ContainsKey(win.Receiver))
            {
                ChatDictWithMsg.Add(win.Receiver, msg);
            }
            else
            {
                var txt = ChatDictWithMsg[win.Receiver];
                ChatDictWithMsg[win.Receiver] = string.IsNullOrEmpty(txt) ? msg : txt + Environment.NewLine + msg;
            }
            if (null != lst_callid.SelectedItem && lst_callid.SelectedItem.ToString() == win.Receiver)
            {
                tb_msgshow.Text = ChatDictWithMsg[win.Receiver];
            }
            UIViewHelper.SelectorResetSource(lst_callid, ChatDictWithMsg.Keys);
        }

        private void ClearTextChatInfo()
        {
            if (null != m_logInfoParm)
            {
                m_logInfoParm.Clear();
            }
        }

        #endregion

        #region  QualityControl

        /// <summary>
        /// click evnet used for qc button
        /// </summary>
        /// <param name="sender">qc button object</param>
        /// <param name="e">click event</param>
        private void QC_ButtonClick(object sender, EventArgs e)
        {
            if (!(sender is Button) || string.IsNullOrEmpty(agentInfoCtrl.WorkNo))
            {
                return;
            }
            var item = sender as Button;
            var res = string.Empty;
            switch (item.Name)
            {
                case "btn_qc_listen":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).AddSupervise(agentInfoCtrl.WorkNo);
                    break;
                case "btn_qc_insert":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).AddInsert(agentInfoCtrl.WorkNo);
                    break;
                case "btn_qc_intercept":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).AddIntercept(agentInfoCtrl.WorkNo);
                    break;
                case "btn_qc_reqwhisper":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).ReqWhisper(agentInfoCtrl.WorkNo);
                    break;
                case "btn_qc_stopwhisper":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).StopWhisper(agentInfoCtrl.WorkNo);
                    break;
                case "btn_qc_switchlst":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).SwitchWhisper(agentInfoCtrl.WorkNo, "0");
                    break;
                case "btn_qc_switchint":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).SwitchWhisper(agentInfoCtrl.WorkNo, "1");
                    break;
                case "btn_qc_switchwhis":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).SwitchWhisper(agentInfoCtrl.WorkNo, "2");
                    break;
                case "btn_qc_stopinsertlisten":
                    res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).StopInsertListen(agentInfoCtrl.WorkNo);
                    break;
                default:
                    break;
            }
            Log4NetHelper.BaseLog("[" + item.Name + "] rslt:" + res);
        }

        private void QC_UIFresh_Click(object sender, EventArgs e)
        {
            var tempList = new List<AgentStateInfo>();
            var result = BusinessAdapter.GetBusinessInstance().GetAllAgentStatusInfo();
            if (null == result || result.Count == 0)
            {
                return;
            }
            for (int i = 0; i < result.Count; ++i)
            {
                AgentStateInfo info = result[i];
                if (!info.status.Equals(Constant.AgentStatus_SignOut))
                {
                    tempList.Add(info);
                }
            }
            agentInfoCtrl.SetSource(tempList);
        }

        private void btn_clearLog_Click(object sender, EventArgs e)
        {
            ClearLogInfo();
        }

        #endregion

        #region  Conferences

        private void InitialConfBusiness()
        {
            try
            {
                if (null == _axConf)
                {
                    _axConf = new AxConferenceLib.AxConference();
                }
                if (null == _confBus)
                {
                    _confBus = new ConferenceBusiness();
                    _confBus.Initial(_axConf);
                    _confBus.AttachEvent(this);
                }

                _formsHost = new WindowsFormsHost();
                _formsHost.Child = _axConf;
                _axConf.BeginInit();
                confGrid.Children.Add(_formsHost);
                _axConf.EndInit();
                //_isConfInitSucc = true;
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("[conf_init_exce] Msg:" + exc.Message);
            }
        }

        /// <summary>
        /// conf click handle center
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfClickHandleCenter(object sender, RoutedEventArgs e)
        {
            try
            {
                if (null == _confBus || !(sender is Button))  // !_isConfInitSucc ||
                {
                    return;
                }
                var btnObj = sender as Button;
                var res = string.Empty;

                switch (btnObj.Name)
                {
                    case "btn_confleave":
                        res = _confBus.LeaveConf();
                        break;
                    case "btn_confexit":
                        res = _confBus.TerminateConf();
                        break;
                    case "btn_reqaction":
                        res = _confBus.RequestOperationPrivilege(PRIVILEGE_TYPE);
                        break;
                    case "btn_setactiontype":
                        var action = m_privilegeAction[cb_actiontype.SelectedIndex];
                        res = _confBus.SetOperationPrivilege(_reqPriUserID, PRIVILEGE_TYPE, action);
                        break;
                    case "btn_startsharescreen":
                        _isSelfInShare = true;
                        var screenShareType = (m_screenShareType[cb_screensharetype.SelectedIndex]);
                        res = _confBus.StartShareScreen(screenShareType);
                        break;
                    case "btn_stopsharescreen":
                        res = _confBus.StopShareScreen();
                        break;
                    case "btn_qryapps":
                        _isSelfInShare = false;
                        var qryRes = _confBus.GetApplicationList();
                        var ret = JsonUtil.DeJsonEx<LocalApplication>(qryRes);
                        if (null == ret || null == ret.appInfoList || ret.appInfoList.Count == 0)
                        {
                            break;
                        }
                        _localApps = ret.appInfoList;
                        var src = ret.appInfoList.Select(item => item.title).ToList();
                        UIViewHelper.SelectorResetSource(cb_localapps, src);
                        cb_localapps.SelectedIndex = 0;
                        break;
                    case "btn_filesend":
                        if (string.IsNullOrEmpty(tb_sendfilepath.Text))
                        {
                            break;
                        }
                        res = _confBus.SendFile("0", tb_sendfilepath.Text);
                        break;
                    case "btn_filerecv":
                        if (string.IsNullOrEmpty(tb_savefilepath.Text) || string.IsNullOrEmpty(_recvFileHandle))
                        {
                            break;
                        }
                        res = _confBus.RevFile(_recvFileHandle, tb_savefilepath.Text);
                        break;
                    case "btn_fileshare":
                        if (string.IsNullOrEmpty(tb_sendfilepath.Text))
                        {
                            break;
                        }
                        SetFileShareWndSizeAndShow();
                        res = _confBus.ShareFileOpen(tb_sendfilepath.Text);
                        break;
                    case "btn_shareclose":
                    case "btn_sharesave":
                    case "btn_prepage":
                    case "btn_nextpage":
                    case "btn_handmove":
                        if (string.IsNullOrEmpty(_shareFileID))
                        {
                            break;
                        }
                        res = ShareFileHandleCenter(btnObj.Name);
                        break;
                    default:
                        break;
                }
                Log4NetHelper.BaseLog("[" + btnObj.Name + "] Rslt:" + res);
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("[conf_click_exce] Msg:" + exc.Message);
            }
        }

        private string ShareFileHandleCenter(string type)
        {
            var res = string.Empty;
            switch (type)
            {
                case "btn_shareclose":
                    res = _confBus.StopShareFile(_shareFileID);
                    break;
                case "btn_sharesave":
                    res = _confBus.ShareFileSave(_shareFileID, tb_savefilepath.Text);
                    break;
                case "btn_prepage":
                    res = _confBus.ShareFilePrePage(_shareFileID, FILE_SYNC);
                    break;
                case "btn_nextpage":
                    res = _confBus.ShareFileNextPage(_shareFileID, FILE_SYNC);
                    break;
                case "btn_handmove":
                    res = _confBus.ShareFileAutoMoveByHand("1", FILE_SYNC);
                    break;
                default:
                    break;
            }
            return res;
        }

        /// <summary>
        /// set share screen wnd & size and show
        /// </summary>
        private void SetScreenShareWndSizeAndShow()
        {
            try
            {
                if (null == _screenShareWin)
                {
                    _screenShareWin = new ConfShareWindow(WIN_WIDTH, WIN_HEIGHT);
                }
                _screenShareWin.Show();
                _screenShareWin.Close();

                if (null != _confBus)
                {
                    var ptr = new WindowInteropHelper(_screenShareWin).Handle;
                    var wndRes = _confBus.SetDisplayShareScreenWnd(ptr + "");
                    Log4NetHelper.ActionLog("Conf", "SetDisplayShareScreenWnd", wndRes);
                    var setSizeRes = _confBus.SetShareScreenDisplaySize(WIN_WIDTH + "", WIN_HEIGHT + "");
                    Log4NetHelper.ActionLog("Conf", "SetDisplayShareScreenSize", setSizeRes);
                }
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("[Conf_SetScreenShareWin_exce] Msg:" + exc.Message);
            }
        }

        /// <summary>
        /// set share file wnd & size and show
        /// </summary>
        private void SetFileShareWndSizeAndShow()
        {
            try
            {
                if (_fileShareWin == null)
                {
                    _fileShareWin = new ConfShareWindow(WIN_WIDTH, WIN_HEIGHT);
                }
                _fileShareWin.Show();

                if (null != _confBus)
                {
                    var winPtr = new WindowInteropHelper(_fileShareWin).Handle;
                    var resSetHwd = _confBus.ShareFileSetDisplayWnd(winPtr + "");
                    Log4NetHelper.ActionLog("Conf", "SetFileShareDisplayWnd", resSetHwd);
                    var setDisRes = _confBus.ShareFileSetDisplaySize(WIN_WIDTH + "", WIN_HEIGHT + "");
                    Log4NetHelper.ActionLog("Conf", "SetFileShareDisplaySize", setDisRes);
                }
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("[Conf_SetFileShareWin_exce] Msg:" + exc.Message);
            }
        }

        private void lst_confers_LostFocus(object sender, RoutedEventArgs e)
        {
            lst_confers.SelectedIndex = -1;
        }

        private void btn_selectfile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            tb_sendfilepath.Text = dlg.FileName;
        }

        /// <summary>
        /// save file select click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_savefileselect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.ShowDialog();
            tb_savefilepath.Text = dlg.FileName;
        }

        private void btn_openfilesharewindow_Click(object sender, RoutedEventArgs e)
        {
            if (null == _screenShareWin)
            {
                _screenShareWin = new ConfShareWindow();
            }
            _screenShareWin.Show();
        }

        #region  Conference Events

        /// <summary>
        /// conference events handle center
        /// </summary>
        /// <param name="type">event type</param>
        /// <param name="info">event info</param>
        private void ConfEventsHandleCenter(string type, string info)
        {
            try
            {
                if (!string.IsNullOrEmpty(info))
                {
                    info = info.Replace("\n", string.Empty);
                }
                m_logInfoParm.Add(new LogShowParm()
                {
                    LogTime = DateTime.Now.ToString(),
                    LogInfo = info,
                    LogType = "Conf_" + type
                });
                UIViewHelper.SelectorResetSource(dgrd_loginfo, m_logInfoParm);

                switch (type)
                {
                    case "ShareFileOpenEvent":
                        var opnRet = JsonUtil.DeJsonEx<ShareFileOpenInfo>(info);
                        if (null == opnRet || !AGWErrorCode.OK.Equals(opnRet.resultCode))
                        {
                            break;
                        }
                        SetFileShareWndSizeAndShow();
                        _shareFileID = opnRet.fileId;
                        break;
                    case "ShareFileCloseEvent":
                        var clsRet = JsonUtil.DeJsonEx<ShareFileCloseInfo>(info);
                        if (null == clsRet || !AGWErrorCode.OK.Equals(clsRet.resultCode))
                        {
                            break;
                        }
                        if (null != _fileShareWin)
                        {
                            _fileShareWin.Close();
                        }
                        _shareFileID = string.Empty;
                        break;
                    case "FileArrivedEvent":
                        var fileArrRet = JsonUtil.DeJsonEx<FileTranInfo>(info);
                        if (null == fileArrRet || !AGWErrorCode.OK.Equals(fileArrRet.resultCode))
                        {
                            break;
                        }
                        tb_recvfilepath.Text = fileArrRet.fileName;
                        _recvFileHandle = fileArrRet.fileHandle;
                        break;
                    case "MemberLeaveConfEvent":
                        var leavRet = JsonUtil.DeJson<MemberEnterConf>(info);
                        if (null == leavRet || string.IsNullOrEmpty(leavRet.userId))
                        {
                            break;
                        }
                        if (leavRet.isSelf)
                        {
                            var leavRes = BusinessAdapter.GetBusinessInstance().StopMultimediaConf(_confID);
                            Log4NetHelper.ActionLog("Agw", "EndConf", leavRes);
                            ClearConferenceInfo();
                        }
                        m_lstConfers.Remove(leavRet.userId);
                        UIViewHelper.SelectorResetSource(lst_confers, m_lstConfers);
                        break;
                    case "MemberEnterConfEvent":
                        var entRet = JsonUtil.DeJson<MemberEnterConf>(info);
                        if (null == entRet || string.IsNullOrEmpty(entRet.userId) || m_lstConfers.Contains(entRet.userId))
                        {
                            break;
                        }
                        m_lstConfers.Add(entRet.userId);
                        UIViewHelper.SelectorResetSource(lst_confers, m_lstConfers);
                        break;
                    case "JoinConfResultEvent":
                        var joinRes = JsonUtil.DeJsonEx<ConfInfo>(info);
                        if (null == joinRes || !AGWErrorCode.OK.Equals(joinRes.resultCode + ""))
                        {
                            break;
                        }
                        var res = BusinessAdapter.GetBusinessInstance().JoinMultimediaConfResponse(_confID, joinRes.resultCode, joinRes.resultCode);
                        Log4NetHelper.ActionLog("AgwConf", "JoinResponse", res);
                        break;
                    case "OperationPrivilegeRequestEvent":
                        var operRet = JsonUtil.DeJsonEx<OperPriReqParm>(info);
                        if (null == operRet)
                        {
                            break;
                        }
                        _reqPriUserID = operRet.userId;
                        break;
                    case "ShareScreenStateNotifyEvent":
                        var shareScrRet = JsonUtil.DeJsonEx<ShareScreenStateData>(info);
                        if (null != shareScrRet && string.Compare(shareScrRet.state, SCREENSHARE_START, true) == 0)
                        {
                            SetScreenShareWndSizeAndShow();
                            if (_isSelfInShare && 1 == cb_screensharetype.SelectedIndex)
                            {
                                if (null == _localApps || 0 == _localApps.Count)
                                {
                                    break;
                                }
                                var app = _localApps[cb_localapps.SelectedIndex];
                                var addAppRes = _confBus.AddApplicationToShare(app.hwnd + "");
                                Log4NetHelper.ActionLog("Conf", "AddApp", addAppRes);
                            }
                        }
                        if (null != shareScrRet && string.Compare(shareScrRet.state, SCREENSHARE_STOP, true) == 0 && _screenShareWin != null)
                        {
                            if (_isSelfInShare && 1 == cb_screensharetype.SelectedIndex)
                            {
                                if (null == _localApps || 0 == _localApps.Count)
                                {
                                    break;
                                }
                                var app = _localApps[cb_localapps.SelectedIndex];
                                var addAppRes = _confBus.DeleteSharedApplication(app.hwnd + "");
                                Log4NetHelper.ActionLog("Conf", "DeleteApp", addAppRes);
                            }
                            _screenShareWin.Close();
                        }
                        break;
                    case "TerminateConfResultEvent":
                        var terRet = JsonUtil.DeJsonEx<ConfInfo>(info);
                        if (null == terRet || !AGWErrorCode.OK.Equals(terRet.resultCode + ""))
                        {
                            break;
                        }
                        var terRes = BusinessAdapter.GetBusinessInstance().StopMultimediaConf(_confID);
                        Log4NetHelper.ActionLog("Agw", "EndConf", terRes);
                        ClearConferenceInfo();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
        }

        public void ShareFileCloseEvent(string info)
        {
            ConfEventsHandleCenter("ShareFileCloseEvent", info);
        }

        public void ShareFileCurrentPageEvent(string info)
        {
            ConfEventsHandleCenter("ShareFileCurrentPageEvent", info);
        }

        public void ShareFileOpenEvent(string info)
        {
            ConfEventsHandleCenter("ShareFileOpenEvent", info);
        }

        public void ShareFileLoadingProgressEvent(string info)
        {
            ConfEventsHandleCenter("ShareFileLoadingProgressEvent", info);
        }

        public void ConfNetworkQosNotifyEvent(string info)
        {
            ConfEventsHandleCenter("NetworkQosNotifyEvent", info);
        }

        public void ShareScreenWndSizeEvent(string info)
        {
            ConfEventsHandleCenter("ShareScreenWndSizeEvent", info);
        }

        public void OperationPrivilegeStateEvent(string info)
        {
            ConfEventsHandleCenter("OperationPrivilegeStateEvent", info);
        }

        public void OperationPrivilegeRequestEvent(string info)
        {
            ConfEventsHandleCenter("OperationPrivilegeRequestEvent", info);
        }

        public void ShareScreenStateNotifyEvent(string info)
        {
            ConfEventsHandleCenter("ShareScreenStateNotifyEvent", info);
        }

        public void SharingOwnerNotifyEvent(string info)
        {
            ConfEventsHandleCenter("SharingOwnerNotifyEvent", info);
        }

        public void StartShareScreenResultEvent(string info)
        {
            ConfEventsHandleCenter("StartShareScreenResultEvent", info);
        }

        public void FileArrivedEvent(string info)
        {
            ConfEventsHandleCenter("FileArrivedEvent", info);
        }

        public void FileTranOverEvent(string info)
        {
            ConfEventsHandleCenter("FileTranOverEvent", info);
        }

        public void FileTranProgressEvent(string info)
        {
            ConfEventsHandleCenter("FileTranProgressEvent", info);
        }

        public void MessageArrivedEvent(string info)
        {
            ConfEventsHandleCenter("MessageArrivedEvent", info);
        }

        public void ConfVideoFlowWarningEvent(string info)
        {
            ConfEventsHandleCenter("VideoFlowWarningEvent", info);
        }

        public void ConfVideoNotifyEvent(string info)
        {
            ConfEventsHandleCenter("VideoNotifyEvent", info);
        }

        public void ConfVideoReconnectedEvent()
        {
            ConfEventsHandleCenter("VideoReconnectedEvent", "ConfVideoReconnectedEvent");
        }

        public void ConfVideoDisconnectedEvent()
        {
            ConfEventsHandleCenter("VideoDisconnectedEvent", "ConfVideoDisconnectedEvent");
        }

        public void VideoSwitchEvent(string info)
        {
            ConfEventsHandleCenter("VideoSwitchEvent", info);
        }

        public void ConfRemainingTimeEvent(string info)
        {
            ConfEventsHandleCenter("RemainingTimeEvent", info);
        }

        public void ConfNetWorkStatusEvent(string info)
        {
            ConfEventsHandleCenter("NetWorkStatusEvent", info);
        }

        public void LoadComponentFailedEvent(string info)
        {
            ConfEventsHandleCenter("LoadComponentFailedEvent", info);
        }

        public void ConfNetWorkReconnectedEvent()
        {
            ConfEventsHandleCenter("NetWorkReconnectedEvent", "ConfNetWorkReconnectedEvent");
        }

        public void ConfNetWorkDisconnectedEvent()
        {
            ConfEventsHandleCenter("NetWorkDisconnectedEvent", "ConfNetWorkDisconnectedEvent");
        }

        public void MemberLeaveConfEvent(string info)
        {
            ConfEventsHandleCenter("MemberLeaveConfEvent", info);
        }

        public void MemberEnterConfEvent(string info)
        {
            ConfEventsHandleCenter("MemberEnterConfEvent", info);
        }

        public void ConfInitResultEvent(string info)
        {
            ConfEventsHandleCenter("InitResult", info);
        }

        public void JoinConfResultEvent(string info)
        {
            ConfEventsHandleCenter("JoinConfResultEvent", info);
        }

        public void TerminateConfResultEvent(string info)
        {
            ConfEventsHandleCenter("TerminateConfResultEvent", info);
        }

        private void ClearConferenceInfo()
        {
            UIViewHelper.SelectorResetSource(lst_confers, null);
            tb_recvfilepath.Text = string.Empty;
            tb_savefilepath.Text = string.Empty;
            tb_sendfilepath.Text = string.Empty;
            _reqPriUserID = string.Empty;
            _shareFileID = string.Empty;
            _localApps = new List<AppDetail>();
            _recvFileHandle = string.Empty;
            //tb_conferinfo.Text = string.Empty;
            m_lstConfers.Clear();
            if (null != _screenShareWin)
            {
                _screenShareWin.Close();
            }
            if (null != _fileShareWin)
            {
                _fileShareWin.Close();
            }
        }

        #endregion

        #endregion

        #region  Others

        /// <summary>
        /// pick up click event
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_phonepickup_Click(object sender, RoutedEventArgs e)
        {
            var ins = BusinessAdapter.GetBusinessInstance() as AgentGatewayBusiness;
            if (null == ins)
            {
                return;
            }
            var res = ins.PhonePickUp();
            Log4NetHelper.ActionLog("Agw", "PickUp", res);
        }

        public void ClearLogInfo()
        {
            if (null != m_logInfoParm)
            {
                m_logInfoParm.Clear();
            }
            UIViewHelper.SelectorResetSource(dgrd_loginfo, null);
            tb_logdetail.Text = string.Empty;
        }

        /// <summary>
        /// clear other info
        /// </summary>
        /// <remarks>
        /// textchat,quality control,conference
        /// </remarks>
        public void ClearOtherInfo()
        {
            // text chat
            if (null != ChatDictWithMsg)
            {
                ChatDictWithMsg.Clear();
            }
            CurrentCallID = string.Empty;
            tb_msgsend.Text = string.Empty;
            tb_msgshow.Text = string.Empty;
            UIViewHelper.SelectorResetSource(lst_callid, null);
            // conference
            if (null != _localApps)
            {
                _localApps.Clear();
            }
            _isSelfInShare = false;
            _reqPriUserID = string.Empty;
            _shareFileID = string.Empty;
            _recvFileHandle = string.Empty;
            tb_recvfilepath.Text = string.Empty;
            tb_savefilepath.Text = string.Empty;
            tb_sendfilepath.Text = string.Empty;
            //_isConfInitSucc = false;
            _confID = 0;
            _confInfo = string.Empty;
            UIViewHelper.SelectorResetSource(lst_confers, null);
            UIViewHelper.SelectorResetSource(cb_localapps, null);
            // quality control
            agentInfoCtrl.SetSourceEx(null);
            _gourpID = 0;
            _hasPickupRight = false;
        }

        /// <summary>
        /// get current agent workgroup id
        /// </summary>
        private void GetCurrentAgentInfo()
        {
            _hasPickupRight = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).HasPickUpRight();
            var info = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).GetCurrentAgentWorkGroupDetail();
            if (null == info)
            {
                Log4NetHelper.BaseLog("[qry_current_workgourp_failed]");
                _gourpID = SPEC_GROUPID;
                return;
            }
            _gourpID = info.id;
        }

        /// <summary>
        /// snatch pickup
        /// </summary>
        /// <remarks>
        /// 1,select agent
        /// 2,has pickup right
        /// 3,agents are in the same workgoup(only qry succ and group ids are not same then return)
        /// </remarks>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SnatchPickup_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(agentInfoCtrl.WorkNo))
            {
                return;
            }
            if (!_hasPickupRight)
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("otherfunc_pickup_noright"));
                return;
            }

            var info = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).GetAgentWorkGroupDetail(agentInfoCtrl.WorkNo);
            if (_gourpID != SPEC_GROUPID && info != null && _gourpID != info.id)
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("otherfunc_pickup_nosamegroup"));
                return;
            }
            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).SnatchPickUp(agentInfoCtrl.WorkNo);
            Log4NetHelper.BaseLog("[snatch_pickup] Rslt:" + res);
            if (!AGWErrorCode.OK.Equals(res))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("otherfunc_pickup_fail") + res);
            }
        }

        #endregion

        /// <summary>
        /// msg clear button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_msgclr_Click(object sender, RoutedEventArgs e)
        {
            tb_msgshow.Text = string.Empty;
        }

        private void lst_callid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lst_callid.SelectedItem == null)
            {
                return;
            }
            if (ChatDictWithMsg.ContainsKey(lst_callid.SelectedItem.ToString()))
            {
                tb_msgshow.Text = ChatDictWithMsg[lst_callid.SelectedItem.ToString()];
            }
            CurrentCallID = lst_callid.SelectedItem.ToString();
        }

        /// <summary>
        /// datagrid selection changed event
        /// </summary>
        /// <param name="sender">datagrid</param>
        /// <param name="e">selection changed event</param>
        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = dgrd_loginfo.SelectedItem as LogShowParm;
            if (null == item)
            {
                return;
            }
            tb_logdetail.Text = item.LogInfo;
        }

        /// <summary>
        /// current window closing event
        /// </summary>
        /// <param name="e">cancel event</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion

        #region  供外部调用的方法

        public static TextChatWindow GetInstance()
        {
            if (null == _chatWindow)
            {
                _chatWindow = new TextChatWindow();
            }
            return _chatWindow;
        }

        /// <summary>
        /// receive sms message
        /// </summary>
        /// <param name="caller">sms caller</param>
        /// <param name="content">sms content</param>
        public void ReceiveSmsMsg(string caller, string content)
        {
            TextChatBuild(caller);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var msg = string.Format(m_smsMsgFormat, caller, DateTime.Now.ToString(), content);
                if (!ChatDictWithMsg.ContainsKey(caller))
                {
                    ChatDictWithMsg.Add(caller, msg);
                }
                else
                {
                    var txt = ChatDictWithMsg[caller];
                    ChatDictWithMsg[caller] = string.IsNullOrEmpty(txt) ? msg : txt + Environment.NewLine + msg;
                }

                if (null != lst_callid.SelectedItem && lst_callid.SelectedItem.ToString() == caller)
                {
                    tb_msgshow.Text = ChatDictWithMsg[caller];
                }
                UIViewHelper.SelectorResetSource(lst_callid, ChatDictWithMsg.Keys);
            }), null);
        }

        public void CloseTextChat(string callID)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (ChatDictWithMsg.ContainsKey(callID))
                    ChatDictWithMsg.Remove(callID);

                UIViewHelper.SelectorResetSource(lst_callid, ChatDictWithMsg.Keys);
                CurrentCallID = string.Empty;
            }), null);
        }

        public void TextChatBuild(string callID)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!ChatDictWithMsg.ContainsKey(callID))
                    ChatDictWithMsg.Add(callID, string.Empty);

                UIViewHelper.SelectorResetSource(lst_callid, ChatDictWithMsg.Keys);
            }), null);
        }

        public void SetInfoContent(string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                return;
            }

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var ret = HuaweiAgentGateway.Utils.JsonUtil.DeJsonEx<AgwCommonRes<AgentEventResult<object>>>(info);
                var item = new LogShowParm()
                {
                    LogTime = DateTime.Now.ToString(),
                    LogInfo = info,
                    LogType = ret != null && ret.@event != null ? ret.@event.eventType : string.Empty
                };
                m_logInfoParm.Add(item);
                UIViewHelper.SelectorResetSource(dgrd_loginfo, m_logInfoParm);
            }), null);
        }

        public void TextChatRing(string callID)
        {
            CurrentCallID = callID;
            var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).AnswerTextChat(callID);
            Log4NetHelper.ActionLog("Common", "Tc_Answer", res);
        }

        public void RecvMsg(string callID, string sender, string content, bool isSucc)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                var msg_format = isSucc ? m_msgSuccFormat : m_msgFailFormat;
                var data = string.Format(msg_format, sender, DateTime.Now.ToString(), content);
                if (!ChatDictWithMsg.ContainsKey(callID)) return;
                var txt = ChatDictWithMsg[callID];
                ChatDictWithMsg[callID] = string.IsNullOrEmpty(txt) ? data : txt + Environment.NewLine + data;

                if (lst_callid.SelectedItem != null && lst_callid.SelectedItem.ToString() == callID)
                {
                    tb_msgshow.Text = ChatDictWithMsg[callID];
                }
            }), null);
        }

        /// <summary>
        /// set conf id & info and join conf
        /// </summary>
        /// <param name="ID">conf ID</param>
        /// <param name="info">conf info</param>
        public void SetConfInfoAndJoin(int ID, string info)
        {
            this._confID = ID;
            this._confInfo = info;
            if (null == _confBus) // || !_isConfInitSucc)
            {
                return;
            }
            var res = _confBus.JoinConf(info);
            Log4NetHelper.ActionLog("Conf", "Join", res);
        }

        /// <summary>
        /// terminate conference 
        /// </summary>
        public void TernimateConference()
        {
            if (null == _confBus || 0 == _confID)
            {
                return;
            }
            var res = _confBus.TerminateConf();
            Log4NetHelper.ActionLog("Conf", "Terminate", res);
        }

        #endregion
    }
}
