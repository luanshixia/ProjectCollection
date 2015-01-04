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

using System.Globalization;

using CIIPP;

namespace DesktopClient
{
    /// <summary>
    /// SummaryPage.xaml 的交互逻辑
    /// </summary>
    public partial class SummaryPage : Page
    {
        public SummaryPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //ProjectList.ItemsSource = DocumentManager.CurrentDocument.Projects;
            this.Update();
        }

        public void Update()
        {
            ProjectGrid.ItemsSource = DocumentManager.CurrentDocument.Projects;
            InfoGroup.DataContext = DocumentManager.CurrentDocument.City;

            ProjectGrid.Columns.Where(x => x.Header.ToString() != "排序").ToList().ForEach(x => x.IsReadOnly = true);
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            //double dpi = 150;
            //double mag = dpi / 96;
            //RenderTargetBitmap bmp = new RenderTargetBitmap(Convert.ToInt32(mag * LayoutRoot.ActualWidth), Convert.ToInt32(mag * LayoutRoot.ActualHeight), dpi, dpi, PixelFormats.Pbgra32);
            //bmp.Render(LayoutRoot);
            //Image img = new Image { Source = bmp };

            VisualBrush vb = new VisualBrush(LayoutRoot) { Stretch = System.Windows.Media.Stretch.None, AlignmentY = AlignmentY.Top };
            Grid grid = new Grid { Background = vb, Width = LayoutRoot.ActualWidth, Height = LayoutRoot.ActualHeight };

            PrintManager.Page = PrintPage.A3_Landscape;
            var doc = PrintManager.GetDocumentFrom(new List<UIElement> { grid });
            var fixedDoc = PrintManager.FlowToFixed(doc);
            //PrintManager.PrintFixedDoc(fixedDoc, "C:\\test.xps");

            PrintPreviewWindow ppw = new PrintPreviewWindow { Owner = MainWindow.Current };
            ppw.SetDocument(fixedDoc);
            ppw.ShowDialog();
        }

        private void btnPip_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("PipPage.xaml");
        }

        private void miColumnChart_Click(object sender, RoutedEventArgs e)
        {
            
            if (ProjectGrid.CurrentColumn != null)
            {
                var column = ProjectGrid.CurrentColumn as DataGridTextColumn;
                if (column != null)
                {
                    if (column.Binding != null)
                    {
                        var binding = column.Binding as Binding;
                        var chart1 = ChartHelper.GetChart(column.Header.ToString(), ChartHelper.ColumnSeries(column.Header.ToString(), ProjectGrid.ItemsSource, "P1A", binding.Path.Path, Brushes.YellowGreen));
                        chart1.Width = 900;
                        var window = new Window { Width = 1024, Height = 768, WindowStartupLocation = WindowStartupLocation.CenterScreen, ShowInTaskbar = false, Owner = MainWindow.Current };
                        window.Content = chart1;
                        try
                        {
                            window.Show();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }       
    }

    public class MultipleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = System.Convert.ToInt32(value);
            return CityStatistics.MultipleEnum[i];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CityStatistics.MultipleEnum.ToList().IndexOf(value.ToString());
        }
    }

    public class PipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 0 ? "Yes" : "No";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Yes" ? 0 : 1;
        }
    }

    public class BumenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = System.Convert.ToInt32((value as List<int>)[0]);
            if (i == -1)
            {
                return string.Empty;
            }
            return ProjectStatistics.Sector[i];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new List<int> { ProjectStatistics.Sector.ToList().IndexOf(value.ToString()) };
        }
    }

    public class ZhuangtaiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = System.Convert.ToInt32(value);
            if (i == -1)
            {
                return string.Empty;
            }
            return ProjectStatistics.StatusOfProject[i];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ProjectStatistics.StatusOfProject.ToList().IndexOf(value.ToString());
        }
    }

    public class HeaderColorConverter : IValueConverter
    {
        public static Dictionary<string, SolidColorBrush> Dict = new Dictionary<string, SolidColorBrush>
        {
            {"项目", Brushes.Silver},
            {"排序", Parse("#333333")},
            {"状况", Parse("#333333")},
            {"部门", Parse("#333333")},
            {"目标得分", Parse("#003366")},
            {"公众意见得分", Parse("#003366")},
            {"环境影响得分", Parse("#003366")},
            {"社会经济影响得分", Parse("#003366")},
            {"可行性得分", Parse("#003366")},
            {"最终得分", Brushes.Maroon},
            {"环境场景", Parse("#99CC00")},
            {"经济场景", Parse("#99CC00")},
            {"收入增加场景", Parse("#99CC00")},
            {"社区1（生活质素）", Parse("#99CC00")},
            {"社区2（减贫）", Parse("#99CC00")},
            {"资本成本", Parse("#333333")},
            {"成本在预算中的占比", Parse("#333333")},
            {"自有资源在成本中的占比", Parse("#333333")},
            {"国家政府投入在成本中的占比", Parse("#333333")},
            {"私人部门投入在成本中的占比", Parse("#333333")},
            {"贷款在成本中的占比", Parse("#333333")},
            {"融资缺口在成本中的占比", Parse("#333333")},
            {"包括在PIP中", Parse("#333333")}
        };

        private static SolidColorBrush Parse(string value)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueString = value == null ? "项目" : value.ToString();
            return Dict[valueString];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class HeaderTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueString = value == null ? "项目" : value.ToString();
            var color = HeaderColorConverter.Dict[valueString].Color;
            if (color.R + color.G + color.B > 255)
            {
                return Brushes.Black;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Empty;
        }
    }

    public class HeaderTextLayoutConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new VerticalTextBlock { Text = (value ?? string.Empty).ToString() };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as VerticalTextBlock).Text;
        }
    }
}
