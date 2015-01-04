using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using SlFlowLib;

namespace FlowEditor
{
    public partial class MainPage : UserControl
    {
        public static MainPage Current { get; private set; }
        public Point Origin { get; private set; }
        public Node CurrentNode { get; set; }
        public List<Line> Griddings = new List<Line>();

        public Dictionary<Node, FlowNodeJsonObject> nodes = new Dictionary<Node, FlowNodeJsonObject>();
        public Dictionary<NodeConnectionJsonObject, BezierLink> connections = new Dictionary<NodeConnectionJsonObject, BezierLink>();

        private Button[] _radioButtons;
        private Button _currentButton;
        private TestResultWindow trw = new TestResultWindow();
        private QueryFlowNameWindow qfnw;

        public double Scale { get; set; }
        public static Uri SiteBaseUri { get; private set; }
        private string flowName;

        public List<BoolExpression> Exprs = new List<BoolExpression>();
        private string workflowId = new Guid().ToString();

        public MainPage()
        {
            InitializeComponent();

            Current = this;

            _radioButtons = new Button[] { btnAddConnection, btnEditConnection, btnAddNode, btnMoveNode, btnSubmit, btnPan, btnSubmit1 };
            SetButtonsHover();
            SetRadioButton(btnPan);

            Scale = 1;
            ViewerToolManager.AddTool(new WheelScalingTool());
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
            ViewerToolManager.SetFrameworkElement(this);

            AddGriddingToCanvas();
            MouseMove += new MouseEventHandler(MainPage_MouseMove);

            SiteBaseUri = new Uri(Application.Current.Host.Source, "../");
        }

        public void InitNodesAndConnections(WorkflowJsonObject flow)
        {
            workflowId = flow.id;
            nodes.Clear();
            connections.Clear();
            foreach (var node in flow.nodes)
            {
                foreach (var x in MyCanvas.Children)
                {
                    if (x is Node)
                    {
                        Node nodeMark = x as Node;
                        if (nodeMark.Text == node.name)
                        {
                            nodes.Add(nodeMark, node);
                            break;
                        }
                    }
                }
            }
            foreach (var conn in flow.connections)
            {
                Point start = new Point(flow.nodes[conn.from].xpos, flow.nodes[conn.from].ypos);
                Point end = new Point(flow.nodes[conn.to].xpos, flow.nodes[conn.to].ypos);
                foreach (var x in MyCanvas.Children)
                {
                    if (x is BezierLink)
                    {
                        BezierLink arrow = x as BezierLink;
                        if (arrow.StartPoint == start && arrow.EndPoint == end)
                        {
                            connections.Add(conn, arrow);
                            break;
                        }
                    }
                }
            }
        }

        public void InitToolbarForModify()
        {
            btnAddConnection.Visibility = System.Windows.Visibility.Collapsed;
            btnAddNode.Visibility = System.Windows.Visibility.Collapsed;
            btnDeleteNode.Visibility = System.Windows.Visibility.Collapsed;
            btnEditConnection.Visibility = System.Windows.Visibility.Collapsed;
            btnSubmit.Visibility = System.Windows.Visibility.Collapsed;
            btnZoomE.Visibility = System.Windows.Visibility.Collapsed;
            btnSubmit1.Visibility = System.Windows.Visibility.Visible;
        }

        public void AddGriddingToCanvas()
        {
            Griddings.Clear();
            int count = 10000;
            for (int i = 0; i <= count; i++)
            {
                Line vline = new Line { X1 = 0, X2 = 0, Y1 = 0, Y2 = 100000, StrokeThickness = 0.2 };
                vline.Stroke = new SolidColorBrush(Colors.DarkGray);
                Griddings.Add(vline);
                Canvas.SetLeft(vline, i * 10 - 50000);
                Canvas.SetTop(vline, -50000);
                MyCanvas.Children.Add(vline);

                Line hline = new Line { X1 = 0, X2 = 100000, Y1 = 0, Y2 = 0, StrokeThickness = 0.2 };
                hline.Stroke = new SolidColorBrush(Colors.DarkGray);
                Griddings.Add(hline);
                Canvas.SetLeft(hline, -50000);
                Canvas.SetTop(hline, i * 10 - 50000);
                MyCanvas.Children.Add(hline);
            }
        }

        void MainPage_MouseMove(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(MainPage.Current.MyCanvas);
            Message.Text = string.Format("{0},{1}", pos.X.ToString("0.00"), pos.Y.ToString("0.00"));
        }

        public void InitializeTool()
        {
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
            SetRadioButton(btnPan);
        }

