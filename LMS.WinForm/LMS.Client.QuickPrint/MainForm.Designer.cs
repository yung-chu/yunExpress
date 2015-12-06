namespace LMS.Client.QuickPrint
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSetting = new System.Windows.Forms.Button();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lswPrintHistory = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkAutoPrint = new System.Windows.Forms.CheckBox();
            this.btnShowPrintPreview = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbFirstPrinter = new System.Windows.Forms.ComboBox();
            this.cbbSecondPrinter = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSetting
            // 
            this.btnSetting.Location = new System.Drawing.Point(139, 342);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(96, 32);
            this.btnSetting.TabIndex = 6;
            this.btnSetting.Text = "打印机设置";
            this.btnSetting.UseVisualStyleBackColor = true;
            this.btnSetting.Visible = false;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // webBrowser1
            // 
            this.webBrowser1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowser1.Location = new System.Drawing.Point(6, 20);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(553, 59);
            this.webBrowser1.TabIndex = 4;
            this.webBrowser1.Url = new System.Uri("", System.UriKind.Relative);
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser1_Navigated);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lswPrintHistory);
            this.groupBox1.Location = new System.Drawing.Point(12, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(559, 181);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "打印记录";
            // 
            // lswPrintHistory
            // 
            this.lswPrintHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.Status});
            this.lswPrintHistory.FullRowSelect = true;
            this.lswPrintHistory.GridLines = true;
            this.lswPrintHistory.Location = new System.Drawing.Point(6, 20);
            this.lswPrintHistory.Name = "lswPrintHistory";
            this.lswPrintHistory.Size = new System.Drawing.Size(547, 155);
            this.lswPrintHistory.TabIndex = 0;
            this.lswPrintHistory.UseCompatibleStateImageBehavior = false;
            this.lswPrintHistory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "时间";
            this.columnHeader1.Width = 77;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "网址";
            this.columnHeader2.Width = 336;
            // 
            // Status
            // 
            this.Status.Text = "状态";
            this.Status.Width = 103;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.webBrowser1);
            this.groupBox2.Location = new System.Drawing.Point(12, 420);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(565, 85);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "预览区";
            // 
            // chkAutoPrint
            // 
            this.chkAutoPrint.AutoSize = true;
            this.chkAutoPrint.Checked = true;
            this.chkAutoPrint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoPrint.Location = new System.Drawing.Point(16, 233);
            this.chkAutoPrint.Name = "chkAutoPrint";
            this.chkAutoPrint.Size = new System.Drawing.Size(72, 16);
            this.chkAutoPrint.TabIndex = 9;
            this.chkAutoPrint.Text = "自动打印";
            this.chkAutoPrint.UseVisualStyleBackColor = true;
            // 
            // btnShowPrintPreview
            // 
            this.btnShowPrintPreview.Location = new System.Drawing.Point(30, 342);
            this.btnShowPrintPreview.Name = "btnShowPrintPreview";
            this.btnShowPrintPreview.Size = new System.Drawing.Size(96, 32);
            this.btnShowPrintPreview.TabIndex = 10;
            this.btnShowPrintPreview.Text = "打印机预览";
            this.btnShowPrintPreview.UseVisualStyleBackColor = true;
            this.btnShowPrintPreview.Visible = false;
            this.btnShowPrintPreview.Click += new System.EventHandler(this.btnShowPrintPreview_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(131, 234);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "第一打印机";
            // 
            // cbbFirstPrinter
            // 
            this.cbbFirstPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbFirstPrinter.FormattingEnabled = true;
            this.cbbFirstPrinter.Location = new System.Drawing.Point(208, 231);
            this.cbbFirstPrinter.Name = "cbbFirstPrinter";
            this.cbbFirstPrinter.Size = new System.Drawing.Size(121, 20);
            this.cbbFirstPrinter.TabIndex = 12;
            this.cbbFirstPrinter.SelectedIndexChanged += new System.EventHandler(this.cbbFirstPrinter_SelectedIndexChanged);
            // 
            // cbbSecondPrinter
            // 
            this.cbbSecondPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbSecondPrinter.FormattingEnabled = true;
            this.cbbSecondPrinter.Location = new System.Drawing.Point(441, 231);
            this.cbbSecondPrinter.Name = "cbbSecondPrinter";
            this.cbbSecondPrinter.Size = new System.Drawing.Size(121, 20);
            this.cbbSecondPrinter.TabIndex = 14;
            this.cbbSecondPrinter.SelectedIndexChanged += new System.EventHandler(this.cbbSecondPrinter_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(364, 234);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "第二打印机";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(131, 267);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(287, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "注：面单请配置第一打印机，发票请配置第二打印机 ";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 299);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbbSecondPrinter);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbbFirstPrinter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnShowPrintPreview);
            this.Controls.Add(this.chkAutoPrint);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSetting);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "快速打印客户端 v1.5";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView lswPrintHistory;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkAutoPrint;
        private System.Windows.Forms.Button btnShowPrintPreview;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbFirstPrinter;
        private System.Windows.Forms.ComboBox cbbSecondPrinter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ColumnHeader Status;
    }
}

