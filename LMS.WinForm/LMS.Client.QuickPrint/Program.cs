using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace LMS.Client.QuickPrint
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                MessageBox.Show("另一个实例正在运行");
                Environment.Exit(0);
            }

            //处理未捕获的异常   
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常   
            Application.ThreadException += Application_ThreadException;
            //处理非UI线程异常   
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string str = "";
            string strDateInfo = String.Format("出现应用程序未处理的异常：{0}\r\n", DateTime.Now);
            Exception error = e.Exception;
            if (error != null)
            {
                //str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n", error.GetType().Name, error.Message, error.StackTrace);
                str = string.Format("异常类型：{0}\r\n异常消息：{1}\r\n", error.GetType().Name, error.ToString());
            }
            else
            {
                str = string.Format("应用程序线程错误:{0}", e);
            }


            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string str = "";
            Exception error = e.ExceptionObject as Exception;
            string strDateInfo = String.Format("出现应用程序未处理的异常：{0}\r\n", DateTime.Now);
            if (error != null)
            {
                //str = string.Format(strDateInfo + "Application UnhandledException:{0};\n\r堆栈信息:{1}", error.Message, error.StackTrace);
                str = string.Format("Application UnhandledException:{0};\n\r", error.Message);
            }
            else
            {
                str = string.Format("Application UnhandledError:{0}", e);
            }

            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}
