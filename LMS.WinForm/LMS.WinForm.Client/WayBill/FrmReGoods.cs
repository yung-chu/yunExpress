using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using LMS.Core;
using LMS.Data.Entity.ExtModel;
using LMS.WinForm.Client.Common;
using LMS.WinForm.Client.Models;
using LighTake.Infrastructure.Common.Logging;

namespace LMS.WinForm.Client.WayBill
{
    public partial class FrmReGoods : Form
    {
        private List<ReturnGoodsExt> listModel = new List<ReturnGoodsExt>();
        private InFeeTotalInfoExtModel entity = null;

        int i = 1;
        public FrmReGoods()
        {
            InitializeComponent();
            plWeight.Visible = cbxWeight.Checked;


        }

        private void cbxWeight_CheckedChanged(object sender, EventArgs e)
        {
            plWeight.Visible = cbxWeight.Checked;
        }

        private void rbtnInStorage_CheckedChanged(object sender, EventArgs e)
        {
            plFree.Visible = !rbtnInStorage.Checked;
        }

        private void txtWeight_TextChanged(object sender, EventArgs e)
        {
            string s = @"^\d{1,7}(?:\.\d{0,2}$|$)";
            Regex reg = new Regex(s);

            if (!reg.IsMatch(this.txtWeight.Text))
            {
                btnSubmit.Enabled = false;
            }
            else
            {
                lblTip.Text = "";
                btnSubmit.Enabled = true;
            }
        }




        /// <summary>
        /// 内部原因选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnInternal_CheckedChanged(object sender, EventArgs e)
        {
            var list = new List<SelectItem>();
            if (rbtnInternal.Checked)
            {
                list.Add(new SelectItem() { Value = 0, Text = "请选择" });
                list.Add(new SelectItem() { Value = 1, Text = "操作错误" });
                list.Add(new SelectItem() { Value = 2, Text = "其它原因" });
            }
            BindComboBox(list);
        }

