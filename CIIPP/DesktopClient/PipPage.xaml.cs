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
    public partial class PipPage : Page
    {
        private ProjectGroup _basket = new ProjectGroup(DocumentManager.CurrentDocument.Projects);
        private static int _slidePos = 0;
        public static PipPage Current { get; private set; }

        public PipPage()
        {
            InitializeComponent();
            Current = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var slideString = NavigationManager.GetQueryString("slide");
            if (slideString != string.Empty)
            {
                _slidePos = Convert.ToInt32(slideString);
            }

            _basket = new ProjectGroup(DocumentManager.CurrentDocument.Projects.Where(x => x.PIP == 0));
            Update().ForEach(x => Pager.Slides.Add(x));
            Pager.ReadyControl(_slidePos);

        }

        public List<StackPanel> Update()
        {
            List<StackPanel> panel = Enumerable.Range(0, 3).Select(x => new StackPanel()).ToList();
            //var Col0 = new StackPanel();
            //var Col1 = new StackPanel();
            //var Col2 = new StackPanel();
            //Pager.Slides.Clear();
            //Pager.Slides.Add(Col0);
            //Pager.Slides.Add(Col1);
            //Pager.Slides.Add(Col2);
            //Pager.ReadyControl(_slidePos);

            var doc = DocumentManager.CurrentDocument;

            //---------------------------------图表1--------------------------------------------------
            var chart1 = ChartHelper.GetMyChart("图1: 资本投资 vs 支出限度",
                ChartHelper.MyColumnSeries("现有支出", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(_basket.C8B[x]))).ToArray(), StackedChart.BuildGradient(Colors.YellowGreen)),
                ChartHelper.MyColumnSeries("资本投资", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(_basket.P20A[x]))).ToArray(), StackedChart.BuildGradient(Color.FromRgb(153, 51, 102))),
                ChartHelper.MyLineSeries("支出限额", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(_basket.C8D[x]))).ToArray(), Brushes.Red));
            panel[0].Children.Add(chart1);
            //Col0.Children.Add(chart1);

            //---------------------------------图表2--------------------------------------------------
            var chart2 = ChartHelper.GetMyChart("图2：需贷款额度 vs 还债能力",
                ChartHelper.MyColumnSeries("现有支出", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(_basket.C7C[x]))).ToArray(), StackedChart.BuildGradient(Color.FromRgb(23, 55, 94))),
                ChartHelper.MyColumnSeries("资本投资", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(_basket.P22A[x]))).ToArray(), StackedChart.BuildGradient(Colors.Orange)),
                ChartHelper.MyLineSeries("估算最大年还债", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(_basket.C8E[x]))).ToArray(), Brushes.Red));
            panel[0].Children.Add(chart2);
            //Col0.Children.Add(chart2);

            //---------------------------------总表--------------------------------------------------
            YearwiseTable tableP20 = new YearwiseTable { SectionNumber = "", Title = "5年计划", Brush_TitleCell = Brushes.Silver, Brush_Title = Brushes.DarkRed };
            tableP20.Rows.Add("P20G", _basket.P20G);
            tableP20.Rows.Add("P20A", _basket.P20A);
            tableP20.Rows.Add("P20B", _basket.P20B);
            tableP20.Rows.Add("P20C", _basket.P20C);
            tableP20.Rows.Add("P20D", _basket.P20D);
            tableP20.Rows.Add("P20E", _basket.P20E);
            tableP20.Rows.Add("P20F", _basket.P20F);
            Enumerable.Range(1, 5).ToList().ForEach(x => tableP20.Years.Add(new YearDefinition(x, false, true)));
            tableP20.Years.Add(YearDefinition.Sum);
            tableP20.Render();
            panel[1].Children.Add(tableP20);
            //Col1.Children.Add(tableP20);

            YearwiseTable tableP21 = new YearwiseTable { SectionNumber = "", Title = "", Brush_TitleCell = Brushes.Silver, Brush_Title = Brushes.DarkRed };
            tableP21.Rows.Add("GC8A", _basket.C8D);
            tableP21.Rows.Add("GC8B", _basket.C8B);
            tableP21.Rows.Add("GC8C", _basket.C8C_1);
            tableP21.Rows.Add("GC7C", _basket.C7C);
            tableP21.Rows.Add("P22A", _basket.P22A);
            tableP21.Rows.Add("GC8E", _basket.C8E);
            Enumerable.Range(1, 5).ToList().ForEach(x => tableP21.Years.Add(new YearDefinition(x, false, true)));
            tableP21.Years.Add(YearDefinition.Sum);
            tableP21.Render();
            panel[1].Children.Add(tableP21);
            //Col1.Children.Add(tableP21);

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
                Enumerable.Range(1, 5).ToList().ForEach(x => table.Years.Add(new YearDefinition(x, false, true)));
                table.Years.Add(YearDefinition.Sum);
                table.Render();
                panel[2].Children.Add(table);
                //Col2.Children.Add(table);
            }
            return panel;
        }

        private static double NaNto0(double num)
        {
            if (double.IsNaN(num))
            {
                return 0;
            }
            return num;
        }

        private void Pager_SlideChanged(object sender, EventArgs e)
        {
            banner.Caption = Caption.Pip(Pager.CurrentSlideNumber);
        }

        public void SetSlidePos(int slidePos)
        {
            _slidePos = slidePos;
            Pager.CurrentSlideNumber = slidePos;
        }
    }
}
