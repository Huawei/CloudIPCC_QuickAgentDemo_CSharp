using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace QuickAgent.Src.Common
{
    public class CommonHelper
    {
        //判断输入的字符是否为数字
        public static bool IsDigit(Key key)
        {
            int num = (int)key;
            bool isDigit = true;
            if (num < 2)
            {
                isDigit = false;
            }
            else if (num > 3 && num < 34)
            {
                isDigit = false;
            }
            else if (num > 43 && num < 74)
            {
                isDigit = false;
            }
            else if (num > 83)
            {
                isDigit = false;
            }  
            return isDigit;
        }

        public static string AgentStatus(string key)
        {
            string result = string.Empty;
            switch (key)
            {
                case "0":
                    result = LanguageResource.FindResourceMessageByKey("100");
                    break;
                case "1":
                    result = LanguageResource.FindResourceMessageByKey("101");
                    break;
                case "2":
                    result = LanguageResource.FindResourceMessageByKey("102");
                    break;
                case "3":
                    result = LanguageResource.FindResourceMessageByKey("busy");
                    break;
                case "4":
                    result = LanguageResource.FindResourceMessageByKey("free");
                    break;
                case "5":
                    result = LanguageResource.FindResourceMessageByKey("clearUp");
                    break;
                case "6":
                    result = LanguageResource.FindResourceMessageByKey("free");
                    break;
                case "7":
                    result = LanguageResource.FindResourceMessageByKey("107");
                    break;
                case "8":
                    result = LanguageResource.FindResourceMessageByKey("rest");
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
