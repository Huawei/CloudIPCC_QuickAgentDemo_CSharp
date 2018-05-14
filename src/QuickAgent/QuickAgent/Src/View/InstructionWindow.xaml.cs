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
    /// 座席条说明界面
    /// </summary>
    public partial class InstructionWindow : Window
    {
        public InstructionWindow()
        {
            InitializeComponent();
            LanguageResource.ChangeWindowLanguage(this);
            tbNotify.Text = LanguageResource.FindResourceMessageByKey("instruction_cti");
            btnClose.Content = LanguageResource.FindResourceMessageByKey("cancel");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
