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
using HuaweiAgentGateway.AgentGatewayEntity;
using QuickAgent.Src.Common;
using QuickAgent.View;
using HuaweiAgentGateway;
using QuickAgent.Constants;
using QuickAgent.Common;
using QuickAgent.Src.UserControl;
using QuickAgent.ViewModel;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 内部求助界面
    /// </summary>
    public partial class InnerConsultWindow : Window
    {
        #region  属性

        private string _skillSelectedId { set; get; }
        private ConsultType _consultType = ConsultType.Agent;
        private static InnerConsultWindow _instance = null;

        /// <summary>
        /// 求助模式：0：求助到座席；1：求助到技能队列
        /// </summary>
        public enum ConsultType
        {
            Agent = 0,
            Skill = 1
        }

        #endregion

        public static InnerConsultWindow Instance()
        {
            if (null == _instance)
            {
                _instance = new InnerConsultWindow();
            }
            return _instance;
        }

        public InnerConsultWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);

            lstSkillsQueue.ItemsSource = MainWindow.Instance().GetSkillInfoList();
            var agentSrc = MainWindow.Instance().GetAgentStateInfoList();
            if (agentSrc != null)
                lstAgentStatusInfo.ItemsSource = agentSrc.Select(item => new AgentStateInfoWrapper(item)).ToList();

            this.Closed += new EventHandler(Close);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("internalhelp_head"));
            tctlConsultType.SelectionChanged += tctlConsultType_SelectionChanged;
        }

        private void lstAgentStatusInfo_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var item = lstAgentStatusInfo.SelectedItem as AgentStateInfoWrapper;
            if (item != null)
                txtAgentId.Text = item.WorkNo;
        }

        private void Close(object sender, EventArgs e)
        {
            _instance = null;
        }

        void lstSkillsQueue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lstSkillsQueue.SelectedItem as SkillInfo;
            if (item != null)
                _skillSelectedId = item.id;
        }

        private void tctlConsultType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _consultType = (ConsultType)tctlConsultType.SelectedIndex;
            switch (_consultType)
            {
                case ConsultType.Agent:
                    txtAgentId.IsEnabled = true;
                    break;
                case ConsultType.Skill:
                    txtAgentId.IsEnabled = false;
                    break;
                default:
                    break;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            AGWInnerHelp innerHelpData = new AGWInnerHelp();
            switch (_consultType)
            {
                case ConsultType.Agent:
                    if (string.IsNullOrEmpty(txtAgentId.Text))
                    {
                        ShowMessage.ShowMessageBox("inputBusinessAgentId");
                        return;
                    }
                    innerHelpData.devicetype = 2;
                    AgentStateInfoWrapper curSelectAgent = lstAgentStatusInfo.SelectedItem as AgentStateInfoWrapper;
                    bool isCall = false;
                    string status = curSelectAgent.status;
                    if (status.Equals("1") || status.Equals("4") || status.Equals("6"))
                    {
                        isCall = true;
                    }
                    if (!isCall)
                    {
                        ShowMessage.ShowMessageBox("isNotIdleAgent");
                        return;
                    }
                    innerHelpData.dstaddress = txtAgentId.Text;
                    break;
                case ConsultType.Skill:
                    if (lstSkillsQueue.SelectedIndex < 0)
                    {
                        ShowMessage.ShowMessageBox("choiceSkillQueue");
                        return;
                    }
                    innerHelpData.devicetype = 1;
                    innerHelpData.dstaddress = _skillSelectedId;

                    break;
                default:
                    break;
            }

            if (rbtnTwoPartiesConsult.IsChecked == true)
            {
                innerHelpData.mode = 1;
            }
            else if (rbtnTripartiteConsult.IsChecked == true)
            {
                innerHelpData.mode = 2;
            }

            string result = BusinessAdapter.GetBusinessInstance().InternalHelp(5, innerHelpData.dstaddress, innerHelpData.mode, innerHelpData.devicetype);
            Log4NetHelper.ActionLog("Common", "Vc_InnerHelp", result);

            if (null == result)
            {
                this.DialogResult = true;
                return;
            }
            if (!result.Equals(AGWErrorCode.OK))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("internalhelp_fail"));
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("internalhelp_fail");
                }
            }
            else
            {
                if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                {
                    MainWindowVM.GetInstance().AgentState = LanguageResource.FindResourceMessageByKey("internalhelp_succ");
                    MainWindow.Instance().TalkStatus = MainWindow.AgentStatus.InternaHelp;
                }
            }
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
