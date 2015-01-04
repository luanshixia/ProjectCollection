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
    /// TileContentPage.xaml 的交互逻辑
    /// </summary>
    public partial class TileContentPage : Page
    {
        public static TileContentPage Current { get; private set; }
        private double _navWidth = 171;
        private double _sensorWidth = 200;

        private int _currentPart;

        public int CurrentPart
        {
            get
            {
                return _currentPart;
            }
            set
            {
                _currentPart = value;
                OnCurrentPartChanged(new EventArgs());
            }
        }

        public event EventHandler CurrentPartChanged;

        public void OnCurrentPartChanged(EventArgs e)
        {
            if (CurrentPartChanged != null)
            {
                CurrentPartChanged(this, e);
            }
        }

        public TileContentPage()
        {
            InitializeComponent();
            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "底图.jpg")));
            Current = this;
            //btnFullscreen.SetBinding(FrameworkElement.ToolTipProperty, new Binding("FullscreenToolTip") { Source = MainWindow.Current });
            Pager.SlideChanged += new EventHandler(Pager_SlideChanged);
        }

        void Pager_SlideChanged(object sender, EventArgs e)
        {
            Pager.ScrollToTop();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var btn1 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-11-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-11-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-11-1.png", UriKind.Relative)) };
            var btn2 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-12-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-12-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-12-1.png", UriKind.Relative)) };
            var btn3 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-13-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-13-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-13-1.png", UriKind.Relative)) };
            var btn4 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-14-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-14-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-14-1.png", UriKind.Relative)) };
            var btn5 = new ImageButton { Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-15-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-15-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-15-1.png", UriKind.Relative)) };

            btn1.Click += (s, arg) => NavigationManager.Navigate("TileSecondaryPage.xaml", "part=0"); ;
            btn2.Click += (s, arg) => NavigationManager.Navigate("TileSecondaryPage.xaml", "part=1");
            btn3.Click += (s, arg) => NavigationManager.Navigate("TileSecondaryPage.xaml", "part=2");
            btn4.Click += (s, arg) => NavigationManager.Navigate("TileSecondaryPage.xaml", "part=3");
            btn5.Click += (s, arg) => NavigationManager.Navigate("TileMainPage.xaml");
            var btns = new ImageButton[] { btn1, btn2, btn3, btn4, btn5 };
            tileStackControl1.Tiles = btns;
            foreach (var btn in btns)
            {
                var btn000 = btn;
                btn000.Click += (s, arg) =>
                {
                    CurrentPart = btns.ToList().IndexOf(btn000);
                    btns.ToList().ForEach(x => x.ToggleOff());
                    btn000.ToggleOn();
                };
            }

            var partString = NavigationManager.GetQueryString("part");
            if (partString == "city" || partString == "city_q")
            {
                CurrentPart = 0;
                btn1.ToggleOn();
            }
            else if (partString == "financial")
            {
                CurrentPart = 1;
                btn2.ToggleOn();
            }
            else if (partString == "project")
            {
                CurrentPart = 2;
                btn3.ToggleOn();
            }
            else if (partString == "pip" || partString == "summary")
            {
                CurrentPart = 3;
                btn4.ToggleOn();
            }
            this.ReadyContent();
        }

        public void ReadyContent()
        {
            var part = NavigationManager.GetQueryString("part");
            int slide = 0;
            var slideString = NavigationManager.GetQueryString("slide");
            if (slideString != string.Empty)
            {
                slide = Convert.ToInt32(slideString);
            }
            int id = 0;
            var idString = NavigationManager.GetQueryString("id");
            if (idString != string.Empty)
            {
                id = Convert.ToInt32(idString);
            }

            if (part == "city")
            {
                RichCityPage richPage = new RichCityPage();
                this.Pager.Slides.Clear();
                richPage.GetPages().ForEach(x => this.Pager.Slides.Add(x));
                Pager.Slides.RemoveAt(Pager.Slides.Count - 1); // 去掉最后一个
                // 问卷
                QuestionPage page = new QuestionPage();
                Pager.Slides.Add(page.GetContent("city", 0));
                Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.City(x)).ToList();
                this.Pager.ReadyControl(slide);
            }
            else if (part == "financial")
            {
                RichFinancialPage richPage = new RichFinancialPage();
                this.Pager.Slides.Clear();
                richPage.GetPages().ForEach(x => this.Pager.Slides.Add(x));
                Pager.Slides.RemoveAt(Pager.Slides.Count - 1); // 去掉最后一个
                Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.Financial(x)).ToList();
                this.Pager.ReadyControl(slide);
            }
            else if (part == "project")
            {
                RichProjectPage richPage = new RichProjectPage();
                richPage.SetProj(id);
                this.Pager.Slides.Clear();
                richPage.GetPages().ForEach(x => this.Pager.Slides.Add(x));
                Pager.Slides.RemoveAt(Pager.Slides.Count - 1); // 去掉最后一个
                Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.Project(x)).ToList();
                this.Pager.ReadyControl(slide);
            }
            else if (part == "pip")
            {
                PipPage richPage = new PipPage();
                this.Pager.Slides.Clear();
                richPage.Update().ForEach(x => this.Pager.Slides.Add(x));
                Pager.NavBar.PointDescripions = Enumerable.Range(0, Pager.Slides.Count).Select(x => Caption.Pip(x)).ToList();
                this.Pager.ReadyControl(slide);
            }
            //else if (part == "city_q")
            //{
            //    QuestionPage page = new QuestionPage();
            //    this.Pager.Slides.Clear();
            //    this.Pager.Slides.Add(page.GetContent("city", 0));
            //    this.Pager.ReadyControl(0);
            //}
            //else if (part == "project_q")
            //{
            //    QuestionPage page = new QuestionPage();
            //    this.Pager.Slides.Clear();
            //    this.Pager.Slides.Add(page.GetContent("project", id));
            //    this.Pager.ReadyControl(0);
            //}
            else if (part == "summary")
            {
                SummaryPage page = new SummaryPage();
                page.Update();
                var content = page.LayoutRoot;
                content.Children.RemoveAt(0);
                content.Children.RemoveAt(content.Children.Count - 2);
                page.grid1.Children.Remove(content);
                this.Pager.Slides.Clear();
                this.Pager.Slides.Add(content);
                this.Pager.ReadyControl(0);
            }
            else if (part == "about")
            {
                AboutPage ap = new AboutPage();
                this.Pager.Slides.Clear();
                this.Pager.Slides.Add(ap);
                this.Pager.ReadyControl(0);
            }
        }

        public List<FrameworkElement> GetPrintContent()
        {
            List<FrameworkElement> list = new List<FrameworkElement>();

            var part = NavigationManager.GetQueryString("part");
            int slide = 0;
            var slideString = NavigationManager.GetQueryString("slide");
            if (slideString != string.Empty)
            {
                slide = Convert.ToInt32(slideString);
            }
            int id = 0;
            var idString = NavigationManager.GetQueryString("id");
            if (idString != string.Empty)
            {
                id = Convert.ToInt32(idString);
            }

            if (part == "city")
            {
                RichCityPage richPage = new RichCityPage();
                richPage.GetPages().ForEach(x => list.Add(x));
                list.RemoveAt(list.Count - 1); // 去掉最后一个
            }
            else if (part == "financial")
            {
                RichFinancialPage richPage = new RichFinancialPage();
                richPage.GetPages().ForEach(x => list.Add(x));
                list.RemoveAt(list.Count - 1); // 去掉最后一个
            }
            else if (part == "project")
            {
                RichProjectPage richPage = new RichProjectPage();
                richPage.SetProj(id);
                richPage.GetPages().ForEach(x => list.Add(x));
                list.RemoveAt(list.Count - 1); // 去掉最后一个
            }
            else if (part == "pip")
            {
                PipPage richPage = new PipPage();
                richPage.Update().ForEach(x =>
                {
                    var fes = x.Children.Cast<FrameworkElement>().ToList();
                    fes.ForEach(y =>
                    {
                        x.Children.Remove(y);
                        list.Add(y);
                    });
                });
            }
            else if (part == "summary")
            {
                SummaryPage richpage = new SummaryPage();
                richpage.Update();
                var content = richpage.LayoutRoot;
                content.Children.RemoveAt(0);
                content.Children.RemoveAt(content.Children.Count - 2);
                richpage.grid1.Children.Remove(content);
                list.Add(content);
            }
            return list;
        }

        private void stack1_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(stack1);
            if (point.X > _sensorWidth || point.X < 0)
            {
                HideNav();
            }
            else
            {
                ShowNav();
            }
        }

        private void HideNav()
        {
            ThicknessAnimation ta = new ThicknessAnimation(new Thickness(-_navWidth, 0, 0, 0), new Duration(TimeSpan.Parse("0:0:0.3")));
            stack1.BeginAnimation(Grid.MarginProperty, ta);
        }

        private void ShowNav()
        {
            ThicknessAnimation ta = new ThicknessAnimation(new Thickness(0, 0, 0, 0), new Duration(TimeSpan.Parse("0:0:0.3")));
            stack1.BeginAnimation(Grid.MarginProperty, ta);
        }

        private void stack1_MouseLeave(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(stack1);
            if (point.X > _sensorWidth || point.X < 0)
            {
                HideNav();
            }
            else
            {
                ShowNav();
            }
        }

        public void PrintContent()
        {
            var list = GetPrintContent();
            list.ForEach(x => x.HorizontalAlignment = System.Windows.HorizontalAlignment.Center);
            var part = NavigationManager.GetQueryString("part");
            if (part == "summary")
            {
                PrintManager.Page = PrintPage.A3_Landscape;
            }
            else
            {
                PrintManager.Page = PrintPage.A3_Portrait;
            }
            var flow = PrintManager.GetDocumentFrom(list);
            var fixedDoc = PrintManager.FlowToFixed(flow);
            PrintPreviewWindow ppw = new PrintPreviewWindow { Owner = MainWindow.Current };
            ppw.SetDocument(fixedDoc);
            ppw.ShowDialog();
        }
    }
}
