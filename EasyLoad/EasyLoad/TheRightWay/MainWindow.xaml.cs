using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Threading;

namespace EasyLoad.TheRightWay
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Current { get; private set; }
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();

            Current = this;
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();
            try
            {
                await TestManager.BeginTest(RequestUrlBox.Text, Convert.ToInt32(RequestCountBox.Text), Convert.ToInt32(RequestGapBox.Text), _cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EasyLoad", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
        }

        private void TheList_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (TheList.SelectedItem != null)
            {
                ResponseDetailWindow rdw = new ResponseDetailWindow { Owner = this };
                rdw.TheTextBox.Text = (TheList.SelectedItem as TestRecord).Response;
                rdw.ShowDialog();
            }
        }

        private void CancelButton_Click_1(object sender, RoutedEventArgs e)
        {
            _cts.Cancel();
        }
    }
}