        private void rbtnExternal_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtnExternal.Checked)
            {
                BindComboBox(new List<SelectItem>());
            }
        }
        public void BindComboBox(List<SelectItem> list)
        {
            if (list.Count == 0)
            {
                list.Add(new SelectItem() { Value = 0, Text = "请选择" });
                list.Add(new SelectItem() { Value = 1, Text = "海关未过" });
                list.Add(new SelectItem() { Value = 2, Text = "无人签收" });
                list.Add(new SelectItem() { Value = 3, Text = "客户要求" });
                list.Add(new SelectItem() { Value = 4, Text = "其它原因" });
                list.Add(new SelectItem() { Value = 5, Text = "海外退货" });
            }
            cbbReGoodsReason.DataSource = list;
            cbbReGoodsReason.ValueMember = "Value";
            cbbReGoodsReason.DisplayMember = "Text";
        }





        /// <summary>
        /// 单元格内容单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsANonHeaderButtonCell(e)) return;
            if (dgvList.Rows[e.RowIndex].Cells[1].Value == null) return;
            string wayBillNumber = dgvList.Rows[e.RowIndex].Cells[1].Value.ToString();
            dgvList.Rows.RemoveAt(e.RowIndex);
            listModel.Remove(listModel.Find(p => p.WayBillNumber == wayBillNumber));
            btnSureReGoods.Enabled = true;
            lblCount.Text = this.dgvList.Rows.Count.ToString();
        }

        private bool IsANonHeaderButtonCell(DataGridViewCellEventArgs cellEvent)
        {
            if (dgvList.Columns[cellEvent.ColumnIndex] is
                DataGridViewButtonColumn &&
                cellEvent.RowIndex != -1)
            {
                return true;
            }
            else { return false; }
        }

        public void PopulateDataGridView(ReturnGoodsExt model)
        {
            if (model == null)
            {
                return;
            }
            int index = this.dgvList.Rows.Add();
            this.dgvList.Rows[index].Cells[0].Value = i;
            this.dgvList.Rows[index].Cells[1].Value = model.WayBillNumber;
            this.dgvList.Rows[index].Cells[2].Value = model.CustomerOrderNumber;
            this.dgvList.Rows[index].Cells[3].Value = model.CustomerName;
            this.dgvList.Rows[index].Cells[4].Value = model.TotalFee;
            this.dgvList.Rows[index].Cells[5].Value = model.PackageNumber;
            this.dgvList.Rows[index].Cells[6].Value = model.WayBillWeight;
            this.dgvList.Rows[index].Cells[7].Value = model.Weight;
            this.dgvList.Rows[index].Cells[8].Value = model.ShippingMethodName;
            this.dgvList.Rows[index].Cells[9].Value = model.CountryName;
            this.dgvList.Rows[index].Cells[10].Value = "取消";
            //运单原重量与退货重量相差100g 用颜色标识该行
            if (cbxWeight.Checked)
            {
                if ((model.WayBillWeight * 1000) - (model.Weight * 1000) >= 100 ||
                    (model.Weight * 1000) - (model.WayBillWeight * 1000) >= 100)
                {
                    this.dgvList.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
                }
            }
            i++;
            //foreach (DataGridViewColumn c in dgvList.Columns)
            //{
            //    c.SortMode = DataGridViewColumnSortMode.Automatic;
            //}

           // DataGridViewColumn sortColumn = dgvList.CurrentCell.OwningColumn;

            //设定排序的方向（升序、降序）
           // ListSortDirection sortDirection = ListSortDirection.Descending;
            //if (dgvList.SortedColumn != null &&
            //     dgvList.SortedColumn.Equals(sortColumn))
            //{
            //    sortDirection =
            //        dgvList.SortOrder == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Ascending;
            //       // ListSortDirection.Descending : ListSortDirection.Ascending;
            //}

            //进行排序
            dgvList.Columns[0].Visible = false;
            //this.dgvList.Rows[index].Cells[0].Visible = false;
            dgvList.Sort(dgvList.Columns[0], System.ComponentModel.ListSortDirection.Descending);
          //  dgvList.Sort(sortColumn, sortDirection);


        }


        private void btnSubmit_Click(object sender, EventArgs e)
        {
            VerificationData();
        }

        /// <summary>
        /// 验证用户输入的数据
        /// </summary>
        public void VerificationData()
        {
            if (entity == null)
            {
                lblTip.Text = "请按下回车键,查询该单是否存在";
                return;
            }



            if (cbxWeight.Checked)
            {
                string s = @"^\d{1,7}(?:\.\d{0,2}$|$)";
                Regex reg = new Regex(s);
                if (!reg.IsMatch(this.txtWeight.Text))
                {
                    btnSubmit.Enabled = false;
                    lblTip.Text = "请输入数字或小数,小数位最多为2位";
                }
                else
                {
                    lblTip.Text = "";
                    btnSubmit.Enabled = true;
                }
            }

            ReturnGoodsExt model = new ReturnGoodsExt()
            {
                UserName = LoginHelper.GetUserName(),
                WayBillNumber = entity.WayBillNumber,
                CustomerName = entity.CustomerName,
                CustomerID = entity.CustomerID,
                CustomerCode = entity.CustomerCode,
                CountryName = entity.ChineseName,
                CustomerOrderNumber = entity.CustomerOrderNumber,
                //Freight = entity.Freight,
                //FuelCharge = entity.FuelCharge,
                InStorageID = entity.InStorageID,
                PackageNumber = entity.PackageNumber,
                TotalFee = entity.TotalFee,
                //Surcharge = entity.Surcharge,
                //Register = entity.Register,
                TariffPrepayFee = entity.TariffPrepayFee,
                ShippingMethodName = entity.ShippingMethodName,
                WayBillWeight = entity.SettleWeight
            };

            //
            if (cbxWeight.Checked)
            {
                decimal weight;
                Decimal.TryParse(txtWeight.Text.Trim(), out weight);
                model.Weight = weight;
            }
            else
            {
                model.Weight = entity.Weight;
            }
            
            model.IsDirectReturnGoods = rbtnDirectReGoods.Checked;

            if (rbtnInternal.Checked || rbtnExternal.Checked)
            {
                model.Type = rbtnInternal.Checked ? 1 : 2;
            }
            else
            {
                lblTip.Text = "请选择退货类型";
                return;
            }
            if (cbbReGoodsReason.Text == "请选择")
            {
                lblTip.Text = "请选择退货原因";
                return;
            }

            model.Reason = cbbReGoodsReason.Text.Trim();
            model.ReasonRemark = txtRemark.Text.Trim();
            if (rbtnDirectReGoods.Checked)
            {
                if (rbtnReturnTrue.Checked || rbtnReturnFalse.Checked)
                {
                    model.IsReturnShipping = rbtnReturnTrue.Checked;
                }
                else
                {
                    lblTip.Text = "请选择是否退运费";
                    return;
                }
            }

            ////lblTip.Text = "";
            //if (listModel.Exists(p => p.WayBillNumber.Equals(model.WayBillNumber)))
            //{
            //    this.txtNumber.Focus();
            //    this.txtNumber.Text = string.Empty;
            //    this.txtWeight.Text = string.Empty;
            //    lblTip.Text = model.WayBillNumber + " 数据已经存在，不能重复添加";
            //    return;
            //}
            lblTip.Text = entity.CustomerName + " 扫描通过";
            listModel.Add(model);
            PopulateDataGridView(model);
            btnSubmit.Enabled = false;
            btnSureReGoods.Enabled = true;
            this.txtNumber.Focus();
            this.txtNumber.Text = string.Empty;
            this.txtWeight.Text = string.Empty;
            lblCount.Text = dgvList.Rows.Count.ToString();
        }

        private void btnSureReGoods_Click(object sender, EventArgs e)
        {
            if (listModel.Count <= 0)
            {
                MessageBox.Show("没有可用的数据!");
                return;
            }
            ResponseResultModel responseResult = InvokeWebApiHelper.PostReturnGoodsResult(listModel);
            if (responseResult.IsSuccess)
            {
                listModel.Clear();
                dgvList.Rows.Clear();
                MessageBox.Show(responseResult.Message);
                txtNumber.Text = string.Empty;
                txtWeight.Text = string.Empty;
                txtNumber.Focus();
                this.txtNumber.Text = string.Empty;
                this.txtWeight.Text = null;
                this.lblCount.Text = this.dgvList.Rows.Count.ToString();
            }
            else
            {
                btnSureReGoods.Enabled = false;
                MessageBox.Show(responseResult.Message);
            }
            i = 1;

        }

        /// <summary>
        /// 播放成功音
        /// </summary>
        private void PlaySuccessSound()
        {
            try
            {
                string soundSuccessPath = Application.StartupPath + @"\Resource\Success.wav";
                if (!File.Exists(soundSuccessPath))
                {
                    MessageBox.Show("成功提示音文件不存在:" + soundSuccessPath);
                    return;
                }
                SoundPlayer simpleSound = new SoundPlayer(soundSuccessPath);
                simpleSound.Play();
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
                string soundSuccessPath = Application.StartupPath + @"\Resource\Faile.wav";
                if (!File.Exists(soundSuccessPath))
                {
                    MessageBox.Show("失败提示音文件不存在:" + soundSuccessPath);
                    return;
                }
                SoundPlayer simpleSound = new SoundPlayer(soundSuccessPath);
                simpleSound.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (string.IsNullOrWhiteSpace(txtNumber.Text.Trim()))
                {
                    lblTip.Text = "请输入单号";
                    txtNumber.Focus();
                    PlayFailSound();
                    return;
                }
                entity = InvokeWebApiHelper.GetInFeeTotalInfo(txtNumber.Text.Trim());

                if (entity != null)
                {
                    if (entity.WayBillStatus ==
                        LMS.Data.Entity.WayBill.StatusToValue(LMS.Data.Entity.WayBill.StatusEnum.WaitOrder) ||
                        entity.WayBillStatus ==
                        LMS.Data.Entity.WayBill.StatusToValue(LMS.Data.Entity.WayBill.StatusEnum.Send))
                    {

                        if (listModel.Exists(p => p.WayBillNumber.Equals(entity.WayBillNumber)))
                        {
                            this.txtNumber.Focus();
                            this.txtNumber.Text = string.Empty;
                            this.txtWeight.Text = string.Empty;
                            lblTip.Text = entity.WayBillNumber + " 数据已经存在，不能重复添加";
                            PlayFailSound();
                            return;
                        }

                        lblTip.Text = entity.CustomerName + " 扫描通过";
                        btnSubmit.Enabled = true;

                        PlaySuccessSound();
                    }
                    else
                    {
                        lblTip.Text = txtNumber.Text + " 该单不是已发货和待转单的运单！";
                        btnSubmit.Enabled = false;
                        this.txtNumber.Focus();
                        this.txtNumber.Text = string.Empty;
                        this.txtWeight.Text = string.Empty;
                        PlayFailSound();
                        return;
                    }
                }
                else
                {
                    lblTip.Text = txtNumber.Text + " 该单不存在或已删除！";
                    btnSubmit.Enabled = false;
                    this.txtNumber.Focus();
                    this.txtNumber.Text = string.Empty;
                    this.txtWeight.Text = string.Empty;
                    PlayFailSound();
                    return;

                }

                if (cbxWeight.Checked)
                {
                    txtWeight.Focus();
                }
                else
                {
                    VerificationData();
                }

                lblCount.Text = dgvList.Rows.Count.ToString();
            }
        }


        private void txtWeight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                VerificationData();
            }
        }

        private void dgvList_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "序号" || e.Column.Name == "运单号")
            {
                e.SortResult = (Convert.ToDouble(e.CellValue1) - Convert.ToDouble(e.CellValue2) > 0) ? 1 : (Convert.ToDouble(e.CellValue1) - Convert.ToDouble(e.CellValue2) < 0) ? -1 : 0;
            }
        }

    }


    public class SelectItem
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }



}
