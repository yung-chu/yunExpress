using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace LighTake.Infrastructure.Common.BizLogging
{
    public class Test
    {        
        public static void Main()
        {
            //CompressTest();
            Console.ReadKey();
        }

        public static void CompressTest()
        {           
            Stopwatch stopWatch = new Stopwatch();
            while (true)
            {
                stopWatch.Start();
                BizLogger.WriteLogCS<BizLog>(
                new BizLog()
                {
                    SystemCode = Enums.SystemType.LMS,
                    ModuleName = "BizLog日志系统",

                    Keyword = null,
                    KeywordType = KeywordType.None,

                    Summary = "测试日志",
                    Details = "",

                    IP = "",
                    Mac = "",
                    URL = "",

                    UserCode = "1",
                    UserRealName = "daniel",
                    UserType = Enums.UserType.LMS_User
                },
                null);
                stopWatch.Stop();

                TimeSpan ts = stopWatch.Elapsed;               
                stopWatch.Reset();

                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
                Console.WriteLine("RunTime : " + ts.Milliseconds);
            }
        }
    }
}
