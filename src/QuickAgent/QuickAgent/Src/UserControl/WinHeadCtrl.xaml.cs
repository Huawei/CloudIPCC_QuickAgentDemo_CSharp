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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuickAgent.Src.UserControl
{
    /// <summary>
    /// 每个界面的头，包含华为图标与一段文字
    /// </summary>
    public partial class WinHeadCtrl : System.Windows.Controls.UserControl
    {
        public WinHeadCtrl()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            txtBlock.Text = text;
        }
    }
}
