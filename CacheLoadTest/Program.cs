using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LighTake.Infrastructure.Common.Caching;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;

namespace CacheLoadTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DistributedCache.InitializeWith(new CacheProviderFactory(ConfigurationManager.AppSettings["DistributedCacheProvider"]));

            int max = 10000 * 100;

            string key = DateTime.Now.Ticks.ToString();
            //Parallel.For(1, 100000, t =>
            //{
            for (int t = 1; t <= max; t++)
            {
                DistributedCache.Add(string.Format("{1}_{0}", t, key), string.Format("daniel_{0}", t));
                Console.WriteLine(string.Format("Add {0} completed.", t));
            }
            Console.WriteLine("Add completed.");          
            for (int t = 1; t <= max; t++)
            {
                DistributedCache.Remove(string.Format("{1}_{0}", t, key));
                Console.WriteLine(string.Format("Remove {0} completed.", t));
            };
            Console.WriteLine("Remove completed.");
            Console.ReadLine();
        }
    }
}
