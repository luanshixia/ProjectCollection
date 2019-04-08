using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CosmosDBQueryCharge
{
    /// <summary>
    /// The request analyzer.
    /// </summary>
    public class RequestAnalyzer
    {
        public static RequestAnalyzer Instance { get; } = new RequestAnalyzer();

        private RequestAnalyzer()
        {
        }

        private List<Record> Records { get; } = new List<Record>();

        public void Log(DateTime timestamp, Guid requestId, bool isEnd, dynamic properties)
        {
            this.Records.Add(new Record
            {
                Timestamp = timestamp,
                RequestId = requestId,
                IsEnd = isEnd,
                Properties = properties
            });
        }

        public void Analyze()
        {
            var starts = this.Records.Where(record => !record.IsEnd).OrderBy(record => record.Timestamp).ToArray();
            var ends = this.Records.Where(record => record.IsEnd).ToDictionary(record => record.RequestId, record => record);
            // Debug.Assert(starts.Length == ends.Count);

            var requests = starts
                .Select(start => ends.ContainsKey(start.RequestId)
                    ? new Request
                    {
                        RequestId = start.RequestId,
                        StartTime = start.Timestamp,
                        EndTime = ends[start.RequestId].Timestamp,
                        Duration = ends[start.RequestId].Timestamp - start.Timestamp
                    }
                    : new Request
                    {
                        RequestId = start.RequestId,
                        StartTime = start.Timestamp,
                        EndTime = DateTime.Now,
                        Duration = DateTime.Now - start.Timestamp
                    })
                .ToArray();

            this.DrawRequestSequenceChart(requests);
        }

        private (DateTime, DateTime) GetTimeWindow()
        {
            return (this.Records.Min(record => record.Timestamp), this.Records.Max(record => record.Timestamp));
        }

        private void DrawRequestSequenceChart(Request[] requests)
        {
            var (minTime, maxTime) = this.GetTimeWindow();
            var windowLength = maxTime - minTime;
            var rows = requests.Select(request =>
            {
                var left = (request.StartTime - minTime).TotalSeconds / windowLength.TotalSeconds;
                var width = request.Duration.TotalSeconds / windowLength.TotalSeconds;
                return $"<tr><td>{request.RequestId}</td><td><div class='bar' style='left: {left * 100:0.##}%; width: {width * 100:0.##}%;' title='Start={request.StartTime}\nEnd={request.EndTime}'></div></td></tr>";
            });

            File.WriteAllText(
                path: $"RequestSequenceChart_{DateTime.Now.ToString("yyyyMMddHHmmss")}.html",
                contents: File.ReadAllText("Resources\\RequestSequenceChart.html").Replace("{{tableRows}}", string.Join(Environment.NewLine, rows)));
        }

        public class Record
        {
            public DateTime Timestamp { get; set; }
            public Guid RequestId { get; set; }
            public bool IsEnd { get; set; }
            public dynamic Properties { get; set; }
        }

        public class Request
        {
            public Guid RequestId { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public TimeSpan Duration { get; set; }
        }
    }
}
