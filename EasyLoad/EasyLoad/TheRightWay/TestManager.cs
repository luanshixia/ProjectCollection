using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Threading;

namespace EasyLoad.TheRightWay
{
    public class TestRecord
    {
        public int Number { get; set; }
        public string SendTime { get; set; }
        public string ReceiveTime { get; set; }
        public double Delay { get; set; }
        public string Response { get; set; }
        public DateTime RawSendTime { get; set; }
    }

    public static class TestManager
    {
        public static async Task BeginTest(string url, int times, int gap, CancellationToken ct)
        {
            MainWindow.Current.TheList.Items.Clear();
            await Task.WhenAll(Enumerable.Range(0, times)
                .Select(i => DoSingleRequest(url, i, gap))
                .ToArray());
        }

        public static async Task DoSingleRequest(string url, int i, int gap)
        {
            using (WebClient client = new WebClient { Encoding = System.Text.Encoding.Default })
            {
                await Task.Delay(i * gap);

                TestRecord record = new TestRecord();
                var sendTime = DateTime.Now;
                record.Number = i + 1;
                record.SendTime = FormatTime(sendTime);
                MainWindow.Current.TheList.Items.Add(record);

                string response = await client.DownloadStringTaskAsync(url);
                var receiveTime = DateTime.Now;
                record.ReceiveTime = FormatTime(receiveTime);
                record.Delay = (receiveTime - sendTime).TotalMilliseconds;
                record.Response = response;
                MainWindow.Current.TheList.Items.Refresh();
            }
        }

        private static string FormatTime(DateTime time)
        {
            return string.Format("{0}:{1}:{2}:{3:000}", time.Hour, time.Minute, time.Second, time.Millisecond);
        }
    }
}
