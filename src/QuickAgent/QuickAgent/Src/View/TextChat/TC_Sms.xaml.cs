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

namespace QuickAgent.Src.View.TextChat
{
    /// <summary>
    /// Sms window
    /// </summary>
    public partial class TC_Sms : Window
    {
        public bool IsConfirm { set; get; }

        /// <summary>
        /// skill id (sms skill)
        /// </summary>
        public string SkillID { set; get; }

        /// <summary>
        /// sms chat id()
        /// </summary>
        public string ChatID { set; get; }

        /// <summary>
        /// sms receiver
        /// </summary>
        public string Receiver { set; get; }

        /// <summary>
        /// sms ip
        /// </summary>
        public string JauIP { set; get; }

        /// <summary>
        /// sms port
        /// </summary>
        public string JauPort { set; get; }

        /// <summary>
        /// sms content
        /// </summary>
        public string SmsContent { set; get; }

        /// <summary>
        /// window initail method
        /// </summary>
        public TC_Sms()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            var business = BusinessAdapter.GetBusinessInstance() as AgentGatewayBusiness;
            if (null == business)
            {
                return; ;
            }
            var lstSkills = business.GetAgentSkills();
            UIViewHelper.SelectorResetSource(cb_skills, lstSkills);
            if (null == lstSkills || 0 == lstSkills.Count)
            {
                return;
            }
            cb_skills.SelectedIndex = 0;
        }

        /// <summary>
        /// send button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_ok_Click(object sender, EventArgs e)
        {
            var item = cb_skills.SelectedItem as SkillInfo;
            if (null == item || string.IsNullOrEmpty(tb_chatID.Text) || string.IsNullOrEmpty(tb_receiver.Text)
                || string.IsNullOrEmpty(tb_jauIP.Text) || string.IsNullOrEmpty(tb_jauPort.Text))
            {
                return;
            }
            this.SkillID = item.id;
            this.ChatID = tb_chatID.Text;
            this.Receiver = tb_receiver.Text;
            this.SmsContent = tb_msg.Text;
            this.JauIP = tb_jauIP.Text;
            this.JauPort = tb_jauPort.Text;
            IsConfirm = true;
            this.Close();
        }

        /// <summary>
        /// cancel button click
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click event</param>
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            IsConfirm = false;
            this.Close();
        }
    }
}
