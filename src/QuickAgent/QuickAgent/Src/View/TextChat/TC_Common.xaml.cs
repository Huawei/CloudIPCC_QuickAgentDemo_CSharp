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

namespace QuickAgent.Src.View.TextChat
{
    /// <summary>
    /// 文字交谈---内部呼叫
    /// </summary>
    public partial class TC_Common : Window
    {
        public string WorkNo { set; get; }

        public bool IsConfirm { set; get; }

        public TC_Common()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }

        private void btn_ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_workNo.Text))
            {
                MessageBox.Show("Please input workNo.");
                return;
            }
            IsConfirm = true;
            WorkNo = tb_workNo.Text;
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirm = false;
            this.Close();
        }
    }
}
