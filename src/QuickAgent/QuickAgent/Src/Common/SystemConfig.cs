namespace QuickAgent.Src.Common
{
    public class SystemConfig
    {
        public string Language { get; set; }
        private static SystemConfig languageChoose;
        //单例模式
        public static SystemConfig SystemConfigInstance()
        {
            if (languageChoose == null)
            {
                languageChoose = FileInfos.Instance().SystemConfigCreat(true);//读取language配置文件
            }
            return languageChoose;
        }
        //重新载入配置文件
        public static SystemConfig ReloadSystemConfig()
        {
            languageChoose = FileInfos.Instance().SystemConfigCreat(true);//读取language配置文件
            return languageChoose;
        }
    }
}
