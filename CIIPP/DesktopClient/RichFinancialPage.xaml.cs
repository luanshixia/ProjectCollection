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

namespace DesktopClient
{
    /// <summary>
    /// RichFinancialPage.xaml 的交互逻辑
    /// </summary>
    public partial class RichFinancialPage : Page
    {
        private List<IUpdateTable> _tables = new List<IUpdateTable>();
        private static int _slidePos = 0;
        public static RichFinancialPage Current { get; private set; }

        public RichFinancialPage()
        {
            InitializeComponent();
            Current = this;
        }

        public List<StackPanel> GetPages()
        {
            List<StackPanel> panel = Enumerable.Range(0, 7).Select(x => new StackPanel()).ToList();

            Document doc = DocumentManager.CurrentDocument;

            //panel[0].Children.Add(new TextBlock { Text = "LOCAL GOVERNMENT BUDGET FORECAST / 当地政府预算预测", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            YearwiseTable tableC6 = new YearwiseTable { SectionNumber = "1.6", Title = "当地政府收入 Local Government Revenues" };
            tableC6.Rows.Add("C6A", doc.City.C6A);
            tableC6.Rows.Add("C6B", doc.City.C6B);
            tableC6.Rows.Add("C6C", doc.City.C6C);
            tableC6.Rows.Add("C6D", doc.City.C6D);
            tableC6.Rows.Add("C6E", doc.City.C6E);
            tableC6.Rows.Add("C6F", doc.City.C6F);
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC6.Years.Add(new YearDefinition(x, false, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableC6.Years.Add(new YearDefinition(x, false, true)));
            tableC6.Render();
            panel[0].Children.Add(tableC6);

            YearwiseTable tableC7 = new YearwiseTable { SectionNumber = "1.7", Title = "当地政府支出 Local Government Expenditures" };
            tableC7.Rows.Add("C7A", doc.City.C7A);
            tableC7.Rows.Add("C7B", doc.City.C7B);
            tableC7.Rows.Add("C7C", doc.City.C7C);
            tableC7.Rows.Add("C7D", doc.City.C7D);
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC7.Years.Add(new YearDefinition(x, false, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableC7.Years.Add(new YearDefinition(x, false, true)));
            tableC7.Render();
            panel[0].Children.Add(tableC7);

            YearwiseTable tableC8 = new YearwiseTable { SectionNumber = "1.8", Title = "总投资能力 Summary Investment Capacity" };
            tableC8.Rows.Add("C8A", doc.City.C8A);
            tableC8.Rows.Add("C8B", doc.City.C8B);
            tableC8.Rows.Add("C8C", doc.City.C8C);
            tableC8.Rows.Add("C8D", doc.City.C8D);
            tableC8.Rows.Add("C8E", doc.City.C8E);
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC8.Years.Add(new YearDefinition(x, false, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableC8.Years.Add(new YearDefinition(x, false, true)));
            tableC8.Render();
            panel[1].Children.Add(tableC8);

            //panel[2].Children.Add(new TextBlock { Text = "ASSUMPTIONS / 预算预测假设", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            YearwiseTable tableC9 = new YearwiseTable { SectionNumber = "1.9", Title = "宏观经济数据假设 Assumptions Macro-Economic Data" };
            tableC9.Rows.Add("C9A", doc.City.C9A);
            tableC9.Rows.Add("C9B", doc.City.C9B);
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC9.Years.Add(new YearDefinition(x, true, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableC9.Years.Add(new YearDefinition(x, true, true)));
            tableC9.SetPercentageRows("C9A", "C9B");
            tableC9.Render();
            tableC9.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableC9);

            PropertyTable tableC10 = new PropertyTable { SectionNumber = "1.10", Title = "商业贷款条件假设 Assumptions Loan Conditions Commercial Lending", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC10.Rows.Add("C10A", Tuple.Create<object, object>(doc.City, 0.0));
            tableC10.Rows.Add("C10B", Tuple.Create<object, object>(doc.City, 0.0));
            tableC10.SetPercentageRows("C10A");
            tableC10.Render();
            tableC10.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableC10);

            PropertyTable tableC11 = new PropertyTable { SectionNumber = "1.11", Title = "优惠贷款条件假设 Assumptions Loan Conditions Preferential Lending", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC11.Rows.Add("C11A", Tuple.Create<object, object>(doc.City, 0.0));
            tableC11.Rows.Add("C11B", Tuple.Create<object, object>(doc.City, 0.0));
            tableC11.SetPercentageRows("C11A");
            tableC11.Render();
            tableC11.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableC11);

            PropertyTable tableC12 = new PropertyTable { SectionNumber = "1.12", Title = "当地税收征管假设 Local Tax Collection", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC12.Rows.Add("C12A", Tuple.Create<object, object>(doc.City, 0.0));
            tableC12.Rows.Add("C12B", Tuple.Create<object, object>(doc.City, 0.0));
            tableC12.Rows.Add("C12C", Tuple.Create<object, object>(doc.City, 0.0));
            tableC12.SetPercentageRows("C12A", "C12C");
            tableC12.Render();
            tableC12.UserInput += (sender, e) => UpdateData();
            panel[3].Children.Add(tableC12);

            PropertyTable tableC13 = new PropertyTable { SectionNumber = "1.13", Title = "汇率假设（兑美元） Assumptions Exchange Rate (to USD)", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC13.Rows.Add("C13A", Tuple.Create<object, object>(doc.City, 0.0));
            tableC13.Render();
            tableC13.UserInput += (sender, e) => UpdateData();
            panel[3].Children.Add(tableC13);

            PropertyTable tableC14 = new PropertyTable { SectionNumber = "1.14", Title = "投资预算和债务清偿假设 Assumptions Investment Budget & Debt Service", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC14.Rows.Add("C14A", Tuple.Create<object, object>(doc.City, 0.0));
            tableC14.Rows.Add("C14B", Tuple.Create<object, object>(doc.City, 0.0));
            tableC14.SetPercentageRows("C14A", "C14B");
            tableC14.Render();
            tableC14.UserInput += (sender, e) => UpdateData();
            panel[3].Children.Add(tableC14);

            PropertyTable tableC15 = new PropertyTable { SectionNumber = "1.15", Title = "收入预测假设 Assumptions Revenue Forecast", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC15.Rows.Add("C15A_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15A_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15B_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15B_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15C_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15C_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15D_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15D_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15E_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.Rows.Add("C15E_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC15.SetPercentageTable();
            tableC15.Render();
            tableC15.UserInput += (sender, e) => UpdateData();
            panel[4].Children.Add(tableC15);

            PropertyTable tableC16 = new PropertyTable { SectionNumber = "1.16", Title = "支出预测假设 Assumptions Expenditure Forecast", Brush_TitleCell = FindResource("gradient_red") as Brush };
            tableC16.Rows.Add("C16A_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC16.Rows.Add("C16A_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC16.Rows.Add("C16B_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC16.Rows.Add("C16B_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC16.Rows.Add("C16C_1", Tuple.Create<object, object>(doc.City, 0.0));
            tableC16.Rows.Add("C16C_2", Tuple.Create<object, object>(doc.City, 0.0));
            tableC16.SetPercentageTable();
            tableC16.Render();
            tableC16.UserInput += (sender, e) => UpdateData();
            panel[4].Children.Add(tableC16);

            //---------------------------------图表--------------------------------------------------
            panel[5].Children.Add(ChartHelper.GetChart("BUDGET FORECAST: REVENUES VERSUS EXPENDITURES",
                ChartHelper.ColumnSeries("经营性支出", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new { Key = x.ToString(), Value = doc.City.C8B[x] }).ToArray(), Brushes.YellowGreen),
                ChartHelper.LineSeries("经营性收入", Enumerable.Range(doc.City.C02 + 1, 5).Select(x => new { Key = x.ToString(), Value = doc.City.C8A[x] }).ToArray(), Brushes.Red)));

            panel[6].Children.Add(new TextBlock { Text = "CITY BASIC / 城市基本数据", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            Button btnFinancial = new Button { Content = "点击以转到城市评估", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            btnFinancial.Click += (sender, e) => NavigationManager.Navigate("RichCityPage.xaml");
            panel[6].Children.Add(btnFinancial);
            //Button btnProj = new Button { Content = "点击以转到项目评估", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            //btnProj.Click += (sender, e) => NavigationManager.Navigate("RichCityPage.xaml");
            //panel[6].Children.Add(btnProj);

            _tables = new List<IUpdateTable> { tableC6, tableC7, tableC8 };

            return panel;
        }

        public void UpdateData()
        {
            _tables.ForEach(x => x.Render());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var slideString = NavigationManager.GetQueryString("slide");
            if (slideString != string.Empty)
            {
                _slidePos = Convert.ToInt32(slideString);
            }

            GetPages().ForEach(x => Pager.Slides.Add(x));
            Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.Financial(x)).ToList();
            Pager.ReadyControl(_slidePos);
        }

        private void Pager_SlideChanged(object sender, EventArgs e)
        {
            banner.Caption = Caption.Financial(Pager.CurrentSlideNumber);
        }

        public void SetSlidePos(int slidePos)
        {
            _slidePos = slidePos;
            Pager.CurrentSlideNumber = slidePos;
        }
    }
}
