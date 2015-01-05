using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace TongJi.Gis.Display
{
    /// <summary>
    /// 地图图层，使用WPF Shape渲染
    /// </summary>
    public class MapLayer : Canvas
    {
        public Dictionary<IFeature, Shape> Features { get; protected set; }
        public List<FrameworkElement> Overlays { get; protected set; } // newly 20130308
        public VectorLayer LayerData { get; protected set; }
        public VectorStyle LayerStyle { get; protected set; }
        public LabelStyle LayerLableStyle { get; protected set; }
        public bool IsLayerVisible { get; set; }
        public MapLayer LabelLayer { get; protected set; }
        protected bool _hasSizeTheme = false; // newly 20130309
        protected IDataSizeTheme _sizeTheme;

        public MapLayer()
        {
            Features = new Dictionary<IFeature, Shape>();
            Overlays = new List<FrameworkElement>();
            LayerStyle = new VectorStyle();
            LayerLableStyle = new LabelStyle(string.Empty);
            IsLayerVisible = true;

            //CacheMode = new BitmapCache();
        }

        public virtual int ElementCount
        {
            get
            {
                return Children.Count;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", LayerData.Name, Features.Count);
        }

        public void AddFeatureChildren(IFeature f, Shape shape)
        {
            Features.Add(f, shape);
            Children.Add(shape);
        }

        public void AddOverlayChildren(FrameworkElement fe)
        {
            Overlays.Add(fe);
            Children.Add(fe);
        }

        public void ClearOverlayChildren()
        {
            Overlays.ForEach(x => Children.Remove(x));
            Overlays.Clear();
        }

        protected VectorStyle GetLayerStyle(VectorLayer layer)
        {
            string layerName = layer.Name;
            if (VectorStyleManager.Styles.Keys.Contains(layerName))
            {
                return VectorStyleManager.Styles.First(f => f.Key == layerName).Value;
            }
            else
            {
                return new VectorStyle();
            }
        }

        protected LabelStyle GetLabelStyle(VectorLayer layer)
        {
            string layerName = layer.Name;
            if (VectorStyleManager.LabelStyles.Keys.Contains(layerName))
            {
                return VectorStyleManager.LabelStyles.First(f => f.Key == layerName).Value;
            }
            else
            {
                return LayerLableStyle;
            }
        }

        public virtual void ClearData()
        {
            Children.Clear();
        }

        private bool IsRoad() // newly 20130411
        {
            string[] roadLayers = { "道路", "主干道", "次干道" };
            return roadLayers.Contains(LayerData.Name);
        }

        private void DrawFeature(IFeature feature)
        {
            if (LayerData.GeoType == "1")
            {
                Geometry.Point2D pos = new Geometry.Point2D(feature.GeoData);
                AddSpot(new Point(pos.x, pos.y), feature);
                AddLable(new Point(pos.x, pos.y), LayerLableStyle.GetLable(feature));
            }
            else if (LayerData.GeoType == "2")
            {
                Geometry.Polyline poly = new Geometry.Polyline(feature.GeoData);
                if (IsRoad())
                {
                    AddRoadStroke(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                }
                if (!IsRoad())
                {
                    AddPolyline(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                }
                else
                {
                    AddRoadFill(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                }

                string text = LayerLableStyle.GetLable(feature);
                double linearRepeatInterval = LayerLableStyle.LinearRepeatInterval;
                double length = poly.Length;
                List<double> positions = new List<double>(); // mod 20130528 与SVG同步
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
                    Geometry.Point2D tempPt = poly.GetPointAtDist(position);
                    double tanVal = poly.GetFirstDerivative(position);
                    double rotation = Math.Atan(tanVal);
                    double angle = 180 / Math.PI * rotation;
                    AddLable(new Point(tempPt.x, tempPt.y), text, angle);
                }
            }
            else if (LayerData.GeoType == "4")
            {
                Geometry.Polygon poly = new Geometry.Polygon(feature.GeoData);
                Geometry.Point2D center = poly.Centroid; // mod 20130507
                AddPolygon(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                AddLable(new Point(center.x, center.y), LayerLableStyle.GetLable(feature));
            }
        }

        public virtual void AddFeature(IFeature feature)
        {
            if (LayerData != null)
            {
                LayerData.Features.Add(feature);
                this.DrawFeature(feature);
            }
        }

        public virtual void RemoveFeature(IFeature feature)
        {
            if (LayerData != null)
            {
                LayerData.Features.Remove(feature);
                if (Features.ContainsKey(feature))
                {
                    this.Children.Remove(Features[feature]);
                    Features.Remove(feature);
                }
            }
        }

        public virtual void SetData(VectorLayer layer)
        {
            LayerData = layer;
            LayerStyle = GetLayerStyle(layer);
            LayerLableStyle = GetLabelStyle(layer);
            Features.Clear();
            Overlays.Clear();
            Children.Clear();
            LabelLayer = new MapLayer();

            if (layer.GeoType == "1")
            {
                foreach (Feature feature in layer.Features)
                {
                    Geometry.Point2D pos = new Geometry.Point2D(feature.GeoData);
                    AddSpot(new Point(pos.x, pos.y), feature);
                    AddLable(new Point(pos.x, pos.y), LayerLableStyle.GetLable(feature));
                }
            }
            else if (layer.GeoType == "2")
            {
                if (IsRoad())
                {
                    foreach (Feature feature in layer.Features)
                    {
                        Geometry.Polyline polyline = new Geometry.Polyline(feature.GeoData);
                        AddRoadStroke(polyline.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                    }
                }
                foreach (Feature feature in layer.Features)
                {
                    Geometry.Polyline poly = new Geometry.Polyline(feature.GeoData);
                    if (!IsRoad())
                    {
                        AddPolyline(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                    }
                    else
                    {
                        AddRoadFill(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                    }
                }
                foreach (Feature feature in layer.Features) // mod 20130516 最后统一加标签
                {
                    Geometry.Polyline poly = new Geometry.Polyline(feature.GeoData);
                    string text = LayerLableStyle.GetLable(feature);
                    double linearRepeatInterval = LayerLableStyle.LinearRepeatInterval;
                    double length = poly.Length;
                    //double scale = 4;  // 这个时候出现主窗口的Scale是不合适的。
                    List<double> positions = new List<double>(); // mod 20130528 与SVG同步
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
                    foreach(double position in positions)
                    {
                        Geometry.Point2D tempPt = poly.GetPointAtDist(position);
                        double tanVal = poly.GetFirstDerivative(position);
                        double rotation = Math.Atan(tanVal);
                        double angle = 180 / Math.PI * rotation;
                        AddLable(new Point(tempPt.x, tempPt.y), text, angle);
                    }
                }
            }
            else if (layer.GeoType == "4")
            {
                foreach (Feature feature in layer.Features)
                {
                    Geometry.Polygon poly = new Geometry.Polygon(feature.GeoData);
                    Geometry.Point2D center = poly.Centroid;
                    AddPolygon(poly.Points.Select(p => new Point(p.x, p.y)).ToArray(), feature);
                    AddLable(new Point(center.x, center.y), LayerLableStyle.GetLable(feature));
                }
            }
        }

        public void SetProperty(string name, Func<IFeature, object> formula) // newly 20130119
        {
            foreach (var f in LayerData.Features)
            {
                f.Properties[name] = formula(f).ToString();
            }
        }

        public void AppendProperties()
        {
            if (LayerData.GeoType == "1")
            {
                foreach (var f in LayerData.Features)
                {
                    f.Properties["点实体-位置"] = f.GeoData;
                }
            }
            else if (LayerData.GeoType == "2")
            {
                foreach (var f in LayerData.Features)
                {
                    f.Properties["线实体-长度"] = new Geometry.Polyline(f.GeoData).Length.ToString("0.00");
                }
            }
            else if (LayerData.GeoType == "4")
            {
                foreach (var f in LayerData.Features)
                {
                    var poly = new Geometry.Polygon(f.GeoData);
                    f.Properties["面实体-面积"] = poly.Area.ToString("0.00");
                    f.Properties["面实体-周长"] = poly.Perimeter.ToString("0.00");
                }
            }
        }

        public virtual void SetMagFactor(double magFactor)
        {
            if (this.LayerData.GeoType == "1" && !_hasSizeTheme)
            {
                foreach (Ellipse ep in this.Features.Values)
                {
                    double size = this.LayerStyle.SpotSize * magFactor;
                    ep.StrokeThickness = this.LayerStyle.StrokeWeight * magFactor;
                    ElementPositionHelper.CenterElementInCanvas(ep, size, size);
                }
            }
            else if (this.LayerData.GeoType == "2" && !IsRoad())
            {
                if (_hasSizeTheme)
                {
                    foreach (var feature in this.Features)
                    {
                        feature.Value.StrokeThickness = _sizeTheme.GetSize(feature.Key) * magFactor;
                    }
                }
                else
                {
                    foreach (var poly in this.Features.Values)
                    {
                        poly.StrokeThickness = this.LayerStyle.StrokeWeight * magFactor;
                    }
                }
            }
            else if (this.LayerData.GeoType == "4")
            {
                foreach (Polygon poly in this.Features.Values)
                {
                    poly.StrokeThickness = this.LayerStyle.StrokeWeight * magFactor;
                }
            }
        }

        public virtual void ApplyColorTheme(IColorTheme theme)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];

                if (LayerData.GeoType == "2")
                {
                    shape.Stroke = new SolidColorBrush(theme.GetColor(feature));
                }
                else
                {
                    shape.Fill = new SolidColorBrush(theme.GetColor(feature));
                    shape.Stroke = new SolidColorBrush(Colors.Black);
                }
            }
        }

        public virtual void ApplySizeTheme(IDataSizeTheme theme)
        {
            if (LayerData.GeoType == "1")
            {
                for (int i = 0; i < Features.Count; i++)
                {
                    var feature = LayerData.Features[i];
                    var shape = Features[feature];

                    double size = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * theme.GetSize(feature);
                    ElementPositionHelper.CenterElementInCanvas(shape, size, size);
                }
            }
            else if (LayerData.GeoType == "2")
            {
                // done: 流量图
                for (int i = 0; i < Features.Count; i++)
                {
                    var feature = LayerData.Features[i];
                    var shape = Features[feature];

                    double size = theme.GetSize(feature);
                    (shape as Polyline).StrokeThickness = size;
                }
            }
            _hasSizeTheme = true;
            _sizeTheme = theme;
        }

        public virtual void ApplyFluidTheme(IDataFluidTheme theme)
        {
            //if (LayerData.GeoType == "2")
            //{
            //    foreach (var feature in Features)
            //    {
            //        var f = feature.Key;
            //        var geometry = PolylineToPathGeometry(feature.Value as Polyline);
            //        var poly = new Geometry.Polyline(f.GeoData);
            //        double length = poly.Length;
            //        double velocity = theme.GetVelocity(f);
            //        double time = length / velocity;
            //        double space = 1 / theme.GetDensity(f);
            //        int spotCount = (int)(length / space) + 1;
            //        var color = theme.GetColor(f);

            //        for (int i = 0; i < spotCount; i++)
            //        {
            //            PointAnimationUsingPath paup = new PointAnimationUsingPath();
            //            paup.PathGeometry = geometry;
            //            paup.Duration = new Duration(new TimeSpan(0, 0, 0, 0, (int)(time * 1000)));
            //            paup.RepeatBehavior = RepeatBehavior.Forever;
            //            paup.BeginTime = new TimeSpan(0, 0, 0, 0, (int)(time / spotCount * i * 1000));

            //            ColorAnimation ca = new ColorAnimation(color.Item1, color.Item2, new Duration(new TimeSpan(0, 0, 0, 0, (int)(time * 1000))));
            //            ca.RepeatBehavior = RepeatBehavior.Forever;
            //            ca.BeginTime = new TimeSpan(0, 0, 0, 0, (int)(time / spotCount * i * 1000));

            //            double radius = theme.GetDiameter(f) / 2;
            //            var fill = new SolidColorBrush(color.Item1);
            //            //EllipseGeometry spot = new EllipseGeometry(new Point(), radius, radius);
            //            //GeometryDrawing spotDrawing = new GeometryDrawing(fill, null, spot);
            //            Ellipse spot = new Ellipse { Width = radius, Height = radius };
            //            this.AddOverlayChildren(spot);
            //            spot.BeginAnimation(EllipseGeometry.CenterProperty, paup);
            //            fill.BeginAnimation(SolidColorBrush.ColorProperty, ca);
            //        }
            //    }
            //}
        }

        //private static PathGeometry PolylineToPathGeometry(Polyline poly)
        //{
        //}

        public virtual void ApplySpotChart(SpotChart chart)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                for (int j = 0; j < chart.Columns.Count; j++)
                {
                    var column = chart.Columns[j];
                    this.AddOverlayChildren(column.GetShape(feature, j));
                }
            }
        }

        public virtual void ApplySpotLink(SpotLink link)
        {
        }

        public virtual void ClearColorTheme()
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];
                if (LayerData.GeoType == "1" || LayerData.GeoType == "4")
                {
                    shape.Fill = LayerStyle.GetFill(feature);
                }
                if (!SelectionSet.Contents.Contains(feature))
                {
                    shape.Stroke = LayerStyle.Stroke;
                }
            }
        }

        public virtual void ClearSpotChart()
        {
            ClearOverlayChildren();
        }

        public virtual void ClearSizeTheme()
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];
                if (LayerData.GeoType == "1")
                {
                    double size = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * LayerStyle.SpotSize;
                    ElementPositionHelper.CenterElementInCanvas(shape, size, size);
                }
                else if (LayerData.GeoType == "2")
                {
                    double size = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * LayerStyle.StrokeWeight;
                    (shape as Polyline).StrokeThickness = size;
                }
            }
            _hasSizeTheme = false;
        }

        public virtual void ApplyToolTip(Func<IFeature, FrameworkElement> mapper)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];
                ToolTipService.SetToolTip(shape, mapper(feature));
            }
        }

        public void ApplyMouseEffects(EventHandler enter, EventHandler leave)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];
                shape.MouseEnter += (s, args) => enter(s, args);
                shape.MouseLeave += (s, args) => leave(s, args);
            }
        }

        public void ApplyClickHandler(Action<IFeature> action)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];
                shape.MouseLeftButtonUp += (s, args) => action(feature);
            }
        }

        public void HighlightFeatures(Func<IFeature, bool> predicate)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var shape = Features[feature];
                if (predicate(feature))
                {
                    shape.Opacity = 1.0;
                }
                else
                {
                    shape.Opacity = 0.4;
                }
            }
        }

        public void ClearMouseEffects()
        {
            // todo: implement this
        }

        protected virtual void AddLable(Point pos, string text, double angle = 0)
        {
            if (!string.IsNullOrEmpty(text)) // 20120315优化
            {
                TextBlock tb = new TextBlock { Text = text }; //此处与silverlight不同
                tb.FontSize = LayerStyle.FontSize;
                tb.Foreground = LayerStyle.FontBrush;
                double actualWidth = LayerStyle.FontSize * 5;
                double actualHeight = LayerStyle.FontSize;

                ElementPositionHelper.SetElementData(tb, "Type", LayerData.GeoType);
                ElementPositionHelper.SetElementData(tb, "Size", LayerStyle.FontSize);
                ElementPositionHelper.SetElementData(tb, "Angle", angle);
                ElementPositionHelper.SetElementDesignPosition(tb, pos);
                //ElementPositionHelper.CenterElementInCanvas(tb, actualWidth, actualHeight);                

                Canvas.SetLeft(tb, pos.X - actualWidth / 2);
                Canvas.SetTop(tb, pos.Y - actualHeight / 2);

                var trans1 = new ScaleTransform { ScaleX = 1, ScaleY = -1, CenterX = actualWidth / 2, CenterY = actualHeight / 2 };
                var trans2 = new RotateTransform { Angle = angle, CenterX = actualWidth / 2, CenterY = actualHeight / 2 };
                var trans = new TransformGroup();
                trans.Children.Add(trans1);
                trans.Children.Add(trans2);
                if (LayerData.GeoType == "1")
                {
                    trans.Children.Add(new TranslateTransform { X = actualWidth / 1.5 });
                }
                tb.RenderTransform = trans;

                LabelLayer.Children.Add(tb);
            }
        }

        protected virtual void AddSpot(Point pos, IFeature f)
        {
            double size = LayerStyle.SpotSize;
            Ellipse result = new Ellipse { Width = size, Height = size };
            ElementPositionHelper.SetElementDesignPosition(result, pos);
            ElementPositionHelper.CenterElementInCanvas(result, size, size);
            result.Stroke = LayerStyle.Stroke;
            result.StrokeThickness = LayerStyle.StrokeWeight;
            result.Fill = LayerStyle.GetFill();
            this.AddFeatureChildren(f, result);
        }

        protected virtual void AddPolyline(Point[] points, IFeature f)
        {
            Polyline poly = new Polyline();
            points.ForEach(p => poly.Points.Add(p));
            poly.Stroke = LayerStyle.Stroke;
            poly.StrokeThickness = LayerStyle.StrokeWeight;
            poly.StrokeLineJoin = PenLineJoin.Bevel;
            this.AddFeatureChildren(f, poly);
        }

        protected virtual void AddPolygon(Point[] points, IFeature f)
        {
            Polygon poly = new Polygon();
            points.ForEach(p => poly.Points.Add(p));
            poly.Stroke = LayerStyle.Stroke;
            poly.StrokeThickness = LayerStyle.StrokeWeight;
            poly.StrokeLineJoin = PenLineJoin.Bevel;
            poly.Fill = LayerStyle.GetFill(f);
            this.AddFeatureChildren(f, poly);
        }

        protected virtual void AddRoadStroke(Point[] points, IFeature f)
        {
            Polyline poly = new Polyline();
            points.ForEach(p => poly.Points.Add(p));
            poly.Stroke = LayerStyle.Stroke;
            poly.StrokeThickness = LayerStyle.StrokeWeight * 1.2;
            poly.StrokeLineJoin = PenLineJoin.Bevel;
            this.AddFeatureChildren(f, poly);
        }

        protected virtual void AddRoadFill(Point[] points, IFeature f)
        {
            Polyline poly = new Polyline();
            points.ForEach(p => poly.Points.Add(p));
            poly.Stroke = LayerStyle.GetFill(f);
            poly.StrokeThickness = LayerStyle.StrokeWeight;
            poly.StrokeLineJoin = PenLineJoin.Bevel;
            Children.Add(poly);
        }

        public Dictionary<Feature, VectorLayer> AllHitTest(Geometry.Extent2D extent)
        {
            Dictionary<Feature, VectorLayer> multiResult = new Dictionary<Feature, VectorLayer>();

            if (LayerData.GeoType == "1")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Point2D p = new Geometry.Point2D(f.GeoData);
                    if (extent.IsPointIn(p))
                    {
                        multiResult.Add(f, LayerData);
                    }
                }
            }
            else if (LayerData.GeoType == "2")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Polyline poly = new Geometry.Polyline(f.GeoData);
                    if (extent.IsPointIn(poly.GetExtent().max) && extent.IsPointIn(poly.GetExtent().min))
                    {
                        multiResult.Add(f, LayerData);
                    }
                }
            }
            else if (LayerData.GeoType == "4")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Polygon poly = new Geometry.Polygon(f.GeoData);
                    if (extent.IsPointIn(poly.GetExtent().max) && extent.IsPointIn(poly.GetExtent().min))
                    {
                        multiResult.Add(f, LayerData);
                    }
                }
            }

            return multiResult;
        }

        public Dictionary<Feature, VectorLayer> PartHitTest(Geometry.Extent2D extent)
        {
            Dictionary<Feature, VectorLayer> multiResult = new Dictionary<Feature, VectorLayer>();

            if (LayerData.GeoType == "1")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Point2D p = new Geometry.Point2D(f.GeoData);
                    if (extent.IsPointIn(p))
                    {
                        multiResult.Add(f, LayerData);
                    }
                }
            }
            else if (LayerData.GeoType == "2")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Polyline poly = new Geometry.Polyline(f.GeoData);
                    foreach (var point in poly.Points)
                    {
                        if (extent.IsPointIn(point))
                        {
                            multiResult.Add(f, LayerData);
                            break;
                        }
                    }
                }
            }
            else if (LayerData.GeoType == "4")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Polygon poly = new Geometry.Polygon(f.GeoData);
                    foreach (var point in poly.Points)
                    {
                        if (extent.IsPointIn(point))
                        {
                            multiResult.Add(f, LayerData);
                            break;
                        }
                    }
                }
            }

            return multiResult;
        }

        public Feature HitTest(Geometry.Point2D worldPos)
        {
            Feature result = null;
            if (LayerData.GeoType == "1")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Point2D p = new Geometry.Point2D(f.GeoData);
                    if (p.DistTo(worldPos) < 5 * MapControl.Current.Scale)
                    {
                        result = f;
                        break;
                    }
                }
            }
            else if (LayerData.GeoType == "2")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Polyline poly = new Geometry.Polyline(f.GeoData);
                    if (poly.DistToPoint(worldPos) < 5 * MapControl.Current.Scale)
                    {
                        result = f;
                        break;
                    }
                }
            }
            else if (LayerData.GeoType == "4")
            {
                foreach (Feature f in LayerData.Features)
                {
                    Geometry.Polygon poly = new Geometry.Polygon(f.GeoData);
                    if (poly.IsPointIn(worldPos))
                    {
                        result = f;
                        break;
                    }
                }
            }
            return result;
        }
    }

    /// <summary>
    /// 元素位置助手
    /// </summary>
    public static class ElementPositionHelper
    {
        //public static void CenterElementInCanvas(FrameworkElement element)
        //{
        //    Point pos = GetElementDesignPosition(element);
        //    if (!IsPointNull(pos))
        //    {
        //        Canvas.SetLeft(element, pos.X - element.ActualWidth / 2);  // wpf与silverlight不同之处，wpf此处取不到实际宽度。
        //        Canvas.SetTop(element, pos.Y - element.ActualHeight / 2);
        //    }
        //}

        public static void CenterElementInCanvas(FrameworkElement element, double width, double height)
        {
            Point pos = GetElementDesignPosition(element);
            if (!IsPointNull(pos))
            {
                element.Width = width;
                element.Height = height;
                Canvas.SetLeft(element, pos.X - width / 2);
                Canvas.SetTop(element, pos.Y - height / 2);
            }
        }

        private static Point _pointNull = new Point(double.NaN, double.NaN);

        public static Point GetElementDesignPosition(FrameworkElement element)
        {
            if (element.Tag != null)
            {
                return (Point)(element.Tag as Dictionary<string, object>)["DesignPosition"];
            }
            else
            {
                return _pointNull;
            }
        }

        public static object GetElementData(FrameworkElement element, string key)
        {
            if (element.Tag != null)
            {
                return (element.Tag as Dictionary<string, object>)[key];
            }
            else
            {
                return null;
            }
        }

        public static void SetElementDesignPosition(FrameworkElement element, Point pos)
        {
            if (element.Tag == null)
            {
                element.Tag = new Dictionary<string, object>();
            }
            Dictionary<string, object> dict = element.Tag as Dictionary<string, object>;
            if (dict.Keys.Contains("DesignPosition"))
            {
                dict["DesignPosition"] = pos;
            }
            else
            {
                dict.Add("DesignPosition", pos);
            }
        }

        public static void SetElementData(FrameworkElement element, string key, object value)
        {
            if (element.Tag == null)
            {
                element.Tag = new Dictionary<string, object>();
            }
            Dictionary<string, object> dict = element.Tag as Dictionary<string, object>;
            if (dict.Keys.Contains(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static bool IsPointNull(Point p)
        {
            return double.IsNaN(p.X);
        }
    }
}
