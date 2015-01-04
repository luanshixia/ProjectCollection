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

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// TaskPage.xaml 的交互逻辑
    /// </summary>
    public partial class TaskPage : UserControl
    {
        public TaskPage()
        {
            InitializeComponent();
        }

        private void btn01_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("RichCityPage.xaml");
        }

        private void btn02_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("RichFinancialPage.xaml");
        }

        private void btn03_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("SummaryPage.xaml");
        }

        private void btn04_Click(object sender, RoutedEventArgs e)
        {
            if (Nav.CheckProjectExist())
            {
                NavigationManager.Navigate("RichProjectPage.xaml");
            }
        }

        private void btn05_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("PipPage.xaml");
        }

        public void CloseAllPopups()
        {
            foreach (var ele in LayoutRoot.Children)
            {
                if (ele is System.Windows.Controls.Primitives.Popup)
                {
                    (ele as System.Windows.Controls.Primitives.Popup).IsOpen = false;
                }
            }
        }

        private void btn01_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseAllPopups();
            //pop01.IsOpen = true;
            pop01.Focus();
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseAllPopups();
        }
    }
}
