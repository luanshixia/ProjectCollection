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

using System.Net;
using System.Threading;

#if net40

namespace EasyLoad
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

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _cts = new CancellationTokenSource();
            try
            {
                TestManager.BeginTest(RequestUrlBox.Text, Convert.ToInt32(RequestCountBox.Text), Convert.ToInt32(RequestGapBox.Text), _cts.Token);
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

    public class TestRecord
    {
        public int Number { get; set; }
        public string SendTime { get; set; }
        public string ReceiveTime { get; set; }
        public double Delay { get; set; }
        public string Response { get; set; }
        public Task<Task<string>> Task { get; set; }
        public DateTime RawSendTime { get; set; }
    }

    public static class TestManager
    {
        public static void BeginTest(string url, int times, int gap, CancellationToken ct)
        {
            MainWindow.Current.TheList.Items.Clear();
            for (int i = 0; i < times; i++)
            {
                int num = i;
                using (WebClient client = new WebClient())
                {
                    TestRecord record = new TestRecord();
                    record.Number = num + 1;
                    client.DownloadStringCompleted += (sender, e) =>
                    {
                        string response = e.Result;
                        var sendTime = record.RawSendTime;
                        var receiveTime = DateTime.Now;
                        
                        record.ReceiveTime = FormatTime(receiveTime);
                        record.Delay = (receiveTime - sendTime).TotalMilliseconds;
                        record.Response = response;

                        MainWindow.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MainWindow.Current.TheList.Items.Refresh();
                        }));
                    };
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(num * gap);
                        record.RawSendTime = DateTime.Now;
                        record.SendTime = FormatTime(record.RawSendTime);
                        client.DownloadStringAsync(new Uri(url));

                        MainWindow.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MainWindow.Current.TheList.Items.Add(record);
                        }));
                    }, ct);
                }
            }
        }

        private static string FormatTime(DateTime time)
        {
            return string.Format("{0}:{1}:{2}:{3:000}", time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }
}

#else

namespace EasyLoad
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

    public class TestRecord
    {
        public int Number { get; set; }
        public string SendTime { get; set; }
        public string ReceiveTime { get; set; }
        public double Delay { get; set; }
        public string Response { get; set; }
        public Task<Task<string>> Task { get; set; }
        public DateTime RawSendTime { get; set; }
    }

    public static class TestManager
    {
        public static async Task BeginTest(string url, int times, int gap, CancellationToken ct)
        {
            MainWindow.Current.TheList.Items.Clear();
            List<TestRecord> tasks = new List<TestRecord>();
            for (int i = 0; i < times; i++)
            {
                int num = i;
                using (WebClient client = new WebClient())
                {
                    TestRecord record = new TestRecord();
                    record.Number = num + 1;
                    var task = Task.Delay(num * gap, ct).ContinueWith(t =>
                    {
                        record.RawSendTime = DateTime.Now;
                        record.SendTime = FormatTime(record.RawSendTime);
                        return client.DownloadStringTaskAsync(url);
                    }, ct);                    
                    //var task = new Task<Task<string>>(async () => 
                    //{
                    //    await Task.Delay(num * gap);
                    //    record.RawSendTime = DateTime.Now;
                    //    record.SendTime = FormatTime(record.RawSendTime);
                    //    MainWindow.Current.TheList.Items.Add(record);
                    //    return client.DownloadStringTaskAsync(url);
                    //}, ct);
                    record.Task = task;
                    tasks.Add(record);
                }
            }
            foreach (var record in tasks)
            {                
                string response = await await record.Task;
                var sendTime = record.RawSendTime;
                var receiveTime = DateTime.Now;
                
                record.ReceiveTime = FormatTime(receiveTime);
                record.Delay = (receiveTime - sendTime).TotalMilliseconds;
                record.Response = response;

                MainWindow.Current.TheList.Items.Add(record);
            }

            //List<Task<TestRecord>> requests = new List<Task<TestRecord>>();
            //for (int i = 0; i < times; i++)
            //{
            //    requests.Add(DoSingleRequest(url, i + 1, gap));
            //}
            //foreach (var r in requests)
            //{
            //    MainWindow.Current.TheList.Items.Add(await r);
            //}

            //var requests = Enumerable.Range(0, times).Select(async i =>
            //{
            //    await Task.Delay(gap);
            //    return DoSingleRequest(url, i + 1);
            //}).ToList();
            //requests.ForEach(async r =>
            //{
            //    MainWindow.Current.TheList.Items.Add(await r);
            //});
            //for (int i = 0; i < times; i++)
            //{
            //    await AddRecord(url, i + 1);
            //    //await Task.Delay(gap);
            //}
        }

        public static async Task<TestRecord> DoSingleRequest(string url, int number, int gap)
        {
            await Task.Delay(number * gap);
            using (WebClient client = new WebClient { Encoding = System.Text.Encoding.Default })
            {
                TestRecord record = new TestRecord();
                var sendTime = DateTime.Now;
                string response = await client.DownloadStringTaskAsync(url);
                var receiveTime = DateTime.Now;

                record.Number = number;
                record.SendTime = FormatTime(sendTime);
                record.ReceiveTime = FormatTime(receiveTime);
                record.Delay = (receiveTime - sendTime).TotalMilliseconds;
                record.Response = response;

                return record;
            }
        }

        //public static async Task AddRecord(string url, int number)
        //{
        //    MainWindow.Current.TheList.Items.Add(await DoSingleRequest(url, number));
        //}

        private static string FormatTime(DateTime time)
        {
            return string.Format("{0}:{1}:{2}:{3:000}", time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }
}

#endif