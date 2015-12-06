using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;

namespace LighTake.Infrastructure.Common.Caching
{
    public class RedisCacheProvider : ICache
    {

        private RedisClient _client;
        private RedisClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new RedisClient(Host, Port);
                }
                return _client;
            }
        }

        private string Host
        {
            get { return Tools.GetAppSettings("redis_server"); }
        }
        private int Port
        {
            get
            {
                var p = Tools.GetAppSettings("redis_port");
                int port;
                if (!int.TryParse(p, out port))
                {
                    port = 6379;
                }
                return port;
            }
        }

        public void Add(string key, object value)
        {
            Client.Add(key, value, DateTime.Now.AddYears(100));
        }

        public void Add(string key, object value, int duration)
        {
            Client.Add(key, value, DateTime.Now.AddMinutes(duration));
        }

        public void Add(string key, object value, DateTime absoluteTime)
        {
            Client.Add(key, value, absoluteTime);
        }

        public void Add(string key, object value, string filePath)
        {
            throw new NotImplementedException();
        }

        public bool Exists(string key)
        {
            return Client.Exists(key) > 0;
        }

        public object Get(string key)
        {
            return Client.Get(key);
        }

        public T Get<T>(string key)
        {
            return Client.Get<T>(key);
        }

        public void Remove(string key)
        {
            Client.Remove(key);
        }

        public void Flush()
        {
            Client.FlushAll();
        }
    }
}
