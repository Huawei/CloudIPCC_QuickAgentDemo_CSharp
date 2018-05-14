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

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 消息查看窗口，收到的公告，便签会展示在此窗口
    /// </summary>
    public partial class MsgViewWindow : Window
    {
        private static List<MessageInfo> m_MsgInfo { set; get; }

        #region  方法

        public MsgViewWindow()
        {
            InitializeComponent();
            if (m_MsgInfo == null)
                m_MsgInfo = new List<MessageInfo>();

            #region  控件设置

            LanguageResource.ChangeWindowLanguage(this);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("msgview_head"));

            var lstColumnHeader = new List<string> { 
                LanguageResource.FindResourceMessageByKey("msgview_time"), 
                LanguageResource.FindResourceMessageByKey("msgview_sender"),
                LanguageResource.FindResourceMessageByKey("msgview_type"),  
            };
            if (dgrdMsgList != null && dgrdMsgList.Columns != null)
            {
                for (int i = 0; i < dgrdMsgList.Columns.Count; i++)
                {
                    dgrdMsgList.Columns[i].Header = lstColumnHeader[i];
                }
            }

            #endregion
        }

        public void AddMsgInfo(MessageInfo msg)
        {
            if (m_MsgInfo == null)
                m_MsgInfo = new List<MessageInfo>();
            m_MsgInfo.Insert(0, msg);

            dgrdMsgList.ItemsSource = m_MsgInfo;
            txtBoxMsg.Text = m_MsgInfo.Count == 0 ? string.Empty : m_MsgInfo[0].Data;
        }

        #endregion

        #region  事件

        private void DataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var msgInfo = dgrdMsgList.SelectedItem as MessageInfo;
            if (msgInfo == null) return;
            txtBoxMsg.Text = msgInfo.Data;
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dgrdMsgList.SelectedItems == null) return;
            foreach (MessageInfo item in dgrdMsgList.SelectedItems)
            {
                m_MsgInfo.Remove(item);
            }

            dgrdMsgList.ItemsSource = null;
            dgrdMsgList.ItemsSource = m_MsgInfo;
        }

        private void btnReply_Click(object sender, EventArgs e)
        {
            if (dgrdMsgList.SelectedItem == null || dgrdMsgList.SelectedItems == null) return;
            var item = dgrdMsgList.SelectedItem as MessageInfo;
            SendMsgWindow msgWindow = new SendMsgWindow(item.WorkNo);
            msgWindow.ShowDialog();
            if (msgWindow.IsConfirm)
            {
                string res = BusinessAdapter.GetBusinessInstance().SendNote(Int32.Parse(msgWindow.WorkNo), msgWindow.TxtContent);
                Log4NetHelper.ActionLog("Common", "Vc_SendNote", res);
                if (!res.Equals(AGWErrorCode.OK))
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("msgview_fail"));
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
