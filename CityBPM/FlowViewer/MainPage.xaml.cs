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

namespace FlowViewer
{
    public partial class MainPage : UserControl
    {
        public static MainPage Current { get; private set; }
        public Point Origin { get; private set; }
        public double Scale { get; set; }

        //public Viewbox viewbox1 = new Viewbox();
        //public ScrollViewer scrollViewer1 = new ScrollViewer { HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, BorderThickness = new Thickness(0) };
        //public Canvas MyCanvas = new Canvas();

        public MainPage()
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

    public class WheelScalingTool : ViewerTool
    {
        private static double[] _zoomLevels = new double[] { 64, 32, 16, 8, 4, 2, 1, 0.5, 0.25, 0.125, 0.0625, 0.03125, 0.015625 };

        public override void MouseWheelHandler(object sender, MouseWheelEventArgs e)
        {
            base.MouseWheelHandler(sender, e);

            Point basePoint = e.GetPosition(MainPage.Current);
            int index = FindScaleIndex(MainPage.Current.Scale);
            index += e.Delta / 120;
            if (index > _zoomLevels.Length - 1) index = _zoomLevels.Length - 1;
            else if (index < 0) index = 0;
            double scale = _zoomLevels[index];
            MainPage.Current.ScaleCanvas(scale, basePoint);
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

        public override void MouseMoveHandler(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                Point pos = e.GetPosition(MainPage.Current);
                Point vector = new Point(pos.X - _mouseDownTemp.X, pos.Y - _mouseDownTemp.Y);
                MainPage.Current.PanCanvas(vector);
                _mouseDownTemp = pos;
            }
        }

        public override void MouseLDownHandler(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _mouseDownTemp = e.GetPosition(MainPage.Current);
        }

        public override void MouseLUpHandler(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }
    }
}
