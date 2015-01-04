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
    /// RichMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class RichMainPage : Page
    {
        public static RichMainPage Current { get; private set; }

        public RichMainPage()
        {
            InitializeComponent();

            Current = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            FileBasedUpdate();
        }

        public void FileBasedUpdate()
        {
            Pager.Slides.Clear();
            Pager.Slides.Add(new TaskPage());
            Pager.Slides.Add(new IntroPage { ControlBack = FindResource("green_banner") as LinearGradientBrush, SectionCaptionBack = Brushes.DarkOliveGreen });
            Pager.Slides.Add(new SettingPage { ControlBack = FindResource("green_banner") as LinearGradientBrush, SectionCaptionBack = Brushes.DarkOliveGreen });
            Pager.NavBar.PointDescripions = new List<string> { "导航", "介绍", "设置" };
            Pager.ReadyControl();
        }
    }
}
