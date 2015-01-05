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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using TongJi.Gis.Display;

namespace CityInfographics
{
    /// <summary>
    /// FluidWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PumpWindow : Window
    {
        private Rectangle _rect;

        public PumpWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            MapDataManager.Open("Data\\pump.ciml");
            TheCanvas.InitializeMap(MapDataManager.LatestMap);
            //InsertImage(MapControl.Current.GridLayer, "Data\\pump.jpg", 55997276, 26989689, 57585558, 28919022);
            InsertImage(MapControl.Current.GridLayer, "Data\\pump_new.jpg", 55707070, 26500159, 57631606, 29226584);
            ReadyScene();
            TheCanvas.Zoom(MapDataManager.LatestMap.GetExtents().Extend(1.5));
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
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

            ImageBrush brush = new ImageBrush(bitmap) { RelativeTransform = new ScaleTransform { ScaleY = -1 }, TileMode = TileMode.Tile };
            _rect = new Rectangle { Width = width, Height = height, Fill = brush };
            Canvas.SetLeft(_rect, minx);
            Canvas.SetTop(_rect, miny);
            mLayer.Children.Add(_rect);
        }

        private static void AddPump(DrawingMapLayer animationLayer, Point pos, Brush stroke, Brush fill, double size, double strokeWidth, double pumpSize, double periord)
        {
            EllipseGeometry geometry = new EllipseGeometry(pos, size, size);
            GeometryDrawing drawing = new GeometryDrawing(fill, new Pen(stroke, strokeWidth), geometry);
            //var mLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == "城镇") as DrawingMapLayer;
            animationLayer.AddOverlayChildren(drawing);

            DoubleAnimationUsingKeyFrames animation = new DoubleAnimationUsingKeyFrames();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(periord));
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(size, KeyTime.FromPercent(0)));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(pumpSize, KeyTime.FromPercent(0.1)));
            animation.KeyFrames.Add(new LinearDoubleKeyFrame(size, KeyTime.FromPercent(1)));

            geometry.BeginAnimation(EllipseGeometry.RadiusXProperty, animation);
            geometry.BeginAnimation(EllipseGeometry.RadiusYProperty, animation);
        }

        private static void AddFluid(DrawingMapLayer animationLayer, TongJi.Gis.Display.IDataFluidTheme theme, TongJi.Gis.IFeature f)
        {
            var poly = new TongJi.Geometry.Polyline(f.GeoData);
            double length = poly.Length;
            if (length < 10)
            {
                return;
            }

            // 生成PathGeometry
            var points = poly.Points.Select(p => new Point(p.x, p.y)).ToList();
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure { StartPoint = points.First() };
            PolyLineSegment segment = new PolyLineSegment(points, true);
            figure.Segments.Add(segment);
            geometry.Figures.Add(figure);

            // 读取参数
            double velocity = theme.GetVelocity(f);
            double time = length / velocity;
            double space = 1 / theme.GetDensity(f);
            int spotCount = (int)(length / space) + 1;
            var color = theme.GetColor(f);

            // 应用动画
            for (int i = 0; i < spotCount; i++)
            {
                PointAnimationUsingPath paup = new PointAnimationUsingPath();
                paup.PathGeometry = geometry;
                paup.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)(time * 1000)));
                paup.RepeatBehavior = RepeatBehavior.Forever;
                paup.BeginTime = new TimeSpan(0, 0, 0, 0, (int)(time / spotCount * i * 1000));

                ColorAnimation ca = new ColorAnimation(color.Item1, color.Item2, new Duration(new TimeSpan(0, 0, 0, 0, (int)(time * 1000))));
                ca.RepeatBehavior = RepeatBehavior.Forever;
                ca.BeginTime = new TimeSpan(0, 0, 0, 0, (int)(time / spotCount * i * 1000));

                double radius = theme.GetDiameter(f) / 2;
                var fill = new SolidColorBrush(color.Item1);
                EllipseGeometry spot = new EllipseGeometry(new Point(), radius, radius);
                GeometryDrawing spotDrawing = new GeometryDrawing(fill, null, spot);
                animationLayer.AddOverlayChildren(spotDrawing);
                spot.BeginAnimation(EllipseGeometry.CenterProperty, paup);
                fill.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            }
        }

        private void ReadyScene()
        {
            // 河道、暗管
            var pipeLayer = MapDataManager.LatestMap.Layers["管线"];
            SimpleFluidTheme theme1 = new SimpleFluidTheme { Velocity = 120000, Diameter = 30000, Density = 1.0 / 48000 };
            theme1.InnerColorTheme.MinColor = Color.FromRgb(77, 216, 233);
            theme1.InnerColorTheme.MaxColor = Color.FromRgb(77, 216, 233);
            var rivers = new string[] { "river1", "river2" };
            foreach (var river in rivers)
            {
                var feature = pipeLayer.Features.First(f => f["名称"] == river);
                AddFluid(MapControl.Current.AnimationLayer, theme1, feature);
            }
            SimpleFluidTheme theme2 = new SimpleFluidTheme { Velocity = 240000, Diameter = 20000, Density = 1.0 / 36000 };
            theme2.InnerColorTheme.MinColor = Color.FromRgb(0, 150, 233);
            theme2.InnerColorTheme.MaxColor = Color.FromRgb(0, 150, 233);
            var pipes = new string[] { "pipe1", "pipe2" };
            foreach (var pipe in pipes)
            {
                var feature = pipeLayer.Features.First(f => f["名称"] == pipe);
                AddFluid(MapControl.Current.AnimationLayer, theme2, feature);
            }

            // 泵
            var pumpLayer = MapDataManager.LatestMap.Layers["城镇"];
            var tishengs = new string[] { "tisheng1", "tisheng2", "tisheng3" };
            foreach (var pump in tishengs)
            {
                var feature = pumpLayer.Features.First(f => f["名称"] == pump);
                var point = new TongJi.Geometry.Point2D(feature.GeoData);
                var pos = new Point(point.x, point.y);
                AddPump(MapControl.Current.AnimationLayer, pos, Brushes.Black, Brushes.Orange, 30000, 15000, 60000, 1000);
            }
            var yongquans = new string[] { "yongquan1", "yongquan2", "yongquan3" };
            foreach (var pump in yongquans)
            {
                var feature = pumpLayer.Features.First(f => f["名称"] == pump);
                var point = new TongJi.Geometry.Point2D(feature.GeoData);
                var pos = new Point(point.x, point.y);
                AddPump(MapControl.Current.AnimationLayer, pos, Brushes.White, Brushes.Red, 40000, 0, 40000, 500);
                AddPump(MapControl.Current.AnimationLayer, pos, Brushes.White, Brushes.Red, 16000, 8000, 32000, 500);
            }
        }
    }
}
