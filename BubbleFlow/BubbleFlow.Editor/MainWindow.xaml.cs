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
using Vector = Dreambuild.Geometry.Vector;

namespace BubbleFlow.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }
        public Point Origin { get; private set; }
        public NodeBubble SelectedBubble { get; set; }
        public GridLines GridLines { get; } = new GridLines();
        public double Scale { get; set; }

        public Dictionary<Guid, NodeBubble> Bubbles { get; } = new Dictionary<Guid, NodeBubble>();
        public DoubleDictionary<Guid, Guid, BezierLink> Arrows { get; } = new DoubleDictionary<Guid, Guid, BezierLink>();

        public MainWindow()
        {
            this.InitializeComponent();

            MainWindow.Current = this;

            this.InitializeTool();
            this.New();
        }

        private void ShowGridLines(bool show = true)
        {
            if (!show)
            {
                this.MyCanvas.Children.Remove(this.GridLines);
            }
            else if (!this.MyCanvas.Children.Contains(this.GridLines))
            {
                this.GridLines.ReadyControl();
                this.MyCanvas.Children.Add(this.GridLines);
            }
        }

        private void InitializeTool()
        {
            var toggleButtons = this.Toolbar.Children
                .Cast<UIElement>()
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

        internal void ScaleCanvas(double newScale, Point basePoint)
        {
            var v1 = basePoint - this.Origin;
            var v2 = (this.Scale / newScale) * v1;

            this.Scale = newScale;
            this.Origin += v1 - v2;
            //this.Origin += (1 - this.Scale / newScale) * (basePoint - this.Origin); // floating point error?
            this.RenderLayers();
        }

        private void RenderLayers()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = 1 / Scale });
            transform.Children.Add(new TranslateTransform { X = Origin.X, Y = Origin.Y });
            this.MyCanvas.RenderTransform = transform;
        }

        private void Submit(bool newFile = false)
        {
            var (succeeded, messages) = DataManager.Validate();
            if (succeeded)
            {
                this.Save(newFile);
            }
            else
            {
                MessageBox.Show(
                    messageBoxText: "Workflow validation has failed. Violation(s):\n" + string.Join("\n", messages),
                    caption: "BubbleFlow");
            }
        }

        private void Save(bool newFile = false)
        {
            if (DataManager.CurrentFileName == null || newFile)
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
                this.ShowGridLines();
            }
        }

        private void New()
        {
            DataManager.New();
            this.MyCanvas.Children.Clear();
            this.ShowGridLines();
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
                this.ResetTool();
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

            var extents = Extents.FromPoints(bubblePositions);
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

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            this.New();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Open();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Submit();
        }

        private void SaveAsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Submit(newFile: true);
        }

        private void GridToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            this.ShowGridLines(true);
        }

        private void GridToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ShowGridLines(false);
        }

        #endregion
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
