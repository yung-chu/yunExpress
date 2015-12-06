using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Forms;
using LMS.WinForm.Client.Customer;
using LMS.WinForm.Client.WayBill;
using LMS.WinForm.Client.Common;
using System.Text.RegularExpressions;
using LMS.WinForm.Client.Models;
using LighTake.Infrastructure.Common.Caching;
using FastReport;
using LMS.Data.Entity;
using PackageRequest = LMS.Data.Entity.PackageRequest;
using PriceProviderExtResult = LMS.Data.Entity.PriceProviderExtResult;

namespace LMS.WinForm.Client.Storage
{
    public partial class FrmInStorage : Form
    {
        private string CustomerTypeID = string.Empty;
        private string CustomerId = string.Empty;
        private string PaymentTypeID = string.Empty;
        private string txtNumberLast = string.Empty;

        List<WayBillInfoModel> waybillList = new List<WayBillInfoModel>();

        List<WaybillPackageDetailModel> waybilldetailList = new List<WaybillPackageDetailModel>();
        InStorageRequestFormModel requestModel = new InStorageRequestFormModel();
        CheckOnWayBillResponseResult inStorageModel = new CheckOnWayBillResponseResult();
        InStorageWayBillModel ResponseModel = new InStorageWayBillModel();
        private bool _volumeLock = false;

        public FrmInStorage()
        {
            InitializeComponent();
            // BindVenders();
            BindGoodsTypeComboBox();
            BindSensitiveTypeComboBox();

        }

        void Bind()
        {
            this.txtWeight.Text = "0";
            if (this.txtWeight.Text == "0")
            {
                string temp = "1";
                this.txtLong.ReadOnly = true;
                this.txtWidth.ReadOnly = true;
                this.txtHeight.ReadOnly = true;
                this.txtLong.Text = temp;
                this.txtWidth.Text = temp;
                this.txtHeight.Text = temp;
                this.dgvPackageList.Rows.Clear();
            }
        }
        public void getValue(string strV)
        {
            if (!string.IsNullOrWhiteSpace(strV))
            {
                string[] strCode = strV.Split( '|');
                this.txtCustomers.Text = strCode[0];
                CustomerTypeID = strCode[1];
                CustomerId = strCode[2];
                PaymentTypeID = strCode[3];

                if (PaymentTypeID == "1")
                {
                    txtPaymentName.Text = "月结";
                }
                else if (PaymentTypeID == "2")
                {
                    txtPaymentName.Text = "周结";
                }
                else if (PaymentTypeID == "3")
                {
                    txtPaymentName.Text = "预付";
                }
                else if (PaymentTypeID == "4")
                {
                    txtPaymentName.Text = "现金";
                }
                else
                {
                    txtPaymentName.Text = PaymentTypeID;
                }

                new BackgroundLoading(this, () => BindVenders(CustomerId, CustomerTypeID), "正在获取该客户下的运输方式...", "正在获取该客户下的运输方式...").Show();

            }
            else
            {
                CustomerTypeID = string.Empty;
                this.txtCustomers.Text = string.Empty;
            }
        }

        /// <summary>
        /// 绑定物流
        /// </summary>
        void BindVenders(string customerId,string typeId)
        {
            this.Invoke(new Action(() =>
                {
                    this.cbbInStorage.DataSource = InvokeWebApiHelper.GetCustomerShippingMethodsByCustomerId(customerId, typeId, false);
                    this.cbbInStorage.ValueMember = "ShippingMethodId";
                    this.cbbInStorage.DisplayMember = "FullName";
                    this.txtNumber.Focus();
                }));
        }

        /// <summary>
        /// 包裹类型
        /// </summary>
        /// <param name="list"></param>
        public void BindGoodsTypeComboBox()
        {
            List<SelectItem> list = new List<SelectItem>();
            list.Add(new SelectItem() {Value = 1, Text = "包裹"});
            list.Add(new SelectItem() {Value = 2, Text = "文件"});
            list.Add(new SelectItem() {Value = 3, Text = "防水袋"});

            cbbGoodsType.DataSource = list;
            cbbGoodsType.ValueMember = "Value";
            cbbGoodsType.DisplayMember = "Text";
        }

        /// <summary>
        /// 带电类型
        /// </summary>
        /// <param name="list"></param>
        public void BindSensitiveTypeComboBox()
        {
            List<SelectItem> list = new List<SelectItem>();
            list.Add(new SelectItem() {Value = 6, Text = "内置电池"});
            list.Add(new SelectItem() {Value = 7, Text = "配套电池"});

            cbbSensitiveType.DataSource = list;
            cbbSensitiveType.ValueMember = "Value";
            cbbSensitiveType.DisplayMember = "Text";
        }

