using Dreambuild.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BubbleFlow
{
    public class WheelScalingTool : ViewerTool
    {
        private static double[] _zoomLevels = new double[] { 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625 };

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            base.MouseWheelHandler(sender, e);

            var basePoint = e.GetPosition(MainWindow.Current);
            int index = WheelScalingTool.FindScaleIndex(MainWindow.Current.Scale);
            index += e.Delta / 120;
            if (index > _zoomLevels.Length - 1)
            {
                index = _zoomLevels.Length - 1;
            }
            else if (index < 0)
            {
                index = 0;
            }

            double scale = _zoomLevels[index];
            MainWindow.Current.ScaleCanvas(scale, basePoint);
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
    }

    public class PanCanvasTool : ViewerTool
    {
        private bool _isDragging = false;
        private Point _mouseDownTemp;

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = false;
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var pos = e.GetPosition(MainWindow.Current);
                var vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                MainWindow.Current.PanCanvas(vector);
                _mouseDownTemp = pos;
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = true;
                _mouseDownTemp = e.GetPosition(MainWindow.Current);
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = false;
            }
        }

        public void StartDrag(Point mousePos)
        {
            _isDragging = true;
            _mouseDownTemp = mousePos;
        }

        public override void EnterToolHandler()
        {
            base.EnterToolHandler();

            ViewerToolManager.AddTool(new SelectNodeTool());
            ViewerToolManager.AddTool(new EditNodeTool());
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();

            var tools = ViewerToolManager.Tools.ToList();
            tools.ForEach(t =>
            {
                if (t is SelectNodeTool || t is EditNodeTool)
                {
                    ViewerToolManager.RemoveTool(t);
                }
            });
        }
    }

    public class AddNodeTool : ViewerTool
    {
        //public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Left)
        //    {
        //        var previousBubble = MainWindow.Current.Bubbles.Last().Value;
        //        MainWindow.Current.MyCanvas.Children.Remove(previousBubble);
        //        MainWindow.Current.Bubbles.Remove(previousBubble);
        //    }
        //}

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var clickPoint = MoveNodeTool.GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));

                var node = new FlowNode
                {
                    ID = Guid.NewGuid(),
                    Name = "New node",
                    Metadata = new FlowElementMetadata
                    {
                        Left = clickPoint.X,
                        Top = clickPoint.Y
                    }
                };

                var bubble = new NodeBubble
                {
                    NodeID = node.ID,
                    Text = node.Name,
                    Position = clickPoint
                };

                Canvas.SetZIndex(bubble, 100);
                MainWindow.Current.MyCanvas.Children.Add(bubble);
                MainWindow.Current.Bubbles.Add(node.ID, bubble);
                bubble.ReadyControl();
            }
        }
    }

    public class AddConnectionTool : ViewerTool
    {
        private NodeBubble _startNode;
        private NodeBubble _endNode;
        private int count = 0;
        private bool _isFinished = false;
        private bool _isAnyPicked = false;
        private bool _isTbAdd = false;
        private bool _isStarted = false;
        private bool _isSelfSelected = false;
        private bool _isConnectionExist = false;
        private Border border = new Border { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.DarkGray) };
        private TextBlock tb = new TextBlock();
        private Point _mouseDownTemp;

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isStarted = true;
                if (_isTbAdd)
                {
                    MainWindow.Current.MyCanvas.Children.Remove(border);
                    _isTbAdd = false;
                }

                if (_isFinished)
                {
                    count = 0;
                    _isFinished = false;
                }
                _isAnyPicked = false;

                _mouseDownTemp = e.GetPosition(MainWindow.Current.MyCanvas);
                foreach (var child in MainWindow.Current.MyCanvas.Children)
                {
                    if (child is NodeBubble node)
                    {
                        if (node.IsPointInNode(_mouseDownTemp))
                        {
                            _isAnyPicked = true;
                            count++;

                            if (count == 1)
                            {
                                _startNode = node;
                                break;
                            }
                            else
                            {
                                if (node != _startNode)
                                {
                                    _endNode = node;
                                    _isSelfSelected = false;
                                    break;
                                }
                                else
                                {
                                    _isSelfSelected = true;
                                }
                            }
                        }
                    }
                }

                if (!_isAnyPicked)
                {
                    border.RenderTransform = new TranslateTransform { X = 0, Y = 0 };
                    border.Child = tb;
                    Canvas.SetLeft(border, _mouseDownTemp.X + 10);
                    Canvas.SetTop(border, _mouseDownTemp.Y + 10);
                    if (count == 0)
                    {
                        tb.Text = "No start node selected.";
                    }
                    else
                    {
                        tb.Text = "No end node selected.";
                    }
                    MainWindow.Current.MyCanvas.Children.Add(border);
                    _isTbAdd = true;
                }
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (count == 2)
                {
                    _isFinished = true;
                    if (_startNode != null && _endNode != null)
                    {
                        double NodeSize = _startNode.Size;
                        var arrow = new BezierLink
                        {
                            StartPoint = _startNode.Position,
                            EndPoint = _endNode.Position,
                            StartOffset = NodeSize / 2,
                            EndOffset = NodeSize / 2
                        };

                        arrow.ReadyControl();
                        Canvas.SetZIndex(arrow, 100);
                        MainWindow.Current.MyCanvas.Children.Add(arrow);

                        var link = new FlowLink
                        {
                            From = _startNode.NodeID,
                            To = _endNode.NodeID,
                            Label = string.Empty
                        };

                        if (!MainWindow.Current.Arrows.ContainsKeys(link.From, link.To))
                        {
                            MainWindow.Current.Arrows.Add(link.From, link.To, arrow);
                            _isConnectionExist = false;
                        }
                        else
                        {
                            _isConnectionExist = true;
                            border.RenderTransform = new TranslateTransform { X = 0, Y = 0 };
                            border.Child = tb;
                            Canvas.SetLeft(border, _mouseDownTemp.X + 10);
                            Canvas.SetTop(border, _mouseDownTemp.Y + 10);
                            tb.Text = "Link already exists!";
                            if (!_isTbAdd)
                            {
                                MainWindow.Current.MyCanvas.Children.Add(border);
                            }
                            _isTbAdd = true;
                            MainWindow.Current.MyCanvas.Children.Remove(arrow);
                        }

                        if (_isSelfSelected)
                        {
                            border.RenderTransform = new TranslateTransform { X = 0, Y = 0 };
                            border.Child = tb;
                            Canvas.SetLeft(border, _mouseDownTemp.X + 10);
                            Canvas.SetTop(border, _mouseDownTemp.Y + 10);
                            tb.Text = "Identical start and end nodes.";
                            if (!_isTbAdd)
                            {
                                MainWindow.Current.MyCanvas.Children.Add(border);
                            }
                            _isTbAdd = true;
                            MainWindow.Current.MyCanvas.Children.Remove(arrow);
                        }
                    }
                }
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!_isAnyPicked)
            {
                var pos = e.GetPosition(MainWindow.Current.MyCanvas);
                var vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                var translate = new TranslateTransform { X = vector.X, Y = vector.Y };
                border.RenderTransform = translate;
            }
            if (_isConnectionExist || _isSelfSelected)
            {
                _isSelfSelected = false;
                _isConnectionExist = false;
                MainWindow.Current.MyCanvas.Children.Remove(border);
                _isTbAdd = false;
                _isConnectionExist = false;
            }
        }

        public override void ExitToolHandler()
        {
            base.ExitToolHandler();

            if (_isTbAdd && _isStarted)
            {
                MainWindow.Current.MyCanvas.Children.Remove(border);
                _isTbAdd = false;
            }
        }
    }

    public class MoveNodeTool : ViewerTool
    {
        private bool _isDragging = false;
        private Point _mouseDownTemp;
        private Point _originPos;
        private NodeBubble _movingNode = new NodeBubble();
        private bool _isPickedAny = false;
        TranslateTransform translate;

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = false;
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging && _isPickedAny)
            {
                var catchPos = MoveNodeTool.GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));
                var vector = new Point(catchPos.X - _mouseDownTemp.X, catchPos.Y - _mouseDownTemp.Y);
                translate = new TranslateTransform { X = vector.X, Y = vector.Y };
                _movingNode.RenderTransform = translate;

                SetConnectionPosition(new Point(_movingNode.Position.X + vector.X, _movingNode.Position.Y + vector.Y));
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isDragging = true;
                _mouseDownTemp = MoveNodeTool.GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));
                foreach (var child in MainWindow.Current.MyCanvas.Children)
                {
                    if (child is NodeBubble node)
                    {
                        if (node.IsPointInNode(e.GetPosition(MainWindow.Current.MyCanvas)))
                        {
                            _movingNode = node;
                            _isPickedAny = true;
                            _originPos = node.Position;
                            break;
                        }
                    }
                }
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isPickedAny)
                {
                    var catchPos = MoveNodeTool.GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));

                    translate = new TranslateTransform { X = 0, Y = 0 };
                    _movingNode.RenderTransform = translate;

                    _movingNode.Position = MoveNodeTool.GetCatchPoint(new Point(_originPos.X + catchPos.X - _mouseDownTemp.X, _originPos.Y + catchPos.Y - _mouseDownTemp.Y));
                    _movingNode.ReadyControl();

                    SetConnectionPosition(_movingNode.Position);
                }
                _isDragging = false;
                _isPickedAny = false;
            }
        }

        public void SetConnectionPosition(Point catchPos)
        {
            var nodeID = _movingNode.NodeID;

            MainWindow.Current.Arrows.RealValues.ForEach(arrow =>
            {
                if (arrow.FromNodeID == nodeID)
                {
                    arrow.StartPoint = catchPos;
                    arrow.ReadyControl();
                }
                else if (arrow.ToNodeID == nodeID)
                {
                    arrow.EndPoint = catchPos;
                    arrow.ReadyControl();
                }
            });
        }

        public static Point GetCatchPoint(Point pos)
        {
            string str_x = pos.X.ToString().Split('.')[0];
            double ix = 0;
            if (pos.X >= 0)
            {
                ix = str_x.Length > 1 ? Convert.ToDouble(str_x.Substring(0, str_x.Length - 1)) : 0;
            }
            else
            {
                ix = str_x.Length > 2 ? Convert.ToDouble(str_x.Substring(0, str_x.Length - 1)) : 0;
            }
            double ix1 = Convert.ToDouble(str_x.Substring(str_x.Length - 1, 1));
            if (ix1 >= 5 && ix >= 0)
            {
                ix += 1;
            }
            else if (ix1 >= 5 && ix < 0)
            {
                ix -= 1;
            }
            string str_y = pos.Y.ToString().Split('.')[0];
            double iy = 0;
            if (pos.Y >= 0)
            {
                iy = str_y.Length > 1 ? Convert.ToDouble(str_y.Substring(0, str_y.Length - 1)) : 0;
            }
            else
            {
                iy = str_y.Length > 2 ? Convert.ToDouble(str_y.Substring(0, str_y.Length - 1)) : 0;
            }
            double iy1 = Convert.ToDouble(str_y.Substring(str_y.Length - 1, 1));
            if (iy1 >= 5 && iy >= 0)
            {
                iy += 1;
            }
            else if (iy1 >= 5 && iy < 0)
            {
                iy -= 1;
            }

            return new Point(ix * 10, iy * 10);
        }
    }

    public class SelectNodeTool : ViewerTool
    {
        private Color _defaultColor = Colors.Gray;
        private Color _highlightColor = Colors.Orange;

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var pos = e.GetPosition(MainWindow.Current.MyCanvas);
                foreach (var child in MainWindow.Current.MyCanvas.Children)
                {
                    if (child is NodeBubble node)
                    {
                        if (node.IsPointInNode(pos))
                        {
                            if (MainWindow.Current.CurrentNode != null)
                            {
                                MainWindow.Current.CurrentNode.SetColor(_defaultColor);
                            }

                            MainWindow.Current.CurrentNode = node;
                            node.SetColor(_highlightColor);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class EditNodeTool : ViewerTool
    {
        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var bubble = MainWindow.Current.CurrentNode;

                var inputs = new[] { "Name", "Role", "User" }.ToDictionary(field => field, field => string.Empty);
                inputs["Name"] = bubble.Text;
                Gui.MultiInputs("Node info", inputs);

                var node = DataManager.CurrentDocument.NodesStore[bubble.NodeID];
                node.Name = inputs["Name"];
                node.Properties = new JObject
                {
                    { "role", inputs["Role"] },
                    { "user", inputs["User"] }
                };

                bubble.SetText(inputs["Name"]);
            }
        }
    }

    public class EditConnectionTool : ViewerTool
    {
        // TODO: don't want to look into this mess. Will revamp it using direct link pick rather than nodes pick.
    }
}
