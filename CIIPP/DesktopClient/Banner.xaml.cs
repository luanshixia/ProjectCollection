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

namespace DesktopClient
{
    /// <summary>
    /// Banner.xaml 的交互逻辑
    /// </summary>
    public partial class Banner : UserControl
    {
        public static Banner Current { get; private set; }

        public Banner()
        {
            InitializeComponent();

            Current = this;
        }

        private void btnNav_MouseEnter(object sender, MouseEventArgs e)
        {
            popNav.IsOpen = true;
            popNav.MouseLeave += (s, arg) => popNav.IsOpen = false;
        }

        private void btnFunc_MouseEnter(object sender, MouseEventArgs e)
        {
            popFunc.IsOpen = true;
            popFunc.MouseLeave += (s, arg) => popFunc.IsOpen = false;
        }

        private void btnProj_MouseEnter(object sender, MouseEventArgs e)
        {
            BannerProj.Current.UpdateProjList();
            popProj.IsOpen = true;
            popProj.MouseLeave += (s, arg) => popProj.IsOpen = false;
        }

        public void CloseAllPopups()
        {
            foreach (var ele in grid1.Children)
            {
                if (ele is System.Windows.Controls.Primitives.Popup)
                {
                    (ele as System.Windows.Controls.Primitives.Popup).IsOpen = false;
                }
            }
        }

        public string Caption
        {
            get
            {
                return tbTitle.Text;
            }
            set
            {
                tbTitle.Text = value;
            }
        }

        public string Caption0
        {
            get
            {
                return tbTitle0.Text;
            }
            set
            {
                tbTitle0.Text = value;
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationManager.Navigate("RichMainPage.xaml");
        }

        private void ImageButton_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.ToggleFullscreen();
        }
    }
}
