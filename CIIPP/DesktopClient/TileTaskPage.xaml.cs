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
using System.Windows.Media.Animation;
using CIIPP;
using System.Windows.Controls.Primitives;
using System.IO;

namespace DesktopClient
{
    /// <summary>
    /// TileTaskPage.xaml 的交互逻辑
    /// </summary>
    public partial class TileTaskPage : UserControl
    {
        public TileTaskPage()
        {
            InitializeComponent();

            btn01.Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 10, ShadowDepth = 3 };
            btn02.Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 10, ShadowDepth = 3 };
            btn03.Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 10, ShadowDepth = 3 };
            btn04.Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 10, ShadowDepth = 3 };

            btn01.NormalImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-01-1.jpg"));
            btn01.HoverImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-01-3.jpg"));
            btn01.DownImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-01-2.jpg"));

            btn02.NormalImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-02-1.jpg"));
            btn02.HoverImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-02-3.jpg"));
            btn02.DownImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-02-2.jpg"));

            btn03.NormalImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-03-1.jpg"));
            btn03.HoverImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-03-3.jpg"));
            btn03.DownImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-03-2.jpg"));

            btn04.NormalImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-04-1.jpg"));
            btn04.HoverImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-04-3.jpg"));
            btn04.DownImage = new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-04-2.jpg"));

            btn05.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "menu1_2.jpg")));
            stackPanel1.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "menu2_2.jpg")));
            stackPanel4.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "menu3_2.jpg")));

            Button[] btns = new Button[] { btn01, btn02, btn03, btn04 };
            foreach (var btn in btns)
            {
                var btn1 = btn;
                btn1.MouseEnter += (s, arg) =>
                {
                    Enlarge(btn1);
                };
                btn1.MouseLeave += (s, arg) =>
                {
                    Lessen(btn1);
                };
                btn1.MouseLeftButtonDown += (s, arg) => { };
                btn1.MouseLeftButtonUp += (s, arg) => { };
            }
            //fullscreen.SetBinding(TextBlock.TextProperty, new Binding("FullscreenToolTip") { Source = MainWindow.Current });
        }

        protected void Enlarge(object sender)
        {
            FrameworkElement slide = sender as FrameworkElement;
            ScaleTransform st = new ScaleTransform(1.1, 1.1, slide.ActualWidth / 2, slide.ActualHeight / 2);
            slide.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1, 1.1, new Duration(TimeSpan.Parse("0:0:0.1")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
            Grid.SetZIndex(btn01, 0);
            Grid.SetZIndex(btn02, 0);
            Grid.SetZIndex(btn03, 0);
            Grid.SetZIndex(btn04, 0);
            Grid.SetZIndex(slide, 100);
        }

        protected void Lessen(object sender)
        {
            FrameworkElement slide = sender as FrameworkElement;
            ScaleTransform st = new ScaleTransform(1.1, 1.1, slide.ActualWidth / 2, slide.ActualHeight / 2);
            slide.RenderTransform = st;
            DoubleAnimation da1 = new DoubleAnimation(1.1, 1, new Duration(TimeSpan.Parse("0:0:0.1")));
            st.BeginAnimation(ScaleTransform.ScaleXProperty, da1);
            st.BeginAnimation(ScaleTransform.ScaleYProperty, da1);
        }

        private void btn03_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TileSecondaryPage.xaml", "part=2");
        }

        private void btn04_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TileSecondaryPage.xaml", "part=3");
        }

        private void btn01_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TileSecondaryPage.xaml", "part=0");
        }

        private void btn02_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("TileSecondaryPage.xaml", "part=1");
        }

        private void miNew_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Commands.New();
        }

        private void miOpen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Commands.Open();
        }

        private void miSave_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Commands.Save();
        }

        private void miSaveAs_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Commands.SaveAs();
        }

        private void btnFullscreen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Current.ToggleFullscreen();
        }

        private void miAbout_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationManager.Navigate("TileContentPage.xaml", "part=about");
        }

        private void miHelp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NavigationManager.Navigate("HelpPage.xaml");
        }

        private void miExit_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Current.Close();
        }

        private void Help1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("用户手册.doc");
        }
    }
}
