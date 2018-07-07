using Dreambuild.Collections;
using Dreambuild.Extensions;
using Dreambuild.Geometry;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        public NodeBubble SelectedBubble { get; set; }
        public double Scale { get; set; }

        public Dictionary<Guid, NodeBubble> Bubbles { get; } = new Dictionary<Guid, NodeBubble>();
        public DoubleDictionary<Guid, Guid, BezierLink> Arrows { get; } = new DoubleDictionary<Guid, Guid, BezierLink>();
        private List<Line> GridLines { get; } = new List<Line>();

        public MainWindow()
        {
            this.InitializeComponent();

            MainWindow.Current = this;

            DataManager.New();
            this.InitializeTool();

            this.AddGridLinesToCanvas();
        }

        private void AddGridLinesToCanvas()
        {
            this.GridLines.Clear();

            var radius = 10000;
            var interval = 25;

            for (var x = - radius; x <= radius; x += interval)
            {
                var line = new Line
                {
                    X1 = x,
                    Y1 = -radius,
                    X2 = x,
                    Y2 = radius,
                    Stroke = new SolidColorBrush(x == 0 ? Colors.DarkGray : Colors.LightGray),
                    StrokeThickness = x == 0 ? 2 : 1,
                    //SnapsToDevicePixels = true
                };

                this.GridLines.Add(line);
                this.MyCanvas.Children.Add(line);

                line = new Line
                {
                    X1 = -radius,
                    Y1 = x,
                    X2 = radius,
                    Y2 = x,
                    Stroke = new SolidColorBrush(x == 0 ? Colors.DarkGray : Colors.LightGray),
                    StrokeThickness = x == 0 ? 2 : 1,
                    //SnapsToDevicePixels = true
                };

                this.GridLines.Add(line);
                this.MyCanvas.Children.Add(line);
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
            this.Scale = Math.Max(extents.Range(0) / this.ActualWidth, extents.Range(1) / this.ActualHeight);
            this.Origin = new Point(this.ActualWidth / 2 - extents.Center().X / this.Scale, this.ActualHeight / 2 - extents.Center().Y / this.Scale);
            this.RenderLayers();
        }

        internal void PanCanvas(System.Windows.Vector displacement)
        {
            this.Origin += displacement;
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
        }

        private void Submit()
        {
            var (succeeded, messages) = DataManager.Validate();
            if (succeeded)
            {
                this.Save();
            }
            else
            {
                MessageBox.Show("Workflow validation has failed. Violation(s):\n" + string.Join("\n", messages));
            }
        }

        private void Save()
        {
            if (DataManager.CurrentFileName == null)
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "JSON files (.json)|*.json|All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    DataManager.SaveAs(saveFileDialog.FileName);
                }

                return;
            }

            DataManager.SaveAs(DataManager.CurrentFileName);
        }

        private void Open()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (.json)|*.json|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                DataManager.Open(openFileDialog.FileName);
                DataManager.CurrentDocument.DrawToCanvas(this.MyCanvas);
            }
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
            this.Message.Text = string.Format("{0:0.00},{1:0.00}", pos.X, pos.Y);
        }

        #endregion

        #region Command event handlers

        private void PanButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
        }

        private void ZoomExtentsButton_Click(object sender, RoutedEventArgs e)
        {
            var bubblePositions = this.MyCanvas.Children
                .Cast<UIElement>()
                .Where(element => element is NodeBubble)
                .Cast<NodeBubble>()
                .Select(bubble => new Vector(bubble.Position.X, bubble.Position.Y))
                .ToArray();

            if (bubblePositions.Length == 0)
            {
                return;
            }

            var extents = new Extents(bubblePositions);
            this.Zoom(extents.Offset(200));
        }

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new AddNodeTool();
        }

        private void AddConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new AddLinkTool();
        }

        private void EditConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new EditLinkTool();
        }

        private void MoveNodeButton_Click(object sender, RoutedEventArgs e)
        {
            ViewerToolManager.ExclusiveTool = new MoveNodeTool();
        }

        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedBubble != null)
            {
                var nodeID = this.SelectedBubble.NodeID;
                this.MyCanvas.Children.Remove(this.SelectedBubble);
                this.Bubbles.Remove(nodeID);
                DataManager.CurrentDocument.NodesStore.Remove(nodeID);
                this.SelectedBubble = null;

                this.Arrows.RealValues.ForEach(arrow =>
                {
                    if (arrow.FromNodeID == nodeID || arrow.ToNodeID == nodeID)
                    {
                        this.MyCanvas.Children.Remove(arrow);
                        this.Arrows.Remove(arrow.FromNodeID, arrow.ToNodeID);
                        DataManager.CurrentDocument.LinksStore.Remove(arrow.FromNodeID, arrow.ToNodeID);
                    }
                });
            }

            ViewerToolManager.ExclusiveTool = new PanCanvasTool();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Submit();
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

    public static class Gui
    {
        /// <summary>
        /// Shows a multi-inputs window.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="entries">The input entries.</param>
        public static void MultiInputs(string title, Dictionary<string, string> entries)
        {
            var mi = new MultiInputs();
            mi.Ready(entries, title);
            mi.ShowDialog();
        }
    }
}
