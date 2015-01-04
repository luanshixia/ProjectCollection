using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TongJi.City
{
    public class OptionsManager
    {
        private static OptionsManager _singleton = new OptionsManager();
        public static OptionsManager Singleton { get { return _singleton; } }

        private OptionsManager()
        {
        }

        private Dictionary<string, object> _options = new Dictionary<string, object>();
        public object this[string key]
        {
            get
            {
                if (_options.ContainsKey(key))
                {
                    return _options[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (_options.ContainsKey(key))
                {
                    _options[key] = value;
                }
                else
                {
                    _options.Add(key, value);
                }
            }
        }

        private const string _fileName = "CityGUI.options";
        public void Load()
        {
            string file = Utility.CurrentFolder + _fileName;
            string[] lines = System.IO.File.ReadAllLines(file).Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            foreach (string line in lines)
            {
                string[] ss = line.Split('=');
                if (ss.Length == 2)
                {
                    _options[ss[0].Trim()] = ss[1].Trim();
                }
            }
        }
        public void Save()
        {
            string file = Utility.CurrentFolder + _fileName;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
            {
                foreach (var entry in _options)
                {
                    sw.WriteLine("{0}={1}", entry.Key, entry.Value);
                }
            }
        }

        public void Foo(string text)
        {
            System.Windows.Forms.MessageBox.Show(text);
        }

        public Color ViewportBackground
        {
            get
            {
                return this["ViewportBackground"].ToString().ParseColor();
            }
            set
            {
                this["ViewportBackground"] = string.Format("{0},{1},{2}", value.R, value.G, value.B);
            }
        }

        public Color ViewportTextColor
        {
            get
            {
                return this["ViewportTextColor"].ToString().ParseColor();
            }
            set
            {
                this["ViewportTextColor"] = string.Format("{0},{1},{2}", value.R, value.G, value.B);
            }
        }

        public Color ViewportLineColor
        {
            get
            {
                return this["ViewportLineColor"].ToString().ParseColor();
            }
            set
            {
                this["ViewportLineColor"] = string.Format("{0},{1},{2}", value.R, value.G, value.B);
            }
        }

        public Color RoadColor
        {
            get
            {
                return this["RoadColor"].ToString().ParseColor();
            }
            set
            {
                this["RoadColor"] = string.Format("{0},{1},{2}", value.R, value.G, value.B);
            }
        }

        public Color MinValueColor
        {
            get
            {
                return this["MinValueColor"].ToString().ParseColor();
            }
            set
            {
                this["MinValueColor"] = string.Format("{0},{1},{2}", value.R, value.G, value.B);
            }
        }

        public Color MaxValueColor
        {
            get
            {
                return this["MaxValueColor"].ToString().ParseColor();
            }
            set
            {
                this["MaxValueColor"] = string.Format("{0},{1},{2}", value.R, value.G, value.B);
            }
        }

        public double MaxValue
        {
            get
            {
                return Convert.ToDouble(this["MaxValue"]);
            }
            set
            {
                this["MaxValue"] = value;
            }
        }

        public double MinValue
        {
            get
            {
                return Convert.ToDouble(this["MinValue"]);
            }
            set
            {
                this["MinValue"] = value;
            }
        }

        public AggregateType AggregateType
        {
            get
            {
                if (this["AggregateType"].ToString() == "Sum")
                {
                    return AggregateType.Sum;
                }
                else if (this["AggregateType"].ToString() == "Min")
                {
                    return AggregateType.Min;
                }
                else // (this["AggregateType"].ToString() == "Max")
                {
                    return AggregateType.Max;
                }
            }
            set
            {
                this["AggregateType"] = value.ToString();
            }
        }
    }

    public static class Utility
    {
        public static string CurrentFolder
        {
            get
            {
                string s = System.Reflection.Assembly.GetCallingAssembly().Location;
                return s.Remove(s.LastIndexOf('\\') + 1);
            }
        }

        public static void CreateLog(object o)
        {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("CityGui.log", true);
            sw.WriteLine(string.Format("[{0} {1}] {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), o));
            sw.Close();
        }

        public static System.Drawing.Color ParseColor(this string rgbString)
        {
            try
            {
                byte[] rgb = rgbString.Split(',').Select(x => Convert.ToByte(x)).ToArray();
                return System.Drawing.Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            }
            catch
            {
                return System.Drawing.Color.Black;
            }
        }

        public static void DispatchWin7Xp(Action win7, Action xp)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                win7();
            }
            else
            {
                xp();
            }
        }

        public static TResult[,] MapArray2d<TSource, TResult>(this TSource[,] source, Func<TSource, TResult> mapper)
        {
            TResult[,] result = new TResult[source.GetLength(0), source.GetLength(1)];
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    result[i, j] = mapper(source[i, j]);
                }
            }
            return result;
        }

        //public static int[,] IndexOfArray2d<T>(this T[,] source, T target)
        //{

        //}
    }

    /// <summary>
    /// 选择集
    /// </summary>
    public static class SSet
    {
        private static List<CityEntity> _contents = new List<CityEntity>();
        public static List<CityEntity> Contents { get { return _contents; } }

        public static void Select(CityEntity[] entities)
        {
            _contents.Clear();
            entities.ToList().ForEach(x => _contents.Add(x));
            PropertyGridHelper.Show(entities);
        }

        public static void Select(CityEntity entity)
        {
            _contents.Clear();
            _contents.Add(entity);
            PropertyGridHelper.Show(entity);
        }

        public static void AddSelection(CityEntity[] entities)
        {
            entities.ToList().ForEach(x => _contents.Add(x));
            PropertyGridHelper.Show(_contents.ToArray());
        }

        public static void AddSelection(CityEntity entity)
        {
            _contents.Add(entity);
            PropertyGridHelper.Show(_contents.ToArray());
        }

        public static void ClearSelection()
        {
            _contents.Clear();
            PropertyWindow.Current.Grid.SelectedObject = null;
        }
    }

    /// <summary>
    /// CityGUI交互式命令。函数名称全小写。
    /// </summary>
    public static class PyCmd
    {
        // ease access from python

        public static DisplayManager display { get { return DisplayManager.Current; } }
        public static AnalysisDistrict city { get { return display.CityModel; } }
        public static Viewer viewer { get { return Viewer.Current; } }

        // selection set

        public static List<CityEntity> sset { get { return SSet.Contents; } }
        public static CityEntity sset0 { get { return sset[0]; } }

        // ease functions

        public static void grade(params double[] divs)
        {
            divs = divs.OrderBy(x => x).ToArray();
            var mapper = new MultiColorGradientMapper();
            mapper.AddStop(divs[0], Color.Black);
            mapper.AddStop((divs[0] + divs.Last()) / 2, Color.Red);
            mapper.AddStop(divs.Last(), Color.White);
            var mapperLeveled = new LeveledColorDecorator(mapper, divs);
            display.ColorMapper = mapperLeveled;
        }

        public static List<IValueUnit> query(double min, double max)
        {
            return ValueBuffer.Units.Where(x => x.Value >= min && x.Value <= max).ToList();
        }

        public static void showquery(double min, double max)
        {
            DisplayManager.Current.ColorMapper = new LogicalColorMapper(x => x >= min && x <= max, Color.Yellow, Color.LightGray);
        }

        public static void levelcolor(double min, double max, int div)
        {
            DisplayManager.Current.ColorMapper = new BiColorLevelsMapper(min, max, div);
        }

        public static void visualizeparcels(string prop)
        {
            ValueBuffer.Units.Clear();
            city.Parcels.ForEach(x => ValueBuffer.Units.Add(new ParcelIndexUnit(x, prop)));
        }

        public static void scalespots(double scale)
        {
            sset.ForEach(x => (x as SpotEntity).Coefficient *= scale);
        }

        public static void scaleroads(double scale)
        {
            sset.ForEach(x => (x as CityRoad).Coefficient *= scale);
        }

        public static SpotEntity addspot(double x, double y)
        {
            var spot = new SpotEntity { Position = new Geometry.Point2D(x, y) };
            city.CitySpots.Add(spot);
            return spot;
        }

        public static void cleanpoly(double epsilon = 1)
        {
            foreach (var road in display.CityModel.Roads)
            {
                var points = road.Alignment.Points;
                var ready = new List<Geometry.Point2D>();
                Geometry.Point2D temp = points.First().Move(new Geometry.Vector2D(100, 100));
                foreach (var point in points)
                {
                    if (point.DistTo(temp) < epsilon)
                    {
                        ready.Add(point);
                    }
                    else
                    {
                        temp = point;
                    }
                }
                ready.ForEach(x => points.Remove(x));
            }
            foreach (var parcel in display.CityModel.Parcels)
            {
                var points = parcel.Domain.Points;
                var ready = new List<Geometry.Point2D>();
                Geometry.Point2D temp = points.First().Move(new Geometry.Vector2D(100, 100));
                foreach (var point in points)
                {
                    if (point.DistTo(temp) < epsilon)
                    {
                        ready.Add(point);
                    }
                    else
                    {
                        temp = point;
                    }
                }
                ready.ForEach(x => points.Remove(x));
            }
        }

        public static void factoron()
        {
            city.ToggleFactors(sset.ToArray(), true);
        }

        public static void factoroff()
        {
            city.ToggleFactors(sset.ToArray(), false);
        }

        public static void pixelboundtest(int i, int j = 6)
        {
            var values = ValueBuffer.Units.Select(x => x.Value).Distinct().ToArray();
            var cells = ValueBuffer.Units.Where(x => x.Value == values[i]).Select(x =>
            {
                var cell = x as GridCell;
                return new Point(cell.Col, cell.Row);
            }).ToList();
            Geometry.PixelBound pb = new Geometry.PixelBound(cells);
            var loop = pb.Trace()[0];
            var loopPoints = loop.Select(x => (ValueBuffer.GetGridCell(x.X, x.Y) as GridCell).Position).ToList();
            var spline = new Geometry.BSpline2D(loopPoints.Select(x => new Geometry.Point2D(x.x, x.y)).ToList());
            var splinePoints = Enumerable.Range(0, 5 * (loopPoints.Count - 1) + 1).Select(x => spline.SolveAt(x / 5.0)).ToArray();
            ViewerTool tool = new ViewerTool
            {
                PaintAction = () =>
                {
                    display.g.DrawPolygon(new Pen(Brushes.Black, 1), splinePoints.Select(x => display.CanvasCoordinateF(x)).ToArray());
                }
            };
            viewer.SetTool(tool);
        }

        public static void voronoi(int level = 5)
        {
            PyDraw.clear();
            PyDraw.fill(128, 128, 128);
            PyDraw.thickness(3);
            PyDraw.alpha(128);

            var coverRangeCells = ValueBuffer.GetCoverRangeGrid();
            var values = coverRangeCells.Select(x => x.Value).Distinct().ToArray();
            foreach (var value in values)
            {
                var binarizedPixels = ValueBuffer.GetBinarizedPixels(coverRangeCells.Cast<GridCell>().ToList(), x => x.Value == value);
                var pb = new Geometry.PixelBound(binarizedPixels);
                var loop = pb.Trace()[0];
                var loopPoints = loop.Select(x => (ValueBuffer.GetGridCell(coverRangeCells, x.X, x.Y) as GridCell).Position).ToList();
                loopPoints = Geometry.PixelBound.Align(loopPoints, level);

                PyDraw.cityspline(loopPoints.ToArray(), true);
            }
        }

        public static void contour(double step)
        {
            PyDraw.clear();
            //PyDraw.fill(128, 255, 128);
            //PyDraw.stroke(96, 255, 96);
            //PyDraw.thickness(1);
            //PyDraw.alpha(48);

            var gridCells = ValueBuffer.GetGrid();
            var values = Functional.Seq.range(0, gridCells.Max(x => x.Value), step).ToArray();
            var cityLoops = new List<List<Geometry.Point2D>>();
            foreach (var value in values)
            {
                var binarizedPixels = ValueBuffer.GetBinarizedPixels(gridCells.Cast<GridCell>().ToList(), x => x.Value >= value);
                var pb = new Geometry.PixelBound(binarizedPixels);
                var loops = pb.Trace();
                foreach (var loop in loops)
                {
                    var loopPoints = loop.Select(x => (ValueBuffer.GetGridCell(gridCells, x.X, x.Y) as GridCell).Position).ToList();
                    loopPoints = Geometry.PixelBound.Align(loopPoints, 5);

                    //PyDraw.cityspline(loopPoints.ToArray(), true);
                    //loopPoints.ForEach(p => PyDraw.cityellipse(p.x, p.y, 1, 1));
                    cityLoops.Add(loopPoints);
                }
            }
            ViewerTool tool = new ViewerTool
            {
                PaintAction = () =>
                {
                    cityLoops.ForEach(loop =>
                    {
                        var pts = loop.Select(p => display.CanvasCoordinateF(p)).ToList();
                        pts.ForEach(p => display.g.FillEllipse(new SolidBrush(Color.Black), p.X, p.Y, 3, 3));
                    });
                }
            };
            viewer.SetTool(tool);
        }

        public static void testclosedfill()
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    PyDraw.cityellipse(i, j, 1, 1);
                }
            }
        }
    }

    /// <summary>
    /// CityGUI交互式绘图API。函数名称全小写。
    /// </summary>
    public static class PyDraw
    {
        private static Color _stroke = Color.Black;
        private static Color _fill = Color.White;
        private static float _strokeThickness = 1;
        private static float[] _strokeDashPattern = { 1 };
        private static Font _font = new Font(new FontFamily("Tahoma"), 10, FontStyle.Regular);

        // setup functions

        public static void defaultsettings()
        {
            _stroke = Color.Black;
            _fill = Color.White;
            _strokeThickness = 1;
            _strokeDashPattern = new float[] { 1 };
            _font = new Font(new FontFamily("Tahoma"), 10, FontStyle.Regular);
        }

        public static void stroke(Color c)
        {
            _stroke = c;
        }

        public static void stroke(byte r, byte g, byte b)
        {
            _stroke = Color.FromArgb(r, g, b);
        }

        public static void fill(Color c)
        {
            _fill = c;
        }

        public static void fill(byte r, byte g, byte b)
        {
            _fill = Color.FromArgb(r, g, b);
        }

        public static void alpha(byte a)
        {
            _fill = Color.FromArgb(a, _fill.R, _fill.G, _fill.B);
            _stroke = Color.FromArgb(a, _stroke.R, _stroke.G, _stroke.B);
        }

        public static void thickness(float t)
        {
            _strokeThickness = t;
        }

        public static void dashpattern(params float[] pattern)
        {
            _strokeDashPattern = pattern;
        }

        public static void fontfamily(string family)
        {
            _font = new Font(new FontFamily(family), _font.Size, _font.Style);
        }

        public static void fontsize(float size)
        {
            _font = new Font(_font.FontFamily, size, _font.Style);
        }

        public static void fontstyle(FontStyle style)
        {
            _font = new Font(_font.FontFamily, _font.Size, style);
        }

        // draw functions

        public static void ellipse(float x, float y, float width, float height)
        {
            Brush b = new SolidBrush(_fill);
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            RectangleF rect = new RectangleF(x, y, width, height);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                g.FillEllipse(b, rect);
                g.DrawEllipse(p, rect);
            });
        }

        public static void cityellipse(double x, double y, double width, double height)
        {
            Brush b = new SolidBrush(_fill);
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                float w = (float)(width / DisplayManager.Current.Scale);
                float h = (float)(height / DisplayManager.Current.Scale);
                var pos = DisplayManager.Current.CanvasCoordinateF(new Geometry.Point2D(x, y));
                RectangleF rect = new RectangleF(pos, new SizeF(w, h));
                g.FillEllipse(b, rect);
                g.DrawEllipse(p, rect);
            });
        }

        public static void rectangle(float x, float y, float width, float height)
        {
            Brush b = new SolidBrush(_fill);
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            RectangleF rect = new RectangleF(x, y, width, height);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                g.FillRectangle(b, rect);
                g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
            });
        }

        public static void cityrectangle(double x, double y, double width, double height)
        {
            Brush b = new SolidBrush(_fill);
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                float w = (float)(width / DisplayManager.Current.Scale);
                float h = (float)(height / DisplayManager.Current.Scale);
                var pos = DisplayManager.Current.CanvasCoordinateF(new Geometry.Point2D(x, y));
                RectangleF rect = new RectangleF(pos, new SizeF(w, h));
                g.FillRectangle(b, rect);
                g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
            });
        }

        public static void line(float x1, float y1, float x2, float y2)
        {
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                g.DrawLine(p, x1, y1, x2, y2);
            });
        }

        public static void cityline(double x1, double y1, double x2, double y2)
        {
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                var p1 = DisplayManager.Current.CanvasCoordinateF(new Geometry.Point2D(x1, y1));
                var p2 = DisplayManager.Current.CanvasCoordinateF(new Geometry.Point2D(x2, y2));
                g.DrawLine(p, p1, p2);
            });
        }

        public static void polyline(PointF[] points, bool closed = false)
        {
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            Brush b = new SolidBrush(_fill);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                g.FillPolygon(b, points);
                if (closed)
                {
                    g.DrawPolygon(p, points);
                }
                else
                {
                    g.DrawLines(p, points);
                }
            });
        }

        public static void citypolyline(Geometry.Point2D[] points, bool closed = false)
        {
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            Brush b = new SolidBrush(_fill);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                var pts = points.Select(x => DisplayManager.Current.CanvasCoordinateF(x)).ToArray();
                g.FillPolygon(b, pts);
                if (closed)
                {
                    g.DrawPolygon(p, pts);
                }
                else
                {
                    g.DrawLines(p, pts);
                }
            });
        }

        public static void spline(PointF[] points, bool closed = false)
        {
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            Brush b = new SolidBrush(_fill);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                g.FillClosedCurve(b, points);
                if (closed)
                {
                    g.DrawClosedCurve(p, points);
                }
                else
                {
                    g.DrawCurve(p, points);
                }
            });
        }

        public static void cityspline(Geometry.Point2D[] points, bool closed = false)
        {
            Pen p = new Pen(new SolidBrush(_stroke), _strokeThickness) { DashPattern = _strokeDashPattern };
            Brush b = new SolidBrush(_fill);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                var pts = points.Select(x => DisplayManager.Current.CanvasCoordinateF(x)).ToArray();
                g.FillClosedCurve(b, pts);
                if (closed)
                {
                    g.DrawClosedCurve(p, pts);
                }
                else
                {
                    g.DrawCurve(p, pts);
                }
            });
        }

        public static void text(string content, float x, float y)
        {
            Brush b = new SolidBrush(_fill);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                g.DrawString(content, _font, b, new PointF(x, y));
            });
        }

        public static void citytext(string content, double x, double y)
        {
            Brush b = new SolidBrush(_fill);
            DisplayManager.Current.AdditionalPaintings.Add(g =>
            {
                var point = DisplayManager.Current.CanvasCoordinateF(new Geometry.Point2D(x, y));
                g.DrawString(content, _font, b, point);
            });
        }

        public static void clear()
        {
            DisplayManager.Current.AdditionalPaintings.Clear();
        }
    }

    public static class PropertyGridHelper
    {
        public static void Show(CityEntity entity)
        {
            if (Viewer.Current.tsbtnInfo.Checked)
            {
                PropertyWindow.Current.Grid.SelectedObject = new CityPropertiesWrapper(entity);
            }
            else
            {
                PropertyWindow.Current.Grid.SelectedObject = entity;
            }
        }

        public static void Show(CityEntity[] entities)
        {
            if (Viewer.Current.tsbtnInfo.Checked)
            {
                PropertyWindow.Current.Grid.SelectedObjects = entities.Select(x => new CityPropertiesWrapper(x)).ToArray();
            }
            else
            {
                PropertyWindow.Current.Grid.SelectedObjects = entities;
            }
        }
    }
}
