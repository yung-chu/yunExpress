using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Acrobat;
using LMS.WinForm.Client.Common;
using Microsoft.Win32;
using Wuyi.Common;

namespace LMS.WinForm.Client
{
    public partial class FrmPrintSetting : Form
    {
        public FrmPrintSetting()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private HttpServer _httpServer;

        /// <summary>
        /// 开始Http服务
        /// </summary>
        private void StartHttpService()
        {
            this._httpServer = new HttpServer(49321, DealRequest);
            _httpServer.OnException += p => MessageBox.Show(p.ToString());
            this._httpServer.Start();
        }

        private void DealRequest(RequestStruct dataStruct)
        {
            try
            {
                ResponseStruct resdata = new ResponseStruct();
                resdata.ResponseSocket = dataStruct.RequestSocket;
                resdata.HttpVersion = dataStruct.HttpVersion;

                string callback = dataStruct.RequestQuerystring("callback");
                string response = "";

                switch (dataStruct.RequestPath)
                {
                        // 打印
                    case "/Print.self":
                        string url = dataStruct.RequestQuerystring("url");
                        string message;
                        DownLoadAndPrint(HttpUtility.UrlDecode(url), out message);

                        response = callback + "(" + "\"" + message + "\"" + ")";

                        this._httpServer.SendToBrowser(resdata, response);
                        break;

                        // 应答
                    case "/Hello.self":
                        response = callback + "(" + "\"OK\"" + ")";
                        this._httpServer.SendToBrowser(resdata, response);
                        break;

                        // 默认返回404错误
                    default:
                    {
                        resdata.StatusCode = "404";
                        this._httpServer.SendToBrowser(resdata, "Not Found<br>" + dataStruct.RequestUrl);
                        break;
                    }
                }

                dataStruct.Close(); //关闭连接
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //打印等待，避免切换打印机时错误
        private readonly object _printWait = new object();

        private readonly Dictionary<int,AutoResetEvent> _queuePrint=new Dictionary<int, AutoResetEvent>();
        private int _maxIndex = 0;
        private readonly object _queuePrintLock = new object();

        /// <summary>
        /// 申请排队号
        /// </summary>
        /// <returns>排队号</returns>
        public int ApplyQueueNumber()
        {
            lock (_queuePrintLock)
            {
                _maxIndex++;
                if (_queuePrint.Count==0)//如果没有上一个，无须等待通知
                {
                    _queuePrint.Add(_maxIndex, new AutoResetEvent(true));
                }
                else
                {
                    _queuePrint.Add(_maxIndex, new AutoResetEvent(false));
                }
                return _maxIndex;
            }
        }

        /// <summary>
        /// 通知下一位排队者
        /// </summary>
        /// <param name="queueNumber">排队号</param>
        public void NotifiyNext(int queueNumber)
        {
            int nextQueueNumber = queueNumber+1;
            if (_queuePrint.ContainsKey(nextQueueNumber))
            {
                _queuePrint[nextQueueNumber].Set();
            }
            _queuePrint[queueNumber].Dispose();
            _queuePrint.Remove(queueNumber);
        }

        /// <summary>
        /// 排队，直到上一位完成
        /// </summary>
        /// <param name="queueNumber">排队号</param>
        public void WaitQueue(int queueNumber)
        {
            if (_queuePrint.ContainsKey(queueNumber))
            {
                _queuePrint[queueNumber].WaitOne();
            }
        }

        private bool WebDownLoadAndPrint(string url, out string message)
        {
            bool success = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            webBrowserBusy.WaitOne();

            //webBrowserBusy.Reset();
            documentCompleted.Reset();

            //WebBrowserEx webBrowser = null;

            //this.Invoke(new MethodInvoker(() =>
            //    {
            //        webBrowser = new WebBrowserEx(autoResetEvent);
            //        webBrowser.DocumentText = "";
            //        autoResetEvent.Reset();
            //        webBrowser.Navigate(Uri.EscapeUriString(url));

            //    }));

            this.Invoke(new MethodInvoker(() =>
            {
                webBrowser1.DocumentText = "";
                webBrowser1.Navigate(Uri.EscapeUriString(url));

            }));

            Log(Guid.NewGuid().ToString(),url); //记录日志

            documentCompleted.WaitOne();

            string documentText = "";

            this.Invoke(new MethodInvoker(() =>
            {
                documentText = webBrowser1.DocumentText;

            }));

            if (documentText.Contains("X-UA-Compatible"))
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    if (url.ToLower().Contains("printer=2"))
                    {
                        SetDefaultPrinter(cbbSecondPrinter.Text);
                    }
                    else
                    {
                        SetDefaultPrinter(cbbFirstPrinter.Text);
                    }
                    
                    if (chkAutoPrint.Checked)
                        webBrowser1.Print();
                    else
                        webBrowser1.ShowPrintPreviewDialog();

                    //SetDefaultPrinter(cbbFirstPrinter.Text);

                }));

