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

using TongJi.Gis.Display;

namespace CityInfographics
{
    /// <summary>
    /// FluidWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FluidWindow : Window
    {
        private Rectangle _rect;

        public FluidWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            MapDataManager.Open("Data\\dibiao.ciml");
            TheCanvas.InitializeMap(MapDataManager.LatestMap);
            InsertImage(MapControl.Current.BaseLayer, "Data\\water.jpg", 59928, 26645, 61831, 29328);

            SimpleFluidTheme theme = new SimpleFluidTheme();
            theme.InnerColorTheme.MinColor = Color.FromRgb(255, 150, 0);
            theme.InnerColorTheme.MaxColor = Color.FromRgb(77, 216, 233);
            var mLayer = TheCanvas.Layers.First(x => x.LayerData.Name == "dibiao");
            mLayer.ApplyFluidTheme(theme);
            mLayer.Visibility = System.Windows.Visibility.Collapsed;

            theme.InnerColorTheme.MinColor = Color.FromRgb(77, 216, 233);
            theme.InnerColorTheme.MaxColor = Color.FromRgb(77, 216, 233);
            theme.Velocity = 40;
            theme.Diameter = 14;
            theme.Density = 1 / 24.0;
            mLayer = TheCanvas.Layers.First(x => x.LayerData.Name == "river");
            mLayer.ApplyFluidTheme(theme);
        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            this.Hide();
            MainWindow2D mw = new MainWindow2D();
            mw.Show();
        }

        private void InsertImage(MapLayer mLayer, string source, double minx, double miny, double maxx, double maxy)
        {
            double width = maxx - minx;
            double height = maxy - miny;
            BitmapImage bitmap = new BitmapImage(new Uri(source, UriKind.Relative));

            //Image image = new Image { Width = width, Height = height, Source = bitmap, Stretch = Stretch.Fill };
            //Border border = new Border { Width = width, Height = height, Child = image, Background = Brushes.Black };
            //Canvas.SetLeft(border, minx);
            //Canvas.SetTop(border, miny);
            //mLayer.Children.Add(border);

            ImageBrush brush = new ImageBrush(bitmap) { RelativeTransform = new ScaleTransform { ScaleY = -1 }, TileMode = TileMode.Tile };
            _rect = new Rectangle { Width = width, Height = height, Fill = brush };
            Canvas.SetLeft(_rect, minx);
            Canvas.SetTop(_rect, miny);
            mLayer.Children.Add(_rect);

            //ImageDrawing image = new ImageDrawing(bitmap, new Rect(minx, miny, width, height));
            //(mLayer as DrawingMapLayer).DrawingGroup.Children.Insert(0, image);
        }

        private void UpdateImage(string source)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(source, UriKind.Relative));
            ImageBrush brush = new ImageBrush(bitmap) { RelativeTransform = new ScaleTransform { ScaleY = -1 }, TileMode = TileMode.Tile };
            _rect.Fill = brush;
        }

        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(MapControl.Current.Map.GetExtents().ToString());
            //var mLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == "dibiao");
            //InsertImage(MapControl.Current.BaseLayer, "Data\\water.jpg", 59928, 26645, 61831, 29328);
        }

        private void SwitchSunnyRainy(bool rainy)
        {
            var mLayer = TheCanvas.Layers.First(x => x.LayerData.Name == "dibiao");
            if (rainy)
            {
                //UpdateImage("Data\\water1.jpg");
                mLayer.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                //UpdateImage("Data\\water.jpg");
                mLayer.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void SwitchControl_Switched(SwitchControl.SwitchSide obj)
        {
            bool rainy = obj == SwitchControl.SwitchSide.Right;
            SwitchSunnyRainy(rainy);
        }
    }
}
