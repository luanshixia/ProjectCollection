using Dreambuild.Gis.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Desktop
{
    public class MainDemo : DemoBase
    {
        public const string LAYER_DRAW_POINT = "lc_draw_point";
        public const string LAYER_DRAW_POLY = "lc_draw_poly";
        private Rectangle _rect;

        public MainDemo()
        {
            Name = "CityGIS Desktop";
            Description = "主程序";
        }

        public override void OnLoad()
        {
            base.OnLoad();

            this.GetDemoTools().ForEach(t =>
            {
                MenuItem mi = new MenuItem { Header = t.Item1 };
                mi.Click += t.Item2;
                MainWindow.Current.ToolsMenu.Items.Add(mi);
            });

            MainWindow.Current.BeforeMapShow += () => PreProcessMap();
        }

        private void PreProcessMap()
        {
            if (!MapDataManager.LatestMap.Layers.Any(l => l.Name == MainDemo.LAYER_DRAW_POINT))
            {
                MapDataManager.LatestMap.Layers.Add(new VectorLayer(MainDemo.LAYER_DRAW_POINT, VectorLayer.GEOTYPE_POINT));
                MapDataManager.LatestMap.Layers.Add(new VectorLayer(MainDemo.LAYER_DRAW_POLY, VectorLayer.GEOTYPE_REGION));
            }
        }

        //[DemoTool("test01")]
        //public void Test01()
        //{
        //    MapControl.Current.Layers.First(x => x.LayerData.Name == "道路").ApplyFluidTheme(new SimpleFluidTheme());
        //}

        [DemoTool("显示用地指标迷你图")]
        public void ShowParcelMiniChart()
        {
            var chart = new Dreambuild.Gis.Display.SpotChart(50, 50);
            string[] props = { "容积率", "建筑密度" };
            double max = MapDataManager.LatestMap.Layers["地块"].Features.Select(f => f["容积率"].TryParseToDouble()).Max();
            foreach (var prop in props)
            {
                var col = new Dreambuild.Gis.Display.SpotChartColumn(chart, prop, max, Colors.DarkRed);
                chart.Columns.Add(col);
            }
            MapControl.Current.FindMapLayers("地块").ForEach(x => x.ApplySpotChart(chart));
        }

        [DemoTool("测试Svg")]
        public void TestSvg()
        {
            Dreambuild.Svg.SvgWriter svg = new Dreambuild.Svg.SvgWriter();
            svg.AddLinearGradient("g1", svg.GradientStops(
                svg.GradientStop(0, "orange"),
                svg.GradientStop(1, "red")
            ));
            svg.SetFill("url(#g1)");
            svg.SetStroke("black");
            svg.AddRect("200", "200", "100", "100");
            svg.Save("D:\\1.svg");
        }

        [DemoTool("地图范围")]
        public void MapExtents()
        {
            MessageBox.Show(MapDataManager.LatestMap.GetExtents().ToString());
        }

        [DemoTool("读取GPX格式")]
        public void LoadGpx()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "GPX File (*.gpx)|*.gpx";
            if (ofd.ShowDialog() == true)
            {
                var gpx = Dreambuild.Gis.Formats.Gpx.Load(ofd.FileName);
                MapControl.Current.InitializeMap(gpx.ToMap());
                MultiColorGradientTheme mcgt = new MultiColorGradientTheme("Velocity", 0, 50);
                mcgt.Stops.Clear();
                mcgt.Stops.Add(0, Colors.Red);
                mcgt.Stops.Add(25, Colors.Goldenrod);
                mcgt.Stops.Add(50, Colors.Green);
                MapControl.Current.FindMapLayers("Link").ForEach(x =>
                {
                    x.LayerStyle.StrokeWeight = 3;
                    x.ApplyColorTheme(mcgt);
                });
            }
        }

        [DemoTool("三维查看器 - 简单")]
        public void ThreeDViewer()
        {
            HelixWindow hw = new HelixWindow();
            Helix.BlockScene scene = new Helix.BlockScene(hw.TheViewport, MapDataManager.LatestMap);
            hw.Show();
        }

        [DemoTool("三维查看器 - 通用")]
        public void GeneralThreeDViewer()
        {
            HelixWindow hw = new HelixWindow();
            Helix.CityScene scene = new Helix.CityScene(hw.TheViewport, MapDataManager.LatestMap);
            scene.Update();
            hw.Show();
        }

        //[DemoTool("test02")]
        //public void test02()
        //{
        //    var features = MapDataManager.LatestMap.Layers["emme_links"].Features;
        //    foreach (var feature in features)
        //    {
        //        var geo = feature.GeoData;
        //        var points = geo.Split('|');
        //        var pointA = points[0].Split(',');
        //        var pointAX = Convert.ToDouble(pointA[0]) * 1000;
        //        var pointAY = Convert.ToDouble(pointA[1]) * 1000;

        //        var pointB = points[1].Split(',');
        //        var pointBX = Convert.ToDouble(pointB[0]) * 1000;
        //        var pointBY = Convert.ToDouble(pointB[1]) * 1000;

        //        feature.GeoData = string.Format("{0},{1}|{2},{3}", pointAX, pointAY, pointBX, pointBY);
        //    }
        //}

        [DemoTool("OpenBitmapReference")]
        public void OpenBitmapReference()
        {
            var ofd = new Microsoft.Win32.OpenFileDialog { Title = "Choose file", Filter = "Bitmap Image Format (*.bmp, *.jpg, *.png, *.tif)|*.bmp;*.jpg;*.png;*.tif|All Files (*.*)|*.*" };
            if (ofd.ShowDialog() == true)
            {
                var extents = MapDataManager.LatestMap.GetExtents();
                InsertImage(MapControl.Current.BaseLayer, ofd.FileName, extents.Min.Value.X, extents.Min.Value.Y, extents.Max.Value.X, extents.Max.Value.Y);
            }
        }

        [DemoTool("TuneBitmapReference")]
        public void TuneBitmapReference()
        {
            if (_rect != null)
            {
                ManipulateTool mt = new ManipulateTool(_rect);
                ViewerToolManager.ExclusiveTool = mt;
            }
        }

        [DemoTool("CloseBitmapReference")]
        public void CloseBitmapReference()
        {
            RemoveImage(MapControl.Current.BaseLayer);
        }

        [DemoTool("AddParcelSpot")]
        public void AddParcelSpot()
        {
            AddSpotInternal(LAYER_DRAW_POINT, p =>
            {
                var f = new Feature(p);
                //InitializeFeature(f, LAYER_DRAW_POINT);
                return f;
            });
        }

        [DemoTool("DrawEntity")]
        public void DrawEntity()
        {
            AddPolyInternal(LAYER_DRAW_POLY, pts =>
            {
                var f = new Feature(pts);
                //InitializeFeature(f, LAYER_DRAW_POLY);
                return f;
            });
        }

        [DemoTool("DrawOrderMoveToBottom")]
        public void DrawOrderMoveToBottom()
        {
            if (SelectionSet.Contents.Count > 0)
            {
                SelectionSet.Contents.ForEach(feature =>
                {
                    MapControl.Current.BringToBack(feature);
                });
            }
            else
            {
                MessageBox.Show("请先选择实体。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        [DemoTool("RemoveEntities")]
        public void RemoveEntities()
        {
            RemoveEntityInternal();
        }

        //[DemoTool("test")]
        //public void test()
        //{
        //    Gui.WebBrowser("test", GetFileUrl("Web/legend.html"), 300, 300);
        //}

        //private static string GetFileUrl(string relativeUrl)
        //{
        //    return string.Format("file://{0}/{1}", Environment.CurrentDirectory, relativeUrl);
        //}

        private void AddSpotInternal(string layerName, Func<Geometry.Vector, IFeature> featureSelector)
        {
            PickPointTool pp = new PickPointTool();
            CursorTipTool ct = new CursorTipTool("指定点");
            CombinedViewerTool cvt = new CombinedViewerTool(pp, ct);
            var mLayer = MapControl.Current.FindMapLayers(layerName).FirstOrDefault();
            if (mLayer == null)
            {
                return;
            }
            pp.PointPicked += p =>
            {
                mLayer.AddFeature(featureSelector(p));
                MapControl.Current.AdjustMagFactor();
            };
            ViewerToolManager.ExclusiveTool = cvt;
        }

        private void AddPolyInternal(string layerName, Func<List<Geometry.Vector>, IFeature> featureSelector)
        {
            DrawLineTool dlt = new DrawLineTool();
            CursorTipTool ctt = new CursorTipTool("指定点");
            CombinedViewerTool cvt = new CombinedViewerTool(dlt, ctt);
            var mLayer = MapControl.Current.FindMapLayers(layerName).FirstOrDefault();
            if (mLayer == null)
            {
                return;
            }
            dlt.Completed += pts =>
            {
                mLayer.AddFeature(featureSelector(pts));
            };
            ViewerToolManager.ExclusiveTool = cvt;
        }

        private void RemoveEntityInternal()
        {
            if (SelectionSet.Contents.Count > 0)
            {
                if (MessageBox.Show(string.Format("选中 {0} 个实体，确定移除它们吗？", SelectionSet.Contents.Count), "提示", MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("请先选择实体。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            MapControl.Current.RemoveFeatures(SelectionSet.Contents);
            SelectionSet.ClearSelection();
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

        private void TuneImage(double minx, double miny, double maxx, double maxy)
        {
            _rect.Width = maxx - minx;
            _rect.Height = maxy - miny;
            Canvas.SetLeft(_rect, minx);
            Canvas.SetTop(_rect, miny);
        }

        private void RemoveImage(MapLayer mLayer)
        {
            mLayer.Children.Remove(_rect);
        }

        private Dictionary<string, string> GetImageExtents()
        {
            var extents = Geometry.Extents.Create(0, 1000, 0, 1000);
            if (MapDataManager.LatestMap != null)
            {
                extents = MapDataManager.LatestMap.GetExtents();
            }
            var dict = new Dictionary<string, string>
            {
                { "minx", extents.Min.Value.X.ToString() },
                { "miny", extents.Min.Value.Y.ToString() },
                { "maxx", extents.Max.Value.X.ToString() },
                { "maxy", extents.Max.Value.Y.ToString() },
            };
            if (Gui.MultiInputs("指定插入范围", dict) == true)
            {
                return dict;
            }
            return null;
        }

        public static void TryOpenFile(string fileName)
        {
            try
            {
                System.Diagnostics.Process.Start(fileName);
            }
            catch
            {

            }
        }
    }
}
