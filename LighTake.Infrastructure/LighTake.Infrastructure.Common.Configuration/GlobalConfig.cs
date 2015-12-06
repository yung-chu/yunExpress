using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace LighTake.Infrastructure.Common.Configuration
{
    /// <summary>
    /// 全局通用配置信息
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年12月13日
    /// 修改历史 : 无
    /// </remarks>
    public partial class GlobalConfig
    {
        /// <summary>
        /// 主站点Url(总是以/结尾)
        /// </summary>
        public static string SiteUrl
        {
            get
            {
                string setting = WebConfigurationManager.AppSettings["Site_Url"];

                if (!setting.EndsWith("/"))
                    setting += "/";

                return setting;
            }
        }

        /// <summary>
        /// 主站点域名
        /// </summary>
        public static string SiteDomain
        {
            get
            {
                string setting = WebConfigurationManager.AppSettings["Site_Domain"];

                return setting;
            }
        }

        /// <summary>
        /// 主站点安全加密传输Url(总是以/结尾)
        /// </summary>
        public static string SSLSiteUrl
        {
            get
            {
                string setting = WebConfigurationManager.AppSettings["SSL_Site_Url"];

                if (!setting.EndsWith("/"))
                    setting += "/";

                return setting;
            }
        }
        /// <summary>
        /// 网站权限码
        /// </summary>
        public static string WebSitePermissionCode
        {
            get
            {
                string setting = WebConfigurationManager.AppSettings["WebSite_PermissionCode"];
                return setting;
            }
        }

        public static string WebImagePath
        {
            get { return ConfigurationManager.AppSettings["WebImagePath"]; }
        }

        public static string ImagePath
        {
            get { return ConfigurationManager.AppSettings["ImagePath"]; }
        }

        public static string WebImagePath_Source
        {
            get { return string.Format("{0}{1}", WebImagePath, "Source/"); }
        }

        public static string ImagePath_Source
        {
            get { return string.Format("{0}{1}", ImagePath, "Source\\"); }
        }

        public static string WebImagePath_Original
        {
            get { return string.Format("{0}{1}", WebImagePath, "Original/"); }
        }
        public static string WebImagePath_Watermarked
        {
            get { return string.Format("{0}{1}", WebImagePath, "Watermarked/"); }
        }

        public static string ImagePath_Original
        {
            get { return string.Format("{0}{1}", ImagePath, "Original\\"); }
        }

        public static string WebImagePath_Small
        {
            get { return string.Format("{0}{1}", WebImagePath, "60x60/"); }
        }

        public static string WebImagePath_Middle
        {
            get { return string.Format("{0}{1}", WebImagePath, "140x140/"); }
        }

        public static string WebImagePath_Large
        {
            get { return string.Format("{0}{1}", WebImagePath, "600x600/"); }
        }
    }
}
