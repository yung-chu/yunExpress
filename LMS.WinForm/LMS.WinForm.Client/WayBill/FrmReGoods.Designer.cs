namespace LMS.WinForm.Client.WayBill
{
    partial class FrmReGoods
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnInStorage = new System.Windows.Forms.RadioButton();
            this.rbtnDirectReGoods = new System.Windows.Forms.RadioButton();
            this.cbxWeight = new System.Windows.Forms.CheckBox();
            this.btnSureReGoods = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.plWeight = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbbReGoodsReason = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.rbtnReturnTrue = new System.Windows.Forms.RadioButton();
            this.rbtnReturnFalse = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.dgvList = new System.Windows.Forms.DataGridView();
            this.plFree = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbtnExternal = new System.Windows.Forms.RadioButton();
            this.rbtnInternal = new System.Windows.Forms.RadioButton();
            this.lblTip = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WayBillNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Amount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.运单原重量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.退货重量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.运输方式 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.发货国家 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.操作 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.plWeight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
            this.plFree.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "退货动作：";
            // 
            // rbtnInStorage
            // 
            this.rbtnInStorage.AutoSize = true;
            this.rbtnInStorage.Location = new System.Drawing.Point(195, 23);
            this.rbtnInStorage.Name = "rbtnInStorage";
            this.rbtnInStorage.Size = new System.Drawing.Size(71, 16);
            this.rbtnInStorage.TabIndex = 20;
            this.rbtnInStorage.Text = "退货入仓";
            this.rbtnInStorage.UseVisualStyleBackColor = true;
            this.rbtnInStorage.Visible = false;
            this.rbtnInStorage.CheckedChanged += new System.EventHandler(this.rbtnInStorage_CheckedChanged);
            // 
            // rbtnDirectReGoods
            // 
            this.rbtnDirectReGoods.AutoSize = true;
            this.rbtnDirectReGoods.Checked = true;
            this.rbtnDirectReGoods.Location = new System.Drawing.Point(106, 23);
            this.rbtnDirectReGoods.Name = "rbtnDirectReGoods";
            this.rbtnDirectReGoods.Size = new System.Drawing.Size(71, 16);
            this.rbtnDirectReGoods.TabIndex = 20;
            this.rbtnDirectReGoods.TabStop = true;
            this.rbtnDirectReGoods.Text = "直接退货";
            this.rbtnDirectReGoods.UseVisualStyleBackColor = true;
            // 
            // cbxWeight
            // 
            this.cbxWeight.AutoSize = true;
            this.cbxWeight.Location = new System.Drawing.Point(326, 22);
            this.cbxWeight.Name = "cbxWeight";
            this.cbxWeight.Size = new System.Drawing.Size(72, 16);
            this.cbxWeight.TabIndex = 10;
            this.cbxWeight.Text = "是否称重";
            this.cbxWeight.UseVisualStyleBackColor = true;
            this.cbxWeight.CheckedChanged += new System.EventHandler(this.cbxWeight_CheckedChanged);
            // 
            // btnSureReGoods
            // 
            this.btnSureReGoods.Location = new System.Drawing.Point(426, 15);
            this.btnSureReGoods.Name = "btnSureReGoods";
            this.btnSureReGoods.Size = new System.Drawing.Size(75, 23);
            this.btnSureReGoods.TabIndex = 6;
            this.btnSureReGoods.Text = "确认退货";
            this.btnSureReGoods.UseVisualStyleBackColor = true;
            this.btnSureReGoods.Click += new System.EventHandler(this.btnSureReGoods_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(51, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "单 号：";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(100, 67);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(132, 21);
            this.txtNumber.TabIndex = 1;
            this.txtNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            // 
            // plWeight
            // 
            this.plWeight.Controls.Add(this.label4);
            this.plWeight.Controls.Add(this.label3);
            this.plWeight.Controls.Add(this.txtWeight);
            this.plWeight.Location = new System.Drawing.Point(30, 94);
            this.plWeight.Name = "plWeight";
            this.plWeight.Size = new System.Drawing.Size(286, 42);
            this.plWeight.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(210, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "kg";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "退货重量：";
            // 
            // txtWeight
            // 
            this.txtWeight.Location = new System.Drawing.Point(70, 11);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(132, 21);
            this.txtWeight.TabIndex = 2;
            this.txtWeight.TextChanged += new System.EventHandler(this.txtWeight_TextChanged);
            this.txtWeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtWeight_KeyDown);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "退货原因：";
            // 
            // cbbReGoodsReason
            // 
            this.cbbReGoodsReason.FormattingEnabled = true;
            this.cbbReGoodsReason.Location = new System.Drawing.Point(215, 144);
            this.cbbReGoodsReason.Name = "cbbReGoodsReason";
            this.cbbReGoodsReason.Size = new System.Drawing.Size(101, 20);
            this.cbbReGoodsReason.TabIndex = 3;
            this.cbbReGoodsReason.Text = "请选择";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(32, 203);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "原因备注：";
            // 
            // txtRemark
            // 
            this.txtRemark.Location = new System.Drawing.Point(99, 175);
            this.txtRemark.Multiline = true;
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(218, 68);
            this.txtRemark.TabIndex = 4;
            // 
            // rbtnReturnTrue
            // 
            this.rbtnReturnTrue.AutoSize = true;
            this.rbtnReturnTrue.Location = new System.Drawing.Point(82, 7);
            this.rbtnReturnTrue.Name = "rbtnReturnTrue";
            this.rbtnReturnTrue.Size = new System.Drawing.Size(35, 16);
            this.rbtnReturnTrue.TabIndex = 1;
            this.rbtnReturnTrue.TabStop = true;
            this.rbtnReturnTrue.Text = "是";
            this.rbtnReturnTrue.UseVisualStyleBackColor = true;
            // 
            // rbtnReturnFalse
            // 
            this.rbtnReturnFalse.AutoSize = true;
            this.rbtnReturnFalse.Location = new System.Drawing.Point(126, 7);
            this.rbtnReturnFalse.Name = "rbtnReturnFalse";
            this.rbtnReturnFalse.Size = new System.Drawing.Size(35, 16);
            this.rbtnReturnFalse.TabIndex = 1;
            this.rbtnReturnFalse.TabStop = true;
            this.rbtnReturnFalse.Text = "否";
            this.rbtnReturnFalse.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(-1, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 7;
            this.label7.Text = "是否退运费：";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(106, 334);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 23);
            this.btnSubmit.TabIndex = 5;
            this.btnSubmit.Text = "提  交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // dgvList
            // 
            this.dgvList.AllowUserToAddRows = false;
            this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.WayBillNumber,
            this.OrderNumber,
            this.CustomerName,
            this.Amount,
            this.Number,
            this.运单原重量,
            this.退货重量,
            this.运输方式,
            this.发货国家,
            this.操作});
            this.dgvList.Location = new System.Drawing.Point(326, 54);
            this.dgvList.Name = "dgvList";
            this.dgvList.RowTemplate.Height = 23;
            this.dgvList.Size = new System.Drawing.Size(919, 446);
            this.dgvList.TabIndex = 11;
            this.dgvList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvList_CellContentClick);
            this.dgvList.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgvList_SortCompare);
            // 
            // plFree
            // 
            this.plFree.Controls.Add(this.rbtnReturnFalse);
            this.plFree.Controls.Add(this.rbtnReturnTrue);
            this.plFree.Controls.Add(this.label7);
            this.plFree.Location = new System.Drawing.Point(30, 260);
            this.plFree.Name = "plFree";
            this.plFree.Size = new System.Drawing.Size(185, 28);
            this.plFree.TabIndex = 12;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtnExternal);
            this.panel2.Controls.Add(this.rbtnInternal);
            this.panel2.Location = new System.Drawing.Point(99, 140);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(106, 28);
            this.panel2.TabIndex = 12;
            // 
            // rbtnExternal
            // 
            this.rbtnExternal.AutoSize = true;
            this.rbtnExternal.Location = new System.Drawing.Point(52, 7);
            this.rbtnExternal.Name = "rbtnExternal";
            this.rbtnExternal.Size = new System.Drawing.Size(47, 16);
            this.rbtnExternal.TabIndex = 19;
            this.rbtnExternal.Text = "外部";
            this.rbtnExternal.UseVisualStyleBackColor = true;
            this.rbtnExternal.CheckedChanged += new System.EventHandler(this.rbtnExternal_CheckedChanged);
            // 
            // rbtnInternal
            // 
            this.rbtnInternal.AutoSize = true;
            this.rbtnInternal.Location = new System.Drawing.Point(3, 7);
            this.rbtnInternal.Name = "rbtnInternal";
            this.rbtnInternal.Size = new System.Drawing.Size(47, 16);
            this.rbtnInternal.TabIndex = 19;
            this.rbtnInternal.Text = "内部";
            this.rbtnInternal.UseVisualStyleBackColor = true;
            this.rbtnInternal.CheckedChanged += new System.EventHandler(this.rbtnInternal_CheckedChanged);
            // 
            // lblTip
            // 
            this.lblTip.AutoSize = true;
            this.lblTip.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTip.ForeColor = System.Drawing.Color.Red;
            this.lblTip.Location = new System.Drawing.Point(97, 309);
            this.lblTip.Name = "lblTip";
            this.lblTip.Size = new System.Drawing.Size(0, 19);
            this.lblTip.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(534, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 19);
            this.label8.TabIndex = 22;
            this.label8.Text = "总件数：";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.ForeColor = System.Drawing.Color.Red;
            this.lblCount.Location = new System.Drawing.Point(611, 20);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(20, 19);
            this.lblCount.TabIndex = 23;
            this.lblCount.Text = "0";
            // 
            // No
            // 
            this.No.Frozen = true;
            this.No.HeaderText = "序号";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.Width = 5;
            // 
            // WayBillNumber
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.WayBillNumber.DefaultCellStyle = dataGridViewCellStyle1;
            this.WayBillNumber.Frozen = true;
            this.WayBillNumber.HeaderText = "运单号";
            this.WayBillNumber.Name = "WayBillNumber";
            this.WayBillNumber.ReadOnly = true;
            // 
            // OrderNumber
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.OrderNumber.DefaultCellStyle = dataGridViewCellStyle2;
            this.OrderNumber.Frozen = true;
            this.OrderNumber.HeaderText = "客户订单号";
            this.OrderNumber.Name = "OrderNumber";
            this.OrderNumber.ReadOnly = true;
            // 
            // CustomerName
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.CustomerName.DefaultCellStyle = dataGridViewCellStyle3;
            this.CustomerName.Frozen = true;
            this.CustomerName.HeaderText = "客户名称";
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.ReadOnly = true;
            // 
            // Amount
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Amount.DefaultCellStyle = dataGridViewCellStyle4;
            this.Amount.HeaderText = "运单金额";
            this.Amount.Name = "Amount";
            this.Amount.ReadOnly = true;
            this.Amount.Width = 80;
            // 
            // Number
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Number.DefaultCellStyle = dataGridViewCellStyle5;
            this.Number.HeaderText = "件数";
            this.Number.Name = "Number";
            this.Number.ReadOnly = true;
            this.Number.Width = 60;
            // 
            // 运单原重量
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.运单原重量.DefaultCellStyle = dataGridViewCellStyle6;
            this.运单原重量.HeaderText = "运单原重量";
            this.运单原重量.Name = "运单原重量";
            this.运单原重量.ReadOnly = true;
            this.运单原重量.Width = 90;
            // 
            // 退货重量
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.退货重量.DefaultCellStyle = dataGridViewCellStyle7;
            this.退货重量.HeaderText = "退货重量";
            this.退货重量.Name = "退货重量";
            this.退货重量.ReadOnly = true;
            this.退货重量.Width = 80;
            // 
            // 运输方式
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.运输方式.DefaultCellStyle = dataGridViewCellStyle8;
            this.运输方式.HeaderText = "运输方式";
            this.运输方式.Name = "运输方式";
            this.运输方式.ReadOnly = true;
            // 
            // 发货国家
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.发货国家.DefaultCellStyle = dataGridViewCellStyle9;
            this.发货国家.HeaderText = "发货国家";
            this.发货国家.Name = "发货国家";
            this.发货国家.ReadOnly = true;
            this.发货国家.Width = 80;
            // 
            // 操作
            // 
            this.操作.HeaderText = "操作";
            this.操作.Name = "操作";
            this.操作.Text = "";
            this.操作.Width = 80;
            // 
            // FrmReGoods
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 525);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblTip);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.plFree);
            this.Controls.Add(this.dgvList);
            this.Controls.Add(this.txtRemark);
            this.Controls.Add(this.cbbReGoodsReason);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.plWeight);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnSureReGoods);
            this.Controls.Add(this.cbxWeight);
            this.Controls.Add(this.rbtnDirectReGoods);
            this.Controls.Add(this.rbtnInStorage);
            this.Controls.Add(this.label1);
            this.Name = "FrmReGoods";
            this.Text = "退货入仓";
            this.plWeight.ResumeLayout(false);
            this.plWeight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
            this.plFree.ResumeLayout(false);
            this.plFree.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbtnInStorage;
        private System.Windows.Forms.RadioButton rbtnDirectReGoods;
        private System.Windows.Forms.CheckBox cbxWeight;
        private System.Windows.Forms.Button btnSureReGoods;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Panel plWeight;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWeight;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbbReGoodsReason;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.RadioButton rbtnReturnTrue;
        private System.Windows.Forms.RadioButton rbtnReturnFalse;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.DataGridView dgvList;
        private System.Windows.Forms.Panel plFree;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbtnExternal;
        private System.Windows.Forms.RadioButton rbtnInternal;
        private System.Windows.Forms.Label lblTip;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn WayBillNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Amount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn 运单原重量;
        private System.Windows.Forms.DataGridViewTextBoxColumn 退货重量;
        private System.Windows.Forms.DataGridViewTextBoxColumn 运输方式;
        private System.Windows.Forms.DataGridViewTextBoxColumn 发货国家;
        private System.Windows.Forms.DataGridViewButtonColumn 操作;
    }
}