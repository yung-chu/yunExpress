using System;
using System.Collections;
using System.Web;

namespace LighTake.Infrastructure.Web.Utities
{
    /// <summary>
    /// 存储在此的数据可以在当前线程、当前WEB请求中共享
    /// </summary>
    /// <remarks>
    /// 编制人员 : 莫涛[Kevin]
    /// 完成时间 : 2010年07月20日
    /// 修改历史 : 无
    /// </remarks>
    public static class Local
    {
        static readonly ILocalData current = new LocalData();

        /// <summary>
        /// 获取当前上下文中存储的数据
        /// </summary>
        public static ILocalData Data
        {
            get { return current; }
        }

        private class LocalData : ILocalData
        {
            [ThreadStatic]
            private static Hashtable _localData;
            private static readonly object LOCAL_DATA_KEY = new object();

            private static Hashtable LocalHashtable
            {
                get
                {
                    if (!IsRunningInWeb)
                    {
                        return _localData ??
                        (
                            _localData = new Hashtable()
                        );
                    }
                    else
                    {
                        var web_hashtable = HttpContext.Current.Items[LOCAL_DATA_KEY] as Hashtable;
                        
                        if (web_hashtable == null)
                        {
                            HttpContext.Current.Items[LOCAL_DATA_KEY] = web_hashtable = new Hashtable();
                        }

                        return web_hashtable;
                    }
                }
            }

            /// <summary>
            /// 根据指定的键值获取对应的数据
            /// </summary>
            /// <param name="key">Key</param>
            /// <returns>指定的键值对应的数据</returns>
            public object this[object key]
            {
                get { return LocalHashtable[key]; }
                set { LocalHashtable[key] = value; }
            }

            /// <summary>
            /// 该实例中存储的数据数量
            /// </summary>
            public int Count
            {
                get { return LocalHashtable.Count; }
            }

            /// <summary>
            /// 清除所有数据
            /// </summary>
            public void Clear()
            {
                LocalHashtable.Clear();
            }

            /// <summary>
            /// 是否为WEB模式
            /// </summary>
            public static bool IsRunningInWeb
            {
                get { return HttpContext.Current != null; }
            }
        }
    }
}
