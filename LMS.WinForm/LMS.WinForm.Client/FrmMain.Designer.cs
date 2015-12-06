namespace LMS.WinForm.Client
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.SysMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wayBillMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ReGoodsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.收货入仓管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PackageInStorage = new System.Windows.Forms.ToolStripMenuItem();
            this.PackageOutStorage = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SysMenu,
            this.wayBillMenu,
            this.helpMenu,
            this.收货入仓管理ToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1294, 25);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "MenuStrip";
            // 
            // SysMenu
            // 
            this.SysMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem,
            this.ScaleToolStripMenuItem,
            this.PrintToolStripMenuItem});
            this.SysMenu.ImageTransparentColor = System.Drawing.SystemColors.ActiveBorder;
            this.SysMenu.Name = "SysMenu";
            this.SysMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SysMenu.Size = new System.Drawing.Size(83, 21);
            this.SysMenu.Text = "系统设置(&S)";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.exitToolStripMenuItem.Text = "退出(&E)";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolsStripMenuItem_Click);
            // 
            // ScaleToolStripMenuItem
            // 
            this.ScaleToolStripMenuItem.Name = "ScaleToolStripMenuItem";
            this.ScaleToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.ScaleToolStripMenuItem.Text = "电子称设置";
            this.ScaleToolStripMenuItem.Click += new System.EventHandler(this.ScaleToolStripMenuItem_Click);
            // 
            // PrintToolStripMenuItem
            // 
            this.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem";
            this.PrintToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.PrintToolStripMenuItem.Text = "快速打印设置";
            this.PrintToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItem_Click);
            // 
            // wayBillMenu
            // 
            this.wayBillMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReGoodsToolStripMenuItem});
            this.wayBillMenu.Name = "wayBillMenu";
            this.wayBillMenu.Size = new System.Drawing.Size(83, 21);
            this.wayBillMenu.Text = "运单管理(&E)";
            // 
            // ReGoodsToolStripMenuItem
            // 
            this.ReGoodsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ReGoodsToolStripMenuItem.Image")));
            this.ReGoodsToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Black;
            this.ReGoodsToolStripMenuItem.Name = "ReGoodsToolStripMenuItem";
            this.ReGoodsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.ReGoodsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.ReGoodsToolStripMenuItem.Text = "退货入仓(&R)";
            this.ReGoodsToolStripMenuItem.Click += new System.EventHandler(this.ReGoodsToolStripMenuItem_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutToolStripMenuItem});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(61, 21);
            this.helpMenu.Text = "帮助(&H)";
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.AboutToolStripMenuItem.Text = "关于";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // 收货入仓管理ToolStripMenuItem
            // 
            this.收货入仓管理ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PackageInStorage,
            this.PackageOutStorage});
            this.收货入仓管理ToolStripMenuItem.Name = "收货入仓管理ToolStripMenuItem";
            this.收货入仓管理ToolStripMenuItem.Size = new System.Drawing.Size(92, 21);
            this.收货入仓管理ToolStripMenuItem.Text = "收货入仓管理";
            // 
            // PackageInStorage
            // 
            this.PackageInStorage.Name = "PackageInStorage";
            this.PackageInStorage.Size = new System.Drawing.Size(148, 22);
            this.PackageInStorage.Text = "速递包裹入仓";
            this.PackageInStorage.Click += new System.EventHandler(this.PackageInStorage_Click);
            // 
            // PackageOutStorage
            // 
            this.PackageOutStorage.Name = "PackageOutStorage";
            this.PackageOutStorage.Size = new System.Drawing.Size(148, 22);
            this.PackageOutStorage.Text = "速递包裹出仓";
            this.PackageOutStorage.Click += new System.EventHandler(this.PackageOutStorage_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 596);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1294, 22);
            this.statusStrip.TabIndex = 2;
            this.statusStrip.Text = "StatusStrip";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(32, 17);
            this.toolStripStatusLabel.Text = "状态";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1294, 618);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "物流系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SysMenu;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wayBillMenu;
        private System.Windows.Forms.ToolStripMenuItem ReGoodsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem 收货入仓管理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PackageInStorage;
        private System.Windows.Forms.ToolStripMenuItem PackageOutStorage;
        private System.Windows.Forms.ToolStripMenuItem ScaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PrintToolStripMenuItem;
    }
}



