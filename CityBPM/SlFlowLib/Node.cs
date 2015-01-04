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

using System.Linq;
using System.Collections.Generic;

namespace SlFlowLib
{
    public class Node : Canvas
    {
        public string Text { get; set; }
        public double FontSize { get; set; }
        public double Size { get; set; }
        public Color FillColor { get; set; }
        public Color StrokeColor { get; set; }
        public Color FontColor { get; set; }
        public Point Position { get; set; }
        public string OnClick { get; set; }
        public bool NeedAlert { get; set; }
        public string LabelText { get; set; } // 可用于显示流程角色 newly 20121029

        private Ellipse _shape = new Ellipse();
        private TextBlock _text = new TextBlock();
        private Storyboard _sb = new Storyboard();

        public Node()
        {
            this.Children.Add(_shape);
            this.Children.Add(_text);

            this.Cursor = Cursors.Hand;
            this.MouseEnter += new MouseEventHandler(_shape_MouseEnter);
            this.MouseLeave += new MouseEventHandler(_shape_MouseLeave);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(_shape_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(_shape_MouseLeftButtonUp);

            DefaultValue();
            ReadyControl();
        }

        public bool IsPointInNode(Point pt)
        {
            if ((Position.X - pt.X) * (Position.X - pt.X) + (Position.Y - pt.Y) * (Position.Y - pt.Y) <= (Size / 2) * (Size / 2))
            {
                return true;
            }
            return false;
        }

        public void SetColor(Color color)
        {
            FillColor = color;
            ReadyControl();
        }

        void _shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(_shape, Canvas.GetLeft(_shape) - 1);
            Canvas.SetTop(_shape, Canvas.GetTop(_shape) - 1);
            if (!string.IsNullOrEmpty(OnClick))
            {
                System.Windows.Browser.HtmlPage.Window.Eval(OnClick);
            }
        }

        void _shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.SetLeft(_shape, Canvas.GetLeft(_shape) + 1);
            Canvas.SetTop(_shape, Canvas.GetTop(_shape) + 1);
        }

        void _shape_MouseLeave(object sender, MouseEventArgs e)
        {
            _shape.Opacity = 1;
        }

        void _shape_MouseEnter(object sender, MouseEventArgs e)
        {
            _shape.Opacity = 0.6;
        }

        private void DefaultValue()
        {
            Text = "结点";
            FontSize = 12;
            Size = 100;
            FillColor = Colors.DarkGray;
            StrokeColor = Colors.Gray;
            FontColor = Colors.Black;
            Position = new Point();
            OnClick = string.Empty;
        }

        public void ReadyControl()
        {
            _shape.Width = Size;
            _shape.Height = Size;
            Canvas.SetLeft(_shape, Position.X - Size / 2);
            Canvas.SetTop(_shape, Position.Y - Size / 2);
            _shape.Stroke = new SolidColorBrush(StrokeColor);
            _shape.StrokeThickness = 1;
            _shape.Fill = BuildFill(FillColor);

            _text.Text = WrapText(Text);
            _text.FontSize = FontSize;
            _text.Foreground = new SolidColorBrush(FontColor);
            _text.TextAlignment = TextAlignment.Center;
            Canvas.SetLeft(_text, Position.X - _text.ActualWidth / 2);
            Canvas.SetTop(_text, Position.Y - _text.ActualHeight / 2);

            //if (Children.Any(x => x is Border))
            //{
            //    Children.Remove(Children.First(x => x is Border));
            //}
            //if (!string.IsNullOrEmpty(LabelText))
            //{
            //    // 绘制标签，可用于显示流程角色。
            //    Border b = new Border { CornerRadius = new CornerRadius(10), Height = 20, Width = 80, Background = new SolidColorBrush(Colors.DarkGray) };

            //}

            if (NeedAlert)
            {
                DoubleAnimation da = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    AutoReverse = true,
                    Duration = new Duration(TimeSpan.Parse("0:0:0.5")),
                    RepeatBehavior = RepeatBehavior.Forever
                };
                _sb.Children.Add(da);
                Storyboard.SetTarget(da, _shape);
                Storyboard.SetTargetProperty(da, new PropertyPath("Opacity"));
                _sb.Begin();
            }
            else
            {
                _sb.Stop();
            }
        }

        public void SetText(string text)
        {
            Text = text;
            _text.Text = WrapText(Text);
            _text.FontSize = FontSize;
            _text.Foreground = new SolidColorBrush(FontColor);
            _text.TextAlignment = TextAlignment.Center;
            Canvas.SetLeft(_text, Position.X - _text.ActualWidth / 2);
            Canvas.SetTop(_text, Position.Y - _text.ActualHeight / 2);
        }

        private string WrapText(string text)
        {
            int lineLength = 6;

            List<int> positions = new List<int>();
            for (int pos = lineLength; pos < text.Length; pos += lineLength)
            {
                positions.Add(pos);
            }
            positions.Reverse();
            positions.ForEach(pos => text = text.Insert(pos, "\n"));
            return text;
        }

        private RadialGradientBrush BuildFill(Color color)
        {
            Color color0 = color;
            color0.A = 128;
            RadialGradientBrush rgb = new RadialGradientBrush(color0, color);
            rgb.Center = new Point(0.5, 0.1);
            rgb.RadiusX = 0.8;
            rgb.RadiusY = 0.8;
            return rgb;
        }

        public void Fade(bool fade)
        {
            if (fade)
            {
                byte b = (byte)((FillColor.R + FillColor.G + FillColor.B) / 3);
                Color c = new Color { A = 255, R = b, G = b, B = b };
                _shape.Fill = BuildFill(c);
            }
            else
            {
                _shape.Fill = BuildFill(FillColor);
            }
        }
    }
}
