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
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuickAgent.Src.Common;
using HuaweiAgentGateway;
using QuickAgent.Src.View;

namespace QuickAgent.Src.UserControl
{
    /// <summary>
    /// 呼叫信息表格控件（用于设置数据,查询数据,取保持,三方通话 界面）
    /// </summary>
    public partial class CallInfoCtrl : System.Windows.Controls.UserControl
    {
        public int SelectIndex { private set; get; }

        public CallInfoCtrl()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            SelectIndex = -1;
            var lstColumnHeader = new List<string> 
            { 
                LanguageResource.FindResourceMessageByKey("indexNo"), 
                LanguageResource.FindResourceMessageByKey("callingNumber"),
                LanguageResource.FindResourceMessageByKey("calledNumber"),  
                LanguageResource.FindResourceMessageByKey("mediaType")
            };
            if (dgrdCallList != null && dgrdCallList.Columns != null)
            {
                for (int i = 0; i < dgrdCallList.Columns.Count; i++)
                {
                    dgrdCallList.Columns[i].Header = lstColumnHeader[i];
                }
            }
        }

        public void SetSource(List<CallInfo> info)
        {
            if (null == info || info.Count == 0) return;
            var items = new List<CallInfoWrapper>();
            if (info != null)
            {
                for (int i = 0; i < info.Count; i++)
                {
                    items.Add(new CallInfoWrapper(info[i]) { Index = i + 1 });
                }
            }
            dgrdCallList.ItemsSource = items;
            dgrdCallList.SelectedIndex = 0;
            dgrdCallList.Focus();
        }

        private void DataGridSelectionChanged(Object sender, EventArgs e)
        {
            SelectIndex = dgrdCallList.SelectedIndex;
        }
    }
}
