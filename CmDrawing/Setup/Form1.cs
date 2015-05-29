using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TongJi.Setup
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtLicense.Text = SetupManager.GetLicenseText();
            txtPath.Text = SetupManager.GetDefaultInstallationPath();
            btnInstall.Focus();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (cbAgree.Checked)
            {
                //try
                //{
                if (AcadProductManager.GetProduct() == null)
                {
                    MessageBox.Show("未检测到 AutoCAD 2010。无法安装本程序。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    SetupManager.Start(SetupManager.BuildSetupConfig(txtPath.Text));
                    MessageBox.Show("安装成功。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //}
                //catch
                //{
                //    MessageBox.Show("安装程序遇到问题。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //}
                btnInstall.Enabled = false;
                btnExit.Focus();
                btnExit.Click -= btnExit_Click;
                btnExit.Click += (s, args) => Application.Exit();
            }
            else
            {
                MessageBox.Show("必须同意安装协议才能安装本软件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确实要退出安装程序吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
