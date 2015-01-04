using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using TongJi.Svg;

namespace TongJi.Gis
{
    public class SvgMapRenderer
    {
        private SvgWriter _svgWriter = new SvgWriter();
        private Config.MapConfig _mapConfig;
        private Map _map;
        private Dictionary<string, Func<IFeature, VectorStyle>> _themeFilters = new Dictionary<string, Func<IFeature, VectorStyle>>();
        private Dictionary<int, Action> _overlayRenderProcedures = new Dictionary<int, Action>();
        private Geometry.Extent2D _extents;
        private Func<double, double> _calcx = x => x;
        private Func<double, double> _calcy = y => y;
        private Func<double, double> _scale = x => x;

        public SvgMapRenderer(Map map, Config.MapConfig config)
        {
            _map = map;
            _mapConfig = config;
            _extents = map.GetExtents();
        }

        public void SetThemeFilter(string layer, Func<IFeature, VectorStyle> themeFunction)
        {
            _themeFilters[layer] = themeFunction;
        }

        public int GetLayerOrder(string layerName)
        {
            var layer = GetLayerConfigByName(layerName);
            if (layer != null)
            {
                return layer.Order;
            }
            else
            {
                return 0;
            }
        }

        private static void SetVectorStyles(SvgWriter writer, Config.Layer layerConfig)
        {
            if (layerConfig != null)
            {
                SetVectorStyles(writer, layerConfig.VectorStyle);
            }
            else
            {
                SetVectorStyles(writer, new VectorStyle());
            }
        }

        private static void SetTextStyles(SvgWriter writer, Config.Layer layerConfig)
        {
            if (layerConfig != null)
            {
                SetTextStyles(writer, layerConfig.TextStyle);
            }
            else
            {
                SetTextStyles(writer, new TextStyle());
            }
        }

        private static void SetVectorStyles(SvgWriter writer, VectorStyle vectorStyle)
        {
            writer.SetStroke(vectorStyle.Stroke);
            writer.SetStrokeWidth(vectorStyle.StrokeWidth);
            writer.SetStrokeOpacity(vectorStyle.StrokeOpacity);
            writer.SetStrokeLineCap(vectorStyle.StrokeLineCap);
            writer.SetStrokeLineJoin(vectorStyle.StrokeLineJoin);
            writer.SetStrokeDashArray(vectorStyle.StrokeDashArray);
            writer.SetFill(vectorStyle.Fill);
            writer.SetFillOpacity(vectorStyle.FillOpacity);
            writer.SetFillRule(vectorStyle.FillRule);
        }

        private static void SetTextStyles(SvgWriter writer, TextStyle textStyle)
        {
            writer.SetStroke(textStyle.Stroke);
            writer.SetStrokeWidth(textStyle.StrokeWidth);
            writer.SetStrokeOpacity(null);
            writer.SetStrokeLineCap(null);
            writer.SetStrokeLineJoin(null);
            writer.SetStrokeDashArray(null);
            writer.SetFill(textStyle.Fill);
            writer.SetFillOpacity(null);
            writer.SetFillRule(null);
            writer.SetFontFamily(textStyle.FontFamily);
            writer.SetFontSize(textStyle.FontSize);
            writer.SetFontWeight(textStyle.FontWeight);
            writer.SetFontStretch(textStyle.FontStretch);
            writer.SetFontStyle(textStyle.FontStyle);
            writer.SetTextDecoration(textStyle.TextDecoration);
        }

        private static void SetVectorStyles(SvgWriter writer, Func<IFeature, VectorStyle> themeFilter, IFeature feature)
        {
            if (themeFilter != null)
            {
                SetVectorStyles(writer, themeFilter(feature));
            }
        }

        private void SetSvgCanvas(SvgWriter writer, Geometry.Extent2D extents, int width, int height)
        {
            if (width <= 0 && height <= 0)
            {
                width = 1600;
            }
            var aspectRatio = extents.XRange / extents.YRange;
            if (width == 0)
            {
                width = (int)(height * aspectRatio);
            }
            else if (height == 0)
            {
                height = (int)(width / aspectRatio);
            }
            writer.SetSvgAttributes(width + "px", height + "px"); //, extents);
            _calcx = TongJi.Maths.LinearFunction(extents.min.x, 0, extents.max.x, width);
            _calcy = TongJi.Maths.LinearFunction(extents.min.y, height, extents.max.y, 0);
            _scale = x => width / (extents.max.x - extents.min.x) * x;
        }

