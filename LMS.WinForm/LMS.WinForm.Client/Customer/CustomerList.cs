using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LMS.WinForm.Client.Common;
using LMS.WinForm.Client.Storage;

namespace LMS.WinForm.Client.Customer
{
    public partial class CustomerList : Form
    {
        public delegate void GetTextHandler(string text);
        public GetTextHandler getTextHandler;//委托对象
        public CustomerList()
        {
            InitializeComponent();
            BindCustomerList();
        }

        private void BindCustomerList()
        {
            new BackgroundLoading(this, Bind, "正在获取客户列表...", "正在获取客户列表...").Show();
        }

        private void Bind()
        {
            string keyWord = this.txtKeyword.Text.Trim();

            var action = new Action(() =>
                {
                    this.ddvCustomerList.DataSource = InvokeWebApiHelper.GetCustomerListCS(keyWord);
                    this.ddvCustomerList.AutoGenerateColumns = true;
                    this.ddvCustomerList.ClearSelection();
                });

            if (this.IsHandleCreated && this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            BindCustomerList();
        }

        private void ddvCustomerList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                string tempValue = string.Empty;
                string name = ddvCustomerList.Rows[e.RowIndex].Cells["CustomerCode"].Value.ToString();
                string customerTypeID = ddvCustomerList.Rows[e.RowIndex].Cells["CustomerTypeID"].Value.ToString();
                string CustomerID = ddvCustomerList.Rows[e.RowIndex].Cells["CustomerID"].Value.ToString();
                string PaymentTypeID = (ddvCustomerList.Rows[e.RowIndex].Cells["PaymentTypeID"].Value??"").ToString();
                tempValue = name + "|" + customerTypeID + "|" + CustomerID + "|" + PaymentTypeID;

                this.Close();

                if (getTextHandler != null)
                {
                    getTextHandler( tempValue.Trim());
                }
            }
        }

        private void btnCleared_Click(object sender, EventArgs e)
        {
            getTextHandler(string.Empty);
            this.Close();
        }

        private void txtKeyword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnSearch_Click(null, null);
                e.Handled = true;

            }
        }


        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                if (ddvCustomerList.Rows.Count > 0)
                    ddvCustomerList.Rows[0].Selected = true;

                ddvCustomerList.Focus();

                e.Handled = true;
            }
        }

        private void ddvCustomerList_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void ddvCustomerList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (ddvCustomerList.SelectedCells.Count > 0)
                {
                    var dataGridViewCellEventArgs = new DataGridViewCellEventArgs(0, ddvCustomerList.SelectedCells[0].RowIndex);
                    ddvCustomerList_CellDoubleClick(ddvCustomerList, dataGridViewCellEventArgs);
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (ddvCustomerList.SelectedCells.Count > 0)
                {
                    var cell = ddvCustomerList.SelectedCells[0];
                    if (cell.RowIndex+1 < ddvCustomerList.Rows.Count)
                    {
                        ddvCustomerList.Rows[cell.RowIndex + 1].Selected = true;
                    }
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (ddvCustomerList.SelectedCells.Count > 0)
                {
                    var cell = ddvCustomerList.SelectedCells[0];
                    if (cell.RowIndex>0)
                    {
                        ddvCustomerList.Rows[cell.RowIndex -1].Selected = true;
                    }
                }
                e.Handled = true;
            }
        }

    }
}
