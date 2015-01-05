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

namespace CityInfographics
{
    /// <summary>
    /// SlidersPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SlidersPanel : UserControl
    {
        public SlidersPanel()
        {
            InitializeComponent();
        }

        public event Action<TimePoint> ReportResult;
        protected void OnReportResult(TimePoint result)
        {
            if (ReportResult != null)
            {
                ReportResult(result);
            }
        }

        private void Update()
        {
            if (MonthSlider != null && DaySlider != null && HourSlider != null)
            {
                OnReportResult(new TimePoint((int)MonthSlider.Value, (int)DaySlider.Value, (int)HourSlider.Value));
            }
        }

        private void MonthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Update();
        }

        private void DaySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Update();
        }

        private void HourSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Update();
        }
    }

    public class TimePoint
    {
        public int Month { get; private set; }
        public int Day { get; private set; }
        public int Hour { get; private set; }

        public TimePoint(int month, int day, int hour)
        {
            Month = month;
            Day = day;
            Hour = hour;
        }
    }
}
