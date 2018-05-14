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
    /// 输入框(只能输入数字)
    /// </summary>
    public partial class InputTextBoxCtrl : System.Windows.Controls.UserControl
    {
        #region  属性

        /// <summary>
        /// 控件内容（只读）
        /// </summary>
        public string Text { get { return txtNumber.Text; } }

        /// <summary>
        /// 是否限制长度（默认不限制，功能未实现）
        /// </summary>
        private bool _isLimit = false;

        /// <summary>
        /// 要限制的长度（默认是 8 位，功能未实现）
        /// </summary>
        private int _length = 8;

        #endregion

        #region  方法

        public InputTextBoxCtrl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="txt"></param>
        public void SetText(string txt)
        {
            txtNumber.Text = txt;
        }

        /// <summary>
        /// 设置是否只读
        /// </summary>
        /// <param name="isReadOnly"></param>
        public void SetReadOnly(bool isReadOnly)
        {
            txtNumber.IsReadOnly = isReadOnly;
        }

        #endregion

        #region  控件事件

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;

            if (_isLimit)
            {
 
            }
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsNumberic(e.Text);
        }

        public static bool IsNumberic(string str)
        {
            return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
        }

        #endregion
    }
}
