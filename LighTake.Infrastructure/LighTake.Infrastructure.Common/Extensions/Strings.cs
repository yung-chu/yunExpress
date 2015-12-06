using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

using System.ComponentModel;
using System.Web.Security;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 字符串操作常用扩展
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    public static class Strings
    {

        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {

                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute),
                false);

                if (attrs != null && attrs.Length > 0)

                    return ((DescriptionAttribute)attrs[0]).Description;

            }

            return en.ToString();
        }



        /// <summary>
        /// 检查该字符串是否为空
        /// </summary>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// 检查该字符串是否为空、null和空白字符
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        public static string FormatWith(this string source, params object[] args)
        {
            return String.Format(source, args);
        }




        /// <summary>
        /// 转换成对应的枚举项
        /// </summary>
        public static T ToEnum<T>(this string Value)
        {
            T oOut = default(T);
            Type t = typeof(T);
            foreach (FieldInfo fi in t.GetFields())
            {
                if (fi.Name.Matches(Value))
                    oOut = (T)fi.GetValue(null);
            }

            return oOut;
        }

        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="source">待转换字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToSBC(this string source)
        {
            //半角转全角：
            char[] c = source.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }


        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="input">待转换字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(this string source)
        {
            char[] c = source.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 将字符串超过指定长度的部分截断,中文按2位,英文1位
        /// </summary>
        /// <param name="source">待截断的字符串</param>
        /// <param name="length">长度</param>
        /// <returns>返回指定长度的部分</returns>
        public static string Cutstring(this string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return source;

            byte[] bytesOfSource = System.Text.Encoding.Default.GetBytes(source);

            if (length > bytesOfSource.Length)
                return source;

            Regex reg = new Regex("[^\x00-\xff]+", RegexOptions.Compiled);

            char[] charsOfSource = source.ToCharArray();

            StringBuilder resultBuilder = new StringBuilder();

            int index = 0;

            for (int i = 0; i < charsOfSource.Length; i++)
            {
                if (reg.IsMatch(charsOfSource[i].ToString()))
                {
                    if (length - index > 1)
                    {
                        resultBuilder.Append(charsOfSource[i]);
                    }

                    index += 2;
                }
                else
                {
                    resultBuilder.Append(charsOfSource[i]);

                    index = index + 1;
                }

                if (index >= length)
                    break;
            }

            return resultBuilder.ToString() + "...";
        }

        /// <summary>
        /// 清除UBB代码
        /// </summary>
        public static string RemoveUBBCode(this string content, int page, int preview)
        {
            string[] ubbKeys = new string[] { "center", "left", "right", "fleft", "fright", "fright" };
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"(\[" + ubbKey + @"\])", "");
                content = content.RegexReplace(@"(\[\/" + ubbKey + @"\])", "");
            }

            content = content.RegexReplace("(script)", "s cript");

            ubbKeys = new string[] { "DIR", "QT", "MP", "RM" };
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"\[" + ubbKey + @"=*([0-9]*),*([0-9]*)\](.[^\[]*)\[\/" + ubbKey + @"]", "");
            }

            content = content.RegexReplace(@"(\[UPLOAD=(.[^\[]*)\])(.[^\[]*)(\[\/UPLOAD\])", "");

            ubbKeys = new string[] { "IMG", "FLASH", "UPLOAD" };
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"(\[" + ubbKey + @"\])(.[^\[]*)(\[\/" + ubbKey + @"\])", "");
            }

            string[] patterns = new string[] { @"(\[URL\])(.[^\[]*)(\[\/URL\])", @"(\[URL=(.[^\[]*)\])(.[^\[]*)(\[\/URL\])" };
            foreach (string pattern in patterns)
            {
                content = content.RegexReplace(pattern, @"<A HREF=""$2\"" TARGET=_blank>$3</A>");
            }

            patterns = new string[] { @"(\[EMAIL\])(.[^\[]*)(\[\/EMAIL\])", @"(\[EMAIL=(.[^\[]*)\])(.[^\[]*)(\[\/EMAIL\])" };
            foreach (string pattern in patterns)
            {
                content = content.RegexReplace(pattern, @"<img align=absmiddle src=""/Images/Hqen/Order/tmail.gif""><A HREF=""mailto:$2"" TARGET=_blank>$3</A>");
            }

            ubbKeys = new string[] { "http", "ftp", "rtsp", "mms" };
            string patternStr = @"://[A-Za-z0-9\./=\?%\-&_~`@':+!]+)";
            string repStr = @"<a target=_blank href=$1>$1</a>";
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"^(" + ubbKey + patternStr, repStr);
                content = content.RegexReplace(@"(" + ubbKey + patternStr + "$", repStr);
                content = content.RegexReplace(@"[^>=""](" + ubbKey + patternStr, repStr);
            }

            ubbKeys = new string[] { "color", "face" };
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"(\[" + ubbKey + @"=(.[^\[]*)\])(.[^\[]*)(\[\/" + ubbKey + @"\])", "$3");
            }

            content = content.RegexReplace(@"(\[align=(.[^\[]*)\])(.*)(\[\/align\])", "$3");

            ubbKeys = new string[] { "QUOTE", "fly", "move" };
            foreach (string ubbKey in ubbKeys)
            {
                repStr = "$2";
                if (ubbKey == "QUOTE")
                {
                    repStr = @"<table cellpadding=0 cellspacing=0 border=0 WIDTH=94% bgcolor=#000000 align=center><tr><td><table width=100% cellpadding=5 cellspacing=1 border=0><TR><TD>$2</table></table><br>";
                }
                content = content.RegexReplace(@"(\[" + ubbKey + @"\])(.*)(\[\/" + ubbKey + @"\])", repStr);
            }

            ubbKeys = new string[] { "GLOW", "SHADOW" };
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"\[" + ubbKey + @"=*([0-9]*),*(#*[a-z0-9]*),*([0-9]*)\](.[^\[]*)\[\/" + ubbKey + @"]", "$4");
            }

            ubbKeys = new string[] { "i", "u", "b" };
            foreach (string ubbKey in ubbKeys)
            {
                content = content.RegexReplace(@"(\[" + ubbKey + @"\])(.[^\[]*)(\[\/" + ubbKey + @"\])", "$2");
            }

            for (int i = 1; i < 5; i++)
            {
                content = content.RegexReplace(@"(\[size=" + i.ToString() + @"\])(.[^\[]*)(\[\/size\])", "$2");
            }


            //分页处理
            if (preview == 1)
            {
                content = content.RegexReplace(@"\[page\]", "");
            }
            else
            {
                string[] splitStr = new string[] { @"\[page\]" };
                string[] contents = content.Split(splitStr, StringSplitOptions.RemoveEmptyEntries);
                if (page > contents.Length)
                {
                    page = contents.Length;
                }
                if (page < 0) page = 1;
                content = contents[page - 1];
            }
            return content;
        }

        /// <summary>
        /// 过滤HTML代码的函数包括过滤CSS和JS
        /// </summary>
        public static string RemoveHtmlTag(this string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;

            //匹配<>中的内容,替换为空字符
            Regex regex = new Regex("<.+?>");
            MatchCollection matchCollection = regex.Matches(content);
            foreach (Match match in matchCollection)
            {
                content = content.Replace(match.Value, "");
            }
            return content;
        }

        /// <summary>
        /// 按正则表达式进行匹配替换
        /// </summary>
        public static string RegexReplace(this string content, string pattern, string replaceStr)
        {
            Regex regex = new Regex(pattern);

            content = regex.Replace(content, replaceStr);

            return content;
        }

        /// <summary>
        /// 替换掉特殊字符
        /// </summary>
        public static string ReplaceSpecialCharacter(this string source)
        {
            string replacePattern = @"[\+\|\-|\&|\!|\(|\)|\{|\}|\[|\]|\^|""|\~|\*|\|\?|\:|\\]+";

            return Regex.Replace(source, replacePattern, new MatchEvaluator(m => m.Value));
        }

        /// <summary>
        /// 格式化成重写后的URL格式
        /// </summary>
        /// <param name="replaceStr">特殊字符替代字符</param>
        /// <returns>重写后的URL格式</returns>
        public static string FormatRewriteUrl(this string source, string replaceStr)
        {
            string[] aryReg = { "'", "<", ">", "%", "‰", "\"\"", ",", ".", ">=", "=<", "-", "_", ";", "||", "[", "]", "&", "/", "／", "-", "－", "|", "(", ")", "Ⅲ", "Ⅱ", "Ⅳ", "*", "×", "”", "~", "@", "#", "＃", "$", "^", "!", "\"", "+", "＋", "°", "℃", "㎡", ":", "；", "±", "≤", "≥", "＝", "≈", "↖", "↗", "↙", "↘", "←", "→", "￡" };
            for (int i = 0; i < aryReg.Length; i++)
            {
                source = source.Replace(aryReg[i], string.Empty);
            }
            return source.Replace(" ", replaceStr);
        }

        /// <summary>
        /// 判断字符串中是否包含1个以上中文字符
        /// </summary>
        public static bool IncludeChineseCharacter(this string source)
        {
            string pattern = @"[\u4e00-\u9fa5|\s]+";

            return Regex.IsMatch(source, pattern);
        }

        /// <summary>
        /// 判断是否与指定字符号相同
        /// </summary>
        public static bool Matches(this string source, string compare)
        {
            return String.Equals(source, compare, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 判断是否与指定字符号相同(去掉首尾空格后)
        /// </summary>
        public static bool MatchesTrimmed(this string source, string compare)
        {
            return String.Equals(source.Trim(), compare.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 判断是否匹配指定正则表达式
        /// </summary>
        public static bool MatchesRegex(this string inputString, string matchPattern)
        {
            return Regex.IsMatch(inputString, matchPattern,
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Strips the last specified chars from a string.
        /// </summary>
        public static string Chop(this string sourceString, int removeFromEnd)
        {
            string result = sourceString;
            if ((removeFromEnd > 0) && (sourceString.Length > removeFromEnd - 1))
                result = result.Remove(sourceString.Length - removeFromEnd, removeFromEnd);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public static string PluralToSingular(this string sourceString)
        {
            return sourceString.MakeSingular();
        }

        /// <summary>
        /// Singulars to plural.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string SingularToPlural(this string sourceString)
        {
            return sourceString.MakePlural();
        }

        /// <summary>
        /// Make plural when count is not one
        /// </summary>
        /// <param name="number">The number of things</param>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string Pluralize(this int number, string sourceString)
        {
            if (number == 1)
                return String.Concat(number, " ", sourceString.MakeSingular());
            return String.Concat(number, " ", sourceString.MakePlural());
        }

        /// <summary>
        /// Removes the specified chars from the beginning of a string.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <param name="removeFromBeginning">The remove from beginning.</param>
        /// <returns></returns>
        public static string Clip(this string sourceString, int removeFromBeginning)
        {
            string result = sourceString;
            if (sourceString.Length > removeFromBeginning)
                result = result.Remove(0, removeFromBeginning);
            return result;
        }

        /// <summary>
        /// Removes chars from the beginning of a string, up to the specified string
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <param name="removeUpTo">The remove up to.</param>
        /// <returns></returns>
        public static string Clip(this string sourceString, string removeUpTo)
        {
            int removeFromBeginning = sourceString.IndexOf(removeUpTo);
            string result = sourceString;

            if (sourceString.Length > removeFromBeginning && removeFromBeginning > 0)
                result = result.Remove(0, removeFromBeginning);

            return result;
        }

        /// <summary>
        /// Strips the last char from a a string.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string Chop(this string sourceString)
        {
            return Chop(sourceString, 1);
        }

        /// <summary>
        /// Strips the last char from a a string.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string Clip(this string sourceString)
        {
            return Clip(sourceString, 1);
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        public static string FastReplace(this string original, string pattern, string replacement)
        {
            return FastReplace(original, pattern, replacement, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static string FastReplace(this string original, string pattern, string replacement,
                                         StringComparison comparisonType)
        {
            if (original == null)
                return null;

            if (String.IsNullOrEmpty(pattern))
                return original;

            int lenPattern = pattern.Length;
            int idxPattern = -1;
            int idxLast = 0;

            StringBuilder result = new StringBuilder();

            while (true)
            {
                idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);

                if (idxPattern < 0)
                {
                    result.Append(original, idxLast, original.Length - idxLast);
                    break;
                }

                result.Append(original, idxLast, idxPattern - idxLast);
                result.Append(replacement);

                idxLast = idxPattern + lenPattern;
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns text that is located between the startText and endText tags.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <param name="startText">The text from which to start the crop</param>
        /// <param name="endText">The endpoint of the crop</param>
        /// <returns></returns>
        public static string Crop(this string sourceString, string startText, string endText)
        {
            int startIndex = sourceString.IndexOf(startText, StringComparison.CurrentCultureIgnoreCase);
            if (startIndex == -1)
                return String.Empty;

            startIndex += startText.Length;
            int endIndex = sourceString.IndexOf(endText, startIndex, StringComparison.CurrentCultureIgnoreCase);
            if (endIndex == -1)
                return String.Empty;

            return sourceString.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// Removes excess white space in a string.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string Squeeze(this string sourceString)
        {
            char[] delim = { ' ' };
            string[] lines = sourceString.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            foreach (string s in lines)
            {
                if (!String.IsNullOrEmpty(s.Trim()))
                    sb.Append(s + " ");
            }
            //remove the last pipe
            string result = Chop(sb.ToString());
            return result.Trim();
        }

        /// <summary>
        /// Removes all non-alpha numeric characters in a string
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string ToAlphaNumericOnly(this string sourceString)
        {
            return Regex.Replace(sourceString, @"\W*", "");
        }

        /// <summary>
        /// Creates a string array based on the words in a sentence
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        public static string[] ToWords(this string sourceString)
        {
            string result = sourceString.Trim();
            return result.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Strips all HTML tags from a string
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        /// <returns></returns>
        public static string StripHTML(this string htmlString)
        {
            return StripHTML(htmlString, String.Empty);
        }

        /// <summary>
        /// Strips all HTML tags from a string and replaces the tags with the specified replacement
        /// </summary>
        /// <param name="htmlString">The HTML string.</param>
        /// <param name="htmlPlaceHolder">The HTML place holder.</param>
        /// <returns></returns>
        public static string StripHTML(this string htmlString, string htmlPlaceHolder)
        {
            const string pattern = @"<(.|\n)*?>";
            string sOut = Regex.Replace(htmlString, pattern, htmlPlaceHolder);
            sOut = sOut.Replace("&nbsp;", String.Empty);
            sOut = sOut.Replace("&amp;", "&");
            sOut = sOut.Replace("&gt;", ">");
            sOut = sOut.Replace("&lt;", "<");
            return sOut;
        }
        /// <summary>
        /// xml特殊字符转义
        /// </summary>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static string StripXML(this string xmlString)
        {
            string sOut = xmlString.Replace("&", " &amp;");
            sOut = sOut.Replace("<", "&lt;");
            sOut = sOut.Replace(">", "&gt;");
            sOut = sOut.Replace("\"", "&quot;");
            sOut = sOut.Replace("'", "&apos;");
            return sOut;
        }

        public static List<string> FindMatches(this string source, string find)
        {
            Regex reg = new Regex(find, RegexOptions.IgnoreCase);

            List<string> result = new List<string>();
            foreach (Match m in reg.Matches(source))
                result.Add(m.Value);
            return result;
        }

        /// <summary>
        /// Converts a generic List collection to a single comma-delimitted string.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static string ToDelimitedList(this IEnumerable<string> list)
        {
            return ToDelimitedList(list, ",");
        }

        /// <summary>
        /// Converts a generic List collection to a single string using the specified delimitter.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        public static string ToDelimitedList(this IEnumerable<string> list, string delimiter)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in list)
                sb.Append(String.Concat(s, delimiter));
            string result = sb.ToString();
            result = Chop(result);
            return result;
        }

        /// <summary>
        /// Strips the specified input.
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <param name="stripValue">The strip value.</param>
        /// <returns></returns>
        public static string Strip(this string sourceString, string stripValue)
        {
            if (!String.IsNullOrEmpty(stripValue))
            {
                string[] replace = stripValue.Split(new[] { ',' });
                for (int i = 0; i < replace.Length; i++)
                {
                    if (!String.IsNullOrEmpty(sourceString))
                        sourceString = Regex.Replace(sourceString, replace[i], String.Empty);
                }
            }
            return sourceString;
        }

        /// <summary>
        /// Converts ASCII encoding to Unicode
        /// </summary>
        /// <param name="asciiCode">The ASCII code.</param>
        /// <returns></returns>
        public static string AsciiToUnicode(this int asciiCode)
        {
            Encoding ascii = Encoding.UTF32;
            char c = (char)asciiCode;
            Byte[] b = ascii.GetBytes(c.ToString());
            return ascii.GetString((b));
        }

        /// <summary>
        /// 将文本的换行符转换为HTML的换行符
        /// </summary>
        public static string HtmlNewline(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return string.Empty;
            }
            if (source.Contains(Environment.NewLine))
            {
                source = source.Replace(Environment.NewLine, "<br />");
            }
            //当用js提交时，换行符为“\n”
            if (source.Contains("\n"))
            {
                source = source.Replace(Environment.NewLine, "\n");
            }
            return source;
        }

        #region 字符串拼接

        public static string StringAnd(this string source, string str, params object[] args)
        {
            return source + str.FormatWith(args);
        }

        #endregion

        

      

        /// <summary>
        /// 对字符串执行不区分大小写的比较
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }


        /// <summary>
        /// 获取安全的HTML字符串，防止跨站脚本攻击XSS
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetSafeHtml(this string source)
        {
            if (source.IsNullOrWhiteSpace())
            {
                return source;
            }
            return Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(source);
        }
        /// <summary>
        /// 字符串按单词截断固定长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length">截断的长度</param>
        /// <returns></returns>
        public static List<string> StringSplitLengthWords(this string str, int length)
        {
            var splitchars = new char[] {' ', ',', '.', '-'};
            var list = new List<string>();
            str = str.ToDBC();
            if (length <= 2 || str.Substring(0, str.Length > length ? length : str.Length).IndexOfAny(splitchars) == -1)
            {
                if (length < str.Length)
                {
                    throw new Exception("字符串格式不对，无法分割");
                }
                list.Add(str);
                return list;
            }
            if (str.Length <= length)
            {
                list.Add(str);
            }
            else
            {
                if (splitchars.Contains(str[length - 1]))
                {
                    list.Add(str.Substring(0, length));
                    list.AddRange(str.Substring(length).StringSplitLengthWords(length));
                }
                else
                {
                    list.Add(str.Substring(0, str.Substring(0, length).LastIndexOfAny(splitchars) + 1));
                    list.AddRange(str.Substring(str.Substring(0, length).LastIndexOfAny(splitchars) + 1).StringSplitLengthWords(length));
                }
            }
            return list;
        }

        /// <summary>
        /// Url参数增加 zxq
        /// </summary>
        /// <param name="url"></param>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        public static string AppendUrlParameters(this string url, NameValueCollection nameValueCollection)
        {
            List<string> listString=new List<string>();
            foreach (string key in nameValueCollection.AllKeys)
            {
                if (string.IsNullOrWhiteSpace(nameValueCollection[key]))
                {
                    continue;
                }
                listString.Add(string.Format("{0}={1}", key, nameValueCollection[key]));
            }

            if (listString.Any())
            {
                string nameValue = string.Join("&", listString.ToArray());

                if (url.Contains("?"))
                {
                    url += "&" + nameValue;
                }
                else
                {
                    url += "?" + nameValue;
                }
            }

            return url;
        }

        /// <summary>
        /// Url参数增加 zxq
        /// </summary>
        public static string AppendUrlParameters(this string url, string name,string value)
        {
            NameValueCollection nameValueCollection=new NameValueCollection {{name, value}};
            return url.AppendUrlParameters(nameValueCollection);
        }

        /// <summary>
        /// Url参数增加 zxq
        /// </summary>
        public static string AppendUrlParametersIf(this string url, string name, string value, Func<bool> func)
        {
            if (func())
            {
                NameValueCollection nameValueCollection = new NameValueCollection {{name, value}};
                return url.AppendUrlParameters(nameValueCollection);
            }
            else
            {
                return url;
            }
        }

        /// <summary>
        /// Url参数增加 zxq
        /// </summary>
        public static string AppendUrlParametersIf(this string url, string name, string value,bool condition)
        {
            if (condition)
            {
                NameValueCollection nameValueCollection = new NameValueCollection { { name, value } };
                return url.AppendUrlParameters(nameValueCollection);
            }
            else
            {
                return url;
            }
        }

        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetNumber(this string str)
        {
            if (str.IsNullOrWhiteSpace())
            {
                return "";
            }
            var reg = new Regex(@"[0-9][0-9]*");
            var mc = reg.Matches(str.Trim());
            var sb = new StringBuilder();
            foreach (Match match in mc)
            {
                sb.Append(match.Value);
            }
            return sb.ToString();
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5Encrypt(this string source)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(source, "MD5");
            //var md5 = new MD5CryptoServiceProvider();
            //string str = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)));
            //str = str.Replace("-", "");
            //return str;
        }
        /// <summary>
        /// 荷兰小包提交时运单号替换
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string WayBillNumberReplace(this string source)
        {
            return source.Replace("YT", "LF");
        }
    }


}