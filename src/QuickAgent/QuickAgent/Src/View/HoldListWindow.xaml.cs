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
using QuickAgent.Src.Common;
using HuaweiAgentGateway.AgentGatewayEntity;
using QuickAgent.View;
using System.Collections.ObjectModel;
using HuaweiAgentGateway;
using QuickAgent.Common;
using QuickAgent.Constants;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 三方通话,连接保持 
    /// </summary>
    public partial class HoldListWindow : Window
    {
        private ObservableCollection<HoldCallInfo> holdInfoList = null;
        private static HoldListWindow _instance = null;
        private OperateType _myType = OperateType.Tripartite;
        private List<HoldListData> holdList = null;

        public enum OperateType
        {
            Tripartite = 0,
            ConnectHold = 1
        }

        public static HoldListWindow Instance(OperateType operateType)
        {
            if (null == _instance)
            {
                _instance = new HoldListWindow(operateType);
            }
            return _instance;
        }

        public OperateType MyType
        {
            get
            {
                return _myType;
            }
            set
            {
                _myType = value;
            }
        }

        public HoldListWindow(OperateType operateType)
        {
            _myType = operateType;
            this.Loaded += new RoutedEventHandler(HoldListWindow_Load);
            LanguageResource.ChangeWindowLanguage(this);
            InitializeComponent();
            this.Closed += new EventHandler(Close);
        }

        private void HoldListWindow_Load(object sender, RoutedEventArgs e)
        {
            this.Title = _myType == OperateType.Tripartite ? LanguageResource.FindResourceMessageByKey("threeCall") : LanguageResource.FindResourceMessageByKey("connectHold");
            holdList = MainWindow.Instance().GetHoldInfoList();
            if (null == holdList || holdList.Count <= 0)
            {
                return;
            }
            holdInfoList = new ObservableCollection<HoldCallInfo>();
            for (int i = 0; i < holdList.Count; ++i)
            {
                HoldListData holdInfo = holdList[i];
                holdInfoList.Add(new HoldCallInfo
                {
                    Number = (i + 1).ToString(),
                    CalledNumber = holdInfo.called,
                    CallerNumber = holdInfo.caller,
                    MediaType = LanguageResource.FindResourceMessageByKey("normalVoiceCall"),
                    callId = holdInfo.callid
                });
            }
            dgrdHoldList.ItemsSource = holdInfoList;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdHoldList.SelectedIndex < 0)
            {
                return;
            }
            string logInfo = string.Empty;
            string result = null;
            int index = dgrdHoldList.SelectedIndex;
            HoldCallInfo curHoldInfo = holdInfoList[index];
            switch (_myType)
            {
                case OperateType.Tripartite:
                    result = BusinessAdapter.GetBusinessInstance().ConfJoinEx(curHoldInfo.callId);
                    Log4NetHelper.ActionLog("Common", "Vc_ConfJoinEx", result);
                    logInfo = "Tripartite Call Failure, ErrorCode : ";
                    break;
                case OperateType.ConnectHold:
                    result = BusinessAdapter.GetBusinessInstance().ConnectHoldEx(curHoldInfo.callId);
                    Log4NetHelper.ActionLog("Common", "Vc_ConnectHold", result);
                    logInfo = "Connect Hold Calling Failure, ErrorCode : ";
                    break;
                default:
                    break;
            }
            if (result != null && !result.Equals(AGWErrorCode.OK))
            {
                MessageBox.Show(logInfo + result);
            }
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Close(object sender, EventArgs e)
        {
            _instance = null;
        }
    }
}
