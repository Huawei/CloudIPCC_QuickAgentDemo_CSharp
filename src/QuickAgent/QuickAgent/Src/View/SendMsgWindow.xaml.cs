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
    /// 发送便签 窗口
    /// </summary>
    public partial class SendMsgWindow : Window
    {
        #region  属性

        /// <summary>
        /// 目标工号
        /// </summary>
        public string WorkNo { private set; get; }

        /// <summary>
        /// 发送的内容
        /// </summary>
        public string TxtContent { private set; get; }

        /// <summary>
        /// 是否点击了确认按钮
        /// </summary>
        public bool IsConfirm { private set; get; }

        #endregion

        #region  构造函数

        public SendMsgWindow()
        {
            InitializeComponent();
            WindowInitial();   
        }

        public SendMsgWindow(string workNo)
        {
            InitializeComponent();
            WindowInitial();
            txtBoxWorkNo.SetText(workNo);
        }

        #endregion

        #region  控件事件

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            WorkNo = txtBoxWorkNo.Text;
            TxtContent = txtBoxContent.Text;
            IsConfirm = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            IsConfirm = false;
            Close();
        }

        #endregion

        #region  其他方法

        /// <summary>
        /// 界面初始化
        /// </summary>
        private void WindowInitial()
        {
            LanguageResource.ChangeWindowLanguage(this);
            ctrlHead.SetText(LanguageResource.FindResourceMessageByKey("sendnote_head"));
        }

        #endregion
    }
}
