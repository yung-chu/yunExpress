namespace LMS.WinForm.Client
{
    partial class FrmScaleSetting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbOut = new System.Windows.Forms.ComboBox();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbBot = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.rdbUSB = new System.Windows.Forms.RadioButton();
            this.rdbCOM = new System.Windows.Forms.RadioButton();
            this.txtSerialData = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbOut);
            this.groupBox1.Controls.Add(this.cbPort);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbBot);
            this.groupBox1.Location = new System.Drawing.Point(37, 101);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(191, 230);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "串口电子称设置";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 163);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "输出模式：";
            // 
            // cbOut
            // 
            this.cbOut.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOut.FormattingEnabled = true;
            this.cbOut.Items.AddRange(new object[] {
            "直接输出",
            "稳定后输出"});
            this.cbOut.Location = new System.Drawing.Point(41, 187);
            this.cbOut.Name = "cbOut";
            this.cbOut.Size = new System.Drawing.Size(135, 20);
            this.cbOut.TabIndex = 9;
            this.cbOut.SelectedIndexChanged += new System.EventHandler(this.cbOut_SelectedIndexChanged);
            // 
            // cbPort
            // 
            this.cbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(41, 61);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(135, 20);
            this.cbPort.TabIndex = 5;
            this.cbPort.SelectedIndexChanged += new System.EventHandler(this.cbPort_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "波特率：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "COM端口：";
            // 
            // cbBot
            // 
            this.cbBot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBot.FormattingEnabled = true;
            this.cbBot.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200"});
            this.cbBot.Location = new System.Drawing.Point(41, 125);
            this.cbBot.Name = "cbBot";
            this.cbBot.Size = new System.Drawing.Size(135, 20);
            this.cbBot.TabIndex = 6;
            this.cbBot.SelectedIndexChanged += new System.EventHandler(this.cbBot_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(112, 44);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "电子秤类型";
            // 
            // rdbUSB
            // 
            this.rdbUSB.AutoSize = true;
            this.rdbUSB.Location = new System.Drawing.Point(194, 42);
            this.rdbUSB.Name = "rdbUSB";
            this.rdbUSB.Size = new System.Drawing.Size(65, 16);
            this.rdbUSB.TabIndex = 3;
            this.rdbUSB.TabStop = true;
            this.rdbUSB.Text = "USB输出";
            this.rdbUSB.UseVisualStyleBackColor = true;
            this.rdbUSB.CheckedChanged += new System.EventHandler(this.rdbUSB_CheckedChanged);
            // 
            // rdbCOM
            // 
            this.rdbCOM.AutoSize = true;
            this.rdbCOM.Location = new System.Drawing.Point(274, 42);
            this.rdbCOM.Name = "rdbCOM";
            this.rdbCOM.Size = new System.Drawing.Size(71, 16);
            this.rdbCOM.TabIndex = 4;
            this.rdbCOM.TabStop = true;
            this.rdbCOM.Text = "串口输出";
            this.rdbCOM.UseVisualStyleBackColor = true;
            this.rdbCOM.CheckedChanged += new System.EventHandler(this.rdbUSB_CheckedChanged);
            // 
            // txtSerialData
            // 
            this.txtSerialData.BackColor = System.Drawing.Color.White;
            this.txtSerialData.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtSerialData.Location = new System.Drawing.Point(257, 202);
            this.txtSerialData.Name = "txtSerialData";
            this.txtSerialData.Size = new System.Drawing.Size(187, 35);
            this.txtSerialData.TabIndex = 12;
            this.txtSerialData.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "串口电子秤实时数据";
            // 
            // FrmScaleSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(466, 350);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtSerialData);
            this.Controls.Add(this.rdbCOM);
            this.Controls.Add(this.rdbUSB);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmScaleSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "电子称设置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmScaleSetting_FormClosing);
            this.Load += new System.EventHandler(this.ScaleSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbOut;
        private System.Windows.Forms.ComboBox cbPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbBot;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton rdbUSB;
        private System.Windows.Forms.RadioButton rdbCOM;
        private System.Windows.Forms.TextBox txtSerialData;
        private System.Windows.Forms.Label label5;
    }
}