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

        public static double ArrowSize { get; set; } = 10;
        public static Color BaseColor { get; set; } = Colors.Gray;
        public static Color HighlightColor { get; set; } = Colors.Red;

        private readonly Path BodayShape = new Path
        {
            Stroke = new SolidColorBrush(BezierLink.BaseColor),
            StrokeThickness = 3
        };

        private readonly Polyline HeadShape = new Polyline
        {
            Stroke = new SolidColorBrush(BezierLink.BaseColor),
            StrokeThickness = 3,
            Opacity = 0.8
        };

        private readonly TextBlock TextLabel = new TextBlock();
        private readonly Storyboard Storyboard = new Storyboard();

        public BezierLink()
        {
            this.Children.Add(this.BodayShape);
            this.Children.Add(this.HeadShape);
            this.Children.Add(this.TextLabel);

            this.ReadyControl();
        }

        public void ReadyControl()
        {
            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            double controlPointOffset = dist / 2;

            var start = new Point(StartPoint.X + StartOffset, StartPoint.Y);
            var end = new Point(EndPoint.X - EndOffset, EndPoint.Y);

            var pathFigure = new PathFigure
            {
                StartPoint = start
            };

            pathFigure.Segments.Add(new BezierSegment
            {
                Point3 = end,
                Point1 = new Point(start.X + controlPointOffset, start.Y),
                Point2 = new Point(end.X - controlPointOffset, end.Y)
            });

            this.HeadShape.Points.Clear();
            this.HeadShape.Points.Add(new Point(end.X - ArrowSize, end.Y - 0.5 * ArrowSize));
            this.HeadShape.Points.Add(new Point(end.X, end.Y));
            this.HeadShape.Points.Add(new Point(end.X - ArrowSize, end.Y + 0.5 * ArrowSize));

            var pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            this.BodayShape.Data = pathGeometry;

            var solidColorBrush = new SolidColorBrush(color: BezierLink.BaseColor);

            var colorAnimation = new ColorAnimation
            {
                From = Colors.Transparent,
                To = BezierLink.HighlightColor,
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
