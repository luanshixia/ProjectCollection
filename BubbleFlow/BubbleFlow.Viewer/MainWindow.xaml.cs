using Dreambuild.Extensions;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BubbleFlow.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }
        public Point Origin { get; private set; }
        public double Scale { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            MainWindow.Current = this;

            this.Scale = 1;
            ViewerToolManager.AddTool(new WheelScalingTool());
            ViewerToolManager.AddTool(new PanCanvasTool());
            ViewerToolManager.SetFrameworkElement(this);
        }

        public void PanCanvas(Vector displacement)
        {
            this.Origin += displacement;
            this.RenderLayers();
        }

        public void ScaleCanvas(double newScale, Point basePoint)
        {
            var v1 = basePoint - this.Origin;
            var v2 = (this.Scale / newScale) * v1;

            this.Scale = newScale;
            this.Origin += v1 - v2;
            //this.Origin += (1 - this.Scale / newScale) * (basePoint - this.Origin); // floating point error?
            this.RenderLayers();
        }

        public void RenderLayers()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = 1 / Scale });
            transform.Children.Add(new TranslateTransform { X = Origin.X, Y = Origin.Y });
            MyCanvas.RenderTransform = transform;
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

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            this.Open();
        }
    }
}
