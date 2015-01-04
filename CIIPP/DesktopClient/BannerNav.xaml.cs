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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClient
{
    /// <summary>
    /// BannerNav.xaml 的交互逻辑
    /// </summary>
    public partial class BannerNav : UserControl
    {
        public BannerNav()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Back();
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Forward();
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("RichMainPage.xaml");
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.RefreshPage();
        }

        private void btnA1_Click(object sender, RoutedEventArgs e)
        {
            Nav.City(1);
        }

        private void btnA2_Click(object sender, RoutedEventArgs e)
        {
            Nav.City(2);
        }

        private void btnA3_Click(object sender, RoutedEventArgs e)
        {
            Nav.City(3);
        }

        private void btnA4_Click(object sender, RoutedEventArgs e)
        {
            Nav.City(4);
        }

        private void btnA5_Click(object sender, RoutedEventArgs e)
        {
            Nav.City(5);
        }

        private void btnA6_Click(object sender, RoutedEventArgs e)
        {
            Nav.Financial(0);
        }

        private void btnA7_Click(object sender, RoutedEventArgs e)
        {
            Nav.Financial(1);
        }

        private void btnA8_Click(object sender, RoutedEventArgs e)
        {
            Nav.Financial(2);
        }

        private void btnA9_Click(object sender, RoutedEventArgs e)
        {
            Nav.Financial(3);
        }

        private void btnA10_Click(object sender, RoutedEventArgs e)
        {
            Nav.Financial(5);
        }

        private void btnB1_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(0);
        }

        private void btnB3_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(1);
        }

        private void btnB4_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(2);
        }

        private void btnB5_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(3);
        }

        private void btnB6_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(4);
        }

        private void btnB7_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(5);
        }

        private void btnB8_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(7);
        }

        private void btnB9_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(9);
        }

        private void btnB10_Click(object sender, RoutedEventArgs e)
        {
            Nav.Project(10);
        }

        private void btnD1_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("SummaryPage.xaml");
        }

        private void btnC1_Click(object sender, RoutedEventArgs e)
        {
            Nav.Pip(0);
        }

        private void btnC2_Click(object sender, RoutedEventArgs e)
        {
            Nav.Pip(1);
        }

        private void btnC3_Click(object sender, RoutedEventArgs e)
        {
            Nav.Pip(2);
        }
    }
}