        private static string GetLabel(Config.Layer layerConfig, IFeature feature)
        {
            return layerConfig == null ? null : layerConfig.LabelStyle == null ? null : string.Format(layerConfig.LabelStyle.Format, layerConfig.LabelStyle.PropertyNames.Select(x => feature[x]).ToArray());
        }

        public Config.Layer GetLayerConfigByName(string layerName) // 用竖线分隔的不同层名，本质上互为别名。
        {
            return _mapConfig.Layers.FirstOrDefault(l => l.Name.Split('|').Contains(layerName));
        }

        public string Render(int width = 0, int height = 0)
        {
            SetSvgCanvas(_svgWriter, _extents, width, height);

            var layers = _map.Layers.OrderBy(l => GetLayerOrder(l.Name)).ToList();
            foreach (VectorLayer layer in layers)
            {
                // Run overlay procedure
                var order = GetLayerOrder(layer.Name);
                if (_overlayRenderProcedures.ContainsKey(order))
                {
                    _overlayRenderProcedures[order]();
                }

                // Set SVG styles using layer style config
                var layerConfig = GetLayerConfigByName(layer.Name);
                SetVectorStyles(_svgWriter, layerConfig);
                var themeFilter = _themeFilters.ContainsKey(layer.Name) ? _themeFilters[layer.Name] : null;

                if (layer.GeoType == "1")
                {
                    foreach (Feature feature in layer.Features)
                    {
                        // Set SVG styles using theme
                        SetVectorStyles(_svgWriter, themeFilter, feature);
                        Geometry.Point2D pos = new Geometry.Point2D(feature.GeoData);
                        AddCircle(20, pos.x, pos.y);
                    }

                    SetTextStyles(_svgWriter, layerConfig);
                    foreach (Feature feature in layer.Features)
                    {
                        Geometry.Point2D pos = new Geometry.Point2D(feature.GeoData);
                        AddText(GetLabel(layerConfig, feature), pos.x, pos.y);
                    }
                }
                else if (layer.GeoType == "2")
                {
                    if (layerConfig != null && layerConfig.StrokeBundle != null && layerConfig.StrokeBundle.Strokes.Count > 0)
                    {
                        foreach (VectorStyle strokeStyle in layerConfig.StrokeBundle.Strokes)
                        {
                            // Set SVG styles using this bundle element style
                            SetVectorStyles(_svgWriter, strokeStyle);
                            foreach (Feature feature in layer.Features)
                            {
                                Geometry.Polyline poly = new Geometry.Polyline(feature.GeoData);
                                AddPolyline(poly.Points);
                            }
                        }
                    }
                    else
                    {
                        foreach (Feature feature in layer.Features)
                        {
                            // Set SVG styles using theme
                            SetVectorStyles(_svgWriter, themeFilter, feature);
                            _svgWriter.SetFill("none");
                            Geometry.Polyline poly = new Geometry.Polyline(feature.GeoData);
                            AddPolyline(poly.Points);
                        }
                    }

                    // Whatever case, add label in the end in case of overlapping with shapes
                    if (layerConfig != null && layerConfig.LabelStyle != null)
                    {
                        SetTextStyles(_svgWriter, layerConfig);
                        foreach (Feature feature in layer.Features)
                        {
                            // Add label every specified length
                            Geometry.Polyline poly = new Geometry.Polyline(feature.GeoData);
                            string text = GetLabel(layerConfig, feature);
                            double linearRepeatInterval = layerConfig.LabelStyle.LinearRepeatInterval;
                            double length = poly.Length;
                            List<double> positions = new List<double>();
                            if (length > 0 && length < 2 * linearRepeatInterval)
                            {
                                positions.Add(length / 2);
                            }
                            else
                            {
                                int index = 1;
                                while (index * linearRepeatInterval < length)
                                {
                                    positions.Add(index * linearRepeatInterval);
                                    index++;
                                }
                            }
                            foreach (double position in positions)
                            {
                                Geometry.Point2D pos = poly.GetPointAtDist(position);
                                double tan = poly.GetFirstDerivative(position);
                                double rotation = Math.Atan(tan);
                                double angle = -180 / Math.PI * rotation;
                                AddText(text, pos.x, pos.y, angle);
                            }
                        }
                    }
                }
                else if (layer.GeoType == "4")
                {
                    foreach (Feature feature in layer.Features)
                    {
                        // Set SVG styles using theme
                        SetVectorStyles(_svgWriter, themeFilter, feature);
                        Geometry.Polygon poly = new Geometry.Polygon(feature.GeoData);
                        AddPolygon(poly.Points);
                    }

                    SetTextStyles(_svgWriter, layerConfig);
                    foreach (Feature feature in layer.Features)
                    {
                        Geometry.Polygon poly = new Geometry.Polygon(feature.GeoData);
                        Geometry.Point2D center = poly.Centroid;
                        AddText(GetLabel(layerConfig, feature), center.x, center.y);
                    }
                }
            }

            return _svgWriter.ToString();
        }