        private void dgvPackageList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvPackageList.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&e.RowIndex != -1)
            {
                RemoveOnePiece(e.RowIndex);
            }
        }

        //包裹列表删除一行
        private void RemoveOnePiece(int rowIndex)
        {
            string rowsDataPackageDetailId = this.dgvPackageList.Rows[rowIndex].Cells["PackageDetailID"].Value.ToString();

            dgvPackageList.Rows.RemoveAt(rowIndex);

            UpdatedgvPackageListNO();

            int successCount= waybilldetailList.RemoveAll(p => p.PackageDetailID == rowsDataPackageDetailId);

            if (successCount != 1)
            {
                MessageBox.Show("删除异常");
            }
        }

        //刷新包裹明细序号
        private void UpdatedgvPackageListNO()
        {
            int count = this.dgvPackageList.Rows.Count;

            foreach (DataGridViewRow row in this.dgvPackageList.Rows)
            {
                row.Cells["No"].Value = count--;
            }
        }


        private void dgvPackageList_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdatedgvPackageListNO();
        }

        private void dgvPackageList_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdatedgvPackageListNO();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            //判断是否有已扫但未提交
            if (dgvPackageList.Rows.Count > 0 || dgvWaybillList.Rows.Count > 0)
            {
                if (MessageBox.Show("有未保存的入仓数据，你确认重新入仓吗？", "操作提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    clearLeft();
                    clearRight();
                    updateScanInfo();
                }
                else
                {
                    return;
                }
            }

            CustomerList coustomerListShow = new CustomerList();
            coustomerListShow.getTextHandler = getValue;
            coustomerListShow.ShowDialog(this);
        }

        private void txtPieces_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtWeights_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtLong_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {

        }

        void GetOrdernumber()
        {
            InStorageRequestFormModel model = new InStorageRequestFormModel();

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            #region 检查输入数据

            if (string.IsNullOrWhiteSpace(this.txtPieces.Text))
            {
                this.lblTsg.Text = "请输入件数！";
                this.txtPieces.Focus();
                return;
            }

            if (int.Parse(this.txtPieces.Text.Trim()) != dgvPackageList.Rows.Count)
            {
                this.lblTsg.Text = "输入的件数与行数不正确！请检查";
                return;
            }

            #endregion

            WayBillInfoModel waybillModel = new WayBillInfoModel();

            CustomerInfoPackageRequest waybillRequest = new CustomerInfoPackageRequest();
            List<PackageRequest> packageRequestList = new List<PackageRequest>();

            //生成运费请求
            foreach (var package in waybilldetailList)
            {
                PackageRequest packageRequest = new PackageRequest();
                packageRequest.Weight = package.Weight.Value;
                packageRequest.Length = package.Length.Value;
                packageRequest.Width = package.Width.Value;
                packageRequest.Height = package.Height.Value;
                packageRequestList.Add(packageRequest);
            }

            waybillRequest.CustomerId = Guid.Parse(this.CustomerId);
            waybillRequest.CountryCode = ResponseModel.CountryCode;
            waybillRequest.ShippingMethodId = (int)cbbInStorage.SelectedValue;
            waybillRequest.ShippingTypeId = (int)cbbGoodsType.SelectedValue;
            waybillRequest.Packages = packageRequestList;
            waybillRequest.EnableTariffPrepay = ResponseModel.EnableTariffPrepay;//是否启用运费预付服务
            waybillRequest.ShippingInfo = inStorageModel.ShippingInfo;

            //计算运费
            PriceProviderExtResult priceResult = InvokeWebApiHelper.PostCustomerPriceAuto(waybillRequest);

            if (priceResult == null)
            {
                this.lblTsg.Text = "网络错误，请重试";
                return;
            }

            //如果不能邮寄
            if (!priceResult.CanShipping)
            {
                this.lblTsg.Text =priceResult.Message;

                clearLeft();

                txtNumber.Focus();
                return;
            }

            var sumSettleWeight = priceResult.Weight;//结算重量
            var sumWeight = priceResult.Weight;

            #region 包裹长宽高求和
            
            decimal sumLeng = 0;
            decimal sumWidth = 0;
            decimal sumHeight = 0;
            if (priceResult.PackageDetails.Count() > 0)
            {
                sumLeng = (from s in priceResult.PackageDetails
                           select s.Length).Sum();
                sumWidth = (from s in priceResult.PackageDetails
                            select s.Width).Sum();
                sumHeight = (from s in priceResult.PackageDetails
                             select s.Height).Sum();
            }

            #endregion

            if (!waybillList.Exists(p => p.WayBillNumber.Equals(ResponseModel.WayBillNumber)))
            {
                waybillModel.CountryCode = ResponseModel.CountryCode;
                waybillModel.Pieces = Convert.ToInt32(txtPieces.Text.Trim());
                
                waybillModel.SettleWeight = sumSettleWeight;
                waybillModel.Length = sumLeng;
                waybillModel.Width = sumWidth;
                waybillModel.Height = sumHeight;
                waybillModel.WayBillNumber = ResponseModel.WayBillNumber;
                waybillModel.TrackingNumber = ResponseModel.TrackingNumber;
                waybillModel.CustomerOrderNumber = ResponseModel.CustomerOrderNumber;
                waybillModel.InShippingMethodID = (int)cbbInStorage.SelectedValue;
                waybillModel.ShippingMethodTypeId = ((ShippingMethodModel) cbbInStorage.SelectedItem).ShippingMethodTypeId;
                waybillModel.InShippingMethodName = cbbInStorage.Text;
                waybillModel.GoodsTypeID = (int)cbbGoodsType.SelectedValue;
                waybillModel.PriceResult = priceResult;
                waybillModel.IsBattery = cbxBattery.Checked;
                waybillModel.SensitiveType = cbxBattery.Checked?(int?)cbbSensitiveType.SelectedValue:null;

                //以下方便测试
                #region 弹出费用明细

                if (File.Exists(Application.StartupPath + @"\debug.txt"))
                {
                    showPriceProviderResult(priceResult);
                }

                #endregion

                #region 包裹明细赋值
                
                for (int j = 0; j < waybilldetailList.Count; j++)
                {
                    var r = waybilldetailList[j];
                    WaybillPackageDetailModel model = new WaybillPackageDetailModel();
                    model.WayBillNumber = r.WayBillNumber;
                    model.Pieces = r.Pieces;
                    model.Weight = priceResult.PackageDetails[j].Weight;
                    model.AddWeight = priceResult.PackageDetails[j].AddWeight;
                    model.SettleWeight = priceResult.PackageDetails[j].SettleWeight;
                    model.Length = r.Length;
                    model.Width = r.Width;
                    model.Height = r.Height;
                    model.LengthFee = priceResult.PackageDetails[j].OverGirthFee;
                    model.WeightFee = priceResult.PackageDetails[j].OverWeightOrLengthFee;
                    waybillModel.WaybillPackageDetaillList.Add(model);
                }

                waybillModel.Weight = waybillModel.WaybillPackageDetaillList.Sum(p => (p.Weight + (p.AddWeight ?? 0)));

                #endregion

               
                waybillList.Add(waybillModel);
            }

            if (cbxPrintLable.Checked)//打印报表
            {
                string frxPath = Application.StartupPath + @"\Resource\PackageLable.frx";

                if (!File.Exists(frxPath))
                {
                    MessageBox.Show("找不到货物标签打印模板：" + frxPath);
                    return;
                }

                Report report = new Report();

                report.Load(frxPath);
                report.RegisterData(waybilldetailList, "waybilldetailList");
                var datasource = report.GetDataSource("waybilldetailList");
                DataBand data1 = report.FindObject("data1") as DataBand;
                if (data1 != null)
                {
                    data1.DataSource = datasource;
                }
                report.Prepare();
                report.PrintSettings.ShowDialog = true;
                //report.Show();
                report.Print();
                report.Dispose();
            }

            updateScanInfo();

            this.dgvWaybillList.Rows.Add(new object[] { waybillModel.WayBillNumber, waybillModel.TrackingNumber, waybillModel.CustomerOrderNumber, waybillModel.InShippingMethodName, waybillModel.Pieces, waybillModel.SettleWeight, waybillModel.CountryCode, "删除" });
            clearLeft();
            txtNumber.Focus();

            PlaySuccessSound();
        }

        //展示费用详情
        private void showPriceProviderResult(PriceProviderExtResult priceResult)
        {
            string feeStr = string.Format("运费:{0}\r\n挂号费:{1}\r\n燃油费:{2}\r\n关税预付服务费:{3}\r\n处理费:{11}\r\n其他费用:{4}\r\n偏远附加费:{5}\r\n超长/超重费:{6}\r\n超周围长费:{7}\r\n安全附加费:{8}\r\n增值税费:{9}\r\n总费用:{10}"
                                          , priceResult.ShippingFee, priceResult.RegistrationFee, priceResult.FuelFee, priceResult.TariffPrepayFee, priceResult.OtherFee,
                                          priceResult.RemoteAreaFee, priceResult.OverWeightOrLengthFee, priceResult.OverGirthFee, priceResult.SecurityAppendFee, priceResult.AddedTaxFee, priceResult.Value,
                                          priceResult.HandlingFee);
            MessageBox.Show(feeStr, "费用明细");
        }

        /// <summary>
        /// 更新扫描信息
        /// </summary>
        private void updateScanInfo()
        {
            lblCount.Text = this.waybillList.Count.ToString();//更新已扫单数

            lblCountWeight.Text = this.waybillList.Sum(w => w.SettleWeight).Value.ToString("0.000") + "KG";//更新总重量
        }

        /// <summary>
        /// 清空左边
        /// </summary>
        private void clearLeft(bool cleartxtNumber=true)
        {
            waybilldetailList.Clear();
            this.dgvPackageList.Rows.Clear();

            if (cleartxtNumber)
            {
                this.txtNumber.Text = string.Empty;
            }

            if (!cbxPieces.Checked && this.txtPieces.Enabled)
            {
                this.txtPieces.Text = string.Empty;
            }
            this.lblCountyName.Text = string.Empty;

            if (!cbxWeight.Checked && this.txtWeights.Enabled)
            {
                this.txtWeights.Text = string.Empty;
            }

            if (!cbxVolume.Checked && this.txtLong.Enabled && this.txtWidth.Enabled && this.txtHeight.Enabled)
            {
                this.txtLong.Text = string.Empty;
                this.txtWidth.Text = string.Empty;
                this.txtHeight.Text = string.Empty;
            }
        }

        /// <summary>
        /// 清空右边
        /// </summary>
        private void clearRight()
        {
            waybillList.Clear(); 
            this.dgvWaybillList.Rows.Clear();
        }

        private void cbxWeight_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxWeight.Checked)
            {
                this.txtWeights.ReadOnly = true;
            }
            else
            {
                this.txtWeights.ReadOnly = false;
            }
        }

        private void cbxVolume_CheckedChanged(object sender, EventArgs e)
        {
            txtLong.ReadOnly = cbxVolume.Checked;
            txtWidth.ReadOnly = cbxVolume.Checked;
            txtHeight.ReadOnly = cbxVolume.Checked;
        }

        private void dgvWaybillList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsANonHeaderButtonCell(e)) return;

            if (dgvWaybillList.Rows[e.RowIndex].Cells[0].Value == null) return;
            string wayBillNumber = dgvWaybillList.Rows[e.RowIndex].Cells[0].Value.ToString();
            dgvWaybillList.Rows.RemoveAt(e.RowIndex);
            waybillList.Remove(waybillList.Find(p => p.WayBillNumber == wayBillNumber));
            waybilldetailList.Remove(waybilldetailList.Find(p => p.WayBillNumber == wayBillNumber));
            updateScanInfo();
            //btnSureReGoods.Enabled = true;
        }

        private bool IsANonHeaderButtonCell(DataGridViewCellEventArgs cellEvent)
        {
            if (dgvWaybillList.Columns[cellEvent.ColumnIndex] is
                DataGridViewButtonColumn &&
                cellEvent.RowIndex != -1)
            {
                return true;
            }
            else { return false; }
        }

        private void txtNumber_KeyDown(object sender, KeyEventArgs e)
        {
            this.txtNumber.Text=this.txtNumber.Text.Trim();

            if (e.KeyCode != Keys.Enter) return;

            txtNumber_Leave(null,null);
        }

        private bool checkHasPackageDetail()
        {
            if (dgvPackageList.Rows.Count > 0)
            {
                if (MessageBox.Show("有未保存的货物体积数据，你确定要重新扫描吗？", "扫描提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                {
                    this.txtNumber.Text = txtNumberLast;
                    return true;
                }
            }
            return false;
        }

        private void btnInStorage_Click(object sender, EventArgs e)
        {
            if (waybillList.Count <= 0)
            {
                MessageBox.Show("入库数据不能为空!");
                return;
            }

            InStorageSaveModel saveModel = new InStorageSaveModel();
            saveModel.CustomerCode = this.txtCustomers.Text.Trim();
            saveModel.OperatorUserName = LoginHelper.CurrentUserName;
            saveModel.ReceivingDate = txtReceivingDate.Text;
            if (waybillList != null && waybillList.Count > 0)
            {
                //遍历每个运单
                foreach (var waybill in waybillList)
                {
                    WayBillInfoSaveModel w = new WayBillInfoSaveModel();
                    w.WayBillNumber = waybill.WayBillNumber;
                    w.Length = waybill.Length.Value;
                    w.Width = waybill.Width.Value;
                    w.Height = waybill.Height.Value;
                    w.SettleWeight = waybill.SettleWeight == null ? 0 : waybill.SettleWeight.Value;
                    w.Weight = waybill.Weight.Value;
                    w.PriceResult = waybill.PriceResult;
                    w.GoodsTypeID = waybill.GoodsTypeID.Value;
                    w.ShippingMethodId = waybill.InShippingMethodID.Value;
                    w.IsBusinessExpress = waybill.ShippingMethodTypeId == 2;
                    w.IsBattery = waybill.IsBattery;
                    w.SensitiveType=waybill.SensitiveType;

                    //遍历运单的每个包裹
                    foreach (var dateil in waybill.WaybillPackageDetaillList)
                    {
                        WaybillPackageDetailModel wayDetailModel = new WaybillPackageDetailModel();
                        wayDetailModel.WayBillNumber = dateil.WayBillNumber;
                        wayDetailModel.Weight = dateil.Weight;
                        wayDetailModel.AddWeight = dateil.AddWeight;
                        wayDetailModel.SettleWeight = dateil.SettleWeight;
                        wayDetailModel.Length = dateil.Length;
                        wayDetailModel.Width = dateil.Width;
                        wayDetailModel.Height = dateil.Height;
                        wayDetailModel.LengthFee = dateil.LengthFee == null ? 0 : dateil.LengthFee.Value;
                        wayDetailModel.WeightFee = dateil.WeightFee == null ? 0 : dateil.WeightFee.Value;
                        w.waybillPackageDetailList.Add(wayDetailModel);
                    }

                    saveModel.WayBillInfoSaveList.Add(w);
                }
            }


            ResponseResult responseResult=null;

            new BackgroundLoading(this, () =>
                {
                    responseResult = InvokeWebApiHelper.CreateInStorageCS(saveModel);
                }, "正在提交到服务器...", "正在提交到服务器...").Show();

            if (responseResult == null)
            {
                MessageBox.Show("保存失败，请重试!");
                return;
            }

            if (responseResult.Result)
            {
                waybillList = new List<WayBillInfoModel>();
                this.dgvWaybillList.Rows.Clear();
                this.lblCount.Text = "0";
                this.lblCountWeight.Text = "0";
                FrmInStorageInfo inStoragt = new FrmInStorageInfo(responseResult.Message);
                inStoragt.ShowDialog();
                //inStoragt.MdiParent = this;
            }
            else
            {
                MessageBox.Show(responseResult.Message);
            }

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {

            string waybillNumber = this.txtWaybillNumber.Text.Trim();
            string trackingNumber = this.txtTrackingNumber.Text.Trim();
            string customersWaybillNumber = this.txtCustomersWaybillNumber.Text.Trim();
            dgvWaybillList.AutoGenerateColumns = false;
            List<WayBillInfoModel> model = new List<WayBillInfoModel>();

            //List<WayBillInfo> ways =
            //    waybillList .GetList(
            //        p => wayBillNumbers.Contains(p.waybillNumber) || wayBillNumbers.Contains(p.trackingNumber) || wayBillNumbers.Contains(p.customersWaybillNumber));
            if (!string.IsNullOrWhiteSpace(waybillNumber))
            {
                model = waybillList.FindAll(p => p.WayBillNumber==waybillNumber);
            }
            else if (!string.IsNullOrWhiteSpace(trackingNumber))
            {
                model = waybillList.FindAll(p => p.TrackingNumber==trackingNumber);
            }

            else if (!string.IsNullOrWhiteSpace(customersWaybillNumber))
            {
                model = waybillList.FindAll(p => p.CustomerOrderNumber==customersWaybillNumber);
            }
            else
            {
                model = waybillList;
            }

            this.dgvWaybillList.Rows.Clear();
            model.ForEach(waybillModel =>
                {
                    this.dgvWaybillList.Rows.Add(new object[] { waybillModel.WayBillNumber, waybillModel.TrackingNumber, waybillModel.CustomerOrderNumber, waybillModel.InShippingMethodName, waybillModel.Pieces, waybillModel.SettleWeight, waybillModel.CountryCode, "删除" });
                });
            
        }

        private void txtHeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = @"^[1-9]\d*$";
                Regex reg = new Regex(s);
                if (!reg.IsMatch(this.txtHeight.Text))
                {
                    btnSubmit.Enabled = false;
                    lblTsg.Text = "请输入正确的数字";
                    return;
                }
                else
                {
                    lblTsg.Text = "";
                    btnSubmit.Enabled = true;
                }

                AddPiece();
            }
        }

        /// <summary>
        /// 检查运单号
        /// </summary>
        private bool CheckWayBill()
        {
            this.lblTsg.Text = "";
            this.lblCountyName.Text = "";

            if (string.IsNullOrWhiteSpace(this.txtCustomers.Text))
            {
                this.lblTsg.Text = "客户不能为空!";
                return false;
            }

            if (cbbInStorage.SelectedValue == null)
            {
                this.lblTsg.Text = "运输方式不能为空!";
                return false;
            }

            if (waybillList.Exists(p => p.WayBillNumber.Equals(this.txtNumber.Text.Trim()) || p.CustomerOrderNumber.Equals(this.txtNumber.Text.Trim())))
            {
                lblTsg.Text = "数据已经存在，不能重复添加";
                return false;
            }

            clearLeft(false);

            InStorageFormModel model = new InStorageFormModel();

            model.CustomerCode = this.txtCustomers.Text.Trim();
            model.GoodsTypeID = (int)cbbGoodsType.SelectedValue;
            model.ShippingMethodId = (int)cbbInStorage.SelectedValue;
            model.WayBillNumber = this.txtNumber.Text.Trim();
            model.CustomerType = int.Parse(CustomerTypeID);
            model.OperatorUserName = LoginHelper.CurrentUserName;


            inStorageModel = InvokeWebApiHelper.CheckOnWayBill(model);
            if (inStorageModel != null)
            {
                this.btnSubmit.Enabled = inStorageModel.Result;
               
                if (inStorageModel.Result)
                {
                    this.lblTsg.Text = "运单: " + model.WayBillNumber + " 扫描通过";
                    this.lblCountyName.Text = inStorageModel.Message;

                    if (this.txtPieces.Enabled && !cbxPieces.Checked)
                    {
                        this.txtPieces.Focus();
                    }
                    else
                    {
                        txtPiecesNext();
                    }

                    return true;
                }
                else    //如果扫描不通过，清空数据
                {
                    this.lblTsg.Text = inStorageModel.Message.Replace("<br/>", "");
                    this.txtNumber.Text = "";

                    waybilldetailList.Clear();
                    this.dgvPackageList.Rows.Clear();

                    this.txtNumber.Focus();

                    this.lblCountyName.Text = string.Empty;

                    if (!cbxWeight.Checked && this.txtWeights.Enabled)
                    {
                        this.txtWeights.Text = string.Empty;
                    }

                    //if (!cbxVolume.Checked)
                    //{
                    //    this.txtLong.Text = string.Empty;
                    //    this.txtWidth.Text = string.Empty;
                    //    this.txtHeight.Text = string.Empty;
                    //}
                }
            }
            else
            {
                MessageBox.Show("网络异常");
            }
            return false;
        }

        /// <summary>
        /// 添加件
        /// </summary>
        private void AddPiece()
        {
            #region 检查数据

            if (waybillList.Exists(p => p.WayBillNumber.Equals(this.txtNumber.Text.Trim()) || p.CustomerOrderNumber.Equals(this.txtNumber.Text.Trim())))
            {
                lblTsg.Text = "数据已经存在，不能重复添加";
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtPieces.Text))
            {
                return;
            }

            if (int.Parse(this.txtPieces.Text.Trim()) == dgvPackageList.Rows.Count)
            {
                lblTsg.Text = "输入件数与行数相等，请确认提交！";
                btnSubmit.Focus();
                return;
            }

            if (int.Parse(this.txtPieces.Text.Trim()) < dgvPackageList.Rows.Count)
            {
                lblTsg.Text = "行数与列表数不对！";
                return;
            }


            if (string.IsNullOrWhiteSpace(this.txtNumber.Text))
            {
                this.lblTsg.Text = "请输入订单号/运单号！";
                this.txtNumber.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtPieces.Text))
            {
                this.lblTsg.Text = "请输入件数！";
                this.txtPieces.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtWeights.Text))
            {
                this.lblTsg.Text = "请输入重量！";
                this.txtWeights.Focus();
                return;
            }

            double weightValue;
            double.TryParse(this.txtWeights.Text, out weightValue);
            if (Convert.ToDouble(weightValue) < 0.005)
            {
                this.lblTsg.Text = "重量不能小于0.005！";
                this.txtWeights.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtLong.Text))
            {
                this.lblTsg.Text = "请输入长！";
                this.txtLong.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtWidth.Text))
            {
                this.lblTsg.Text = "请输入宽！";
                this.txtWidth.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(this.txtHeight.Text))
            {
                this.lblTsg.Text = "请输入高！";
                this.txtHeight.Focus();
                return;
            }

            #endregion

            decimal[] wlw = new decimal[] {decimal.Parse(txtLong.Text.Trim()), decimal.Parse(txtWidth.Text.Trim()), decimal.Parse(txtHeight.Text.Trim())};
            decimal[] wlwSorted = wlw.OrderByDescending(m => m).ToArray();
            WaybillPackageDetailModel waybillDetailModel = new WaybillPackageDetailModel()
                {
                    PackageDetailID = Guid.NewGuid().ToString(), //唯一包裹序号
                };

            requestModel.CustomerCode = this.txtCustomers.Text.Trim();
            requestModel.GoodsTypeID = (int) cbbGoodsType.SelectedValue;
            requestModel.ShippingMethodId = (int) cbbInStorage.SelectedValue;
            requestModel.IsBusinessExpress = ((ShippingMethodModel) cbbInStorage.SelectedItem).ShippingMethodTypeId == 2;
            requestModel.OperatorUserName = LoginHelper.CurrentUserName;
            requestModel.WayBillNumber = this.txtNumber.Text.Trim();
            requestModel.CustomerType = int.Parse(CustomerTypeID);
            requestModel.Weight = decimal.Parse(txtWeights.Text.Trim());
            requestModel.Length = wlwSorted[0];
            requestModel.Width = wlwSorted[1];
            requestModel.Height = wlwSorted[2];
            waybillDetailModel.ShippingMethod = cbbInStorage.SelectedText;
            waybillDetailModel.WayBillNumber = txtWaybillNumber.Text.Trim();
            waybillDetailModel.Weight = decimal.Parse(txtWeights.Text.Trim());
            waybillDetailModel.Length = wlwSorted[0];
            waybillDetailModel.Width = wlwSorted[1];
            waybillDetailModel.Height = wlwSorted[2];

            //检查单个包裹
            ResponseModel = InvokeWebApiHelper.CheckOnInStorage(requestModel);
            if (ResponseModel == null)
            {
                return;
            }

            //检查失败
            if (!ResponseModel.IsSuccess)
            {
                this.lblTsg.Text = ResponseModel.Message.Replace("<br/>", "");
                PlayFailSound();
                return;
            }

            if (ResponseModel.SettleWeight == 0)
            {
                MessageBox.Show("错误：结算重量为零");
                return;
            }


            waybilldetailList.Add(waybillDetailModel);
            this.dgvPackageList.Rows.Insert(0, new object[] {0, waybillDetailModel.PackageDetailID, waybillDetailModel.Weight, waybillDetailModel.Length, waybillDetailModel.Width, waybillDetailModel.Height});

            #region 清空数据，重新定位光标
            
            if (!cbxWeight.Checked)
                txtWeights.Focus();
            else if (cbxVolume.Checked)
            {
                txtHeight.Focus();
            }
            else
            {
                txtLong.Focus();
            }

            if (!cbxWeight.Checked && txtWeights.Enabled)
            {
                txtWeights.Text = "";
            }
            if (!cbxVolume.Checked && (txtLong.Enabled && txtWidth.Enabled && txtHeight.Enabled))
            {
                txtLong.Text = "";
                txtWidth.Text = "";
                txtHeight.Text = "";
            }

            #endregion

            
            //如果件数与已录入的件数相等，直接提交
            if (int.Parse(this.txtPieces.Text.Trim()) == dgvPackageList.Rows.Count)
            {
                if (btnSubmit.Enabled)
                    btnSubmit_Click(null, null);
            }
        }

        /// <summary>
        /// 播放成功音
        /// </summary>
        private void PlaySuccessSound()
        {
            try
            {
                string soundPath = Application.StartupPath + @"\Resource\Success.wav";
                if (!File.Exists(soundPath))
                {
                    MessageBox.Show("成功提示音文件不存在:" + soundPath);
                    return;
                }
                SoundPlayer simpleSound = new SoundPlayer(soundPath);
                simpleSound.Play();

                //new SoundPaly().Play(soundPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// 播放成功音
        /// </summary>
        private void PlaySuccessSound2()
        {
            try
            {
                string soundPath = Application.StartupPath + @"\Resource\Success2.wav";
                if (!File.Exists(soundPath))
                {
                    MessageBox.Show("成功提示音文件不存在:" + soundPath);
                    return;
                }
                SoundPlayer simpleSound = new SoundPlayer(soundPath);
                simpleSound.Play();

                //new SoundPaly().Play(soundPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 播放失败音
        /// </summary>
        private void PlayFailSound()
        {
            try
            {
                string soundPath = Application.StartupPath + @"\Resource\Faile.wav";
                if (!File.Exists(soundPath))
                {
                    MessageBox.Show("失败提示音文件不存在:" + soundPath);
                    return;
                }
                SoundPlayer simpleSound = new SoundPlayer(soundPath);
                simpleSound.Play();
                //new SoundPaly().Play(soundPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtPieces_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPiecesNext();
            }
        }

        private void txtPiecesNext()
        {
            string s = @"^[1-9]\d*$";
            Regex reg = new Regex(s);
            if (!reg.IsMatch(this.txtPieces.Text))
            {
                btnSubmit.Enabled = false;
                lblTsg.Text = "请输入正确的数字";
            }
            else
            {
                lblTsg.Text = "";

                if (cbxWeight.Checked)
                {
                    AddPiece();
                }
                else if (!this.txtWeights.Enabled)
                {
                    if (!string.IsNullOrWhiteSpace(this.txtWeights.Text) && Convert.ToDouble(this.txtWeights.Text) > 0.005)
                    {
                         AddPiece();
                    }
                }
                else
                {
                    this.txtWeights.Focus();
                }

                btnSubmit.Enabled = true;
            }
        }

        private void txtWeights_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtWeightsNext();
            }
        }

        private void txtWeightsNext()
        {
            decimal weights;

            if (new Regex("^[0-9]+(.[0-9]{1,3})?$").IsMatch(this.txtWeights.Text.Trim()) && decimal.TryParse(this.txtWeights.Text.Trim(), out weights))
            {
                lblTsg.Text = "";

                if (cbxVolume.Checked || (!this.txtLong.Enabled && !this.txtWidth.Enabled && !this.txtHeight.Enabled))
                {
                    AddPiece();
                    return;
                }
                else
                {
                    this.txtLong.Focus();
                }


                btnSubmit.Enabled = true;

                if (string.IsNullOrWhiteSpace(this.txtWeight.Text) || weights > decimal.Parse(this.txtWeight.Text.Trim()) || weights==0)
                {
                    this.txtLong.ReadOnly = false;
                    this.txtWidth.ReadOnly = false;
                    this.txtHeight.ReadOnly = false;
                }
                else
                {
                    this.txtLong.Text = "1";
                    this.txtWidth.Text = "1";
                    this.txtHeight.Text = "1";

                    this.txtLong.ReadOnly = true;
                    this.txtWidth.ReadOnly = true;
                    this.txtHeight.ReadOnly = true;

                    AddPiece();
                }
            }
            else
            {
                //btnSubmit.Enabled = false;
                lblTsg.Text = "请输入数字或小数,小数位最多为3位";
            }
        }

        private void txtLong_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = @"^[1-9]\d*$";
                Regex reg = new Regex(s);
                if (!reg.IsMatch(this.txtLong.Text))
                {
                    btnSubmit.Enabled = false;
                    lblTsg.Text = "请输入正确数字";
                }
                else
                {
                    lblTsg.Text = "";
                    this.txtWidth.Focus();
                    btnSubmit.Enabled = true;
                }
            }
        }

        private void txtWidth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string s = @"^[1-9]\d*$";
                Regex reg = new Regex(s);
                if (!reg.IsMatch(this.txtWidth.Text))
                {
                    btnSubmit.Enabled = false;
                    lblTsg.Text = "请输入正确数字";
                }
                else
                {
                    lblTsg.Text = "";
                    this.txtHeight.Focus();
                    btnSubmit.Enabled = true;
                }
            }
        }

        private void txtWeight_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal weigth = string.IsNullOrWhiteSpace(this.txtWeight.Text) ? 0 : decimal.Parse(this.txtWeight.Text.Trim());
                decimal weigths = string.IsNullOrWhiteSpace(this.txtWeights.Text) ? 0 : decimal.Parse(this.txtWeights.Text.Trim());
                string temp = "1";
                if (weigths <= weigth && weigth>0)
                {
                    this.txtWidth.Text = temp;
                    this.txtLong.Text = temp;
                    this.txtHeight.Text = temp;
                    this.txtHeight.ReadOnly = true;
                    this.txtLong.ReadOnly = true;
                    this.txtWidth.ReadOnly = true;
                }
                else
                {
                    this.txtHeight.ReadOnly = false;
                    this.txtLong.ReadOnly = false;
                    this.txtWidth.ReadOnly = false;
                    //this.txtWidth.Text = this.txtLong.Text = this.txtHeight.Text = "";
                }
            }
            catch
            {
                MessageBox.Show("只能输入数字和小数点");
                txtWeight.Text = "";
            }
        }

        private void FrmInStorage_Load(object sender, EventArgs e)
        {
            FrmScaleSetting.OnWeightChange += OnWeightChange;
            FrmScaleSetting.OnScaleModleChange += (s =>
                {
                    txtWeights.Enabled = s != 1;
                });
            FrmScaleSetting.Initialize();

            txtWeights.Enabled = FrmScaleSetting.ScaleModle != 1;
            txtReceivingDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void OnWeightChange(string value)
        {
            if (!this.IsHandleCreated) return;
            this.Invoke(new Action(() =>
                {
                    if (!cbxWeight.Checked)
                    {
                        txtWeights.Text = value;

                        //判断是否为稳定输出模式
                        if (!string.IsNullOrWhiteSpace(txtNumber.Text) && FrmScaleSetting.WeightReaderService.OutModel == 1)
                        {
                            txtWeightsNext();
                        }
                    }
                }));
        }

        private void txtNumber_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text))return;

            if (checkHasPackageDetail()) return;

            if (CheckWayBill())
            {
                txtNumberLast = this.txtNumber.Text;
                PlaySuccessSound2();
            }
            else
            {
                PlayFailSound();
            }
        }

        private void cbbInStorage_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearLeft();

            var shippingMethodModelSelected = cbbInStorage.SelectedItem as ShippingMethodModel;

            if (shippingMethodModelSelected == null)
            {
                return;
            }

            if (!shippingMethodModelSelected.CalcVolumeweight)
            {
                _volumeLock = true;
                txtLong.Text = txtWidth.Text = txtHeight.Text = "1";
                txtLong.Enabled = txtWidth.Enabled = txtHeight.Enabled = txtWeight.Enabled = cbxVolume.Enabled = false;
            }
            else
            {
                _volumeLock = false;
                txtLong.Text = txtWidth.Text = txtHeight.Text = "";
                txtLong.Enabled = txtWidth.Enabled = txtHeight.Enabled = txtWeight.Enabled = cbxVolume.Enabled = true;
                txtLong.ReadOnly = txtWidth.ReadOnly = txtHeight.ReadOnly = txtWeight.ReadOnly  = false;
            }

            if (shippingMethodModelSelected.ShippingMethodTypeId==2)
            {
                txtPieces.Text = "";
                txtPieces.Enabled = cbxPieces.Enabled = true;
            }
            else
            {
                txtPieces.Text = "1";
                txtPieces.Enabled = cbxPieces.Enabled = false;
            }

            txtWeight_TextChanged(null, null);
        }

        private void FrmInStorage_FormClosing(object sender, FormClosingEventArgs e)
        {
            FrmScaleSetting.OnWeightChange -= OnWeightChange;
        }

        private void KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
            }
        }

        private void txtReceivingDate_Click(object sender, EventArgs e)
        {
            mcdReceivingDate.Visible = true;
        }

        private void mcdReceivingDate_DateChanged(object sender, DateRangeEventArgs e)
        {
            mcdReceivingDate.Visible = false;
            txtReceivingDate.Text = e.Start.Date.ToString("yyyy-MM-dd");
            txtNumber.Focus();
        }

        private void mcdReceivingDate_MouseLeave(object sender, EventArgs e)
        {
            mcdReceivingDate.Visible = false;
        }

        private void cbxBattery_CheckedChanged(object sender, EventArgs e)
        {
            cbbSensitiveType.Enabled = cbxBattery.Checked;
        }
    }
}
