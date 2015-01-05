using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CityInfographics
{
    public class TimeRecordList
    {
        public List<TimeRecord> Records { get; private set; }

        public TimeRecordList(string fileName)
        {
            var lines = System.IO.File.ReadAllLines(fileName, Encoding.Default).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Split(',')).ToArray();
            Records = lines.Select(x => new TimeRecord(new TimePoint(x[0].TryParseToInt32(), x[1].TryParseToInt32(), x[2].TryParseToInt32()), 
                x[3].TryParseToDouble(), x[4].TryParseToDouble())).ToList();
        }

        public double GetValue(int m, int d, int h, int i)
        {
            foreach (var record in Records)
            {
                if (record.Time.Month == m && record.Time.Day == d && record.Time.Hour == h)
                {
                    return record.Data[i];
                }
            }
            return 0;
        }
    }

    public class TimeRecord
    {
        public TimePoint Time { get; private set; }
        public List<double> Data { get; private set; }

        public TimeRecord(TimePoint time, params double[] data)
        {
            Time = time;
            Data = data.ToList();
        }
    }
}