        public string GetRenderedResult()
        {
            return _svgWriter.ToString();
        }

        //private static double FlipY(double y, Geometry.Extent2D viewBox)
        //{
        //    return viewBox.min.y + viewBox.max.y - y;
        //}

        //private static Geometry.Point2D FlipY(Geometry.Point2D p, Geometry.Extent2D viewBox)
        //{
        //    double y = viewBox.min.y + viewBox.max.y - p.y;
        //    return new Geometry.Point2D(p.x, y);
        //}

        //private static List<Geometry.Point2D> FlipY(IEnumerable<Geometry.Point2D> points, Geometry.Extent2D viewBox)
        //{
        //    return points.Select(p => FlipY(p, viewBox)).ToList();
        //}

        // 世界坐标绘图函数以Add开头

        public void AddLine(double x1, double y1, double x2, double y2)
        {
            x1 = _calcx(x1);
            y1 = _calcy(y1);
            x2 = _calcx(x2);
            y2 = _calcy(y2);
            _svgWriter.AddLine(x1.ToString("0.##"), y1.ToString("0.##"), x2.ToString("0.##"), y2.ToString("0.##"));
        }

        public void AddCircle(double r, double cx, double cy)
        {
            cx = _calcx(cx);
            cy = _calcy(cy);
            r = _scale(r);
            _svgWriter.AddCircle(r.ToString("0.##"), cx.ToString("0.##"), cy.ToString("0.##"));
        }

        public void AddEllipse(double rx, double ry, double cx, double cy)
        {
            cx = _calcx(cx);
            cy = _calcy(cy);
            rx = _scale(rx);
            ry = _scale(ry);
            _svgWriter.AddEllipse(rx.ToString("0.##"), ry.ToString("0.##"), cx.ToString("0.##"), cy.ToString("0.##"));
        }

        public void AddRect(double width, double height, double x, double y, double rx = 0, double ry = 0)
        {
            x = _calcx(x);
            y = _calcy(y);
            width = _scale(width);
            height = _scale(height);
            rx = _scale(rx);
            ry = _scale(ry);
            _svgWriter.AddRect(width.ToString("0.##"), height.ToString("0.##"), x.ToString("0.##"), y.ToString("0.##"), rx.ToString("0.##"), ry.ToString("0.##"));
        }

        public void AddPolygon(IEnumerable<Geometry.Point2D> points)
        {
            _svgWriter.AddPolygon(points.Select(p => new Geometry.Point2D(_calcx(p.x), _calcy(p.y))).ToList());
        }

        public void AddPolyline(IEnumerable<Geometry.Point2D> points)
        {
            _svgWriter.AddPolyline(points.Select(p => new Geometry.Point2D(_calcx(p.x), _calcy(p.y))).ToList());
        }

        public void AddText(string text, double x, double y, double angle = 0)
        {
            if (!string.IsNullOrEmpty(text))
            {
                x = _calcx(x);
                y = _calcy(y);
                _svgWriter.AddText(text, x.ToString(), y.ToString(), angle == 0 ? null : string.Format("rotate({0:0} {1:0.##},{2:0.##})", angle, x, y));
            }
        }

        // 画布坐标绘图函数以Draw开头

        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            _svgWriter.AddLine(x1.ToString("0.##"), y1.ToString("0.##"), x2.ToString("0.##"), y2.ToString("0.##"));
        }

