﻿using BubbleMind.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleMind.Components
{
    public class NodeBubble : Canvas
    {
        public Guid NodeID
        {
            get => (Guid)this.Tag;
            set => this.Tag = value;
        }

        public string Text { get; set; }
        public double Size { get; set; } = 100;
        public double FontSize { get; set; } = 12;
        public Color FontColor { get; set; } = Colors.Black;
        public Color FillColor { get; set; } = Colors.DarkGray;
        public Color StrokeColor { get; set; } = Colors.Gray;
        public Point Position { get; set; }
        public bool Flashing { get; set; }

        private readonly Ellipse RoundShape = new Ellipse();
        private readonly TextBlock TextLabel = new TextBlock();
        private readonly Storyboard Storyboard = new Storyboard();

        public NodeBubble()
        {
            this.Children.Add(RoundShape);
            this.Children.Add(TextLabel);

            this.Cursor = Cursors.Hand;
            this.MouseEnter += (sender, e) => this.RoundShape.Opacity = 0.6;
            this.MouseLeave += (sender, e) => this.RoundShape.Opacity = 1.0;
            this.MouseLeftButtonDown += (sender, e) => this.RenderTransform = new TranslateTransform(1, 1);
            this.MouseLeftButtonUp += (sender, e) => this.RenderTransform = null;

            this.ReadyControl();
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
            return new RadialGradientBrush(
                startColor: color.With(c => c.A = 128),
                endColor: color)
            {
                Center = new Point(0.5, 0.1),
                RadiusX = 0.8,
                RadiusY = 0.8
            };
        }
    }
}
