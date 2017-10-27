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

namespace NotAwait
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Button _tb = new Button();
        private int _count = 0;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this._tb.Click += _tb_Click;
        }

        private void _tb_Click(object sender, RoutedEventArgs e)
        {
            this._count--;
            this._tb.Content = this._count.ToString();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Content = this._tb;
            this.RunBackgroundTask();
        }

        private async void RunBackgroundTask()
        {
            while (true)
            {
                this._count++;
                this._tb.Content = this._count.ToString();
                await Task.Delay(2000);
            }
        }
    }
}
