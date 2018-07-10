using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// The map control.
    /// </summary>
    public class MapControl : Canvas
    {
        #region Properties and constructors

        public Map Map { get; private set; }
        public List<Map> Maps { get; private set; }

        public List<MapLayer> Layers { get; private set; }
        public List<MapLayer> TempLayers => new List<MapLayer> { SelectionLayer, GridLayer, MarkLayer, BaseLayer, AnimationLayer };
        public List<MapLayer> LabelLayers => Layers.Where(x => x.LabelLayer != null).Select(x => x.LabelLayer).ToList();

        public Point Origin { get; private set; }
        public double Scale { get; set; }
        public double InitialScale { get; private set; }

        public static MapControl Current { get; private set; }
        public bool IsMapInitialized { get { return Map != null; } }
        private static double[] _zoomLevels = Enumerable.Range(-15, 31).Select(i => Math.Pow(2, i)).Reverse().ToArray(); // newly 20130403

        public MapLayer SelectionLayer { get; private set; }
        public MapLayer MarkLayer { get; private set; }
        public MapLayer GridLayer { get; private set; }
        public MapLayer BaseLayer { get; private set; }
        public DrawingMapLayer AnimationLayer { get; private set; } // newly 20130505

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

            base.MouseMove += new MouseEventHandler(MapControl_MouseMove);
            base.MouseLeftButtonDown += new MouseButtonEventHandler(MapControl_MouseLeftButtonDown);
            base.MouseLeftButtonUp += new MouseButtonEventHandler(MapControl_MouseLeftButtonUp);
            base.MouseWheel += new MouseWheelEventHandler(MapControl_MouseWheel);
            base.MouseRightButtonUp += (s, arg) => OnNeedToInitializeViewerTools();
            base.MouseDown += new MouseButtonEventHandler(MapControl_MouseDown);
            base.MouseUp += new MouseButtonEventHandler(MapControl_MouseUp);

            //CacheMode = new BitmapCache();
        }

        #endregion

        #region Events

        public event Action NeedToInitializeViewerTools;
        protected void OnNeedToInitializeViewerTools()
        {
            NeedToInitializeViewerTools?.Invoke();
        }

        public event Action LayersChanged;
        protected void OnLayersChanged()
        {
            LayersChanged?.Invoke();
        }

        public event Action ViewChanged;
        protected void OnViewChanged()
        {
            ViewChanged?.Invoke();
        }

        public event Action ChildrenChanged;
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            ChildrenChanged?.Invoke();
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

        public Extents ViewerExtents
        {
            get
            {
                Point pt1 = new Point(0, 0);
                Point pt2 = new Point(this.ActualWidth, this.ActualHeight);
                var p1 = GetWorldCoord(pt1);
                var p2 = GetWorldCoord(pt2);
                return Extents.FromPoints(p1, p2);
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
                var result = new List<Tuple<Map, List<MapLayer>>>();
                foreach (var map in AllMaps)
                {
                    result.Add(Tuple.Create(map, Layers.Where(x => map.Layers.Contains(x.LayerData)).ToList()));
                }
                return result;
            }
        }

        public MapLayer AddLayer(VectorLayer layer)
        {
            var mLayer = new DrawingMapLayer(); // mod 20130308
            //MapLayer mLayer = new MapLayer();
            mLayer.SetData(layer);
            MapControl.Current.Layers.Add(mLayer);
            MapControl.Current.Children.Add(mLayer);
            OnLayersChanged();
            RenderLayers();
            return mLayer;
        }

        public void RemoveFeatures(IEnumerable<IFeature> features) // newly 20140624
        {
            foreach (var mLayer in Layers)
            {
                foreach (var feature in features)
                {
                    mLayer.RemoveFeature(feature);
                }
            }
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
                var mLayer = new DrawingMapLayer(); // mod 20130302
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

        public void UpdateMap() // newly 20140624
        {
            Layers.Clear();
            Children.Clear();
            TempLayers.ForEach(x => x.Children.Clear());
            LabelLayers.ForEach(x => x.Children.Clear());

            foreach (VectorLayer layer in Map.Layers)
            {
                var mLayer = new DrawingMapLayer(); // mod 20130302
                //MapLayer mLayer = new MapLayer();
                mLayer.SetData(layer);
                mLayer.AppendProperties(); // newly 20120319
                Layers.Add(mLayer);
                Children.Add(mLayer);
            }

            TempLayers.ForEach(x => this.Children.Add(x));
            LabelLayers.ForEach(x => this.Children.Add(x));
            RenderLayers();
        }

        #endregion

        #region Event handlers

        void MapControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var basePoint = e.GetPosition(this);
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
            ViewerToolManager.Tools.ForEach(t => t.MouseUpHandler(sender, e));
        }

        void MapControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                if (!(ViewerToolManager.ExclusiveTool is PanCanvasTool))
                {
                    var panCanvasTool = new PanCanvasTool();
                    panCanvasTool.StartDrag(e.GetPosition(MapControl.Current));
                    ViewerToolManager.AddTool(panCanvasTool);
                }
            }
            ViewerToolManager.Tools.ForEach(t => t.MouseDownHandler(sender, e));
        }

        void MapControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(t => t.MouseLUpHandler(sender, e));
        }

        void MapControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(t => t.MouseLDownHandler(sender, e));

            if (e.ClickCount >= 2)
            {
                ViewerToolManager.Tools.ForEach(t => t.MouseLDoubleClickHandler(sender, e));
            }
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

        public void PanCanvas(System.Windows.Vector displacement)
        {
            this.Origin += displacement;
            this.RenderLayers();
        }

        public void ScaleCanvas(double newScale, Point basePoint)
        {
            var v1 = basePoint - this.Origin;
            var v2 = (this.Scale / newScale) * v1;

            this.Scale = newScale;
            this.Origin += v1 - v2;

            this.RenderLayers();
            this.AdjustMagFactor();
        }

        public void LocateCanvas(Geometry.Vector location)
        {
            double radius = 500;
            var pt1 = new Geometry.Vector(location.X + radius, location.Y + radius);
            var pt2 = new Geometry.Vector(location.X - radius, location.Y - radius);
            var extents = Extents.FromPoints(pt2, pt1);

            this.Zoom(extents);
        }

        private static int FindScaleIndex(double scale)
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

        public void Zoom(Extents extents)
        {
            Scale = Math.Max(extents.Range(0) / this.ActualWidth, extents.Range(1) / this.ActualHeight);
            Origin = new Point(this.ActualWidth / 2 - extents.Center().X / Scale, this.ActualHeight / 2 + extents.Center().Y / Scale);
            this.RenderLayers();
            this.AdjustMagFactor();
        }

        public void CenterCanvas()
        {
            var extents = Map.GetExtents();
            Origin = new Point(this.ActualWidth / 2 - extents.Center().X / Scale, this.ActualHeight / 2 + extents.Center().Y / Scale);
            this.RenderLayers();
        }

        public void AdjustMagFactor() // newly 20140804
        {
            double magFactor = GetMagFactor(Scale);
            foreach (MapLayer layer in Layers)
            {
                layer.SetMagFactor(magFactor);
            }
        }

        public void RenderLayers() // mod 20130226
        {
            var transform = new TransformGroup(); // mod 20130301
            transform.Children.Add(new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = -1 / Scale });
            transform.Children.Add(new TranslateTransform { X = Origin.X, Y = Origin.Y });
            //MapTransform.Freeze();

            foreach (MapLayer layer in Layers) // mod 20130302 精简，通过MapLayer.SetMagFactor()
            {
                layer.RenderTransform = transform;
            }

            TempLayers.ForEach(x => x.RenderTransform = transform);
            LabelLayers.ForEach(x => x.RenderTransform = transform);
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
            var bmp = new RenderTargetBitmap(Convert.ToInt32(width * mag), Convert.ToInt32(height * mag), dpix, dpiy, PixelFormats.Pbgra32);
            bmp.Render(this);

            using (var fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                encoder.Save(fs);
            }
        }

        #endregion

        #region Tools

        public Geometry.Vector GetWorldCoord(Point canvasCoord)
        {
            double x = (canvasCoord.X - Origin.X) * Scale;
            double y = (Origin.Y - canvasCoord.Y) * Scale;
            return new Geometry.Vector(x, y);
        }

        public Point GetCanvasCoord(Geometry.Vector worldCoord)
        {
            double x = worldCoord.X / Scale + Origin.X;
            double y = Origin.Y - worldCoord.Y / Scale;
            return new Point(x, y);
        }

        public List<MapLayer> FindMapLayers(params string[] layerNames) // todo: 使用此函数代替现有用法。// mod 20140618 support multiple names
        {
            return this.Layers
                .Where(x => layerNames.Contains(x.LayerData.Name))
                .ToList();
        }

        public FrameworkElement FindFeatureElement(IFeature feature) // newly 20140716
        {
            foreach (var layer in Layers)
            {
                if (layer.Features.ContainsKey(feature))
                {
                    return layer.Features[feature];
                }
            }
            return null;
        }

        public void BringToFront(IFeature feature)
        {
            foreach (var layer in Layers)
            {
                if (layer is DrawingMapLayer)
                {
                    var dlayer = layer as DrawingMapLayer;
                    if (dlayer.Features.ContainsKey(feature))
                    {
                        dlayer.BringToFront(dlayer.Features[feature]);
                    }
                }
                else
                {
                    if (layer.Features.ContainsKey(feature))
                    {
                        layer.BringToFront(layer.Features[feature]);
                    }
                }
            }
        }

        public void BringToBack(IFeature feature)
        {
            foreach (var layer in Layers)
            {
                if (layer is DrawingMapLayer)
                {
                    var dlayer = layer as DrawingMapLayer;
                    if (dlayer.Features.ContainsKey(feature))
                    {
                        dlayer.BringToBack(dlayer.Features[feature]);
                    }
                }
                else
                {
                    if (layer.Features.ContainsKey(feature))
                    {
                        layer.BringToBack(layer.Features[feature]);
                    }
                }
            }
        }

        #endregion
    }
}
