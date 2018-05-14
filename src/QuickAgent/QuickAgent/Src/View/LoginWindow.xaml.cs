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
using QuickAgent.Common;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 登陆窗口
    /// </summary>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// 座席电话
        /// </summary>
        public string PhoneNo { private set; get; }

        /// <summary>
        /// 座席密码
        /// </summary>
        public string Password { private set; get; }

        /// <summary>
        /// 座席工号
        /// </summary>
        public string WorkNo { private set; get; }

        /// <summary>
        /// 是否点击了确定按钮
        /// </summary>
        public bool IsConfirm { private set; get; }

        public LoginWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }

        public LoginWindow(AgentInfo info)
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            tbPhone.Text = info.PhoneNumber;
            tbWorkNo.SetText(info.AgentId);
        }

        public LoginWindow(AgentInfo info, bool useVoice, string phoneNum)
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            tbWorkNo.SetText(info.AgentId);
            tbPhone.IsEnabled = !useVoice;
            tbPhone.Text = useVoice ? phoneNum : info.PhoneNumber;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbPhone.Text) || string.IsNullOrEmpty(tbWorkNo.Text))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("login_input"));
                return;
            }

            PhoneNo = tbPhone.Text;
            WorkNo = tbWorkNo.Text;
            Password = pbPassword.Password;
            IsConfirm = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
