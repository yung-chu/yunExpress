using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using mshtml;
using SHDocVw;

namespace LMS.Client.QuickPrint.Common
{
    public class IEPrintHelper : IDisposable
    {
        private ManualResetEvent documentPrinted = new ManualResetEvent(false);
        private ManualResetEvent documentLoaded = new ManualResetEvent(false);

        private string originalDefaultPrinterName;

        private InternetExplorer ie;

        private HTMLDocument _htmlDocument;

        public HTMLDocument HTMLDocument
        {
            get { return _htmlDocument; }
        }

        public string DocumentHtml;

        public IEPrintHelper()
        {
            ie = new InternetExplorer();
            ie.DocumentComplete += IeDocumentComplete;
            ie.PrintTemplateTeardown += IePrintTemplateTeardown;
        }

        /// <summary>
        /// 页面加载完成
        /// </summary>
        /// <param name="pDisp"></param>
        /// <param name="url"></param>
        private void IeDocumentComplete(object pDisp, ref object url)
        {
            _htmlDocument = (ie.Document as HTMLDocument);

            IHTMLElementCollection hec = _htmlDocument.all;

            foreach (IHTMLElement elem in hec)
            {
                DocumentHtml += elem.innerHTML;
            }

            while (ie.Busy || ie.ReadyState != tagREADYSTATE.READYSTATE_COMPLETE)
            {
                Thread.SpinWait(100);
            }
            Debug.WriteLine(url);
            documentLoaded.Set();
        }

        /// <summary>
        /// 打印模块发送到打印机完成
        /// </summary>
        /// <param name="pDisp"></param>
        private void IePrintTemplateTeardown(object pDisp)
        {
            documentPrinted.Set();
        }

        /// <summary>
        /// 导航到Url
        /// </summary>
        /// <param name="htmlFilename"></param>
        public void Navigate(string htmlFilename)
        {
            documentLoaded.Reset();
            documentPrinted.Reset();

            object missing = Missing.Value;

            ie.Navigate(htmlFilename, ref missing, ref missing, ref missing, ref missing);

            documentLoaded.WaitOne();
        }

        /// <summary>
        /// 导航并直接打印
        /// </summary>
        /// <param name="htmlFilename">网址</param>
        /// <param name="printerName">打印机名称</param>
        public void NavigatePrint(string htmlFilename, string printerName)
        {
            Navigate(htmlFilename);
            Print(printerName);
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="printerName"></param>
        public void Print(string printerName)
        {
            //更改默认打印机
            if (!string.IsNullOrWhiteSpace(printerName))
            {
                originalDefaultPrinterName = GetDefaultPrinterName();
                SetDefaultPrinter(printerName);
            }

            object missing = Missing.Value;

            //等待加载完成
            documentLoaded.WaitOne();

            ie.ExecWB(OLECMDID.OLECMDID_PRINT, OLECMDEXECOPT.OLECMDEXECOPT_DONTPROMPTUSER, ref missing, ref missing);

            //等待打印完成
            documentPrinted.WaitOne();

            //还原默认打印机
            if (!string.IsNullOrWhiteSpace(printerName) && GetDefaultPrinterName() != originalDefaultPrinterName)
            {
                SetDefaultPrinter(originalDefaultPrinterName);
            }
        }

        /// <summary>
        /// 打印预览
        /// </summary>
        public void ShowPrintPreviewDialog()
        {
            object missing = Missing.Value;
            ie.ExecWB(OLECMDID.OLECMDID_PRINTPREVIEW, OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref missing, ref missing);
            //等待打印完成
            documentPrinted.WaitOne();
        }

        public void Close()
        {
            ie.DocumentComplete -= IeDocumentComplete;
            ie.PrintTemplateTeardown -= IePrintTemplateTeardown;
            ie.Quit();
        }

        public static string GetDefaultPrinterName()
        {
            var query = new ObjectQuery("SELECT * FROM Win32_Printer");
            var searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject mo in searcher.Get())
            {
                if (((bool?) mo["Default"]) ?? false)
                {
                    return mo["Name"] as string;
                }
            }

            return null;
        }

        public static bool SetDefaultPrinter(string defaultPrinter)
        {
            using (ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer"))
            {
                using (ManagementObjectCollection objectCollection = objectSearcher.Get())
                {
                    foreach (ManagementObject mo in objectCollection)
                    {
                        if (string.Compare(mo["Name"].ToString(), defaultPrinter, true) == 0)
                        {
                            mo.InvokeMethod("SetDefaultPrinter", null, null);
                            return true;
                        }
                    }
                }
            }
            return true;
        }

        #region Dispose

        private bool _mDisposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_mDisposed)
            {
                if (disposing)
                {
                    // Release managed resources
                }

                // Release unmanaged resources
                this.Close();
                _mDisposed = true;
            }
        }

        ~IEPrintHelper()
        {
            Dispose(false);
        }

        #endregion
    }

}
