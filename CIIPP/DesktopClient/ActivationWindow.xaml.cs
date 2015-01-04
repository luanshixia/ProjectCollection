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

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// ActivationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ActivationWindow : Window
    {
        public const string LicenseFile = "license.dat";

        public ActivationWindow()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Validate(txtRequestCode.Text, txtKey.Text))
            {
                Security.SaveLicense(LicenseFile, new License { RequestCode = txtRequestCode.Text, Key = txtKey.Text });
                this.DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LicenseState state = Security.CheckLicense(LicenseFile);
            // todo: show state in ui

            txtRequestCode.Text = Security.GetRequestCode();
        }

        public static bool CheckLicense()
        {
            if (MainWindow.LicenseState != LicenseState.Licensed)
            {
                MessageBox.Show("此功能必须激活后才能使用，请您购买并激活产品。", "产品许可", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                return false;
            }
            return true;
        }

        private void btnTry_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
