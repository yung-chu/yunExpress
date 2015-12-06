using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LMS.WinForm.Client.Common;
using LMS.WinForm.Client.Storage;
using LMS.WinForm.Client.WayBill;

namespace LMS.WinForm.Client
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
            //this.Close();
        }
       
        private void ReGoodsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmReGoods reGoods=new FrmReGoods();
            reGoods.MdiParent = this;
            reGoods.Show();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout about = new FrmAbout();
            about.MdiParent = this;
            about.Show();
        }

        private void PackageInStorage_Click(object sender, EventArgs e)
        {
            FrmInStorage inStoragt = new FrmInStorage();
            inStoragt.MdiParent = this;
            inStoragt.Show();
        }

        private void PackageOutStorage_Click(object sender, EventArgs e)
        {
            Form outStoragt = new FrmOutStorage();
            outStoragt.MdiParent = this;
            outStoragt.Show();
        }

        private Form _frmScaleSetting;
        private Form _frmPrintSetting;

        private void ScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_frmScaleSetting == null || _frmScaleSetting.IsDisposed)
            {
                _frmScaleSetting = new FrmScaleSetting();
                _frmScaleSetting.MdiParent = this;
            }
            _frmScaleSetting.WindowState = FormWindowState.Normal;
            _frmScaleSetting.Show();
            _frmScaleSetting.Focus();
        }

        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_frmPrintSetting == null || _frmPrintSetting.IsDisposed)
            {
                _frmPrintSetting = new FrmPrintSetting();
                _frmPrintSetting.MdiParent = this;
            }
            _frmPrintSetting.WindowState = FormWindowState.Normal;
            _frmPrintSetting.Show();
            _frmPrintSetting.Focus();
        }
    }
}
