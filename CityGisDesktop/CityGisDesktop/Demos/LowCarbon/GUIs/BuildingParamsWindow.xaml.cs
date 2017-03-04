using System.Windows;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// BuildingParamsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BuildingParamsWindow : Window
    {
        public BuildingParamsWindow()
        {
            InitializeComponent();
            BuildingParamsGrid.DataContext = Parameter.Building;
            ProductionGrid.DataContext = Parameter.Maintenance;
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
