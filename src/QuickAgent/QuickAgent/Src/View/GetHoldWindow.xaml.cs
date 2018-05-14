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
using QuickAgent.Src.Common;
using QuickAgent.Common;
using QuickAgent.Constants;
using QuickAgent.View;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 取保持界面
    /// </summary>
    public partial class GetHoldWindow : Window
    {
        private List<CallInfo> m_CallInfo { set; get; }

        public GetHoldWindow()
        {
            InitialWindow();
        }

        public GetHoldWindow(List<CallInfo> info)
        {
            InitialWindow();
            m_CallInfo = info;
            dgrdCallList.SetSource(info);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (-1 == dgrdCallList.SelectIndex)
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("setdata_selecttalk"));
                return;
            }
            var data = m_CallInfo[dgrdCallList.SelectIndex];
            string result = BusinessAdapter.GetBusinessInstance().GetHoldEx(data.CallId);
            Log4NetHelper.ActionLog("Common", "Vc_GetHold", result);

            if (!string.IsNullOrEmpty(result) && !AGWErrorCode.OK.Equals(result))
            {
                MainWindow.Instance().MessageBoxForErr(result);
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            m_CallInfo = null;
            Close();
        }

        private void InitialWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }
    }
}
