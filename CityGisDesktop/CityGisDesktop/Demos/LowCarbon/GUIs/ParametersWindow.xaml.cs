using Dreambuild.Gis.Display;
using System.Windows;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// ParametersWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ParametersWindow : Window
    {
        private TripParams backup;

        public ParametersWindow()
        {
            InitializeComponent();

            backup = new TripParams(Parameter.Trip);
            TheGrid.DataContext = Parameter.Trip;
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
                var model = new LcComputation(MapDataManager.LatestMap, Demo.CaculateType);
            if (backup.PerCapitaIndustry != Parameter.Trip.PerCapitaIndustry)
            {
                model.SetPerCapitaIndustry(Parameter.Trip);              
            }
            if (backup.PerCapitaOffice != Parameter.Trip.PerCapitaOffice)
            {
                model.SetPerCapitaOffice(Parameter.Trip);                              
            }
            if(backup.PerCapitaResidencial != Parameter.Trip.PerCapitaResidencial)
            {
                model.SetPerCapitaResidencial(Parameter.Trip);
            }
            (MainWindow.DemoInstance as Demo).RefreshComputation();

            this.Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Parameter.Trip = backup;
            this.Close();
        }

    }
}
