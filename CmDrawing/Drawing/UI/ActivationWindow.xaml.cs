using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TongJi.Drawing
{
    /// <summary>
    /// ActivationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ActivationWindow : Window
    {
        public bool HasTimeLimit { get; set; }

        public ActivationWindow()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            if (Services.LicenseManager.TryActivate(txtRequestCode.Text, txtKey.Text))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("无效的激活码。");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtRequestCode.Text = Security.GetRequestCode();
            if (HasTimeLimit)
            {
                License license = Services.LicenseManager.License;
                txtMessage.Text = string.Format("剩余试用天数：{0}；剩余试用次数：{1}", Math.Max(0, (license.Expire - DateTime.Now).Days), license.Times);
                if (Services.LicenseManager.IsTrialExpired())
                {
                    btnTry.Content = "退出";
                }
            }
        }

        private void btnTry_Click(object sender, RoutedEventArgs e)
        {
            Try();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Try();
        }

        private void Try()
        {
            this.DialogResult = false;
            if (Services.LicenseManager.IsTrialExpired())
            {
            }
            else
            {
                Services.LicenseManager.StartTrial();
            }
        }
    }
}
