using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SlFlowLib
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

        private Line _body = new Line();
        private Polyline _head = new Polyline();
        private Storyboard _sb = new Storyboard();

        public Arrow()
        {
            this.Children.Add(_body);
            this.Children.Add(_head);

            DefaultValue();
            ReadyControl();
        }

        private void DefaultValue()
        {
            StartPoint = new Point();
            EndPoint = new Point(100, 0);
            StartOffset = 10;
            EndOffset = 10;
            ArrowSize = 10;
            BaseColor = Colors.Gray;
            HighlightColor = Colors.Red;
        }

        public void ReadyControl()
        {
            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            double r1 = StartOffset / dist;
            double r2 = EndOffset / dist;

            _body.X1 = r1 * dist;
            _body.Y1 = 0;
            _body.X2 = dist - r2 * dist;
            _body.Y2 = 0;

            _head.Points.Clear();
            _head.Points.Add(new Point(_body.X2 - ArrowSize, _body.Y2 - 0.5 * ArrowSize));
            _head.Points.Add(new Point(_body.X2, _body.Y2));
            _head.Points.Add(new Point(_body.X2 - ArrowSize, _body.Y2 + 0.5 * ArrowSize));

            TransformGroup transform = new TransformGroup();
            RotateTransform rotate = new RotateTransform { Angle = Math.Atan2(dy, dx) * 180 / Math.PI };
            TranslateTransform translate = new TranslateTransform { X = StartPoint.X, Y = StartPoint.Y };
            transform.Children.Add(rotate);
            transform.Children.Add(translate);
            _body.RenderTransform = transform;
            _head.RenderTransform = transform;

            _body.StrokeThickness = 3;
            _head.StrokeThickness = 3;
            SolidColorBrush scb = new SolidColorBrush(BaseColor);
            _body.Stroke = scb;
            _head.Stroke = scb;

            ColorAnimation ca = new ColorAnimation
            {
                From = Colors.Transparent,
                To = HighlightColor,
                AutoReverse = true,
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                RepeatBehavior = RepeatBehavior.Forever
            };
            _sb.Children.Add(ca);
            Storyboard.SetTarget(ca, scb);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Color"));
        }

        public void DoAlert()
        {
            _sb.Begin();
        }

        public void CancelAlert()
        {
            _sb.Stop();
        }
    }

    public enum LineMethod
    {
        Linear,
        Bezier
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

        private Path _body = new Path();
        private TextBlock _label = new TextBlock();
        private Storyboard _sb = new Storyboard();
        private Polyline _head = new Polyline();

        public BezierLink()
        {
            this.Children.Add(_body);
            this.Children.Add(_head);
            this.Children.Add(_label);

            DefaultValue();
            ReadyControl();
        }

        private void DefaultValue()
        {
            StartPoint = new Point();
            EndPoint = new Point(100, 0);
            StartOffset = 10;
            EndOffset = 10;
            ArrowSize = 10;
            BaseColor = Colors.Gray;
            HighlightColor = Colors.Red;
            //LabelText = string.Empty;
            //LabelColor = Colors.Gray;
        }

        public void ReadyControl()
        {
            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double dist = Math.Sqrt(dx * dx + dy * dy);
            double cpOffset = dist / 2;
            double r1 = StartOffset / dist;
            double r2 = EndOffset / dist;

            Point start = new Point(StartPoint.X + StartOffset, StartPoint.Y);
            Point end = new Point(EndPoint.X - EndOffset, EndPoint.Y);

            PathFigure pf = new PathFigure();
            BezierSegment bs = new BezierSegment();
            pf.StartPoint = start;
            bs.Point3 = end;
            bs.Point1 = new Point(start.X + cpOffset, start.Y);
            bs.Point2 = new Point(end.X - cpOffset, end.Y);
            pf.Segments.Add(bs);

            _head.Points.Clear();
            _head.Points.Add(new Point(end.X - ArrowSize, end.Y - 0.5 * ArrowSize));
            _head.Points.Add(new Point(end.X, end.Y));
            _head.Points.Add(new Point(end.X - ArrowSize, end.Y + 0.5 * ArrowSize));

            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);
            _body.Data = pg;

            _head.StrokeThickness = 3;
            _body.StrokeThickness = 3;
            SolidColorBrush scb = new SolidColorBrush(BaseColor);
            _body.Stroke = scb;
            _body.Opacity = 0.8;
            _head.Stroke = scb;

            //_label.Text = LabelText;
            //_label.FontSize = 9;
            //_label.Foreground = new SolidColorBrush(LabelColor);
            //Point labelPos = new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2);
            //Canvas.SetLeft(_label, labelPos.X);
            //Canvas.SetTop(_label, labelPos.Y);

            ColorAnimation ca = new ColorAnimation
            {
                From = Colors.Transparent,
                To = HighlightColor,
                AutoReverse = true,
                Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                RepeatBehavior = RepeatBehavior.Forever
            };
            _sb.Children.Add(ca);
            Storyboard.SetTarget(ca, scb);
            Storyboard.SetTargetProperty(ca, new PropertyPath("Color"));
        }

        public void DoAlert()
        {
            _sb.Begin();
        }

        public void CancelAlert()
        {
            _sb.Stop();
        }
    }
}
