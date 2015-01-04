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
    /// IntroPage.xaml 的交互逻辑
    /// </summary>
    public partial class IntroPage : UserControl
    {
        public IntroPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = DocumentManager.CurrentDocument.City;
            
            //grid1.DataContext = new TwoColors { Light = "Gray", Dark = "DimGray" };
        }

        public Brush ControlBack
        {
            get
            {
                return border1.Background;
            }
            set
            {
                border1.Background = value;
            }
        }

        public Brush SectionCaptionBack
        {
            get
            {
                return (stack1.Children[0] as TextBlock).Background;
            }
            set
            {
                foreach (var child in stack1.Children)
                {
                    if (child is TextBlock)
                    {
                        (child as TextBlock).Background = value;
                    }
                }
            }
        }
    }
}
