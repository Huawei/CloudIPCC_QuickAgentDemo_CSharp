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
using HuaweiAgentGateway;
using QuickAgent.View;
using QuickAgent.Common;
using QuickAgent.Constants;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// Rest window
    /// </summary>
    public partial class RestWindow : Window
    {
        public bool IsRestSuccess { private set; get; }

        public RestWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this); 
        }

        private void RestWindow_Load(object sender, RoutedEventArgs e)
        {
            LanguageResource.ChangeWindowLanguage(this); 
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            int num;
            bool parseSuccess = int.TryParse(txtTime.Text, out num);
            if (!parseSuccess)
            {
                ShowMessage.ShowMessageBox("contentUnLegal");
                return;
            }
            if (string.IsNullOrEmpty(txtTime.Text) || num < 1 || num > 59)
            {
                ShowMessage.ShowMessageBox("contentUnLegal");
                return;
            }
            string agentId = MainWindow.Instance().AgentInfo.AgentId;
            string result = BusinessAdapter.GetBusinessInstance().RestEx(num * 60, 0);
            Log4NetHelper.ActionLog("Common", "Vc_Rest", result);

            if (result != null)
            {
                IsRestSuccess = result.Equals(AGWErrorCode.OK);
                if (!result.Equals(AGWErrorCode.OK))
                {                  
                    MessageBox.Show("Rest failed,Error Code ; " + result);
                }
                else
                {
                    MainWindow.Instance().RestTime = num.ToString();
                }
            }
            this.DialogResult = true;
        }

        private void txtTime_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!CommonHelper.IsDigit(e.Key))
            {
                e.Handled = true;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
