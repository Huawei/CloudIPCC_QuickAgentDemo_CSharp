using System.Windows;
using System.Windows.Threading;
using System;
using QuickAgent.Src.Common;
using QuickAgent.View;
using QuickAgent.Common;
namespace QuickAgent.View
{
    /// <summary>
    /// 消息窗口，模态提示用户
    /// </summary>
    public sealed partial class MessageWindow : Window
    {
        DispatcherTimer myDispatcherTimer;
        public MessageWindow(string message, double seconds)
        {
            InitializeComponent();
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            txtMessage.Text = message;
            myDispatcherTimer = new DispatcherTimer();
            myDispatcherTimer.Interval = TimeSpan.FromSeconds(seconds);//几秒后提示框自动关闭
            myDispatcherTimer.Tick += new EventHandler(myDispatcherTimer_Tick);
            myDispatcherTimer.Start();
            MainWindow.Instance().IsOpenMessge = true;
            this.Topmost = true;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }
        //设置窗体5秒后自动关闭
        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            myDispatcherTimer.Tick -= myDispatcherTimer_Tick;
            this.Close();
            MainWindow.Instance().IsOpenMessge = false;
        }
        //窗体加载事件
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //刷新窗体语言内容
            LanguageResource.ChangeWindowLanguage(this);
        }

        //确定
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow.Instance().IsOpenMessge = false;
            myDispatcherTimer.Tick -= myDispatcherTimer_Tick;
        }

        //取消
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow.Instance().IsOpenMessge = false;
            myDispatcherTimer.Tick -= myDispatcherTimer_Tick;
        }
    }
}
