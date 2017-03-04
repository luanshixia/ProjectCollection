using Dreambuild.Gis.Display;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon
{
    /// <summary>
    /// BaseParamsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BaseParamsWindow : Window
    {  
        private bool IsRunning = false;
        public BaseParams baseParams = Parameter.Base;
        private EnergySavingRatio originalEnergySavingRatio;

        public BaseParamsWindow()
        {
            InitializeComponent();

            //fill the window control with initial value
            BaseParamsGrid.DataContext = baseParams;
            CityComboBox.ItemsSource = baseParams.CityNames;
            BaseYearsComboBox.ItemsSource = baseParams.BaseYears;
            CityComboBox.SelectedIndex = baseParams.CitySelectedIndex;
            BaseYearsComboBox.SelectedIndex = baseParams.BaseYearSelectedIndex;

            SavingRatioInit();
        }

        private void SavingRatioInit()
        {
            SavingRatioTextBox.Text = "";
            EnergySavingRatio standard = baseParams.Standard;
            originalEnergySavingRatio = standard;

            switch (standard.EnergyType)
            {
                case EnergySavingType.SavingRatioType:
                    SavingRatioType.IsChecked = true;
                    SavingRatioTextBox.Text = standard.TempValue.ToString();
                    break;
                case EnergySavingType.LeedStandardType:
                    LeedStandardType.IsChecked = true;
                    LEEDComboBox.SelectedIndex = (int) standard.TempValue;
                    break;
                case EnergySavingType.ChineseGreenBuildingStandardType:
                    ChineseGreenBuildingStandardType.IsChecked = true;
                    ChineseGreenBuildingStandar_ComboBox.SelectedIndex = (int) standard.TempValue;
                    break;
                default:
                    throw new Exception("Unknown EnergyType!");
            }

            IsRunning = true;
        }

        private void CityComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            baseParams.CitySelectedIndex = CityComboBox.SelectedIndex;

            if (IsRunning)
            {
                switch (baseParams.CityClimate)
                {
                    case CityClimateType.NorthClimate:
                        baseParams.ElectricityProductionFactor = 0.8115;
                        break;
                    case CityClimateType.MiddleClimate:
                        baseParams.ElectricityProductionFactor = 0.7495;
                        break;
                    case CityClimateType.SouthClimate:
                        baseParams.ElectricityProductionFactor = 0.6323;
                        break;
                    default:
                        Console.WriteLine("error occurred");
                        return;
                }

                MaintenanceParams maintenanceParams = Parameter.Maintenance;
                maintenanceParams.Refresh();
                (MainWindow.DemoInstance as Demo).RefreshComputation();
            }
            ElectricityCO2FactorTextBox.Text = baseParams.ElectricityProductionFactor.ToString();
        }

        private void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            int sum = baseParams.NaturalGasRatio + baseParams.CoalElectricityRatio + baseParams.OtherCleanEnergyRatio;

            if (sum != 100)
            {
                MessageBox.Show("比例之和必须为100");
            }
            else
            {
                this.DialogResult = true;

                if (originalEnergySavingRatio != baseParams.Standard)
                {
                    var model = new LcComputation(MapDataManager.LatestMap, Demo.CaculateType);
                    model.SetEnergySavingRatio(baseParams.Standard);
                }
                (MainWindow.DemoInstance as Demo).RefreshComputation();
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BaseYearsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Todo: update here , get from DB
            baseParams.BaseYear = 2005 + BaseYearsComboBox.SelectedIndex;
            baseParams.BaseYearSelectedIndex = BaseYearsComboBox.SelectedIndex;

            MaintenanceParams maintenanceParams = Parameter.Maintenance;
            maintenanceParams.Refresh();
            (MainWindow.DemoInstance as Demo).RefreshComputation();
        }

        private void Within100IntValidation(object sender, RoutedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            int energySavingRatio;
            try
            {
                energySavingRatio = Convert.ToInt32(text);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("输入格式错误");
                return;
            }

            if (energySavingRatio > 100 || energySavingRatio < 0)
            {
                MessageBox.Show("比例必须介于0-100");
                return;
            }
        }

        private void SavingRatioTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsRunning)
            {
                return;
            }

            string text = (sender as TextBox).Text;
            double energySavingRatio;
            try
            {
                energySavingRatio = Convert.ToDouble(text);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("输入格式错误");
                return;
            }

            if (energySavingRatio > 100 || energySavingRatio < 0)
            {
                MessageBox.Show("比例必须介于0-100");
                return;
            }

            if (SavingRatioType.IsChecked == true)
            {
                baseParams.Standard = new EnergySavingRatio(EnergySavingType.SavingRatioType, energySavingRatio);
            }
        }

        private void LEEDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LeedStandardType.IsChecked == true && IsRunning)
            {
                switch (LEEDComboBox.SelectedIndex)
                {
                    case 0:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 0);
                        break;
                    case 1:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 1);
                        break;
                    case 2:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 2);
                        break;
                    case 3:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 3);
                        break;
                    default:
                        return;
                }
            }
        }

        private void ChineseGreenBuildingStandar_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChineseGreenBuildingStandardType.IsChecked == true && IsRunning)
            {
                switch (ChineseGreenBuildingStandar_ComboBox.SelectedIndex)
                {
                    case 0:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.ChineseGreenBuildingStandardType, 0);
                        break;
                    case 1:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.ChineseGreenBuildingStandardType, 1);
                        break;
                    case 2:
                        baseParams.Standard = new EnergySavingRatio(EnergySavingType.ChineseGreenBuildingStandardType, 2);
                        break;
                    default:
                        return;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!IsRunning)
            {
                return;
            }

            var button = sender as RadioButton;

            EnergySavingType energyType = (EnergySavingType)Enum.Parse(typeof(EnergySavingType), button.Name);

            switch (energyType)
            {
                case EnergySavingType.SavingRatioType:
                    double value = 0;
                    Double.TryParse(SavingRatioTextBox.Text, out value);
                    baseParams.Standard = new EnergySavingRatio(energyType, value);
                    if (string.IsNullOrEmpty(SavingRatioTextBox.Text))
                    {
                        SavingRatioTextBox.Text = "0";
                    }
                    break;
                case EnergySavingType.LeedStandardType:
                    baseParams.Standard = new EnergySavingRatio(energyType, LEEDComboBox.SelectedIndex);
                    break;
                case EnergySavingType.ChineseGreenBuildingStandardType:
                    baseParams.Standard = new EnergySavingRatio(energyType, ChineseGreenBuildingStandar_ComboBox.SelectedIndex);
                    break;
                default:
                    throw new Exception("Unknown EnergyType!");
            }

            (MainWindow.DemoInstance as Demo).RefreshComputation();
        }
    }
}
