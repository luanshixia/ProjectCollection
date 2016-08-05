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

namespace MyAsync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // This case shows a long CPU task will block the UI just as sync op...
        private async void ButtonLongRun_Click(object sender, RoutedEventArgs e)
        {
            long result = await Task.FromResult(Fib(40));
            MessageBox.Show(result.ToString());
        }

        // ...unless using Task.Run() to do it in a new thread
        private async void ButtonLongRun2_Click(object sender, RoutedEventArgs e)
        {
            long result = await Task.Run(() => Fib(40));
            MessageBox.Show(result.ToString());
        }

        private static long Fib(long n)
        {
            if (n <= 1)
            {
                return 1;
            }
            else
            {
                return Fib(n - 1) + Fib(n - 2);
            }
        }

        // This case shows the executing order with await.
        private async void ButtonLoopAwait_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                Label1.Content = i;
                await Task.Delay(500);
            }
            Label1.Content = "done.";
        }

        private async Task<string> GetUrlContents(string url)
        {
            using (var client = new System.Net.WebClient())
            {
                return await client.DownloadStringTaskAsync(url);
            }
        }
        
        // Shows how to use WhenAll()
        private async void ButtonLoopAwaitAll_Click(object sender, RoutedEventArgs e)
        {
            var wc = new System.Net.WebClient();
            string[] urls = { "www.baidu.com", "www.qq.com" };
            var results = await Task.WhenAll(urls.Select(url => GetUrlContents(url)));
        }
    }
}
