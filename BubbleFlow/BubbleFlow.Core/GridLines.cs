using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BubbleFlow
{
    public class GridLines : Canvas
    {
        public double Radius { get; set; } = 10000;
        public double Interval { get; set; } = 25;

        private readonly List<Line> Lines = new List<Line>();

        public void ReadyControl()
        {
            this.Lines.ForEach(line => this.Children.Remove(line));
            this.Lines.Clear();

            for (var x = -this.Radius; x <= this.Radius; x += this.Interval)
            {
                var line = new Line
                {
                    X1 = x,
                    Y1 = -this.Radius,
                    X2 = x,
                    Y2 = this.Radius,
                    Stroke = new SolidColorBrush(x == 0 ? Colors.DarkGray : Colors.LightGray),
                    StrokeThickness = x == 0 ? 2 : 1,
                    //SnapsToDevicePixels = true
                };

                this.Lines.Add(line);
                this.Children.Add(line);

                line = new Line
                {
                    X1 = -this.Radius,
                    Y1 = x,
                    X2 = this.Radius,
                    Y2 = x,
                    Stroke = new SolidColorBrush(x == 0 ? Colors.DarkGray : Colors.LightGray),
                    StrokeThickness = x == 0 ? 2 : 1,
                    //SnapsToDevicePixels = true
                };

                this.Lines.Add(line);
                this.Children.Add(line);
            }
        }

        public Point Snap(Point position)
        {
            return new Point(
                Math.Truncate(position.X / this.Interval) * this.Interval,
                Math.Truncate(position.Y / this.Interval) * this.Interval);
        }
    }
}
