using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleFlow
{
    public class Arrow : Canvas
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double StartOffset { get; set; }
        public double EndOffset { get; set; }
        public double ArrowSize { get; set; }
        public Color BaseColor { get; set; }
        public Color HighlightColor { get; set; }

        private Line BodyShape { get; } = new Line();
        private Polyline HeadShape { get; } = new Polyline();
        private Storyboard Storyboard { get; } = new Storyboard();

        public Arrow()
        {
            this.Children.Add(BodyShape);
            this.Children.Add(HeadShape);

            this.DefaultValue();
            this.ReadyControl();
        }

        private void DefaultValue()
        {
            this.StartPoint = new Point();
            this.EndPoint = new Point(100, 0);
            this.StartOffset = 10;
            this.EndOffset = 10;
            this.ArrowSize = 10;
            this.BaseColor = Colors.Gray;
            this.HighlightColor = Colors.Red;
        }

        public void ReadyControl()
        {
            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            double r1 = StartOffset / dist;
            double r2 = EndOffset / dist;

            this.BodyShape.X1 = r1 * dist;
            this.BodyShape.Y1 = 0;
            this.BodyShape.X2 = dist - r2 * dist;
            this.BodyShape.Y2 = 0;

            this.HeadShape.Points.Clear();
            this.HeadShape.Points.Add(new Point(BodyShape.X2 - ArrowSize, BodyShape.Y2 - 0.5 * ArrowSize));
            this.HeadShape.Points.Add(new Point(BodyShape.X2, BodyShape.Y2));
            this.HeadShape.Points.Add(new Point(BodyShape.X2 - ArrowSize, BodyShape.Y2 + 0.5 * ArrowSize));

            var transform = new TransformGroup();
            transform.Children.Add(new RotateTransform { Angle = Math.Atan2(dy, dx) * 180 / Math.PI });
            transform.Children.Add(new TranslateTransform { X = StartPoint.X, Y = StartPoint.Y });
            this.BodyShape.RenderTransform = transform;
            this.HeadShape.RenderTransform = transform;

            this.BodyShape.StrokeThickness = 3;
            this.HeadShape.StrokeThickness = 3;

            var solidColorBrush = new SolidColorBrush(color: this.BaseColor);
            this.BodyShape.Stroke = solidColorBrush;
            this.HeadShape.Stroke = solidColorBrush;

            var colorAnimation = new ColorAnimation
            {
                From = Colors.Transparent,
                To = HighlightColor,
                AutoReverse = true,
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                RepeatBehavior = RepeatBehavior.Forever
            };

            this.Storyboard.Children.Add(colorAnimation);
            Storyboard.SetTarget(element: colorAnimation, value: solidColorBrush);
            Storyboard.SetTargetProperty(element: colorAnimation, path: new PropertyPath("Color"));
        }

        public void DoAlert()
        {
            this.Storyboard.Begin();
        }

        public void CancelAlert()
        {
            this.Storyboard.Stop();
        }
    }

    public class BezierLink : Canvas
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double StartOffset { get; set; }
        public double EndOffset { get; set; }
        public double ArrowSize { get; set; }
        public Color BaseColor { get; set; }
        public Color HighlightColor { get; set; }
        //public string LabelText { get; set; }
        //public Color LabelColor { get; set; }

        private Path BodayShape { get; } = new Path();
        private Polyline HeadShape { get; } = new Polyline();
        private TextBlock TextLabel { get; } = new TextBlock();
        private Storyboard Storyboard { get; } = new Storyboard();

        public BezierLink()
        {
            this.Children.Add(BodayShape);
            this.Children.Add(HeadShape);
            this.Children.Add(TextLabel);

            this.DefaultValue();
            this.ReadyControl();
        }

        private void DefaultValue()
        {
            this.StartPoint = new Point();
            this.EndPoint = new Point(100, 0);
            this.StartOffset = 10;
            this.EndOffset = 10;
            this.ArrowSize = 10;
            this.BaseColor = Colors.Gray;
            this.HighlightColor = Colors.Red;
            //this.LabelText = string.Empty;
            //this.LabelColor = Colors.Gray;
        }

        public void ReadyControl()
        {
            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            double controlPointOffset = dist / 2;

            var start = new Point(StartPoint.X + StartOffset, StartPoint.Y);
            var end = new Point(EndPoint.X - EndOffset, EndPoint.Y);

            var pathFigure = new PathFigure();
            var bezierSegment = new BezierSegment();
            pathFigure.StartPoint = start;
            bezierSegment.Point3 = end;
            bezierSegment.Point1 = new Point(start.X + controlPointOffset, start.Y);
            bezierSegment.Point2 = new Point(end.X - controlPointOffset, end.Y);
            pathFigure.Segments.Add(bezierSegment);

            this.HeadShape.Points.Clear();
            this.HeadShape.Points.Add(new Point(end.X - ArrowSize, end.Y - 0.5 * ArrowSize));
            this.HeadShape.Points.Add(new Point(end.X, end.Y));
            this.HeadShape.Points.Add(new Point(end.X - ArrowSize, end.Y + 0.5 * ArrowSize));

            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            this.BodayShape.Data = pathGeometry;

            this.HeadShape.StrokeThickness = 3;
            this.BodayShape.StrokeThickness = 3;

            var solidColorBrush = new SolidColorBrush(color: this.BaseColor);
            this.BodayShape.Stroke = solidColorBrush;
            this.BodayShape.Opacity = 0.8;
            this.HeadShape.Stroke = solidColorBrush;

            //_label.Text = LabelText;
            //_label.FontSize = 9;
            //_label.Foreground = new SolidColorBrush(LabelColor);
            //Point labelPos = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            //Canvas.SetLeft(_label, labelPos.X);
            //Canvas.SetTop(_label, labelPos.Y);

            var colorAnimation = new ColorAnimation
            {
                From = Colors.Transparent,
                To = HighlightColor,
                AutoReverse = true,
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                RepeatBehavior = RepeatBehavior.Forever
            };

            this.Storyboard.Children.Add(colorAnimation);
            Storyboard.SetTarget(element: colorAnimation, value: solidColorBrush);
            Storyboard.SetTargetProperty(element: colorAnimation, path: new PropertyPath("Color"));
        }

        public void DoAlert()
        {
            this.Storyboard.Begin();
        }

        public void CancelAlert()
        {
            this.Storyboard.Stop();
        }
    }
}
