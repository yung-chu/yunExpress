using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using WeightHelper;
using Timer = System.Threading.Timer;

namespace LMS.WinForm.Client
{
    public partial class FrmScaleSetting : Form
    {
        public FrmScaleSetting()
        {
            InitializeComponent();
        }

        private static bool _initialized = false;
        private bool _frmInitialized = false;
        public static WeightReaderService WeightReaderService;

        private static Action<string> _onWeightChange { get; set; }

        public static Action<string> OnWeightChange
        {
            get { return _onWeightChange; }
            set
            {
                _onWeightChange = value;
                if (WeightReaderService != null)
                {
                    WeightReaderService.OnValueChange += _onWeightChange;
                }
            }
        }

        public static Action<string> OnSerialDataReceived;
        public static Action<int> OnScaleModleChange { get; set; }

        /// <summary>
        /// 电子称输出模式:0USB;1串口
        /// </summary>
        private static int _scaleModle = -1;

        public static int ScaleModle
        {
            get { return _scaleModle; }
            set
            {
                if (_scaleModle != value)
                {
                    if (OnScaleModleChange != null)
                    {
                        OnScaleModleChange(value);
                    }
                }
                _scaleModle = value;
            }
        }

        public static void Initialize()
        {
            if (_initialized) return;

            if (ConfigurationManager.AppSettings["ScaleModle"] == "1")
            {
                if (WeightReaderService == null)
                {
                    WeightReaderService = new WeightReaderService();
                }

                WeightReaderService.BaudRate = Convert.ToInt32(ConfigurationManager.AppSettings["ScaleBot"]);
                WeightReaderService.PortName = ConfigurationManager.AppSettings["ScalePort"];
                WeightReaderService.OutModel = Convert.ToInt32(ConfigurationManager.AppSettings["ScaleOut"]);
                WeightReaderService.OnValueChange += OnWeightChange;
                WeightReaderService.OnSerialDataReceived += OnSerialDataReceived;
                WeightReaderService.Start();
                ScaleModle = 1;
            }
            else if (ConfigurationManager.AppSettings["ScaleModle"] == "0")
            {
                ScaleModle = 0;
            }

            _initialized = true;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            if (!_initialized) return;

            StartWeightReaderService();
        }

        /// <summary>
        /// 启动电子秤服务
        /// </summary>
        private void StartWeightReaderService()
        {

            if (!_frmInitialized)
                return;

            ScaleModle = 0;

            if (rdbUSB.Checked)
            {
                UpdateAppSettings("ScaleModle", "0");

                if (WeightReaderService != null)
                {
                    WeightReaderService.Close();
                }

                return;
            }

            if (rdbCOM.Checked)
            {
                UpdateAppSettings("ScaleModle", "1");
            }

            UpdateAppSettings("ScalePort", cbPort.Text);
            UpdateAppSettings("ScaleBot", cbBot.Text);
            UpdateAppSettings("ScaleOut", cbOut.SelectedIndex.ToString());

            if (cbPort.Text != "" && cbBot.Text != "")
            {
                if (WeightReaderService != null)
                {
                    WeightReaderService.Close();
                }

                WeightReaderService = new WeightReaderService();
                WeightReaderService.BaudRate = Convert.ToInt32(cbBot.Text);
                WeightReaderService.PortName = cbPort.Text;
                WeightReaderService.OutModel = cbOut.SelectedIndex;
                WeightReaderService.OnValueChange += OnWeightChange;
                WeightReaderService.OnSerialDataReceived += OnSerialDataReceived;
                WeightReaderService.OnSerialDataReceived += UpdateTxtSerialData;
                WeightReaderService.Start();
                ScaleModle = 1;
            }
            _initialized = true;
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

        private void rdbUSB_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbUSB.Checked)
            {
                cbPort.Enabled = cbBot.Enabled = cbOut.Enabled =txtSerialData.Enabled= false;
            }

            if (rdbCOM.Checked)
            {
                cbPort.Enabled = cbBot.Enabled = cbOut.Enabled = txtSerialData.Enabled = true;
            }

            SaveConfig();
        }

        private void cbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void cbBot_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void cbOut_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WeightReaderService != null)
            {
                WeightReaderService.OutModel = cbOut.SelectedIndex;
            }
        }

        private void ScaleSetting_Load(object sender, EventArgs e)
        {

            try
            {

                if (ConfigurationManager.AppSettings["ScaleModle"] == "0")
                {
                    rdbUSB.Checked = true;
                }
                else if (ConfigurationManager.AppSettings["ScaleModle"] == "1")
                {
                    rdbCOM.Checked = true;
                }

                foreach (string s in SerialPort.GetPortNames())
                {
                    cbPort.Items.Add(s);
                }

                cbPort.Text = ConfigurationManager.AppSettings["ScalePort"];
                cbBot.Text = ConfigurationManager.AppSettings["ScaleBot"];
                cbOut.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["ScaleOut"]);

                Initialize();

                if (WeightReaderService != null)
                {
                    WeightReaderService.OnSerialDataReceived += UpdateTxtSerialData;
                }

            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            finally
            {
                _frmInitialized = true;
            }
        }

        private void UpdateTxtSerialData(string value)
        {
            if (this.IsHandleCreated)
            {
                this.Invoke(new Action(() =>
                {
                    txtSerialData.Text = value;
                }));
            }
        }

        private void FrmScaleSetting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WeightReaderService != null)
            {
                WeightReaderService.OnSerialDataReceived -= UpdateTxtSerialData;
            }
        }
    }

}

