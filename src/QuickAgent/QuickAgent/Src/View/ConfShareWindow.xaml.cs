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
using System.Windows.Interop;

namespace QuickAgent.Src.View
{
    /// <summary>
    /// 共享窗口界面
    /// </summary>
    public partial class ConfShareWindow : Window
    {
        public ConfShareWindow()
        {
            InitializeComponent();
        }

        public ConfShareWindow(int height, int width)
        {
            this.Height = height;
            this.Width = width;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}