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
    /// Interaction logic for LanguageSelect.xaml
    /// </summary>
    public partial class LanguageSelect : Window
    {
        public bool IsConfirm { set; get; }

        /// <summary>
        /// language culture
        /// </summary>
        public string Culture { set; get; }

        public LanguageSelect()
        {
            InitializeComponent();
        }

        private void Btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            IsConfirm = true;
            Close();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Rd_chiness_Checked(object sender, RoutedEventArgs e)
        {
            Culture = "ZH-CN";
            LanguageResource.ReloadLanguageResource(Culture);
        }

        private void Rd_english_Checked(object sender, RoutedEventArgs e)
        {
            Culture = "EN-US";
            LanguageResource.ReloadLanguageResource(Culture);
        }
    }
}