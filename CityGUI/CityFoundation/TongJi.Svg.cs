using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

namespace TongJi.Svg
{
    /// <summary>
    /// SVG生成器，是TongJi.Svg早期一种较底层的SVG操作方式，仅隐藏了XML操作的细节，调用者需要知道SVG的一些细节，使用此类仅仅为了调用方便。
    /// </summary>
    public class SvgWriter
    {
        private static XNamespace _ns = XNamespace.Get("http://www.w3.org/2000/svg");
        /// <summary>
        /// SVG命名空间
        /// </summary>
        public static XNamespace ns { get { return _ns; } }
        private XDocument _xd = new XDocument();
        private XElement _root;

        private string _stroke;
        private string _strokeWidth;
        private string _strokeOpacity;
        private string _strokeDashArray;
        private string _strokeLineCap;
        private string _strokeLineJoin;
        private string _fill;
        private string _fillOpacity;
        private string _fillRule;
        private string _fontFamily;
        private string _fontSize;
        private string _fontWeight;
        private string _fontStretch;
        private string _fontStyle;
        private string _textDecoration;

        public SvgWriter()
        {
            _root = new XElement(_ns + "svg");
            _xd.Add(_root);
        }

        #region 参数设置

        /// <summary>
        /// 设置描边
        /// </summary>
        /// <param name="stroke"></param>
        public void SetStroke(string stroke)
        {
            _stroke = stroke;
        }

        /// <summary>
        /// 设置描边宽度
        /// </summary>
        /// <param name="strokeWidth"></param>
        public void SetStrokeWidth(string strokeWidth)
        {
            _strokeWidth = strokeWidth;
        }

        /// <summary>
        /// 设置描边不透明度
        /// </summary>
        /// <param name="strokeOpacity"></param>
        public void SetStrokeOpacity(string strokeOpacity)
        {
            _strokeOpacity = strokeOpacity;
        }

        /// <summary>
        /// 设置描边虚线样式
        /// </summary>
        /// <param name="strokeDashArray"></param>
        public void SetStrokeDashArray(string strokeDashArray)
        {
            _strokeDashArray = strokeDashArray;
        }

        /// <summary>
        /// 设置描边线条终端样式
        /// </summary>
        /// <param name="strokeLineCap">butt|round|square</param>
        public void SetStrokeLineCap(string strokeLineCap)
        {
            _strokeLineCap = strokeLineCap;
        }

        /// <summary>
        /// 设置描边线条连接样式
        /// </summary>
        /// <param name="strokeLineJoin">miter|round|bevel</param>
        public void SetStrokeLineJoin(string strokeLineJoin)
        {
            _strokeLineJoin = strokeLineJoin;
        }

        /// <summary>
        /// 设置填充
        /// </summary>
        /// <param name="fill"></param>
        public void SetFill(string fill)
        {
            _fill = fill;
        }

        /// <summary>
        /// 设置填充不透明度
        /// </summary>
        /// <param name="fillOpacity"></param>
        public void SetFillOpacity(string fillOpacity)
        {
            _fillOpacity = fillOpacity;
        }

        /// <summary>
        /// 设置填充规则
        /// </summary>
        /// <param name="fillRule">nonzero|evenodd</param>
        public void SetFillRule(string fillRule)
        {
            _fillRule = fillRule;
        }

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="fontFamily"></param>
        public void SetFontFamily(string fontFamily)
        {
            _fontFamily = fontFamily;
        }

        /// <summary>
        /// 设置字体大小
        /// </summary>
        /// <param name="fontSize"></param>
        public void SetFontSize(string fontSize)
        {
            _fontSize = fontSize;
        }

        /// <summary>
        /// 设置字体粗细
        /// </summary>
        /// <param name="fontWeight">thin|light|normal|regular|medium|bold|black|heavy|...(100-950)</param>
        public void SetFontWeight(string fontWeight)
        {
            _fontWeight = fontWeight;
        }

        /// <summary>
        /// 设置字体拉伸
        /// </summary>
        /// <param name="fontStretch">condensed|normal|medium|expanded|...(50%-200%)</param>
        public void SetFontStretch(string fontStretch)
        {
            _fontStretch = fontStretch;
        }

        /// <summary>
        /// 设置字体样式
        /// </summary>
        /// <param name="fontStyle">normal|italic|oblique</param>
        public void SetFontStyle(string fontStyle)
        {
            _fontStyle = fontStyle;
        }

        /// <summary>
        /// 设置文字修饰
        /// </summary>
        /// <param name="textDecoration">baseline|overline|strikethrough|underline</param>
        public void SetTextDecoration(string textDecoration)
        {
            _textDecoration = textDecoration;
        }

        #endregion

        #region 绘图函数

        private void SetShapeAttributes(XElement xe)
        {
            xe.SetAttributeValue("stroke", _stroke);
            xe.SetAttributeValue("stroke-width", _strokeWidth);
            xe.SetAttributeValue("stroke-opacity", _strokeOpacity);
            xe.SetAttributeValue("stroke-dasharray", _strokeDashArray);
            xe.SetAttributeValue("stroke-linecap", _strokeLineCap);
            xe.SetAttributeValue("stroke-linejoin", _strokeLineJoin);
            xe.SetAttributeValue("fill", _fill);
            xe.SetAttributeValue("fill-opacity", _strokeOpacity);
            xe.SetAttributeValue("fill-rule", _fillRule);
        }

        private void SetTextAttributes(XElement xe)
        {
            xe.SetAttributeValue("font-family", _fontFamily);
            xe.SetAttributeValue("font-size", _fontSize);
            xe.SetAttributeValue("font-weight", _fontWeight);
            xe.SetAttributeValue("font-stretch", _fontStretch);
            xe.SetAttributeValue("font-style", _fontStyle);
            xe.SetAttributeValue("text-decoration", _textDecoration);
        }

