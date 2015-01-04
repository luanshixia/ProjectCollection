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
    /// StarBar.xaml 的交互逻辑
    /// </summary>
    public partial class StarBar : UserControl
    {
        public StarBar()
        {
            InitializeComponent();
        }

        public void SetStar(double value)
        {
            string lightstar = "Resources/3.gif";
            string darkstar = "Resources/4.gif";
            if (value <= 0)
            {
                star1.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star2.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star3.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star4.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star5.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
            }
            else if (0<value&&value <=2)
            {
                star1.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star2.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star3.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star4.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star5.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
            }
            else if (2 < value&&value <= 4)
            {
                star1.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star2.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star3.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star4.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star5.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
            }
            else if (4 < value && value <= 6)
            {
                star1.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star2.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star3.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star4.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
                star5.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
            }
            else if (6 < value && value <= 8)
            {
                star1.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star2.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star3.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star4.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star5.Source = new BitmapImage(new Uri(darkstar, UriKind.Relative));
            }
            else
            {
                star1.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star2.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star3.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star4.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
                star5.Source = new BitmapImage(new Uri(lightstar, UriKind.Relative));
            }
        }

        //private void ShowStar(double value)
    }
}
