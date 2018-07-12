using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleFlow
{
    public class BezierLink : Canvas
    {
        public Guid FromNodeID
        {
            get
            {
                var (from, _) = ((Guid, Guid))this.Tag;
                return from;
            }
            set
            {
                var (_, to) = ((Guid, Guid))this.Tag;
                this.Tag = (value, to);
            }
        }

        public Guid ToNodeID
        {
            get
            {
                var (_, to) = ((Guid, Guid))this.Tag;
                return to;
            }
            set
            {
                var (from, _) = ((Guid, Guid))this.Tag;
                this.Tag = (from, value);
            }
        }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public double StartOffset { get; set; }
        public double EndOffset { get; set; }
        public double ArrowSize { get; set; }
        public Color BaseColor { get; set; }
        public Color HighlightColor { get; set; }

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
