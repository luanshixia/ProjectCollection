using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using System.Windows.Controls.DataVisualization.Charting;

namespace DesktopClient
{
    public static class ChartHelper
    {
        public static StackedChart GetMyChart(string title, params DataSeries[] series)
        {
            StackedChart chart = new StackedChart { Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 10, ShadowDepth = 3 }, Title = title, Margin = new Thickness(5, 5, 5, 20) };
            StackedDataSeries stacked = new StackedDataSeries();
            chart.XLabels = series[0].Data.Keys.ToList();
            chart.YValues = new List<DataSeries>();
            chart.YValues.Add(stacked);
            foreach (var s in series)
            {
                if (s is ColumnDataSeries)
                {
                    stacked.Parts.Add(s as ColumnDataSeries);
                }
                else
                {
                    chart.YValues.Add(s);
                }
            }
            chart.ReadyControl();
            return chart;
        }

        public static ColumnDataSeries MyColumnSeries(string title, KeyValuePair<string, double>[] data, Brush brush)
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();
            data.ToList().ForEach(x => dict.Add(x.Key, x.Value));
            return new ColumnDataSeries { Data = dict, Title = title, Background = brush };
        }

        public static LineDataSeries MyLineSeries(string title, KeyValuePair<string, double>[] data, Brush brush)
        {
            Dictionary<string, double> dict = new Dictionary<string, double>();
            data.ToList().ForEach(x => dict.Add(x.Key, x.Value));
            return new LineDataSeries { Data = dict, Title = title, Background = brush };
        }

        public static Chart GetChart(string title, params Series[] series)
        {
            Chart chart = new Chart { BorderBrush = Brushes.Silver, Background = Brushes.White, Effect = new System.Windows.Media.Effects.DropShadowEffect { BlurRadius = 10, ShadowDepth = 3 }, Width = 600, Height = 400, Margin = new Thickness(5, 5, 5, 20) };
            chart.TitleStyle = new Style(typeof(Control));
            chart.TitleStyle.Setters.Add(new Setter(Control.FontSizeProperty, 12.0));
            chart.TitleStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.Bold));
            chart.TitleStyle.Setters.Add(new Setter(Control.HorizontalAlignmentProperty, HorizontalAlignment.Center));
            chart.TitleStyle.Setters.Add(new Setter(Control.MarginProperty, new Thickness(10)));
            //chart.LegendStyle = new Style(typeof(Control));
            //chart.LegendStyle.Setters.Add(new Setter(Control.FontSizeProperty, 10.0));
            foreach (var s in series)
            {
                chart.Series.Add(s);
            }
            return chart;
        }

        public static ColumnSeries ColumnSeries(string title, object[] data, Brush brush)
        {
            return ColumnSeries(title, data, "Key", "Value", brush);
        }

        public static ColumnSeries ColumnSeries(string title, IEnumerable source, string xMemberPath, string yMemberPath, Brush brush)
        {
            Style style = new Style(typeof(Control));
            style.Setters.Add(new Setter(Control.BackgroundProperty, brush));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, brush));
            return new ColumnSeries { Title = title, IndependentValueBinding = new Binding(xMemberPath), DependentValueBinding = new Binding(yMemberPath), DataPointStyle = style, ItemsSource = source };
        }

        public static LineSeries LineSeries(string title, object[] data, Brush brush)
        {
            Style style = new Style(typeof(Control));
            style.Setters.Add(new Setter(Control.BackgroundProperty, brush));
            style.Setters.Add(new Setter(Control.BorderBrushProperty, brush));
            return new LineSeries { Title = title, IndependentValueBinding = new Binding("Key"), DependentValueBinding = new Binding("Value"), DataPointStyle = style, ItemsSource = data };
        }
    }
}
