using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections;

namespace DesktopClient
{
    /// <summary>
    /// StackedChart.xaml 的交互逻辑
    /// </summary>
    public partial class StackedChart : UserControl
    {
        public List<string> XLabels { get; set; }
        public List<DataSeries> YValues { get; set; }
        //public List<LineDataSeries> Lines { get; set; }
        //public List<StackedDataSeries> Stacks { get; set; }

        private double _margin = 60;
        private double _marginBottom = 40;
        private double _maxValue;
        private int _xLabelsCount = 5;
        private CanvasHelper _helper;

        public double ChartAreaWidth { get { return ChartArea.ActualWidth; } }
        public double ChartAreaHeight { get { return ChartArea.ActualHeight; } }
        public double ChartGridWidth { get { return ChartArea.ActualWidth - _margin; } }
        public double ChartGridHeight { get { return ChartArea.ActualHeight - _marginBottom; } }

        public string Title
        {
            get
            {
                return txtTitle.Text;
            }
            set
            {
                txtTitle.Text = value;
            }
        }

        public StackedChart()
        {
            InitializeComponent();

            this.Measure(this.RenderSize);
            this.Arrange(new Rect(this.RenderSize));

            DefaultData();
            ReadyControl();
        }

        public void ReadyControl()
        {
            ChartArea.Children.Clear();
            Legend.Children.Clear();

            _maxValue = YValues.Max(x => x.Max) > 0 ? CanvasHelper.GetGridLineMax(YValues.Max(x => x.Max)) : 10;
            _xLabelsCount = XLabels.Count > 0 ? XLabels.Count : 1;
            _helper = new CanvasHelper(new Point(0, 0), new Point(_margin, ChartGridHeight), new Point(_xLabelsCount, _maxValue), new Point(ChartAreaWidth, 0));

            DrawBackground();
            DrawGridLines();
            DrawDataPoint();
            DrawXAxis();
            DrawYAxis();
            DrawLegend();
        }

        private void DefaultData()
        {
            XLabels = new List<string> { "2011", "2012", "2013", "2014", "2015" };

            var data1 = new Dictionary<string, double> { { "2011", 135 }, { "2012", 143 }, { "2013", 196 }, { "2014", 203 }, { "2015", 210 } };
            var data2 = new Dictionary<string, double> { { "2011", 96 }, { "2012", 101 }, { "2013", 133 }, { "2014", 150 }, { "2015", 180 } };
            var data3 = new Dictionary<string, double> { { "2011", 221 }, { "2012", 256 }, { "2013", 334 }, { "2014", 409 }, { "2015", 523 } };

            ColumnDataSeries series1 = new ColumnDataSeries { Data = data1, Title = "DATA1", Background = BuildGradient(Colors.YellowGreen) };
            ColumnDataSeries series2 = new ColumnDataSeries { Data = data2, Title = "DATA2", Background = BuildGradient(Colors.Yellow) };
            LineDataSeries series3 = new LineDataSeries { Data = data3, Title = "DATA3", Background = Brushes.Red };
            StackedDataSeries series12 = new StackedDataSeries { Parts = new List<ColumnDataSeries> { series1, series2 } };

            YValues = new List<DataSeries> { series12, series3 };
        }

        private void DrawBackground()
        {
            Rectangle rect = new Rectangle { Width = ChartGridWidth, Height = ChartGridHeight, Stroke = Brushes.Gray, StrokeThickness = 1, Fill = FindResource("gradient") as Brush };
            Canvas.SetLeft(rect, _margin);
            Canvas.SetTop(rect, 0);
            ChartArea.Children.Add(rect);
        }

        private void DrawGridLines()
        {
            double delta = _maxValue / 10;
            Enumerable.Range(1, 9).ToList().ForEach(i =>
            {
                var p1 = _helper.CanvasPoint(new Point(0, i * delta));
                var p2 = _helper.CanvasPoint(new Point(_xLabelsCount, i * delta));
                Line ln = new Line { X1 = p1.X, X2 = p2.X, Y1 = p1.Y, Y2 = p2.Y, Stroke = Brushes.Gray, StrokeThickness = 0.5 };
                ChartArea.Children.Add(ln);
            });
        }

