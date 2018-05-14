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
using QuickAgent.Src.Common;
using HuaweiAgentGateway.Entity.AGWEntity;
using HuaweiAgentGateway.AgentGatewayEntity;

namespace QuickAgent.Src.View.TextChat
{
    /// <summary>
    /// qry call content window
    /// </summary>
    public partial class TC_CallContent : Window
    {
        public TC_CallContent()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            btn_qry.Click += btn_qry_Click;
            dgrd_callcontent.SelectionChanged += DataGridSelectionChanged;
            var lstColumnHeader = new List<string> 
            { 
                LanguageResource.FindResourceMessageByKey("textchat_id"), 
                LanguageResource.FindResourceMessageByKey("textchat_callID"),
                LanguageResource.FindResourceMessageByKey("textchat_subMediaType"),  
                LanguageResource.FindResourceMessageByKey("textchat_sender"),
                LanguageResource.FindResourceMessageByKey("textchat_receiver"), 
                LanguageResource.FindResourceMessageByKey("textchat_contenttype"), 
                LanguageResource.FindResourceMessageByKey("textchat_content"), 
                LanguageResource.FindResourceMessageByKey("textchat_direction"), 
            };
            if (null == dgrd_callcontent || null == dgrd_callcontent.Columns ||
                lstColumnHeader.Count != dgrd_callcontent.Columns.Count)
            {
                return;
            }
            for (int i = 0; i < dgrd_callcontent.Columns.Count; i++)
            {
                dgrd_callcontent.Columns[i].Header = lstColumnHeader[i];
            }
        }

        /// <summary>
        /// dgrd selection changed event
        /// </summary>
        /// <param name="sender">dgrd</param>
        /// <param name="e">selection changed event</param>
        private void DataGridSelectionChanged(Object sender, EventArgs e)
        {
            var item = dgrd_callcontent.SelectedItem as CallContentData;
            if (null == item)
            {
                return;
            }
            tb_msgshow.Text = HuaweiAgentGateway.Utils.JsonUtil.ToJson(item);
        }

        /// <summary>
        /// qry button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_qry_Click(object sender, EventArgs e)
        {
            tb_msgshow.Text = string.Empty;
            if (string.IsNullOrEmpty(tb_pageNo.Text) || string.IsNullOrEmpty(tb_pageSize.Text))
            {
                return;
            }

            try
            {
                if (BusinessAdapter.CurrentBusinessType == BusinessType.OCX)
                {
                    Log4NetHelper.BaseLog("Instance is ocx");
                    return;
                }
                if (null == BusinessAdapter.GetBusinessInstance())
                {
                    Log4NetHelper.BaseLog("Instance is null");
                    return;
                }

                var res = ((AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance()).QryCallContent(tb_callID.Text, tb_pageSize.Text, tb_pageNo.Text);
                if (null == res)
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("textchat_qryfailed"));
                    return;
                }
                Log4NetHelper.ActionLog("Agw", "QryCallContent", res.retcode);
                if (null == res.result)
                {
                    return;
                }
                lbl_totalPage.Content = res.result.totalPageNo;
                lbl_totalCount.Content = res.result.totalCount;
                UIViewHelper.SelectorResetSource(dgrd_callcontent, res.result.weccCallContentList);
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
        }
    }
}
