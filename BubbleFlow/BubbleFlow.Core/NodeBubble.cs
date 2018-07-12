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
        public bool Flashing { get; set; }

        private Ellipse RoundShape { get; } = new Ellipse();
        private TextBlock TextLabel { get; } = new TextBlock();
        private Storyboard Storyboard { get; } = new Storyboard();

        public NodeBubble()
        {
            this.Children.Add(RoundShape);
            this.Children.Add(TextLabel);

            this.Cursor = Cursors.Hand;
            this.MouseEnter += (sender, e) => this.RoundShape.Opacity = 0.6;
            this.MouseLeave += (sender, e) => this.RoundShape.Opacity = 1.0;
            this.MouseLeftButtonDown += (sender, e) => this.RenderTransform = new TranslateTransform(1, 1);
            this.MouseLeftButtonUp += (sender, e) => this.RenderTransform = null;

            this.DefaultValue();
            this.ReadyControl();
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
            this.RoundShape.Width = this.Size;
            this.RoundShape.Height = this.Size;
            Canvas.SetLeft(this.RoundShape, this.Position.X - this.Size / 2);
            Canvas.SetTop(this.RoundShape, this.Position.Y - this.Size / 2);
            this.RoundShape.Stroke = new SolidColorBrush(this.StrokeColor);
            this.RoundShape.StrokeThickness = 1;
            this.RoundShape.Fill = NodeBubble.BuildFill(this.FillColor);

            this.TextLabel.Text = this.Text;
            this.TextLabel.FontSize = this.FontSize;
            this.TextLabel.Foreground = new SolidColorBrush(this.FontColor);
            this.TextLabel.TextAlignment = TextAlignment.Center;
            this.Measure(this.DesiredSize);
            this.Arrange(new Rect(0, 0, this.DesiredSize.Width, this.DesiredSize.Height));
            Canvas.SetLeft(this.TextLabel, this.Position.X - this.TextLabel.ActualWidth / 2);
            Canvas.SetTop(this.TextLabel, this.Position.Y - this.TextLabel.ActualHeight / 2);

            if (this.Flashing)
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
                Storyboard.SetTarget(element: doubleAnimation, value: this.RoundShape);
                Storyboard.SetTargetProperty(element: doubleAnimation, path: new PropertyPath("Opacity"));
                this.Storyboard.Begin();
            }
            else
            {
                this.Storyboard.Stop();
            }
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
    }
}
