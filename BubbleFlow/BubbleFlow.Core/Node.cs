using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleFlow
{
    public class NodeBubble : Canvas
    {
        public Guid NodeID
        {
            get => (Guid)this.Tag;
            set => this.Tag = value;
        }

        public string Text { get; set; }
        public double FontSize { get; set; }
        public double Size { get; set; }
        public Color FillColor { get; set; }
        public Color StrokeColor { get; set; }
        public Color FontColor { get; set; }
        public Point Position { get; set; }
        //public string OnClick { get; set; }
        public bool NeedAlert { get; set; }
        //public string LabelText { get; set; } // 可用于显示流程角色 newly 20121029

        private Ellipse RoundShape { get; } = new Ellipse();
        private TextBlock TextLabel { get; } = new TextBlock();
        private Storyboard Storyboard { get; } = new Storyboard();

        public NodeBubble()
        {
            this.Children.Add(RoundShape);
            this.Children.Add(TextLabel);

            this.Cursor = Cursors.Hand;
            this.MouseEnter += new MouseEventHandler(_shape_MouseEnter);
            this.MouseLeave += new MouseEventHandler(_shape_MouseLeave);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(_shape_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(_shape_MouseLeftButtonUp);

            this.DefaultValue();
            this.ReadyControl();
        }

        public bool IsPointInNode(Point pt)
        {
            return (Position.X - pt.X) * (Position.X - pt.X) + (Position.Y - pt.Y) * (Position.Y - pt.Y) <= (Size / 2) * (Size / 2);
        }

        public void SetColor(Color color)
        {
            this.FillColor = color;
            this.ReadyControl();
        }

        void _shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(this.RoundShape, Canvas.GetLeft(this.RoundShape) - 1);
            Canvas.SetTop(this.RoundShape, Canvas.GetTop(this.RoundShape) - 1);
            //if (!string.IsNullOrEmpty(OnClick))
            //{
            //    // System.Windows.Browser.HtmlPage.Window.Eval(OnClick);
            //}
        }

        void _shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(this.RoundShape, Canvas.GetLeft(this.RoundShape) + 1);
            Canvas.SetTop(this.RoundShape, Canvas.GetTop(this.RoundShape) + 1);
        }

        void _shape_MouseLeave(object sender, MouseEventArgs e)
        {
            this.RoundShape.Opacity = 1;
        }

        void _shape_MouseEnter(object sender, MouseEventArgs e)
        {
            this.RoundShape.Opacity = 0.6;
        }

        private void DefaultValue()
        {
            this.Text = "Node";
            this.FontSize = 12;
            this.Size = 100;
            this.FillColor = Colors.DarkGray;
            this.StrokeColor = Colors.Gray;
            this.FontColor = Colors.Black;
            this.Position = new Point();
        }

        public void ReadyControl()
        {
            this.RoundShape.Width = Size;
            this.RoundShape.Height = Size;
            Canvas.SetLeft(this.RoundShape, Position.X - Size / 2);
            Canvas.SetTop(this.RoundShape, Position.Y - Size / 2);
            this.RoundShape.Stroke = new SolidColorBrush(StrokeColor);
            this.RoundShape.StrokeThickness = 1;
            this.RoundShape.Fill = BuildFill(FillColor);

            this.TextLabel.Text = WrapText(Text);
            this.TextLabel.FontSize = FontSize;
            this.TextLabel.Foreground = new SolidColorBrush(FontColor);
            this.TextLabel.TextAlignment = TextAlignment.Center;
            this.Measure(this.DesiredSize);
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            Canvas.SetLeft(this.TextLabel, Position.X - TextLabel.ActualWidth / 2);
            Canvas.SetTop(this.TextLabel, Position.Y - TextLabel.ActualHeight / 2);

            //if (Children.Any(x => x is Border))
            //{
            //    Children.Remove(Children.First(x => x is Border));
            //}
            //if (!string.IsNullOrEmpty(LabelText))
            //{
            //    // 绘制标签，可用于显示流程角色。
            //    Border b = new Border { CornerRadius = new CornerRadius(10), Height = 20, Width = 80, Background = new SolidColorBrush(Colors.DarkGray) };

            //}

            if (this.NeedAlert)
            {
                var doubleAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    AutoReverse = true,
                    Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                this.Storyboard.Children.Add(doubleAnimation);
                Storyboard.SetTarget(doubleAnimation, RoundShape);
                Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity"));
                this.Storyboard.Begin();
            }
            else
            {
                this.Storyboard.Stop();
            }
        }

        public void SetText(string text)
        {
            this.Text = text;
            this.TextLabel.Text = NodeBubble.WrapText(this.Text);
            this.TextLabel.FontSize = this.FontSize;
            this.TextLabel.Foreground = new SolidColorBrush(this.FontColor);
            this.TextLabel.TextAlignment = TextAlignment.Center;
            Canvas.SetLeft(this.TextLabel, Position.X - TextLabel.ActualWidth / 2);
            Canvas.SetTop(this.TextLabel, Position.Y - TextLabel.ActualHeight / 2);
        }

        private static string WrapText(string text)
        {
            int lineLength = 6;
            var positions = new List<int>();
            for (int pos = lineLength; pos < text.Length; pos += lineLength)
            {
                positions.Add(pos);
            }

            positions.Reverse();
            positions.ForEach(pos => text = text.Insert(pos, "\n"));
            return text;
        }

        private static RadialGradientBrush BuildFill(Color color)
        {
            var color0 = color;
            color0.A = 128;
            return new RadialGradientBrush(color0, color)
            {
                Center = new Point(0.5, 0.1),
                RadiusX = 0.8,
                RadiusY = 0.8
            };
        }

        public void Fade(bool fade)
        {
            if (fade)
            {
                var b = (byte)((FillColor.R + FillColor.G + FillColor.B) / 3);
                var c = new Color { A = 255, R = b, G = b, B = b };
                this.RoundShape.Fill = NodeBubble.BuildFill(c);
            }
            else
            {
                this.RoundShape.Fill = NodeBubble.BuildFill(FillColor);
            }
        }
    }
}
