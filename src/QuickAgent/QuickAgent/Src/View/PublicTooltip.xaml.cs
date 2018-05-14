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
using QuickAgent.View;
using QuickAgent.ViewModel;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// PublicTooltip.xaml 的交互逻辑
    /// </summary>
    public partial class PublicTooltip : Window
    {
        private static PublicTooltip _instance = null;
        public static PublicTooltip GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PublicTooltip();
            }
            return _instance;
        }
        public PublicTooltip()
        {
            this.Loaded += new RoutedEventHandler(PublicTooltip_Load);
            InitializeComponent();         
            this.Closed += new EventHandler(Window_Close);
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            this.Topmost = true;
        }

        public void PublicTooltip_Load(object sender, RoutedEventArgs e)
        {
            //刷新窗体语言内容
            LanguageResource.ChangeWindowLanguage(this);

            //this.Title = MainWindow.Instance().TryFindResource("publicTooltip") != null ? MainWindow.Instance().TryFindResource("publicTooltip").ToString() : "";
            //this.Btn_OK.Content = MainWindow.Instance().TryFindResource("ok") != null ? MainWindow.Instance().TryFindResource("ok") : "";
            //this.Btn_Cancel.Content = MainWindow.Instance().TryFindResource("cancel") != null ? MainWindow.Instance().TryFindResource("cancel") : "";
        }

        public void LoadMessage(string key)
        {
            lbl_Message.Content = LanguageResource.FindResourceMessageByKey(key);
        }

        public void OK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public void Window_Close(object sender, EventArgs e)
        {
            _instance = null;
        }
    }
}
