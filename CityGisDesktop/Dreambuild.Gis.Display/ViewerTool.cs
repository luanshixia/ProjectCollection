using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Dreambuild.Gis.Display
{
    /// <summary>
    /// The viewer tool manager.
    /// </summary>
    public static class ViewerToolManager
    {
        private static ViewerTool _exclusiveTool;
        public static ViewerTool ExclusiveTool
        {
            get
            {
                return _exclusiveTool;
            }
            set
            {
                if (_exclusiveTool != null)
                {
                    _exclusiveTool.ExitToolHandler();
                }
                _exclusiveTool = value;
                _exclusiveTool.EnterToolHandler();
            }
        }

        private static List<ViewerTool> _overlayTools = new List<ViewerTool>();
        public static ReadOnlyCollection<ViewerTool> OverlayTools
        {
            get
            {
                return _overlayTools.AsReadOnly();
            }
        }

        public static IEnumerable<ViewerTool> Tools
        {
            get
            {
                if (ExclusiveTool != null)
                {
                    yield return ExclusiveTool;
                }
                foreach (var tool in OverlayTools)
                {
                    yield return tool;
                }
            }
        }

        public static void AddTool(ViewerTool tool)
        {
            tool.EnterToolHandler();
            _overlayTools.Add(tool);
        }

        public static void RemoveTool(ViewerTool tool)
        {
            if (_overlayTools.Remove(tool))
            {
                tool.ExitToolHandler();
            }
        }

        public static void ClearTools()
        {
            _overlayTools.ForEach(x => x.ExitToolHandler());
            _overlayTools.Clear();
        }

        public static void SetFrameworkElement(FrameworkElement element)
        {
            element.MouseMove += (s, args) => Tools.ForEach(t => t.MouseMoveHandler(s, args));
            element.MouseDown += (s, args) =>
            {
                if (args.ClickCount >= 2)
                {
                    Tools.ForEach(t => t.MouseDoubleClickHandler(s, args));
                }
                else
                {
                    Tools.ForEach(t => t.MouseDownHandler(s, args));
                }
            };
            element.MouseUp += (s, args) => Tools.ForEach(t => t.MouseUpHandler(s, args));
            element.MouseWheel += (s, args) => Tools.ForEach(t => t.MouseWheelHandler(s, args));
            element.KeyDown += (s, args) => Tools.ForEach(t => t.KeyDownHandler(s, args));
        }
    }

    /// <summary>
    /// The viewer tool
    /// </summary>
    public abstract class ViewerTool
    {
        public virtual IEnumerable<UIElement> CanvasElements { get { yield break; } } // mod 20140707
        public virtual IEnumerable<UIElement> WorldElements { get { yield break; } } // newly 20140707
        public virtual ContextMenu ContextMenu { get { return null; } } // newly 20140623

        public virtual void Render()
        {
        }

        public virtual void MouseMoveHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseDownHandler(object sender, MouseButtonEventArgs e) // newly 20140721
        {
        }

        public virtual void MouseUpHandler(object sender, MouseButtonEventArgs e) // newly 20140721
        {
        }

        public virtual void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
        }

        public virtual void KeyDownHandler(object sender, KeyEventArgs e)
        {
        }

        public virtual void EnterToolHandler()
        {
            this.CanvasElements.ForEach(element => MapControl.Current.Children.Add(element));
            this.WorldElements.ForEach(element => MapControl.Current.MarkLayer.Children.Add(element));
            MapControl.Current.ContextMenu = this.ContextMenu;
        }

        public virtual void ExitToolHandler()
        {
            this.CanvasElements.ForEach(element => MapControl.Current.Children.Remove(element));
            this.WorldElements.ForEach(element => MapControl.Current.MarkLayer.Children.Remove(element));
            MapControl.Current.ContextMenu = null;
        }
    }

    /// <summary>
    /// The combined viewer tool.
    /// </summary>
    public class CombinedViewerTool : ViewerTool
    {
        private ViewerTool[] _tools;

        public CombinedViewerTool(params ViewerTool[] tools)
        {
            _tools = tools;
        }

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                return _tools.SelectMany(x => x.CanvasElements);
            }
        }

        public override IEnumerable<UIElement> WorldElements
        {
            get
            {
                return _tools.SelectMany(x => x.WorldElements);
            }
        }

        public override void Render()
        {
            _tools.ForEach(x => x.Render());
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            _tools.ForEach(x => x.MouseMoveHandler(sender, e));
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseDownHandler(sender, e));
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseUpHandler(sender, e));
        }

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseDoubleClickHandler(sender, e));
        }

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            _tools.ForEach(x => x.MouseWheelHandler(sender, e));
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            _tools.ForEach(x => x.KeyDownHandler(sender, e));
        }

        public override void EnterToolHandler()
        {
            _tools.ForEach(x => x.EnterToolHandler());
        }

        public override void ExitToolHandler()
        {
            _tools.ForEach(x => x.ExitToolHandler());
        }
    }

    /// <summary>
    /// Wheel zoom tool.
    /// </summary>
    public class WheelScalingTool : ViewerTool
    {
        private static readonly double[] ZoomLevels = Enumerable
            .Range(-15, 31)
            .Select(i => Math.Pow(2, i))
            .Reverse()
            .ToArray(); // newly 20130403

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            base.MouseWheelHandler(sender, e);

            var basePoint = e.GetPosition(MapControl.Current);
            int index = WheelScalingTool.FindScaleIndex(MapControl.Current.Scale);
            index += e.Delta / 120;
            if (index > ZoomLevels.Length - 1)
            {
                index = ZoomLevels.Length - 1;
            }
            else if (index < 0)
            {
                index = 0;
            }

            double scale = ZoomLevels[index];
            MapControl.Current.ScaleCanvas(scale, basePoint); // TODO: look into ScaleCanvas().
        }

        private static int FindScaleIndex(double scale)
        {
            for (int i = 0; i < ZoomLevels.Length; i++)
            {
                if (scale > ZoomLevels[i] * 0.75)
                {
                    return i;
                }
            }

            return ZoomLevels.Length - 1;
        }
    }

    /// <summary>
    /// Pan canvas tool.
    /// TODO: find other ways to support middle button pan.
    /// </summary>
    public class PanCanvasTool : ViewerTool
    {
        private Point PreviousPosition;

        public override ContextMenu ContextMenu
        {
            get
            {
                var menu = new ContextMenu();
                var mi = new MenuItem { Header = "DrawOrder: Move To Bottom" };
                mi.Click += DrawOrderMoveToBottom;
                menu.Items.Add(mi);
                return menu;
            }
        }

        void DrawOrderMoveToBottom(object sender, RoutedEventArgs e)
        {
            if (SelectionSet.Contents.Count > 0)
            {
                SelectionSet.Contents.ForEach(feature =>
                {
                    MapControl.Current.BringToBack(feature);
                });
            }
            else
            {
                MessageBox.Show("Select first.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(MapControl.Current);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MapControl.Current.PanCanvas(position - this.PreviousPosition);
            }

            this.PreviousPosition = position;
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
            {
                var pt = e.GetPosition(MapControl.Current);
                var pos = MapControl.Current.GetWorldCoord(pt);
                var queryResult = MapDataManager.LatestMap.QueryFeatures(SpatialQueryOperation.Point, pos, 5 * MapControl.Current.Scale).ToList();
                if (queryResult.Count > 0)
                {
                    var single = queryResult.Last();
                    SelectionSet.Select(new[] { single });
                }
                else
                {
                    SelectionSet.ClearSelection();
                }
            }
        }

        public override void EnterToolHandler()
        {
            base.EnterToolHandler();
            MapControl.Current.Cursor = Cursors.SizeAll;
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            MapControl.Current.Cursor = Cursors.Arrow;
        }
    }

    /// <summary>
    /// Point selection tool.
    /// </summary>
    public class SelectionTool : ViewerTool
    {
        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pt = e.GetPosition(MapControl.Current);
                var pos = MapControl.Current.GetWorldCoord(pt);
                var queryResult = MapDataManager.LatestMap
                    .QueryFeatures(SpatialQueryOperation.Point, pos, 5 * MapControl.Current.Scale)
                    .ToList();

                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) // mod 20130222
                {
                    if (queryResult.Count > 0)
                    {
                        var single = queryResult.Last();
                        if (SelectionSet.Contents.Contains(single))
                        {
                            SelectionSet.SubtractSelection(new[] { single });
                        }
                        else
                        {
                            SelectionSet.AddSelection(new[] { single });
                        }
                    }
                }
                else
                {
                    if (queryResult.Count > 0)
                    {
                        var single = queryResult.Last();
                        SelectionSet.Select(new[] { single });
                    }
                    else
                    {
                        SelectionSet.ClearSelection();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rect selection tool.
    /// </summary>
    public class RectSelectionTool : ViewerTool
    {
        private bool _isDragging = false;
        private Geometry.Vector _mouseDownOrigin;
        private Geometry.Vector _mouseDownEnd;
        private readonly Rectangle Rect = new Rectangle();
        private bool IsWindow => _mouseDownEnd.X >= _mouseDownOrigin.X;

        private static readonly SolidColorBrush BLACK = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush BLUE = new SolidColorBrush(new Color { A = 128, G = 128, B = 255 });
        private static readonly SolidColorBrush GREEN = new SolidColorBrush(new Color { A = 128, G = 255 });
        private static readonly DoubleCollection CONTINUOUS = new DoubleCollection { 1, 0 };
        private static readonly DoubleCollection DASHED = new DoubleCollection { 3, 3 };

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                yield return this.Rect;
            }
        }

        public override void Render()
        {
            Point p1 = MapControl.Current.GetCanvasCoord(_mouseDownOrigin);
            Point p2 = MapControl.Current.GetCanvasCoord(_mouseDownEnd);

            this.Rect.Width = Math.Abs(p1.X - p2.X);
            this.Rect.Height = Math.Abs(p1.Y - p2.Y);
            Canvas.SetLeft(this.Rect, Math.Min(p1.X, p2.X));
            Canvas.SetTop(this.Rect, Math.Min(p1.Y, p2.Y));

            if (IsWindow)
            {
                this.Rect.Stroke = BLACK;
                this.Rect.StrokeDashArray = CONTINUOUS;
                this.Rect.StrokeThickness = 1;
                this.Rect.Fill = BLUE;
            }
            else
            {
                this.Rect.Stroke = BLACK;
                this.Rect.StrokeDashArray = DASHED;
                this.Rect.StrokeThickness = 1;
                this.Rect.Fill = GREEN;
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                this.Rect.Visibility = Visibility.Visible;
                _mouseDownEnd = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                this.Render();
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isDragging == false)
                {
                    _isDragging = true;
                    _mouseDownOrigin = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                    return;
                }

                _mouseDownEnd = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                var extents = Extents.FromPoints(_mouseDownOrigin, _mouseDownEnd);
                var queryResult = MapDataManager.LatestMap
                    .QueryFeatures(IsWindow ? SpatialQueryOperation.Window : SpatialQueryOperation.Cross, extents, 0)
                    .ToArray();

                if (queryResult.Length > 0)
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                        && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) // mod 20140725
                    {
                        SelectionSet.SubtractSelection(queryResult);
                    }
                    else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) // mod 20120810
                    {
                        SelectionSet.AddSelection(queryResult);
                    }
                    else
                    {
                        SelectionSet.Select(queryResult); // mod 20120725
                    }
                }
                else
                {
                    SelectionSet.ClearSelection();
                }

                _isDragging = false;
                this.Rect.Visibility = Visibility.Collapsed;
            }
        }
    }

    /// <summary>
    /// Rect scaling tool.
    /// </summary>
    public class RectScaleTool : ViewerTool
    {
        private bool _isDragging = false;
        private bool _isMoving = false;
        private bool _isStarted = false;
        private Point _mouseDownOrigin;
        private Point _mouseDownEnd;
        private readonly Rectangle Rect = new Rectangle();

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isMoving = true;

                Rect.Visibility = Visibility.Visible;

                Point pos = e.GetPosition(MapControl.Current);
                _mouseDownEnd = pos;

                double width = Math.Abs(pos.X - _mouseDownOrigin.X);
                double height = Math.Abs(pos.Y - _mouseDownOrigin.Y);

                Rect.Width = width;
                Rect.Height = height;

                Canvas.SetLeft(Rect, Math.Min(_mouseDownOrigin.X, _mouseDownEnd.X));
                Canvas.SetTop(Rect, Math.Min(_mouseDownOrigin.Y, _mouseDownEnd.Y));

                Rect.Stroke = new SolidColorBrush(new Color { A = 255, R = 255 });
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_isStarted)
                {
                    _isStarted = true;
                    _isDragging = true;
                    _mouseDownOrigin = e.GetPosition(MapControl.Current);

                    MapControl.Current.Children.Add(Rect);
                }
                else
                {
                    _isStarted = false;
                }
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isStarted == false)
                {
                    if (_isMoving == true)
                    {
                        var pt1 = MapControl.Current.GetWorldCoord(_mouseDownOrigin);
                        var pt2 = MapControl.Current.GetWorldCoord(_mouseDownEnd);

                        var extents = Extents.FromPoints(pt1, pt2);
                        MapControl.Current.Zoom(extents);

                        _isMoving = false;
                    }

                    MapControl.Current.Children.Remove(Rect);
                    Rect.Visibility = Visibility.Collapsed;
                    _isDragging = false;
                }
            }
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            MapControl.Current.Children.Remove(Rect);
        }
    }

    /// <summary>
    /// Area measuring tool.
    /// </summary>
    public class AreaMeasureTool : ViewerTool
    {
        private bool _isDragging = false;
        private bool _isStarted = false;
        private bool _isEnded = false;

        private Geometry.Vector _mouseDownBegin;
        private Geometry.Vector _mouseDownOrigin;
        private Geometry.Vector _mouseDownEnd;

        private static readonly SolidColorBrush _strokeBrush = new SolidColorBrush(new Color { A = 255, B = 255, R = 128 });
        private static readonly SolidColorBrush _fillBrush = new SolidColorBrush(new Color { A = 64, B = 255, R = 128 });

        private readonly TextBlock _areaLabel = new TextBlock();
        private readonly Polygon _measurePolygon = new Polygon
        {
            Stroke = _strokeBrush,
            Fill = _fillBrush,
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Bevel
        };

        private List<Geometry.Vector> _clickPoints = new List<Geometry.Vector>();

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                yield return _measurePolygon;
                yield return _areaLabel;
            }
        }

        public override void Render()
        {
            _measurePolygon.Points.Clear();
            foreach (var pt in _clickPoints)
            {
                _measurePolygon.Points.Add(MapControl.Current.GetCanvasCoord(pt));
            }
            if (!_isEnded)
            {
                _measurePolygon.Points.Add(MapControl.Current.GetCanvasCoord(_mouseDownEnd));
            }

            if (_clickPoints.Count > 1)
            {
                Canvas.SetLeft(_areaLabel, MapControl.Current.GetCanvasCoord(_clickPoints.ElementAt(_clickPoints.Count - 1)).X + 10);
                Canvas.SetTop(_areaLabel, MapControl.Current.GetCanvasCoord(_clickPoints.ElementAt(_clickPoints.Count - 1)).Y + 15);
            }
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();

            _areaLabel.Text = string.Empty;
            _measurePolygon.Points.Clear();
            _clickPoints.Clear();
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!_isEnded && _isDragging)
            {
                _mouseDownEnd = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
            }
            this.Render();
        }

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            _isEnded = true;
            _isStarted = false;
            _isDragging = false;
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isEnded)
                {
                    _areaLabel.Text = string.Empty;
                    _measurePolygon.Points.Clear();
                    _clickPoints.Clear();

                    _isEnded = false;
                }

                _isStarted = true;
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isStarted && !_isEnded && !_isDragging)  // First click
                {
                    _isDragging = true;
                    _mouseDownOrigin = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                    _mouseDownBegin = _mouseDownOrigin;
                    _clickPoints.Add(_mouseDownBegin);
                }
                else if (_isEnded)  // Last click
                {
                    //_clickPoints.Remove(_mouseDownOrigin);
                    this.Render();

                    if (_clickPoints.Count > 1)
                    {
                        var areaPoly = new PointString(_clickPoints);
                        _areaLabel.Text = "Area: " + areaPoly.Area().ToString("0.000") + "m²";
                        Canvas.SetLeft(_areaLabel, MapControl.Current.GetCanvasCoord(_clickPoints.ElementAt(_clickPoints.Count - 1)).X + 10);
                        Canvas.SetTop(_areaLabel, MapControl.Current.GetCanvasCoord(_clickPoints.ElementAt(_clickPoints.Count - 1)).Y + 15);
                    }
                }
                else // Middle clicks
                {
                    _mouseDownOrigin = MapControl.Current.GetWorldCoord((e.GetPosition(MapControl.Current)));
                    _clickPoints.Add(_mouseDownOrigin);
                    this.Render();
                }
            }
        }
    }

    /// <summary>
    /// Distance measuring tool.
    /// </summary>
    public class MeasureTool : ViewerTool
    {
        private bool _isDragging = false;
        private bool _isStarted = false;
        private bool _isEnded = false;

        private Geometry.Vector _mouseDownOrigin = new Geometry.Vector();
        private Geometry.Vector _mouseDownEnd = new Geometry.Vector();
        private Geometry.Vector _tempMouseDownEnd = new Geometry.Vector();

        private readonly Polyline _measureLine = new Polyline
        {
            Stroke = new SolidColorBrush(new Color { A = 255, R = 255, B = 255 }),
            StrokeThickness = 4,
            StrokeLineJoin = PenLineJoin.Bevel
        };

        private readonly Dictionary<TextBlock, Geometry.Vector> _labels = new Dictionary<TextBlock, Geometry.Vector>();
        private readonly List<Geometry.Vector> _clickPoints = new List<Geometry.Vector>();

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                yield return _measureLine;
                foreach (var t in _labels)
                {
                    yield return t.Key;
                }
            }
        }

        public override void Render()
        {
            _measureLine.Points.Clear();
            foreach (var pt in _clickPoints)
            {
                _measureLine.Points.Add(MapControl.Current.GetCanvasCoord(pt));
            }
            if (!_isEnded)
            {
                _measureLine.Points.Add(MapControl.Current.GetCanvasCoord(_mouseDownEnd));
            }

            foreach (var t in _labels)
            {
                Canvas.SetLeft(t.Key, MapControl.Current.GetCanvasCoord(t.Value).X + 10);
                Canvas.SetTop(t.Key, MapControl.Current.GetCanvasCoord(t.Value).Y + 15);
            }
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();

            _labels.ForEach(x => MapControl.Current.Children.Remove(x.Key));
            _measureLine.Points.Clear();
            _clickPoints.Clear();
            _labels.Clear();
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!_isEnded && _isDragging)
            {
                _mouseDownEnd = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
            }
            this.Render();
        }

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            _isEnded = true;
            _isStarted = false;
            _isDragging = false;
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isEnded)
                {
                    _labels.ForEach(x => MapControl.Current.Children.Remove(x.Key));
                    _clickPoints.Clear();
                    _measureLine.Points.Clear();
                    _labels.Clear();

                    _isEnded = false;
                }

                _isStarted = true;
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_isEnded && _isStarted && !_isDragging)
                {
                    _isDragging = true;
                    _mouseDownOrigin = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                    _clickPoints.Add(_mouseDownOrigin);
                }
                else if (_isEnded)
                {
                }
                else
                {
                    _tempMouseDownEnd = _mouseDownEnd;
                    _clickPoints.Add(_mouseDownEnd);

                    double distance = Enumerable.Range(0, _clickPoints.Count - 1).Sum(i => _clickPoints[i].Dist(_clickPoints[i + 1]));
                    var tempLabel = new TextBlock { Text = distance.ToString("0.000") + "m" };
                    Canvas.SetLeft(tempLabel, MapControl.Current.GetCanvasCoord(_mouseDownEnd).X + 10);
                    Canvas.SetTop(tempLabel, MapControl.Current.GetCanvasCoord(_mouseDownEnd).Y + 15);

                    _labels.Add(tempLabel, _mouseDownEnd);
                    MapControl.Current.Children.Add(tempLabel);
                }
            }
        }
    }

    /// <summary>
    /// Set marks tool.
    /// </summary>
    public class SetMarksTool : ViewerTool
    {

    }

    /// <summary>
    /// Cursor tip tool.
    /// </summary>
    public class CursorTipTool : ViewerTool // todo: when this is on, app exit has issue.
    {
        public string Tip { get; set; }

        private Point Position;

        private readonly Popup ToolTipPopup = new Popup
        {
            Placement = PlacementMode.Relative,
            PlacementTarget = MapControl.Current
        };

        private readonly TextBlock ToolTipTextBlock = new TextBlock
        {
            Padding = new Thickness(5),
            Background = Brushes.Pink
        };

        public CursorTipTool(string tip)
        {
            this.Tip = tip;
            this.ToolTipPopup.Child = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = this.ToolTipTextBlock
            };
        }

        public override void Render()
        {
            base.Render();

            this.ToolTipTextBlock.Text = this.Tip;
            this.ToolTipPopup.HorizontalOffset = this.Position.X + 20;
            this.ToolTipPopup.VerticalOffset = this.Position.Y;
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            this.Position = e.GetPosition(MapControl.Current);
            this.Render();
        }

        public override void EnterToolHandler() // newly 20140624
        {
            base.EnterToolHandler();
            this.ToolTipPopup.IsOpen = true;
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            this.ToolTipPopup.IsOpen = false;
        }
    }

    /// <summary>
    /// Pick point tool.
    /// </summary>
    public class PickPointTool : ViewerTool
    {
        public Geometry.Vector? Point { get; private set; }
        public bool Picked { get; private set; }

        public event Action<Geometry.Vector> PointPicked;
        protected void OnPointPicked(Geometry.Vector p)
        {
            this.PointPicked?.Invoke(p);
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pos = e.GetPosition(MapControl.Current);
                var point = MapControl.Current.GetWorldCoord(pos);
                this.Point = point;
                this.OnPointPicked(point);
                this.Picked = true;
            }
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Picked = true;
            }
        }

        public override void EnterToolHandler()
        {
            base.EnterToolHandler();
            MapControl.Current.Cursor = Cursors.Cross;
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            MapControl.Current.Cursor = Cursors.Arrow;
        }
    }

    /// <summary>
    /// Pick extents tool.
    /// </summary>
    public class PickExtentsTool : ViewerTool
    {
        public Extents Extents { get; private set; }
        public bool Picked { get; private set; }

        private bool _isDragging = false;
        private bool _isMoving = false;
        private bool _isStarted = false;
        private Geometry.Vector _mouseDownOrigin;
        private Geometry.Vector _mouseDownEnd;

        private readonly Rectangle Rect = new Rectangle
        {
            Stroke = new SolidColorBrush(new Color { A = 255, R = 255 }),
            StrokeThickness = 3
        };

        public event Action<Extents> Completed;
        protected void OnCompleted(Extents extents)
        {
            this.Completed?.Invoke(Extents);
        }

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                yield return Rect;
            }
        }

        public override void Render()
        {
            Point p1 = MapControl.Current.GetCanvasCoord(_mouseDownOrigin);
            Point p2 = MapControl.Current.GetCanvasCoord(_mouseDownEnd);

            Rect.Width = Math.Abs(p1.X - p2.X);
            Rect.Height = Math.Abs(p1.Y - p2.Y);
            Canvas.SetLeft(Rect, Math.Min(p1.X, p2.X));
            Canvas.SetTop(Rect, Math.Min(p1.Y, p2.Y));
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isMoving = true;
                Rect.Visibility = Visibility.Visible;

                Point pos = e.GetPosition(MapControl.Current);
                _mouseDownEnd = MapControl.Current.GetWorldCoord(pos);
                this.Render();
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_isStarted)
                {
                    _isStarted = true;
                    _isDragging = true;
                    Point pos = e.GetPosition(MapControl.Current);
                    _mouseDownOrigin = MapControl.Current.GetWorldCoord(pos);
                }
                else
                {
                    _isStarted = false;
                }
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isStarted == false)
                {
                    if (_isMoving == true)
                    {
                        Extents = Extents.FromPoints(_mouseDownOrigin, _mouseDownEnd);
                        Picked = true;
                        _isMoving = false;
                    }

                    Rect.Visibility = Visibility.Collapsed;
                    _isDragging = false;
                }
            }
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Extents = Extents.Empty;
                Picked = true;
            }
        }
    }

    /// <summary>
    /// Draw line tool.
    /// </summary>
    public class DrawLineTool : ViewerTool
    {
        private static SolidColorBrush _strokeBrush = new SolidColorBrush(new Color { A = 255, G = 128, R = 255 });
        private static SolidColorBrush _fillBrush = new SolidColorBrush(new Color { A = 64, G = 128, R = 255 });

        private bool _isDragging = false;
        private bool _isStarted = false;
        private Geometry.Vector _pos = new Geometry.Vector();

        private readonly Polygon _poly = new Polygon
        {
            Stroke = _strokeBrush,
            Fill = _fillBrush,
            StrokeThickness = 2,
            StrokeLineJoin = PenLineJoin.Bevel
        };

        private List<Geometry.Vector> _pts = new List<Geometry.Vector>();

        public event Action<List<Geometry.Vector>> Completed;
        protected void OnCompleted(List<Geometry.Vector> pts)
        {
            this.Completed?.Invoke(pts);
        }

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                yield return _poly;
            }
        }

        public override void Render()
        {
            _poly.Points.Clear();
            foreach (var pt in _pts)
            {
                _poly.Points.Add(MapControl.Current.GetCanvasCoord(pt));
            }
            if (_isStarted)
            {
                _poly.Points.Add(MapControl.Current.GetCanvasCoord(_pos));
            }
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();

            _poly.Points.Clear();
            _pts.Clear();
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isStarted && _isDragging)
            {
                _pos = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
            }
            this.Render();
        }

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            _isStarted = false;
            _isDragging = false;
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (!_isStarted)
                {
                    _poly.Points.Clear();
                    _pts.Clear();
                }

                _isStarted = true;
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isStarted && !_isDragging)  // First click
                {
                    _isDragging = true;
                    var pt = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                    _pts.Add(pt);
                }
                else if (!_isStarted)  // Last click (the second of a double click)
                {
                    this.Render();
                    this.OnCompleted(_pts);
                }
                else // Other clicks (including the first of a double click)
                {
                    var pt = MapControl.Current.GetWorldCoord((e.GetPosition(MapControl.Current)));
                    _pts.Add(pt);
                    this.Render();
                }
            }
        }
    }

    /// <summary>
    /// Manipulate tool.
    /// </summary>
    public class ManipulateTool : ViewerTool
    {
        private FrameworkElement _element;
        private FrameworkElement _ghost;
        private Rectangle _anchorResize;
        private Rectangle _anchorMove;
        private const double AnchorSize = 10;

        private bool _readyToResize = false;
        private bool _readyToMove = false;
        private bool _isDragging = false;

        private double _minx;
        private double _miny;
        private double _maxx;
        private double _maxy;

        public ManipulateTool(FrameworkElement element)
        {
            _element = element;
            _ghost = element.Clone();
            _ghost.Opacity = 0;

            _anchorResize = new Rectangle
            {
                Stroke = Brushes.Black,
                Fill = Brushes.White,
                Cursor = Cursors.SizeNESW,
                Width = AnchorSize,
                Height = AnchorSize
            };

            _anchorResize.MouseEnter += (sender, e) =>
            {
                _readyToResize = true;
                MapControl.Current.Cursor = Cursors.SizeNESW;
            };

            _anchorResize.MouseLeave += (sender, e) =>
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    _readyToResize = false;
                    MapControl.Current.Cursor = Cursors.Arrow;
                }
            };

            _anchorMove = new Rectangle
            {
                Stroke = Brushes.Black,
                Fill = Brushes.DodgerBlue,
                Cursor = Cursors.SizeAll,
                Width = AnchorSize,
                Height = AnchorSize
            };

            _anchorMove.MouseEnter += (sender, e) =>
            {
                _readyToMove = true;
                MapControl.Current.Cursor = Cursors.SizeAll;
            };

            _anchorMove.MouseLeave += (sender, e) =>
            {
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    _readyToMove = false;
                    MapControl.Current.Cursor = Cursors.Arrow;
                }
            };

            _minx = Canvas.GetLeft(_element);
            _miny = Canvas.GetTop(_element);
            _maxx = Canvas.GetLeft(_element) + _element.Width;
            _maxy = Canvas.GetTop(_element) + _element.Height;

            UpdateAnchor();
        }

        private void UpdateAnchor()
        {
            var pt1 = MapControl.Current.GetCanvasCoord(new Geometry.Vector(_maxx, _maxy));
            Canvas.SetLeft(_anchorResize, pt1.X);
            Canvas.SetTop(_anchorResize, pt1.Y - AnchorSize);

            var pt2 = MapControl.Current.GetCanvasCoord(new Geometry.Vector(_minx, _miny));
            Canvas.SetLeft(_anchorMove, pt2.X - AnchorSize);
            Canvas.SetTop(_anchorMove, pt2.Y);
        }

        private void UpdateGhost()
        {
            TuneRect(_ghost, _minx, _miny, _maxx, _maxy);
        }

        private void UpdateElement()
        {
            TuneRect(_element, _minx, _miny, _maxx, _maxy);
        }

        private static void TuneRect(FrameworkElement rect, double minx, double miny, double maxx, double maxy)
        {
            rect.Width = Math.Max(0, maxx - minx);
            rect.Height = Math.Max(0, maxy - miny);
            Canvas.SetLeft(rect, minx);
            Canvas.SetTop(rect, miny);
        }

        public event Action<FrameworkElement> Completed;
        protected void OnCompleted(FrameworkElement element)
        {
            UpdateElement();
            this.Completed?.Invoke(element);
        }

        public override IEnumerable<UIElement> CanvasElements
        {
            get
            {
                yield return _anchorResize;
                yield return _anchorMove;
            }
        }

        public override IEnumerable<UIElement> WorldElements
        {
            get
            {
                yield return _ghost;
            }
        }

        public override void Render()
        {
            UpdateGhost();
            UpdateAnchor();
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isDragging)
            {
                if (_readyToResize)
                {
                    var pt = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                    _maxx = pt.X;
                    _maxy = pt.Y;
                }
                else if (_readyToMove)
                {
                    var pt = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                    var minx = _minx;
                    var miny = _miny;
                    _minx = pt.X;
                    _miny = pt.Y;
                    _maxx = _minx + _maxx - minx;
                    _maxy = _miny + _maxy - miny;
                }
                this.Render();
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_readyToResize || _readyToMove)
                {
                    _isDragging = true;
                    _ghost.Opacity = 0.5;
                }
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = false;
                _readyToResize = false;
                _readyToMove = false;
                _ghost.Opacity = 0;
                MapControl.Current.Cursor = Cursors.Arrow;
                OnCompleted(_element);
            }
        }
    }

    public static class Extensions
    {
        public static T Clone<T>(this T element) where T : FrameworkElement
        {
            string xaml = XamlWriter.Save(element);
            var stringReader = new StringReader(xaml);
            var xmlReader = System.Xml.XmlReader.Create(stringReader);
            return (T)XamlReader.Load(xmlReader);
        }
    }
}
