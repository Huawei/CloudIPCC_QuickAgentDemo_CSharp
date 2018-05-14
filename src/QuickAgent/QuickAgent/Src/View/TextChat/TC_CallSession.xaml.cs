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
    /// Qry Call Session Window
    /// </summary>
    public partial class TC_CallSession : Window
    {
        public TC_CallSession()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            var lstColumnHeader = new List<string> 
            { 
                LanguageResource.FindResourceMessageByKey("textchat_id"), 
                LanguageResource.FindResourceMessageByKey("textchat_callID"),
                LanguageResource.FindResourceMessageByKey("textchat_isInnerCall"),  
                LanguageResource.FindResourceMessageByKey("textchat_subMediaType"),
                LanguageResource.FindResourceMessageByKey("textchat_realcaller"), 
                LanguageResource.FindResourceMessageByKey("textchat_callerName"), 
                LanguageResource.FindResourceMessageByKey("textchat_starttime"), 
                LanguageResource.FindResourceMessageByKey("textchat_endtime"), 
                LanguageResource.FindResourceMessageByKey("textchat_releasecause"), 
            };
            if (null == dgrd_callsession || null == dgrd_callsession.Columns ||
                lstColumnHeader.Count != dgrd_callsession.Columns.Count)
            {
                return;
            }
            for (int i = 0; i < dgrd_callsession.Columns.Count; i++)
            {
                dgrd_callsession.Columns[i].Header = lstColumnHeader[i];
            }
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

            var instance = (AgentGatewayBusiness)BusinessAdapter.GetBusinessInstance();
            if (null == instance)
            {
                Log4NetHelper.BaseLog("Instance is null");
                return;
            }
            var qryParm = new CallSessionQryParm()
            {
                startTime = TransferDateToTimeStamp(tb_startTime.Text),
                endTime = TransferDateToTimeStamp(tb_endTime.Text),
                pageNo = tb_pageNo.Text,
                pageSize = tb_pageSize.Text,
                callType = tb_callType.Text,
                caller = tb_caller.Text,
                called = tb_called.Text,
                mediaType = "1",
                subMediaType = "2",
                realCaller = "",
                callerName = ""
            };
            var qryInfo = HuaweiAgentGateway.Utils.JsonUtil.ToJson(qryParm);

            try
            {
                var res = instance.QryCallSession(qryInfo);
                if (null == res)
                {
                    MessageBox.Show(LanguageResource.FindResourceMessageByKey("textchat_qryfailed"));
                    return;
                }
                Log4NetHelper.ActionLog("Agw", "QryCallSession", res.retcode);
                if (null == res.result)
                {
                    return;
                }
                lbl_totalPage.Content = res.result.totalPageNo;
                lbl_totalCount.Content = res.result.totalCount;
                UIViewHelper.SelectorResetSource(dgrd_callsession, res.result.weccCallBillList);
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
        }

        /// <summary>
        /// dgrd selection changed event
        /// </summary>
        /// <param name="sender">dgrd</param>
        /// <param name="e">selection changed event</param>
        private void DataGridSelectionChanged(Object sender, EventArgs e)
        {
            var item = dgrd_callsession.SelectedItem as CallSessionData;
            if (null == item)
            {
                return;
            }
            tb_msgshow.Text = HuaweiAgentGateway.Utils.JsonUtil.ToJson(item);
        }

        /// <summary>
        /// transfer time string to timestamp
        /// </summary>
        /// <param name="time">time string</param>
        /// <returns>timestamp</returns>
        private string TransferDateToTimeStamp(string time)
        {
            if (string.IsNullOrEmpty(time))
            {
                return time;
            }
            var timeStamp = time;

            try
            {
                var dt = Convert.ToDateTime(time);
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
                var intResult = (dt.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
                timeStamp = intResult + string.Empty;
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog(exc.Message);
            }
            return timeStamp;
        }
    }
}
