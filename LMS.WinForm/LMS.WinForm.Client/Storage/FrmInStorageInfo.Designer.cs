namespace LMS.WinForm.Client.Storage
{
    partial class FrmInStorageInfo
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
            this.lblInStorageCode = new System.Windows.Forms.Label();
            this.lblCustomers = new System.Windows.Forms.Label();
            this.lblReceiving = new System.Windows.Forms.Label();
            this.lblInStorageCodeText = new System.Windows.Forms.Label();
            this.lblCustomersText = new System.Windows.Forms.Label();
            this.lblReceivingText = new System.Windows.Forms.Label();
            this.lblSettlement = new System.Windows.Forms.Label();
            this.lblSettlementText = new System.Windows.Forms.Label();
            this.btnPrintSingle = new System.Windows.Forms.Button();
            this.btnInStorage = new System.Windows.Forms.Button();
            this.lblTypeF = new System.Windows.Forms.Label();
            this.lblFeeRMB = new System.Windows.Forms.Label();
            this.Count = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.PieceCount = new System.Windows.Forms.Label();
            this.lblPieceCount = new System.Windows.Forms.Label();
            this.CountWeight = new System.Windows.Forms.Label();
            this.lblCountWeight = new System.Windows.Forms.Label();
            this.dgvWaybillList = new System.Windows.Forms.DataGridView();
            this.lblCountPhysicalTotalWeight = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.InShippingMethodName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChineseNameAndCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Piece = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.货物重量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SettleWeight = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaybillList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInStorageCode
            // 
            this.lblInStorageCode.AutoSize = true;
            this.lblInStorageCode.Location = new System.Drawing.Point(42, 28);
            this.lblInStorageCode.Name = "lblInStorageCode";
            this.lblInStorageCode.Size = new System.Drawing.Size(65, 12);
            this.lblInStorageCode.TabIndex = 0;
            this.lblInStorageCode.Text = "入仓单号：";
            // 
            // lblCustomers
            // 
            this.lblCustomers.AutoSize = true;
            this.lblCustomers.Location = new System.Drawing.Point(256, 28);
            this.lblCustomers.Name = "lblCustomers";
            this.lblCustomers.Size = new System.Drawing.Size(41, 12);
            this.lblCustomers.TabIndex = 2;
            this.lblCustomers.Text = "客户：";
            // 
            // lblReceiving
            // 
            this.lblReceiving.AutoSize = true;
            this.lblReceiving.Location = new System.Drawing.Point(390, 27);
            this.lblReceiving.Name = "lblReceiving";
            this.lblReceiving.Size = new System.Drawing.Size(53, 12);
            this.lblReceiving.TabIndex = 4;
            this.lblReceiving.Text = "收货员：";
            // 
            // lblInStorageCodeText
            // 
            this.lblInStorageCodeText.AutoSize = true;
            this.lblInStorageCodeText.Location = new System.Drawing.Point(110, 28);
            this.lblInStorageCodeText.Name = "lblInStorageCodeText";
            this.lblInStorageCodeText.Size = new System.Drawing.Size(77, 12);
            this.lblInStorageCodeText.TabIndex = 5;
            this.lblInStorageCodeText.Text = "R20140419001";
            // 
            // lblCustomersText
            // 
            this.lblCustomersText.AutoSize = true;
            this.lblCustomersText.Location = new System.Drawing.Point(292, 27);
            this.lblCustomersText.Name = "lblCustomersText";
            this.lblCustomersText.Size = new System.Drawing.Size(29, 12);
            this.lblCustomersText.TabIndex = 6;
            this.lblCustomersText.Text = "张三";
            // 
            // lblReceivingText
            // 
            this.lblReceivingText.AutoSize = true;
            this.lblReceivingText.Location = new System.Drawing.Point(441, 27);
            this.lblReceivingText.Name = "lblReceivingText";
            this.lblReceivingText.Size = new System.Drawing.Size(29, 12);
            this.lblReceivingText.TabIndex = 7;
            this.lblReceivingText.Text = "小张";
            // 
            // lblSettlement
            // 
            this.lblSettlement.AutoSize = true;
            this.lblSettlement.Location = new System.Drawing.Point(508, 26);
            this.lblSettlement.Name = "lblSettlement";
            this.lblSettlement.Size = new System.Drawing.Size(65, 12);
            this.lblSettlement.TabIndex = 8;
            this.lblSettlement.Text = "结算方式：";
            // 
            // lblSettlementText
            // 
            this.lblSettlementText.AutoSize = true;
            this.lblSettlementText.Location = new System.Drawing.Point(570, 26);
            this.lblSettlementText.Name = "lblSettlementText";
            this.lblSettlementText.Size = new System.Drawing.Size(29, 12);
            this.lblSettlementText.TabIndex = 9;
            this.lblSettlementText.Text = "周结";
            // 
            // btnPrintSingle
            // 
            this.btnPrintSingle.Location = new System.Drawing.Point(638, 17);
            this.btnPrintSingle.Name = "btnPrintSingle";
            this.btnPrintSingle.Size = new System.Drawing.Size(85, 30);
            this.btnPrintSingle.TabIndex = 10;
            this.btnPrintSingle.Text = "打印交接单";
            this.btnPrintSingle.UseVisualStyleBackColor = true;
            this.btnPrintSingle.Click += new System.EventHandler(this.btnPrintSingle_Click);
            // 
            // btnInStorage
            // 
            this.btnInStorage.Location = new System.Drawing.Point(638, 80);
            this.btnInStorage.Name = "btnInStorage";
            this.btnInStorage.Size = new System.Drawing.Size(75, 30);
            this.btnInStorage.TabIndex = 11;
            this.btnInStorage.Text = "继续入仓";
            this.btnInStorage.UseVisualStyleBackColor = true;
            this.btnInStorage.Click += new System.EventHandler(this.btnInStorage_Click);
            // 
            // lblTypeF
            // 
            this.lblTypeF.AutoSize = true;
            this.lblTypeF.Location = new System.Drawing.Point(508, 89);
            this.lblTypeF.Name = "lblTypeF";
            this.lblTypeF.Size = new System.Drawing.Size(65, 12);
            this.lblTypeF.TabIndex = 14;
            this.lblTypeF.Text = "结算币别：";
            // 
            // lblFeeRMB
            // 
            this.lblFeeRMB.AutoSize = true;
            this.lblFeeRMB.Location = new System.Drawing.Point(568, 89);
            this.lblFeeRMB.Name = "lblFeeRMB";
            this.lblFeeRMB.Size = new System.Drawing.Size(41, 12);
            this.lblFeeRMB.TabIndex = 15;
            this.lblFeeRMB.Text = "人民币";
            // 
            // Count
            // 
            this.Count.AutoSize = true;
            this.Count.Location = new System.Drawing.Point(55, 89);
            this.Count.Name = "Count";
            this.Count.Size = new System.Drawing.Size(53, 12);
            this.Count.TabIndex = 26;
            this.Count.Text = "总票数：";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(108, 89);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(11, 12);
            this.lblCount.TabIndex = 27;
            this.lblCount.Text = "0";
            // 
            // PieceCount
            // 
            this.PieceCount.AutoSize = true;
            this.PieceCount.Location = new System.Drawing.Point(162, 89);
            this.PieceCount.Name = "PieceCount";
            this.PieceCount.Size = new System.Drawing.Size(53, 12);
            this.PieceCount.TabIndex = 28;
            this.PieceCount.Text = "总件数：";
            // 
            // lblPieceCount
            // 
            this.lblPieceCount.AutoSize = true;
            this.lblPieceCount.Location = new System.Drawing.Point(210, 89);
            this.lblPieceCount.Name = "lblPieceCount";
            this.lblPieceCount.Size = new System.Drawing.Size(11, 12);
            this.lblPieceCount.TabIndex = 29;
            this.lblPieceCount.Text = "0";
            // 
            // CountWeight
            // 
            this.CountWeight.AutoSize = true;
            this.CountWeight.Location = new System.Drawing.Point(380, 89);
            this.CountWeight.Name = "CountWeight";
            this.CountWeight.Size = new System.Drawing.Size(77, 12);
            this.CountWeight.TabIndex = 30;
            this.CountWeight.Text = "结算总重量：";
            // 
            // lblCountWeight
            // 
            this.lblCountWeight.AutoSize = true;
            this.lblCountWeight.Location = new System.Drawing.Point(454, 89);
            this.lblCountWeight.Name = "lblCountWeight";
            this.lblCountWeight.Size = new System.Drawing.Size(11, 12);
            this.lblCountWeight.TabIndex = 31;
            this.lblCountWeight.Text = "0";
            // 
            // dgvWaybillList
            // 
            this.dgvWaybillList.AllowUserToAddRows = false;
            this.dgvWaybillList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWaybillList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.InShippingMethodName,
            this.ChineseNameAndCode,
            this.Piece,
            this.货物重量,
            this.SettleWeight});
            this.dgvWaybillList.Location = new System.Drawing.Point(44, 147);
            this.dgvWaybillList.Name = "dgvWaybillList";
            this.dgvWaybillList.RowTemplate.Height = 23;
            this.dgvWaybillList.Size = new System.Drawing.Size(620, 287);
            this.dgvWaybillList.TabIndex = 32;
            // 
            // lblCountPhysicalTotalWeight
            // 
            this.lblCountPhysicalTotalWeight.AutoSize = true;
            this.lblCountPhysicalTotalWeight.Location = new System.Drawing.Point(330, 89);
            this.lblCountPhysicalTotalWeight.Name = "lblCountPhysicalTotalWeight";
            this.lblCountPhysicalTotalWeight.Size = new System.Drawing.Size(11, 12);
            this.lblCountPhysicalTotalWeight.TabIndex = 34;
            this.lblCountPhysicalTotalWeight.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(256, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 33;
            this.label2.Text = "货物总重量：";
            // 
            // InShippingMethodName
            // 
            this.InShippingMethodName.DataPropertyName = "InShippingMethodName";
            this.InShippingMethodName.HeaderText = "运输方式";
            this.InShippingMethodName.Name = "InShippingMethodName";
            this.InShippingMethodName.ReadOnly = true;
            this.InShippingMethodName.Width = 120;
            // 
            // ChineseNameAndCode
            // 
            this.ChineseNameAndCode.DataPropertyName = "ChineseNameAndCode";
            this.ChineseNameAndCode.HeaderText = "目的地国家";
            this.ChineseNameAndCode.Name = "ChineseNameAndCode";
            this.ChineseNameAndCode.ReadOnly = true;
            this.ChineseNameAndCode.Width = 120;
            // 
            // Piece
            // 
            this.Piece.DataPropertyName = "Pieces";
            this.Piece.HeaderText = "件数";
            this.Piece.Name = "Piece";
            this.Piece.ReadOnly = true;
            // 
            // 货物重量
            // 
            this.货物重量.DataPropertyName = "Weight";
            this.货物重量.HeaderText = "货物重量(KG)";
            this.货物重量.Name = "货物重量";
            // 
            // SettleWeight
            // 
            this.SettleWeight.DataPropertyName = "SettleWeight";
            this.SettleWeight.HeaderText = "结算重量(KG)";
            this.SettleWeight.Name = "SettleWeight";
            this.SettleWeight.ReadOnly = true;
            this.SettleWeight.Width = 120;
            // 
            // FrmInStorageInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 465);
            this.Controls.Add(this.lblCountPhysicalTotalWeight);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvWaybillList);
            this.Controls.Add(this.lblCountWeight);
            this.Controls.Add(this.CountWeight);
            this.Controls.Add(this.lblPieceCount);
            this.Controls.Add(this.PieceCount);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.Count);
            this.Controls.Add(this.lblFeeRMB);
            this.Controls.Add(this.lblTypeF);
            this.Controls.Add(this.btnInStorage);
            this.Controls.Add(this.btnPrintSingle);
            this.Controls.Add(this.lblSettlementText);
            this.Controls.Add(this.lblSettlement);
            this.Controls.Add(this.lblReceivingText);
            this.Controls.Add(this.lblCustomersText);
            this.Controls.Add(this.lblInStorageCodeText);
            this.Controls.Add(this.lblReceiving);
            this.Controls.Add(this.lblCustomers);
            this.Controls.Add(this.lblInStorageCode);
            this.Name = "FrmInStorageInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "入仓单信息";
            this.Load += new System.EventHandler(this.FrmInStorageInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWaybillList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInStorageCode;
        private System.Windows.Forms.Label lblCustomers;
        private System.Windows.Forms.Label lblReceiving;
        private System.Windows.Forms.Label lblInStorageCodeText;
        private System.Windows.Forms.Label lblCustomersText;
        private System.Windows.Forms.Label lblReceivingText;
        private System.Windows.Forms.Label lblSettlement;
        private System.Windows.Forms.Label lblSettlementText;
        private System.Windows.Forms.Button btnPrintSingle;
        private System.Windows.Forms.Button btnInStorage;
        private System.Windows.Forms.Label lblTypeF;
        private System.Windows.Forms.Label lblFeeRMB;
        private System.Windows.Forms.Label Count;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label PieceCount;
        private System.Windows.Forms.Label lblPieceCount;
        private System.Windows.Forms.Label CountWeight;
        private System.Windows.Forms.Label lblCountWeight;
        private System.Windows.Forms.DataGridView dgvWaybillList;
        private System.Windows.Forms.Label lblCountPhysicalTotalWeight;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn InShippingMethodName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ChineseNameAndCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Piece;
        private System.Windows.Forms.DataGridViewTextBoxColumn 货物重量;
        private System.Windows.Forms.DataGridViewTextBoxColumn SettleWeight;
    }
}