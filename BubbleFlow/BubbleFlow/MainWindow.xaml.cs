using Dreambuild.Extensions;
using Dreambuild.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Vector = Dreambuild.Geometry.Vector;

namespace BubbleFlow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }
        public Point Origin { get; private set; }
        public Node CurrentNode { get; set; }
        public double Scale { get; set; }

        public Dictionary<Node, FlowNodeJsonObject> Nodes { get; } = new Dictionary<Node, FlowNodeJsonObject>();
        public Dictionary<NodeConnectionJsonObject, BezierLink> Connections { get; } = new Dictionary<NodeConnectionJsonObject, BezierLink>();
        public List<BoolExpression> Expressions { get; } = new List<BoolExpression>();
        private List<Line> GridLines { get; } = new List<Line>();

        public MainWindow()
        {
            this.InitializeComponent();

            MainWindow.Current = this;

            this.InitializeTool();

            this.AddGridLinesToCanvas();
        }

        private void InitNodesAndConnections(WorkflowJsonObject flow)
        {
            //var workflowId = flow.id;

            this.Nodes.Clear();
            foreach (var node in flow.nodes)
            {
                foreach (var child in MyCanvas.Children)
                {
                    if (child is Node nodeMark)
                    {
                        if (nodeMark.Text == node.name)
                        {
                            this.Nodes.Add(nodeMark, node);
                            break;
                        }
                    }
                }
            }

            this.Connections.Clear();
            foreach (var conn in flow.connections)
            {
                var start = new Point(flow.nodes[conn.from].xpos, flow.nodes[conn.from].ypos);
                var end = new Point(flow.nodes[conn.to].xpos, flow.nodes[conn.to].ypos);
                foreach (var child in MyCanvas.Children)
                {
                    if (child is BezierLink arrow)
                    {
                        if (arrow.StartPoint == start && arrow.EndPoint == end)
                        {
                            this.Connections.Add(conn, arrow);
                            break;
                        }
                    }
                }
            }
        }

        private void AddGridLinesToCanvas()
        {
            this.GridLines.Clear();
            int count = 10000;
            for (int i = 0; i <= count; i++)
            {
                var vline = new Line { X1 = 0, X2 = 0, Y1 = 0, Y2 = 100000, StrokeThickness = 0.2 };
                vline.Stroke = new SolidColorBrush(Colors.DarkGray);
                this.GridLines.Add(vline);
                Canvas.SetLeft(vline, i * 10 - 50000);
                Canvas.SetTop(vline, -50000);
                this.MyCanvas.Children.Add(vline);

                var hline = new Line { X1 = 0, X2 = 100000, Y1 = 0, Y2 = 0, StrokeThickness = 0.2 };
                hline.Stroke = new SolidColorBrush(Colors.DarkGray);
                this.GridLines.Add(hline);
                Canvas.SetLeft(hline, -50000);
                Canvas.SetTop(hline, i * 10 - 50000);
                this.MyCanvas.Children.Add(hline);
            }
        }

        private void InitializeTool()
        {
            var toggleButtons = this.Toolbar.Children
                .Cast<ButtonBase>()
                .Where(button => button is ToggleButton)
                .Cast<ToggleButton>()
                .ToArray();

            toggleButtons.ForEach(toggleButton => toggleButton.Click += (sender, e) =>
            {
                toggleButtons.ForEach(other => other.IsChecked = false);
                (sender as ToggleButton).IsChecked = true;
            });

            this.Scale = 1;
            ViewerToolManager.AddTool(new WheelScalingTool());
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
            ViewerToolManager.SetFrameworkElement(this);
        }

        private void ResetTool()
        {
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
        }

        private void Zoom(Extents extents)
        {
            Scale = Math.Max(extents.Range(0) / this.ActualWidth, extents.Range(1) / this.ActualHeight);
            Origin = new Point(this.ActualWidth / 2 - extents.Center().X / Scale, this.ActualHeight / 2 - extents.Center().Y / Scale);
            RenderLayers();
        }

        internal void PanCanvas(Point vector)
        {
            this.Origin = new Point(Origin.X + vector.X, Origin.Y + vector.Y);
            this.RenderLayers();
        }

        internal void ScaleCanvas(double scale, Point basePoint)
        {
            double scale0 = this.Scale;
            double vx = basePoint.X - Origin.X;
            double vy = basePoint.Y - Origin.Y;
            double v1x = (scale0 / scale) * vx;
            double v1y = (scale0 / scale) * vy;
            double v2x = vx - v1x;
            double v2y = vy - v1y;

            this.Scale = scale;
            this.Origin = new Point(Origin.X + v2x, Origin.Y + v2y);
            this.RenderLayers();
        }

        private void RenderLayers()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = 1 / Scale });
            transform.Children.Add(new TranslateTransform { X = Origin.X, Y = Origin.Y });
            this.MyCanvas.RenderTransform = transform;
            ViewerToolManager.Tools.ForEach(tool => tool.Render());
            if (!this.MyCanvas.Children.Contains(this.GridLines[0]))
            {
                this.AddGridLinesToCanvas();
            }
        }

        private void SubmitAction(object sender, RoutedEventArgs e)
        {
            //var flowName = qfnw.GetFlowName();
            Nodes.ForEach(nodePair =>
            {
                nodePair.Value.xpos = nodePair.Key.Position.X;
                nodePair.Value.ypos = nodePair.Key.Position.Y;
            });

            var data = DataManager.ToJson(new WorkflowJsonObject
            {
                nodes = Nodes.Values.ToList(),
                connections = Connections.Keys.ToList()
            });

            //WebClient wc = new WebClient();
            //Uri uri = new Uri(SiteBaseUri, string.Format("Workflow/NewFlowFromJson")); // mod 20130605 // mod 20130621
            //wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //wc.UploadStringAsync(uri, "POST", "name=" + flowName + "&data=" + data);
            //wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
        }

        //void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        //{
        //    Uri uri = new Uri(SiteBaseUri, string.Format("Workflow/FlowList")); // mod 20130605
        //    System.Windows.Browser.HtmlPage.Window.Eval(string.Format("location='{0}'", uri.ToString()));
        //}

        private void InitializeExpression()
        {
            this.Expressions.Add(new BoolExpression(this.GetRule1()));
            this.Expressions.Add(new BoolExpression(this.GetRule2()));
        }

        private Func<bool> GetRule1()
        {
            foreach (var node in this.Nodes.Keys)
            {
                int index = this.Nodes.Keys.ToList().IndexOf(node);
                if (this.Connections.Keys.All(c => c.from != index && c.to != index))
                {
                    return () => false;
                }
            }
            return () => true;
        }

        private Func<bool> GetRule2()
        {
            int startcount = 0;
            int endcount = 0;
            foreach (var node in this.Nodes.Keys)
            {
                int index = this.Nodes.Keys.ToList().IndexOf(node);
                if (this.Connections.Keys.All(c => c.to != index) && this.Connections.Keys.Any(c => c.from == index))
                {
                    startcount++;
                }
                if (this.Connections.Keys.All(c => c.from != index) && this.Connections.Keys.Any(c => c.to == index))
                {
                    endcount++;
                }
            }
            if (startcount == 1 && endcount == 1)
            {
                return () => true;
            }
            return () => false;
        }

        #region General event handlers

        private void MyCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ResetTool();
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(x => x.KeyDownHandler(sender, e));
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(this.MyCanvas);
            Message.Text = string.Format("{0},{1}", pos.X.ToString("0.00"), pos.Y.ToString("0.00"));
        }

        #endregion

        #region Command event handlers

        private void PanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
        }

        private void ZoomExtentsButton_Click(object sender, RoutedEventArgs e)
        {
            var points = new List<Vector>();
            foreach (var child in MyCanvas.Children)
            {
                if (child is Node node)
                {
                    points.Add(new Vector(node.Position.X, node.Position.Y));
                }
            }
            if (points.Count == 0)
            {
                return;
            }
            var poly = new PointString(points);
            var extents = poly.GetExtents();
            var et = new Extents(extents.Min.Value.Add(new Vector(-200, -200)), extents.Max.Value.Add(new Vector(200, 200)));
            this.Zoom(et);
        }

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new AddNodeTool();
        }

        private void AddConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new AddConnectionTool();
        }

        private void EditConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new EditConnectionTool();
        }

        private void MoveNodeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new MoveNodeTool();
        }

        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentNode != null)
            {
                int index = Nodes.Keys.ToList().IndexOf(CurrentNode);
                var conns = new Dictionary<NodeConnectionJsonObject, BezierLink>();

                MyCanvas.Children.Remove(CurrentNode);
                Nodes.Remove(CurrentNode);
                CurrentNode = null;

                Connections.ForEach(c =>
                {
                    if (c.Key.from == index || c.Key.to == index)
                    {
                        conns.Add(c.Key, c.Value);
                    }
                    if (c.Key.from > index)
                    {
                        c.Key.from -= 1;
                    }
                    if (c.Key.to > index)
                    {
                        c.Key.to -= 1;
                    }
                });
                conns.ForEach(c =>
                {
                    MyCanvas.Children.Remove(c.Value);
                    Connections.Remove(c.Key);
                });
            }

            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            Expressions.Clear();
            InitializeExpression();

            bool flag = false;
            if (Expressions.Count == 0)
            {
                flag = true;
            }
            if (Expressions.Count > 0 && Expressions.All(x => x.GetValue()))
            {
                flag = true;
            }

            if (flag)
            {
                var qfnw = new QueryFlowNameWindow();
                qfnw.Show();
                qfnw.OKButton.Click += new RoutedEventHandler(SubmitAction);
            }
            else
            {
                var trw = new TestResultWindow();
                trw.SetInfomation("Invalid node(s) detected.");
                trw.Show();
                SubmitButton.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        #endregion
    }

    public class BoolExpression
    {
        private Func<bool> _expression;

        public BoolExpression()
        {
            _expression = new Func<bool>(() => true);
        }

        public BoolExpression(Func<bool> expression)
        {
            _expression = expression;
        }

        public bool GetValue()
        {
            return _expression();
        }

        public void SetExpression(Func<bool> expr)
        {
            _expression = expr;
        }
    }
}
