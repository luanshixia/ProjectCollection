using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Media.Media3D;
using TongJi.Gis.Display;
using HelixToolkit.Wpf;

namespace CityInfographics
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChartScene2 _scene03;
        private RegionPoints _rp;
        private int _dataI = 0;
        //private TimeRecordList _timeData = new TimeRecordList("Data\\time_distribution.csv");

        public MainWindow()
        {
            InitializeComponent();

            App.View3D = this;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (DateTime.Now > DateTime.Parse("2099-5-30"))
            {
                MessageBox.Show("该演示版本已过期，请更新到新版本，或联系软件作者。");
                App.Current.Shutdown();
            }

            //SlidersPanel sp = new SlidersPanel();
            //sp.ReportResult += sp_ReportResult;
            //AddPanel("Time", sp);

            MapDataManager.Open("Data\\new_building.ciml");

            ReadyScene03();
            //var extents = MapDataManager.LatestMap.GetExtents();
            AddImageBase("Data\\energy.png", 46418, 23946, 49155, 27895); //extents.min.x, extents.min.y, extents.max.x, extents.max.y);
            lh.Total = 7 * 24; //11
        }

        void sp_ReportResult(TimePoint obj)
        {
            UpdateScene03(obj.Month, obj.Month, obj.Hour, _dataI);
        }

        //private void Run()
        //{
        //    MapControl mc = new MapControl();
        //    Grid grid = new Grid();
        //    grid.Width = 800;
        //    grid.Height = 800;
        //    grid.Children.Add(mc);
        //    BasePlane.Visual = grid;

        //    MapDataManager.Open("Data\\base.ciml");
        //    mc.InitializeMap(MapDataManager.LatestMap);
        //}

        private void ClearScene()
        {
            TheViewport.Children.Clear();
            TheViewport.Children.Add(new SunLight());
        }

        private void ReadyScene01()
        {
            ChartScene scene = new ChartScene(TheViewport, 10, 10);
        }

        private void ReadyScene02()
        {
            BlockScene scene = new BlockScene(TheViewport, MapDataManager.LatestMap.Layers["building"].Features);
        }

        private void ReadyScene03()
        {
            _rp = new RegionPoints(MapDataManager.LatestMap);
            _scene03 = new ChartScene2(TheViewport, _rp.Points.Keys.ToList(), RegionPoints.CellSize);
        }

        private void UpdateScene03(int m, int d, int h, int i)
        {
            if (_rp != null)
            {
                //double value = _timeData.GetValue(m, d, h, i);
                var heights = _rp.Points.Values.Select(f =>
                {
                    string code = f["c"];
                    double area = _rp.BuildingAreas.ContainsKey(code) ? _rp.BuildingAreas[code] : 0; ;
                    return _rp.GetValue(f, m, d, h, i) * area * 5;
                    //f["f"].TryParseToInt32() * RegionPoints.CellSize * RegionPoints.CellSize * value / 10;
                }).ToArray();
                _scene03.UpdateBoxes(heights);
            }
        }

        //private void TestMap()
        //{
        //    MainWindow2D mw2d = new MainWindow2D();
        //    mw2d.Show();
        //    this.Hide();
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClearScene();
            ReadyScene03();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (App.View2D == null)
            {
                new MainWindow2D();
            }
            App.View2D.Show();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            //App.Current.Shutdown();
        }

        //public void AddPanel(string name, UserControl panelControl)
        //{
        //    Expander exp = new Expander { Header = name, Margin = new Thickness(5), Padding = new Thickness(5), Background = new SolidColorBrush(VectorStyleManager.ParseColor("#444444")), BorderThickness = new Thickness(0), IsExpanded = true };
        //    exp.Content = panelControl;
        //    PanelStack.Children.Add(exp);
        //}

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ClearScene();
            ReadyScene02();
        }

        private void SwitchControl_Switched_1(SwitchControl.SwitchSide obj)
        {
            if (obj == SwitchControl.SwitchSide.Left)
            {
                _dataI = 0;
            }
            else if (obj == SwitchControl.SwitchSide.Right)
            {
                _dataI = 1;
            }
        }

        private void TextBlock_MouseLeftButtonUp_1(object sender, MouseButtonEventArgs e)
        {
            //FluidWindow fw = new FluidWindow();
            //fw.Show();
        }

        private void AutoPlay() // newly 20130415
        {
            System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //int month = (int)sp.MonthSlider.Value;
            //month = month + 1;
            //month = month > 12 ? month - 12 : month;
            //sp.MonthSlider.Value = month;
            //lh.SetValue(month - 1);

            int day = (int)sp.DaySlider.Value;
            int hour = (int)sp.HourSlider.Value;
            hour = hour + 1;
            if (hour > 24)
            {
                hour = hour - 24;
                day = day + 1;
            }
            sp.HourSlider.Value = hour;
            sp.DaySlider.Value = day;
            lh.SetValue((day - 1) * 24 + hour);
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AutoPlay();
        }

        private void AddImageBase(string source, double minx, double miny, double maxx, double maxy)
        {
            double width = maxx - minx;
            double height = maxy - miny;
            double cx = (minx + maxx) / 2;
            double cy = (miny + maxy) / 2;
            BitmapImage bitmap = new BitmapImage(new Uri(source, UriKind.Relative));
            ImageBrush brush = new ImageBrush(bitmap) { RelativeTransform = new RotateTransform(90), TileMode = TileMode.Tile };
            RectangleVisual3D rect = new RectangleVisual3D { Length = width, Width = height, Origin = new Point3D(cx, cy, 0), Fill = brush };
            TheViewport.Children.Add(rect);
        }
    }
}
