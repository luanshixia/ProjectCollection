using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;
using TongJi.Gis.Display;

namespace CityInfographics
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow2D : Window
    {
        public MainWindow2D()
        {
            InitializeComponent();

            App.View2D = this;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

        }        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MapDataManager.Open("Data\\base.ciml");
            TheCanvas.InitializeMap(MapDataManager.LatestMap);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BiColorGradientTheme theme = new BiColorGradientTheme("f", 0, 5);
            theme.MaxColor = Colors.DodgerBlue;
            theme.MinColor = Colors.White;
            MapControl.Current.Layers[0].ApplyColorTheme(theme);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
