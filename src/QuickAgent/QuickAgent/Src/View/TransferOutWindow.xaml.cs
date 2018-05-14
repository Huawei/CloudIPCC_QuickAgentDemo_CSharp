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
using QuickAgent.ViewModel;
using HuaweiAgentGateway.AgentGatewayEntity;
using QuickAgent.View;
using HuaweiAgentGateway;
using QuickAgent.Common;
using QuickAgent.Constants;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 转出界面 的交互逻辑
    /// </summary>
    public partial class TransferOutWindow : Window
    {
        private const int OCX_MEDIA_TYPE = 5;

        public TransferOutWindow()
        {
            InitializeComponent();

            LanguageResource.ChangeWindowLanguage(this);
            txtCallerNumber.SetText(MainWindowVM.GetInstance().CallerNumber);
            txtCalledNumber.SetText(MainWindowVM.GetInstance().CalledNumber);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("transout_head"));

            txtCallerNumber.SetReadOnly(true);
            txtCalledNumber.SetReadOnly(true);
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtCallNumber.Text))
            {
                ShowMessage.ShowMessageBox("nullTransferObject");
                return;
            }

            AGWTransferData transferData = new AGWTransferData();
            transferData.devicetype = 5;//转给外部号码
            transferData.address = txtCallNumber.Text;
            transferData.caller = txtCallerNumber.Text;
            if (rbtnReleaseTransfer.IsChecked == true)
            {
                transferData.mode = 1;
            }
            else if (rbtnSuccessTransfer.IsChecked == true)
            {
                transferData.mode = 2;
            }
            else if (rbtnCallTransfer.IsChecked == true)
            {
                transferData.mode = 3;
            }
            else if (rbtnTripartiteTransfer.IsChecked == true)
            {
                transferData.mode = 4;
            }

            string result = BusinessAdapter.GetBusinessInstance().TransOutEx(OCX_MEDIA_TYPE, transferData.caller, transferData.address, transferData.mode, 0, "");
            Log4NetHelper.ActionLog("Common", "Vc_TransOut", result);
            if (result != null)
            {
                if (!result.Equals(AGWErrorCode.OK))
                {
                    MessageBox.Show("Transfer out failed.error code : " + result);
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
