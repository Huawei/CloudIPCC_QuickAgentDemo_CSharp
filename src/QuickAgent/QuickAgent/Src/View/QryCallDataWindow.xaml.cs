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

namespace QuickAgent.Src.View
{
    /// <summary>
    /// Qry call data window
    /// </summary>
    public partial class QryCallDataWindow : Window
    {
        #region  构造函数

        public QryCallDataWindow()
        {
            InitializeComponent();
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("setdata_head"));
        }

        public QryCallDataWindow(List<CallInfo> info)
        {
            InitializeComponent();
            var items = new List<CallInfoWrapper>();
            if (info != null)
            {
                for (int i = 0; i < info.Count; i++)
                {
                    items.Add(new CallInfoWrapper(info[i]) { Index = i + 1 });
                }
            }
            dgrdCallList.ItemsSource = items;
            if (null != items && 0 < items.Count)
            {
                dgrdCallList.SelectedItem = dgrdCallList.Items[0];
            }
            dgrdCallList.Focus();

            #region  控件显示

            var lstColumnHeader = new List<string> { 
                LanguageResource.FindResourceMessageByKey("indexNo"), 
                LanguageResource.FindResourceMessageByKey("callingNumber"),
                LanguageResource.FindResourceMessageByKey("calledNumber"),  
                LanguageResource.FindResourceMessageByKey("mediaType")
            };
            this.Title = LanguageResource.FindResourceMessageByKey("qryDataTitle");
            this.btnClose.Content = LanguageResource.FindResourceMessageByKey("close");
            this.txtBlockRes.Text = LanguageResource.FindResourceMessageByKey("qryResult");
            if (dgrdCallList != null && dgrdCallList.Columns != null)
            {
                for (int i = 0; i < dgrdCallList.Columns.Count; i++)
                {
                    dgrdCallList.Columns[i].Header = lstColumnHeader[i];
                }
            }
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("setdata_head"));

            #endregion
        }

        #endregion

        #region  事件

        private void DataGridSelectionChanged(Object sender, EventArgs e)
        {
            var item = dgrdCallList.SelectedItem as CallInfo;
            if (null == item)
            {
                return;
            }
            txtBoxData.Text = BusinessAdapter.GetBusinessInstance().QryData(item.CallId);
        }

        private void BtnClose_Click(Object sender, EventArgs e)
        {
            Close();
        }

        #endregion
    }
}
