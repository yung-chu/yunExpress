using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LMS.WinForm.Client.Common;
using LMS.WinForm.Client.Models;

namespace LMS.WinForm.Client
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();

            txtUserName.Text = LoginHelper.GetUserName();
        }

        private const string AppName = "LMS.WinForm.Client";
        private const string Version = "BuildOn2014-12-26";

        private void btnLogin_Click(object sender, EventArgs e)
        {
            new BackgroundLoading(this, () => this.Invoke(new Action(() =>
                {
                    string userName = txtUserName.Text.Trim();
                    string pwd = txtPwd.Text.Trim();
                    if (ValidateLogOn(userName, pwd))
                    {
                        LoginModel loginModel = new LoginModel() {UserName = userName, Pwd = pwd};
                        var info = InvokeWebApiHelper.PostLogin(loginModel);
                        if (info != null)
                        {
                            LoginHelper.CurrentUserName = userName;

                            if (ckbRemember.Checked)
                            {
                                LoginHelper.SaveUserInfo(userName);
                            }
                            else
                            {
                                LoginHelper.DeleteUserInfo();
                            }
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            lblPwdTip.Text = "用户名或密码错误";
                        }
                    }
                })), "登录中...", "正在验证用户名和密码...").Show();
        }


        /// <summary>
        /// 验证用户输入登录信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool ValidateLogOn(string userName, string pwd)
        {
            bool bResult = true;
            if (string.IsNullOrWhiteSpace(userName))
            {
                lblUserNameTip.Text = "用户名不能为空";
                bResult = false;
            }
            else
            {
                lblUserNameTip.Text = "";
            }
            if (string.IsNullOrWhiteSpace(pwd))
            {
                lblPwdTip.Text = "密码不能为空";
                bResult = false;
            }
            else
            {
                lblPwdTip.Text = "";
            }
            return bResult;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            ChechUpdate();
        }

        private void ChechUpdate()
        {
            var updateResponse = InvokeWebApiHelper.GetLatestVersion(AppName);
            if (updateResponse == null || !updateResponse.Success)
            {
                MessageBox.Show("检测版本失败！");
                Environment.Exit(0);
            }
            else if (updateResponse.Version != Version)
            {
                MessageBox.Show("有新版本发布,请升级！");
                Process p = new Process();
                p.StartInfo.FileName = "iexplore.exe";
                p.StartInfo.Arguments = updateResponse.Url;
                p.Start();
                p.Dispose();
                Environment.Exit(0);
            }

        }
    }
}