        private void SetRadioButton(Button btn)
        {
            foreach (var b in _radioButtons)
            {
                b.Background = new SolidColorBrush(Colors.Transparent);
            }
            btn.Background = new SolidColorBrush(Color.FromArgb(255, 204, 51, 0));
            _currentButton = btn;
        }

        private void SetButtonsHover()
        {
            foreach (var btn in _radioButtons)
            {
                btn.MouseMove += new MouseEventHandler(btn_MouseMove);
                btn.MouseLeave += new MouseEventHandler(btn_MouseLeave);
            }
            btnZoomE.MouseMove += new MouseEventHandler(btnZoomE_MouseMove);
            btnZoomE.MouseLeave += new MouseEventHandler(btnZoomE_MouseLeave);
            btnDeleteNode.MouseMove += new MouseEventHandler(btnZoomE_MouseMove);
            btnDeleteNode.MouseLeave += new MouseEventHandler(btnZoomE_MouseLeave);
        }

        void btnZoomE_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = new SolidColorBrush(Colors.Transparent);
        }

        void btnZoomE_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            btn.Background = new SolidColorBrush(Colors.Orange);
        }

        void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == _currentButton)
            {
                return;
            }
            btn.Background = new SolidColorBrush(Colors.Transparent);
        }

        void btn_MouseMove(object sender, MouseEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == _currentButton)
            {
                return;
            }
            btn.Background = new SolidColorBrush(Colors.Orange);
        }


        public void Zoom(TongJi.Geometry.Extent2D extents)
        {
            Scale = Math.Max(extents.XRange / this.ActualWidth, extents.YRange / this.ActualHeight);
            Origin = new Point(this.ActualWidth / 2 - extents.Center.x / Scale, this.ActualHeight / 2 - extents.Center.y / Scale);
            RenderLayers();
        }

        private void btnPan_Click_1(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();

            Button btn = sender as Button;
            SetRadioButton(btn);
        }

        private void btnZoomE_Click_1(object sender, RoutedEventArgs e)
        {
            List<TongJi.Geometry.Point2D> points = new List<TongJi.Geometry.Point2D>();
            foreach (var child in MyCanvas.Children)
            {
                if (child is Node)
                {
                    Node node = child as Node;
                    points.Add(new TongJi.Geometry.Point2D(node.Position.X, node.Position.Y));
                }
            }
            if (points.Count == 0)
            {
                return;
            }
            TongJi.Geometry.Polygon poly = new TongJi.Geometry.Polygon(points);
            var extents = poly.GetExtent();
            TongJi.Geometry.Extent2D et = new TongJi.Geometry.Extent2D(extents.min.x - 200, extents.min.y - 200, extents.max.x + 200, extents.max.y + 200);
            Zoom(et);
        }

        private void btnAddNode_Click_1(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new AddNodeTool();
            Button btn = sender as Button;
            SetRadioButton(btn);
        }

        private void btnEditConnection_Click_1(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new EditConnectionTool();
            Button btn = sender as Button;
            SetRadioButton(btn);
        }

        private void btnAddConnection_Click_1(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new AddConnectionTool();
            Button btn = sender as Button;
            SetRadioButton(btn);
        }

        private void btnMoveNode_Click_1(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new MoveNodeTool();
            Button btn = sender as Button;
            SetRadioButton(btn);
        }

        private void btnDeleteNode_Click_1(object sender, RoutedEventArgs e)
        {
            if (CurrentNode != null)
            {
                int index = nodes.Keys.ToList().IndexOf(CurrentNode);
                Dictionary<NodeConnectionJsonObject, BezierLink> conns = new Dictionary<NodeConnectionJsonObject, BezierLink>();

                MyCanvas.Children.Remove(CurrentNode);
                nodes.Remove(CurrentNode);
                CurrentNode = null;

                connections.ForEach(c =>
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
                    connections.Remove(c.Key);
                });
            }

            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
            Button btn = btnPan;
            SetRadioButton(btn);
        }

        public void PanCanvas(Point vector)
        {
            Origin = new Point(Origin.X + vector.X, Origin.Y + vector.Y);
            RenderLayers();
        }

        public void ScaleCanvas(double scale, Point basePoint)
        {
            double scale0 = this.Scale;
            double vx = basePoint.X - Origin.X;
            double vy = basePoint.Y - Origin.Y;
            double v1x = (scale0 / scale) * vx;
            double v1y = (scale0 / scale) * vy;
            double v2x = vx - v1x;
            double v2y = vy - v1y;

            Scale = scale;
            Origin = new Point(Origin.X + v2x, Origin.Y + v2y);
            RenderLayers();
        }

        public void RenderLayers()
        {
            TranslateTransform translate = new TranslateTransform { X = Origin.X, Y = Origin.Y };
            ScaleTransform scale = new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = 1 / Scale };
            TransformGroup transform = new TransformGroup();
            transform.Children.Add(scale);
            transform.Children.Add(translate);
            MyCanvas.RenderTransform = transform;
            ViewerToolManager.Tools.ForEach(t => t.Render());
            if (!MyCanvas.Children.Contains(Griddings[0]))
            {
                AddGriddingToCanvas();
            }
        }

        private void MyCanvas_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {

        }

        private void UserControl_Loaded_1(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                InitializeTool();
            }
        }

        private void UserControl_KeyDown_1(object sender, KeyEventArgs e)
        {
            ViewerToolManager.Tools.ForEach(x => x.KeyDownHandler(sender, e));
        }

        private void btnSubmit1_Click(object sender, RoutedEventArgs e)
        {
            nodes.ForEach(nodePair =>
            {
                nodePair.Value.xpos = nodePair.Key.Position.X;
                nodePair.Value.ypos = nodePair.Key.Position.Y;
            });
            WorkflowJsonObject json = new WorkflowJsonObject();
            json.nodes = nodes.Values.ToList();
            json.connections = connections.Keys.ToList();
            var data = DataManager.ToJson(json);

            WebClient wc = new WebClient();
            //Uri uri = new Uri(SiteBaseUri, string.Format("Workflow/SaveFlow?data={0}", data));
            Uri uri = new Uri(SiteBaseUri, string.Format("Workflow/SaveFlow")); // mod 20130621
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringAsync(uri, "POST", "data=" + data);
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
        }

        private void btnSubmit_Click_1(object sender, RoutedEventArgs e)
        {
            Exprs.Clear();
            InitialiseExpression();

            bool flag = false;
            if (Exprs.Count == 0)
            {
                flag = true;
            }
            if (Exprs.Count > 0 && Exprs.All(x => x.GetValue()))
            {
                flag = true;
            }

            if (flag)
            {
                qfnw = new QueryFlowNameWindow();
                qfnw.Show();
                qfnw.OKButton.Click += new RoutedEventHandler(OKButton_Click);
            }
            else
            {
                trw.SetInfomation("当前流程中存在无效结点,请调整后再重新提交!");
                trw.Show();
                btnSubmit.Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        void OKButton_Click(object sender, RoutedEventArgs e)
        {
            flowName = qfnw.GetFlowName();
            nodes.ForEach(nodePair =>
            {
                nodePair.Value.xpos = nodePair.Key.Position.X;
                nodePair.Value.ypos = nodePair.Key.Position.Y;
            });
            WorkflowJsonObject json = new WorkflowJsonObject();
            json.nodes = nodes.Values.ToList();
            json.connections = connections.Keys.ToList();
            var data = DataManager.ToJson(json);

            WebClient wc = new WebClient();
            Uri uri = new Uri(SiteBaseUri, string.Format("Workflow/NewFlowFromJson")); // mod 20130605 // mod 20130621
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.UploadStringAsync(uri, "POST", "name=" + flowName + "&data=" + data);
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted);
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            Uri uri = new Uri(SiteBaseUri, string.Format("Workflow/FlowList")); // mod 20130605
            System.Windows.Browser.HtmlPage.Window.Eval(string.Format("location='{0}'", uri.ToString()));
        }

        public void InitialiseExpression()
        {
            BoolExpression exp1 = new BoolExpression(GetRule1());
            BoolExpression exp2 = new BoolExpression(GetRule2());
            Exprs.Add(exp1);
            Exprs.Add(exp2);
        }

        public Func<bool> GetRule1()
        {
            foreach (var node in nodes.Keys)
            {
                int index = nodes.Keys.ToList().IndexOf(node);
                if (connections.Keys.All(c => c.from != index && c.to != index))
                {
                    return () => false;
                }
            }
            return () => true;
        }

        public Func<bool> GetRule2()
        {
            int startcount = 0;
            int endcount = 0;
            foreach (var node in nodes.Keys)
            {
                int index = nodes.Keys.ToList().IndexOf(node);
                if (connections.Keys.All(c => c.to != index) && connections.Keys.Any(c => c.from == index))
                {
                    startcount++;
                }
                if (connections.Keys.All(c => c.from != index) && connections.Keys.Any(c => c.to == index))
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
