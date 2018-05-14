using System;
using System.Runtime.InteropServices;

namespace QuickAgent.Src.Common
{
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal extern static int WritePrivateProfileString(string segName, string keyName, string sValue, string fileName);
    }
}
