using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Linq;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace TongJi.Gis.Display
{
    /// <summary>
    /// 地图控件
    /// </summary>
    public class MapControl : Canvas
    {
        public Map Map { get; private set; }
        public List<Map> Maps { get; private set; }

        public List<MapLayer> Layers { get; private set; }
        public List<MapLayer> TempLayers { get { return new List<MapLayer> { SelectionLayer, GridLayer, MarkLayer, BaseLayer, AnimationLayer }; } }
        public List<MapLayer> LabelLayers { get { return Layers.Where(x => x.LabelLayer != null).Select(x => x.LabelLayer).ToList(); } }

        public Point Origin { get; private set; }
        public double Scale { get; set; }
        public double InitialScale { get; private set; }

        public static MapControl Current { get; private set; }
        public bool IsMapInitialized { get { return Map != null; } }
        private static double[] _zoomLevels; // = new double[] { 4096, 2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625, 0.0078125, 0.00390625, 0.001953125, 0.0009765625, 0.00048828125 };
        private DateTime _time = new DateTime(1, 1, 1, 1, 0, 0, 0, 0);

        public MapLayer SelectionLayer { get; private set; }
        public MapLayer MarkLayer { get; private set; }
        public MapLayer GridLayer { get; private set; }
        public MapLayer BaseLayer { get; private set; }
        public DrawingMapLayer AnimationLayer { get; private set; } // newly 20130505

        public VectorLayer HitTestLayerResult { get; private set; }
        public Feature HitTestFeatureResult { get; private set; }
        public Dictionary<Feature, VectorLayer> HitTestMultiResult { get; private set; }

        static MapControl() // newly 20130403
        {
            _zoomLevels = Enumerable.Range(-15, 31).Select(i => Math.Pow(2, i)).Reverse().ToArray();
        }

        public MapControl()
        {
            Scale = 8;

            Maps = new List<Gis.Map>();
            SelectionLayer = new MapLayer();
            MarkLayer = new MapLayer();
            GridLayer = new MapLayer();
            BaseLayer = new MapLayer();
            AnimationLayer = new DrawingMapLayer();
            Canvas.SetZIndex(MarkLayer, 100);
            Canvas.SetZIndex(BaseLayer, -100);
            Layers = new List<MapLayer>();

            Current = this;
            HitTestMultiResult = new Dictionary<Feature, VectorLayer>();

            base.MouseMove += new MouseEventHandler(MapControl_MouseMove);
            base.MouseLeftButtonDown += new MouseButtonEventHandler(MapControl_MouseLeftButtonDown);
            base.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseLeftButtonUp);
            base.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);
            base.MouseRightButtonUp += (s, arg) => OnNeedToInitializeViewerTools();
            base.MouseDown += new MouseButtonEventHandler(MapControl_MouseDown);
            base.MouseUp += new MouseButtonEventHandler(MapControl_MouseUp);

            //CacheMode = new BitmapCache();
        }

        #region Events

        public event Action NeedToInitializeViewerTools;
        protected void OnNeedToInitializeViewerTools()
        {
            if (NeedToInitializeViewerTools != null)
            {
                NeedToInitializeViewerTools();
            }
        }

        public event Action LayersChanged;
        protected void OnLayersChanged()
        {
            if (LayersChanged != null)
            {
                LayersChanged();
            }
        }

        public event Action ViewChanged;
        protected void OnViewChanged()
        {
            if (ViewChanged != null)
            {
                ViewChanged();
            }
        }

        public event Action ChildrenChanged;
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            if (ChildrenChanged != null)
            {
                ChildrenChanged();
            }
        }

        #endregion

        #region Map data

        public int ElementCount
        {
            get
            {
                return Layers.Sum(x => x.ElementCount);
            }
        }

        public Geometry.Extent2D ViewerExtents
        {
            get
            {
                Point pt1 = new Point(0, 0);
                Point pt2 = new Point(this.ActualWidth, this.ActualHeight);
                Geometry.Point2D p1 = GetWorldCoord(pt1);
                Geometry.Point2D p2 = GetWorldCoord(pt2);
                return new Geometry.Extent2D(p1, p2);
            }
        }

        public IEnumerable<Map> AllMaps
        {
            get
            {
                if (Map != null)
                {
                    yield return Map;
                }
                foreach (var map in Maps)
                {
                    yield return map;
                }
            }
        }

        public List<Tuple<Map, List<MapLayer>>> MapAndLayers
        {
            get
            {
                List<Tuple<Map, List<MapLayer>>> result = new List<Tuple<Map, List<MapLayer>>>();
                foreach (var map in AllMaps)
                {
                    result.Add(Tuple.Create(map, Layers.Where(x => map.Layers.Contains(x.LayerData)).ToList()));
                }
                return result;
            }
        }

        public MapLayer AddLayer(VectorLayer layer)
        {
            MapLayer mLayer = new DrawingMapLayer(); // mod 20130308
            //MapLayer mLayer = new MapLayer();
            mLayer.SetData(layer);
            MapControl.Current.Layers.Add(mLayer);
            MapControl.Current.Children.Add(mLayer);
            OnLayersChanged();
            RenderLayers();
            return mLayer;
        }

        public void InitializeMap(Map map)
        {
            Map = map;
            Layers.Clear();
            Children.Clear();
            TempLayers.ForEach(x => x.Children.Clear());
            LabelLayers.ForEach(x => x.Children.Clear());
            
            foreach (VectorLayer layer in map.Layers)
            {
                MapLayer mLayer = new DrawingMapLayer(); // mod 20130302
                //MapLayer mLayer = new MapLayer();
                mLayer.SetData(layer);
                mLayer.AppendProperties(); // newly 20120319
                Layers.Add(mLayer);
                Children.Add(mLayer);
            }

            TempLayers.ForEach(x => this.Children.Add(x));
            LabelLayers.ForEach(x => this.Children.Add(x));
            this.ZoomExtents();
            InitialScale = Scale;
            OnLayersChanged();
        }

        #endregion

        #region Event handlers

        void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point basePoint = e.GetPosition(this);
            int index = FindScaleIndex(Scale);
            index += e.Delta / 120;
            if (index > _zoomLevels.Length - 1) index = _zoomLevels.Length - 1;
            else if (index < 0) index = 0;
            double scale = _zoomLevels[index];
            ScaleCanvas(scale, basePoint);
        }

        void MapControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (ViewerToolManager.OverlayTools.Any(x => x is PanCanvasTool))
                {
                    ViewerToolManager.RemoveTool(ViewerToolManager.OverlayTools.First(x => x is PanCanvasTool));
                }
            }
        }

        void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (!(ViewerToolManager.ExclusiveTool is PanCanvasTool))
                {
                    PanCanvasTool pct = new PanCanvasTool();
                    pct.StartDrag(e.GetPosition(MapControl.Current));
                    ViewerToolManager.AddTool(pct);
                }
            }
        }

        void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(t => t.MouseLUpHandler(sender, e));
        }

        void MapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(t => t.MouseLDownHandler(sender, e));

            DateTime nextTime = DateTime.Now;
            TimeSpan span = nextTime - _time;
            if (span.TotalMilliseconds <= 300)
            {
                ViewerToolManager.Tools.ForEach(t => t.MouseLDoubleClickHandler(sender, e));
            }

            _time = nextTime;
        }

        void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(t => t.MouseMoveHandler(sender, e));
        }

        public void MapControl_KeyDown(object sender, KeyEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(t => t.KeyDownHandler(sender, e));
        }

        #endregion

        #region View transforms

        public double GetMagFactor(double scale)
        {
            return scale / 2;
        }

        public void PanCanvas(Point vector)
        {
            Origin = new Point(Origin.X + vector.X, Origin.Y + vector.Y);
            RenderLayers();
        }

        public void ScaleCanvas(double scale, Point basePoint)
        {
            double scale0 = this.Scale;
            Geometry.Vector2D v = new Geometry.Vector2D(basePoint.X - Origin.X, basePoint.Y - Origin.Y);
            Geometry.Vector2D v1 = (scale0 / scale) * v;
            Geometry.Vector2D v2 = v - v1;

            Scale = scale;
            Origin = new Point(Origin.X + v2.x, Origin.Y + v2.y);

            RenderLayers(true);
        }

        public void LocateCanvas(Geometry.Point2D location)
        {
            double ext = 500;
            Geometry.Point2D pt1 = new Geometry.Point2D(location.x + ext, location.y + ext);
            Geometry.Point2D pt2 = new Geometry.Point2D(location.x - ext, location.y - ext);
            Geometry.Extent2D extent = new Geometry.Extent2D(pt2, pt1);

            this.Zoom(extent);
        }

        private int FindScaleIndex(double scale)
        {
            for (int i = 0; i < _zoomLevels.Length; i++)
            {
                if (scale > _zoomLevels[i] * 0.75)
                {
                    return i;
                }
            }
            return _zoomLevels.Length - 1;
        }

        public void ZoomExtents()
        {
            if (Map == null)
            {
                return;
            }
            Zoom(Map.GetExtents());
        }

        public void Zoom(Geometry.Extent2D extents)
        {
            Scale = Math.Max(extents.XRange / this.ActualWidth, extents.YRange / this.ActualHeight);
            Origin = new Point(this.ActualWidth / 2 - extents.Center.x / Scale, this.ActualHeight / 2 + extents.Center.y / Scale);
            this.RenderLayers(true);
        }

        public void CenterCanvas()
        {
            var extents = Map.GetExtents();
            Origin = new Point(this.ActualWidth / 2 - extents.Center.x / Scale, this.ActualHeight / 2 + extents.Center.y / Scale);
            this.RenderLayers();
        }

        public void RenderLayers(bool modScale = false) // mod 20130226
        {
            TranslateTransform translate = new TranslateTransform { X = Origin.X, Y = Origin.Y };
            ScaleTransform scale = new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = -1 / Scale };
            TransformGroup MapTransform = new TransformGroup(); // mod 20130301
            MapTransform.Children.Add(scale);
            MapTransform.Children.Add(translate);
            //MapTransform.Freeze();

            double magFactor = GetMagFactor(Scale);
            foreach (MapLayer layer in Layers) // mod 20130302 精简，通过MapLayer.SetMagFactor()
            {
                layer.RenderTransform = MapTransform;
                if (modScale)
                {
                    layer.SetMagFactor(magFactor);
                }
            }

            TempLayers.ForEach(x => x.RenderTransform = MapTransform);
            LabelLayers.ForEach(x => x.RenderTransform = MapTransform);
            base.Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };

            OnViewChanged();
            ViewerToolManager.Tools.ForEach(t => t.Render());
        }

        public void SaveImage(string fileName, int width = 0, int height = 0) // newly 20130306
        {
            if (width == 0 || height == 0)
            {
                width = (int)this.ActualWidth;
                height = (int)this.ActualHeight;
            }
            double dpi = 96;
            double mag = dpi / 96;
            double dpix = width / this.ActualWidth * dpi;
            double dpiy = height / this.ActualHeight * dpi;
            RenderTargetBitmap bmp = new RenderTargetBitmap(Convert.ToInt32(width * mag), Convert.ToInt32(height * mag), dpix, dpiy, PixelFormats.Pbgra32);
            bmp.Render(this);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.OpenOrCreate))
            {
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
            }
        }

        #endregion

        #region Tools

        private Shape GetMark(Point pt)
        {
            Path shape = new Path();
            PathGeometry path = new PathGeometry();
            PathFigure pathf = new PathFigure();
            pathf.StartPoint = new Point(pt.X, pt.Y);
            LineSegment lineSeg1 = new LineSegment();
            lineSeg1.Point = new Point(pt.X - 5, pt.Y - 8);
            pathf.Segments.Add(lineSeg1);
            ArcSegment arcSeg = new ArcSegment();
            arcSeg.Point = new Point(pt.X + 5, pt.Y - 8);
            arcSeg.Size = new Size(10, 12);
            arcSeg.IsLargeArc = true;
            arcSeg.SweepDirection = SweepDirection.Clockwise;
            pathf.Segments.Add(arcSeg);
            LineSegment lineSeg2 = new LineSegment();
            lineSeg2.Point = new Point(pt.X, pt.Y);
            pathf.Segments.Add(lineSeg2);
            path.Figures.Add(pathf);
            shape.Data = path;
            shape.Fill = new SolidColorBrush(new Color { A = 255, R = 255, G = 0, B = 0 });
            shape.Stroke = new SolidColorBrush(new Color { A = 255, R = 255, G = 200, B = 0 });
            shape.StrokeThickness = 1;
            return shape;
        }

        public void Mark(IEnumerable<Geometry.Point2D> pts)
        {
            MarkLayer.Children.Clear();
            pts.ForEach(p =>
            {
                Shape shape = GetMark(new Point(p.x, p.y));
                MarkLayer.Children.Add(shape);
            });
        }

        public Geometry.Point2D GetWorldCoord(Point canvasCoord)
        {
            double x = (canvasCoord.X - Origin.X) * Scale;
            double y = (Origin.Y - canvasCoord.Y) * Scale;
            return new Geometry.Point2D(x, y);
        }

        public Point GetCanvasCoord(Geometry.Point2D worldCoord)
        {
            double x = worldCoord.x / Scale + Origin.X;
            double y = Origin.Y - worldCoord.y / Scale;
            return new Point(x, y);
        }

        public bool HitTest(Point canvasPos)
        {
            Geometry.Point2D worldPos = GetWorldCoord(canvasPos);
            var layers = Enumerable.Reverse(Layers).ToList(); // test from top layers
            foreach (MapLayer layer in layers)
            {
                if (layer.Visibility == Visibility.Visible)
                {
                    Feature f = layer.HitTest(worldPos);
                    if (f != null)
                    {
                        HitTestLayerResult = layer.LayerData;
                        HitTestFeatureResult = f;

                        HitTestMultiResult.Clear();
                        HitTestMultiResult.Add(HitTestFeatureResult, HitTestLayerResult);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool AllHitTest(Geometry.Extent2D extent)
        {
            bool flag = false;

            if (HitTestMultiResult.Count != 0)
            {
                HitTestMultiResult.Clear();
            }

            var layers = Enumerable.Reverse(Layers).ToList(); // test from top layers
            foreach (MapLayer layer in layers)
            {
                if (layer.Visibility == Visibility.Visible)
                {
                    Dictionary<Feature, VectorLayer> tempResult = layer.AllHitTest(extent);
                    if (tempResult.Count != 0)
                    {
                        flag = true;
                        foreach (var element in tempResult)
                        {
                            HitTestMultiResult.Add(element.Key, element.Value);
                        }
                    }
                }
            }

            return flag;
        }

        public bool PartHitTest(Geometry.Extent2D extent)
        {
            bool flag = false;

            if (HitTestMultiResult.Count != 0)
            {
                HitTestMultiResult.Clear();
            }

            var layers = Enumerable.Reverse(Layers).ToList(); // test from top layers
            foreach (MapLayer layer in layers)
            {
                if (layer.Visibility == Visibility.Visible)
                {
                    Dictionary<Feature, VectorLayer> tempResult = layer.PartHitTest(extent);
                    if (tempResult.Count != 0)
                    {
                        flag = true;
                        foreach (var element in tempResult)
                        {
                            HitTestMultiResult.Add(element.Key, element.Value);
                        }
                    }
                }
            }

            return flag;
        }

        public List<MapLayer> FindMapLayers(string layerName) // todo: 使用此函数代替现有用法。
        {
            return Layers.Where(x => x.LayerData.Name == layerName).ToList();
        }

        #endregion
    }
}
