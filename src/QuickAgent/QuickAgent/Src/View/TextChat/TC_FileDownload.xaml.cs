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
    /// 文字交谈：下载界面
    /// </summary>
    public partial class TC_FileDownload : Window
    {
        public string FilePath { set; get; }

        public string FileName { set; get; }

        public bool IsConfirm { set; get; }

        public TC_FileDownload()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }

        private void btn_pathselect_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            
            dlg.ShowDialog();
            tb_filesavepath.Text = dlg.FileName;
        }

        private void btn_download_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_downloadfilepath.Text) || string.IsNullOrEmpty(tb_filesavepath.Text))
            {
                MessageBox.Show("Please input download file path and save file path.");
                return;
            }
            FilePath = tb_downloadfilepath.Text;
            FileName = tb_filesavepath.Text;
            IsConfirm = true;
            Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirm = false;
            FilePath = string.Empty;
            FileName = string.Empty;
            this.Close();
        }
    }
}
