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
    /// RichProjectPage.xaml 的交互逻辑
    /// </summary>
    public partial class RichProjectPage : Page
    {
        private static int _projId = 0;
        private List<IUpdateTable> _tables = new List<IUpdateTable>();
        private static int _slidePos = 0;

        public static int ProjId { get { return _projId; } set { _projId = value; } }

        private Brush _titleBrush;

        public static RichProjectPage Current { get; private set; }

        public RichProjectPage()
        {
            InitializeComponent();

            _titleBrush = FindResource("gradient_red") as Brush;
            Current = this;
        }

        public List<StackPanel> GetPages()
        {
            List<StackPanel> panel = Enumerable.Range(0, 12).Select(x => new StackPanel()).ToList();

            Document doc = DocumentManager.CurrentDocument;
            ProjectStatistics proj = doc.Projects[_projId];

            Binding pipBinding = new Binding("PIP") { Source = proj, Mode = BindingMode.TwoWay };
            panel[0].Children.Add(new TextBlock { Text = "包含在PIP中？ Include project in PIP?", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            ComboBox cbbPipTop = new ComboBox { Width = 200, HorizontalAlignment = HorizontalAlignment.Left, ItemsSource = new string[] { "Yes", "No" } };
            cbbPipTop.SetBinding(ComboBox.SelectedIndexProperty, pipBinding);
            panel[0].Children.Add(cbbPipTop);

            //---------------------------------表2.1--------------------------------------------------
            panel[0].Children.Add(new TextBlock { Text = "2.1 项目描述 PROJECT DESCRIPTORS", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 0) });
            StackPanel tableP1 = new StackPanel { Width = 500, HorizontalAlignment = HorizontalAlignment.Left };

            tableP1.Children.Add(new TextBlock { Text = "A. Project Name/项目名称是什么", Foreground = Brushes.Maroon, Margin = new Thickness(0, 5, 0, 0) });
            TextBox txtP1A = new TextBox { Text = proj.P1A };
            txtP1A.LostFocus += GetTextBoxLostFocusHandler(txtP1A, GetValidator(txtP1A), () => proj.P1A = txtP1A.Text, txtP1A.Text);
            tableP1.Children.Add(txtP1A);
            tableP1.Children.Add(new TextBlock { Text = "B. Location/项目位于城市何处", Foreground = Brushes.Maroon, Margin = new Thickness(0, 5, 0, 0) });
            TextBox txtP1B = new TextBox { Text = proj.P1B };
            txtP1B.LostFocus += GetTextBoxLostFocusHandler(txtP1B, GetValidator(txtP1B), () => proj.P1B = txtP1B.Text, txtP1B.Text);
            tableP1.Children.Add(txtP1B);
            tableP1.Children.Add(new TextBlock { Text = "C. Sector - To which sector does the project belong/项目属于哪个部门", Foreground = Brushes.Maroon, Margin = new Thickness(0, 5, 0, 0) });
            ComboBox cbbP1C = new ComboBox();
            ProjectStatistics.Sector.ToList().ForEach(x => cbbP1C.Items.Add(x));
            cbbP1C.SelectedIndex = proj.P1C[0];
            cbbP1C.SelectionChanged += (sender, e) => proj.P1C[0] = cbbP1C.SelectedIndex;
            tableP1.Children.Add(cbbP1C);
            tableP1.Children.Add(new TextBlock { Text = "D. Purpose - What is the main dimension of the project/项目主要方面", Foreground = Brushes.Maroon, Margin = new Thickness(0, 5, 0, 0) });
            ComboBox cbbP1D = new ComboBox();
            ProjectStatistics.Purpose.ToList().ForEach(x => cbbP1D.Items.Add(x));
            cbbP1D.SelectedIndex = proj.P1D;
            cbbP1D.SelectionChanged += (sender, e) => proj.P1D = cbbP1D.SelectedIndex;
            tableP1.Children.Add(cbbP1D);
            tableP1.Children.Add(new TextBlock { Text = "E. Status of Project - What is the current status of the project/项目现状如何", Foreground = Brushes.Maroon, Margin = new Thickness(0, 5, 0, 0) });
            ComboBox cbbP1E = new ComboBox();
            ProjectStatistics.StatusOfProject.ToList().ForEach(x => cbbP1E.Items.Add(x));
            cbbP1E.SelectedIndex = proj.P1E;
            cbbP1E.SelectionChanged += (sender, e) => proj.P1E = cbbP1E.SelectedIndex;
            tableP1.Children.Add(cbbP1E);
            tableP1.Children.Add(new TextBlock { Text = "F. Time Frame - Expected commencement & completion year/时间框架", Foreground = Brushes.Maroon, Margin = new Thickness(0, 5, 0, 0) });
            ComboBox cbbP1F_1 = new ComboBox();
            ComboBox cbbP1F_2 = new ComboBox { Margin = new Thickness(0, 5, 0, 0) };
            Enumerable.Range(0, 10).Select(x => proj.P02 + x).ToList().ForEach(x => { cbbP1F_1.Items.Add(x); cbbP1F_2.Items.Add(x); });
            cbbP1F_1.SelectedItem = proj.P1F_1;
            cbbP1F_2.SelectedItem = proj.P1F_2;
            cbbP1F_1.SelectionChanged += (sender, e) => proj.P1F_1 = Convert.ToInt32(cbbP1F_1.SelectedItem);
            cbbP1F_2.SelectionChanged += (sender, e) => proj.P1F_2 = Convert.ToInt32(cbbP1F_2.SelectedItem);
            tableP1.Children.Add(cbbP1F_1);
            tableP1.Children.Add(cbbP1F_2);

            panel[0].Children.Add(tableP1);

            //---------------------------------表2.2--------------------------------------------------
            panel[1].Children.Add(new TextBlock { Text = "FINANCIAL PARAMETERS", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            YearwiseTable tableP2 = new YearwiseTable();
            tableP2.SectionNumber = "2.2";
            tableP2.Title = "资本成本 Capital Cost";
            tableP2.Rows.Add("P2A", proj.P2A);
            tableP2.Rows.Add("P2B", proj.P2B);
            tableP2.Rows.Add("P2C", proj.P2C);
            tableP2.Rows.Add("P2D", proj.P2D);
            tableP2.Rows.Add("P2E", proj.P2E);
            tableP2.Rows.Add("P2F", proj.P2F);
            tableP2.LockRows("P2F");
            Enumerable.Range(proj.P1F_1 - proj.P02, 5).ToList().ForEach(x => tableP2.Years.Add(new YearDefinition(x, true, true)));
            tableP2.Years.Add(YearDefinition.Sum);
            tableP2.Render();
            tableP2.UserInput += (sender, e) => UpdateData();
            panel[1].Children.Add(tableP2);

            //---------------------------------表2.3--------------------------------------------------
            YearwiseTable tableP3 = new YearwiseTable();
            tableP3.SectionNumber = "2.3";
            tableP3.Title = "资本投资的预期资金来源 Anticipated Source for Capital";
            tableP3.Rows.Add("P3A", proj.P3A);
            tableP3.Rows.Add("P3B", proj.P3B);
            tableP3.Rows.Add("P3C", proj.P3C);
            tableP3.Rows.Add("P3D", proj.P3D);
            tableP3.Rows.Add("P3E", proj.P3E);
            tableP3.Rows.Add("P3T", proj.P3T);
            tableP3.Rows.Add("P3F", proj.P3F);
            tableP3.LockRows("P3T", "P3F");
            Enumerable.Range(proj.P1F_1 - proj.P02, 5).ToList().ForEach(x => tableP3.Years.Add(new YearDefinition(x, true, true)));
            tableP3.Years.Add(YearDefinition.Sum);
            tableP3.Render();
            tableP3.UserInput += (sender, e) => UpdateData();
            panel[1].Children.Add(tableP3);

            //---------------------------------表2.4--------------------------------------------------
            PropertyTable tableP4 = new PropertyTable { SectionNumber = "2.4", Title = "运营与维护成本 Operating & Maintenance Costs" };
            tableP4.Rows.Add("P4A", Tuple.Create<object, object>(proj, 0.0));
            tableP4.Render();
            tableP4.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableP4);

            //---------------------------------表2.5--------------------------------------------------
            PropertyTable tableP5 = new PropertyTable { SectionNumber = "2.5", Title = "新增商业贷款 Commercial Loans Required" };
            tableP5.Rows.Add("P5A", Tuple.Create<object, object>(proj, 0));
            tableP5.Rows.Add("P5B", Tuple.Create<object, object>(proj, 0));
            tableP5.Rows.Add("P5C", Tuple.Create<object, object>(proj, 0));
            tableP5.Rows.Add("P5D", Tuple.Create<object, object>(proj, 0.0));
            tableP5.SetPercentageRows("P5D");
            tableP5.Render();
            tableP5.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableP5);

            //---------------------------------表2.6--------------------------------------------------
            PropertyTable tableP6 = new PropertyTable { SectionNumber = "2.6", Title = "新增优惠贷款 Preferential Loans Required" };
            tableP6.Rows.Add("P6A", Tuple.Create<object, object>(proj, 0));
            tableP6.Rows.Add("P6B", Tuple.Create<object, object>(proj, 0));
            tableP6.Rows.Add("P6C", Tuple.Create<object, object>(proj, 0));
            tableP6.Rows.Add("P6D", Tuple.Create<object, object>(proj, 0.0));
            tableP6.SetPercentageRows("P6D");
            tableP6.Render();
            tableP6.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableP6);

            //---------------------------------表2.7--------------------------------------------------
            PropertyTable tableP7 = new PropertyTable { SectionNumber = "2.7", Title = "运营与维护成本的资金来源 Source of Operating & Maintenance Costs" };
            tableP7.Rows.Add("P7A", Tuple.Create<object, object>(proj, 0.0));
            tableP7.Rows.Add("P7B", Tuple.Create<object, object>(proj, 0.0));
            tableP7.Rows.Add("P7C", Tuple.Create<object, object>(proj, 0.0));
            tableP7.Rows.Add("P7D", Tuple.Create<object, object>(proj, 0.0));
            tableP7.Rows.Add("P7E", Tuple.Create<object, object>(proj, 0.0));
            tableP7.Render();
            tableP7.UserInput += (sender, e) => UpdateData();
            panel[2].Children.Add(tableP7);

            //---------------------------------问卷--------------------------------------------------

            //Button btnToQ = new Button { Content = "打开问卷(总结模式)", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            //btnToQ.Click += (sender, e) => NavigationManager.Navigate("Questionnaire2.xaml", "id=" + _projId.ToString());
            //panel[3].Children.Add(btnToQ);

            if (!MainWindow.Current.IsUglyTheme)
            {
                panel[3].Children.Add(new TextBlock { Text = "PRIORITISATION PARAMETERS", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
                Button btnToQ1 = new Button { Content = "打开问卷(演示模式)", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
                btnToQ1.Click += (sender, e) => NavigationManager.Navigate("QuestionPage.xaml", "type=project&id=" + _projId.ToString());
                panel[3].Children.Add(btnToQ1);
            }
            else
            {
                QuestionPage page = new QuestionPage();
                panel[3].Children.Add(page.GetContent("project", _projId));
            }

            //---------------------------------场景得分--------------------------------------------------
            PropertyTable tablePS = new PropertyTable { SectionNumber = "2.S", Title = "场景得分 Senario Scores", Brush_ResultCell = Brushes.DarkRed, Brush_TitleCell = _titleBrush, Margin = new Thickness(0, 20, 0, 0) };
            tablePS.Rows.Add("EnvironmentScenario", Tuple.Create<object, object>(proj, 0.0));
            tablePS.Rows.Add("EconomicScenario", Tuple.Create<object, object>(proj, 0.0));
            tablePS.Rows.Add("RevenueScenario", Tuple.Create<object, object>(proj, 0.0));
            tablePS.Rows.Add("Social", Tuple.Create<object, object>(proj, 0.0));
            tablePS.Rows.Add("PovertyScenario", Tuple.Create<object, object>(proj, 0.0));
            tablePS.LockRows("EnvironmentScenario", "EconomicScenario", "RevenueScenario", "Social", "PovertyScenario");
            tablePS.Render();
            if (MainWindow.Current.IsUglyTheme)
            {
                //Button btnRefreshTablePS = new Button { Content = "刷新场景得分", Margin = new Thickness(0, 20, 0, 0), Width = 100 };
                //btnRefreshTablePS.Click += (s, arg) => tablePS.UpdateData();
                //panel[3].Children.Add(btnRefreshTablePS);
                //tablePS.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            }
            else
            {
                panel[3].Children.Add(tablePS);
            }

            //---------------------------------表2.13--------------------------------------------------
            //panel[4].Children.Add(new TextBlock { Text = "2.13 项目附加数据 ADDITIONAL PROJECT DATA", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            PropertyTable tableP13 = new PropertyTable { SectionNumber = "2.13", Title = "项目附加数据 ADDITIONAL PROJECT DATA", Brush_ResultCell = Brushes.DarkRed, Brush_TitleCell = _titleBrush };
            tableP13.Rows.Add("P13A1", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13A2", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13A3", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13B", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13C", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13D1", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13D2", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Rows.Add("P13D3", Tuple.Create<object, object>(proj, string.Empty));
            tableP13.Render();
            panel[4].Children.Add(tableP13);

            //---------------------------------表2.14--------------------------------------------------
            panel[5].Children.Add(new TextBlock { Text = "ANTICIPATED IMPACT OF THE PROJECT ON THE BUDGET", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            YearwiseTable tableP14 = new YearwiseTable { SectionNumber = "2.14", Title = "新增商业贷款 New Commercial Loans", Brush_TitleCell = _titleBrush };
            tableP14.Rows.Add("P14A", proj.P14A);
            tableP14.Rows.Add("P14B", proj.P14B);
            tableP14.Rows.Add("P14C", proj.P14C);
            tableP14.Rows.Add("P14D", proj.P14D);
            Enumerable.Range(-3, 5).ToList().ForEach(x => tableP14.Years.Add(new YearDefinition(x, true, false)));
            Enumerable.Range(2, 9).ToList().ForEach(x => tableP14.Years.Add(new YearDefinition(x, false, true)));
            tableP14.Render();
            tableP14.UserInput += (sender, e) => UpdateData();
            panel[5].Children.Add(tableP14);

            //---------------------------------表2.15--------------------------------------------------
            YearwiseTable tableP15 = new YearwiseTable { SectionNumber = "2.15", Title = "新增优惠贷款 New Preferential Loans", Brush_TitleCell = _titleBrush };
            tableP15.Rows.Add("P15A", proj.P15A);
            tableP15.Rows.Add("P15B", proj.P15B);
            tableP15.Rows.Add("P15C", proj.P15C);
            tableP15.Rows.Add("P15D", proj.P15D);
            Enumerable.Range(-3, 5).ToList().ForEach(x => tableP15.Years.Add(new YearDefinition(x, true, false)));
            Enumerable.Range(2, 9).ToList().ForEach(x => tableP15.Years.Add(new YearDefinition(x, false, true)));
            tableP15.Render();
            tableP15.UserInput += (sender, e) => UpdateData();
            panel[5].Children.Add(tableP15);

            //---------------------------------表2.16--------------------------------------------------
            YearwiseTable tableP16 = new YearwiseTable { SectionNumber = "2.16", Title = "额外收入预估值 Estimated Additional Revenues", Brush_TitleCell = _titleBrush };
            tableP16.Rows.Add("P16A", proj.P16A);
            tableP16.Rows.Add("P16B", proj.P16B);
            Enumerable.Range(-3, 5).ToList().ForEach(x => tableP16.Years.Add(new YearDefinition(x, true, false)));
            Enumerable.Range(2, 9).ToList().ForEach(x => tableP16.Years.Add(new YearDefinition(x, false, true)));
            tableP16.Render();
            tableP16.UserInput += (sender, e) => UpdateData();
            panel[5].Children.Add(tableP16);

            //---------------------------------表2.17--------------------------------------------------
            YearwiseTable tableP17 = new YearwiseTable { SectionNumber = "2.17", Title = "额外支出预估值 Estimated Additional Expenditures", Brush_TitleCell = _titleBrush };
            tableP17.Rows.Add("P17A", proj.P17A);
            tableP17.Rows.Add("P17B", proj.P17B);
            Enumerable.Range(-3, 5).ToList().ForEach(x => tableP17.Years.Add(new YearDefinition(x, true, false)));
            Enumerable.Range(2, 9).ToList().ForEach(x => tableP17.Years.Add(new YearDefinition(x, false, true)));
            tableP17.Render();
            tableP17.UserInput += (sender, e) => UpdateData();
            panel[5].Children.Add(tableP17);

            //---------------------------------表2.18--------------------------------------------------
            YearwiseTable tableP18 = new YearwiseTable { SectionNumber = "2.18", Title = "ORIGINAL BUDGET FORECAST (FROM FORECAST SHEET)", Brush_TitleCell = _titleBrush };
            tableP18.Rows.Add("P18A", proj.P18A);
            tableP18.Rows.Add("P18B", proj.P18B);
            tableP18.Rows.Add("P18C", proj.P18C);
            tableP18.Rows.Add("P18D", proj.P18D);
            tableP18.Rows.Add("P18E", proj.P18E);
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableP18.Years.Add(new YearDefinition(x, false, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableP18.Years.Add(new YearDefinition(x, false, true)));
            tableP18.Render();
            panel[6].Children.Add(tableP18);

            //---------------------------------表2.19--------------------------------------------------
            YearwiseTable tableP19 = new YearwiseTable { SectionNumber = "2.19", Title = "PROJECT IMPACT ON BUDGET FORECAST", Brush_TitleCell = _titleBrush };
            tableP19.Rows.Add("P19A", proj.P19A);
            tableP19.Rows.Add("P19B", proj.P19B);
            tableP19.Rows.Add("P19C", proj.P19C);
            tableP19.Rows.Add("P19D", proj.P19D);
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableP19.Years.Add(new YearDefinition(x, false, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableP19.Years.Add(new YearDefinition(x, false, true)));
            tableP19.Render();
            panel[6].Children.Add(tableP19);

            //---------------------------------图表1--------------------------------------------------
            var chart1 = ChartHelper.GetChart("GRAPH 1: IMPACT OF PROJECT ON LOCAL GOVERNMENT INVESTMENT CAPACITY",
                ChartHelper.ColumnSeries("预计预算", Enumerable.Range(doc.City.C02 - 3, 14).Select(x => new { Key = x.ToString(), Value = NaNto0(proj.P18D[x]) }).ToArray(), Brushes.Silver),
                ChartHelper.ColumnSeries("项目影响", Enumerable.Range(doc.City.C02 - 3, 14).Select(x => new { Key = x.ToString(), Value = NaNto0(proj.P19D[x]) }).ToArray(), Brushes.YellowGreen));
            chart1.Width = 800;
            panel[7].Children.Add(chart1);

            //---------------------------------表2.20--------------------------------------------------
            YearwiseTable tableP20 = new YearwiseTable { SectionNumber = "2.20", Title = "重要投资资金来源 Funding Source Capital Investment", Brush_TitleCell = _titleBrush };
            tableP20.Rows.Add("P20A", proj.P20A);
            tableP20.Rows.Add("P20B", proj.P20B);
            tableP20.Rows.Add("P20C", proj.P20C);
            tableP20.Rows.Add("P20D", proj.P20D);
            tableP20.Rows.Add("P20E", proj.P20E);
            tableP20.Rows.Add("P20F", proj.P20F);
            tableP20.Rows.Add("P20G", proj.P20G);
            Enumerable.Range(1, 9).ToList().ForEach(x => tableP20.Years.Add(new YearDefinition(x, false, true)));
            tableP20.Years.Add(YearDefinition.Sum);
            tableP20.Render();
            panel[8].Children.Add(tableP20);

            //---------------------------------表2.21--------------------------------------------------
            YearwiseTable tableP21 = new YearwiseTable { SectionNumber = "2.21", Title = "预估支出限额 Estimated Expenditure Limit", Brush_TitleCell = _titleBrush };
            tableP21.Rows.Add("P21A", proj.P21A);
            tableP21.Rows.Add("P21B", proj.P21B);
            tableP21.Rows.Add("P21C", proj.P21C);
            Enumerable.Range(1, 9).ToList().ForEach(x => tableP21.Years.Add(new YearDefinition(x, false, true)));
            tableP21.Render();
            panel[8].Children.Add(tableP21);

            //---------------------------------表2.22--------------------------------------------------
            YearwiseTable tableP22 = new YearwiseTable { SectionNumber = "2.22", Title = "预估最大负债 Estimated Debt Ceiling", Brush_TitleCell = _titleBrush };
            tableP22.Rows.Add("P22A", proj.P22A);
            tableP22.Rows.Add("P22B", proj.P22B);
            tableP22.Rows.Add("P22C", proj.P22C);
            Enumerable.Range(1, 9).ToList().ForEach(x => tableP22.Years.Add(new YearDefinition(x, false, true)));
            tableP22.Render();
            panel[8].Children.Add(tableP22);

            //---------------------------------图表2--------------------------------------------------
            var chart2 = ChartHelper.GetMyChart("GRAPH 2: CAPITAL INVESTMENT REQUIRED VERSUS EXPENDITURE CEILING",
                ChartHelper.MyColumnSeries("现有支出", Enumerable.Range(doc.City.C02 + 1, 9).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(proj.P21B[x]))).ToArray(), StackedChart.BuildGradient(Colors.YellowGreen)),
                ChartHelper.MyColumnSeries("资本投资", Enumerable.Range(doc.City.C02 + 1, 9).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(proj.P21A[x]))).ToArray(), StackedChart.BuildGradient(Color.FromRgb(153, 51, 102))),
                ChartHelper.MyLineSeries("支出限额", Enumerable.Range(doc.City.C02 + 1, 9).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(proj.P21C[x]))).ToArray(), Brushes.Red));
            panel[9].Children.Add(chart2);

            //---------------------------------图表3--------------------------------------------------
            var chart3 = ChartHelper.GetMyChart("GRAPH 3: LOANS REQUIRED VERSUS DEBT SERVICE CAPACITY",
                ChartHelper.MyColumnSeries("还本付息", Enumerable.Range(doc.City.C02 + 1, 9).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(proj.P22B[x]))).ToArray(), StackedChart.BuildGradient(Color.FromRgb(23, 55, 94))),
                ChartHelper.MyColumnSeries("新增贷款", Enumerable.Range(doc.City.C02 + 1, 9).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(proj.P22A[x]))).ToArray(), StackedChart.BuildGradient(Colors.Orange)),
                ChartHelper.MyLineSeries("最大偿力", Enumerable.Range(doc.City.C02 + 1, 9).Select(x => new KeyValuePair<string, double>(x.ToString(), NaNto0(proj.P22C[x]))).ToArray(), Brushes.Red));
            panel[10].Children.Add(chart3);

            panel[11].Children.Add(new TextBlock { Text = "Include project in PIP?", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            ComboBox cbbPipBottom = new ComboBox { Width = 200, HorizontalAlignment = HorizontalAlignment.Left, ItemsSource = new string[] { "Yes", "No" } };
            cbbPipBottom.SetBinding(ComboBox.SelectedIndexProperty, pipBinding);
            panel[11].Children.Add(cbbPipBottom);

            panel[11].Children.Add(new TextBlock { Text = "Go to...", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 0) });
            Button btnFinancial = new Button { Content = "点击转到总结表单", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            btnFinancial.Click += (sender, e) => NavigationManager.Navigate("SummaryPage.xaml");
            panel[11].Children.Add(btnFinancial);
            Button btnNext = new Button { Content = "下一项目", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            btnNext.Click += (sender, e) =>
            {
                ReadyProject((_projId + 1) % doc.Projects.Count);
                //Pager.CurrentSlideNumber = 0;
            };
            panel[11].Children.Add(btnNext);

            _tables = new List<IUpdateTable> { tableP2, tableP3, tableP5, tableP6, tableP7, tablePS, tableP14, tableP15, tableP16, tableP17, tableP18, tableP19, tableP20, tableP21, tableP22 };

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

        public void UpdateData()
        {
            _tables.ForEach(x => x.UpdateData());
        }

        private RoutedEventHandler GetTextBoxLostFocusHandler(TextBox tb, Func<bool> validator, Action actionOnSuccess, string valueToRestoreOnFailure)
        {
            return (sender, e) =>
            {
                if (validator())
                {
                    actionOnSuccess();
                }
                else
                {
                    tb.Text = valueToRestoreOnFailure;
                }
            };
        }

        private Func<bool> GetValidator(TextBox tb)
        {
            return () => tb.Text.Length > 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var idString = NavigationManager.GetQueryString("id");
            if (idString != string.Empty)
            {
                _projId = Convert.ToInt32(idString);
            }
            var slideString = NavigationManager.GetQueryString("slide");
            if (slideString != string.Empty)
            {
                _slidePos = Convert.ToInt32(slideString);
            }
            ReadyProject(_projId, _slidePos);
        }

        public void ReadyProject(int id, int slidePos = 0)
        {
            _projId = id;
            Pager.Slides.Clear();
            GetPages().ForEach(x => Pager.Slides.Add(x));
            Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.Project(x)).ToList();
            Pager.ReadyControl(slidePos);
        }

        public void SetProj(int id)
        {
            _projId = id;
            Pager.Slides.Clear();
            GetPages().ForEach(x => Pager.Slides.Add(x));
            Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.Project(x)).ToList();
            Pager.ReadyControl(_slidePos);
        }

        public void SetSlidePos(int slidePos)
        {
            _slidePos = slidePos;
            Pager.CurrentSlideNumber = slidePos;
        }

        private void Pager_SlideChanged(object sender, EventArgs e)
        {
            _slidePos = Pager.CurrentSlideNumber;
            banner.Caption = DocumentManager.CurrentDocument.Projects[_projId].P1A + " - " + Caption.Project(Pager.CurrentSlideNumber);
        }
    }
}
