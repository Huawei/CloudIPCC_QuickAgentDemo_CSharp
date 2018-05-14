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
using QuickAgent.Common;
using QuickAgent.Constants;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 内部呼叫窗口 
    /// </summary>
    public partial class InnerCall : Window
    {
        #region  属性

        private static int OCX_MEDIA_TYPE = 5;

        #endregion

        #region  方法

        public InnerCall()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            var lstAgent = MainWindow.Instance().GetAgentStateInfoList();
            agentInfoCtrl.SetSource(lstAgent);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("internalcall_head"));
        }

        #endregion

        #region 事件

        public void OK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AgentStateInfo statusInfo = agentInfoCtrl.SelectedAgent;
                if (null == statusInfo)
                {
                    return;
                }
                if (string.Compare(MainWindow.Instance().AgentInfo.AgentId, statusInfo.workno, true) == 0)
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("innercall_choseselef"));
                    return;
                }

                string result = BusinessAdapter.GetBusinessInstance().CallInnerEx(statusInfo.workno, OCX_MEDIA_TYPE);
                Log4NetHelper.ActionLog("Common", "Vc_CallInner", result);

                if (result != null)
                {
                    if (!result.Equals(AGWErrorCode.OK))
                    {
                        MessageBox.Show(LanguageResource.FindResourceMessageByKey("internalcall_fail"));
                    }
                    else
                    {
                        if (BusinessAdapter.CurrentBusinessType == BusinessType.AgentGateway)
                            MainWindow.Instance().TalkStatus = MainWindow.AgentStatus.InternaCall;
                    }
                }
                this.DialogResult = true;
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "InnerCall", exc.Message);
            }
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }
}
