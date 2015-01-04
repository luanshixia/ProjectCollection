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
    /// TestPage.xaml 的交互逻辑
    /// </summary>
    public partial class TestPage : Page
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Document doc = Document.New();
            yearwiseTable1.SectionNumber = "1.1";
            yearwiseTable1.Title = "Hello World";
            yearwiseTable1.Rows.Add("C1A", doc.City.C1A);
            yearwiseTable1.Rows.Add("C1B", doc.City.C1B);
            yearwiseTable1.Years.Add(new YearDefinition(2007, true, false));
            yearwiseTable1.Years.Add(new YearDefinition(2008, true, false));
            yearwiseTable1.Years.Add(new YearDefinition(2009, true, false));
            yearwiseTable1.Years.Add(new YearDefinition(2010, true, false));
            yearwiseTable1.Years.Add(new YearDefinition(2011, false, true));
            yearwiseTable1.Years.Add(new YearDefinition(2012, false, true));
            yearwiseTable1.Years.Add(new YearDefinition(2013, false, true));
            yearwiseTable1.Years.Add(new YearDefinition(2014, false, true));
            yearwiseTable1.Render();

            testList.ReadyAnimation();

            //NavBar.ReadyControl(5);
            //NavBar.CurrentPoint = 0;

            pager.Slides.Add(new TextBlock { Text = "A page." });
            pager.Slides.Add(new TextBlock { Text = "A page." });
            pager.Slides.Add(new TextBlock { Text = "A page." });
            pager.Slides.Add(new TextBlock { Text = "A page." });
            pager.Slides.Add(new TextBlock { Text = "A page." });
            pager.Slides.Add(new TextBlock { Text = "A page." });
            pager.ReadyControl();

            tileGridControl1.Tiles = Enumerable.Range(0, 13).Select(x =>
            {
                return new Ellipse { Width = 150, Height = 100, Fill = Brushes.Black };
            }).ToList();
        }
    }
}
