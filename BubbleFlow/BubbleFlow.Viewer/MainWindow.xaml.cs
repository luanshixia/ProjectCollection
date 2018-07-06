using Dreambuild.Extensions;
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

        public void AddAssist()
        {
            var ellipse = new Ellipse
            {
                Width = 100,
                Height = 100,
                Fill = new SolidColorBrush(Colors.Black)
            };

            Canvas.SetLeft(ellipse, 100);
            Canvas.SetTop(ellipse, 100);
            MyCanvas.Children.Add(ellipse);
        }

        public void PanCanvas(Vector displacement)
        {
            this.Origin += displacement;
            this.RenderLayers();
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

            this.Scale = scale;
            this.Origin = new Point(Origin.X + v2x, Origin.Y + v2y);

            this.RenderLayers();
        }

        public void RenderLayers()
        {
            var transform = new TransformGroup();
            transform.Children.Add(new ScaleTransform { CenterX = 0, CenterY = 0, ScaleX = 1 / Scale, ScaleY = 1 / Scale });
            transform.Children.Add(new TranslateTransform { X = Origin.X, Y = Origin.Y });
            MyCanvas.RenderTransform = transform;

            ViewerToolManager.Tools.ForEach(tool => tool.Render());
        }
    }
}
