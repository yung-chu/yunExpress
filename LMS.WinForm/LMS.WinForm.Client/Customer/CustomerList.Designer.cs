namespace LMS.WinForm.Client.Customer
{
    partial class CustomerList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblKeyword = new System.Windows.Forms.Label();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCleared = new System.Windows.Forms.Button();
            this.ddvCustomerList = new System.Windows.Forms.DataGridView();
            this.CustomerCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerTypeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PaymentTypeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.ddvCustomerList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblKeyword
            // 
            this.lblKeyword.AutoSize = true;
            this.lblKeyword.Location = new System.Drawing.Point(48, 30);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(53, 12);
            this.lblKeyword.TabIndex = 0;
            this.lblKeyword.Text = "关键字：";
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(107, 27);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(130, 21);
            this.txtKeyword.TabIndex = 1;
            this.txtKeyword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyDown);
            this.txtKeyword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtKeyword_KeyPress);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(253, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "搜 索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnCleared
            // 
            this.btnCleared.Location = new System.Drawing.Point(358, 25);
            this.btnCleared.Name = "btnCleared";
            this.btnCleared.Size = new System.Drawing.Size(75, 23);
            this.btnCleared.TabIndex = 3;
            this.btnCleared.Text = "清 空";
            this.btnCleared.UseVisualStyleBackColor = true;
            this.btnCleared.Click += new System.EventHandler(this.btnCleared_Click);
            // 
            // ddvCustomerList
            // 
            this.ddvCustomerList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ddvCustomerList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CustomerCode,
            this.ColumnName,
            this.CustomerTypeID,
            this.CustomerID,
            this.PaymentTypeID});
            this.ddvCustomerList.Location = new System.Drawing.Point(38, 68);
            this.ddvCustomerList.MultiSelect = false;
            this.ddvCustomerList.Name = "ddvCustomerList";
            this.ddvCustomerList.RowTemplate.Height = 23;
            this.ddvCustomerList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ddvCustomerList.Size = new System.Drawing.Size(443, 382);
            this.ddvCustomerList.TabIndex = 4;
            this.ddvCustomerList.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ddvCustomerList_CellDoubleClick);
            this.ddvCustomerList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ddvCustomerList_KeyDown);
            this.ddvCustomerList.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ddvCustomerList_KeyPress);
            // 
            // CustomerCode
            // 
            this.CustomerCode.DataPropertyName = "CustomerCode";
            this.CustomerCode.Frozen = true;
            this.CustomerCode.HeaderText = "客户编码";
            this.CustomerCode.Name = "CustomerCode";
            this.CustomerCode.ReadOnly = true;
            this.CustomerCode.Width = 150;
            // 
            // ColumnName
            // 
            this.ColumnName.DataPropertyName = "Name";
            this.ColumnName.HeaderText = "客户名称";
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.Width = 230;
            // 
            // CustomerTypeID
            // 
            this.CustomerTypeID.DataPropertyName = "CustomerTypeID";
            this.CustomerTypeID.HeaderText = "客户类型";
            this.CustomerTypeID.Name = "CustomerTypeID";
            this.CustomerTypeID.ReadOnly = true;
            this.CustomerTypeID.Visible = false;
            // 
            // CustomerID
            // 
            this.CustomerID.DataPropertyName = "CustomerID";
            this.CustomerID.HeaderText = "客户ID";
            this.CustomerID.Name = "CustomerID";
            this.CustomerID.Visible = false;
            // 
            // PaymentTypeID
            // 
            this.PaymentTypeID.DataPropertyName = "PaymentTypeID";
            this.PaymentTypeID.HeaderText = "结算类型";
            this.PaymentTypeID.Name = "PaymentTypeID";
            this.PaymentTypeID.Visible = false;
            // 
            // CustomerList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 478);
            this.Controls.Add(this.ddvCustomerList);
            this.Controls.Add(this.btnCleared);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtKeyword);
            this.Controls.Add(this.lblKeyword);
            this.MaximizeBox = false;
            this.Name = "CustomerList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "客户列表";
            ((System.ComponentModel.ISupportInitialize)(this.ddvCustomerList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblKeyword;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCleared;
        private System.Windows.Forms.DataGridView ddvCustomerList;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerTypeID;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerID;
        private System.Windows.Forms.DataGridViewTextBoxColumn PaymentTypeID;
    }
}