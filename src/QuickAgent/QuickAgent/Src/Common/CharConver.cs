using System;
using System.Windows.Controls.Primitives;
using System.Collections;
using QuickAgent.Common;

namespace QuickAgent.Src.Common
{
    static class CharConver
    {
        const char CHARACTER_A_CAP = 'A';
        const char CHARACTER_B_CAP = 'B';
        const char CHARACTER_C_CAP = 'C';
        const char CHARACTER_D_CAP = 'D';
        const char CHARACTER_E_CAP = 'E';
        const char CHARACTER_F_CAP = 'F';
        const char CHARACTER_G_CAP = 'G';
        const char CHARACTER_H_CAP = 'H';
        const char CHARACTER_I_CAP = 'I';
        const char CHARACTER_J_CAP = 'J';
        const char CHARACTER_K_CAP = 'K';
        const char CHARACTER_L_CAP = 'L';
        const char CHARACTER_M_CAP = 'M';
        const char CHARACTER_N_CAP = 'N';
        const char CHARACTER_O_CAP = 'O';
        const char CHARACTER_P_CAP = 'P';
        const char CHARACTER_Q_CAP = 'Q';
        const char CHARACTER_R_CAP = 'R';
        const char CHARACTER_S_CAP = 'S';
        const char CHARACTER_T_CAP = 'T';
        const char CHARACTER_U_CAP = 'U';
        const char CHARACTER_V_CAP = 'V';
        const char CHARACTER_W_CAP = 'W';
        const char CHARACTER_X_CAP = 'X';
        const char CHARACTER_Y_CAP = 'Y';
        const char CHARACTER_Z_CAP = 'Z';
        const char CHARACTER_XHY = '_';
        const char CHARACTER_ZHY = '-';

        public static String StrChange(String strInput)
        {
            String strReturn = null;
            //将字符串转换成字符数组
            char[] strArrayInput = strInput.ToCharArray();
            int length = strArrayInput.Length;
            for (int i = 0; i < length; i++)
            {
                char currLetter = strArrayInput[i];
                strReturn += GetConstChar(currLetter);
            }

            return strReturn;
        }

        public static char GetConstChar(char inputChar)
        {
            char outputChar = ' ';
            switch (inputChar)
            {
                case 'A':
                    outputChar = CHARACTER_A_CAP;
                    break;
                case 'B':
                    outputChar = CHARACTER_B_CAP;
                    break;
                case 'C':
                    outputChar = CHARACTER_C_CAP;
                    break;
                case 'D':
                    outputChar = CHARACTER_D_CAP;
                    break;
                case 'E':
                    outputChar = CHARACTER_E_CAP;
                    break;
                case 'F':
                    outputChar = CHARACTER_F_CAP;
                    break;
                case 'G':
                    outputChar = CHARACTER_G_CAP;
                    break;
                case 'H':
                    outputChar = CHARACTER_H_CAP;
                    break;
                case 'I':
                    outputChar = CHARACTER_I_CAP;
                    break;
                case 'J':
                    outputChar = CHARACTER_J_CAP;
                    break;
                case 'K':
                    outputChar = CHARACTER_K_CAP;
                    break;
                case 'L':
                    outputChar = CHARACTER_L_CAP;
                    break;
                case 'M':
                    outputChar = CHARACTER_M_CAP;
                    break;
                case 'N':
                    outputChar = CHARACTER_N_CAP;
                    break;
                case 'O':
                    outputChar = CHARACTER_O_CAP;
                    break;
                case 'P':
                    outputChar = CHARACTER_P_CAP;
                    break;
                case 'Q':
                    outputChar = CHARACTER_Q_CAP;
                    break;
                case 'R':
                    outputChar = CHARACTER_R_CAP;
                    break;
                case 'S':
                    outputChar = CHARACTER_S_CAP;
                    break;
                case 'T':
                    outputChar = CHARACTER_T_CAP;
                    break;
                case 'U':
                    outputChar = CHARACTER_U_CAP;
                    break;
                case 'V':
                    outputChar = CHARACTER_V_CAP;
                    break;
                case 'W':
                    outputChar = CHARACTER_W_CAP;
                    break;
                case 'X':
                    outputChar = CHARACTER_X_CAP;
                    break;
                case 'Y':
                    outputChar = CHARACTER_Y_CAP;
                    break;
                case 'Z':
                    outputChar = CHARACTER_Z_CAP;
                    break;
                case '-':
                    outputChar = CHARACTER_ZHY;
                    break;
            }

            return outputChar;
        }
    }

    /// <summary>
    /// helper class used for wpf ui component
    /// </summary>
    public static class UIViewHelper
    {
        /// <summary>
        /// component(implement Selector interface) reset source
        /// </summary>
        /// <param name="obj">object that needs reset source</param>
        /// <param name="src">new itemsource</param>
        public static void SelectorResetSource(Selector obj, IEnumerable src)
        {
            try
            {
                if (null == obj)
                {
                    return;
                }
                obj.ItemsSource = null;
                obj.Items.Clear();
                obj.ItemsSource = src;
            }
            catch (Exception exc)
            {
                Log4NetHelper.BaseLog("[selector_resetsrc_failed] obj:" + obj + " msg:" + exc.Message);
            }
        }
    }
}
