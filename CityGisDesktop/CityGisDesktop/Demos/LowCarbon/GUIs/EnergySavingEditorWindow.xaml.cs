using System;
using System.Windows;
using System.Windows.Controls;

namespace Dreambuild.Gis.Desktop.Demos.LowCarbon.GUIs
{
    /// <summary>
    /// EnergySavingEditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EnergySavingEditorWindow : Window
    {
        public EnergySavingRatio Standard;
        public bool IsOk = false;
        private bool IsRunning = false;

        public EnergySavingEditorWindow(EnergySavingRatio standard)
        {
            InitializeComponent();

            Standard = standard;

            Initial();
            IsRunning = true;
        }

        private void Initial()
        {
            SavingRatioTextBox.Text = "";

            switch (Standard.EnergyType)
            {
                case EnergySavingType.SavingRatioType:
                    SavingRatioType.IsChecked = true;
                    SavingRatioTextBox.Text = Standard.TempValue.ToString();
                    break;
                case EnergySavingType.LeedStandardType:
                    LeedStandardType.IsChecked = true;
                    LEEDComboBox.SelectedIndex = (int)Standard.TempValue;
                    break;
                case EnergySavingType.ChineseGreenBuildingStandardType:
                    ChineseGreenBuildingStandardType.IsChecked = true;
                    ChineseGreenBuildingStandar_ComboBox.SelectedIndex = (int)Standard.TempValue;
                    break;
                default:
                    throw new Exception("Unknown EnergyType!");
            }
        }

        private void LEEDComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LeedStandardType.IsChecked == true && IsRunning)
            {
                switch (LEEDComboBox.SelectedIndex)
                {
                    case 0:
                        Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 0);
                        break;
                    case 1:
                        Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 1);
                        break;
                    case 2:
                        Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 2);
                        break;
                    case 3:
                        Standard = new EnergySavingRatio(EnergySavingType.LeedStandardType, 3);
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
                        Standard = new EnergySavingRatio(EnergySavingType.ChineseGreenBuildingStandardType, 0);
                        break;
                    case 1:
                        Standard = new EnergySavingRatio(EnergySavingType.ChineseGreenBuildingStandardType, 1);
                        break;
                    case 2:
                        Standard = new EnergySavingRatio(EnergySavingType.ChineseGreenBuildingStandardType, 2);
                        break;
                    default:
                        return;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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
                Standard = new EnergySavingRatio(EnergySavingType.SavingRatioType, energySavingRatio);
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
                    Standard = new EnergySavingRatio(energyType, value);
                    if (string.IsNullOrEmpty(SavingRatioTextBox.Text))
                    {
                        SavingRatioTextBox.Text = "0";
                    }
                    break;
                case EnergySavingType.LeedStandardType:
                    Standard = new EnergySavingRatio(energyType, LEEDComboBox.SelectedIndex);
                    break;
                case EnergySavingType.ChineseGreenBuildingStandardType:
                    Standard = new EnergySavingRatio(energyType, ChineseGreenBuildingStandar_ComboBox.SelectedIndex);
                    break;
                default:
                    throw new Exception("Unknown EnergyType!");
            }
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            IsOk = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
