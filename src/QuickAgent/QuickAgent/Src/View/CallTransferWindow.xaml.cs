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
using QuickAgent.Constants;
using QuickAgent.View;
using HuaweiAgentGateway;
using QuickAgent.Common;
using QuickAgent.Src.UserControl;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 呼叫转移界面 的交互逻辑
    /// </summary>
    public partial class CallTransferWindow : Window
    {
        #region  属性

        /// <summary>
        ///  MCP 媒体类型，默认是 5
        /// </summary>
        private const int OCX_MEDIA_TYPE = 5;

        private TransferType _transferType = TransferType.Agent;

        /// <summary>
        /// 转移类型
        /// 0:转座席
        /// 1:转 IVR
        /// 2:转技能队列
        /// 3:转系统接入码
        /// </summary>
        public enum TransferType
        {
            Agent = 0,
            Ivr = 1,
            Skill = 2,
            AccessCode = 3
        }

        #endregion

        #region  方法

        public CallTransferWindow()
        {
            InitializeComponent();
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("transcall_head"));

            LanguageResource.ChangeWindowLanguage(this);
            LoadAgentStatusInfo();
            lstIVRDevice.ItemsSource = MainWindow.Instance().GetIvrInfoList();
            lstSkillsQueue.ItemsSource = MainWindow.Instance().GetSkillInfoList();
        }

        /// <summary>
        /// 获取座席信息
        /// </summary>
        private void LoadAgentStatusInfo()
        {
            var agentStatusInfoList = MainWindow.Instance().GetAgentStateInfoList();
            var agentSource = new List<AgentStateInfoWrapper>();
            if (null != agentStatusInfoList && 0 < agentStatusInfoList.Count)
            {
                foreach (var item in agentStatusInfoList)
                {
                    agentSource.Add(new AgentStateInfoWrapper(item));
                }
                lstAgentStatusInfo.ItemsSource = agentSource;
            }
        }

        #endregion

        #region  控件事件

        void lstAgentStatusInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lstAgentStatusInfo.SelectedItem as AgentStateInfoWrapper;
            if (item != null)
                txtAgentId.Text = item.WorkNo;
        }

        void lstIVRDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lstIVRDevice.SelectedItem as IvrInfo;
            if (item != null)
                txtIVRAccessCode.Text = item.InNo;
        }

        void lstSkillsQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lstSkillsQueue.SelectedItem as SkillInfo;
            if (item != null)
                txtSkill.Text = item.name;
        }

        private void tctlTransferType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _transferType = (TransferType)tctlTransferType.SelectedIndex; ;
        }

        private void cboTransferWay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            AGWTransferData transferData = new AGWTransferData();
            switch (_transferType)
            {
                case TransferType.Agent:
                    if (string.IsNullOrEmpty(txtAgentId.Text))
                    {
                        ShowMessage.ShowMessageBox("nullTransferObject");
                        return;
                    }
                    transferData.devicetype = 2;
                    transferData.address = txtAgentId.Text;
                    int selectIndex = cboTransferWay.SelectedIndex;
                    if (selectIndex.Equals(0))
                    {
                        transferData.mode = 0;
                    }
                    else if (selectIndex.Equals(1))
                    {
                        transferData.mode = 2;
                    }
                    else if (selectIndex.Equals(2))
                    {
                        transferData.mode = 3;
                    }
                    else if (selectIndex.Equals(3))
                    {
                        transferData.mode = 4;
                    }
                    break;
                case TransferType.Ivr:
                    if (string.IsNullOrEmpty(txtIVRAccessCode.Text))
                    {
                        ShowMessage.ShowMessageBox("nullTransferObject");
                        return;
                    }
                    transferData.devicetype = 3;
                    transferData.address = txtIVRAccessCode.Text;
                    if (rbtnHungUp_IVR.IsChecked == true)
                    {
                        transferData.mode = 1;
                    }
                    else if (rbtnRelease_IVR.IsChecked == true)
                    {
                        transferData.mode = 0;
                    }
                    break;
                case TransferType.Skill:
                    if (string.IsNullOrEmpty(txtSkill.Text))
                    {
                        ShowMessage.ShowMessageBox("nullTransferObject");
                        return;
                    }
                    transferData.devicetype = 1;
                    string id = (lstSkillsQueue.SelectedItem as SkillInfo).id;
                    transferData.address = id;
                    if (rbtnSuccess_Skill.IsChecked == true)
                    {
                        transferData.mode = 2;
                    }
                    else if (rbtnRelease_Skill.IsChecked == true)
                    {
                        transferData.mode = 0;
                    }
                    break;
                case TransferType.AccessCode:
                    if (string.IsNullOrEmpty(txtAccessCode.Text))
                    {
                        ShowMessage.ShowMessageBox("nullTransferObject");
                        return;
                    }
                    transferData.devicetype = 4;
                    transferData.address = txtAccessCode.Text;
                    if (rbtnSuccess_AccessCode.IsChecked == true)
                    {
                        transferData.mode = 2;
                    }
                    else if (rbtnRelease_AccessCode.IsChecked == true)
                    {
                        transferData.mode = 0;
                    }
                    break;
                default:
                    break;
            }

            string result = BusinessAdapter.GetBusinessInstance().TransInnerEx(OCX_MEDIA_TYPE, transferData.mode, transferData.address, transferData.devicetype);
            Log4NetHelper.ActionLog("Common", "Vc_CallTrans", result);
            if (result != null)
            {
                if (!result.Equals(AGWErrorCode.OK))
                {
                    MainWindow.Instance().MessageBoxForErr(result);
                }
            }
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }
}