        public void DrawCircle(double r, double cx, double cy)
        {
            _svgWriter.AddCircle(r.ToString("0.##"), cx.ToString("0.##"), cy.ToString("0.##"));
        }

        public void DrawEllipse(double rx, double ry, double cx, double cy)
        {
            _svgWriter.AddEllipse(rx.ToString("0.##"), ry.ToString("0.##"), cx.ToString("0.##"), cy.ToString("0.##"));
        }

        public void DrawRect(double width, double height, double x, double y, double rx = 0, double ry = 0)
        {
            _svgWriter.AddRect(width.ToString("0.##"), height.ToString("0.##"), x.ToString("0.##"), y.ToString("0.##"), rx.ToString("0.##"), ry.ToString("0.##"));
        }

        public void DrawPolygon(IEnumerable<Geometry.Point2D> points)
        {
            _svgWriter.AddPolygon(points);
        }

        public void DrawPolyline(IEnumerable<Geometry.Point2D> points)
        {
            _svgWriter.AddPolyline(points);
        }

        public void DrawText(string text, double x, double y, double angle = 0)
        {
            if (!string.IsNullOrEmpty(text))
            {
                _svgWriter.AddText(text, x.ToString("0.##"), y.ToString("0.##"), angle == 0 ? null : string.Format("rotate({0:0} {1:0.##},{2:0.##})", angle, x, y));
            }
        }

        public void AddTitle(string text, string fontFamily, string fontSize)
        {
            _svgWriter.SetFill("black");
            _svgWriter.SetFontFamily(fontFamily);
            _svgWriter.SetFontSize(fontSize);
            DrawText(text, 100, 100);
        }

        public void AddLegend(List<Tuple<string, string>> items)
        {
            double x = 100;
            double y = 200;
            _svgWriter.SetFontFamily("SimSun");
            _svgWriter.SetFontSize("16px");
            _svgWriter.SetFill("none");
            _svgWriter.SetStroke("black");
            DrawRect(200, items.Count * 20 + 20, x, y);
            x += 10;
            y += 10;
            foreach (var item in items)
            {
                _svgWriter.SetFill(item.Item2);
                DrawRect(50, 15, x, y + 2.5);
                _svgWriter.SetFill("black");
                DrawText(item.Item1, x + 60, y + 15);
                y += 20;
            }
        }

        public void SetVectorStyles(VectorStyle style)
        {
            SetVectorStyles(_svgWriter, style);
        }

        public void SetTextStyles(TextStyle style)
        {
            SetTextStyles(_svgWriter, style);
        }

        public void ApplyFeatureOverlay(string layerName, IFeatureOverlay overlay)
        {
            var layer = _map.Layers[layerName];
            foreach (var f in layer.Features)
            {
                overlay.Render(this, f);
            }
        }

