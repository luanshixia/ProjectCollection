using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

namespace BubbleFlow
{
    public class WheelScalingTool : ViewerTool
    {
        private static double[] _zoomLevels = new double[] { 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625 };

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            base.MouseWheelHandler(sender, e);

            Point basePoint = e.GetPosition(MainWindow.Current);
            int index = FindScaleIndex(MainWindow.Current.Scale);
            index += e.Delta / 120;
            if (index > _zoomLevels.Length - 1) index = _zoomLevels.Length - 1;
            else if (index < 0) index = 0;
            double scale = _zoomLevels[index];
            MainWindow.Current.ScaleCanvas(scale, basePoint);
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
    }

    public class PanCanvasTool : ViewerTool
    {
        private bool _isDragging = false;
        private Point _mouseDownTemp;

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point pos = e.GetPosition(MainWindow.Current);
                Point vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                MainWindow.Current.PanCanvas(vector);
                _mouseDownTemp = pos;
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _mouseDownTemp = e.GetPosition(MainWindow.Current);
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
        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            deletePreNode();
        }

        private void deletePreNode()
        {
            Node preNode = MainWindow.Current.nodes.Last().Key;
            MainWindow.Current.MyCanvas.Children.Remove(preNode);
            MainWindow.Current.nodes.Remove(preNode);
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            Point catchPos = GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));
            
            Node node = new Node();
            node.Position = new Point(catchPos.X + 1, catchPos.Y + 1);
            Canvas.SetZIndex(node, 100);
            MainWindow.Current.MyCanvas.Children.Add(node);

            string name = node.Text;
            int s = 1;
            while (MainWindow.Current.nodes.Values.ToList().Any(n => n.name == name))
            {
                name = string.Format("{0}{1}", node.Text, s.ToString());
                s++;
            }
            node.SetText(name);
            node.ReadyControl();

            FlowNodeJsonObject nodeJson = new FlowNodeJsonObject();
            nodeJson.name = name;
            nodeJson.role = string.Empty;
            nodeJson.user = string.Empty;

            MainWindow.Current.nodes.Add(node, nodeJson);
        }

        public Point GetCatchPoint(Point pos)
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
            Point catchPos = new Point(ix * 10, iy * 10);

            return catchPos;
        }
    }

    public class AddConnectionTool : ViewerTool
    {
        private Node _startNode;
        private Node _endNode;
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

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
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
                if (child is Node)
                {
                    Node node = child as Node;
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
                    tb.Text = "当前未选中任何StartNode\n请重新选择StartNode";
                }
                else
                {
                    tb.Text = "当前未选中任何EndNode\n请重新选择EndNode";
                }
                MainWindow.Current.MyCanvas.Children.Add(border);
                _isTbAdd = true;
            }            
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (count == 2)
            {
                _isFinished = true;
                if (_startNode != null && _endNode != null)
                {
                    double NodeSize = _startNode.Size;
                    BezierLink arrow = new BezierLink();
                    arrow.StartPoint = _startNode.Position;
                    arrow.EndPoint = _endNode.Position;
                    arrow.StartOffset = NodeSize / 2;
                    arrow.EndOffset = NodeSize / 2;

                    arrow.ReadyControl();
                    Canvas.SetZIndex(arrow, 100);
                    MainWindow.Current.MyCanvas.Children.Add(arrow);

                    NodeConnectionJsonObject conn = new NodeConnectionJsonObject();
                    conn.from = MainWindow.Current.nodes.Keys.ToList().IndexOf(_startNode);
                    conn.to = MainWindow.Current.nodes.Keys.ToList().IndexOf(_endNode);
                    conn.label = string.Empty;
                    if (!MainWindow.Current.connections.Any(c => c.Key.from == conn.from && c.Key.to == conn.to))
                    {
                        MainWindow.Current.connections.Add(conn, arrow);
                        _isConnectionExist = false;
                    }
                    else
                    {
                        _isConnectionExist = true;
                        border.RenderTransform = new TranslateTransform { X = 0, Y = 0 };
                        border.Child = tb;
                        Canvas.SetLeft(border, _mouseDownTemp.X + 10);
                        Canvas.SetTop(border, _mouseDownTemp.Y + 10);
                        tb.Text = "当前Connection已经存在!";
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
                        tb.Text = "所选的EndNode与StartNode相同!";
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

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!_isAnyPicked)
            {
                Point pos = e.GetPosition(MainWindow.Current.MyCanvas);
                Point vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                TranslateTransform translate = new TranslateTransform { X = vector.X, Y = vector.Y };
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
        private Node _movingNode = new Node();
        private bool _isPickedAny = false;
        TranslateTransform translate;

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging && _isPickedAny)
            {
                Point catchPos = GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));

                Point vector = new Point(catchPos.X - _mouseDownTemp.X, catchPos.Y - _mouseDownTemp.Y);
                translate = new TranslateTransform { X = vector.X, Y = vector.Y };
                _movingNode.RenderTransform = translate;

                SetConnectionPosition(new Point(_movingNode.Position.X + vector.X, _movingNode.Position.Y + vector.Y));
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _mouseDownTemp = GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));
            foreach (var child in MainWindow.Current.MyCanvas.Children)
            {
                if (child is Node)
                {
                    Node node = child as Node;
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

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (_isPickedAny)
            {
                Point catchPos = GetCatchPoint(e.GetPosition(MainWindow.Current.MyCanvas));

                translate = new TranslateTransform { X = 0, Y = 0 };
                _movingNode.RenderTransform = translate;

                _movingNode.Position = GetCatchPoint(new Point(_originPos.X + catchPos.X - _mouseDownTemp.X, _originPos.Y + catchPos.Y - _mouseDownTemp.Y));
                _movingNode.ReadyControl();

                SetConnectionPosition(_movingNode.Position);
            }
            _isDragging = false;
            _isPickedAny = false;
        }

        public void SetConnectionPosition(Point catchPos)
        {
            int index = MainWindow.Current.nodes.Keys.ToList().IndexOf(_movingNode);
            Dictionary<NodeConnectionJsonObject, BezierLink> fromConns = new Dictionary<NodeConnectionJsonObject, BezierLink>();
            Dictionary<NodeConnectionJsonObject, BezierLink> toConns = new Dictionary<NodeConnectionJsonObject, BezierLink>();

            MainWindow.Current.connections.ForEach(c =>
            {
                if (c.Key.from == index)
                {
                    fromConns.Add(c.Key, c.Value);
                }
                else if (c.Key.to == index)
                {
                    toConns.Add(c.Key, c.Value);
                }
            });
            fromConns.ForEach(c =>
            {
                c.Value.StartPoint = catchPos;
                c.Value.ReadyControl();
            });
            toConns.ForEach(c =>
            {
                c.Value.EndPoint = catchPos;
                c.Value.ReadyControl();
            });
        }

        public Point GetCatchPoint(Point pos)
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
            Point catchPos = new Point(ix * 10, iy * 10);
            
            return catchPos;
        }
    }

    /// <summary>
    /// 单击结点高亮选中（准备删除，或查看属性）
    /// </summary>
    public class SelectNodeTool : ViewerTool
    {
        private Color _defaultColor = Colors.Gray;
        private Color _highlightColor = Colors.Orange;

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(MainWindow.Current.MyCanvas);
            foreach (var child in MainWindow.Current.MyCanvas.Children)
            {
                if (child is Node)
                {
                    Node node = child as Node;
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

    /// <summary>
    /// 双击结点，更改结点信息
    /// </summary>
    public class EditNodeTool : ViewerTool
    {
        public Node currentNode = new Node();
        public FlowNodeJsonObject currentNodeJson = new FlowNodeJsonObject();
        private NodeInfoWindow nodeInfow;

        public override void MouseLDoubleClickHandler(object sender, MouseEventArgs e)
        {
            Point _mouseDownTemp = e.GetPosition(MainWindow.Current.MyCanvas);
            foreach (var child in MainWindow.Current.MyCanvas.Children)
            {
                if (child is Node)
                {
                    Node node = child as Node;
                    if (node.IsPointInNode(_mouseDownTemp))
                    {
                        currentNode = MainWindow.Current.CurrentNode;
                        currentNodeJson = MainWindow.Current.nodes[currentNode];

                        nodeInfow = new NodeInfoWindow();
                        nodeInfow.SetNodeName(currentNodeJson.name);
                        nodeInfow.SetNodeRole(currentNodeJson.role);
                        nodeInfow.SetNodeUser(currentNodeJson.user);
                        nodeInfow.Show();

                        nodeInfow.OKButton.Click += new RoutedEventHandler(OKButton_Click);

                        break;
                    }
                }
            }
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string name = nodeInfow.GetNodeName();
            int s = 1;
            while (MainWindow.Current.nodes.Keys.Where(n=>n!=currentNode).ToList().Any(n => n.Text == name))
            {
                name = string.Format("{0}{1}", nodeInfow.GetNodeName(), s.ToString());
                s++;
            }
            currentNodeJson.name = name;
            currentNodeJson.role = nodeInfow.GetNodeRole();
            currentNodeJson.user = nodeInfow.GetNodeUser();
            currentNode.SetText(name);
        }
    }

    public class EditConnectionTool : ViewerTool
    {
        private ConnectionInfoWindow _connectionInfow;
        private NodeConnectionJsonObject _currentConn;
        private Node _startNode;
        private Node _endNode;
        private int count = 0;
        private bool _isFinished = false;
        private bool _isAnyPicked = false;
        private bool _isTbAdd = false;
        private bool _isStarted = false;
        //private bool _isSelfSelected = false;
        private bool _isConnectionExist = false;
        private Border border = new Border { BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.DarkGray) };
        private TextBlock tb = new TextBlock();
        private Point _mouseDownTemp;

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
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
                if (child is Node)
                {
                    Node node = child as Node;
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
                            _endNode = node;
                            break;
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
                    tb.Text = "当前未选中任何StartNode\n请重新选择StartNode";
                }
                else
                {
                    tb.Text = "当前未选中任何EndNode\n请重新选择EndNode";
                }
                MainWindow.Current.MyCanvas.Children.Add(border);
                _isTbAdd = true;
            }            
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            if (count == 2)
            {
                _isFinished = true;
                bool isfindconn = false;
                if (_startNode != null && _endNode != null)
                {
                    int start = MainWindow.Current.nodes.Keys.ToList().IndexOf(_startNode);
                    int end = MainWindow.Current.nodes.Keys.ToList().IndexOf(_endNode);
                    if (MainWindow.Current.connections.Keys.Any(x=>x.from == start))
                    {
                        MainWindow.Current.connections.Keys.Where(x => x.from == start).ForEach(x =>
                        {
                            if (x.to == end)
                            {
                                _currentConn = x;
                                isfindconn = true;
                            }                                
                        });                        
                    }
                    else
                    {
                        isfindconn = false;                        
                    }

                    if (!isfindconn)
                    {
                        border.RenderTransform = new TranslateTransform { X = 0, Y = 0 };
                        border.Child = tb;
                        Canvas.SetLeft(border, _mouseDownTemp.X + 10);
                        Canvas.SetTop(border, _mouseDownTemp.Y + 10);
                        tb.Text = "当前所选Connection不存在!";
                        if (!_isTbAdd)
                        {
                            MainWindow.Current.MyCanvas.Children.Add(border);
                        }
                        _isTbAdd = true;
                        _isConnectionExist = false;
                    }

                    else
                    {
                        _isConnectionExist = true;

                        _connectionInfow = new ConnectionInfoWindow();
                        _connectionInfow.SetConnection(_currentConn.label);
                        _connectionInfow.Show();

                        _connectionInfow.OKButton.Click += new RoutedEventHandler(OKButton_Click);
                    }
                }
            }
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionInfo = _connectionInfow.GetConnection();
            _currentConn.label = connectionInfo;
            //MainWindow.Current.connections[_currentConn].LabelText = connectionInfo;
            MainWindow.Current.connections[_currentConn].ReadyControl();
        }

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (!_isAnyPicked)
            {
                Point pos = e.GetPosition(MainWindow.Current.MyCanvas);
                Point vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                TranslateTransform translate = new TranslateTransform { X = vector.X, Y = vector.Y };
                border.RenderTransform = translate;
            }
            if (!_isConnectionExist)
            {
                _isConnectionExist = true;
                MainWindow.Current.MyCanvas.Children.Remove(border);
                _isTbAdd = false;
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
}
