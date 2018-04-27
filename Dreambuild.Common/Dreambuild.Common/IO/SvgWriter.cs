using Dreambuild.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dreambuild.Svg
{
    /// <summary>
    /// The SVG writer.
    /// </summary>
    public class SvgWriter
    {
        internal static readonly XNamespace _ns = XNamespace.Get("http://www.w3.org/2000/svg");
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

        #region Settings

        /// <summary>
        /// Sets the stroke.
        /// </summary>
        /// <param name="stroke">The stroke.</param>
        public void SetStroke(string stroke)
        {
            _stroke = stroke;
        }

        /// <summary>
        /// Sets the stroke width.
        /// </summary>
        /// <param name="strokeWidth">The stroke width.</param>
        public void SetStrokeWidth(string strokeWidth)
        {
            _strokeWidth = strokeWidth;
        }

        /// <summary>
        /// Sets the stroke opacity.
        /// </summary>
        /// <param name="strokeOpacity">The stroke opacity.</param>
        public void SetStrokeOpacity(string strokeOpacity)
        {
            _strokeOpacity = strokeOpacity;
        }

        /// <summary>
        /// Sets the stroke dash array.
        /// </summary>
        /// <param name="strokeDashArray">The stroke dash array.</param>
        public void SetStrokeDashArray(string strokeDashArray)
        {
            _strokeDashArray = strokeDashArray;
        }

        /// <summary>
        /// Sets the stroke line cap.
        /// </summary>
        /// <param name="strokeLineCap">The stroke line cap: butt|round|square.</param>
        public void SetStrokeLineCap(string strokeLineCap)
        {
            _strokeLineCap = strokeLineCap;
        }

        /// <summary>
        /// Sets the stroke line join.
        /// </summary>
        /// <param name="strokeLineJoin">The stroke line join: miter|round|bevel.</param>
        public void SetStrokeLineJoin(string strokeLineJoin)
        {
            _strokeLineJoin = strokeLineJoin;
        }

        /// <summary>
        /// Sets the fill.
        /// </summary>
        /// <param name="fill">The fill.</param>
        public void SetFill(string fill)
        {
            _fill = fill;
        }

        /// <summary>
        /// Sets the fill opacity.
        /// </summary>
        /// <param name="fillOpacity">The fill opacity.</param>
        public void SetFillOpacity(string fillOpacity)
        {
            _fillOpacity = fillOpacity;
        }

        /// <summary>
        /// Sets the fill rule.
        /// </summary>
        /// <param name="fillRule">The fill rule: nonzero|evenodd.</param>
        public void SetFillRule(string fillRule)
        {
            _fillRule = fillRule;
        }

        /// <summary>
        /// Sets the font family.
        /// </summary>
        /// <param name="fontFamily">The font family.</param>
        public void SetFontFamily(string fontFamily)
        {
            _fontFamily = fontFamily;
        }

        /// <summary>
        /// Sets the font size.
        /// </summary>
        /// <param name="fontSize">The font size.</param>
        public void SetFontSize(string fontSize)
        {
            _fontSize = fontSize;
        }

        /// <summary>
        /// Sets the font weight.
        /// </summary>
        /// <param name="fontWeight">The font weight: thin|light|normal|regular|medium|bold|black|heavy|...(100-950).</param>
        public void SetFontWeight(string fontWeight)
        {
            _fontWeight = fontWeight;
        }

        /// <summary>
        /// Sets the font stretch.
        /// </summary>
        /// <param name="fontStretch">The font stretch: condensed|normal|medium|expanded|...(50%-200%).</param>
        public void SetFontStretch(string fontStretch)
        {
            _fontStretch = fontStretch;
        }

        /// <summary>
        /// Sets the font style.
        /// </summary>
        /// <param name="fontStyle">The font style: normal|italic|oblique.</param>
        public void SetFontStyle(string fontStyle)
        {
            _fontStyle = fontStyle;
        }

        /// <summary>
        /// Sets the text decoration.
        /// </summary>
        /// <param name="textDecoration">The text decoration: baseline|overline|strikethrough|underline.</param>
        public void SetTextDecoration(string textDecoration)
        {
            _textDecoration = textDecoration;
        }

        #endregion

        #region Draw

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
        /// Sets SVG attributes.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="viewBox">The view box.</param>
        public void SetSvgAttributes(string width = null, string height = null, Extents viewBox = null)
        {
            _root.SetAttributeValue("width", width);
            _root.SetAttributeValue("height", height);
            if (viewBox != null)
            {
                _root.SetAttributeValue("viewBox", string.Format("{0:0.##} {1:0.##} {2:0.##} {3:0.##}", viewBox.Min.Value.X, viewBox.Min.Value.Y, viewBox.Range(0), viewBox.Range(1)));
            }
        }

        /// <summary>
        /// Adds a line.
        /// </summary>
        /// <param name="x1">The X coordinate of the first point.</param>
        /// <param name="y1">The Y coordinate of the first point.</param>
        /// <param name="x2">The X coordinate of the second point.</param>
        /// <param name="y2">The Y coordinate of the second point.</param>
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
        /// Adds a circle.
        /// </summary>
        /// <param name="r">The radius.</param>
        /// <param name="cx">The X coordinate of the center.</param>
        /// <param name="cy">The Y coordinate of the center.</param>
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
        /// Adds an ellipse.
        /// </summary>
        /// <param name="rx">The X radius.</param>
        /// <param name="ry">The Y radius.</param>
        /// <param name="cx">The X coordinate of the center.</param>
        /// <param name="cy">The Y coordinate of the center.</param>
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
        /// Adds a rectangle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="x">The left.</param>
        /// <param name="y">The top.</param>
        /// <param name="rx">The X corner radius.</param>
        /// <param name="ry">The Y corner radius.</param>
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

        private static string GetPointString(IEnumerable<Vector> points)
        {
            return string.Join(" ", points.Select(p => string.Format("{0:0.##},{1:0.##}", p.X, p.Y)).ToArray());
        }

        /// <summary>
        /// Adds a polyline.
        /// </summary>
        /// <param name="points">The points.</param>
        public void AddPolyline(params Vector[] points)
        {
            XElement xe = new XElement(_ns + "polyline");
            xe.SetAttributeValue("points", GetPointString(points));
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        /// <summary>
        /// Adds a polygon.
        /// </summary>
        /// <param name="points">The points.</param>
        public void AddPolygon(params Vector[] points)
        {
            XElement xe = new XElement(_ns + "polygon");
            xe.SetAttributeValue("points", GetPointString(points));
            SetShapeAttributes(xe);
            _root.Add(xe);
        }

        public void AddPath()
        {
            // todo: implement Path
        }

        /// <summary>
        /// Adds text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="x">The left.</param>
        /// <param name="y">The top.</param>
        /// <param name="transform">The transform.</param>
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
        /// Adds a linear gradient.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="stops">The stops.</param>
        /// <param name="gradientTransform">The gradient transform.</param>
        public void AddLinearGradient(string id, GradientStop[] stops, string gradientTransform = null)
        {
            XElement xe = new XElement(_ns + "linearGradient");
            xe.SetAttributeValue("id", id);
            xe.SetAttributeValue("gradientTransform", gradientTransform);
            xe.Add(stops.Select(x => x.ToXElement()).ToArray());
            _root.Add(xe);
        }

        /// <summary>
        /// Adds a radial gradient.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="stops">The stops.</param>
        /// <param name="gradientTransform">The gradient transform.</param>
        public void AddRadialGradient(string id, GradientStop[] stops, string gradientTransform = null)
        {
            XElement xe = new XElement(_ns + "radialGradient");
            xe.SetAttributeValue("id", id);
            xe.SetAttributeValue("gradientTransform", gradientTransform);
            xe.Add(stops.Select(x => x.ToXElement()).ToArray());
            _root.Add(xe);
        }

        /// <summary>
        /// Creates a gradient stop.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="stopColor">The stop color.</param>
        /// <param name="stopOpacity">The stop opacity.</param>
        /// <returns>The result.</returns>
        public GradientStop GradientStop(double offset, string stopColor, double stopOpacity = 1)
        {
            return new GradientStop { Offset = offset, StopColor = stopColor, StopOpacity = stopOpacity };
        }

        /// <summary>
        /// Creates gradient stop array.
        /// </summary>
        /// <param name="stops">The stops.</param>
        /// <returns>The result.</returns>
        public GradientStop[] GradientStops(params GradientStop[] stops)
        {
            return stops;
        }

        #endregion

        /// <summary>
        /// Saves to file.
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            _xd.Save(fileName);
        }

        /// <summary>
        /// Gets SVG string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _xd.ToString();
        }
    }

    /// <summary>
    /// The gradient stop.
    /// </summary>
    public class GradientStop
    {
        public double Offset { get; set; }
        public string StopColor { get; set; }
        public double StopOpacity { get; set; }

        public XElement ToXElement()
        {
            XElement xe = new XElement(SvgWriter._ns + "stop");
            xe.SetAttributeValue("offset", Offset);
            xe.SetAttributeValue("stop-color", StopColor);
            xe.SetAttributeValue("stop-opacity", StopOpacity);
            return xe;
        }
    }
}
