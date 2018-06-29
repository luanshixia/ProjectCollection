using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
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

        //public Viewbox viewbox1 = new Viewbox();
        //public ScrollViewer scrollViewer1 = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, BorderThickness = new Thickness(0) };
        //public Canvas MyCanvas = new Canvas();

        public MainWindow()
        {
            InitializeComponent();

            Current = this;

            //Zoom(false);
            Scale = 1;
            ViewerToolManager.AddTool(new WheelScalingTool());
            ViewerToolManager.AddTool(new PanCanvasTool());
            ViewerToolManager.SetFrameworkElement(this);
        }

        public void AddAssist()
        {
            Ellipse ell = new Ellipse { Width = 100, Height = 100, Fill = new SolidColorBrush(Colors.Black) };
            Canvas.SetLeft(ell, 100);
            Canvas.SetTop(ell, 100);
            MyCanvas.Children.Add(ell);
        }

        public void Zoom(bool extents)
        {
            if (extents)
            {
                //scrollViewer1.Content = null;
                //viewbox1.Child = MyCanvas;
                //border1.Child = viewbox1;
            }
            else
            {
                //viewbox1.Child = null;
                //scrollViewer1.Content = MyCanvas;
                //border1.Child = scrollViewer1;
            }
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

            //MainPage.Current.SRuler.SetScaleRulerValue(Scale);
            ViewerToolManager.Tools.ForEach(t => t.Render());
        }
    }
}
