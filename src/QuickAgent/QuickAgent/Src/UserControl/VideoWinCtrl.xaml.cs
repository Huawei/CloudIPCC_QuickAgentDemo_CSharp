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

namespace QuickAgent.Src.UserControl
{
    /// <summary>
    /// Video Window
    /// </summary>
    public partial class VideoWinCtrl : Window
    {
        public bool IsForceClose { set; get; }

        public VideoWinCtrl()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        public VideoWinCtrl(double left, double top, double width, double height, string windowTitle)
        {
            InitializeComponent();
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.Title = windowTitle;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsForceClose)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        public void SetVisibility(Visibility visi)
        {
            this.Visibility = visi;
        }

        public void CheckVisibilityAndShow()
        {
            
        }
    }
}
