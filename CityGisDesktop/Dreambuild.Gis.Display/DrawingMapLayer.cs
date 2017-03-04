using System;
using System.Collections.Generic;
using System.Linq;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// 地图图层，使用WPF Drawing渲染
    /// </summary>
    public class DrawingMapLayer : MapLayer
    {
        public new Dictionary<IFeature, GeometryDrawing> Features { get; protected set; }
        public new List<Drawing> Overlays { get; protected set; } // newly 20130308

        protected DrawingVisual _visual = new DrawingVisual();
        protected DrawingGroup _drawingGroup = new DrawingGroup();
        public DrawingGroup DrawingGroup { get { return _drawingGroup; } }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            return _visual;
        }

        public DrawingMapLayer()
            : base()
        {
            AddVisualChild(_visual);
            AddLogicalChild(_visual);
            Features = new Dictionary<IFeature, GeometryDrawing>();
            Overlays = new List<Drawing>();

            using (DrawingContext dc = _visual.RenderOpen())
            {
                dc.DrawDrawing(_drawingGroup);
            }
        }

        public override int ElementCount
        {
            get
            {
                return _drawingGroup.Children.Count;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}|{1}", LayerData.Name, Features.Count);
        }

        private void AddFeatureChildren(IFeature f, GeometryDrawing drawing)
        {
            Features.Add(f, drawing);
            _drawingGroup.Children.Add(drawing);
        }

        private void AddOverlayChildren(Drawing drawing)
        {
            Overlays.Add(drawing);
            _drawingGroup.Children.Add(drawing);
        }

        private void ClearOverlayChildren()
        {
            Overlays.ForEach(x => _drawingGroup.Children.Remove(x));
            Overlays.Clear();
        }

        public override void ClearData()
        {
            _visual.Children.Clear();
        }

        public override void RemoveFeature(IFeature feature)
        {
            if (LayerData != null)
            {
                LayerData.Features.Remove(feature);
                if (Features.ContainsKey(feature))
                {
                    _drawingGroup.Children.Remove(Features[feature]);
                }
                Features.Remove(feature);
            }
        }

        public override void SetData(VectorLayer layer)
        {
            _visual.Children.Clear();
            Features.Clear();
            Overlays.Clear();

            base.SetData(layer);
        }

        public override void SetMagFactor(double magFactor)
        {
            double strokeWeight = this.LayerStyle.StrokeWeight * magFactor;
            double radius = this.LayerStyle.SpotSize * magFactor / 2;
            if (this.LayerData.GeoType == VectorLayer.GEOTYPE_POINT && !_hasSizeTheme)
            {
                foreach (GeometryDrawing drawing in Features.Values)
                {
                    EllipseGeometry geometry = drawing.Geometry as EllipseGeometry;
                    geometry.RadiusX = radius;
                    geometry.RadiusY = radius;
                    drawing.Pen.Thickness = strokeWeight;
                }
            }
            else if (this.LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR && this.LayerData.Name != "道路")
            {
                foreach (GeometryDrawing drawing in Features.Values)
                {
                    drawing.Pen.Thickness = strokeWeight;
                }
            }
            else if (this.LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
            {
                foreach (GeometryDrawing drawing in Features.Values)
                {
                    drawing.Pen.Thickness = strokeWeight;
                }
            }
        }

        public override void ApplyColorTheme(IColorTheme theme)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var drawing = Features[feature];
 
                if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
                {
                    drawing.Pen.Brush = new SolidColorBrush(theme.GetColor(feature));
                }
                else
                {
                    drawing.Brush = new SolidColorBrush(theme.GetColor(feature));
                    drawing.Pen.Brush = Brushes.Black;
                }
            }
        }

        public override void ApplySizeTheme(IDataSizeTheme theme)
        {
            if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
            {
                for (int i = 0; i < Features.Count; i++)
                {
                    var feature = LayerData.Features[i];
                    var drawing = Features[feature];
                    var geometry = drawing.Geometry as EllipseGeometry;

                    if (geometry != null)
                    {
                        double radius = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * theme.GetSize(feature) / 2;
                        geometry.RadiusX = radius;
                        geometry.RadiusY = radius;
                    }
                }
            }
            else if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                // done: 流量图
                for (int i = 0; i < Features.Count; i++)
                {
                    var feature = LayerData.Features[i];
                    var drawing = Features[feature];
                    double weight = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * theme.GetSize(feature);
                    drawing.Pen.Thickness = weight;
                }
            }
            _hasSizeTheme = true;
        }

        public override void ApplyFluidTheme(IDataFluidTheme theme)
        {
            if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
            {
                foreach (var feature in Features)
                {
                    var f = feature.Key;
                    var geometry = feature.Value.Geometry as PathGeometry;
                    var poly = new Geometry.PointString(f.GeoData);
                    double length = poly.Length();
                    if (length < 10)
                    {
                        continue;
                    }
                    double velocity = theme.GetVelocity(f);
                    double time = length / velocity;
                    double space = 1 / theme.GetDensity(f);
                    int spotCount = (int)(length / space) + 1;
                    var color = theme.GetColor(f);

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
                        this.AddOverlayChildren(spotDrawing);
                        spot.BeginAnimation(EllipseGeometry.CenterProperty, paup);
                        fill.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                    }
                }
            }
        }

        public override void ApplySpotChart(SpotChart chart)
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                for (int j = 0; j < chart.Columns.Count; j++)
                {
                    var column = chart.Columns[j];
                    this.AddOverlayChildren(column.GetDrawing(feature, j));
                }
            }
        }

        public override void ApplySpotLink(SpotLink link)
        {
        }

        public override void ClearColorTheme()
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var drawing = Features[feature];
                if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT || LayerData.GeoType == VectorLayer.GEOTYPE_REGION)
                {
                    drawing.Brush = LayerStyle.GetFill(feature);
                    if (!SelectionSet.Contents.Contains(feature))
                    {
                        drawing.Pen.Brush = LayerStyle.Stroke;
                    }
                }
            }
        }

        public override void ClearSizeTheme()
        {
            for (int i = 0; i < Features.Count; i++)
            {
                var feature = LayerData.Features[i];
                var drawing = Features[feature];
                if (LayerData.GeoType == VectorLayer.GEOTYPE_POINT)
                {
                    var geometry = drawing.Geometry as EllipseGeometry;
                    if (geometry != null)
                    {
                        double radius = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * LayerStyle.SpotSize / 2;
                        geometry.RadiusX = radius;
                        geometry.RadiusY = radius;
                    }
                }
                else if (LayerData.GeoType == VectorLayer.GEOTYPE_LINEAR)
                {
                    double weight = MapControl.Current.GetMagFactor(MapControl.Current.InitialScale) * LayerStyle.StrokeWeight;
                    drawing.Pen.Thickness = weight;
                }
            }
            _hasSizeTheme = false;
        }

        public override void ApplyToolTip(Func<IFeature, FrameworkElement> mapper)
        {
            // todo: drawing的工具提示
        }

        protected override void AddSpot(Point pos, IFeature f)
        {
            double radius = LayerStyle.SpotSize / 2;
            EllipseGeometry geometry = new EllipseGeometry(pos, radius, radius);
            GeometryDrawing drawing = new GeometryDrawing(LayerStyle.GetFill(), new Pen(LayerStyle.Stroke, LayerStyle.StrokeWeight), geometry);
            this.AddFeatureChildren(f, drawing);
        }

        protected override void AddPolyline(Point[] points, IFeature f)
        {
            AddPolylineInternal(points, f, null, LayerStyle.Stroke, LayerStyle.StrokeWeight);
        }

        protected override void AddPolygon(Point[] points, IFeature f)
        {
            if (points.First() != points.Last()) // newly 20140624
            {
                var pts = points.ToList();
                pts.Add(pts.First());
                points = pts.ToArray();
            }
            AddPolylineInternal(points, f, LayerStyle.GetFill(), LayerStyle.Stroke, LayerStyle.StrokeWeight);
        }

        protected override void AddRoadStroke(Point[] points, IFeature f)
        {
            AddPolylineInternal(points, f, null, LayerStyle.Stroke, LayerStyle.StrokeWeight * 1.2);
        }

        protected override void AddRoadFill(Point[] points, IFeature f)
        {
            AddPolylineInternal(points, f, null, LayerStyle.GetFill(), LayerStyle.StrokeWeight, false);
        }

        private void AddPolylineInternal(Point[] points, IFeature f, Brush fill, Brush stroke, double strokeWeight, bool addToFeatureList = true)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure { StartPoint = points.First() };
            PolyLineSegment segment = new PolyLineSegment(points, true);
            figure.Segments.Add(segment);
            geometry.Figures.Add(figure);
            GeometryDrawing drawing = new GeometryDrawing(fill, new Pen(stroke, strokeWeight) { LineJoin = PenLineJoin.Bevel }, geometry);
            _drawingGroup.Children.Add(drawing);
            if (addToFeatureList)
            {
                Features.Add(f, drawing);
            }
        }

        public void BringToFront(GeometryDrawing drawing)
        {
            _drawingGroup.Children.Remove(drawing);
            _drawingGroup.Children.Add(drawing);
        }

        public void BringToBack(GeometryDrawing drawing)
        {
            _drawingGroup.Children.Remove(drawing);
            _drawingGroup.Children.Insert(0, drawing);
        }
    }
}
