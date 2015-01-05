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

namespace CityInfographics
{
    /// <summary>
    /// LineHand.xaml 的交互逻辑
    /// </summary>
    public partial class LineHand : UserControl
    {
        public double Total { get; set; }

        public LineHand()
        {
            InitializeComponent();
        }

        public void SetValue(double value)
        {
            double pos = value / Total * this.ActualWidth;
            Hand.Margin = new Thickness(pos, 0, 0, 0);
        }
    }
}
