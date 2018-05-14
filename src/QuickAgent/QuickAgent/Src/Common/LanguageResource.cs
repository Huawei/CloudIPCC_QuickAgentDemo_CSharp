using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using QuickAgent.Constants;
using System.Windows;
using QuickAgent.View;
using QuickAgent.Common;
using System.Windows.Markup;

namespace QuickAgent.Src.Common
{
    public static class LanguageResource
    {
        /// <summary>
        /// current language
        /// </summary>
        public static string CurrentLanguage { set; get; }

        /// <summary>
        /// language resource dictionary
        /// </summary>
        private static ResourceDictionary _lanResDict { set; get; }

       /// <summary>
       /// initial language resource
       /// </summary>
        public static void InitLanguageResource()
        {
            var languageChoose = SystemConfig.SystemConfigInstance();
            if (null == languageChoose)
            {
                return;
            }
            _lanResDict = readLanguagePackage(languageChoose.Language);
        }

        /// <summary>
        /// change window language
        /// </summary>
        /// <param name="window">window object</param>
        public static void ChangeWindowLanguage(FrameworkElement window)
        {
            if (null == _lanResDict)
            {
                InitLanguageResource();
            }
            if (window.Resources.MergedDictionaries.Count > 0)
            {
                foreach (var languageDic in window.Resources.MergedDictionaries)
                {
                    if (null == languageDic.Source)
                    {
                        //如果上一次已经添加过该资源（languageDictionary），先移除。
                        window.Resources.MergedDictionaries.Remove(languageDic);
                        break;
                    }
                }
            }
            window.Resources.MergedDictionaries.Add(_lanResDict);
        }

        /// <summary>
        /// read language package
        /// </summary>
        /// <param name="language">language</param>
        /// <returns>language resource</returns>
        private static ResourceDictionary readLanguagePackage(string language)
        {
            ResourceDictionary reLanguageDictionary = null;
            try
            {
                reLanguageDictionary = (ResourceDictionary)Application.LoadComponent(new Uri("Src/Language/" + language.ToUpper() + ".xaml", UriKind.Relative));
                CurrentLanguage = language;
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Common", "ReadLanguagePackage", exc.Message);
                CurrentLanguage = "ZH-CN";
                reLanguageDictionary = (ResourceDictionary)Application.LoadComponent(new Uri("Src/Language/" + "ZH-CN.xaml", UriKind.Relative));
            }
            return reLanguageDictionary;
        }

        /// <summary>
        /// find src by key
        /// </summary>
        /// <param name="key">resource key</param>
        /// <returns>resource message</returns>
        public static string FindResourceMessageByKey(string key)
        {
            string msg = string.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                msg = MainWindow.Instance().TryFindResource(key) != null ? MainWindow.Instance().TryFindResource(key).ToString() : "";
            }
            return msg;
        }

        /// <summary>
        /// reload language resource
        /// </summary>
        /// <param name="language"></param>
        public static void ReloadLanguageResource(string language)
        {
            if (string.IsNullOrEmpty(language))
            {
                return;
            }
            if (!"EN-US".Equals(language.ToUpper()) && !"ZH-CN".Equals(language.ToUpper()))
            {
                return;
            }

            try
            {
                List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
                foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                {
                    dictionaryList.Add(dictionary);
                }
                ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(new Uri("Src/Language/" + language + ".xaml", UriKind.Relative));
                if (null == resourceDictionary || 0 == resourceDictionary.Count)
                {
                    Log4NetHelper.BaseLog("load resource dictionary failed.");
                }
                if (resourceDictionary != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }
            }
            catch (Exception exc)
            {
                Log4NetHelper.ExcepLog("Sys", "ReloadLanguageResource", exc.Message);
            }
        }
    }
}
