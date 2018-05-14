using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using QuickAgent.View;
using QuickAgent.Common;

namespace QuickAgent.Src.Common
{
    public static class ShowMessage
    {
        static string strMsg;

        private static MessageWindow recordFailMsgWindow = null;

        public static MessageWindow RecordFailMsgWindow
        {
            get { return recordFailMsgWindow; }
            set { recordFailMsgWindow = value; }
        }

        //全局通用提示消息框
        public static void ShowMessageBox(string key)
        {
            strMsg = FindMsgByKey(key);
            MessageWindow mageWindow = new MessageWindow(strMsg, 3);
            SetMsgWindowDisplay(mageWindow);
            mageWindow.Show();
        }

        private static void SetMsgWindowDisplay(MessageWindow messageWindow)
        {
            messageWindow.Owner = MainWindow.Instance();
            if (MainWindow.Instance().Top < 0)
            {
                messageWindow.Top = 0;
            }
            else
            {
                messageWindow.Top = MainWindow.Instance().Top;
            }
            if (MainWindow.Instance().Left < 0)
            {
                messageWindow.Left = 0;
            }
            else
            {
                messageWindow.Left = MainWindow.Instance().Left + 35;
            }
        }

        public static string FindMsgByKey(string key)
        {
            return LanguageResource.FindResourceMessageByKey(key);
        }
    }
}
