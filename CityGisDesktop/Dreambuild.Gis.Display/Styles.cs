using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// 矢量样式
    /// </summary>
    public class VectorStyle
    {
        public double StrokeWeight = 1;
        public Brush Stroke = new SolidColorBrush(Colors.Gray); //new Color { A = 255, R = 64, G = 64, B = 32 });
        public DoubleCollection StrokeDashArray;
        public Brush Fill = new SolidColorBrush(new Color { A = 255, R = 255, G = 255, B = 180 }); //{ A = 255, R = 180, G = 200, B = 150 });
        public double SpotSize = 20;
        public double FontSize = 20;
        public Brush FontBrush = new SolidColorBrush(Colors.Black);

        private Feature _emptyFeature = new Feature();
        public IColorTheme FillTheme;
        public Brush GetFill(IFeature feature)
        {
            if (FillTheme != null)
            {
                return new SolidColorBrush(FillTheme.GetColor(feature));
            }
            else
            {
                return Fill;
            }
        }

        public Brush GetFill()
        {
            return GetFill(_emptyFeature);
        }
    }

    /// <summary>
    /// 标签样式
    /// </summary>
    public class LabelStyle
    {
        public string Format { get; private set; }
        public string[] PropNames { get; private set; }
        public double LinearRepeatInterval = 1000;

        public LabelStyle(string format, params string[] props)
        {
            Format = format;
            PropNames = props;
        }

        public string GetLable(IFeature feature)
        {
            var values = PropNames.Select(x => feature[x]).ToArray();
            return string.Format(Format, values);
        }
    }

    /// <summary>
    /// 矢量样式管理
    /// </summary>
    public static class VectorStyleManager
    {
        public static Dictionary<string, VectorStyle> Styles { get; private set; }
        public static Dictionary<string, LabelStyle> LabelStyles { get; private set; }

        static VectorStyleManager()
        {
            Styles = new Dictionary<string, VectorStyle>();
            foreach (XElement xe in CityGisConfig.XValue.Elements("LayerStyles"))
            {
                foreach (var element in xe.Elements())
                {
                    string layerName = element.AttValue("Layer");

                    VectorStyle vs = new VectorStyle();
                    vs.StrokeWeight = Convert.ToDouble(element.AttValue("StrokeWeight"));
                    vs.SpotSize = Convert.ToDouble(element.AttValue("SpotSize"));
                    vs.FontSize = Convert.ToDouble(element.AttValue("FontSize"));

                    var strokeString = element.AttValue("Stroke");
                    if (strokeString.StartsWith("#"))
                    {
                        Color stroke = VectorStyleManager.ParseColor(element.AttValue("Stroke"));
                        vs.Stroke = new SolidColorBrush(stroke);
                    }
                    else if (strokeString == "{ClTheme}")
                    {
                    }
                    var strokeDashString = element.AttValue("StrokeDashArray");
                    if (!string.IsNullOrEmpty(strokeDashString))
                    {
                        vs.StrokeDashArray = new DoubleCollection();
                        strokeDashString.Split(',').ForEach(x => vs.StrokeDashArray.Add(x.TryParseToDouble()));
                    }

                    var fillString = element.AttValue("Fill");
                    if (fillString.StartsWith("#"))
                    {
                        vs.FillTheme = new SingleColorTheme(VectorStyleManager.ParseColor(fillString));
                        Color fill = VectorStyleManager.ParseColor(fillString);
                        vs.Fill = new SolidColorBrush(fill);
                    }
                    else if (fillString == "{MapTheme}")
                    {
                        vs.FillTheme = new MapTheme();
                    }
                    else if (fillString == "{ClTheme}")
                    {
                        vs.FillTheme = new ControlLineTheme();
                    }
                    else if (fillString == "{UnitTheme}")
                    {
                        vs.FillTheme = new UnitTheme();
                    }

                    Color fontbrush = VectorStyleManager.ParseColor(element.AttValue("FontBrush"));
                    vs.FontBrush = new SolidColorBrush(fontbrush);

                    Styles.Add(layerName, vs);
                }
            }

            LabelStyles = new Dictionary<string, LabelStyle>();
            foreach (XElement xe in CityGisConfig.XValue.Elements("LabelStyles"))
            {
                foreach (var element in xe.Elements())
                {
                    string layerName = element.AttValue("Layer");

                    string format = element.AttValue("Format").Replace("\\n", "\n");
                    string linearRepeatInterval = element.AttValue("LinearRepeatInterval");
                    double interval = 1000;
                    if (linearRepeatInterval != string.Empty)
                    {
                        interval = Convert.ToDouble(linearRepeatInterval);
                    }
                    string fields = element.AttValue("Fields");
                    LabelStyle ls = new LabelStyle(format, fields.Split('|'));
                    ls.LinearRepeatInterval = interval;

                    LabelStyles.Add(layerName, ls);
                }
            }
        }

        public static Color ParseColor(string colorName)
        {
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
            return new Color()
            {
                A = colorName.Length == 8 ? Convert.ToByte((v >> 24) & 255) : (byte)255,
                R = Convert.ToByte((v >> 16) & 255),
                G = Convert.ToByte((v >> 8) & 255),
                B = Convert.ToByte((v >> 0) & 255)
            };
        }

        public static void UpdateWithMapSpecific()
        {
        }
    }
    
    /// <summary>
    /// 数据可视化主题接口
    /// </summary>
    public interface IDataVisualizationTheme
    {
        string Property { get; set; }
    }

    /// <summary>
    /// 颜色主题接口
    /// </summary>
    public interface IColorTheme
    {
        Color GetColor(IFeature feature);
    }

    /// <summary>
    /// 数据颜色主题接口
    /// </summary>
    //public interface IDataColorTheme : IDataVisualizationTheme, IColorTheme
    //{
    //}

    /// <summary>
    /// 数据大小主题接口
    /// </summary>
    public interface IDataSizeTheme : IDataVisualizationTheme
    {
        double GetSize(IFeature feature);
    }

    /// <summary>
    /// 数据流体主题接口
    /// </summary>
    public interface IDataFluidTheme : IDataVisualizationTheme
    {
        double GetVelocity(IFeature feature);
        double GetDiameter(IFeature feature);
        double GetDensity(IFeature feature);
        Tuple<Color, Color> GetColor(IFeature feature);
    }

    public abstract class ColorThemeBase : IColorTheme // newly 20140702
    {
        public abstract double GetValue(IFeature f);

        public abstract Color GetColorByValue(double value);

        public virtual Color GetColor(IFeature f)
        {
            return GetColorByValue(GetValue(f));
        }
    }

    public abstract class DataColorThemeBase : ColorThemeBase, IDataVisualizationTheme
    {
        public string Property { get; set; }

        public override double GetValue(IFeature f)
        {
            return f[Property].TryParseToDouble();
        }
    }

    /// <summary>
    /// 断言着色
    /// </summary>
    public class PredicateTheme : IColorTheme
    {
        protected Func<IFeature, bool> _predicate;
        protected Color _trueColor;
        protected Color _falseColor;

        public PredicateTheme(Func<IFeature, bool> predicate, Color trueColor, Color falseColor)
        {
            _predicate = predicate;
            _trueColor = trueColor;
            _falseColor = falseColor;
        }

        public Color GetColor(IFeature feature)
        {
            return _predicate(feature) ? _trueColor : _falseColor;
        }
    }

    /// <summary>
    /// 多条件着色
    /// </summary>
    public class MultiConditionTheme : IColorTheme
    {
        protected List<Tuple<Func<IFeature, bool>, Color>> _conditions = new List<Tuple<Func<IFeature, bool>, Color>>();

        public void AddCondition(Func<IFeature, bool> condition, Color color)
        {
            _conditions.Add(Tuple.Create(condition, color));
        }

        public Color GetColor(IFeature feature)
        {
            foreach (var condition in _conditions)
            {
                if (condition.Item1(feature))
                {
                    return condition.Item2;
                }
            }
            return Colors.Transparent;
        }
    }

    /// <summary>
    /// 单色着色
    /// </summary>
    public class SingleColorTheme : IColorTheme
    {
        public Color Color { get; private set; }

        public SingleColorTheme(Color color)
        {
            Color = color;
        }

        public Color GetColor(IFeature feature)
        {
            return Color;
        }
    }

    /// <summary>
    /// 地图着色
    /// </summary>
    public class MapTheme : IColorTheme
    {
        readonly Random rand = new Random(0);

        public Color GetColor(IFeature feature)
        {
            byte r = (byte)rand.Next(100, 200);
            byte g = (byte)rand.Next(100, 200);
            byte b = (byte)rand.Next(0, 100);
            return Color.FromArgb(255, r, g, b);
        }
    }

    /// <summary>
    /// 用地性质着色
    /// </summary>
    public class ParcelUsageTheme : MultiConditionTheme
    {
        public Dictionary<string, Color> DictColor { get; private set; }

        public ParcelUsageTheme()
        {
            DictColor = new Dictionary<string, Color>();
            foreach (var entry in ParcelColorCfg.GetParcelColor())
            {
                Color color = VectorStyleManager.ParseColor(entry.Value);
                if (entry.Key == "_")
                {
                    this.AddCondition(f => true, color);
                    DictColor.Add("其余", color);
                }
                else
                {
                    string key1 = entry.Key;
                    string key2 = ParcelColorCfg.GetParcelUsage().First(f => f.Key == key1).Value;
                    string key = string.Format("{0}:{1}", key1, key2);
                    DictColor.Add(key, color);
                    this.AddCondition(f => f["用地代码"].StartsWith(key1), color);
                }
            }
        }
    }

    /// <summary>
    /// 控制线着色 newly 20130327
    /// </summary>
    public class ControlLineTheme : MultiConditionTheme
    {
        public ControlLineTheme()
        {
            this.AddCondition(f => f["类型"] == "红线", GetColorWithAlpha(128, Colors.Magenta));
            this.AddCondition(f => f["类型"] == "橙线", GetColorWithAlpha(128, Colors.Orange));
            this.AddCondition(f => f["类型"] == "蓝线", GetColorWithAlpha(128, Colors.Blue));
            this.AddCondition(f => f["类型"] == "绿线", GetColorWithAlpha(128, Colors.Green));
            this.AddCondition(f => f["类型"] == "紫线", GetColorWithAlpha(128, Colors.Purple));
            this.AddCondition(f => f["类型"] == "黄线", GetColorWithAlpha(128, Colors.Yellow));
            this.AddCondition(f => f["类型"] == "黑线", GetColorWithAlpha(128, Colors.Black));
            this.AddCondition(f => f["类型"] == "青线", GetColorWithAlpha(128, Colors.Cyan));
        }

        public static Color GetColorWithAlpha(byte alpha, Color color)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }
    }

    /// <summary>
    /// 单元着色 newly 20130328
    /// </summary>
    public class UnitTheme : IColorTheme
    {
        readonly Random rand = new Random(0);

        public Color GetColor(IFeature feature)
        {
            byte r = (byte)rand.Next(200, 255);
            byte g = (byte)rand.Next(200, 255);
            byte b = (byte)rand.Next(200, 255);
            return Color.FromArgb(128, r, g, b);
        }
    }

    /// <summary>
    /// double的数据大小主题
    /// </summary>
    public class DoubleDataSizeTheme : IDataSizeTheme
    {
        public string Property { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double MinSize { get; set; }
        public double MaxSize { get; set; }

        public DoubleDataSizeTheme()
        {
            MinSize = 10;
            MaxSize = 50;
        }

        public double GetSize(IFeature feature)
        {
            double value = 0;
            double.TryParse(feature[Property], out value);
            if (value > MaxValue)
            {
                return MaxSize;
            }
            else if (value < MinValue)
            {
                return MinSize;
            }
            return (value - MinValue) / (MaxValue - MinValue) * (MaxSize - MinSize) + MinSize;
        }
    }

    /// <summary>
    /// 枚举的数据大小主题
    /// </summary>
    public class EnumDataSizeTheme : IDataSizeTheme
    {
        public string Property { get; set; }
        public Dictionary<string, double> Sizes { get; private set; }

        public EnumDataSizeTheme()
        {
            Sizes = new Dictionary<string, double>();
        }

        public double GetSize(IFeature feature)
        {
            string value = feature[Property];
            return Sizes[value];
        }
    }

    /// <summary>
    /// 双色渐变着色
    /// </summary>
    public class BiColorGradientTheme : DataColorThemeBase
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public Color MinColor { get; set; }
        public Color MaxColor { get; set; }

        public BiColorGradientTheme()
        {
            MinColor = new Color { A = 255, B = 255, G = 255, R = 255 };
            MaxColor = new Color { A = 255, B = 0, G = 94, R = 0 };
        }

        public BiColorGradientTheme(string property, double min, double max)
            : this()
        {
            Property = property;
            MinValue = min;
            MaxValue = max;
        }

        public override Color GetColorByValue(double value)
        {
            if (value > MaxValue)
            {
                value = MaxValue;
            }
            else if (value < MinValue)
            {
                value = MinValue;
            }

            double interval = MaxValue - MinValue;
            double proportion = (value - MinValue) / interval;

            double red = (double)MinColor.R - ((double)MinColor.R - (double)MaxColor.R) * proportion;
            double green = (double)MinColor.G - ((double)MinColor.G - (double)MaxColor.G) * proportion;
            double blue = (double)MinColor.B - ((double)MinColor.B - (double)MaxColor.B) * proportion;

            return new Color { A = 255, R = (byte)red, G = (byte)green, B = (byte)blue };
        }
    }

    /// <summary>
    /// 多色渐变着色
    /// </summary>
    public class MultiColorGradientTheme : DataColorThemeBase
    {
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        private SortedDictionary<double, Color> _stops = new SortedDictionary<double, Color>();
        public SortedDictionary<double, Color> Stops { get { return _stops; } }

        public MultiColorGradientTheme(string property, double min, double max)
        {
            Property = property;
            MinValue = min;
            MaxValue = max;
            AddStop(0.0, Colors.White);
            AddStop(1.0, Colors.Black);
        }

        public void AddStop(double pos, Color color)
        {
            _stops.Add(pos, color);
        }

        public override Color GetColorByValue(double value)
        {
            double pos = (value - MinValue) / (MaxValue - MinValue);
            double[] posArr = _stops.Keys.ToArray();
            if (pos < posArr.First())
            {
                return _stops.First().Value;
            }
            else
            {
                for (int i = 0; i < posArr.Length - 1; i++)
                {
                    double a = posArr[i];
                    double b = posArr[i + 1];
                    if (pos >= a && pos < b)
                    {
                        var bcgm = new BiColorGradientTheme { MinValue = a, MaxValue = b, MinColor = _stops[a], MaxColor = _stops[b] };
                        return bcgm.GetColorByValue(pos);
                    }
                }
                return _stops.Last().Value;
            }
        }
    }

    /// <summary>
    /// 分级着色
    /// </summary>
    public class GradeTheme : DataColorThemeBase
    {
        public MultiColorGradientTheme InnerTheme { get; private set; }
        private List<double> _borders;

        public GradeTheme(string property, IEnumerable<double> borders)
        {
            Property = property;
            _borders = borders.ToList();
            double min = 1;
            double max = borders.Count() - 1;
            InnerTheme = new MultiColorGradientTheme(property, min, max);
            InnerTheme.AddStop((min + max) / 2, Colors.Red);
        }

        public override Color GetColorByValue(double value)
        {
            if (value < _borders[0])
            {
                return InnerTheme.GetColorByValue(1);
            }
            for (int i = 1; i < _borders.Count; i++)
            {
                if (_borders[i - 1] <= value && value < _borders[i])
                {
                    return InnerTheme.GetColorByValue(i);
                }
            }
            return InnerTheme.GetColorByValue(_borders.Count - 1);
        }
    }

    /// <summary>
    /// 简单流体主题
    /// </summary>
    public class SimpleFluidTheme : IDataFluidTheme
    {
        public string Property { get; set; }
        public string StartColorProperty { get; protected set; }
        public string EndColorProperty { get; protected set; }
        public BiColorGradientTheme InnerColorTheme { get; protected set; }

        public double Velocity = 20;
        public double Diameter = 7;
        public double Density = 1 / 12.0;

        public SimpleFluidTheme()
        {
            InnerColorTheme = new BiColorGradientTheme("", 0, 1);
            //InnerColorTheme.MinColor = Color.FromRgb(255, 150, 0); //Colors.Peru; //Colors.SaddleBrown;
            //InnerColorTheme.MaxColor = Color.FromRgb(77, 216, 233); //Colors.Cyan;
        }

        public SimpleFluidTheme(string startProp, string endProp, BiColorGradientTheme colorTheme)
        {
            StartColorProperty = startProp;
            EndColorProperty = endProp;
            InnerColorTheme = colorTheme;
        }

        public virtual double GetVelocity(IFeature f)
        {
            return Velocity; // 200
        }

        public virtual double GetDiameter(IFeature f)
        {
            return Diameter; // 20
        }

        public virtual double GetDensity(IFeature f)
        {
            return Density; // 100
        }

        public virtual Tuple<Color, Color> GetColor(IFeature f)
        {
            if (StartColorProperty != null)
            {
                return Tuple.Create(InnerColorTheme.GetColorByValue(f[StartColorProperty].TryParseToDouble()), InnerColorTheme.GetColorByValue(f[EndColorProperty].TryParseToDouble()));
            }
            else
            {
                return Tuple.Create(InnerColorTheme.GetColorByValue(0), InnerColorTheme.GetColorByValue(1));
            }
        }
    }

    /// <summary>
    /// 点实体的在位柱状图
    /// </summary>
    public class SpotChart
    {
        public double Width { get; protected set; }
        public double Height { get; protected set; }
        public List<SpotChartColumn> Columns { get; protected set; }

        public SpotChart(double width, double height)
        {
            Width = width;
            Height = height;
            Columns = new List<SpotChartColumn>();
        }
    }

    /// <summary>
    /// 点实体在位柱状图的数据列
    /// </summary>
    public class SpotChartColumn
    {
        public string Property { get; set; }
        public double MaxValue { get; set; }
        public Color Color { get; set; }
        public SpotChart Chart { get; set; }
        public Func<IFeature, double> ValueSelector { get; set; }

        public SpotChartColumn(SpotChart chart, string prop, double maxVal, Color color)
        {
            Chart = chart;
            Property = prop;
            MaxValue = maxVal;
            Color = color;
            ValueSelector = f => f[Property].TryParseToDouble();
        }

        public Size GetSize(IFeature f)
        {
            return new Size(Chart.Width / Chart.Columns.Count, (ValueSelector(f) / MaxValue) * Chart.Height);
        }

        public Rectangle GetShape(IFeature f, int i)
        {
            Geometry.Vector p = new Geometry.PointString(f.GeoData).Centroid();
            Point pos = new Point(p.X, p.Y);
            Size size = GetSize(f);
            double x = pos.X - Chart.Width / 2 + i * size.Width;
            double y = pos.Y;
            Rectangle rect = new Rectangle { Width = size.Width, Height = size.Height, Fill = new SolidColorBrush(Color), Stroke = null };
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            ToolTipService.SetToolTip(rect, UIHelper.TitledToolTip(Property, ValueSelector(f).ToString()));
            return rect;
        }

        public GeometryDrawing GetDrawing(IFeature f, int i)
        {
            Geometry.Vector p = new Geometry.PointString(f.GeoData).GetExtents().Center();
            Point pos = new Point(p.X, p.Y);
            Size size = GetSize(f);
            double x = pos.X - Chart.Width / 2 + i * size.Width;
            double y = pos.Y;
            RectangleGeometry geometry = new RectangleGeometry(new Rect(new Point(x, y), size));
            GeometryDrawing drawing = new GeometryDrawing(new SolidColorBrush(Color), null, geometry);
            return drawing;
        }
    }

    /// <summary>
    /// 点实体的连接图
    /// </summary>
    public class SpotLink
    {
    }

    /// <summary>
    /// 点实体的连接图的记录
    /// </summary>
    public class SpotLinkRecord
    {

    }

    /// <summary>
    /// 地块颜色配置
    /// </summary>
    public static class ParcelColorCfg
    {
        public static Dictionary<string, string> ParcelUsage2011;
        public static Dictionary<string, string> ParcelColor2011;
        public static Dictionary<string, string> ParcelUsage;
        public static Dictionary<string, string> ParcelColor;

        static ParcelColorCfg()
        {
            var data1 = System.IO.File.ReadAllLines("Configs\\LandUse.cfg").Select(x => x.Split(',')).ToArray();
            var data2 = System.IO.File.ReadAllLines("Configs\\LandUseOld.cfg").Select(x => x.Split(',')).ToArray();
            ParcelUsage2011 = data1.ToDictionary(x => x[0], x => x[1]);
            ParcelColor2011 = data1.ToDictionary(x => x[0], x => x[2]);
            ParcelUsage = data2.ToDictionary(x => x[0], x => x[1]);
            ParcelColor = data2.ToDictionary(x => x[0], x => x[2]);
        }

        public static string GetUsageByCode(string code) // newly 20130222
        {
            var dict = GetParcelUsage();
            if (dict.ContainsKey(code))
            {
                return dict[code];
            }
            else
            {
                return string.Empty;
            }
        }

        public static Dictionary<string, string> GetParcelUsage()
        {
            if (MapDataManager.IsNewGB)
            {
                return ParcelColorCfg.ParcelUsage2011;
            }
            else
            {
                return ParcelColorCfg.ParcelUsage;
            }
        }

        public static Dictionary<string, string> GetParcelColor()
        {
            if (MapDataManager.IsNewGB)
            {
                return ParcelColorCfg.ParcelColor2011;
            }
            else
            {
                return ParcelColorCfg.ParcelColor;
            }
        }
    }

    /// <summary>
    /// 配置文件
    /// </summary>
    public static class CityGisConfig
    {
        public static XElement XValue { get; set; }

        static CityGisConfig()
        {
            XValue = XDocument.Load("Configs\\CityWebGis.cfg").Root;
        }
    }
}