        /// <summary>
        /// 设置SVG属性：宽度、高度、viewBox
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="viewBox"></param>
        public void SetSvgAttributes(string width = null, string height = null, Geometry.Extent2D viewBox = null) // newly 20130507
        {
            _root.SetAttributeValue("width", width);
            _root.SetAttributeValue("height", height);
            if (viewBox != null)
            {
                _root.SetAttributeValue("viewBox", string.Format("{0:0.##} {1:0.##} {2:0.##} {3:0.##}", viewBox.min.x, viewBox.min.y, viewBox.XRange, viewBox.YRange));
            }
        }

        /// <summary>
        /// 绘制线段
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void AddLine(string x1, string y1, string x2, string y2)
        {
            XElement xe = new XElement(_ns + "line");
            xe.SetAttributeValue("x1", x1);
            xe.SetAttributeValue("y1", y1);
            xe.SetAttributeValue("x2", x2);
            xe.SetAttributeValue("y2", y2);
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        /// <summary>
        /// 绘制圆
        /// </summary>
        /// <param name="r"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        public void AddCircle(string r, string cx, string cy)
        {
            XElement xe = new XElement(_ns + "circle");
            xe.SetAttributeValue("r", r);
            xe.SetAttributeValue("cx", cx);
            xe.SetAttributeValue("cy", cy);
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        /// <summary>
        /// 绘制椭圆
        /// </summary>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        public void AddEllipse(string rx, string ry, string cx, string cy)
        {
            XElement xe = new XElement(_ns + "ellipse");
            xe.SetAttributeValue("rx", rx);
            xe.SetAttributeValue("ry", ry);
            xe.SetAttributeValue("cx", cx);
            xe.SetAttributeValue("cy", cy);
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        /// <summary>
        /// 绘制矩形
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="rx"></param>
        /// <param name="ry"></param>
        public void AddRect(string width, string height, string x, string y, string rx = null, string ry = null)
        {
            XElement xe = new XElement(_ns + "rect");
            xe.SetAttributeValue("width", width);
            xe.SetAttributeValue("height", height);
            xe.SetAttributeValue("x", x);
            xe.SetAttributeValue("y", y);
            xe.SetAttributeValue("rx", rx);
            xe.SetAttributeValue("ry", ry);
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        private static string GetPointString(IEnumerable<Geometry.Point2D> points)
        {
            return string.Join(" ", points.Select(p => string.Format("{0:0.##},{1:0.##}", p.x, p.y)).ToArray());
        }

        /// <summary>
        /// 绘制多段线
        /// </summary>
        /// <param name="points"></param>
        public void AddPolyline(IEnumerable<Geometry.Point2D> points)
        {
            XElement xe = new XElement(_ns + "polyline");
            xe.SetAttributeValue("points", GetPointString(points));
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        /// <summary>
        /// 绘制多边形
        /// </summary>
        /// <param name="points"></param>
        public void AddPolygon(IEnumerable<Geometry.Point2D> points)
        {
            XElement xe = new XElement(_ns + "polygon");
            xe.SetAttributeValue("points", GetPointString(points));
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        public void AddPath()
        {
            // todo: 实现Path
        }

        /// <summary>
        /// 绘制文本
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddText(string text, string x, string y, string transform = null)
        {
            XElement xe = new XElement(_ns + "text", text);
            xe.SetAttributeValue("x", x);
            xe.SetAttributeValue("y", y);
            xe.SetAttributeValue("transform", transform);
            SetShapeAttributes(xe);
            SetTextAttributes(xe);
            _root.Add(xe);
        }

        /// <summary>
        /// 创建线性渐变
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stops"></param>
        public void AddLinearGradient(string id, List<GradientStop> stops, string gradientTransform = null)
        {
            XElement xe = new XElement(_ns + "linearGradient");
            xe.SetAttributeValue("id", id);
            xe.SetAttributeValue("gradientTransform", gradientTransform);
            xe.Add(stops.Select(x => x.ToXElement()).ToArray());
            _root.Add(xe);
        }

        /// <summary>
        /// 创建径向渐变
        /// </summary>
        /// <param name="id"></param>
        /// <param name="stops"></param>
        public void AddRadialGradient(string id, List<GradientStop> stops, string gradientTransform = null)
        {
            XElement xe = new XElement(_ns + "radialGradient");
            xe.SetAttributeValue("id", id);
            xe.SetAttributeValue("gradientTransform", gradientTransform);
            xe.Add(stops.Select(x => x.ToXElement()).ToArray());
            _root.Add(xe);
        }

        public GradientStop GradientStop(double offset, string stopColor, double stopOpacity = 1)
        {
            return new GradientStop { Offset = offset, StopColor = stopColor, StopOpacity = stopOpacity };
        }

        public List<GradientStop> GradientStops(params GradientStop[] stops)
        {
            return stops.ToList();
        }

        #endregion

        /// <summary>
        /// 保存图形
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            _xd.Save(fileName);
        }

        /// <summary>
        /// 获取SVG字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _xd.ToString();
        }
    }

    /// <summary>
    /// 渐变控制点
    /// </summary>
    public class GradientStop
    {
        public double Offset { get; set; }
        public string StopColor { get; set; }
        public double StopOpacity { get; set; }

        public XElement ToXElement()
        {
            XElement xe = new XElement(SvgWriter.ns + "stop");
            xe.SetAttributeValue("offset", Offset);
            xe.SetAttributeValue("stop-color", StopColor);
            xe.SetAttributeValue("stop-opacity", StopOpacity);
            return xe;
        }
    }
}
