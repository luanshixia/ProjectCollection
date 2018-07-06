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
            tools.ForEach(tool =>
            {
                if (tool is SelectNodeTool || tool is EditNodeTool)
                {
                    ViewerToolManager.RemoveTool(tool);
                }
            });
        }
    }

    public class AddNodeTool : ViewerTool
    {
        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var clickPoint = e.GetPosition(MainWindow.Current.MyCanvas);

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

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_isFinished)
                {
                    count = 0;
                    _isFinished = false;
                }

                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.Parent is NodeBubble bubble)
                    {
                        count++;

                        if (count == 1)
                        {
                            _startNode = bubble;
                        }
                        else
                        {
                            if (bubble != _startNode)
                            {
                                _endNode = bubble;
                            }
                        }
                    }
                }

                // TODO: use status bar to show this info.
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
                        }
                    }
                }
            }
        }
    }

    public class MoveNodeTool : ViewerTool
    {
        private bool IsDragging = false;
        private NodeBubble BubbleToMove;

        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.IsDragging = false;
            }
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (this.IsDragging && this.BubbleToMove != null)
            {
                var position = e.GetPosition(MainWindow.Current.MyCanvas);
                this.BubbleToMove.Position = position;
                this.BubbleToMove.ReadyControl();
                this.UpdateAllLinks(position);
            }
        }

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.IsDragging = true;
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.Parent is NodeBubble bubble)
                    {
                        this.BubbleToMove = bubble;

                        return;
                    }
                }

                this.BubbleToMove = null;
            }
        }

        public override void MouseUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.BubbleToMove != null)
                {
                    var position = e.GetPosition(MainWindow.Current.MyCanvas);
                    this.BubbleToMove.Position = position;
                    this.BubbleToMove.ReadyControl();
                    this.UpdateAllLinks(position);
                }

                this.IsDragging = false;
                this.BubbleToMove = null;
            }
        }

        private void UpdateAllLinks(Point position)
        {
            MainWindow.Current.Arrows.RealValues.ForEach(arrow =>
            {
                if (arrow.FromNodeID == this.BubbleToMove.NodeID)
                {
                    arrow.StartPoint = position;
                    arrow.ReadyControl();
                }
                else if (arrow.ToNodeID == this.BubbleToMove.NodeID)
                {
                    arrow.EndPoint = position;
                    arrow.ReadyControl();
                }
            });
        }
    }

    public class SelectNodeTool : ViewerTool
    {
        // TODO: move the colors to Node class when CanvasIndentifiableElement base class is done.
        private static readonly Color DefaultColor = Colors.Gray;
        private static readonly Color HighlightColor = Colors.Orange;

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (MainWindow.Current.SelectedBubble != null)
                {
                    MainWindow.Current.SelectedBubble.SetColor(DefaultColor);
                }

                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.Parent is NodeBubble bubble)
                    {
                        MainWindow.Current.SelectedBubble = bubble;
                        bubble.SetColor(HighlightColor);

                        return;
                    }
                }

                MainWindow.Current.SelectedBubble = null;
            }
        }
    }

    public class EditNodeTool : ViewerTool
    {
        public override void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                var bubble = MainWindow.Current.SelectedBubble;

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
