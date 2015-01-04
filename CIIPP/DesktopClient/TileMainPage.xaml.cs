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
using System.Windows.Media.Animation;
using CIIPP;

using System.Windows.Controls.Primitives;

namespace DesktopClient
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class TileMainPage : Page
    {
        public static TileMainPage Current { get; private set; }
        
        public TileMainPage()
        {            
            InitializeComponent();
            this.Background = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "底图.jpg")));
            Current = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FileBasedUpdate();
        }

        public void FileBasedUpdate()
        {            
            Pager.Slides.Clear();
            Pager.Slides.Add(new TileTaskPage());
            //Pager.Slides.Add(new TileProject());
            //Pager.Slides.Add(new IntroPage());
            Pager.Slides.Add(new SettingPage());
            Pager.ReadyControl();
        }
    }
}
