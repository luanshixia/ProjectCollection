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
    /// ProjectButton.xaml 的交互逻辑
    /// </summary>
    public partial class ProjectButton : UserControl
    {
        public ProjectButton()
        {
            InitializeComponent();
            p.Background = new ImageBrush(new BitmapImage(new Uri(_normalImage)));
            mouseEnterbg = _hoverImage;
            mouseLeavebg = _normalImage;
        }

        public string ButtonText
        {
            get
            {
                return this.txtContent.Text;
            }
            set
            {
                this.txtContent.Text = value;
            }
        }

        public string buttonState = "nomal";
        private string _normalImage = "pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-16.jpg";
        private string _hoverImage = "pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "part-06.jpg";
        private string _delImage = "pack://application:,,,/DesktopClient;Component/" + MainWindow.ImagePath + "del_proj.png";
        public string mouseEnterbg;
        public string mouseLeavebg;

        public void NormalImage(string image)
        {
            _normalImage = image;
            p.Background = new ImageBrush(new BitmapImage(new Uri(_normalImage)));
            mouseLeavebg = _normalImage;
        }

        public void HoverImage(string image)
        {
            _hoverImage = image;
            mouseEnterbg = _hoverImage;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            this.Background = new ImageBrush(new BitmapImage(new Uri(mouseEnterbg)));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Background = new ImageBrush(new BitmapImage(new Uri(mouseLeavebg)));
        }

        public void ShowDelState()
        {
            mouseEnterbg = _delImage;
            mouseLeavebg = _delImage;
            this.Background = new ImageBrush(new BitmapImage(new Uri(mouseEnterbg)));
            this.buttonState = "delete";
        }

        public void ShowBack()
        {
            mouseEnterbg = _hoverImage;
            mouseLeavebg = _normalImage;
            this.Background = new ImageBrush(new BitmapImage(new Uri(mouseLeavebg)));
            this.buttonState = "nomal";
        }
    }
}
