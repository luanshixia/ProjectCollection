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
    /// RichCityPage.xaml 的交互逻辑
    /// </summary>
    public partial class RichCityPage : Page
    {
        private List<IUpdateTable> _tables = new List<IUpdateTable>();
        private static int _slidePos = 0;
        public static RichCityPage Current { get; private set; }

        public RichCityPage()
        {
            InitializeComponent();
            Current = this;
        }

        public List<StackPanel> GetPages()
        {
            List<StackPanel> pages = Enumerable.Range(0, 6).Select(x => new StackPanel()).ToList();

            Document doc = DocumentManager.CurrentDocument;

            //pages[0].Children.Add(new TextBlock { Text = "LOCAL GOVERNMENT KEY DATA", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            PropertyTable tableC0 = new PropertyTable { SectionNumber = "1.0", Title = "当地政府关键数据 LOCAL GOVERNMENT KEY DATA" };
            tableC0.Rows.Add("C01", Tuple.Create<object, object>(doc.City, doc.City.C01));
            tableC0.Rows.Add("C02", Tuple.Create<object, object>(doc.City, DateTime.Now.Year));
            tableC0.Rows.Add("C03", Tuple.Create<object, object>(doc.City, 0));
            tableC0.Rows.Add("C04", Tuple.Create<object, object>(doc.City, 0.0));
            tableC0.Rows.Add("C05", Tuple.Create<object, object>(doc.City, DateTime.Now.Date));
            tableC0.LockRows("C02");
            tableC0.SetPercentageRows("C04");
            tableC0.Render();
            tableC0.UserInput += (sender, e) => UpdateData();
            pages[0].Children.Add(tableC0);
            IntroPage IntroP = new IntroPage { ControlBack = FindResource("green_banner") as LinearGradientBrush, SectionCaptionBack = Brushes.DarkOliveGreen, HorizontalAlignment=System.Windows.HorizontalAlignment.Left };
            pages[0].Children.Add(IntroP);

            //pages[1].Children.Add(new TextBlock { Text = "LOCAL GOVERNMENT FISCAL ASSESSMENT", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold });
            YearwiseTable tableC1 = new YearwiseTable { SectionNumber = "1.1", Title = "当地政府收入 Local Government Revenues" };
            tableC1.Rows.Add("C1A", doc.City.C1A);
            tableC1.Rows.Add("C1B", doc.City.C1B);
            tableC1.Rows.Add("C1C", doc.City.C1C);
            tableC1.Rows.Add("C1D", doc.City.C1D);
            tableC1.Rows.Add("C1E", doc.City.C1E);
            tableC1.Rows.Add("C1F", doc.City.C1F);
            tableC1.Rows.Add("C1G", doc.City.C1G);
            tableC1.Rows.Add("C1H", doc.City.C1H);
            tableC1.Rows.Add("C1I", doc.City.C1I);
            tableC1.LockRows("C1F", "C1G", "C1H", "C1I");
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC1.Years.Add(new YearDefinition(x, true, false)));
            tableC1.Years.Add(YearDefinition.Average);
            tableC1.SetPercentageRows("C1G", "C1H", "C1I");
            tableC1.Render();
            tableC1.UserInput += (sender, e) => UpdateData();
            pages[1].Children.Add(tableC1);

            YearwiseTable tableC2 = new YearwiseTable { SectionNumber = "1.2", Title = "当地政府支出 Local Government Expenditures" };
            tableC2.Rows.Add("C2A", doc.City.C2A);
            tableC2.Rows.Add("C2B", doc.City.C2B);
            tableC2.Rows.Add("C2C", doc.City.C2C);
            tableC2.Rows.Add("C2D", doc.City.C2D);
            tableC2.Rows.Add("C2E", doc.City.C2E);
            tableC2.Rows.Add("C2F", doc.City.C2F);
            tableC2.LockRows("C2C", "C2D", "C2E", "C2F");
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC2.Years.Add(new YearDefinition(x, true, false)));
            tableC2.Years.Add(YearDefinition.Average);
            tableC2.SetPercentageRows("C2E", "C2F");
            tableC2.Render();
            tableC2.UserInput += (sender, e) => UpdateData();
            pages[2].Children.Add(tableC2);

            YearwiseTable tableC3 = new YearwiseTable { SectionNumber = "1.3", Title = "当地政府资产 Local Government Assets" };
            tableC3.Rows.Add("C3A", doc.City.C3A);
            tableC3.Rows.Add("C3B", doc.City.C3B);
            tableC3.Rows.Add("C3C", doc.City.C3C);
            tableC3.Rows.Add("C3D", doc.City.C3D);
            tableC3.Rows.Add("C3E", doc.City.C3D);
            tableC3.LockRows("C3E");
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC3.Years.Add(new YearDefinition(x, true, false)));
            tableC3.Years.Add(YearDefinition.Average);
            tableC3.Render();
            tableC3.UserInput += (sender, e) => UpdateData();
            pages[3].Children.Add(tableC3);

            YearwiseTable tableC4 = new YearwiseTable { SectionNumber = "1.4", Title = "当地政府债务 Debt Service Expenditures" };
            tableC4.Rows.Add("C4A", doc.City.C4A);
            tableC4.Rows.Add("C4B", doc.City.C4B);
            tableC4.Rows.Add("C4C", doc.City.C4C);
            tableC4.Rows.Add("C4D", doc.City.C4D);
            tableC4.LockRows("C4D");
            Enumerable.Range(-3, 4).ToList().ForEach(x => tableC4.Years.Add(new YearDefinition(x, true, false)));
            Enumerable.Range(1, 10).ToList().ForEach(x => tableC4.Years.Add(new YearDefinition(x, true, true)));
            tableC4.Render();
            tableC4.UserInput += (sender, e) => UpdateData();
            pages[4].Children.Add(tableC4);

            pages[5].Children.Add(new TextBlock { Text = "1.5 LOCAL GOVERNMENT FINANCIAL SYSTEM ASSESSMENT", Foreground = Brushes.DarkRed });
            //Button btnToQ = new Button { Content = "打开问卷(总结模式)", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            //btnToQ.Click += (sender, e) => NavigationManager.Navigate("Questionnaire.xaml");
            //pages[5].Children.Add(btnToQ);
            Button btnToQ1 = new Button { Content = "打开问卷(演示模式)", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            btnToQ1.Click += (sender, e) => NavigationManager.Navigate("QuestionPage.xaml", "type=city");

            pages[5].Children.Add(btnToQ1);

            pages[5].Children.Add(new TextBlock { Text = "1.6 and Others...", Foreground = Brushes.DarkRed, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 0) });
            Button btnFinancial = new Button { Content = "点击以转到城市财力评估", Width = 200, HorizontalAlignment = HorizontalAlignment.Left };
            btnFinancial.Click += (sender, e) => NavigationManager.Navigate("RichFinancialPage.xaml");
            pages[5].Children.Add(btnFinancial);

            _tables = new List<IUpdateTable> { tableC0, tableC1, tableC2, tableC3, tableC4 };

            return pages;
        }

        public void UpdateData()
        {
            _tables.ForEach(x => x.UpdateData());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var slideString = NavigationManager.GetQueryString("slide");
            if (slideString != string.Empty)
            {
                _slidePos = Convert.ToInt32(slideString);
            }

            GetPages().ForEach(x => Pager.Slides.Add(x));
            Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.City(x)).ToList();
            Pager.ReadyControl(_slidePos);
        }

        private void Pager_SlideChanged(object sender, EventArgs e)
        {
            _slidePos = Pager.CurrentSlideNumber;
            banner.Caption = Caption.City(_slidePos);
        }

        public void SetSlidePos(int slidePos)
        {
            _slidePos = slidePos;
            Pager.CurrentSlideNumber = slidePos;
        }
    }
}
