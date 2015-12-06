using System.Configuration;

namespace LMS.UserCenter.Core
{
    /// <summary>
    /// 整个站点的Cookie
    /// </summary>
    public static class Cookies
    {
        private static string _cookieKey = string.Empty;

        private static string CookieKey
        {
            get
            {
                if (string.IsNullOrEmpty(_cookieKey))
                {
                    try
                    {
                        _cookieKey = ConfigurationManager.AppSettings["CookieKey"];
                        if (string.IsNullOrEmpty(_cookieKey))
                        {
                            _cookieKey = "Drop";
                        }
                    }
                    catch
                    {
                        _cookieKey = "Drop";
                    }

                }
                return _cookieKey;
            }
        }

        /// <summary>
        /// 错误登陆次数.
        /// </summary>
        public static string LoginErrorCount = string.Format("C_{0}_LoginErrorCount", CookieKey);

    }

    /// <summary>
    /// Webconfig配置
    /// </summary>
    public class Configs : ConfigurationSection
    {
        /// <summary>
        /// 主站点域名
        /// </summary>
        public static string SiteDomain = ConfigurationManager.AppSettings["Site_Domain"];

        /// <summary>
        ///  包含iframe主站点域名
        /// </summary>
        public static string MainDomain = ConfigurationManager.AppSettings["Main_Domain"];

        /// <summary>
        /// 图片服务器路径
        /// </summary>
        public static string WebImagePath = ConfigurationManager.AppSettings["WebImagePath"];

        /// <summary>
        /// 上传文件路径
        /// </summary>
        public static string UploadPath = ConfigurationManager.AppSettings["Upload_Path"];

        /// <summary>
        /// 上传文件的Http路径
        /// </summary>
        public static string UploadWebPath = ConfigurationManager.AppSettings["Upload_Web_Path"];

        /// <summary>
        /// Excel模板路径
        /// </summary>
        public static string ExcelTemplatePath = UploadPath + @"ExcelTemplate\";

        /// <summary>
        /// Excel模板Http路径
        /// </summary>
        public static string ExcelTemplateWebPath = UploadWebPath + "ExcelTemplate/";

        /// <summary>
        /// 临时文件目录
        /// </summary>
        public static string TemporaryPath = UploadPath + "TemporaryFolder/";

        public static string DefaultDateFormat = "yyyy-MM-dd";

        public static string DefaultDatetimeFormat = "yyyy-MM-dd HH:mm:ss";


    }

    /// <summary>
    /// 图片设置
    /// </summary>
    public static class ImageSettings
    {

        public static string GetImageSmall(this string strImagePath)
        {
            return string.Format("{0}{1}", Configs.WebImagePath, "60x60" + strImagePath);
        }

        public static string GetImageMiddle(this string strImagePath)
        {
            return string.Format("{0}{1}", Configs.WebImagePath, "600x600" + strImagePath);
        }

        public static string GetImageLarge(this string strImagePath)
        {
            return string.Format("{0}{1}", Configs.WebImagePath, "400x400" + strImagePath);
        }

        public static string GetImageLogo(this string strImagePath)
        {
            return string.Format("{0}{1}", Configs.WebImagePath, "MeetGift" + strImagePath);
        }


    }
}