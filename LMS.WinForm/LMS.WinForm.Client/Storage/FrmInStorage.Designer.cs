namespace LMS.WinForm.Client.Storage
{
    partial class FrmInStorage
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
            this.lblCustomers = new System.Windows.Forms.Label();
            this.txtCustomers = new System.Windows.Forms.TextBox();
            this.btnCustomers = new System.Windows.Forms.Button();
            this.lblInStorage = new System.Windows.Forms.Label();
            this.cbbInStorage = new System.Windows.Forms.ComboBox();
            this.lblGoodsType = new System.Windows.Forms.Label();
            this.cbbGoodsType = new System.Windows.Forms.ComboBox();
            this.cbxPrintLable = new System.Windows.Forms.CheckBox();
            this.lblWeight = new System.Windows.Forms.Label();
            this.txtWeight = new System.Windows.Forms.TextBox();
            this.lblKG = new System.Windows.Forms.Label();
            this.btnInStorage = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbxPieces = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblCountyName = new System.Windows.Forms.Label();
            this.lblTsg = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblCountWeight = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvWaybillList = new System.Windows.Forms.DataGridView();
            this.WayBillNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TrackingNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CustomerOrderNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InShippingMethodName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Pieces = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettleWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CountryCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.操作 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtCustomersWaybillNumber = new System.Windows.Forms.TextBox();
            this.lblCustomersWaybillNumber = new System.Windows.Forms.Label();
            this.txtTrackingNumber = new System.Windows.Forms.TextBox();
            this.lblTrackingNumber = new System.Windows.Forms.Label();
            this.txtWaybillNumber = new System.Windows.Forms.TextBox();
            this.lblWaybillNumber = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblScanningCount = new System.Windows.Forms.Label();
            this.lblShipment = new System.Windows.Forms.Label();
            this.lblScanning = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.dgvPackageList = new System.Windows.Forms.DataGridView();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PackageDetailID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Weight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Width = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Height = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnDelete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.lblHeight = new System.Windows.Forms.Label();
            this.lblWidth = new System.Windows.Forms.Label();
            this.lblLong = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.cbxVolume = new System.Windows.Forms.CheckBox();
            this.txtLong = new System.Windows.Forms.TextBox();
            this.lblVolume = new System.Windows.Forms.Label();
            this.cbxWeight = new System.Windows.Forms.CheckBox();
            this.txtWeights = new System.Windows.Forms.TextBox();
            this.lblWeights = new System.Windows.Forms.Label();
            this.txtPieces = new System.Windows.Forms.TextBox();
            this.lblPieces = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.lblNumber = new System.Windows.Forms.Label();
            this.mcdReceivingDate = new System.Windows.Forms.MonthCalendar();
            this.cbxBattery = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPaymentName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtReceivingDate = new System.Windows.Forms.TextBox();
            this.cbbSensitiveType = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaybillList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPackageList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCustomers
            // 
            this.lblCustomers.AutoSize = true;
            this.lblCustomers.Location = new System.Drawing.Point(46, 23);
            this.lblCustomers.Name = "lblCustomers";
            this.lblCustomers.Size = new System.Drawing.Size(41, 12);
            this.lblCustomers.TabIndex = 0;
            this.lblCustomers.Text = "客户：";
            // 
            // txtCustomers
            // 
            this.txtCustomers.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCustomers.Location = new System.Drawing.Point(117, 19);
            this.txtCustomers.Name = "txtCustomers";
            this.txtCustomers.ReadOnly = true;
            this.txtCustomers.Size = new System.Drawing.Size(100, 21);
            this.txtCustomers.TabIndex = 1;
            // 
            // btnCustomers
            // 
            this.btnCustomers.Location = new System.Drawing.Point(233, 16);
            this.btnCustomers.Name = "btnCustomers";
            this.btnCustomers.Size = new System.Drawing.Size(75, 23);
            this.btnCustomers.TabIndex = 2;
            this.btnCustomers.Text = "选择";
            this.btnCustomers.UseVisualStyleBackColor = true;
            this.btnCustomers.Click += new System.EventHandler(this.btnCustomers_Click);
            // 
            // lblInStorage
            // 
            this.lblInStorage.AutoSize = true;
            this.lblInStorage.Location = new System.Drawing.Point(46, 69);
            this.lblInStorage.Name = "lblInStorage";
            this.lblInStorage.Size = new System.Drawing.Size(65, 12);
            this.lblInStorage.TabIndex = 3;
            this.lblInStorage.Text = "运输方式：";
            // 
            // cbbInStorage
            // 
            this.cbbInStorage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbInStorage.FormattingEnabled = true;
            this.cbbInStorage.Location = new System.Drawing.Point(117, 65);
            this.cbbInStorage.Name = "cbbInStorage";
            this.cbbInStorage.Size = new System.Drawing.Size(191, 20);
            this.cbbInStorage.TabIndex = 4;
            this.cbbInStorage.SelectedIndexChanged += new System.EventHandler(this.cbbInStorage_SelectedIndexChanged);
            // 
            // lblGoodsType
            // 
            this.lblGoodsType.AutoSize = true;
            this.lblGoodsType.Location = new System.Drawing.Point(372, 68);
            this.lblGoodsType.Name = "lblGoodsType";
            this.lblGoodsType.Size = new System.Drawing.Size(65, 12);
            this.lblGoodsType.TabIndex = 5;
            this.lblGoodsType.Text = "货物类型：";
            // 
            // cbbGoodsType
            // 
            this.cbbGoodsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbGoodsType.FormattingEnabled = true;
            this.cbbGoodsType.Location = new System.Drawing.Point(443, 65);
            this.cbbGoodsType.Name = "cbbGoodsType";
            this.cbbGoodsType.Size = new System.Drawing.Size(105, 20);
            this.cbbGoodsType.TabIndex = 6;
            // 
            // cbxPrintLable
            // 
            this.cbxPrintLable.AutoSize = true;
            this.cbxPrintLable.Location = new System.Drawing.Point(328, 15);
            this.cbxPrintLable.Name = "cbxPrintLable";
            this.cbxPrintLable.Size = new System.Drawing.Size(110, 18);
            this.cbxPrintLable.TabIndex = 7;
            this.cbxPrintLable.Text = "打印货物标签";
            this.cbxPrintLable.UseVisualStyleBackColor = true;
            this.cbxPrintLable.Visible = false;
            // 
            // lblWeight
            // 
            this.lblWeight.AutoSize = true;
            this.lblWeight.Location = new System.Drawing.Point(793, 69);
            this.lblWeight.Name = "lblWeight";
            this.lblWeight.Size = new System.Drawing.Size(113, 12);
            this.lblWeight.TabIndex = 8;
            this.lblWeight.Text = "重量范围内不计泡：";
            // 
            // txtWeight
            // 
            this.txtWeight.Location = new System.Drawing.Point(904, 60);
            this.txtWeight.Name = "txtWeight";
            this.txtWeight.Size = new System.Drawing.Size(58, 21);
            this.txtWeight.TabIndex = 9;
            this.txtWeight.Text = "0";
            this.txtWeight.TextChanged += new System.EventHandler(this.txtWeight_TextChanged);
            this.txtWeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // lblKG
            // 
            this.lblKG.AutoSize = true;
            this.lblKG.Location = new System.Drawing.Point(964, 65);
            this.lblKG.Name = "lblKG";
            this.lblKG.Size = new System.Drawing.Size(65, 12);
            this.lblKG.TabIndex = 10;
            this.lblKG.Text = "（kg包含）";
            // 
            // btnInStorage
            // 
            this.btnInStorage.Location = new System.Drawing.Point(1128, 22);
            this.btnInStorage.Name = "btnInStorage";
            this.btnInStorage.Size = new System.Drawing.Size(95, 46);
            this.btnInStorage.TabIndex = 11;
            this.btnInStorage.Text = "保存入仓";
            this.btnInStorage.UseVisualStyleBackColor = true;
            this.btnInStorage.Click += new System.EventHandler(this.btnInStorage_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cbxPieces);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.lblCountyName);
            this.panel1.Controls.Add(this.lblTsg);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.lblShipment);
            this.panel1.Controls.Add(this.lblScanning);
            this.panel1.Controls.Add(this.btnSubmit);
            this.panel1.Controls.Add(this.dgvPackageList);
            this.panel1.Controls.Add(this.lblHeight);
            this.panel1.Controls.Add(this.lblWidth);
            this.panel1.Controls.Add(this.lblLong);
            this.panel1.Controls.Add(this.txtHeight);
            this.panel1.Controls.Add(this.txtWidth);
            this.panel1.Controls.Add(this.cbxVolume);
            this.panel1.Controls.Add(this.txtLong);
            this.panel1.Controls.Add(this.lblVolume);
            this.panel1.Controls.Add(this.cbxWeight);
            this.panel1.Controls.Add(this.txtWeights);
            this.panel1.Controls.Add(this.lblWeights);
            this.panel1.Controls.Add(this.txtPieces);
            this.panel1.Controls.Add(this.lblPieces);
            this.panel1.Controls.Add(this.txtNumber);
            this.panel1.Controls.Add(this.lblNumber);
            this.panel1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel1.ForeColor = System.Drawing.Color.Black;
            this.panel1.Location = new System.Drawing.Point(25, 109);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1244, 523);
            this.panel1.TabIndex = 12;
            // 
            // cbxPieces
            // 
            this.cbxPieces.AutoSize = true;
            this.cbxPieces.Location = new System.Drawing.Point(292, 84);
            this.cbxPieces.Name = "cbxPieces";
            this.cbxPieces.Size = new System.Drawing.Size(82, 18);
            this.cbxPieces.TabIndex = 24;
            this.cbxPieces.Text = "固定件数";
            this.cbxPieces.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(259, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 14);
            this.label2.TabIndex = 23;
            this.label2.Text = "KG";
            // 
            // lblCountyName
            // 
            this.lblCountyName.AutoSize = true;
            this.lblCountyName.Location = new System.Drawing.Point(183, 483);
            this.lblCountyName.Name = "lblCountyName";
            this.lblCountyName.Size = new System.Drawing.Size(0, 14);
            this.lblCountyName.TabIndex = 22;
            // 
            // lblTsg
            // 
            this.lblTsg.AutoSize = true;
            this.lblTsg.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTsg.ForeColor = System.Drawing.Color.Red;
            this.lblTsg.Location = new System.Drawing.Point(211, 436);
            this.lblTsg.Name = "lblTsg";
            this.lblTsg.Size = new System.Drawing.Size(0, 21);
            this.lblTsg.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblCountWeight);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.dgvWaybillList);
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Controls.Add(this.txtCustomersWaybillNumber);
            this.panel2.Controls.Add(this.cbxPrintLable);
            this.panel2.Controls.Add(this.lblCustomersWaybillNumber);
            this.panel2.Controls.Add(this.txtTrackingNumber);
            this.panel2.Controls.Add(this.lblTrackingNumber);
            this.panel2.Controls.Add(this.txtWaybillNumber);
            this.panel2.Controls.Add(this.lblWaybillNumber);
            this.panel2.Controls.Add(this.lblCount);
            this.panel2.Controls.Add(this.lblScanningCount);
            this.panel2.Location = new System.Drawing.Point(498, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(746, 538);
            this.panel2.TabIndex = 20;
            // 
            // lblCountWeight
            // 
            this.lblCountWeight.AutoSize = true;
            this.lblCountWeight.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCountWeight.ForeColor = System.Drawing.Color.Red;
            this.lblCountWeight.Location = new System.Drawing.Point(256, 15);
            this.lblCountWeight.Name = "lblCountWeight";
            this.lblCountWeight.Size = new System.Drawing.Size(20, 22);
            this.lblCountWeight.TabIndex = 30;
            this.lblCountWeight.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(175, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 22);
            this.label1.TabIndex = 29;
            this.label1.Text = "总重量：";
            // 
            // dgvWaybillList
            // 
            this.dgvWaybillList.AllowUserToAddRows = false;
            this.dgvWaybillList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWaybillList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.WayBillNumber,
            this.TrackingNumber,
            this.CustomerOrderNumber,
            this.InShippingMethodName,
            this.Pieces,
            this.SettleWeight,
            this.CountryCode,
            this.操作});
            this.dgvWaybillList.Location = new System.Drawing.Point(16, 95);
            this.dgvWaybillList.Name = "dgvWaybillList";
            this.dgvWaybillList.RowTemplate.Height = 23;
            this.dgvWaybillList.Size = new System.Drawing.Size(714, 410);
            this.dgvWaybillList.TabIndex = 28;
            this.dgvWaybillList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvWaybillList_CellContentClick);
            // 
            // WayBillNumber
            // 
            this.WayBillNumber.DataPropertyName = "WayBillNumber";
            this.WayBillNumber.HeaderText = "运单号";
            this.WayBillNumber.Name = "WayBillNumber";
            this.WayBillNumber.ReadOnly = true;
            this.WayBillNumber.Width = 110;
            // 
            // TrackingNumber
            // 
            this.TrackingNumber.DataPropertyName = "TrackingNumber";
            this.TrackingNumber.HeaderText = "跟踪号";
            this.TrackingNumber.Name = "TrackingNumber";
            this.TrackingNumber.ReadOnly = true;
            this.TrackingNumber.Width = 110;
            // 
            // CustomerOrderNumber
            // 
            this.CustomerOrderNumber.DataPropertyName = "CustomerOrderNumber";
            this.CustomerOrderNumber.HeaderText = "客户订单号";
            this.CustomerOrderNumber.Name = "CustomerOrderNumber";
            this.CustomerOrderNumber.ReadOnly = true;
            this.CustomerOrderNumber.Width = 90;
            // 
            // InShippingMethodName
            // 
            this.InShippingMethodName.DataPropertyName = "InShippingMethodName";
            this.InShippingMethodName.HeaderText = "运输方式";
            this.InShippingMethodName.Name = "InShippingMethodName";
            this.InShippingMethodName.ReadOnly = true;
            this.InShippingMethodName.Width = 90;
            // 
            // Pieces
            // 
            this.Pieces.DataPropertyName = "Pieces";
            this.Pieces.HeaderText = "件数";
            this.Pieces.Name = "Pieces";
            this.Pieces.ReadOnly = true;
            this.Pieces.Width = 50;
            // 
            // SettleWeight
            // 
            this.SettleWeight.DataPropertyName = "SettleWeight";
            this.SettleWeight.HeaderText = "结算重量(KG)";
            this.SettleWeight.Name = "SettleWeight";
            this.SettleWeight.ReadOnly = true;
            // 
            // CountryCode
            // 
            this.CountryCode.DataPropertyName = "CountryCode";
            this.CountryCode.HeaderText = "目的地";
            this.CountryCode.Name = "CountryCode";
            this.CountryCode.ReadOnly = true;
            this.CountryCode.Width = 70;
            // 
            // 操作
            // 
            this.操作.DataPropertyName = "操作";
            this.操作.HeaderText = "操作";
            this.操作.Name = "操作";
            this.操作.ReadOnly = true;
            this.操作.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.操作.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.操作.Text = "删除";
            this.操作.UseColumnTextForButtonValue = true;
            this.操作.Width = 50;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(624, 63);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 27;
            this.btnSearch.Text = "筛选";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtCustomersWaybillNumber
            // 
            this.txtCustomersWaybillNumber.Location = new System.Drawing.Point(487, 64);
            this.txtCustomersWaybillNumber.Name = "txtCustomersWaybillNumber";
            this.txtCustomersWaybillNumber.Size = new System.Drawing.Size(129, 23);
            this.txtCustomersWaybillNumber.TabIndex = 26;
            // 
            // lblCustomersWaybillNumber
            // 
            this.lblCustomersWaybillNumber.AutoSize = true;
            this.lblCustomersWaybillNumber.Location = new System.Drawing.Point(405, 69);
            this.lblCustomersWaybillNumber.Name = "lblCustomersWaybillNumber";
            this.lblCustomersWaybillNumber.Size = new System.Drawing.Size(91, 14);
            this.lblCustomersWaybillNumber.TabIndex = 25;
            this.lblCustomersWaybillNumber.Text = "客户订单号：";
            // 
            // txtTrackingNumber
            // 
            this.txtTrackingNumber.Location = new System.Drawing.Point(264, 66);
            this.txtTrackingNumber.Name = "txtTrackingNumber";
            this.txtTrackingNumber.Size = new System.Drawing.Size(129, 23);
            this.txtTrackingNumber.TabIndex = 24;
            // 
            // lblTrackingNumber
            // 
            this.lblTrackingNumber.AutoSize = true;
            this.lblTrackingNumber.Location = new System.Drawing.Point(206, 69);
            this.lblTrackingNumber.Name = "lblTrackingNumber";
            this.lblTrackingNumber.Size = new System.Drawing.Size(63, 14);
            this.lblTrackingNumber.TabIndex = 23;
            this.lblTrackingNumber.Text = "跟踪号：";
            // 
            // txtWaybillNumber
            // 
            this.txtWaybillNumber.Location = new System.Drawing.Point(71, 64);
            this.txtWaybillNumber.Name = "txtWaybillNumber";
            this.txtWaybillNumber.Size = new System.Drawing.Size(129, 23);
            this.txtWaybillNumber.TabIndex = 22;
            // 
            // lblWaybillNumber
            // 
            this.lblWaybillNumber.AutoSize = true;
            this.lblWaybillNumber.Location = new System.Drawing.Point(13, 68);
            this.lblWaybillNumber.Name = "lblWaybillNumber";
            this.lblWaybillNumber.Size = new System.Drawing.Size(63, 14);
            this.lblWaybillNumber.TabIndex = 21;
            this.lblWaybillNumber.Text = "运单号：";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.BackColor = System.Drawing.SystemColors.Control;
            this.lblCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblCount.ForeColor = System.Drawing.Color.Red;
            this.lblCount.Location = new System.Drawing.Point(135, 14);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(20, 22);
            this.lblCount.TabIndex = 20;
            this.lblCount.Text = "0";
            // 
            // lblScanningCount
            // 
            this.lblScanningCount.AutoSize = true;
            this.lblScanningCount.BackColor = System.Drawing.SystemColors.Control;
            this.lblScanningCount.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblScanningCount.ForeColor = System.Drawing.Color.Red;
            this.lblScanningCount.Location = new System.Drawing.Point(23, 14);
            this.lblScanningCount.Name = "lblScanningCount";
            this.lblScanningCount.Size = new System.Drawing.Size(106, 22);
            this.lblScanningCount.TabIndex = 19;
            this.lblScanningCount.Text = "已扫描运单：";
            // 
            // lblShipment
            // 
            this.lblShipment.AutoSize = true;
            this.lblShipment.BackColor = System.Drawing.SystemColors.Control;
            this.lblShipment.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblShipment.ForeColor = System.Drawing.Color.Red;
            this.lblShipment.Location = new System.Drawing.Point(87, 478);
            this.lblShipment.Name = "lblShipment";
            this.lblShipment.Size = new System.Drawing.Size(90, 22);
            this.lblShipment.TabIndex = 18;
            this.lblShipment.Text = "发货国家：";
            // 
            // lblScanning
            // 
            this.lblScanning.AutoSize = true;
            this.lblScanning.BackColor = System.Drawing.SystemColors.Control;
            this.lblScanning.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblScanning.ForeColor = System.Drawing.Color.Red;
            this.lblScanning.Location = new System.Drawing.Point(86, 435);
            this.lblScanning.Name = "lblScanning";
            this.lblScanning.Size = new System.Drawing.Size(122, 22);
            this.lblScanning.TabIndex = 17;
            this.lblScanning.Text = "扫描信息提示：";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(233, 378);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(75, 34);
            this.btnSubmit.TabIndex = 16;
            this.btnSubmit.Text = "提  交";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // dgvPackageList
            // 
            this.dgvPackageList.AllowUserToAddRows = false;
            this.dgvPackageList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPackageList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.PackageDetailID,
            this.Weight,
            this.Length,
            this.Width,
            this.Height,
            this.btnDelete});
            this.dgvPackageList.Location = new System.Drawing.Point(85, 219);
            this.dgvPackageList.Name = "dgvPackageList";
            this.dgvPackageList.RowHeadersVisible = false;
            this.dgvPackageList.RowTemplate.Height = 23;
            this.dgvPackageList.Size = new System.Drawing.Size(371, 139);
            this.dgvPackageList.TabIndex = 15;
            this.dgvPackageList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPackageList_CellContentClick);
            this.dgvPackageList.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvPackageList_RowsAdded);
            this.dgvPackageList.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvPackageList_RowsRemoved);
            // 
            // No
            // 
            this.No.HeaderText = "序号";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            this.No.Width = 40;
            // 
            // PackageDetailID
            // 
            this.PackageDetailID.HeaderText = "包裹ID";
            this.PackageDetailID.Name = "PackageDetailID";
            this.PackageDetailID.Visible = false;
            // 
            // Weight
            // 
            this.Weight.HeaderText = "重量";
            this.Weight.Name = "Weight";
            this.Weight.ReadOnly = true;
            this.Weight.Width = 60;
            // 
            // Length
            // 
            this.Length.HeaderText = "长";
            this.Length.Name = "Length";
            this.Length.ReadOnly = true;
            this.Length.Width = 60;
            // 
            // Width
            // 
            this.Width.HeaderText = "宽";
            this.Width.Name = "Width";
            this.Width.ReadOnly = true;
            this.Width.Width = 60;
            // 
            // Height
            // 
            this.Height.HeaderText = "高";
            this.Height.Name = "Height";
            this.Height.ReadOnly = true;
            this.Height.Width = 60;
            // 
            // btnDelete
            // 
            this.btnDelete.HeaderText = "操作";
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.ReadOnly = true;
            this.btnDelete.Text = "删除";
            this.btnDelete.UseColumnTextForButtonValue = true;
            this.btnDelete.Width = 70;
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(344, 167);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(21, 14);
            this.lblHeight.TabIndex = 14;
            this.lblHeight.Text = "CM";
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(259, 167);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(21, 14);
            this.lblWidth.TabIndex = 13;
            this.lblWidth.Text = "CM";
            // 
            // lblLong
            // 
            this.lblLong.AutoSize = true;
            this.lblLong.Location = new System.Drawing.Point(183, 167);
            this.lblLong.Name = "lblLong";
            this.lblLong.Size = new System.Drawing.Size(21, 14);
            this.lblLong.TabIndex = 12;
            this.lblLong.Text = "CM";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(292, 164);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(48, 23);
            this.txtHeight.TabIndex = 11;
            this.txtHeight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHeight_KeyDown);
            this.txtHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(210, 164);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(48, 23);
            this.txtWidth.TabIndex = 10;
            this.txtWidth.TextChanged += new System.EventHandler(this.txtWidth_TextChanged);
            this.txtWidth.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtWidth_KeyDown);
            this.txtWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // cbxVolume
            // 
            this.cbxVolume.AutoSize = true;
            this.cbxVolume.Location = new System.Drawing.Point(379, 166);
            this.cbxVolume.Name = "cbxVolume";
            this.cbxVolume.Size = new System.Drawing.Size(82, 18);
            this.cbxVolume.TabIndex = 9;
            this.cbxVolume.Text = "固定体积";
            this.cbxVolume.UseVisualStyleBackColor = true;
            // 
            // txtLong
            // 
            this.txtLong.Location = new System.Drawing.Point(129, 164);
            this.txtLong.Name = "txtLong";
            this.txtLong.Size = new System.Drawing.Size(48, 23);
            this.txtLong.TabIndex = 8;
            this.txtLong.TextChanged += new System.EventHandler(this.txtLong_TextChanged);
            this.txtLong.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLong_KeyDown);
            this.txtLong.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(82, 167);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(49, 14);
            this.lblVolume.TabIndex = 7;
            this.lblVolume.Text = "体积：";
            // 
            // cbxWeight
            // 
            this.cbxWeight.AutoSize = true;
            this.cbxWeight.Location = new System.Drawing.Point(292, 128);
            this.cbxWeight.Name = "cbxWeight";
            this.cbxWeight.Size = new System.Drawing.Size(82, 18);
            this.cbxWeight.TabIndex = 6;
            this.cbxWeight.Text = "固定重量";
            this.cbxWeight.UseVisualStyleBackColor = true;
            // 
            // txtWeights
            // 
            this.txtWeights.Location = new System.Drawing.Point(130, 123);
            this.txtWeights.Name = "txtWeights";
            this.txtWeights.Size = new System.Drawing.Size(128, 23);
            this.txtWeights.TabIndex = 5;
            this.txtWeights.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtWeights_KeyDown);
            this.txtWeights.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // lblWeights
            // 
            this.lblWeights.AutoSize = true;
            this.lblWeights.Location = new System.Drawing.Point(82, 127);
            this.lblWeights.Name = "lblWeights";
            this.lblWeights.Size = new System.Drawing.Size(49, 14);
            this.lblWeights.TabIndex = 4;
            this.lblWeights.Text = "重量：";
            // 
            // txtPieces
            // 
            this.txtPieces.Location = new System.Drawing.Point(131, 82);
            this.txtPieces.Name = "txtPieces";
            this.txtPieces.Size = new System.Drawing.Size(128, 23);
            this.txtPieces.TabIndex = 3;
            this.txtPieces.TextChanged += new System.EventHandler(this.txtPieces_TextChanged);
            this.txtPieces.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPieces_KeyDown);
            this.txtPieces.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // lblPieces
            // 
            this.lblPieces.AutoSize = true;
            this.lblPieces.Location = new System.Drawing.Point(82, 85);
            this.lblPieces.Name = "lblPieces";
            this.lblPieces.Size = new System.Drawing.Size(49, 14);
            this.lblPieces.TabIndex = 2;
            this.lblPieces.Text = "件数：";
            this.lblPieces.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(131, 33);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(129, 23);
            this.txtNumber.TabIndex = 1;
            this.txtNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNumber_KeyDown);
            this.txtNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyPress);
            // 
            // lblNumber
            // 
            this.lblNumber.AutoSize = true;
            this.lblNumber.Location = new System.Drawing.Point(19, 36);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(112, 14);
            this.lblNumber.TabIndex = 0;
            this.lblNumber.Text = "订单号/运单号：";
            this.lblNumber.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // mcdReceivingDate
            // 
            this.mcdReceivingDate.Location = new System.Drawing.Point(903, 42);
            this.mcdReceivingDate.Name = "mcdReceivingDate";
            this.mcdReceivingDate.TabIndex = 18;
            this.mcdReceivingDate.Visible = false;
            this.mcdReceivingDate.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.mcdReceivingDate_DateChanged);
            this.mcdReceivingDate.MouseLeave += new System.EventHandler(this.mcdReceivingDate_MouseLeave);
            // 
            // cbxBattery
            // 
            this.cbxBattery.AutoSize = true;
            this.cbxBattery.Location = new System.Drawing.Point(562, 69);
            this.cbxBattery.Name = "cbxBattery";
            this.cbxBattery.Size = new System.Drawing.Size(72, 16);
            this.cbxBattery.TabIndex = 13;
            this.cbxBattery.Text = "是否带电";
            this.cbxBattery.UseVisualStyleBackColor = true;
            this.cbxBattery.CheckedChanged += new System.EventHandler(this.cbxBattery_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(371, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "结算方式：";
            // 
            // txtPaymentName
            // 
            this.txtPaymentName.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPaymentName.Location = new System.Drawing.Point(442, 18);
            this.txtPaymentName.Name = "txtPaymentName";
            this.txtPaymentName.ReadOnly = true;
            this.txtPaymentName.Size = new System.Drawing.Size(106, 21);
            this.txtPaymentName.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(793, 23);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "业务日期：";
            // 
            // txtReceivingDate
            // 
            this.txtReceivingDate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txtReceivingDate.Location = new System.Drawing.Point(903, 19);
            this.txtReceivingDate.Name = "txtReceivingDate";
            this.txtReceivingDate.Size = new System.Drawing.Size(126, 21);
            this.txtReceivingDate.TabIndex = 17;
            this.txtReceivingDate.Click += new System.EventHandler(this.txtReceivingDate_Click);
            // 
            // cbbSensitiveType
            // 
            this.cbbSensitiveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbSensitiveType.Enabled = false;
            this.cbbSensitiveType.FormattingEnabled = true;
            this.cbbSensitiveType.Location = new System.Drawing.Point(640, 67);
            this.cbbSensitiveType.Name = "cbbSensitiveType";
            this.cbbSensitiveType.Size = new System.Drawing.Size(84, 20);
            this.cbbSensitiveType.TabIndex = 18;
            // 
            // FrmInStorage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 644);
            this.Controls.Add(this.mcdReceivingDate);
            this.Controls.Add(this.cbbSensitiveType);
            this.Controls.Add(this.txtReceivingDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPaymentName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbxBattery);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnInStorage);
            this.Controls.Add(this.lblKG);
            this.Controls.Add(this.txtWeight);
            this.Controls.Add(this.lblWeight);
            this.Controls.Add(this.cbbGoodsType);
            this.Controls.Add(this.lblGoodsType);
            this.Controls.Add(this.cbbInStorage);
            this.Controls.Add(this.lblInStorage);
            this.Controls.Add(this.btnCustomers);
            this.Controls.Add(this.txtCustomers);
            this.Controls.Add(this.lblCustomers);
            this.Name = "FrmInStorage";
            this.Text = "速递包裹入仓";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmInStorage_FormClosing);
            this.Load += new System.EventHandler(this.FrmInStorage_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaybillList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPackageList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomers;
        private System.Windows.Forms.TextBox txtCustomers;
        private System.Windows.Forms.Button btnCustomers;
        private System.Windows.Forms.Label lblInStorage;
        private System.Windows.Forms.ComboBox cbbInStorage;
        private System.Windows.Forms.Label lblGoodsType;
        private System.Windows.Forms.ComboBox cbbGoodsType;
        private System.Windows.Forms.CheckBox cbxPrintLable;
        private System.Windows.Forms.Label lblWeight;
        private System.Windows.Forms.TextBox txtWeight;
        private System.Windows.Forms.Label lblKG;
        private System.Windows.Forms.Button btnInStorage;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvPackageList;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.Label lblLong;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.CheckBox cbxVolume;
        private System.Windows.Forms.TextBox txtLong;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.CheckBox cbxWeight;
        private System.Windows.Forms.TextBox txtWeights;
        private System.Windows.Forms.Label lblWeights;
        private System.Windows.Forms.TextBox txtPieces;
        private System.Windows.Forms.Label lblPieces;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvWaybillList;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtCustomersWaybillNumber;
        private System.Windows.Forms.Label lblCustomersWaybillNumber;
        private System.Windows.Forms.TextBox txtTrackingNumber;
        private System.Windows.Forms.Label lblTrackingNumber;
        private System.Windows.Forms.TextBox txtWaybillNumber;
        private System.Windows.Forms.Label lblWaybillNumber;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblScanningCount;
        private System.Windows.Forms.Label lblShipment;
        private System.Windows.Forms.Label lblScanning;
        private System.Windows.Forms.Label lblTsg;
        private System.Windows.Forms.Label lblCountyName;
        private System.Windows.Forms.Label lblCountWeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn WayBillNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrackingNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn CustomerOrderNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn InShippingMethodName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Pieces;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettleWeight;
        private System.Windows.Forms.DataGridViewTextBoxColumn CountryCode;
        private System.Windows.Forms.DataGridViewButtonColumn 操作;
        private System.Windows.Forms.CheckBox cbxPieces;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn PackageDetailID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Weight;
        private System.Windows.Forms.DataGridViewTextBoxColumn Length;
        private System.Windows.Forms.DataGridViewTextBoxColumn Width;
        private System.Windows.Forms.DataGridViewTextBoxColumn Height;
        private System.Windows.Forms.DataGridViewButtonColumn btnDelete;
        private System.Windows.Forms.CheckBox cbxBattery;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPaymentName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtReceivingDate;
        private System.Windows.Forms.MonthCalendar mcdReceivingDate;
        private System.Windows.Forms.ComboBox cbbSensitiveType;
    }
}