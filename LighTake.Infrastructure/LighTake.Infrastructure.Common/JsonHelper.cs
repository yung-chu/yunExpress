using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// JSON帮助类 
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 将DataTable中的数据转换成JSON格式
        /// </summary>
        /// <param name="dt">数据源DataTable</param>
        /// <param name="displayCount">是否输出数据总条数</param>
        /// <returns></returns>
        public static string CreateJsonParameters(DataTable dt, bool displayCount)
        {
            return CreateJsonParameters(dt, displayCount, dt.Rows.Count);
        }

        /// <summary>
        /// 将DataTable中的数据转换成JSON格式
        /// </summary>
        /// <param name="dt">数据源DataTable</param>
        /// <returns></returns>
        public static string CreateJsonParameters(DataTable dt)
        {
            return CreateJsonParameters(dt, true);
        }

        /// <summary>
        /// 将List中的数据转换成JSON格式
        /// </summary>
        /// <param name="dt">数据源list</param>
        /// <param name="displayCount">是否输出数据总条数</param>
        /// <param name="totalcount">JSON中显示的数据总条数</param>
        /// <returns></returns>
        public static string CreateJsonParameters<T>(List<T> list, bool displayCount, int totalcount)
        {
            if (list.Count == 0)
                return "{\"rows\": 0,\"total\":0}";
            StringBuilder JsonString = new StringBuilder();
            if (list != null)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"rows\":[ ");
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                List<string> titleList = new List<string>();
                titleList = propertys.Select(t => t.Name).ToList();
                foreach (var item in list)
                {
                    JsonString.Append("{ ");
                    int titleLength = 1;
                    foreach (var titleItem in titleList)
                    {
                        PropertyInfo pi = propertys.First(p => p.Name == titleItem);
                        var value = pi.GetValue(item, null);
                        JsonString.Append(CheckPropertyDataType(pi.PropertyType, titleItem, value));
                        if (titleLength == titleList.Count)
                            JsonString.Remove((JsonString.Length - 1), 1);
                        titleLength++;
                    }
                    JsonString.Append("}, ");
                }
                JsonString.Remove((JsonString.Length - 2), 1);
                JsonString.Append("]");
                if (displayCount)
                    JsonString.Append(",\"total\":" + totalcount);
            }
            JsonString.Append("}");
            return new Regex("\\s+").Replace(JsonString.ToString(),"");
        }




        /// <summary>
        /// 将List中的数据转换成JSON格式
        /// </summary>
        /// <param name="dt">数据源list</param>
        /// <param name="displayCount">是否输出数据总条数</param>
        /// <param name="totalcount">JSON中显示的数据总条数</param>
        /// <returns></returns>
        public static string CreateJsonParameters<T>(List<T> list, bool displayCount, int totalcount, List<string> tlst)
        {
            StringBuilder JsonString = new StringBuilder();
            if (list != null)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"rows\":[ ");
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                List<string> titleList = new List<string>();
                titleList = propertys.Select(t => t.Name).ToList();

                foreach (var item in list)
                {
                    JsonString.Append("{ ");
                    int titleLength = 1;
                    foreach (var titleItem in titleList)
                    {

                        PropertyInfo pi = propertys.First(p => p.Name == titleItem);
                        if (!tlst.Any(p => p == pi.Name))
                            continue;
                        var value = pi.GetValue(item, null);
                        JsonString.Append(CheckPropertyDataType(pi.PropertyType, titleItem, value));
                        if (titleLength == tlst.Count)
                            JsonString.Remove((JsonString.Length - 1), 1);
                        titleLength++;
                    }

                    JsonString.Append("}, ");
                }
                JsonString.Remove((JsonString.Length - 2), 1);
                JsonString.Append("]");
                if (displayCount)
                    JsonString.Append(",\"total\":" + totalcount);
            }
            JsonString.Append("}");
            return new Regex("\\s+").Replace(JsonString.ToString(), "");
        }

        /// <summary>
        /// 检测属性数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="titleName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string CheckPropertyDataType(Type type, string titleName, object value)
        {
            if (value == null) value = "";
            if (type == typeof(bool))
            {
                return "\"" + titleName + "\":\"" + value + "\",";
            }
            if (type == typeof(string))
            {
                return "\"" + titleName + "\":" + "\"" + value.ToString().Replace("\"", "\\\"") + "\",";
            }
            return "\"" + titleName + "\":" + "\"" + value + "\",";
        }

        /// <summary>
        /// 将DataTable中的数据转换成JSON格式
        /// </summary>
        /// <param name="dt">数据源DataTable</param>
        /// <param name="displayCount">是否输出数据总条数</param>
        /// <param name="totalcount">JSON中显示的数据总条数</param>
        /// <returns></returns>
        public static string CreateJsonParameters(DataTable dt, bool displayCount, int totalcount)
        {
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        

            if (dt != null)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"rows\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            if (j < dt.Columns.Count - 1)
                            {
                                //if (dt.Rows[i][j] == DBNull.Value) continue;
                                if (dt.Columns[j].DataType == typeof(bool))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" +
                                                      dt.Rows[i][j].ToString().ToLower() + ",");
                                }
                                else if (dt.Columns[j].DataType == typeof(string))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j].ToString().Replace("\"", "\\\"") + "\",");
                                }
                                else
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j] + "\",");
                                }
                            }
                            else if (j == dt.Columns.Count - 1)
                            {
                                //if (dt.Rows[i][j] == DBNull.Value) continue;
                                if (dt.Columns[j].DataType == typeof(bool))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" +
                                                      dt.Rows[i][j].ToString().ToLower());
                                }
                                else if (dt.Columns[j].DataType == typeof(string))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"");
                                }
                                else
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j] + "\"");
                                }
                            }
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            if (j < dt.Columns.Count - 1)
                            {
                                //if (dt.Rows[i][j] == DBNull.Value) continue;
                                if (dt.Columns[j].DataType == typeof(bool))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" +
                                                      dt.Rows[i][j].ToString().ToLower() + ",");
                                }
                                else if (dt.Columns[j].DataType == typeof(string))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j].ToString().Replace("\"", "\\\"") + "\",");
                                }
                                else
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j] + "\",");
                                }
                            }
                            else if (j == dt.Columns.Count - 1)
                            {
                                //if (dt.Rows[i][j] == DBNull.Value) continue;
                                if (dt.Columns[j].DataType == typeof(bool))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" +
                                                      dt.Rows[i][j].ToString().ToLower());
                                }
                                else if (dt.Columns[j].DataType == typeof(string))
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j].ToString().Replace("\"", "\\\"") + "\"");
                                }
                                else
                                {
                                    JsonString.Append("\"JSON_" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                      dt.Rows[i][j] + "\"");
                                }
                            }
                        }
                    }
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]");

                if (displayCount)
                {
                    JsonString.Append(",");

                    JsonString.Append("\"total\":");
                    JsonString.Append(totalcount);
                }

                JsonString.Append("}");
                return new Regex("\\s+").Replace(JsonString.ToString(), "");
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// JSON转换成实体对象
        /// </summary>
        /// <param name="jsonString">数据源json格式参数</param>
        /// <param name="ety">要序列化的实体对象</param>
        /// <returns></returns>
        public static object JsonToEntity(string jsonString, object ety)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(ety.GetType());
            MemoryStream mStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            return serializer.ReadObject(mStream);
        }

        /// <summary>    
        /// JSON序列化    
        /// </summary>    
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            //替换Json的Date字符串    
            string p = @"///Date/((/d+)/+/d+/)///"; /*////Date/((([/+/-]/d+)|(/d+))[/+/-]/d+/)////*/
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }
        /// <summary>    
        /// JSON反序列化    
        /// </summary>    
        public static T JsonDeserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"//Date(1294499956278+0800)//"格式   
            string p =
                @"(((((0[48]00)|(0[1-9]((0[48])|([2468][048])|([13579][26]))))-02-29)|((0[1-9][0-9][0-9])-((((0[13578])|(1[02]))-31)|(((0[1,3-9])|(1[0-2]))-(29|30))|(((0[1-9])|(1[0-2]))-((0[1-9])|(1[0-9])|(2[0-8])))))))|((((((([13579][26])|([2468][048]))00)|([1-9][0-9]((0[48])|([13579][26])|([2468][048]))))-02-29)|(([1-9][0-9][0-9][0-9])-((((0[13578])|(1[02]))-31)|(((0[1,3-9])|(1[0-2]))-(29|30))|(((0[1-9])|(1[0-2]))-((0[1-9])|(1[0-9])|(2[0-8])))))))|((((00((0[48])|([2468][048])|([13579][26])))-02-29)|((00((0[1-9])|([1-9][0-9])))-((((0[13578])|(1[02]))-31)|(((0[1,3-9])|(1[0-2]))-(29|30))|(((0[1-9])|(1[0-2]))-((0[1-9])|(1[0-9])|(2[0-8])))))))";

            //string p =
            //    @"(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)";

            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);

            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj; 
        }

        /// <summary>    
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串    
        /// </summary>    
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>    
        /// 将时间字符串转为Json时间    
        /// </summary>    
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format(@"\/Date({0}+0800)\/", ts.TotalMilliseconds);
            return result;
        }    

    }
}