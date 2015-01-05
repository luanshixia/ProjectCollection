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
    /// SwitchControl.xaml 的交互逻辑
    /// </summary>
    public partial class SwitchControl : UserControl
    {
        public Color OnColor { get; set; }
        public Color OffColor { get; set; }
        public double OnSize { get; set; }
        public double OffSize { get; set; }
        public string LeftOption { get { return Left.Text; } set { Left.Text = value; } }
        public string RightOption { get { return Right.Text; } set { Right.Text = value; } }

        public event Action<SwitchSide> Switched;
        protected void OnSwitched(SwitchSide side)
        {
            if (Switched != null)
            {
                Switched(side);
            }
        }

        public SwitchControl()
        {
            InitializeComponent();

            OnColor = Colors.Cyan;
            OffColor = Colors.LightGray;
            OnSize = 20;
            OffSize = 12;

            Left_MouseLeftButtonUp_1(null, null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Left.FontSize = OnSize;
            Left.Foreground = new SolidColorBrush(OnColor);
            Right.FontSize = OffSize;
            Right.Foreground = new SolidColorBrush(OffColor);
        }

        private void Left_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            Left.FontSize = OnSize;
            Left.Foreground = new SolidColorBrush(OnColor);
            Right.FontSize = OffSize;
            Right.Foreground = new SolidColorBrush(OffColor);
            OnSwitched(SwitchSide.Left);
        }

        private void Right_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            Right.FontSize = OnSize;
            Right.Foreground = new SolidColorBrush(OnColor);
            Left.FontSize = OffSize;
            Left.Foreground = new SolidColorBrush(OffColor);
            OnSwitched(SwitchSide.Right);
        }

        public enum SwitchSide
        {
            Left,
            Right
        }
    }
}
