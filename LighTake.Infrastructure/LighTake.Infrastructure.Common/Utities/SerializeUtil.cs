using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;
using Newtonsoft.Json;

namespace LighTake.Infrastructure.Common
{
    /// <summary>
    /// 序列化及反序列化的辅助类
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2009年08月19日
    /// 修改历史 : 无
    /// </remarks>
    public static class SerializeUtil
    {
      

        /// <summary>
        /// 将对象序列化为二进制字节
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object obj)
        {
            byte[] bytes = new byte[2500];
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter bformatter = new BinaryFormatter();
                bformatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, 0);

                if (memoryStream.Length > bytes.Length)
                {
                    bytes = new byte[memoryStream.Length];
                }
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// 将文件对象序列化到文件中
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="path">文件路径</param>
        /// <param name="fileMode">文件打开模式</param>
        public static void SerializeToBinary(object obj, string path, FileMode fileMode)
        {
            using (FileStream fs = new FileStream(path, fileMode))
            {
                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, obj);
            }
        }

        /// <summary>
        /// 将文件对象序列化到文件中
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="path">文件路径</param>
        public static void SerializeToBinary(object obj, string path)
        {
            SerializeToBinary(obj, path, FileMode.Create);
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns>XML字符串</returns>
        public static string SerializeToXml(object obj)
        {
            string xml = "";
            if (obj != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(memoryStream, obj);
                    memoryStream.Seek(0, 0);
                    xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            else
            {
                xml = "NULL";
            }
            return xml;
        }



        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="ns"></param>
        /// <returns>XML字符串</returns>
        public static string SerializeToXml(object obj,XmlSerializerNamespaces ns)
        {
            string xml = "";
            if (obj != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(memoryStream, obj,ns);
                    memoryStream.Seek(0, 0);
                    xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            else
            {
                xml = "NULL";
            }
            return xml;
        }
        /// <summary>
        /// 将对象序列化为XML字符串返回纯XML，没有声明，前缀和命名空间
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns></returns>
        public static string ObjectToXmlSerializer(Object obj)
        {
            string xml = "";
            if (obj != null)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                //去除xml声明
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.Encoding = Encoding.UTF8;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (XmlWriter writer = XmlWriter.Create(memoryStream, settings))
                    {
                        //去除默认命名空间xmlns:xsd和xmlns:xsi
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        XmlSerializer formatter = new XmlSerializer(obj.GetType());
                        formatter.Serialize(writer, obj, ns);
                    }
                    xml = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
            }
            return xml;
        }

        /// <summary>
        /// 将对象序列化为XML字符串并保存到文件
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="path">保存的文件路径</param>
        /// <param name="fileMode">文件打开模式</param>
        public static void SerializeToXmlFile(object obj, string path, FileMode fileMode)
        {
            using (FileStream fileStream = new FileStream(path, fileMode))
            {
                // Construct a BinaryFormatter and use it to serialize the data to the stream.
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(fileStream, obj);
            }
        }

        /// <summary>
        /// 将对象序列化为XML字符串并保存到文件
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="path">保存的文件路径</param>
        public static void SerializeToXmlFile(object obj, string path)
        {
            SerializeToXmlFile(obj, path, FileMode.Create);
        }

        /// <summary>
        /// 从XML文件中反序列化为Object对象
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="path">XML文件</param>
        /// <returns>反序列化后得到的对象</returns>
        public static object DeserializeFromXmlFile(Type type, string path)
        {
            object result = new object();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(type);
                result = serializer.Deserialize(fileStream);
            }

            return result;
        }

        /// <summary>
        /// 从XML文件中反序列化为对象
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns>反序列化后得到的对象</returns>
        public static object DeserializeFromXml(Type type, string xml)
        {
            object result = new object();
            XmlSerializer serializer = new XmlSerializer(type);
            result = serializer.Deserialize(new StringReader(xml));

            return result;
        }

        /// <summary>
        /// 从XML文件中反序列化为对象
        /// </summary>
        public static T DeserializeFromXml<T>(string xml)
        {
            T result = default(T);
            var serializer = new XmlSerializer(typeof(T));
            result = (T)serializer.Deserialize(new StringReader(xml));

            return result;
        }

        /// <summary>
        /// 从二进制字节中反序列化为对象
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="bytes">字节数组</param>
        /// <returns>反序列化后得到的对象</returns>
        public static object DeserializeFromBinary(Type type, byte[] bytes)
        {
            object result = new object();
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                result = serializer.Deserialize(memoryStream);
            }

            return result;
        }

        /// <summary>
        /// 从二进制文件中反序列化为对象
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="path">二进制文件路径</param>
        /// <returns>反序列化后得到的对象</returns>
        public static object DeserializeFromBinary(Type type, string path)
        {
            object result = new object();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                result = serializer.Deserialize(fileStream);
            }

            return result;
        }

        /// <summary>
        /// 获取对象的转换为二进制的字节大小
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetByteSize(object obj)
        {
            long result;
            BinaryFormatter bFormatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                bFormatter.Serialize(stream, obj);
                result = stream.Length;
            }
            return result;
        }

        /// <summary>
        /// 克隆一个对象
        /// </summary>
        /// <param name="obj">待克隆的对象</param>
        /// <returns>克隆的一个新的对象</returns>
        public static object Clone(object obj)
        {
            object cloned = null;
            BinaryFormatter bFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    bFormatter.Serialize(memoryStream, obj);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    cloned = bFormatter.Deserialize(memoryStream);
                }
                catch //(Exception e)
                {
                    ;
                }
            }

            return cloned;
        }

        /// <summary>
        /// 从文件中读取文本内容
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>文件的内容</returns>
        public static string ReadFile(string path)
        {
            string content = string.Empty;
            using (StreamReader reader = new StreamReader(path))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }

        /// <summary>
        /// 读取嵌入资源的文本内容
        /// </summary>
        /// <param name="fileWholeName">包含命名空间的嵌入资源文件名路径</param>
        /// <returns>文件中的文本内容</returns>
        public static string ReadFileFromEmbedded(string fileWholeName)
        {
            string result = string.Empty;

            Assembly assembly = Assembly.GetEntryAssembly();
            using (TextReader reader = new StreamReader(assembly.GetManifestResourceStream(fileWholeName)))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// 将对象序列化为JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj)
        {
            if ((object)obj == null)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 将JSON字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string jsonString)
        {
            if (jsonString.IsNullOrWhiteSpace())
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

    }
}
