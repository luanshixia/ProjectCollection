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

namespace CityInfographics
{
    /// <summary>
    /// LaunchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LaunchWindow : Window
    {
        public LaunchWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button btn1 = new Button { Content = "能源" };
            btn1.Click += (s, a) =>
            {
                MainWindow w = new MainWindow();
                w.Show();
            };
            LayoutRoot.Children.Add(btn1);

            Button btn2 = new Button { Content = "水" };
            btn2.Click += (s, a) =>
            {
                FluidWindow w = new FluidWindow();
                w.Show();
            };
            LayoutRoot.Children.Add(btn2);

            Button btn3 = new Button { Content = "泵" };
            btn3.Click += (s, a) =>
            {
                PumpWindow w = new PumpWindow();
                w.Show();
            };
            LayoutRoot.Children.Add(btn3);
        }
    }
}
