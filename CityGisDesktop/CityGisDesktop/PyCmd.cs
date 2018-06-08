using Dreambuild.Extensions;
using Dreambuild.Geometry;
using Dreambuild.Gis.Display;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Dreambuild.Gis.Desktop
{
    public static class SpotDiagram
    {
        // 测试代码
        // sd.layer('城镇')
        // sd.prop('testvalue')
        // sd.apply()
        // sd.mag(5)
        // sd.maxcolor(255,0,0)
        // sd.apply()
        // sd.clear()
        // sd.layer('地震点')
        // sd.prop('里氏震级')
        // sd.useenumsize()
        // sd.newenumsize()
        // sd.enumsize('8.0级以上',50)
        // sd.enumsize('7.0-7.9级',30)
        // sd.enumsize('6.0-6.9级',10)
        // sd.applysize()

        private static string _layer = string.Empty;
        private static string _prop = string.Empty;
        private static DoubleDataSizeTheme _sizeTheme = new DoubleDataSizeTheme();
        private static double _magfac = 1;
        private static BiColorGradientTheme _colorTheme = new BiColorGradientTheme();
        private static double _minSize = 10;
        private static double _maxSize = 50;
        private static bool _useEnumSize = false;
        private static EnumDataSizeTheme _enumSizeTheme = new EnumDataSizeTheme();

        public static void layer(string layer)
        {
            clear(); // 换layer之前先清除theme
            _layer = layer;
        }

        public static void prop(string prop)
        {
            _prop = prop;
            _sizeTheme.Property = prop;
            _colorTheme.Property = prop;
            _enumSizeTheme.Property = prop;
        }

        public static string layer()
        {
            return _layer;
        }

        public static string prop()
        {
            return _prop;
        }

        public static void size(double min, double max)
        {
            _minSize = min;
            _maxSize = max;
        }

        public static void mag(double mag)
        {
            _magfac = mag * _magfac;
            applysize();
        }

        public static void magfac(double magfac)
        {
            _magfac = magfac;
        }

        public static double magfac()
        {
            return _magfac;
        }

        public static void mincolor(byte r, byte g, byte b)
        {
            _colorTheme.MinColor = System.Windows.Media.Color.FromRgb(r, g, b);
        }

        public static void maxcolor(byte r, byte g, byte b)
        {
            _colorTheme.MaxColor = System.Windows.Media.Color.FromRgb(r, g, b);
        }

        public static void apply()
        {
            applysize();
            applycolor();
        }

        public static void clear()
        {
            // 测试代码：sd.clear()

            if (MapControl.Current.Layers.Any(x => x.LayerData.Name == _layer))
            {
                var mLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == _layer);
                mLayer.ClearColorTheme();
                mLayer.ClearSizeTheme();
            }
        }

        public static void applysize()
        {
            // 早期测试代码：pycmd.sizedspotdiagram('城镇','testvalue', 3)

            var mLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == _layer);
            if (_useEnumSize)
            {
                mLayer.ApplySizeTheme(_enumSizeTheme);
            }
            else
            {
                var values = mLayer.LayerData.Features.Select(x =>
                {
                    double value = 0;
                    double.TryParse(x[_prop], out value);
                    return value;
                }).ToList();
                _sizeTheme.MinValue = values.Min();
                _sizeTheme.MaxValue = values.Max();
                _sizeTheme.MinSize = _magfac * _minSize;
                _sizeTheme.MaxSize = _magfac * _maxSize;
                mLayer.ApplySizeTheme(_sizeTheme);
            }
        }

        public static void applycolor()
        {
            // 早期测试代码：pycmd.coloredspotdiagram('城镇','testvalue')

            var mLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == _layer);
            var values = mLayer.LayerData.Features.Select(x =>
            {
                double value = 0;
                double.TryParse(x[_prop], out value);
                return value;
            }).ToList();
            _colorTheme.MinValue = values.Min();
            _colorTheme.MaxValue = values.Max();
            mLayer.ApplyColorTheme(_colorTheme);
        }

        public static void singlecolor(byte r, byte g, byte b)
        {
            var mLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == _layer);
            var theme = new SingleColorTheme(System.Windows.Media.Color.FromRgb(r, g, b));
            mLayer.ApplyColorTheme(theme);
        }

        public static void useenumsize()
        {
            _useEnumSize = true;
        }

        public static void usedoublesize()
        {
            _useEnumSize = false;
        }

        public static void newenumsize()
        {
            _enumSizeTheme.Sizes.Clear();
        }

        public static void enumsize(string value, double size)
        {
            _enumSizeTheme.Sizes[value] = size;
        }
    }

    public static class PyCmd
    {
        public static void toolbox(string title, Dictionary<string, string> tools)
        {
            Window window = new Window { Width = 250, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.Manual, ShowInTaskbar = false, Topmost = true, WindowStyle = WindowStyle.ToolWindow, Title = title, Left = MainWindow.Current.Left + 100, Top = MainWindow.Current.Top + 400, Owner = MainWindow.Current };
            StackPanel sp = new StackPanel { Margin = new Thickness(5) };
            var btns = tools.Select(x => new Button { Content = x.Key, Tag = x.Value }).ToList();
            btns.ForEach(x => x.Click += (s, args) => { PythonConsole.Current.RunString(x.Tag.ToString()); });
            btns.ForEach(x => sp.Children.Add(x));
            window.Content = sp;
            window.Show();
        }

        public static void player()
        {
            PythonPlayer pp = new PythonPlayer();
            pp.Left = MainWindow.Current.Left;
            pp.Top = MainWindow.Current.Top;
            pp.Show();
        }

        public static void calcfield(string layer, string fieldName, Func<IFeature, double> expr)
        {
            MapDataManager.LatestMap.Layers[layer].Features.ForEach(f => f.Properties[fieldName] = expr(f).ToString());
        }

        public static void query(string layer, Func<IFeature, bool> predicate)
        {
            // pycmd.query('区域',lambda x:System.Convert.ToDouble(x.Properties['全县地震点数'])>5)

            var entities = MapDataManager.LatestMap.Layers[layer].Features.Where(f => predicate(f)).ToArray();
            SelectionSet.Select(entities);
        }

        public static void find(string key)
        {
            // pycmd.find('当雄')

            var entities = MapDataManager.LatestMap.Layers.SelectMany(x => x.Features).Where(f => f.Properties.Any(x => x.Value.ToUpper().Contains(key.ToUpper()))).ToArray();
            SelectionSet.Select(entities);
        }

        public static void showprop()
        {
            QueryResultWindow qrw = new QueryResultWindow();
            qrw.SetData(SelectionSet.Contents);
            qrw.Owner = MainWindow.Current;
            qrw.ShowDialog();
        }

        public static void saveas()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Ciml (*.ciml)|*.ciml";
            if (sfd.ShowDialog() == true)
            {
                MapDataManager.SaveAs(sfd.FileName);
            }
        }

        public static void saveselection()
        {
            Map map = new Map();
            MapDataManager.LatestMap.Layers.ForEach(l =>
            {
                var layer = l as VectorLayer;
                var newLayer = new VectorLayer(layer.Name, layer.GeoType) { Code = layer.Code };
                layer.Features.Where(f => SelectionSet.Contents.Contains(f)).ForEach(f => newLayer.Features.Add(f));
                map.Layers.Add(newLayer);
            });
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "Ciml (*.ciml)|*.ciml";
            if (sfd.ShowDialog() == true)
            {
                map.Save(sfd.FileName);
            }
        }
    }

    public static class UserInput
    {
        public static Geometry.Vector? point(string tip)
        {
            PickPointTool pp = new PickPointTool();
            ViewerToolManager.AddTool(pp);
            CursorTipTool ct = new CursorTipTool(tip);
            ViewerToolManager.AddTool(ct);
            MapControl.Current.Cursor = System.Windows.Input.Cursors.Cross;

            while (true)
            {
                Dreambuild.Gis.Desktop.App.DoEvents();
                if (pp.Picked)
                {
                    break;
                }
            }

            ViewerToolManager.RemoveTool(ct);
            //MainWindow.Current.InitializeTool();
            ViewerToolManager.RemoveTool(pp);
            MapControl.Current.Cursor = System.Windows.Input.Cursors.Arrow;
            return pp.Point;
        }

        public static Extents extents(string tip)
        {
            PickExtentsTool pe = new PickExtentsTool();
            ViewerToolManager.AddTool(pe);
            CursorTipTool ct = new CursorTipTool(tip);
            ViewerToolManager.AddTool(ct);
            MapControl.Current.Cursor = System.Windows.Input.Cursors.Cross;

            while (true)
            {
                Dreambuild.Gis.Desktop.App.DoEvents();
                if (pe.Picked)
                {
                    break;
                }
            }

            ViewerToolManager.RemoveTool(ct);
            ViewerToolManager.RemoveTool(pe);
            MapControl.Current.Cursor = System.Windows.Input.Cursors.Arrow;
            return pe.Extents;
        }

        public static void pause(string tip = "")
        {
            bool isClosed = false;
            Window window = new Window { Width = 300, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.Manual, ShowInTaskbar = false, Topmost = true, WindowStyle = WindowStyle.ToolWindow, Title = "提示", Left = MainWindow.Current.Left + 100, Top = MainWindow.Current.Top + 400 };
            StackPanel sp = new StackPanel { Margin = new Thickness(5) };
            TextBlock tb = new TextBlock { Text = (tip == string.Empty ? "脚本执行已暂停。" : tip), TextWrapping = TextWrapping.Wrap };
            Button btn = new Button { Content = "单击或关闭窗口以继续执行", Margin = new Thickness(0, 5, 0, 0) };
            btn.Click += (s, args) => isClosed = true;
            window.Closed += (s, args) => isClosed = true;
            sp.Children.Add(tb);
            sp.Children.Add(btn);
            window.Content = sp;
            window.Show();
            while (true)
            {
                Dreambuild.Gis.Desktop.App.DoEvents();
                if (isClosed)
                {
                    break;
                }
            }
            window.Close();
        }

        public static int option(string tip, params string[] options)
        {
            Window window = new Window { Width = 300, SizeToContent = SizeToContent.Height, WindowStartupLocation = System.Windows.WindowStartupLocation.Manual, ShowInTaskbar = false, Topmost = true, WindowStyle = WindowStyle.ToolWindow, Title = "请选择", Left = MainWindow.Current.Left + 100, Top = MainWindow.Current.Top + 400, Owner = MainWindow.Current };
            StackPanel sp = new StackPanel { Margin = new Thickness(5) };
            TextBlock tb = new TextBlock { Text = (tip == string.Empty ? "请选择一个选项。" : tip), TextWrapping = TextWrapping.Wrap };
            int result = -1;
            var btns = options.Select((x, i) => new Button { Content = x, Tag = i }).ToList();
            btns.ForEach(x => x.Click += (s, args) => { result = (int)x.Tag; window.DialogResult = true; });
            sp.Children.Add(tb);
            btns.ForEach(x => sp.Children.Add(x));
            window.Content = sp;
            window.ShowDialog();
            return result;
        }
    }

    public static class GridAnalysis
    {
        public static double cellsize { get; set; }
        public static double spotsize { get; set; }
        public static Color spotcolor { get; set; }
        public static bool showgridline { get; set; }
        public static List<Geometry.Vector> grid { get; private set; }
        public static Func<Geometry.Vector, double> function { get; set; }

        public static Dictionary<Geometry.Vector, double> valuecache = new Dictionary<Geometry.Vector, double>();
        private static BiColorGradientTheme _colorTheme = new BiColorGradientTheme();
        private static VectorLayer _layer = new VectorLayer("grid", VectorLayer.GEOTYPE_REGION);

        public static List<IFeature> factors { get; private set; }
        public static Func<IFeature, Func<Geometry.Vector, double>> formula { get; set; }

        public static void init()
        {
            cellsize = 100;
            spotsize = 80;
            spotcolor = Colors.Blue;
            showgridline = true;
            function = p => 0; // p.x + p.y;
            factors = new List<IFeature>();
            formula = f => p => 0;
            openoraddspotlayer("spot");
        }

        public static void defaultfunction()
        {
            function = p => factors.Sum(f => formula(f)(p));
        }

        public static void factorlayers(params string[] layers)
        {
            factors = layers.SelectMany(x => MapDataManager.LatestMap.Layers[x].Features).ToList();
        }

        public static void defaultformula()
        {
            formula = f => p =>
            {
                if (f["ga-type"] == "default")
                {
                    var pos = f.GeoData[0];
                    double radius = f["ga-radius"].TryParseToDouble();
                    double mass = f["ga-mass"].TryParseToDouble();
                    double d = pos.Dist(p);
                    return (d > radius) ? 0 : mass * (1 - d / radius);
                }
                else if (f["ga-type"] == "road")
                {
                    var pos = new PointString(f.GeoData);
                    double radius = f["ga-radius"].TryParseToDouble();
                    double mass = f["ga-mass"].TryParseToDouble();
                    double d = pos.DistToPoint(p);
                    return (d > radius) ? 0 : mass * (1 - d / radius);
                }
                return 0d;
            };
        }

        public static void addprop(string layer, string prop, string defaultValue)
        {
            MapDataManager.LatestMap.Layers[layer].Features.ForEach(f =>
            {
                f[prop] = defaultValue;
            });
        }

        public static void readygrid()
        {
            UserInput.pause("即将建立网格。");
            int[] cellSizes = { 50, 100, 200 };
            int cellSizeOption = UserInput.option("选择网格大小。", "50x50", "100x100", "200x200");
            cellsize = cellSizes[cellSizeOption];

            defaultformula();
            factorlayers("spot", "道路");
            defaultfunction();
            if (UserInput.option("怎样确定分析范围？", "使用地图范围", "指定范围") == 0)
            {
                buildgrid(MapControl.Current.Map.GetExtents());
            }
            else
            {
                buildgrid(UserInput.extents("请指定分析范围。"));
            }
            applycolor();
        }

        public static void resetgrid()
        {
            applycolor();
            clearmark();
        }

        public static List<Geometry.Vector> buildgrid(Extents extents)
        {
            // 测试代码
            // ga.buildgrid(mc.MapExtents)

            //var parcels = DocumentManager.CurrentMap.Layers["地块"].Features.Select(f=>new Geometry.Polygon(f.GeoData)).ToList();
            grid = extents.GetGrid(cellsize); //.Where(p => parcels.Any(x => x.IsPointIn(p))).ToList();
            _layer = new VectorLayer("grid", VectorLayer.GEOTYPE_REGION);
            grid.ForEach(p => _layer.Features.Add(getcellfeature(p)));
            MapControl.Current.GridLayer.SetData(_layer);
            return grid;
        }

        public static void hidegrid()
        {
            MapControl.Current.GridLayer.ClearData();
        }

        private static IFeature getcellfeature(Geometry.Vector cell)
        {
            var p1 = cell.Add(new Geometry.Vector(cellsize / 2, cellsize / 2));
            var p2 = cell.Add(new Geometry.Vector(cellsize / 2, -cellsize / 2));
            var p3 = cell.Add(new Geometry.Vector(-cellsize / 2, -cellsize / 2));
            var p4 = cell.Add(new Geometry.Vector(-cellsize / 2, cellsize / 2));
            Feature f = new Feature(p1, p2, p3, p4);
            double value = function(cell);
            f.Properties["value"] = value.ToString();
            valuecache[cell] = value;
            return f;
        }

        public static void mincolor(byte r, byte g, byte b)
        {
            _colorTheme.MinColor = System.Windows.Media.Color.FromRgb(r, g, b);
        }

        public static void maxcolor(byte r, byte g, byte b)
        {
            _colorTheme.MaxColor = System.Windows.Media.Color.FromRgb(r, g, b);
        }

        public static void setspotcolor(byte r, byte g, byte b)
        {
            spotcolor = System.Windows.Media.Color.FromRgb(r, g, b);
        }

        public static void applycolor()
        {
            _colorTheme.Property = "value";
            _colorTheme.MaxValue = valuecache.Max(x => x.Value);
            _colorTheme.MinValue = valuecache.Min(x => x.Value);
            MapControl.Current.GridLayer.ApplyColorTheme(_colorTheme);
        }

        public static void applyspot()
        {
            MapControl.Current.MarkLayer.Children.Cast<System.Windows.Shapes.Ellipse>().ForEach(x =>
            {
                double cx = Canvas.GetLeft(x) + x.Width / 2;
                double cy = Canvas.GetTop(x) + x.Height / 2;
                x.Width = spotsize;
                x.Height = spotsize;
                Canvas.SetLeft(x, cx - spotsize / 2);
                Canvas.SetTop(x, cy - spotsize / 2);
                x.Fill = new SolidColorBrush(spotcolor);
            });
        }

        public static void clearcolor()
        {
            MapControl.Current.GridLayer.ClearColorTheme();
        }

        public static List<Geometry.Vector> range(double min, double max)
        {
            MapControl.Current.GridLayer.ApplyColorTheme(new PredicateTheme(f => f["value"].TryParseToDouble() >= min && f["value"].TryParseToDouble() <= max, System.Windows.Media.Colors.Yellow, System.Windows.Media.Colors.Gray));
            return valuecache.Where(x => x.Value >= min && x.Value <= max).Select(x => x.Key).ToList();
        }

        public static List<Geometry.Vector> getrange(double min, double max)
        {
            return valuecache.Where(x => x.Value >= min && x.Value <= max).Select(x => x.Key).ToList();
        }

        public static void markcell(Geometry.Vector p)
        {
            System.Windows.Shapes.Ellipse ell = new System.Windows.Shapes.Ellipse { Width = spotsize, Height = spotsize, Fill = new SolidColorBrush(spotcolor) };
            System.Windows.Controls.Canvas.SetLeft(ell, p.X - spotsize / 2);
            System.Windows.Controls.Canvas.SetTop(ell, p.Y - spotsize / 2);
            MapControl.Current.MarkLayer.Children.Add(ell);
        }

        public static void clearmark()
        {
            MapControl.Current.MarkLayer.Children.Clear();
        }

        private static MapLayer _spotLayer;

        public static void addspotlayer(string name = "spot")
        {
            VectorLayer layer = new VectorLayer(name, VectorLayer.GEOTYPE_POINT);
            MapDataManager.LatestMap.Layers.Add(layer);
            _spotLayer = MapControl.Current.AddLayer(layer);
        }

        public static void openoraddspotlayer(string name)
        {
            if (MapControl.Current.Layers.Any(x => x.LayerData.Name == name))
            {
                _spotLayer = MapControl.Current.Layers.First(x => x.LayerData.Name == name);
            }
            else
            {
                addspotlayer(name);
            }
        }

        public static IFeature addspot(Geometry.Vector pos)
        {
            Feature f = new Feature(pos);
            f["ga-type"] = "default";
            f["ga-radius"] = "500";
            f["ga-mass"] = "1000";
            _spotLayer.AddFeature(f);
            return f;
        }

        public static IFeature addspot(double x, double y)
        {
            return addspot(new Geometry.Vector(x, y));
        }

        public static void addspotui()
        {
            openoraddspotlayer("spot");
            while (true)
            {
                var pos = UserInput.point("指定插入点实体的位置，按Esc结束");
                if (!pos.HasValue)
                {
                    break;
                }
                addspot(pos.Value);
            }
        }

        public static void delspotui()
        {
            SelectionSet.Contents.Where(x => SelectionSet.FindLayer(x).Name == _spotLayer.LayerData.Name).ForEach(x => _spotLayer.RemoveFeature(x));
        }

        public static void addroadprops()
        {
            addprop("道路", "ga-type", "road");
            addprop("道路", "ga-radius", "500");
            addprop("道路", "ga-mass", "1000");
        }

        public static void mathtest()
        {
            function = p => p.X + p.Y;
            buildgrid(MapControl.Current.Map.GetExtents());
            var cells = range(98000, 99000);
            Random rand = new Random(0);
            var searches = Enumerable.Range(0, 1000).Select(x =>
            {
                var pts = Enumerable.Range(0, 4).Select(i => cells[rand.Next(0, cells.Count - 1)]).ToList();
                return System.Tuple.Create(pts, evaluate(pts, 98500));
            }).ToList();
            var mineval = searches.Min(y => y.Item2);
            var target = searches.First(x => x.Item2 == mineval);
            target.Item1.ForEach(p => markcell(p));
        }

        public static double evaluate(System.Collections.IEnumerable pts1, double targetValue)
        {
            var pts = pts1.Cast<Geometry.Vector>().ToList();

            double Ep = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                for (int j = i; j < pts.Count; j++)
                {
                    double deltaEp = 1 / (pts[i].Dist(pts[j]));
                    Ep += deltaEp;
                }
            }

            double Near = pts.Sum(x => Math.Pow(valuecache[x] - targetValue, 2));

            return 1 / (100 / Ep + 1 / Near);
        }
    }
}
