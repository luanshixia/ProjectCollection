using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Display
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

        private void AddFeatureChildren(IFeature f, Shape shape)
        {
            Features.Add(f, shape);
            Children.Add(shape);
        }

        private void AddOverlayChildren(FrameworkElement fe)
        {
            Overlays.Add(fe);
            Children.Add(fe);
        }

        private void ClearOverlayChildren()
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
            if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                var pos = feature.GeoData[0];
                AddSpot(new Point(pos.X, pos.Y), feature);
                AddLable(new Point(pos.X, pos.Y), LayerLableStyle.GetLable(feature));
            }
            else if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                var poly = new PointString(feature.GeoData);
                if (IsRoad())
                {
                    AddRoadStroke(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                }
                if (!IsRoad())
                {
                    AddPolyline(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                }
                else
                {
                    AddRoadFill(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                }

                string text = LayerLableStyle.GetLable(feature);
                double linearRepeatInterval = LayerLableStyle.LinearRepeatInterval;
                double length = poly.Length();
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
                    var tempPt = poly.GetPointAtDist(position);
                    double rotation = poly.Dir(position).Heading();
                    double angle = 180 / Math.PI * rotation;
                    AddLable(new Point(tempPt.X, tempPt.Y), text, angle);
                }
            }
            else if (LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                var poly = new PointString(feature.GeoData);
                if (poly.Points.Count >= 3)
                {
                    var center = poly.Centroid(); // mod 20130507
                    AddPolygon(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                    AddLable(new Point(center.X, center.Y), LayerLableStyle.GetLable(feature));                   
                }
            }
        }

        public virtual void AddFeature(IFeature feature)
        {
            if (LayerData != null)
            {
                if ((LayerData.GeoType == VectorLayer.GEOTYPE_REGION) && (feature.GeoData.Count <= 2)) // 非法数据检测
                {
                    return;
                }
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
                }
                Features.Remove(feature);
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

            if (layer.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                foreach (Feature feature in layer.Features)
                {
                    var pos = feature.GeoData[0];
                    AddSpot(new Point(pos.X, pos.Y), feature);
                    AddLable(new Point(pos.X, pos.Y), LayerLableStyle.GetLable(feature));
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                if (IsRoad())
                {
                    foreach (Feature feature in layer.Features)
                    {
                        var polyline = new PointString(feature.GeoData);
                        AddRoadStroke(polyline.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                    }
                }
                foreach (Feature feature in layer.Features)
                {
                    var poly = new PointString(feature.GeoData);
                    if (!IsRoad())
                    {
                        AddPolyline(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                    }
                    else
                    {
                        AddRoadFill(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                    }
                }
                foreach (Feature feature in layer.Features) // mod 20130516 最后统一加标签
                {
                    var poly = new PointString(feature.GeoData);
                    string text = LayerLableStyle.GetLable(feature);
                    double linearRepeatInterval = LayerLableStyle.LinearRepeatInterval;
                    double length = poly.Length();
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
                    foreach (double position in positions)
                    {
                        var tempPt = poly.GetPointAtDist(position);
                        double rotation = poly.Dir(position).Heading();
                        double angle = 180 / Math.PI * rotation;
                        AddLable(new Point(tempPt.X, tempPt.Y), text, angle);
                    }
                }
            }
            else if (layer.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                foreach (Feature feature in layer.Features)
                {
                    var poly = new PointString(feature.GeoData);
                    var center = poly.Centroid();
                    AddPolygon(poly.Points.Select(p => new Point(p.X, p.Y)).ToArray(), feature);
                    AddLable(new Point(center.X, center.Y), LayerLableStyle.GetLable(feature));
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
            if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                foreach (var f in LayerData.Features)
                {
                    f.Properties["点实体-位置"] = f.GeoData[0].ToString();
                }
            }
            else if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                foreach (var f in LayerData.Features)
                {
                    f.Properties["线实体-长度"] = new PointString(f.GeoData).Length().ToString("0.00");
                }
            }
            else if (LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                foreach (var f in LayerData.Features)
                {
                    var poly = new PointString(f.GeoData);
                    f.Properties["面实体-面积"] = poly.Area().ToString("0.00");
                }
            }
        }

        public virtual void SetMagFactor(double magFactor)
        {
            if (this.LayerData.GeoType == VectorLayer.GEOTYPE_POINT && !_hasSizeTheme)
            {
                foreach (Ellipse ep in this.Features.Values)
                {
                    double size = this.LayerStyle.SpotSize * magFactor;
                    ep.StrokeThickness = this.LayerStyle.StrokeWeight * magFactor;
                    ElementPositionHelper.CenterElementInCanvas(ep, size, size);
                }
            }
            else if (this.LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR && !IsRoad())
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
            else if (this.LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
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

                if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
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
            if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                for (int i = 0; i < Features.Count; i++)
                {
                    var feature = LayerData.Features[i];
                    var shape = Features[feature];

                    double size = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * theme.GetSize(feature);
                    ElementPositionHelper.CenterElementInCanvas(shape, size, size);
                }
            }
            else if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
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
        }

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
                if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT || LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
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
                if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
                {
                    double size = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * LayerStyle.SpotSize;
                    ElementPositionHelper.CenterElementInCanvas(shape, size, size);
                }
                else if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
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
                if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
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

        public void BringToFront(Shape shape)
        {
            Canvas.SetZIndex(shape, 10);
        }

        public void BringToBack(Shape shape)
        {
            Canvas.SetZIndex(shape, -10);
        }
    }

    /// <summary>
    /// 元素位置助手
    /// </summary>
    public static class ElementPositionHelper
    {
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