        private void DrawXAxis()
        {
            Line ln = new Line { X1 = _margin, X2 = ChartAreaWidth, Y1 = ChartGridHeight, Y2 = ChartGridHeight, Stroke = Brushes.Black, StrokeThickness = 1 };
            ChartArea.Children.Add(ln);
            Enumerable.Range(0, _xLabelsCount).ToList().ForEach(i =>
            {
                var p = _helper.CanvasPoint(new Point(i, 0));
                Line ln1 = new Line { X1 = p.X, X2 = p.X, Y1 = p.Y, Y2 = p.Y + 5, Stroke = Brushes.Black, StrokeThickness = 1 };
                ChartArea.Children.Add(ln1);

                TextBlock tb = new TextBlock { Text = XLabels[i] };
                Canvas.SetLeft(tb, p.X);
                Canvas.SetTop(tb, p.Y + 5);
                ChartArea.Children.Add(tb);
            });
        }

        private void DrawYAxis()
        {
            Line ln = new Line { X1 = _margin, X2 = _margin, Y1 = ChartGridHeight, Y2 = 0, Stroke = Brushes.Black, StrokeThickness = 1 };
            ChartArea.Children.Add(ln);
            double delta = _maxValue / 10;
            Enumerable.Range(0, 11).ToList().ForEach(i =>
            {
                var p = _helper.CanvasPoint(new Point(0, i * delta));
                Line ln1 = new Line { X1 = p.X, X2 = p.X - 5, Y1 = p.Y, Y2 = p.Y, Stroke = Brushes.Black, StrokeThickness = 1 };
                ChartArea.Children.Add(ln1);

                TextBlock tb = new TextBlock { Text = (i * delta).ToString("0.##") };
                Canvas.SetRight(tb, ChartGridWidth + 5);
                Canvas.SetTop(tb, p.Y - 8);
                ChartArea.Children.Add(tb);
            });
        }

