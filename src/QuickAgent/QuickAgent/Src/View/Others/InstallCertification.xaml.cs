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

namespace QuickAgent.Src.View.Others
{
    /// <summary>
    /// Interaction logic for InstallCertification.xaml
    /// </summary>
    public partial class InstallCertification : Window
    {
        public bool IsConfirm { get; set; }

        public string Ip { get; set; }

        public string Port { get; set; }

        public InstallCertification()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            IsConfirm = true;
            Ip = Tb_Ip.Text;
            Port = Tb_Port.Text;
            this.Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
