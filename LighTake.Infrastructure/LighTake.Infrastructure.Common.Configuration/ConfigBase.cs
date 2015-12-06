using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace LighTake.Infrastructure.Common.Configuration
{
    /// <summary>
    /// 为系统配置文件提供一个抽象基类
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年04月21日
    /// 修改历史 : 无
    /// </remarks>
    [Serializable]
    public abstract class ConfigBase<T> where T : class, new()
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public abstract string ConfigFileFullPath { get; }

        /// <summary>
        /// 从文件中读取配置信息
        /// </summary>
        /// <returns>配置节点类型</returns>
        public T LoadFromFile()
        {
            T result = default(T);

            StreamReader sr = new StreamReader(ConfigFileFullPath, System.Text.Encoding.UTF8);

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));

                result = ser.Deserialize(sr) as T;
            }
            catch (Exception e)
            {
                throw new Exception("读取配置文件出错:" + e.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

            return result;
        }

        /// <summary>
        /// 保存数据到配置文件
        /// </summary>
        public void Save2ConfigFile()
        {
            StreamWriter sw = new StreamWriter(ConfigFileFullPath, false, System.Text.Encoding.UTF8);

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));

                ser.Serialize(sw, this);
            }
            catch (Exception e)
            {
                throw new Exception("保存配置文件出错:" + e.Message);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
    }
}
