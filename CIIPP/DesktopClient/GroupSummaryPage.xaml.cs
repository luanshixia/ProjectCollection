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

using CIIPP;

using System.Windows.Media.Animation;

namespace DesktopClient
{
    /// <summary>
    /// GroupSummaryPage.xaml 的交互逻辑
    /// </summary>
    public partial class GroupSummaryPage : Page
    {
        private ProjectGroup _basket = new ProjectGroup(DocumentManager.CurrentDocument.Projects);
        private List<UIElement> _toClear = new List<UIElement>();

        public GroupSummaryPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            cbbGroup.SelectedIndex = 0;
            Update();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbGroup.SelectedIndex == 0)
            {
                _basket = new ProjectGroup(DocumentManager.CurrentDocument.Projects);
            }
            else if (cbbGroup.SelectedIndex == 4)
            {
                _basket = new ProjectGroup(DocumentManager.CurrentDocument.Projects.Where(x => x.PIP == 0));
            }
            else
            {
                int p1e = cbbGroup.SelectedIndex - 1;
                _basket = new ProjectGroup(DocumentManager.CurrentDocument.Projects.Where(x => x.P1E == p1e));
            }
            this.Update();
        }

        private void Update()
        {
            //StackPanel Col0 = new StackPanel();
            //StackPanel Col1 = new StackPanel();
            //StackPanel Col2 = new StackPanel();
            //Grid.SetColumn(Col0, 0);
            //Grid.SetColumn(Col1, 1);
            //Grid.SetColumn(Col2, 2);

            //_toClear.ForEach(x => LayoutRoot.Children.Remove(x));
            Col0.Children.Clear();
            Col1.Children.Clear();
            Col2.Children.Clear();
            var doc = DocumentManager.CurrentDocument;

            //---------------------------------图表1--------------------------------------------------
            var chart1 = ChartHelper.GetChart("图1: 资本投资 vs 支出限度",
                ChartHelper.ColumnSeries("资本投资", Enumerable.Range(doc.City.C02 + 2, 5).Select(x => new { Key = x.ToString(), Value = NaNto0(_basket.P20A[x]) }).ToArray(), Brushes.Purple),
                ChartHelper.ColumnSeries("现有支出", Enumerable.Range(doc.City.C02 + 2, 5).Select(x => new { Key = x.ToString(), Value = NaNto0(_basket.C8B[x]) }).ToArray(), Brushes.YellowGreen),
                ChartHelper.LineSeries("支出限额", Enumerable.Range(doc.City.C02 + 2, 5).Select(x => new { Key = x.ToString(), Value = NaNto0(_basket.C8A[x]) }).ToArray(), Brushes.Red));
            _toClear.Add(chart1);
            Col0.Children.Add(chart1);

            //---------------------------------图表2--------------------------------------------------
            var chart2 = ChartHelper.GetChart("图2：需贷款额度 vs 还债能力",
                ChartHelper.ColumnSeries("新增贷款年还债", Enumerable.Range(doc.City.C02 + 2, 5).Select(x => new { Key = x.ToString(), Value = NaNto0(_basket.P22A[x]) }).ToArray(), Brushes.Orange),
                ChartHelper.ColumnSeries("现有年还债能力", Enumerable.Range(doc.City.C02 + 2, 5).Select(x => new { Key = x.ToString(), Value = NaNto0(_basket.C7C[x]) }).ToArray(), Brushes.Navy),
                ChartHelper.LineSeries("估算最大年还债", Enumerable.Range(doc.City.C02 + 2, 5).Select(x => new { Key = x.ToString(), Value = NaNto0(_basket.C8E[x]) }).ToArray(), Brushes.Red));
            _toClear.Add(chart2);
            Col0.Children.Add(chart2);

            //---------------------------------总表--------------------------------------------------
            YearwiseTable tableP20 = new YearwiseTable { SectionNumber = "", Title = "5年计划", Brush_TitleCell = Brushes.Silver, Brush_Title = Brushes.DarkRed };
            tableP20.Rows.Add("P20A", _basket.P20A);
            tableP20.Rows.Add("P20B", _basket.P20B);
            tableP20.Rows.Add("P20C", _basket.P20C);
            tableP20.Rows.Add("P20D", _basket.P20D);
            tableP20.Rows.Add("P20E", _basket.P20E);
            tableP20.Rows.Add("P20F", _basket.P20F);
            tableP20.Rows.Add("P20G", _basket.P20G);
            Enumerable.Range(2, 5).ToList().ForEach(x => tableP20.Years.Add(new YearDefinition(x, false, true)));
            tableP20.Years.Add(YearDefinition.Sum);
            tableP20.Render();
            _toClear.Add(tableP20);
            Col1.Children.Add(tableP20);

            YearwiseTable tableP21 = new YearwiseTable { SectionNumber = "", Title = "", Brush_TitleCell = Brushes.Silver, Brush_Title = Brushes.DarkRed };
            tableP21.Rows.Add("GC8A", _basket.C8A);
            tableP21.Rows.Add("GC8B", _basket.C8B);
            tableP21.Rows.Add("GC8C", _basket.C8C_1);
            tableP21.Rows.Add("GC7C", _basket.C7C);
            tableP21.Rows.Add("P22A", _basket.P22A);
            tableP21.Rows.Add("GC8E", _basket.C8E);
            Enumerable.Range(2, 5).ToList().ForEach(x => tableP21.Years.Add(new YearDefinition(x, false, true)));
            tableP21.Years.Add(YearDefinition.Sum);
            tableP21.Render();
            _toClear.Add(tableP21);
            Col1.Children.Add(tableP21);

            //---------------------------------分表--------------------------------------------------
            foreach (var proj in _basket.Projects)
            {
                YearwiseTable table = new YearwiseTable { SectionNumber = "", Title = proj.P1A, Brush_TitleCell = Brushes.Navy, Brush_Title = Brushes.White };
                table.Rows.Add("P20A", proj.P20A);
                table.Rows.Add("P20B", proj.P20B);
                table.Rows.Add("P20C", proj.P20C);
                table.Rows.Add("P20D", proj.P20D);
                table.Rows.Add("P20E", proj.P20E);
                table.Rows.Add("P20F", proj.P20F);
                table.Rows.Add("P22A", proj.P22A);
                Enumerable.Range(2, 5).ToList().ForEach(x => table.Years.Add(new YearDefinition(x, false, true)));
                table.Years.Add(YearDefinition.Sum);
                table.Render();
                _toClear.Add(table);
                Col2.Children.Add(table);
            }

            //cvs.Height = new List<StackPanel> { Col0, Col1, Col2 }.Max(x => x.ActualHeight); // 为什么AcutalHeight都为0？
            //var h = new List<StackPanel> { Col0, Col1, Col2 }.Max(x => x.RenderSize.Height);
        }

        private static double NaNto0(double num)
        {
            if (double.IsNaN(num))
            {
                return 0;
            }
            return num;
        }

        private void btnPage1_Click(object sender, RoutedEventArgs e)
        {
            //DoubleAnimation da = new DoubleAnimation(Canvas.GetLeft(SwipePanel), 0, new Duration(TimeSpan.Parse("0:0:0.3")));
            //SwipePanel.BeginAnimation(Canvas.LeftProperty, da);

            ThicknessAnimation ta = new ThicknessAnimation(SwipePanel.Margin, new Thickness(0, 0, 0, 0), new Duration(TimeSpan.Parse("0:0:0.3")));
            SwipePanel.BeginAnimation(Grid.MarginProperty, ta);
        }

        private void btnPage2_Click(object sender, RoutedEventArgs e)
        {
            //DoubleAnimation da = new DoubleAnimation(Canvas.GetLeft(SwipePanel), -1024, new Duration(TimeSpan.Parse("0:0:0.3")));
            //SwipePanel.BeginAnimation(Canvas.LeftProperty, da);

            ThicknessAnimation ta = new ThicknessAnimation(SwipePanel.Margin, new Thickness(-1024, 0, 0, 0), new Duration(TimeSpan.Parse("0:0:0.3")));
            SwipePanel.BeginAnimation(Grid.MarginProperty, ta);
        }

        private void btnPage3_Click(object sender, RoutedEventArgs e)
        {
            //DoubleAnimation da = new DoubleAnimation(Canvas.GetLeft(SwipePanel), -2048, new Duration(TimeSpan.Parse("0:0:0.3")));
            //SwipePanel.BeginAnimation(Canvas.LeftProperty, da);

            ThicknessAnimation ta = new ThicknessAnimation(SwipePanel.Margin, new Thickness(-2048, 0, 0, 0), new Duration(TimeSpan.Parse("0:0:0.3")));
            SwipePanel.BeginAnimation(Grid.MarginProperty, ta);
        }
    }
}
