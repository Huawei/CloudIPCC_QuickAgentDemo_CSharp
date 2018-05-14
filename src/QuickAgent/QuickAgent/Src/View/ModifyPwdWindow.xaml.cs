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

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 修改密码界面
    /// </summary>
    public partial class ModifyPwdWindow : Window
    {
        public string NewPwd { private set; get; }

        public string OldPwd { private set; get; }

        public ModifyPwdWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pbNewPwd.Password) || string.IsNullOrEmpty(pbOldPwd.Password))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("modifypwd_inputcheck"));
                return;
            }
            if (!pbNewPwd.Password.Equals(pbPwdConfirm.Password))
            {
                MessageBox.Show(LanguageResource.FindResourceMessageByKey("modifypwd_pwdnotsame"));
                return;
            }

            NewPwd = pbNewPwd.Password;
            OldPwd = pbOldPwd.Password;
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
