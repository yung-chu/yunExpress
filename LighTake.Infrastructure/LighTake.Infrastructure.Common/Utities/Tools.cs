using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 常用工具类
    /// </summary>
    public static class Tools
    {
        #region 产生随机字符串

        /// <summary>
        /// 产生随机字符串
        /// </summary>
        /// <param name="length">随机字符串的长度</param>
        /// <param name="strs">从指定的字符产生</param>
        /// <returns>随机字符串</returns>
        private static string MakeRandomString(int length, string strs)
        {
            string randomString = string.Empty;

            var rd = new Random(Convert.ToInt32(DateTime.Now.ToString("HHmmss")));

            for (int i = 0; i < length; i++)
                randomString += strs[rd.Next(strs.Length)];

            return randomString;
        }

        /// <summary>
        /// 产生随机字符串，从"ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890"中随机产生
        /// </summary>
        /// <param name="length">随机字符串的长度</param>
        /// <returns>随机字符串</returns>
        public static string MakeRandomString(int length)
        {
            const string strs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            return MakeRandomString(length, strs);
        }

        /// <summary>
        /// 产生随机数字字符串，从"1234567890"中随机产生
        /// </summary>
        /// <param name="length">随机字符串的长度</param>
        /// <returns>随机数字字符串</returns>
        public static string MakeNumricRandomString(int length)
        {
            const string strs = "1234567890";

            return MakeRandomString(length, strs);
        }

        /// <summary>
        /// 随机产生临时文件名
        /// 规则：YYYYMMDDhhnnss + 6位随机串 + 扩展名
        /// </summary>
        /// <param name="extName">文件扩展名</param>
        public static string MakeRandomFileName(string extName)
        {
            DateTime dt = DateTime.Now;
            string YYYY = dt.Year.ToString();
            string MM = ((dt.Month < 10) ? ("0" + dt.Month.ToString()) : dt.Month.ToString());
            string DD = ((dt.Day < 10) ? ("0" + dt.Day.ToString()) : dt.Day.ToString());
            string hh = ((dt.Hour < 10) ? ("0" + dt.Hour.ToString()) : dt.Hour.ToString());
            string nn = ((dt.Minute < 10) ? ("0" + dt.Minute.ToString()) : dt.Minute.ToString());
            string ss = ((dt.Second < 10) ? ("0" + dt.Second.ToString()) : dt.Second.ToString());
            string ffff = dt.Millisecond.ToString();

            string rndStr = YYYY + MM + DD + hh + nn + ss + ffff + MakeRandomString(6) + extName;
            return rndStr;
        }

        /// <summary> 
        /// 返回随机数组 
        /// </summary> 
        /// <param name="minValue">最小值</param> 
        /// <param name="maxValue">最大值</param> 
        /// <param name="count">个数</param> 
        /// <returns></returns> 
        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            var rnd = new Random();
            int length = maxValue - minValue + 1;
            var keys = new byte[length];
            rnd.NextBytes(keys);
            var items = new int[length];
            for (int i = 0; i < length; i++)
            {
                items[i] = i + minValue;
            }
            Array.Sort(keys, items);
            var result = new int[count];
            Array.Copy(items, result, count);
            return result;
        }

        #endregion



        #region 读写Config
        /// <summary>
        /// 获取 web.config 文件中指定 key 的值
        /// </summary>
        /// <param name="keyName">key名称</param>
        /// <returns></returns>
        public static string GetAppSettings(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName];
        }

        #endregion

        public static DataTable ConvertToTable(IQueryable query)
        {
            var dtList = new DataTable();
            bool isAdd = false;
            PropertyInfo[] objProterties = null;
            foreach (var item in query)
            {
                if (!isAdd)
                {
                    objProterties = item.GetType().GetProperties();
                    foreach (var itemProterty in objProterties)
                    {
                        Type type;
                        if (itemProterty.PropertyType != typeof(string) && itemProterty.PropertyType != typeof(int) && itemProterty.PropertyType != typeof(DateTime))
                        {
                            type = typeof(string);
                        }
                        else
                        {
                            type = itemProterty.PropertyType;
                        }
                        dtList.Columns.Add(itemProterty.Name, type);
                    }
                    isAdd = true;
                }
                var row = dtList.NewRow();
                foreach (var pi in objProterties)
                {
                    row[pi.Name] = pi.GetValue(item, null);
                }
                dtList.Rows.Add(row);
            }

            return dtList;
        }

        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {

            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|^Guest");
        }


        /// <summary>
        /// 将TexBoxt的文本转换为Html格式表在
        /// </summary>
        /// <param name="text">来自TextBox的文本</param>
        public static string TextToHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }

            text = text.Replace("\r\n", "<br />");
            //  Text = Text.Replace("\r", "<br />");
            text = text.Replace("\n", "<br />");
            text = text.Replace("  ", "&nbsp;&nbsp;");
            //Text = Text.Replace("'", "''"); Kevin.Mo  Modify 2010.08.24

            return text.Trim();
        }

        /// <summary>
        /// 将Html格式转换到TextBox的文本显示
        /// </summary>
        /// <param name="html">来自HTML编码</param>
        public static string HtmlToText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return "";

            html = html.Replace("<br />", "\r\n");
            html = html.Replace("<br>", "\r\n");
            html = html.Replace("&nbsp;&nbsp;", "  ");

            return html.Trim();
        }
        /// <summary>
        /// 过滤所有的html代码,只提取文字部分
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string FilterAllHtml(string html)
        {
            Regex regex = new Regex(@"</?.*?>", RegexOptions.IgnoreCase);
            html = regex.Replace(html, "");
            html = html.Replace(" ", " ");
            return html;
        }

        /// <summary>
        /// 比较两个数组的内容是否相同，如果相同返回true,反之false
        /// </summary>
        /// <param name="arr1">数组1</param>
        /// <param name="arr2">数组2</param>
        /// <returns></returns>
        public static bool CompareArrContent<T>(T[] arr1, T[] arr2) where T:struct 
        {
            var q = from a in arr1 join b in arr2 on a equals b select a;
            bool flag = arr1.Length == arr2.Length && q.Count() == arr1.Length;
            return flag;
        }

        public static bool CheckPostCode(string val)
        {
            string s = @"^((1|2|3|4|6)\d{5})$";
            Regex reg = new Regex(s);
            return reg.IsMatch(val.Trim());
        }

        public static bool CheckShippingPhone(string val)
        {
            string s = @"^\d{1,11}$";
            Regex reg = new Regex(s);
            return reg.IsMatch(val.Trim());
        }


        /// <summary>
        /// base64编码的文本转为图片
        /// </summary>
        /// <param name="fileName">保存的路径加文件名</param>
        /// <param name="strBase64">要转换的文本</param>
        /// <param name="fileExtension">文件扩展名 如:(".jpg",".bmp",".png")</param>
        public static void Base64StringToImage(string fileName, string strBase64, string fileExtension)
        {
            try
            {
                byte[] arr = Convert.FromBase64String(strBase64);
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                switch (fileExtension.ToLowerInvariant())
                {
                    case ".jpg":
                    case ".jpeg":
                        bmp.Save(fileName + fileExtension,ImageFormat.Jpeg);
                        break;
                    case ".bmp":
                        bmp.Save(fileName + ".bmp", ImageFormat.Bmp);
                        break;
                    case ".gif":
                        bmp.Save(fileName + ".gif", ImageFormat.Gif);
                        break;
                    case ".png":
                        bmp.Save(fileName + ".png", ImageFormat.Png);
                        break;
                }
                ms.Close();

            }
            catch (Exception ex)
            {
                throw new BusinessLogicException("base64编码的文本转为图片出错!");
            }
        }


        /// <summary>
        /// base64编码的文本转为图片
        /// </summary>
        /// <param name="fileNamePath">保存的路径加文件名</param>
        /// <param name="fileName"></param>
        /// <param name="base64Bytes">byte[]</param>
        /// <param name="fileExtension">文件扩展名 如:(".jpg",".bmp",".png")</param>
        public static string Base64StringToImage(string fileNamePath,string fileName, byte[] base64Bytes, string fileExtension)
        {
            string result = string.Empty;
            try
            {
                byte[] arr = base64Bytes;
                MemoryStream ms = new MemoryStream(arr);
                Bitmap bmp = new Bitmap(ms);
                switch (fileExtension.ToLowerInvariant())
                {
                    case ".jpg":
                    case ".jpeg":
                        bmp.Save(fileNamePath+fileName + fileExtension, ImageFormat.Jpeg);
                        break;
                    case ".bmp":
                        bmp.Save(fileNamePath + fileName + fileExtension, ImageFormat.Bmp);
                        break;
                    case ".gif":
                        bmp.Save(fileNamePath + fileName + fileExtension, ImageFormat.Gif);
                        break;
                    case ".png":
                        bmp.Save(fileNamePath + fileName + fileExtension, ImageFormat.Png);
                        break;
                }
                ms.Close();
                result=fileName + fileExtension;
            }
            catch (Exception ex)
            {
                throw new BusinessLogicException("base64编码的文本转为图片出错!");
            }
            return result;
        }
    }
}
