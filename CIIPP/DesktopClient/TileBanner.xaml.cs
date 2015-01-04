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
    /// TileBanner.xaml 的交互逻辑
    /// </summary>
    public partial class TileBanner : UserControl
    {
        

        public TileBanner()
        {
            InitializeComponent();

            AppName.Text = DesktopClient.Properties.Resources.AppName;
            AppName.FontFamily = new System.Windows.Media.FontFamily("微软雅黑");
            Name1.Text = string.Empty;
            Name2.Text = string.Empty;
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TileMainPage.xaml");
        }

        private void btnFullscreen_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.ToggleFullscreen();
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.TryPrint();
        }

        private void btncolor_Click(object sender, RoutedEventArgs e)
        {
            popcolor.IsOpen = true;
            popcolor.MouseLeave += (s, arg) => popcolor.IsOpen = false;
        }

        private void miSkin1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ImagePath = "Resources/";
            MainWindow.Current.NavFrame.Refresh();
            OptionsManager.Options["Imagepath"] = "Resources/";
            OptionsManager.Save();
        }

        private void miSkin2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.ImagePath = "Resources/green/";
            MainWindow.Current.NavFrame.Refresh();
            OptionsManager.Options["Imagepath"]= "Resources/green/";
            OptionsManager.Save();
        }
    }
}
