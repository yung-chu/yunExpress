using LighTake.Infrastructure.Common.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;

namespace LighTake.Infrastructure.RabbitManager
{
    public class ConnectionHelper
    {
        static IConnection _rabbitConnection;

        /// <summary>
        /// 获取RabbiMQ Channel
        /// </summary>
        /// <param name="configKey">Rabbits config key(不指定取第一个)</param>
        /// <param name="isStick">黏住(一直等待)</param>
        /// <returns></returns>     
        public static IModel GetChannel(string configKey = "", bool isStick = false)
        {
            if (_rabbitConnection == null)
            {
                _rabbitConnection = GetConnect(configKey, isStick);
            }
            else
            {
                //check connection 是否打开
                if (!_rabbitConnection.IsOpen || _rabbitConnection.CloseReason != null)
                {
                    //创建一个新的
                    _rabbitConnection = GetConnect(configKey, isStick);
                }
            }

            int retryTimes = 1;
            while (retryTimes <= 3)
            {
                try
                {
                    return _rabbitConnection.CreateModel();
                }
                catch (Exception ex)
                {
                    var innerEx = ex.InnerException != null ? ex.InnerException : ex;
                    switch (retryTimes)
                    {
                        case 1:
                            Log.Error("RabbitManager.ConnectionHelper.GetChannel [失败], 1s 之后重试 .<br/>" + innerEx.ToString());
                            Thread.Sleep(1000);
                            break;
                        case 2:
                            Log.Error("RabbitManager.ConnectionHelper.GetChannel [失败], 3s 之后重试 .<br/>" + innerEx.ToString());
                            Thread.Sleep(3000);
                            break;
                        case 3:
                        default:
                            if (isStick)
                            {
                                Log.Error("RabbitManager.ConnectionHelper.GetChannel [失败], 已启用[Stick],5s之后重试 .<br/>" + innerEx.ToString());
                                retryTimes = 0; //重置 , 让它一直尝试 , 如果服务器恢复 ， 客户端不需要人工干预 ，就能恢复正常
                                Thread.Sleep(5000);
                            }
                            break;
                    }
                    retryTimes++;
                }
            }
            throw new Exception("Can not create channel .");
        }


        /// <summary>
        /// 初始化RabbitMQ连接
        /// </summary>
        /// <param name="configKey">Rabbits config key</param>
        /// <param name="isStick">黏住(一直等待)</param>
        /// <returns></returns>
        static IConnection GetConnect(string configKey, bool isStick = false)
        {
            var config = GetConfig(configKey);
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = config.HostName; //string.IsNullOrWhiteSpace(host) ? ConfigurationManager.AppSettings["RabbitMQ_Host"] : host;// "192.168.5.85";
            factory.UserName = config.UserName; //string.IsNullOrWhiteSpace(user) ? ConfigurationManager.AppSettings["RabbitMQ_UserName"] : user;//"Bizlog";
            factory.Password = config.Password; //string.IsNullOrWhiteSpace(password) ? ConfigurationManager.AppSettings["RabbitMQ_Password"] : password;//"Bizlog";
            factory.VirtualHost = config.VirtualHost; //string.IsNullOrWhiteSpace(vhost) ? ConfigurationManager.AppSettings["RabbitMQ_VirtualHost"] : vhost;//"/BizLog/";
            factory.RequestedHeartbeat = 3;

            int retryTimes = 1;
            while (retryTimes <= 3)
            {
                try
                {
                    return factory.CreateConnection();
                }
                catch (Exception ex)
                {
                    var innerEx = ex.InnerException != null ? ex.InnerException : ex;
                    switch (retryTimes)
                    {
                        case 1:
                            Log.Error("RabbitManager.ConnectionHelper.GetConnect [失败], 1s 之后重试 .<br/>" + innerEx.ToString());
                            Thread.Sleep(1000);
                            break;
                        case 2:
                            Log.Error("RabbitManager.ConnectionHelper.GetConnect [失败], 3s 之后重试 .<br/>" + innerEx.ToString());
                            Thread.Sleep(3000);
                            break;
                        case 3:
                        default:
                            if (isStick)
                            {
                                Log.Error("RabbitManager.ConnectionHelper.GetConnect [失败], 已启用[Stick],5s之后重试 .<br/>" + innerEx.ToString());
                                retryTimes = 0; //重置 , 让它一直尝试 , 如果服务器恢复 ， 客户端不需要人工干预 ，就能恢复正常
                                Thread.Sleep(5000);
                            }
                            break;
                    }
                    retryTimes++;
                }
            }

            throw new Exception(string.Concat("Can not setup connection to ", factory.HostName));
        }

        static RabbitMQConfig GetConfig(string configKey)
        {
            RabbitMQConfig config = new RabbitMQConfig();
            string value = string.Empty;

            var rabbits = ConfigurationManager.GetSection("Rabbits") as System.Collections.Specialized.NameValueCollection;
            if (rabbits.Count == 0) throw new ArgumentNullException("Config文件中Rabbits节点不存在。");
            if (!string.IsNullOrWhiteSpace(configKey))
            {
                value = rabbits[configKey];
            }
            else
            {
                value = rabbits[0];
            }


            if (!string.IsNullOrWhiteSpace(value))
            {
                var array = value.Trim().Split('|');
                if (array.Length != 4)
                {
                    throw new ArgumentOutOfRangeException(string.Format("Config 文件中 Rabbits/{0}配置出错.Value格式说明: ip|username|password|vhost 。", configKey));
                }
                else
                {
                    config.HostName = array[0];
                    config.UserName = array[1];
                    config.Password = array[2];
                    config.VirtualHost = array[3];

                    CheckIsNullOrWhiteSpace(config.HostName, "IP");
                    CheckIsNullOrWhiteSpace(config.UserName, "用户名");
                    CheckIsNullOrWhiteSpace(config.Password, "密码");
                    CheckIsNullOrWhiteSpace(config.VirtualHost, "VirtualHost");
                }
            }
            else
            {
                throw new ArgumentNullException(string.Format("Config 文件中 Rabbits 下面的 {0} 为空或者不存在。", configKey));
            }

            return config;

        }
        static void CheckIsNullOrWhiteSpace(string str, string argumentName)
        {
            if (string.IsNullOrWhiteSpace(str)) throw new ArgumentNullException(string.Concat(argumentName, "不能为空"));
        }
    }
}
