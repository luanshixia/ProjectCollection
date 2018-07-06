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
                    },
                    Properties = new JObject
                    {
                        { "role", string.Empty },
                        { "user", string.Empty }
                    }
                };

                var bubble = new NodeBubble
                {
                    NodeID = node.ID,
                    Text = node.Name,
                    Position = clickPoint
                };

                Canvas.SetZIndex(bubble, 100);
                bubble.ReadyControl();

                MainWindow.Current.MyCanvas.Children.Add(bubble);
                MainWindow.Current.Bubbles.Add(node.ID, bubble);
                DataManager.CurrentDocument.NodesStore.Add(node.ID, node);
            }
        }
    }

    public class AddLinkTool : ViewerTool
    {
        private NodeBubble StartNode;
        private NodeBubble EndNode;
        private int ClickCount = 0;

        public override void MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.OriginalSource is FrameworkElement element)
                {
                    if (element.Parent is NodeBubble bubble)
                    {
                        this.ClickCount++;

                        if (this.ClickCount == 1)
                        {
                            this.StartNode = bubble;
                        }
                        else
                        {
                            if (bubble != this.StartNode)
                            {
                                this.EndNode = bubble;
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
                if (this.ClickCount >= 2)
                {
                    if (this.StartNode != null && this.EndNode != null)
                    {
                        if (!MainWindow.Current.Arrows.ContainsKeys(this.StartNode.NodeID, this.EndNode.NodeID))
                        {
                            var link = new FlowLink
                            {
                                From = this.StartNode.NodeID,
                                To = this.EndNode.NodeID,
                                Label = string.Empty
                            };

                            var arrow = new BezierLink
                            {
                                StartPoint = this.StartNode.Position,
                                EndPoint = this.EndNode.Position,
                                StartOffset = DataManager.NodeSize / 2,
                                EndOffset = DataManager.NodeSize / 2,
                                Tag = (link.From, link.To)
                            };

                            Canvas.SetZIndex(arrow, 100);
                            arrow.ReadyControl();

                            MainWindow.Current.MyCanvas.Children.Add(arrow);
                            MainWindow.Current.Arrows.Add(link.From, link.To, arrow);
                            DataManager.CurrentDocument.LinksStore.Add(link.From, link.To, link);
                        }

                        this.ClickCount = 0;
                        this.StartNode = null;
                        this.EndNode = null;
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

                    var node = DataManager.CurrentDocument.NodesStore[this.BubbleToMove.NodeID];
                    node.Metadata.Left = position.X;
                    node.Metadata.Top = position.Y;
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
                var node = DataManager.CurrentDocument.NodesStore[bubble.NodeID];

                var inputs = new Dictionary<string, string>
                {
                    { "Name", node.Name },
                    { "Role", node.Properties["role"].ToObject<string>() },
                    { "User", node.Properties["user"].ToObject<string>() }
                };

                Gui.MultiInputs("Node info", inputs);

                node.Name = inputs["Name"];
                node.Properties["role"] = inputs["Role"];
                node.Properties["user"] = inputs["User"];

                bubble.SetText(inputs["Name"]);
            }
        }
    }

    public class EditLinkTool : ViewerTool
    {
        // TODO: don't want to look into this mess. Will revamp it using direct link pick rather than nodes pick.
    }
}
