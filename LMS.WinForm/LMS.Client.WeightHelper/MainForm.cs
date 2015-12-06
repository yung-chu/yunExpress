using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WeightHelper;
using Wuyi.Common;
using Timer = System.Threading.Timer;

namespace LMS.Client.WeightHelper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private WeightReader _wr = null;
        private HttpServer _httpServer = null;
        private readonly string _appConfigPath = AppDomain.CurrentDomain.BaseDirectory + "\\config.xml";
        private readonly ManualResetEvent _comReceivedWaitHandle = new ManualResetEvent(true);
        private const string InitialValue = "00.000";
        private string _currentWeight = InitialValue;
        private const int HttpPort = 49331;
        private int _delayTime = 500;//稳定等待时间
        private double _filterWeight = 0.005;//最小过滤重量
        private Timer _timer = null;
        private bool _initialized = false;
        private bool _isHold= false;
        private readonly LogWriter _logWriter=new LogWriter();

        private void StartWeightReaderService()
        {
            if (_wr != null)
                _wr.Dispose();

            if (cbPort.Text != "" && cbBot.Text != "")
            {
                _wr = new WeightReader();
                _wr.InitCom(cbPort.Text, Convert.ToInt32(cbBot.Text), 300);
                _wr.sp.ReadTimeout = 1000;
                _wr.sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            }
        }

        private void StartHttpService()
        {
            this._httpServer = new HttpServer(HttpPort, DealHttpRequest);
            this._httpServer.ExBack += p => { MessageBox.Show(p.Message); Environment.Exit(0); };
            this._httpServer.Start();
        }

        private void DealHttpRequest(RequestStruct dataStruct)
        {
            try
            {
                ResponseStruct resdata = new ResponseStruct();
                resdata.ResponseSocket = dataStruct.RequestSocket;
                resdata.HttpVersion = dataStruct.HttpVersion;

                string callback = dataStruct.RequestQuerystring("callback");
                string requstWeight = dataStruct.RequestQuerystring("value");

                string response = "";

                switch (dataStruct.RequestPath)
                {
                    case "/GetWeight.self":

                        if (requstWeight == _currentWeight)
                        {
                            _isHold = true;
                            _comReceivedWaitHandle.Reset();
                            _comReceivedWaitHandle.WaitOne(60 * 1000); //阻塞
                            _isHold = false;
                        }

                        response = callback + "(" + "\"" + _currentWeight + "\"" + ")";

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

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string receivedString = (_wr.sp.ReadLine());

                this.Invoke(new MethodInvoker(() =>
                {
                    txtWeightDisplay.Text = receivedString;
                    lblError.Text = "";
                }));

            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    lblError.Text = ex.Message;
                }));
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
            cbOut.SelectedIndex = 1;
            cbBot.SelectedIndex = 2;

            try
            {

                foreach (string s in SerialPort.GetPortNames())
                {
                    cbPort.Items.Add(s);
                }

                if (File.Exists(_appConfigPath))
                {
                    DataSet config = new DataSet();
                    config.ReadXml(_appConfigPath);
                    DataRow row = config.Tables[0].Rows[0];
                    cbPort.Text = row["port"].ToString();
                    cbBot.Text = row["bot"].ToString();
                    cbOut.Text = row["out"].ToString();
                    _delayTime = Convert.ToInt32(row["DelayTime"].ToString());
                    _filterWeight = Convert.ToDouble(row["FilterWeight"].ToString());
                }
                else
                {
                    MessageBox.Show("配置文件缺失，请从新获取");
                    Environment.Exit(0);
                }
                StartWeightReaderService();
                StartHttpService();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message); Environment.Exit(0); 
            }
            finally
            {
                _initialized = true;
            }
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            if(!_initialized) return;

            DataTable dt = new DataTable();
            dt.Columns.Add("port");
            dt.Columns.Add("bot");
            dt.Columns.Add("out");
            dt.Columns.Add("DelayTime");
            dt.Columns.Add("FilterWeight");
            dt.Rows.Add(cbPort.Text, cbBot.Text, cbOut.Text, _delayTime,_filterWeight);

            var ds = new DataSet();
            ds.Tables.Add(dt);

            ds.WriteXml(_appConfigPath);
            ds.Clear();
        }

        private void cbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnConfigChange();
        }

        private void cbBot_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnConfigChange();
        }

        /// <summary>
        /// 重启电子称服务并保存配置
        /// </summary>
        private void OnConfigChange()
        {
            StartWeightReaderService();
            SaveConfig();
        }

        private void CloseAllService()
        {

            if (_wr != null)
                _wr.Dispose();

            if (_httpServer != null)
                _httpServer.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("关闭本程序后网页上将无法读取电子称读数，你确认关闭吗？", "操作提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (_logWriter != null)
                {
                    _logWriter.Dispose();
                }
                Environment.Exit(0);
            }
            e.Cancel = true;
        }

        private void txtWeightDisplay_TextChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("当前读数：" + txtWeightDisplay.Text);

            _logWriter.WriterLine(txtWeightDisplay.Text);

            double valueWeightDisplay;

            double.TryParse(Regex.Match(txtWeightDisplay.Text, @"[\d\.]+").ToString(), out valueWeightDisplay);

            if (valueWeightDisplay <= _filterWeight)
            {
                _currentWeight = InitialValue;
            }
            else
            {
                _currentWeight = valueWeightDisplay.ToString();
            }

            if (_isHold)
            {
                if (cbOut.SelectedIndex == 0)
                {
                    _comReceivedWaitHandle.Set();
                }
                else if (cbOut.SelectedIndex == 1)
                {
                    if (_timer != null)
                    {
                        _timer.Dispose();
                    }

                    double value;

                    double.TryParse(_currentWeight, out value);

                    //if ( value< 0.01)
                    //{
                    //    _timer = new Timer(TimerCallback, _currentWeight, 2000, 0);
                    //}
                    //else
                    //{
                        _timer = new Timer(TimerCallback, _currentWeight, _delayTime, 0);
                    //}
                }

            }

        }

        private void TimerCallback(object value)
        {
            //_currentWeight = value.ToString();
            _comReceivedWaitHandle.Set();
            _timer.Dispose();
        }

    }

    /// <summary>
    /// 写日志记录类
    /// </summary>
    public class LogWriter
    {
        /// <summary>
        /// 写日志文件流
        /// </summary>
        private StreamWriter _logStreamWriter = null;

        /// <summary>
        /// 日志文件名
        /// </summary>
        private string _fileName = null;

        /// <summary>
        /// 定时将日志从缓冲区写入磁盘
        /// </summary>
        private readonly System.Timers.Timer _timer; 

        public LogWriter(string fileName=null)
        {
            _timer = new System.Timers.Timer(10000);//定时周期2秒
            _timer.Elapsed += TimerElapsed;//到2秒了做的事件
            _timer.Enabled = true;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = string.Format("{0}\\Logs\\{1}.txt", Application.StartupPath, DateTime.Now.ToString("yyyy-MM-dd"));
            }

            if (fileName != _fileName)
            {
                _fileName = fileName;
                CreateDirectory(_fileName);
                _logStreamWriter = new StreamWriter(new FileStream(_fileName, FileMode.Append));
            }
        }

        public void WriterLine(string value)
        {
            string fileName = string.Format("{0}\\Logs\\{1}.txt", Application.StartupPath, DateTime.Now.ToString("yyyy-MM-dd"));

            if (fileName != _fileName)
            {
                _fileName = fileName;
                CreateDirectory(_fileName);
                _logStreamWriter = new StreamWriter(new FileStream(_fileName, FileMode.Append));
            }

            _logStreamWriter.Write(string.Format("{0}\t{1}\r\n", DateTime.Now, value));
        }

        private void CreateDirectory(string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
        }

        private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logStreamWriter != null)
            {
                _logStreamWriter.Flush();
            }
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

                    if (_logStreamWriter != null)
                    {
                        _logStreamWriter.Dispose();
                    }
                }

                // Release unmanaged resources
                _mDisposed = true;
            }
        }

        ~LogWriter()
        {
            Dispose(false);
        }

        #endregion
    }
}
