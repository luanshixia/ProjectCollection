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

namespace HtmlToImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            TextArea.Text = string.Empty;
        }

        private void MenuItem_Show_Click(object sender, RoutedEventArgs e)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                var renderer = new ImageRenderer();
                renderer.ExportAsync(TextArea.Text, stream);
                var bf = BitmapFrame.Create(stream, 
                    BitmapCreateOptions.None, 
                    BitmapCacheOption.OnLoad);
                Image image = new Image { Source = bf, Width = bf.PixelWidth, Height = bf.PixelHeight };
                ScrollViewer sv = new ScrollViewer { Content = image, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
                Window window = new Window { Content = sv, Title = "HTML Preview", 
                    Owner = this, WindowStyle = WindowStyle.ToolWindow };
                window.ShowDialog();
            }
        }
    }
}