        private void DrawDataPoint()
        {
            foreach (var series in YValues)
            {
                if (series is StackedDataSeries)
                {
                    var stacked = series as StackedDataSeries;
                    foreach (var part in stacked.Parts)
                    {
                        int partIndex = stacked.Parts.IndexOf(part);
                        int i = 0;
                        foreach (var item in part.Data)
                        {
                            Rectangle rect = new Rectangle { Width = ChartGridWidth / _xLabelsCount / 2.5, Height = _helper.CanvasHeight(item.Value), Fill = part.Background, ToolTip = item.Value, Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 3, ShadowDepth = 2, Opacity = 0.5 } };
                            var p = _helper.CanvasPoint(new Point(i + 0.3, Enumerable.Range(0, partIndex + 1).Sum(x => stacked.Parts[x].Data.ElementAt(i).Value)));
                            Canvas.SetLeft(rect, p.X);
                            Canvas.SetTop(rect, p.Y);
                            ChartArea.Children.Add(rect);
                            i++;
                        }
                    }
                }
                else if (series is LineDataSeries)
                {
                    Polyline poly = new Polyline { Stroke = series.Background, StrokeThickness = 2, Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 5, ShadowDepth = 2, Opacity = 0.8 } };
                    ChartArea.Children.Add(poly);
                    int i = 0;
                    foreach (var item in series.Data)
                    {
                        var p = _helper.CanvasPoint(new Point(i + 0.5, item.Value));

                        Ellipse circle = new Ellipse { Width = 10, Height = 10, Fill = series.Background, ToolTip = item.Value, Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 3, ShadowDepth = 2, Opacity = 0.5 } };
                        Canvas.SetLeft(circle, p.X - 5);
                        Canvas.SetTop(circle, p.Y - 5);
                        ChartArea.Children.Add(circle);

                        poly.Points.Add(p);
                        i++;
                    }
                }
            }
        }

        private void DrawLegend()
        {
            foreach (var series in YValues)
            {
                if (series is StackedDataSeries)
                {
                    var stacked = series as StackedDataSeries;
                    foreach (var part in stacked.Parts)
                    {
                        StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
                        Border colorBlock = new Border { Width = 10, Height = 10, Background = part.Background, Margin = new Thickness(3) };
                        TextBlock label = new TextBlock { Text = part.Title, FontSize = 10, TextWrapping = TextWrapping.Wrap };
                        sp.Children.Add(colorBlock);
                        sp.Children.Add(label);
                        Legend.Children.Add(sp);
                    }
                }
                else if (series is LineDataSeries)
                {
                    StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal };
                    Border colorBlock = new Border { Width = 10, Height = 10, Background = series.Background, Margin = new Thickness(3) };
                    TextBlock label = new TextBlock { Text = series.Title, FontSize = 10, TextWrapping = TextWrapping.Wrap };
                    sp.Children.Add(colorBlock);
                    sp.Children.Add(label);
                    Legend.Children.Add(sp);
                }
            }
        }

        public static Brush BuildGradient(Color baseColor)
        {
            return new LinearGradientBrush(Colors.White, baseColor, new Point(-1, 0), new Point(1, 0));
        }
    }

    public class DataSeries
    {
        //public IEnumerable ItemsSource { get; set; }
        //public string XBinding { get; set; }
        //public string YBinding { get; set; }
        public Dictionary<string, double> Data { get; set; }
        public string Title { get; set; }
        public Brush Background { get; set; }

        public virtual double Max { get { return Data.Max(x => x.Value); } }
        public virtual double Min { get { return Data.Min(x => x.Value); } }
    }

    public class LineDataSeries : DataSeries
    {
    }

    public class ColumnDataSeries : DataSeries
    {
    }

    public class StackedDataSeries : DataSeries
    {
        public List<ColumnDataSeries> Parts { get; set; }

        public override double Max { get { return Parts.Sum(x => x.Max); } }
        public override double Min { get { return Parts.Sum(x => x.Min); } }

        public StackedDataSeries()
        {
            Parts = new List<ColumnDataSeries>();
        }
    }

    public class CanvasHelper
    {
        private Point _p1w;
        private Point _p1c;
        private Point _p2w;
        private Point _p2c;

        public CanvasHelper(Point p1w, Point p1c, Point p2w, Point p2c)
        {
            _p1w = p1w;
            _p1c = p1c;
            _p2w = p2w;
            _p2c = p2c;
        }

        public Point WorldPoint(Point canvasPoint)
        {
            double x = (canvasPoint.X - _p1c.X) / (_p2c.X - _p1c.X) * (_p2w.X - _p1w.X) + _p1w.X;
            double y = (canvasPoint.Y - _p1c.Y) / (_p2c.Y - _p1c.Y) * (_p2w.Y - _p1w.Y) + _p1w.Y;
            return new Point(x, y);
        }

        public Point CanvasPoint(Point worldPoint)
        {
            double x = (worldPoint.X - _p1w.X) / (_p2w.X - _p1w.X) * (_p2c.X - _p1c.X) + _p1c.X;
            double y = (worldPoint.Y - _p1w.Y) / (_p2w.Y - _p1w.Y) * (_p2c.Y - _p1c.Y) + _p1c.Y;
            return new Point(x, y);
        }

        public double CanvasHeight(double worldHeight)
        {
            return Math.Abs((_p2c.Y - _p1c.Y) / (_p2w.Y - _p1w.Y) * worldHeight);
        }

        public static double GetGridLineMax(double maxValue)
        {
            //----- Python test code ------
            //def f(maxValue):
            //    powerOf10 = Math.Pow(10, Math.Floor(Math.Log10(maxValue)) - 1);
            //    return Math.Ceiling(maxValue / powerOf10) * powerOf10 / 10;

            double powerOf10 = Math.Pow(10, Math.Floor(Math.Log10(maxValue)) - 1);
            return Math.Ceiling(maxValue / powerOf10) * powerOf10;
        }
    }
}
