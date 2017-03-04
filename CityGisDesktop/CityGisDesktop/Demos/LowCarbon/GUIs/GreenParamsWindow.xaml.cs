using System.Windows;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// GreenParamsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GreenParamsWindow : Window
    {
        public GreenParamsWindow()
        {
            InitializeComponent();
            GreenParamsGrid.DataContext = Parameter.Green;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            (MainWindow.DemoInstance as Demo).RefreshComputation();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
