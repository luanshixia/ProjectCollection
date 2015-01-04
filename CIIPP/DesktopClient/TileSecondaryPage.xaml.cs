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
    /// TileSecondaryPage.xaml 的交互逻辑
    /// </summary>
    public partial class TileSecondaryPage : Page
    {
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
            UpdateGrid();
            if (CurrentPartChanged != null)
            {
                CurrentPartChanged(this, e);
            }
        }

        public TileSecondaryPage()
        {
            InitializeComponent();
            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "底图.jpg")));
            //fullscreenbtn.SetBinding(FrameworkElement.ToolTipProperty, new Binding("FullscreenToolTip") { Source = MainWindow.Current });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var btn1 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-11-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-11-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-11-1.png", UriKind.Relative)) };
            var btn2 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-12-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-12-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-12-1.png", UriKind.Relative)) };
            var btn3 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-13-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-13-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-13-1.png", UriKind.Relative)) };
            var btn4 = new ImageButton { IsToggleButton = true, Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-14-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-14-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-14-1.png", UriKind.Relative)) };
            var btn5 = new ImageButton { Width = 171, Height = 78, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-15-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-15-3.png", UriKind.Relative)), DownImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "p2-15-1.png", UriKind.Relative)) };

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

            var idString = NavigationManager.GetQueryString("part");
            if (idString == "0")
            {
                CurrentPart = 0;
                btn1.ToggleOn();
            }
            else if (idString == "1")
            {
                CurrentPart = 1;
                btn2.ToggleOn();
            }
            else if (idString == "2")
            {
                CurrentPart = 2;
                btn3.ToggleOn();
            }
            else if (idString == "3")
            {
                CurrentPart = 3;
                btn4.ToggleOn();
            }
        }

        public void UpdateGrid()
        {
            if (CurrentPart == 0)
            {
                var Cbtn1 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-1-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-1-1.png", UriKind.Relative)) };
                var Cbtn2 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-2-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-2-1.png", UriKind.Relative)) };
                var Cbtn3 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-3-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-3-1.png", UriKind.Relative)) };
                var Cbtn4 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-4-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-4-1.png", UriKind.Relative)) };
                var Cbtn5 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-5-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-5-1.png", UriKind.Relative)) };
                var Cbtn6 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-6-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "21-6-1.png", UriKind.Relative)) };
                tileGridControl1.Tiles = new Button[] { Cbtn1, Cbtn2, Cbtn3, Cbtn4, Cbtn5, Cbtn6 };
                Cbtn1.Click += (s, arg) => TileNav.City(0);
                Cbtn2.Click += (s, arg) => TileNav.City(1);
                Cbtn3.Click += (s, arg) => TileNav.City(2);
                Cbtn4.Click += (s, arg) => TileNav.City(3);
                Cbtn5.Click += (s, arg) => TileNav.City(4);
                Cbtn6.Click += (s, arg) => TileNav.City(5);
                //Cbtn6.Click += (s, arg) => TileNav.Question("city");
            }
            else if (CurrentPart == 1)
            {
                var Fbtn1 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-1-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-1-1.png", UriKind.Relative)) };
                var Fbtn2 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-2-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-2-1.png", UriKind.Relative)) };
                var Fbtn3 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-3-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-3-1.png", UriKind.Relative)) };
                var Fbtn4 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-4-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-4-1.png", UriKind.Relative)) };
                var Fbtn5 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-5-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-5-1.png", UriKind.Relative)) };
                var Fbtn6 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-6-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "22-6-1.png", UriKind.Relative)) };
                tileGridControl1.Tiles = new Button[] { Fbtn1, Fbtn2, Fbtn3, Fbtn4, Fbtn5, Fbtn6 };
                Fbtn1.Click += (s, arg) => TileNav.Financial(0);
                Fbtn2.Click += (s, arg) => TileNav.Financial(1);
                Fbtn3.Click += (s, arg) => TileNav.Financial(2);
                Fbtn4.Click += (s, arg) => TileNav.Financial(3);
                Fbtn5.Click += (s, arg) => TileNav.Financial(4);
                Fbtn6.Click += (s, arg) => TileNav.Financial(5);
            }
            else if (CurrentPart == 2)
            {
                bool readyToDel = false;
                var tiles = DocumentManager.CurrentDocument.Projects.Select(x =>
                {
                    var id = DocumentManager.CurrentDocument.Projects.IndexOf(x);
                    var button = new ProjectButton { ButtonText = x.P1A };
                    SetImage(x.ProjIconId, button);

                    button.MouseLeftButtonUp += (sender, e) =>
                    {
                        if (button.buttonState == "nomal")
                        {
                            TileNav.ProjectById(id);
                        }
                        else if (button.buttonState == "delete")
                        {
                            if (MessageBox.Show("确实要删除该项目吗？", "确认删除", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                DocumentManager.CurrentDocument.Projects.RemoveAt(DocumentManager.CurrentDocument.Projects.IndexOf(x));
                                UpdateGrid();
                            }
                        }
                    };

                    button.MouseRightButtonDown += (sender, e) =>
                    {
                        readyToDel = !readyToDel;
                        if (readyToDel)
                        {
                            foreach (var btn in tileGridControl1.Tiles)
                            {
                                if (btn is ProjectButton)
                                {
                                    (btn as ProjectButton).ShowDelState();
                                }
                            }
                        }
                        else
                        {
                            foreach (var btn in tileGridControl1.Tiles)
                            {
                                if (btn is ProjectButton)
                                {
                                    (btn as ProjectButton).ShowBack();
                                }
                            }
                        }
                    };
                    return button;
                }).ToList<FrameworkElement>();
                ImageButton btnNewProj = new ImageButton { NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "add_proj.png", UriKind.Relative)), Width = 200, Height = 150, Cursor = Cursors.Hand, ToolTip = "新建项目" };
                btnNewProj.Click += (s, arg) =>
                {
                    NewProj np = new NewProj { Owner = MainWindow.Current };
                    if (np.ShowDialog() == true)
                    {
                        ProjectStatistics projStcs = new ProjectStatistics(np.txtProjName.Text, np.txtProjLocation.Text);
                        DocumentManager.CurrentDocument.Projects.Add(projStcs);
                        ProjectImage(np.imageProjId.SelectedIndex, projStcs);

                        UpdateGrid();
                    }
                };
                tiles.Add(btnNewProj);
                tileGridControl1.Tiles = tiles;
            }
            else if (CurrentPart == 3)
            {
                var btn1 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "24-1-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "24-1-1.png", UriKind.Relative)) };
                var btn4 = new ImageButton { Width = 265, Height = 199, Margin = new Thickness(2), NormalImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "24-4-2.png", UriKind.Relative)), HoverImage = new BitmapImage(new Uri(@MainWindow.ImagePath + "24-4-1.png", UriKind.Relative)) };
                tileGridControl1.Tiles = new Button[] { btn4, btn1 };
                btn1.Click += (s, arg) => TileNav.Pip(0);
                btn4.Click += (s, arg) => TileNav.Summary();
            }
            else if (CurrentPart == 4)
            {
                NavigationManager.Navigate("TileMainPage.xaml");
            }
        }

        public void ProjectImage(int imageIndex, ProjectStatistics proj)
        {
            proj.ProjIconId = imageIndex;
        }

        public void SetImage(int Iconid, ProjectButton button)
        {
            button.NormalImage(ProjIconManager.Icons[Iconid]);
            //button.HoverImage(ProjIconManager.GetIcon(Iconid,1));
            //button.HoverImage("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "add_proj.png");
        }
    }
}