                success = true;

                message = "PrintOK";
            }
            else
            {
                message = documentText;
                //MessageBox.Show("暂不支持打印的页面");
            }

            webBrowserBusy.Set();

            sw.Stop();

            Debug.WriteLine("下载页面耗时：" + sw.Elapsed.TotalSeconds.ToString());

            return success;
        }

        private bool WebDownLoadAndPrint2(string url, out string message)
        {
            bool success = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            string logKey = Guid.NewGuid().ToString();

            //申请队号
            var queueNumber = ApplyQueueNumber();

            try
            {
                Log(logKey, url); //记录日志

                UpdateStatus(logKey, "准备下载页面");

                using (var ie = new IEPrintHelper())
                {
                    UpdateStatus(logKey, "开始下载页面");
                    url = HttpRequestUnit.ToUrlEncodeChinese(url);
                    ie.Navigate(url);
                    string documentText = ie.DocumentHtml;

                    int retryTime = 0;
                    while (!documentText.Contains("X-UA-Compatible") && retryTime++<3)
                    {
                        UpdateStatus(logKey, string.Format("不支持打印的页面，正在重试第{0}次", retryTime));
                        ie.Navigate(url);
                        documentText = ie.DocumentHtml;
                    }

                    //记录Html日志
                    WriteHtmlLog(url, documentText);

                    if (documentText.Contains("X-UA-Compatible"))
                    {
                        UpdateStatus(logKey, "等待发送到打印机");

                        //等待被通知
                        WaitQueue(queueNumber);

                        //串行执行
                        lock (_printWait)
                        {
                            if (chkAutoPrint.Checked)
                            {
                                if (url.ToLower().Contains("printer=2"))
                                {
                                    ie.Print(cbbSecondPrinter.Text);
                                }
                                else if (url.ToLower().Contains("printer=3"))
                                {
                                    ie.Print(cbbThirdlyPrinter.Text);
                                }
                                else if (url.ToLower().Contains("printer=4"))
                                {
                                    ie.Print(cbbFourthPrinter.Text);
                                }
                                else
                                {
                                    ie.Print(cbbFirstPrinter.Text);
                                }
                            }
                            else
                            {
                                ie.ShowPrintPreviewDialog();
                            }
                        }

                        success = true;

                        message = "PrintOK";

                        UpdateStatus(logKey, "打印完成");
                    }
                    else
                    {
                        message = documentText;
                        //MessageBox.Show(string.Format("暂不支持打印的页面：{0}", url));
                        UpdateStatus(logKey, "不支持打印的页面");
                    }
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;
                UpdateStatus(logKey, ex.Message);
            }

            //通知下一位
            NotifiyNext(queueNumber);

            sw.Stop();

            Debug.WriteLine("下载页面耗时：" + sw.Elapsed.TotalSeconds.ToString());

            return success;
        }

        private bool PdfLoadAndPrint(string url, out string message)
        {
            //申请队号
            var queueNumber = ApplyQueueNumber();

            string logKey = Guid.NewGuid().ToString();

            try
            {

                Log(logKey, url); //记录日志

                string downloadPath = Application.StartupPath + "\\downfiles\\pdf\\";

                if (!Directory.Exists(downloadPath))
                {
                    Directory.CreateDirectory(downloadPath);
                }

                string saveFilePath = downloadPath + Path.GetFileName(url);

                UpdateStatus(logKey, "开始下载文件");

                int reTryTime = 3;

                bool downSuccess= DownloadFile(url, saveFilePath);

                while (!downSuccess && reTryTime-- > 0)
                {
                    downSuccess = DownloadFile(url, saveFilePath);

                }

                if (!downSuccess)
                {
                    message = "下载pdf文件失败";
                    UpdateStatus(logKey, "下载pdf文件失败");
                    return false;
                }

                UpdateStatus(logKey, "等待发送到打印机");

                WaitQueue(queueNumber);

                 //串行执行
                lock (_printWait)
                {
                    SetDefaultPrinter(cbbFirstPrinter.Text);

                    AcrobatPrint(saveFilePath);
                }

                message = "PrintOK";

                UpdateStatus(logKey, "打印完成");

                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;

                UpdateStatus(logKey, ex.Message);

                return false;
            }
            finally
            {
                NotifiyNext(queueNumber);
            }

        }

        private bool DownloadFile(string url, string filePath)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(url, filePath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        private void AcrobatPrint(string filePath)
        {
            AcroApp app = new AcroApp();
            app.Hide();
            AcroAVDoc doc = new AcroAVDoc();
            doc.Open(filePath, "打印预览");
            int pageCount = doc.GetPDDoc().GetNumPages();
            doc.PrintPagesSilent(0, pageCount - 1, 0, 1, 1);
            doc.Close(1);
            app.Exit();
        }

        private bool DownLoadAndPrint(string url, out string message)
        {
            if (url.EndsWith("pdf", false, new CultureInfo("zh-CN")))
            {
                return PdfLoadAndPrint(url, out message);
            }
            else
            {
                return WebDownLoadAndPrint2(url, out message);
            }
        }

        private void Log(string key,string message)
        {
            this.BeginInvoke(new Action(() =>
                {

                    ListViewItem item = new ListViewItem(DateTime.Now.ToString("HH:mm:ss"));
                    item.Tag = key;
                    item.SubItems.AddRange(message.Split('\t'));
                    item.SubItems.Add("");
                    lswPrintHistory.Items.Insert(0, item);
                }));
        }

        /// <summary>
        /// 写Html日志
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        private void WriteHtmlLog(string url, string content)
        {
            lock (this)
            {
                string fileName = Application.StartupPath + "\\downfiles\\html\\" + DateTime.Now.ToString("yyyy-MM-dd_hhmmssfff") + ".html";

                if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                }

                File.WriteAllText(fileName, string.Format("DateTime:{0}\r\nUrl:{1}\r\n{2}", DateTime.Now, url, content));
            }
        }

        private void UpdateStatus(string key,string message)
        {
            this.BeginInvoke(new Action(() =>
            {
                foreach (ListViewItem item in lswPrintHistory.Items)
                {
                    if (key == item.Tag.ToString())
                    {
                        item.SubItems[item.SubItems.Count-1].Text= message;
                    }
                }

            }));
        }


        /// <summary>
        /// 获取本机打印机信息
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalPrinters()
        {
            PrintDocument fPrintDocument = new PrintDocument();
            List<string> fPrinters = new  List<string>() ;
            fPrinters.Add(fPrintDocument.PrinterSettings.PrinterName); // 默认打印机始终出现在列表的第一项
            foreach (String fPrinterName in PrinterSettings.InstalledPrinters)
            {
                if (!fPrinters.Contains(fPrinterName))
                    fPrinters.Add(fPrinterName);
            }
            return fPrinters;
        }
        
        /// <summary>
        /// 调用win api将指定名称的打印机设置为默认打印机
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        [DllImport("winspool.drv")]
        public static extern bool SetDefaultPrinter(string printerName); 

        public static bool SetDefaultPrinter2(string defaultPrinter)
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

        private const string KEY_NAME = @"SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";

        private const string IEVerCode = "8888";//Internet Explorer 8，强制IE8标准模式显示

        /// <summary>
        /// 注册WebBrowser的IE版本
        /// </summary>
        private void RegisterWebBrowser()
        {
            try
            {
                string name = Path.GetFileName(this.GetType().Assembly.Location);

                if (ReadValue(Registry.LocalMachine, KEY_NAME, name) != IEVerCode)
                {
                    SetValue(Registry.LocalMachine, KEY_NAME, name, IEVerCode, RegistryValueKind.DWord);

                    //MessageBox.Show("初始化完成使用，请重启软件");

                    Application.Restart();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        /// <summary>
        /// 写入注册表,如果指定项已经存在,则修改指定项的值
        /// </summary>
        public bool SetValue(RegistryKey keytype, string registryPath, string name, string value, RegistryValueKind valueKind = RegistryValueKind.String)
        {

            RegistryKey registryKey = keytype.OpenSubKey(registryPath, true);

            if (registryKey == null)
            {
                throw (new Exception("要写入的项不存在"));
            }

            registryKey.SetValue(name, value, valueKind);

            return true;

        }

        /// <summary>
        /// 读取注册表
        /// </summary>
        public string ReadValue(RegistryKey keytype, string registryPath, string name)
        {
            RegistryKey registryKey = keytype.OpenSubKey(registryPath, true);

            if (registryKey == null)
            {
                throw (new Exception("要读取的项不存在"));
            }

            var value = registryKey.GetValue(name);

            return value == null ? null : value.ToString();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //RegisterWebBrowser();
            StartHttpService();

            #region 添加打印机
            
            GetLocalPrinters().ForEach(p =>
                {
                    cbbFirstPrinter.Items.Add(p);
                    cbbSecondPrinter.Items.Add(p);
                    cbbFourthPrinter.Items.Add(p);
                    cbbThirdlyPrinter.Items.Add(p);
                });

            cbbFirstPrinter.SelectedIndex = 0;

            string secondPrinterName = ConfigurationManager.AppSettings["SecondPrinterName"];
            string thirdlyPrinterName = ConfigurationManager.AppSettings["ThirdlyPrinterName"];
            string fourthPrinterName = ConfigurationManager.AppSettings["FourthPrinterName"];

            if (string.IsNullOrWhiteSpace(secondPrinterName) || !cbbSecondPrinter.Items.Contains(secondPrinterName))
            {
                cbbSecondPrinter.SelectedIndex = 0;
            }
            else
            {
                cbbSecondPrinter.Text = secondPrinterName;
            }

            if (string.IsNullOrWhiteSpace(thirdlyPrinterName) || !cbbThirdlyPrinter.Items.Contains(thirdlyPrinterName))
            {
                cbbThirdlyPrinter.SelectedIndex = 0;
            }
            else
            {
                cbbThirdlyPrinter.Text = thirdlyPrinterName;
            }

            if (string.IsNullOrWhiteSpace(fourthPrinterName) || !cbbThirdlyPrinter.Items.Contains(fourthPrinterName))
            {
                cbbFourthPrinter.SelectedIndex = 0;
            }
            else
            {
                cbbFourthPrinter.Text = fourthPrinterName;
            }

            #endregion
        }

        private ManualResetEvent documentCompleted = new ManualResetEvent(false);

        private AutoResetEvent webBrowserBusy = new AutoResetEvent(true);

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.ReadyState == WebBrowserReadyState.Complete&& !webBrowser1.IsBusy)
            {
                documentCompleted.Set();
            }
        }

        private void btnShowPrintPreview_Click(object sender, EventArgs e)
        {
            webBrowser1.ShowPrintPreviewDialog();
        }

        private void cbbFirstPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetDefaultPrinter(cbbFirstPrinter.Text);
        }

        private void cbbSecondPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAppSettings("SecondPrinterName", cbbSecondPrinter.Text);
        }

        public static void UpdateAppSettings(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings.AllKeys.Contains(key))
            {
                config.AppSettings.Settings[key].Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, value);
            }
            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void FrmPrintSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this._httpServer!=null)
                this._httpServer.Close();
        }

        private void cbbThirdlyPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAppSettings("ThirdlyPrinterName", cbbThirdlyPrinter.Text);
        }

        private void cbbFourthPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateAppSettings("FourthPrinterName", cbbFourthPrinter.Text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (Process p = new Process())
            {
                p.StartInfo.FileName = "notepad.exe";
                p.StartInfo.Arguments = Application.StartupPath + "\\Resource\\打印使用说明.txt";
                p.Start();
            }
        }

    }
    
}