        public void RegisterInterruptProcedure(int order, Action action)
        {
            _overlayRenderProcedures[order] = action;
        }
    }

    /// <summary>
    /// 要素叠加物
    /// </summary>
    public interface IFeatureOverlay
    {
        void Render(SvgMapRenderer renderer, IFeature f);
    }

    /// <summary>
    /// 泡泡图
    /// </summary>
    public class BubbleChart : IFeatureOverlay
    {
        private Func<IFeature, double> _radius;
        private Func<IFeature, string> _fill;

        public BubbleChart(Func<IFeature, double> radius, Func<IFeature, string> fill)
        {
            _radius = radius;
            _fill = fill;
        }

        public void Render(SvgMapRenderer renderer, IFeature f)
        {
            VectorStyle style = new VectorStyle { Fill = _fill(f), Stroke = null };
            var radius = _radius(f);
            var pos = new Geometry.Polygon(f.GeoData).Centroid;
            renderer.SetVectorStyles(style);
            renderer.AddCircle(radius, pos.x, pos.y);
        }
    }

    /// <summary>
    /// 在位柱状图
    /// </summary>
    public class SpotChart : IFeatureOverlay
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

        public SpotChartColumn AddColumn(string prop, double maxVal, string fill)
        {
            SpotChartColumn column = new SpotChartColumn(this, prop, maxVal, fill);
            Columns.Add(column);
            return column;
        }

        public void Render(SvgMapRenderer renderer, IFeature f)
        {
            double xOffset = 0;
            foreach (var spotChartColumn in Columns)
            {
                VectorStyle style = new VectorStyle { Fill = spotChartColumn.Fill, Stroke = null };
                var size = spotChartColumn.GetSize(f);
                var pos = new Geometry.Polygon(f.GeoData).Centroid;
                renderer.SetVectorStyles(style);
                renderer.AddRect(size.Item1, size.Item2, pos.x + xOffset, pos.y + size.Item2);
                xOffset += size.Item1;
            }
        }
    }

    /// <summary>
    /// 在位柱状图的数据列
    /// </summary>
    public class SpotChartColumn
    {
        public string Property { get; set; }
        public double MaxValue { get; set; }
        public string Fill { get; set; }
        public SpotChart Chart { get; set; }
        public Func<IFeature, double> ValueSelector { get; set; }

        internal SpotChartColumn(SpotChart chart, string prop, double maxVal, string fill)
        {
            Chart = chart;
            Property = prop;
            MaxValue = maxVal;
            Fill = fill;
            ValueSelector = f => f[Property].TryParseToDouble();
        }

        public Tuple<double, double> GetSize(IFeature f)
        {
            return Tuple.Create(Chart.Width / Chart.Columns.Count, (ValueSelector(f) / MaxValue) * Chart.Height);
        }
    }

    /// <summary>
    /// 组合线型。地图上的线实体往往是由一组线样式绘制而成的，例如公路、铁路的图例，可看作不同颜色、粗细、虚线的若干线型的组合。
    /// </summary>
    public class StrokeBundle
    {
        public List<VectorStyle> Strokes { get; set; }
    }

    /// <summary>
    /// SVG矢量样式
    /// </summary>
    public class VectorStyle
    {
        [XmlAttribute]
        public string Stroke { get; set; }
        [XmlAttribute]
        public string StrokeWidth { get; set; }
        [XmlAttribute]
        public string StrokeOpacity { get; set; }
        [XmlAttribute]
        public string StrokeDashArray { get; set; }
        [XmlAttribute]
        public string StrokeLineCap { get; set; }
        [XmlAttribute]
        public string StrokeLineJoin { get; set; }
        [XmlAttribute]
        public string Fill { get; set; }
        [XmlAttribute]
        public string FillOpacity { get; set; }
        [XmlAttribute]
        public string FillRule { get; set; }

        public VectorStyle()
        {
            Stroke = "black";
            StrokeWidth = "1";
            Fill = "#FFFFB4";
        }
    }

    /// <summary>
    /// SVG文字样式
    /// </summary>
    public class TextStyle
    {
        [XmlAttribute]
        public string Stroke { get; set; }
        [XmlAttribute]
        public string StrokeWidth { get; set; }
        [XmlAttribute]
        public string Fill { get; set; }
        [XmlAttribute]
        public string FontFamily { get; set; }
        [XmlAttribute]
        public string FontSize { get; set; }
        [XmlAttribute]
        public string FontWeight { get; set; }
        [XmlAttribute]
        public string FontStretch { get; set; }
        [XmlAttribute]
        public string FontStyle { get; set; }
        [XmlAttribute]
        public string TextDecoration { get; set; }
    }

    /// <summary>
    /// 文字标注样式
    /// </summary>
    public class LabelStyle
    {
        [XmlAttribute]
        public string Format { get; set; }
        public List<string> PropertyNames { get; set; }
        [XmlAttribute]
        public double LinearRepeatInterval { get; set; }
    }
}

namespace TongJi.Gis.Config
{
    public class Layer
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int Order { get; set; }
        public VectorStyle VectorStyle { get; set; }
        public TextStyle TextStyle { get; set; }
        public LabelStyle LabelStyle { get; set; }
        public StrokeBundle StrokeBundle { get; set; }
    }

    public class MapConfig
    {
        public List<Layer> Layers { get; set; }

        public void Save(string fileName)
        {
            TongJi.IO.Serialization.XmlSave(this, fileName);
        }

        public static MapConfig Load(string fileName)
        {
            return TongJi.IO.Serialization.XmlLoad<MapConfig>(fileName);
        }
    }
}