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
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingPage : UserControl
    {
        public SettingPage()
        {
            InitializeComponent();

            CityStatistics.CurrencyEnum.ToList().ForEach(x => this.comboBox1.Items.Add(x));
            CityStatistics.MultipleEnum.ToList().ForEach(x => this.comboBox2.Items.Add(x));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = DocumentManager.CurrentDocument.City;
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
