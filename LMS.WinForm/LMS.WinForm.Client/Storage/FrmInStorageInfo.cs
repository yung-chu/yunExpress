using System.IO;
using FastReport;
//using LMS.Core;
using LMS.Data.Entity;
using LMS.WinForm.Client.Common;
using LMS.WinForm.Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LMS.WinForm.Client.Storage
{
    public partial class FrmInStorageInfo : Form
    {
        string instorageinfoID = string.Empty;

        public FrmInStorageInfo( string inStorageID)
        {
            InitializeComponent();
            instorageinfoID = inStorageID;
            Bind(inStorageID);
        }

        List<InstorageInfoViewModel> BrandModel(string inStorageID)
        {
            List<InstorageInfoViewModel> model = InvokeWebApiHelper.GetPrintInStorageInvoice(inStorageID);
            return model;
        }

        void Bind(string inStorageID)
        {
            List<InstorageInfoViewModel> modelList = BrandModel(inStorageID);
            if (modelList != null )
            {

                lblInStorageCodeText.Text = modelList[0].InStorageID;
                lblCustomersText.Text = modelList[0].Name;
                lblReceivingText.Text = modelList[0].ReceivingClerk;
                lblSettlementText.Text = modelList[0].PaymentName;
                lblCount.Text = modelList[0].TotalQty.ToString();
                lblCountWeight.Text = modelList[0].TotalWeight.HasValue?modelList[0].TotalWeight.Value.ToString("0.000"):"";
                lblCountPhysicalTotalWeight.Text = modelList[0].PhysicalTotalWeight.ToString("0.000");
                lblFeeRMB.Text = "人民币";
                lblPieceCount.Text = modelList.Sum(m=>m.Pieces).ToString();
            }
            else
            {
                MessageBox.Show("查看入仓信息出错");
                this.Close();
            }
            dgvWaybillList.AutoGenerateColumns = false;
            dgvWaybillList.DataSource = modelList;
        }



        private void btnInStorage_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrintSingle_Click(object sender, EventArgs e)
        {
            string frxPath = Application.StartupPath + @"\Resource\InstorageLable.frx";

            if (!File.Exists(frxPath))
            {
                MessageBox.Show("找不到入仓打印模板：" + frxPath);
                return;
            }

            Report report = new Report();
            report.Load(frxPath);
            List<InstorageInfoViewModel> model = BrandModel(instorageinfoID);
            int totalPieces = model.Sum(m => m.Pieces);//该次入仓总件数
            model.ForEach(m => { m.TotalPieces = totalPieces;
                                   m.TotalWayBill = model.Count;
            });

             report.RegisterData(model, "waybillList");
            var datasource = report.GetDataSource("waybillList");
            DataBand data1 = report.FindObject("data1") as DataBand;
            if (data1 != null)
            {
                data1.DataSource = datasource;
            }
            report.Prepare();
            //report.Show();
            report.Print();
            report.Dispose();
        }

        private void FrmInStorageInfo_Load(object sender, EventArgs e)
        {

        }
    }
}
