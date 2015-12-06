namespace LMS.WinForm.Client.Storage
{
    partial class FrmOutStorage
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
            this.lblServiceProvider = new System.Windows.Forms.Label();
            this.cbbServiceProvider = new System.Windows.Forms.ComboBox();
            this.cbbShippingMethods = new System.Windows.Forms.ComboBox();
            this.lblShippingMethods = new System.Windows.Forms.Label();
            this.cbbGoodsType = new System.Windows.Forms.ComboBox();
            this.lblGoodsType = new System.Windows.Forms.Label();
            this.rbtnCheckTrackingNumber = new System.Windows.Forms.RadioButton();
            this.rbtnRecordTrackingNumber = new System.Windows.Forms.RadioButton();
            this.rbtnByOrderId = new System.Windows.Forms.RadioButton();
            this.btnSubmitOutStorage = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblShipment = new System.Windows.Forms.Label();
            this.lblScanning = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblScanningCount = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtCustomersWaybillNumber = new System.Windows.Forms.TextBox();
            this.lblCustomersWaybillNumber = new System.Windows.Forms.Label();
            this.txtTrackingNumber = new System.Windows.Forms.TextBox();
            this.lblTrackingNumber = new System.Windows.Forms.Label();
            this.txtWaybillNumber = new System.Windows.Forms.TextBox();
            this.lblWaybillNumber = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.WayBillNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackingNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShippingMethodName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Operate = new System.Windows.Forms.DataGridViewButtonColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblServiceProvider
            // 
            this.lblServiceProvider.AutoSize = true;
            this.lblServiceProvider.Location = new System.Drawing.Point(44, 26);
            this.lblServiceProvider.Name = "lblServiceProvider";
            this.lblServiceProvider.Size = new System.Drawing.Size(53, 12);
            this.lblServiceProvider.TabIndex = 0;
            this.lblServiceProvider.Text = "服务商：";
            // 
            // cbbServiceProvider
            // 
            this.cbbServiceProvider.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbbServiceProvider.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbbServiceProvider.FormattingEnabled = true;
            this.cbbServiceProvider.Location = new System.Drawing.Point(93, 22);
            this.cbbServiceProvider.Name = "cbbServiceProvider";
            this.cbbServiceProvider.Size = new System.Drawing.Size(100, 20);
            this.cbbServiceProvider.TabIndex = 1;
            // 
            // cbbShippingMethods
            // 
            this.cbbShippingMethods.FormattingEnabled = true;
            this.cbbShippingMethods.Location = new System.Drawing.Point(294, 22);
            this.cbbShippingMethods.Name = "cbbShippingMethods";
            this.cbbShippingMethods.Size = new System.Drawing.Size(100, 20);
            this.cbbShippingMethods.TabIndex = 3;
            // 
            // lblShippingMethods
            // 
            this.lblShippingMethods.AutoSize = true;
            this.lblShippingMethods.Location = new System.Drawing.Point(232, 26);
            this.lblShippingMethods.Name = "lblShippingMethods";
            this.lblShippingMethods.Size = new System.Drawing.Size(65, 12);
            this.lblShippingMethods.TabIndex = 2;
            this.lblShippingMethods.Text = "运输方式：";
            // 
            // cbbGoodsType
            // 
            this.cbbGoodsType.FormattingEnabled = true;
            this.cbbGoodsType.Location = new System.Drawing.Point(500, 22);
            this.cbbGoodsType.Name = "cbbGoodsType";
            this.cbbGoodsType.Size = new System.Drawing.Size(100, 20);
            this.cbbGoodsType.TabIndex = 5;
            // 
            // lblGoodsType
            // 
            this.lblGoodsType.AutoSize = true;
            this.lblGoodsType.Location = new System.Drawing.Point(438, 26);
            this.lblGoodsType.Name = "lblGoodsType";
            this.lblGoodsType.Size = new System.Drawing.Size(65, 12);
            this.lblGoodsType.TabIndex = 4;
            this.lblGoodsType.Text = "货物类型：";
            // 
            // rbtnCheckTrackingNumber
            // 
            this.rbtnCheckTrackingNumber.AutoSize = true;
            this.rbtnCheckTrackingNumber.Location = new System.Drawing.Point(640, 25);
            this.rbtnCheckTrackingNumber.Name = "rbtnCheckTrackingNumber";
            this.rbtnCheckTrackingNumber.Size = new System.Drawing.Size(83, 16);
            this.rbtnCheckTrackingNumber.TabIndex = 6;
            this.rbtnCheckTrackingNumber.TabStop = true;
            this.rbtnCheckTrackingNumber.Text = "检查跟踪号";
            this.rbtnCheckTrackingNumber.UseVisualStyleBackColor = true;
            // 
            // rbtnRecordTrackingNumber
            // 
            this.rbtnRecordTrackingNumber.AutoSize = true;
            this.rbtnRecordTrackingNumber.Location = new System.Drawing.Point(744, 26);
            this.rbtnRecordTrackingNumber.Name = "rbtnRecordTrackingNumber";
            this.rbtnRecordTrackingNumber.Size = new System.Drawing.Size(83, 16);
            this.rbtnRecordTrackingNumber.TabIndex = 7;
            this.rbtnRecordTrackingNumber.TabStop = true;
            this.rbtnRecordTrackingNumber.Text = "录入跟踪号";
            this.rbtnRecordTrackingNumber.UseVisualStyleBackColor = true;
            // 
            // rbtnByOrderId
            // 
            this.rbtnByOrderId.AutoSize = true;
            this.rbtnByOrderId.Location = new System.Drawing.Point(842, 26);
            this.rbtnByOrderId.Name = "rbtnByOrderId";
            this.rbtnByOrderId.Size = new System.Drawing.Size(71, 16);
            this.rbtnByOrderId.TabIndex = 8;
            this.rbtnByOrderId.TabStop = true;
            this.rbtnByOrderId.Text = "按订单号";
            this.rbtnByOrderId.UseVisualStyleBackColor = true;
            // 
            // btnSubmitOutStorage
            // 
            this.btnSubmitOutStorage.Location = new System.Drawing.Point(935, 16);
            this.btnSubmitOutStorage.Name = "btnSubmitOutStorage";
            this.btnSubmitOutStorage.Size = new System.Drawing.Size(75, 30);
            this.btnSubmitOutStorage.TabIndex = 9;
            this.btnSubmitOutStorage.Text = "提交出仓";
            this.btnSubmitOutStorage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblShipment);
            this.panel1.Controls.Add(this.lblScanning);
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(25, 63);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 515);
            this.panel1.TabIndex = 10;
            // 
            // lblShipment
            // 
            this.lblShipment.AutoSize = true;
            this.lblShipment.BackColor = System.Drawing.SystemColors.Control;
            this.lblShipment.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblShipment.ForeColor = System.Drawing.Color.Red;
            this.lblShipment.Location = new System.Drawing.Point(19, 302);
            this.lblShipment.Name = "lblShipment";
            this.lblShipment.Size = new System.Drawing.Size(90, 22);
            this.lblShipment.TabIndex = 20;
            this.lblShipment.Text = "发货国家：";
            // 
            // lblScanning
            // 
            this.lblScanning.AutoSize = true;
            this.lblScanning.BackColor = System.Drawing.SystemColors.Control;
            this.lblScanning.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblScanning.ForeColor = System.Drawing.Color.Red;
            this.lblScanning.Location = new System.Drawing.Point(18, 259);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(122, 22);
            this.lblScanning.TabIndex = 19;
            this.lblScanning.Text = "扫描信息提示：";
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox2.Location = new System.Drawing.Point(121, 95);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(160, 21);
            this.textBox2.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "跟踪号：";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.Location = new System.Drawing.Point(121, 27);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(160, 21);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "订单号/运单号：";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.BackColor = System.Drawing.SystemColors.Control;
            this.lblCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.ForeColor = System.Drawing.Color.Red;
            this.lblCount.Location = new System.Drawing.Point(520, 72);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(20, 22);
            this.lblCount.TabIndex = 22;
            this.lblCount.Text = "0";
            // 
            // lblScanningCount
            // 
            this.lblScanningCount.AutoSize = true;
            this.lblScanningCount.BackColor = System.Drawing.SystemColors.Control;
            this.lblScanningCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblScanningCount.ForeColor = System.Drawing.Color.Red;
            this.lblScanningCount.Location = new System.Drawing.Point(408, 72);
            this.lblScanningCount.Name = "lblScanningCount";
            this.lblScanningCount.Size = new System.Drawing.Size(106, 22);
            this.lblScanningCount.TabIndex = 21;
            this.lblScanningCount.Text = "已扫描运单：";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(954, 120);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 34;
            this.btnSearch.Text = "筛选";
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtCustomersWaybillNumber
            // 
            this.txtCustomersWaybillNumber.Location = new System.Drawing.Point(817, 121);
            this.txtCustomersWaybillNumber.Name = "txtCustomersWaybillNumber";
            this.txtCustomersWaybillNumber.Size = new System.Drawing.Size(129, 21);
            this.txtCustomersWaybillNumber.TabIndex = 33;
            // 
            // lblCustomersWaybillNumber
            // 
            this.lblCustomersWaybillNumber.AutoSize = true;
            this.lblCustomersWaybillNumber.Location = new System.Drawing.Point(735, 126);
            this.lblCustomersWaybillNumber.Name = "lblCustomersWaybillNumber";
            this.lblCustomersWaybillNumber.Size = new System.Drawing.Size(77, 12);
            this.lblCustomersWaybillNumber.TabIndex = 32;
            this.lblCustomersWaybillNumber.Text = "客户订单号：";
            // 
            // txtTrackingNumber
            // 
            this.txtTrackingNumber.Location = new System.Drawing.Point(594, 123);
            this.txtTrackingNumber.Name = "txtTrackingNumber";
            this.txtTrackingNumber.Size = new System.Drawing.Size(129, 21);
            this.txtTrackingNumber.TabIndex = 31;
            // 
            // lblTrackingNumber
            // 
            this.lblTrackingNumber.AutoSize = true;
            this.lblTrackingNumber.Location = new System.Drawing.Point(536, 126);
            this.lblTrackingNumber.Name = "lblTrackingNumber";
            this.lblTrackingNumber.Size = new System.Drawing.Size(53, 12);
            this.lblTrackingNumber.TabIndex = 30;
            this.lblTrackingNumber.Text = "跟踪号：";
            // 
            // txtWaybillNumber
            // 
            this.txtWaybillNumber.Location = new System.Drawing.Point(401, 121);
            this.txtWaybillNumber.Name = "txtWaybillNumber";
            this.txtWaybillNumber.Size = new System.Drawing.Size(129, 21);
            this.txtWaybillNumber.TabIndex = 29;
            // 
            // lblWaybillNumber
            // 
            this.lblWaybillNumber.AutoSize = true;
            this.lblWaybillNumber.Location = new System.Drawing.Point(343, 125);
            this.lblWaybillNumber.Name = "lblWaybillNumber";
            this.lblWaybillNumber.Size = new System.Drawing.Size(53, 12);
            this.lblWaybillNumber.TabIndex = 28;
            this.lblWaybillNumber.Text = "运单号：";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WayBillNumber,
            this.TrackingNumber,
            this.ShippingMethodName,
            this.CountryName,
            this.Operate});
            this.dataGridView1.Location = new System.Drawing.Point(345, 159);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(684, 419);
            this.dataGridView1.TabIndex = 35;
            // 
            // WayBillNumber
            // 
            this.WayBillNumber.HeaderText = "运单号";
            this.WayBillNumber.Name = "WayBillNumber";
            this.WayBillNumber.ReadOnly = true;
            // 
            // TrackingNumber
            // 
            this.TrackingNumber.HeaderText = "跟踪号";
            this.TrackingNumber.Name = "TrackingNumber";
            this.TrackingNumber.ReadOnly = true;
            // 
            // ShippingMethodName
            // 
            this.ShippingMethodName.HeaderText = "运输方式";
            this.ShippingMethodName.Name = "ShippingMethodName";
            this.ShippingMethodName.ReadOnly = true;
            // 
            // CountryName
            // 
            this.CountryName.HeaderText = "目的国家";
            this.CountryName.Name = "CountryName";
            this.CountryName.ReadOnly = true;
            // 
            // Operate
            // 
            this.Operate.HeaderText = "操作";
            this.Operate.Name = "Operate";
            this.Operate.ReadOnly = true;
            this.Operate.Text = "取消";
            // 
            // FrmOutStorage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 612);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtCustomersWaybillNumber);
            this.Controls.Add(this.lblCustomersWaybillNumber);
            this.Controls.Add(this.txtTrackingNumber);
            this.Controls.Add(this.lblTrackingNumber);
            this.Controls.Add(this.txtWaybillNumber);
            this.Controls.Add(this.lblWaybillNumber);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lblScanningCount);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSubmitOutStorage);
            this.Controls.Add(this.rbtnByOrderId);
            this.Controls.Add(this.rbtnRecordTrackingNumber);
            this.Controls.Add(this.rbtnCheckTrackingNumber);
            this.Controls.Add(this.cbbGoodsType);
            this.Controls.Add(this.lblGoodsType);
            this.Controls.Add(this.cbbShippingMethods);
            this.Controls.Add(this.lblShippingMethods);
            this.Controls.Add(this.cbbServiceProvider);
            this.Controls.Add(this.lblServiceProvider);
            this.Name = "FrmOutStorage";
            this.Text = "速递包裹出仓";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServiceProvider;
        private System.Windows.Forms.ComboBox cbbServiceProvider;
        private System.Windows.Forms.ComboBox cbbShippingMethods;
        private System.Windows.Forms.Label lblShippingMethods;
        private System.Windows.Forms.ComboBox cbbGoodsType;
        private System.Windows.Forms.Label lblGoodsType;
        private System.Windows.Forms.RadioButton rbtnCheckTrackingNumber;
        private System.Windows.Forms.RadioButton rbtnRecordTrackingNumber;
        private System.Windows.Forms.RadioButton rbtnByOrderId;
        private System.Windows.Forms.Button btnSubmitOutStorage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblShipment;
        private System.Windows.Forms.Label lblScanning;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblScanningCount;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtCustomersWaybillNumber;
        private System.Windows.Forms.Label lblCustomersWaybillNumber;
        private System.Windows.Forms.TextBox txtTrackingNumber;
        private System.Windows.Forms.Label lblTrackingNumber;
        private System.Windows.Forms.TextBox txtWaybillNumber;
        private System.Windows.Forms.Label lblWaybillNumber;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn WayBillNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrackingNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ShippingMethodName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountryName;
        private System.Windows.Forms.DataGridViewButtonColumn Operate;
    }
}