using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TongJi.Gis.Display
{
    /// <summary>
    /// 视图工具管理
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
        public static System.Collections.ObjectModel.ReadOnlyCollection<ViewerTool> OverlayTools
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
            tool.ExitToolHandler();
            _overlayTools.Remove(tool);
        }

        public static void ClearTools()
        {
            _overlayTools.ForEach(x => x.ExitToolHandler());
            _overlayTools.Clear();
        }
    }

    /// <summary>
    /// 视图工具抽象类
    /// </summary>
    public abstract class ViewerTool
    {
        public virtual IEnumerable<UIElement> TempElements { get { yield break; } }

        public virtual void Render()
        {
        }

        public virtual void MouseMoveHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
        }

        public virtual void KeyDownHandler(object sender, KeyEventArgs e)
        {
        }

        public virtual void EnterToolHandler()
        {
            TempElements.ForEach(x => MapControl.Current.Children.Add(x));
        }

        public virtual void ExitToolHandler()
        {
            TempElements.ForEach(x => MapControl.Current.Children.Remove(x));
        }
    }

    /// <summary>
    /// 组合的视图工具
    /// </summary>
    public class CombinedViewerTool : ViewerTool
    {
        private ViewerTool[] _tools;

        public CombinedViewerTool(params ViewerTool[] tools)
        {
            _tools = tools;
        }

        public override IEnumerable<UIElement> TempElements
        {
            get
            {
                return _tools.SelectMany(x => x.TempElements);
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

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseLDownHandler(sender, e));
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            _tools.ForEach(x => x.MouseLUpHandler(sender, e));
        }

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            _tools.ForEach(x => x.MouseLDoubleClickHandler(sender, e));
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
    /// 平移画布工具
    /// </summary>
    public class PanCanvasTool : ViewerTool
    {
        private bool _isDragging = false;
        private Point _mouseDownTemp;

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point pos = e.GetPosition(MapControl.Current);
                Point vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                MapControl.Current.PanCanvas(vector);
                _mouseDownTemp = pos;
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _mouseDownTemp = e.GetPosition(MapControl.Current);
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        public void StartDrag(Point mousePos)
        {
            _isDragging = true;
            _mouseDownTemp = mousePos;
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
    /// 点选工具
    /// </summary>
    public class SelectionTool : ViewerTool
    {
        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(MapControl.Current);
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) // mod 20130222
            {
                if (MapControl.Current.HitTest(pos))
                {
                    if (SelectionSet.Contents.Contains(MapControl.Current.HitTestFeatureResult))
                    {
                        SelectionSet.SubtractSelection(new Feature[] { MapControl.Current.HitTestFeatureResult });
                    }
                    else
                    {
                        SelectionSet.AddSelection(new Feature[] { MapControl.Current.HitTestFeatureResult });
                    }
                }
                else
                {                    
                }
            }
            else
            {
                if (MapControl.Current.HitTest(pos))
                {
                    SelectionSet.Select(new Feature[] { MapControl.Current.HitTestFeatureResult });
                }
                else
                {
                    SelectionSet.ClearSelection();
                }
            }
        }
    }

    /// <summary>
    /// 矩形选择工具
    /// </summary>
    public class RectSelectionTool : ViewerTool
    {
        private bool _isDragging = false;
        private Geometry.Point2D _mouseDownOrigin;
        private Geometry.Point2D _mouseDownEnd;
        private Rectangle _selRect = new Rectangle();
        private bool IsWindow { get { return _mouseDownEnd.x >= _mouseDownOrigin.x; } }

        private static SolidColorBrush _black = new SolidColorBrush(Colors.Black);
        private static SolidColorBrush _blue = new SolidColorBrush(new Color { A = 128, G = 128, B = 255 });
        private static SolidColorBrush _green = new SolidColorBrush(new Color { A = 128, G = 255 });
        private static DoubleCollection _continuous = new DoubleCollection { 1, 0 };
        private static DoubleCollection _dashed = new DoubleCollection { 3, 3 };

        public override IEnumerable<UIElement> TempElements
        {
            get
            {
                yield return _selRect;
            }
        }

        public override void Render()
        {
            Point p1 = MapControl.Current.GetCanvasCoord(_mouseDownOrigin);
            Point p2 = MapControl.Current.GetCanvasCoord(_mouseDownEnd);

            _selRect.Width = Math.Abs(p1.X - p2.X);
            _selRect.Height = Math.Abs(p1.Y - p2.Y);
            Canvas.SetLeft(_selRect, Math.Min(p1.X, p2.X));
            Canvas.SetTop(_selRect, Math.Min(p1.Y, p2.Y));

            if (IsWindow)
            {
                _selRect.Stroke = _black;
                _selRect.StrokeDashArray = _continuous;
                _selRect.StrokeThickness = 1;
                _selRect.Fill = _blue;
            }
            else
            {
                _selRect.Stroke = _black;
                _selRect.StrokeDashArray = _dashed;
                _selRect.StrokeThickness = 1;
                _selRect.Fill = _green;
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _selRect.Visibility = Visibility.Visible;
                _mouseDownEnd = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                this.Render();
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging == false)
            {
                _isDragging = true;
                _mouseDownOrigin = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                return;
            }

            _mouseDownEnd = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
            var extent = new Geometry.Extent2D(_mouseDownOrigin, _mouseDownEnd);

            bool success = IsWindow ? MapControl.Current.AllHitTest(extent) : MapControl.Current.PartHitTest(extent);
            if (success)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) // mod 20120810
                {
                    SelectionSet.SubtractSelection(MapControl.Current.HitTestMultiResult.Select(x => x.Key).ToArray());
                }
                else
                {
                    SelectionSet.AddSelection(MapControl.Current.HitTestMultiResult.Select(x => x.Key).ToArray()); // mod 20120725
                }
            }
            else
            {
                //SelectionSet.ClearSelection();
            }

            _isDragging = false;
            _selRect.Visibility = Visibility.Collapsed;
        }
    }

    /// <summary>
    /// 矩形缩放工具
    /// </summary>
    public class RectScaleTool : ViewerTool
    {
        private bool _isDragging = false;
        private bool _isMoving = false;
        private bool _isStarted = false;
        private Point _mouseDownOrigin;
        private Point _mouseDownEnd;
        private Rectangle currentRect = new Rectangle();

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isMoving = true;

                currentRect.Visibility = Visibility.Visible;

                Point pos = e.GetPosition(MapControl.Current);
                _mouseDownEnd = pos;

                double width = Math.Abs(pos.X - _mouseDownOrigin.X);
                double height = Math.Abs(pos.Y - _mouseDownOrigin.Y);

                currentRect.Width = width;
                currentRect.Height = height;

                Canvas.SetLeft(currentRect, Math.Min(_mouseDownOrigin.X, _mouseDownEnd.X));
                Canvas.SetTop(currentRect, Math.Min(_mouseDownOrigin.Y, _mouseDownEnd.Y));

                currentRect.Stroke = new SolidColorBrush(new Color { A = 255, R = 255 });
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (!_isStarted)
            {
                _isStarted = true;
                _isDragging = true;
                _mouseDownOrigin = e.GetPosition(MapControl.Current);

                MapControl.Current.Children.Add(currentRect);
            }
            else
            {
                _isStarted = false;
            }
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (_isStarted == false)
            {
                if (_isMoving == true)
                {
                    TongJi.Geometry.Point2D pt1 = MapControl.Current.GetWorldCoord(_mouseDownOrigin);
                    TongJi.Geometry.Point2D pt2 = MapControl.Current.GetWorldCoord(_mouseDownEnd);

                    var extent = new Geometry.Extent2D(pt1, pt2);
                    MapControl.Current.Zoom(extent);

                    _isMoving = false;
                }

                MapControl.Current.Children.Remove(currentRect);
                currentRect.Visibility = Visibility.Collapsed;
                _isDragging = false;
            }
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            MapControl.Current.Children.Remove(currentRect);
        }
    }

    /// <summary>
    /// 面积测量工具
    /// </summary>
    public class AreaMeasureTool : ViewerTool
    {
        private bool _isDragging = false;
        private bool _isStarted = false;
        private bool _isEnded = false;

        private Geometry.Point2D _mouseDownBegin;
        private Geometry.Point2D _mouseDownOrigin;
        private Geometry.Point2D _mouseDownEnd;

        private static SolidColorBrush _strokeBrush = new SolidColorBrush(new Color { A = 255, B = 255, R = 128 });
        private static SolidColorBrush _fillBrush = new SolidColorBrush(new Color { A = 64, B = 255, R = 128 });

        private TextBlock _areaLabel = new TextBlock();
        private Polygon _measurePolygon = new Polygon { Stroke = _strokeBrush, Fill = _fillBrush, StrokeThickness = 2, StrokeLineJoin = PenLineJoin.Bevel };

        private List<Geometry.Point2D> _clickPoints = new List<Geometry.Point2D>();

        public override IEnumerable<UIElement> TempElements
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

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            _isEnded = true;
            _isStarted = false;
            _isDragging = false;
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
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

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (_isStarted && !_isEnded && !_isDragging)  // 第一次点击
            {
                _isDragging = true;
                _mouseDownOrigin = MapControl.Current.GetWorldCoord(e.GetPosition(MapControl.Current));
                _mouseDownBegin = _mouseDownOrigin;
                _clickPoints.Add(_mouseDownBegin);
            }
            else if (_isEnded)  // 结束点击
            {
                //_clickPoints.Remove(_mouseDownOrigin);
                this.Render();

                if (_clickPoints.Count > 1)
                {
                    TongJi.Geometry.Polygon areaPoly = new Geometry.Polygon(_clickPoints);
                    _areaLabel.Text = "面积：" + areaPoly.Area.ToString("0.000") + "m²";
                    Canvas.SetLeft(_areaLabel, MapControl.Current.GetCanvasCoord(_clickPoints.ElementAt(_clickPoints.Count - 1)).X + 10);
                    Canvas.SetTop(_areaLabel, MapControl.Current.GetCanvasCoord(_clickPoints.ElementAt(_clickPoints.Count - 1)).Y + 15);
                }
            }
            else // 中间点击
            {
                _mouseDownOrigin = MapControl.Current.GetWorldCoord((e.GetPosition(MapControl.Current)));
                _clickPoints.Add(_mouseDownOrigin);
                this.Render();
            }
        }
    }

    /// <summary>
    /// 距离测量工具
    /// </summary>
    public class MeasureTool : ViewerTool
    {
        private bool _isDragging = false;
        private bool _isStarted = false;
        private bool _isEnded = false;

        private TongJi.Geometry.Point2D _mouseDownOrigin = new TongJi.Geometry.Point2D();
        private TongJi.Geometry.Point2D _mouseDownEnd = new TongJi.Geometry.Point2D();
        private TongJi.Geometry.Point2D _tempMouseDownEnd = new TongJi.Geometry.Point2D();

        private static SolidColorBrush _strokeBrush = new SolidColorBrush(new Color { A = 255, R = 255, B = 255 });
        private Polyline _measureLine = new Polyline { Stroke = _strokeBrush, StrokeThickness = 4, StrokeLineJoin = PenLineJoin.Bevel };
        private TextBlock _tempLabel = new TextBlock();

        private Dictionary<TextBlock, TongJi.Geometry.Point2D> _labels = new Dictionary<TextBlock, Geometry.Point2D>();
        private List<TongJi.Geometry.Point2D> _clickPoints = new List<TongJi.Geometry.Point2D>();

        public override IEnumerable<UIElement> TempElements
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

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            _isEnded = true;
            _isStarted = false;
            _isDragging = false;
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
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

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
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

                double distance = Enumerable.Range(0, _clickPoints.Count - 1).Sum(i => _clickPoints[i].DistTo(_clickPoints[i + 1]));
                _tempLabel = new TextBlock { Text = distance.ToString("0.000") + "m" };
                Canvas.SetLeft(_tempLabel, MapControl.Current.GetCanvasCoord(_mouseDownEnd).X + 10);
                Canvas.SetTop(_tempLabel, MapControl.Current.GetCanvasCoord(_mouseDownEnd).Y + 15);

                _labels.Add(_tempLabel, _mouseDownEnd);
                MapControl.Current.Children.Add(_tempLabel);
            }
        }
    }

    /// <summary>
    /// 设置标记工具
    /// </summary>
    public class SetMarksTool : ViewerTool
    {

    }

    /// <summary>
    /// 光标跟随提示工具
    /// </summary>
    public class CursorTip : ViewerTool
    {
        public string Tip { get; set; }
        private System.Windows.Controls.Primitives.Popup _popup = new System.Windows.Controls.Primitives.Popup { Placement = System.Windows.Controls.Primitives.PlacementMode.Relative, PlacementTarget = MapControl.Current };
        private Border _border = new Border { BorderBrush = Brushes.Gray, BorderThickness = new Thickness(1) };
        private TextBlock _txtTip = new TextBlock { Padding = new Thickness(5), Background = Brushes.Pink };
        private Point _pos;

        public CursorTip(string tip)
        {
            Tip = tip;
            _border.Child = _txtTip;
            _popup.Child = _border;
            _popup.IsOpen = true;
        }

        public override void Render()
        {
            base.Render();

            _txtTip.Text = Tip;
            _popup.HorizontalOffset = _pos.X + 20;
            _popup.VerticalOffset = _pos.Y;
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            _pos = e.GetPosition(MapControl.Current);
            this.Render();
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            _popup.IsOpen = false;
        }
    }

    /// <summary>
    /// 选点工具
    /// </summary>
    public class PickPoint : ViewerTool
    {
        public Geometry.Point2D Point { get; private set; }
        public bool Picked { get; private set; }

        public event Action<Geometry.Point2D> PointPicked;
        protected void OnPointPicked(Geometry.Point2D p)
        {
            if (PointPicked != null)
            {
                PointPicked(p);
            }
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(MapControl.Current);
            Point = MapControl.Current.GetWorldCoord(pos);
            OnPointPicked(Point);
            Picked = true;
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Point = Geometry.Point2D.Null;
                Picked = true;
            }
        }

        public override void EnterToolHandler()
        {
            base.EnterToolHandler();
            MapControl.Current.Cursor = System.Windows.Input.Cursors.Cross;
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();
            MapControl.Current.Cursor = System.Windows.Input.Cursors.Arrow;
        }
    }

    /// <summary>
    /// 选范围工具
    /// </summary>
    public class PickExtents : ViewerTool
    {
        public Geometry.Extent2D Extents { get; private set; }
        public bool Picked { get; private set; }

        private bool _isDragging = false;
        private bool _isMoving = false;
        private bool _isStarted = false;
        private Geometry.Point2D _mouseDownOrigin;
        private Geometry.Point2D _mouseDownEnd;
        private Rectangle _rect = new Rectangle { Stroke = new SolidColorBrush(new Color { A = 255, R = 255 }), StrokeThickness = 3 };

        public override IEnumerable<UIElement> TempElements
        {
            get
            {
                yield return _rect;
            }
        }

        public override void Render()
        {
            Point p1 = MapControl.Current.GetCanvasCoord(_mouseDownOrigin);
            Point p2 = MapControl.Current.GetCanvasCoord(_mouseDownEnd);

            _rect.Width = Math.Abs(p1.X - p2.X);
            _rect.Height = Math.Abs(p1.Y - p2.Y);
            Canvas.SetLeft(_rect, Math.Min(p1.X, p2.X));
            Canvas.SetTop(_rect, Math.Min(p1.Y, p2.Y));
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                _isMoving = true;
                _rect.Visibility = Visibility.Visible;

                Point pos = e.GetPosition(MapControl.Current);
                _mouseDownEnd = MapControl.Current.GetWorldCoord(pos);
                this.Render();
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
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

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (_isStarted == false)
            {
                if (_isMoving == true)
                {
                    Extents = new Geometry.Extent2D(_mouseDownOrigin, _mouseDownEnd);
                    Picked = true;
                    _isMoving = false;
                }

                _rect.Visibility = Visibility.Collapsed;
                _isDragging = false;
            }
        }

        public override void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Extents = Geometry.Extent2D.Null;
                Picked = true;
            }
        }
    }
}
