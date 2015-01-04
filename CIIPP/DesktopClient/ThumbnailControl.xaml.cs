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
    /// ThumbnailControl.xaml 的交互逻辑
    /// </summary>
    public partial class ThumbnailControl : UserControl
    {
        private static Random _random = new Random(DateTime.Now.Second);

        public ThumbnailControl()
        {
            InitializeComponent();

            Paper.Background = new SolidColorBrush(GetRandomColor());
        }

        private Color GetRandomColor()
        {
            byte r = (byte)_random.Next(220, 255);
            byte g = (byte)(r - 10);
            byte b = (byte)_random.Next(140, 200);
            return Color.FromArgb(255, r, g, b);
        }
    }
}
