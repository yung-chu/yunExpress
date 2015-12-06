using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

namespace WeightHelper
{
    /// <summary>
    /// 电子秤接口信息类，封装COM口数据
    /// </summary>
    public class WeightInformation
    {
        private string _wdata;
        private string _wunit;
        private string _qdata;
        private string _qunit;
        private string _percentage;

        /// <summary>
        /// 获取或设置重量
        /// </summary>
        public string WData
        {
            get { return this._wdata; }
            set { this._wdata = value; }
        }

        /// <summary>
        /// 获取或设置重量单位
        /// </summary>
        public string WUnit
        {
            get { return this._wunit; }
            set { this._wunit = value; }
        }

        /// <summary>
        /// 获取或设置数量
        /// </summary>
        public string QData
        {
            get { return this._qdata; }
            set { this._qdata = value; }
        }

        /// <summary>
        /// 获取或设置数量单位
        /// </summary>
        public string QUnit
        {
            get { return this._qunit; }
            set { this._qunit = value; }
        }

        /// <summary>
        /// 获取或设置百分数
        /// </summary>
        public string Percentage
        {
            get { return this._percentage; }
            set { this._percentage = value; }
        }
    }

    /// <summary>
    /// 电子称数据读取类
    /// </summary>
    public class WeightReader : IDisposable
    {
        #region 字段、属性与构造函数

        public SerialPort sp;
        private int _speed = 300;

        /// <summary>
        /// 获取或设置电脑取COM数据缓冲时间，单位毫秒
        /// </summary>
        public int Speed
        {
            get { return this._speed; }
            set
            {
                if (value < 300)
                    throw new Exception("串口读取缓冲时间不能小于300毫秒!");
                this._speed = value;
            }
        }

        public bool InitCom(string PortName)
        {
            return this.InitCom(PortName, 4800, 300);
        }

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="PortName">数据传输端口</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="Speed">串口读数缓冲时间</param>
        /// <returns></returns>
        public bool InitCom(string PortName, int BaudRate, int Speed)
        {
            if (sp != null)
                sp.Dispose();
            sp = new SerialPort(PortName, BaudRate, Parity.None, 8);
            sp.ReceivedBytesThreshold = 10;
            sp.Handshake = Handshake.RequestToSend;
            sp.Parity = Parity.None;

            this.Speed = Speed;
            if (!sp.IsOpen)
            {
                sp.Open();
            }
            return true;

        }

        #endregion

        #region 串口数据读取方法

        /// <summary>
        /// 将COM口缓存数据全部读取
        /// </summary>
        /// <returns></returns>
        public string ReadCom() //返回信息 
        {
            string res = "";
            if (this.sp.IsOpen)
            {


                try
                {

                    res = sp.ReadLine();


                    return res;
                }
                catch (Exception err)
                {

                    return err.Message;
                }

            }
            return "";
        }

        #endregion

        #region 释放所有资源

        public void Dispose()
        {
            if (sp != null && sp.IsOpen)
            {
                sp.Close();

            }

            if (sp != null)
            {

                sp.Dispose();
            }
        }

        #endregion
    }

    public class WeightReaderService
    {
        private WeightReader _wr;
        private Timer _timer;

        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int OutModel { get; set; }

        public Action<string> OnSerialDataReceived;
        public Action<string> OnValueChange { get; set; }

        private string _currentValue;

        public string CurrentValue
        {
            get { return _currentValue; }
            set
            {
                if (_currentValue != value)
                {
                    if (OutModel == 0)
                    {
                        if (OnValueChange != null)
                        {
                            OnValueChange(value);
                        }
                    }
                    else if (OutModel == 1)
                    {
                        StableTextChanged(value);
                    }
                }
                _currentValue = value;
            }
        }

        private int _delayTime = 500; //稳定等待时间

        public int DelayTime
        {
            get { return _delayTime; }
            set { _delayTime = value; }
        }

        public void Start()
        {
            if (_wr != null)
            {
                _wr.sp.DataReceived -= SpDataReceived;
                _wr.Dispose();
            }

            _wr = new WeightReader();
            _wr.InitCom(PortName, BaudRate, 300);
            _wr.sp.ReadTimeout = 1000;
            _wr.sp.DataReceived += SpDataReceived;
        }

        public void Close()
        {
            if (_wr != null)
            {
                _wr.sp.DataReceived -= SpDataReceived;
                _wr.Dispose();
            }
        }

        private void SpDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _wr.sp.ReadLine();
                if (OnSerialDataReceived != null)
                {
                    OnSerialDataReceived(data);
                }
                CurrentValue = Regex.Match(data, @"[\d\.]+").ToString();
            }
            catch (Exception ex)
            {
                CurrentValue = ex.Message;
            }
        }

        private void StableTextChanged(string valueString)
        {
            Debug.WriteLine("当前读数：" + valueString);

            //double valueWeightDisplay;

            string str = Regex.Match(valueString, @"[\d\.]+").ToString();
            //double.TryParse(str, out valueWeightDisplay);


            if (_timer != null)
            {
                _timer.Dispose();
            }


            _timer = new Timer(TimerCallback, str, _delayTime, 0);


        }

        private void TimerCallback(object value)
        {
            _timer.Dispose();

            if (OnValueChange != null)
            {
                OnValueChange(value.ToString());
            }
        }
    }
}


