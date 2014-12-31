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

namespace PictureBookViewer
{
    /// <summary>
    /// ZoomToWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ZoomToWindow : Window
    {
        public double Magnification
        {
            get
            {
                string source = TextBox1.Text;
                if (source.EndsWith("%"))
                {
                    source = source.Remove(source.Length - 1);
                    source = source.Insert(source.Length - 2, ".");
                }
                double mag = 0;
                if (double.TryParse(source, out mag))
                {
                    return mag;
                }
                else
                {
                    return 1;
                }
            }
        }

        public ZoomToWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
