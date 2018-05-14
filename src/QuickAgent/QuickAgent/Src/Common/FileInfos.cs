using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickAgent.Common;
using System.IO;

namespace QuickAgent.Src.Common
{
    public class FileInfos
    {
        static readonly string pathLan = AppDomain.CurrentDomain.BaseDirectory + "Config";

        public static readonly string PathConfig = AppDomain.CurrentDomain.BaseDirectory + "Config\\SystemConfig.ini";

        bool isFind = false;

        private static FileInfos fileInfos;

        private FileInfos()
        {
        }

        public static FileInfos Instance()
        {
            if (null == fileInfos)
            {
                fileInfos = new FileInfos();
            }
            return fileInfos;
        }

        public SystemConfig SystemConfigCreat(bool isRead)
        {
            SystemConfig lanCh = InitializeLanguage();
            DirectoryInfo di = new DirectoryInfo(pathLan);
            if (!di.Exists)
            {
                di.Create();//创建文件夹
                CreateSystemConfigFile("SystemConfig", lanCh);//创建文件
            }
            else
            {
                if (isRead)//如果isRead为true，获取文件内容
                {
                    //获取该文件夹下的子文件和在该文件夹下创建新文件
                    FileInfo fi = new FileInfo(PathConfig);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        if (file.Name == fi.Name)
                        {
                            lanCh = GetLanValue(file);
                            isFind = true;
                        }
                    }
                    if (!isFind)
                    {
                        CreateSystemConfigFile("SystemConfig", lanCh);
                    }
                }
            }
            return lanCh;
        }

        public static void SetLanguageAndWriteIni(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return;
            }
            if (!"ZH-CN".Equals(language.ToUpper()) && !"EN-US".Equals(language.ToUpper()))
            {
                return;
            }

            SystemConfig lanCh = InitializeLanguage(language);
            DirectoryInfo di = new DirectoryInfo(pathLan);
            if (!di.Exists)
            {
                di.Create();//创建文件夹
                CreateSystemConfigFile("SystemConfig", lanCh);//创建文件
            }
        }

        /// <summary>
        /// create language config file
        /// </summary>
        /// <param name="fileName">filename</param>
        /// <param name="lau">language</param>
        public static void CreateSystemConfigFile(string fileName, SystemConfig lau)
        {
            if (string.IsNullOrEmpty(fileName) || null == lau)
            {
                return;
            }

            if (string.IsNullOrEmpty(lau.Language))
            {
                lau.Language = "ZH-CN";
            }
            SafeNativeMethods.WritePrivateProfileString("SystemConfig", "Language", lau.Language, pathLan + @"\" + fileName + ".ini");
        }

        private static SystemConfig GetLanValue(FileInfo fileinfo)
        {
            if (null == fileinfo)
            {
                return null;
            }

            var dicItem = new Dictionary<string, string>();
            string values = OpenReadFile(fileinfo);
            SystemConfig lanCh = new SystemConfig();
            string[] encods = values.Replace("\r\n", "*").Split('*');
            string[] strValues;
            for (int i = 0; i < encods.Length; i++)
            {
                strValues = encods[i].Split('=');
                if (strValues.Length > 1)
                {
                    dicItem.Add(strValues[0], strValues[1]);
                }
            }
            Boolean isFindKey;
            if (dicItem.Count > 0)
            {
                isFindKey = dicItem.ContainsKey("Language");
                if (isFindKey)
                {
                    lanCh.Language = dicItem["Language"];
                }
                else
                {
                    lanCh.Language = "ZH-CN";
                }
            }
            return lanCh;
        }

        public static SystemConfig InitializeLanguage()
        {
            SystemConfig languageChoose = new SystemConfig();
            languageChoose.Language = "ZH-CN";
            return languageChoose;
        }

        public static SystemConfig InitializeLanguage(string language)
        {
            SystemConfig languageChoose = new SystemConfig();
            languageChoose.Language = language;
            return languageChoose;
        }

        public static string OpenReadFile(FileInfo fileinfo)
        {
            if (null == fileinfo)
            {
                return string.Empty;
            }

            string values = string.Empty;
            FileStream fsr = fileinfo.OpenRead();
            byte[] datar = new byte[(int)fsr.Length];
            int bRead = 0;
            while (bRead < (int)fsr.Length)
            {
                int resultRead = fsr.Read(datar, bRead, (int)fsr.Length - bRead);
                if (resultRead == -1)
                {
                    Log4NetHelper.BaseLog("read error");
                }
                bRead += resultRead;
            }
            values = System.Text.Encoding.Default.GetString(datar);
            fsr.Close();
            return values;
        }
    }
}
