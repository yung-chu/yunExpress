
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using StackExchange.Redis;
using LighTake.Infrastructure.Common;

namespace LighTake.Infrastructure.Common.Caching.Redis
{
    public class StackChangeRedisCacheProvider : ICache
    {
        private readonly double _defaultTimeOutSeconds;
        private readonly IDatabase _cache;
        private static ConnectionMultiplexer _connectionMultiplexer;

        static StackChangeRedisCacheProvider()
        {
            var connection = ConfigurationManager.AppSettings["RedisConnection"];
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultTimeOutSeconds">默认保存时间8小时</param>
        public StackChangeRedisCacheProvider(double defaultTimeOutSeconds = 28800)
        {
            _defaultTimeOutSeconds = defaultTimeOutSeconds;
            _cache = _connectionMultiplexer.GetDatabase();
        }        

        public void Add(string key, object value)
        {
            string s = SerializeUtil.ToJson(value);
            var ts = TimeSpan.FromSeconds(_defaultTimeOutSeconds);
            _cache.StringSet(key, s, ts);
        }

        public void Add(string key, object value, int duration)
        {
            string s = SerializeUtil.ToJson(value);
            var ts = TimeSpan.FromSeconds(duration);
            _cache.StringSet(key, s, ts);
        }

        public void Add(string key, object value, DateTime absoluteTime)
        {
            string s = SerializeUtil.ToJson(value);
            var span = absoluteTime.Subtract(DateTime.Now);
            _cache.StringSet(key, s, span);
        }

        public void Add(string key, object value, string filePath)
        {
            throw new NotImplementedException();
        }

        object ICache.Get(string key)
        {
            return _cache.StringGet(key);
        }

        public T Get<T>(string key)
        {
            string json = _cache.StringGet(key);
            return SerializeUtil.FromJson<T>(json);
            //throw new NotImplementedException();
        }

        public void Flush()
        {
            var endpoints = _connectionMultiplexer.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        public bool Exists(string key)
        {
            return _cache.KeyExists(key);
        }

        public void Remove(string key)
        {
            _cache.KeyDelete(key);
        }
    }
}
