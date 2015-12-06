using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Amib.Threading;
using Newtonsoft.Json;
using LighTake.Infrastructure.CommonQueue;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.Client.SubmitOrder
{
    internal class Program
    {
        private static int _sleepTime = 5*1000;
        private static int _doItemPer = 50;
        private static int _threadCount = 1;

        private const string QueueName = "SubmitOrder";
        private static bool _break; //中断
        private static readonly ManualResetEvent WaitPerWorkComplete = new ManualResetEvent(true);

        private static readonly string SubmitFailDirectory = Environment.CurrentDirectory + "\\SubmitFail\\";
        private static readonly string OrderIdsFile = Environment.CurrentDirectory + "\\orderIds.txt";

        #region 防止意外关闭

        public delegate bool HandlerRoutine(int dwCtrlType);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handlerRoutine, bool add);

        #endregion

        private static void Main(string[] args)
        {
            CheckSingleton();
            GetSleepTime();
            GetDoItemPer();
            GetThreadCount();

            if (!SetConsoleCtrlHandler(HandlerRoutineMethod, true))
            {
                Log.Info("Unable to install event handler!");
            }

            Log.Info("初始化完毕!");
                // 参数：订单轮询时间[_sleepTime]:{0} ,每次处理订单数[_doItemPer]:{1},线程数[_threadCount]:{2}", _sleepTime, _doItemPer, _threadCount));

            //Log.Info("---------------------------------------");

            //开启记录订单提交失败监控线程
            new Thread(RecordOrderSubmitErrorToDbWork).Start();

            //处理订单
            DoWork();

            //var threadPool = new SmartThreadPool {MaxThreads = 3};

            //for (int i = 0; i < _threadCount; i++)
            //{
            //    threadPool.QueueWorkItem(DoWork);
            //}

            //threadPool.WaitForIdle();
        }

        /// <summary>
        /// 检查是否单进程
        /// </summary>
        private static void CheckSingleton()
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                Console.WriteLine("另一个实例正在运行");
                Environment.Exit(0);
            }
        }

        public static bool HandlerRoutineMethod(int dwCtrlType)
        {
            _break = true;

            Log.Info("等待处理完成...");

            WaitPerWorkComplete.WaitOne();

            return false;
        }

        private static void GetSleepTime()
        {
            var sleepTime = ConfigurationManager.AppSettings["SleepTime"];
            if (sleepTime != null)
            {
                _sleepTime = Convert.ToInt32(sleepTime);
                Log.Info(string.Format("订单轮询时间：{0}毫秒", _sleepTime));
            }
            else
            {
                Log.Info(string.Format("使用默认订单轮询时间：{0}毫秒", _sleepTime));
            }
        }

        private static void GetThreadCount()
        {
            var threadCount = ConfigurationManager.AppSettings["ThreadCount"];
            if (threadCount != null)
            {
                _threadCount = Convert.ToInt32(threadCount);
                Log.Info(string.Format("线程数：{0}个", _threadCount));
            }
            else
            {
                Log.Info(string.Format("线程数：{0}个", _threadCount));
            }
        }

        private static void GetDoItemPer()
        {
            var doItemPer = ConfigurationManager.AppSettings["DoItemPer"];
            if (doItemPer != null)
            {
                _doItemPer = Convert.ToInt32(doItemPer);
                Log.Info(string.Format("每次处理订单数：{0}个", _doItemPer));
            }
            else
            {
                Log.Info(string.Format("使用默认每次处理订单数：{0}个", _doItemPer));
            }
        }

        private static List<string> GetCustomerOrderIdsFromFile()
        {
            if (File.Exists(OrderIdsFile))
            {
                return File.ReadAllLines(OrderIdsFile).Where(p => !string.IsNullOrWhiteSpace(p)).Distinct().ToList();
            }
            return null;
        }

        private static void RemoveCustomerOrderIdsInFile(ICollection<string> lines)
        {
            if (File.Exists(OrderIdsFile))
            {
                List<string> newLines = File.ReadAllLines(OrderIdsFile).ToList();
                newLines.RemoveAll(lines.Contains);
                File.WriteAllLines(OrderIdsFile, newLines);
            }
        }

        private static List<int> GetCustomerOrderIds(int countMax = 20)
        {
            List<int> customerOrderIds = new List<int>();

            //先尝试从文件中获取订单ID
            List<string> customerOrderIdsFromFile = GetCustomerOrderIdsFromFile();
            if (customerOrderIdsFromFile != null && customerOrderIdsFromFile.Any())
            {
                customerOrderIds.AddRange(customerOrderIdsFromFile.Select(p => Convert.ToInt32(p)));

                Log.Info(string.Format("从文件中获取到订单数:{0}", customerOrderIdsFromFile.Count()));

                return customerOrderIds;
            }

            do
            {

                var message = QueueHelper.Dequeue(QueueName);
                if (string.IsNullOrWhiteSpace(message))
                    break;

                //记录订单ID到文件
                File.AppendAllText(OrderIdsFile, message+Environment.NewLine);

                customerOrderIds.Add(Convert.ToInt32(message));

                Log.Info(string.Format("获取到订单ID:{0}", message));

            } while (customerOrderIds.Count < countMax);

            return customerOrderIds;
        }

        //private static bool WaitEnqueue()
        //{
        //    LtQueue ltQueue = new LtQueue();

        //    return ltQueue.WaitEnqueue(QueueName, 5*60*1000);
        //}

        private static void DoWork()
        {
            Stopwatch stopwatch1 = new Stopwatch();

            while (!_break)
            {
                try
                {
                    Log.Info("正在获取订单...");

                    List<int> customerOrderIds = GetCustomerOrderIds(_doItemPer);

                    if (customerOrderIds.Count == 0)
                    {
                        Log.Info("没有获取到需要处理的订单");
                        Thread.Sleep(_sleepTime);
                        continue;
                    }
                    Log.Info(string.Format("开始处理这批订单，共{0}个", customerOrderIds.Count));

                    stopwatch1.Restart();

                    WaitPerWorkComplete.Reset();

                    //处理订单
                    new CustomerOrderService().OrderSubmit(customerOrderIds);

                    //从文件中移除
                    RemoveCustomerOrderIdsInFile(customerOrderIds.Select(p => p.ToString()).ToList());

                    WaitPerWorkComplete.Set();

                    Log.Info(string.Format("订单ID:{0} 处理完成\r\n耗时：{1}秒",
                                           string.Join(",", customerOrderIds.Select(p => p.ToString())),
                                           stopwatch1.Elapsed.TotalSeconds));
                }
                catch (Exception ex)
                {
                    Log.Exception(ex);
                    Thread.Sleep(_sleepTime);
                }
                finally
                {
                    Log.Info("处理完成!");
                }
            }
        }


        /// <summary>
        ///把保存在文件中的订单提交失败的错误信息记录到数据库
        /// </summary>
        private static void RecordOrderSubmitErrorToDbWork()
        {
            while (true)
            {
                Thread.Sleep(1000*60);

                if (!Directory.Exists(SubmitFailDirectory))
                {
                    Directory.CreateDirectory(SubmitFailDirectory);
                }

                var files = Directory.GetFiles(SubmitFailDirectory, "*.txt");

                if (files.Length == 0)
                {
                    Log.Info("没有找到需要记录到数据库的订单失败的错误信息!");
                    continue;
                }

                files.ToList().ForEach(file =>
                    {
                        try
                        {
                            string text = File.ReadAllText(file);
                            List<OrderSubmitResult> listOrderSubmitResult =
                                JsonConvert.DeserializeObject<List<OrderSubmitResult>>(text);

                            if (listOrderSubmitResult == null || !listOrderSubmitResult.Any()) return;


                            new CustomerOrderService().RecordOrderSubmitErrorToDb(listOrderSubmitResult);

                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Log.Exception(ex);
                        }
                    });
            }
        }
    }
}
