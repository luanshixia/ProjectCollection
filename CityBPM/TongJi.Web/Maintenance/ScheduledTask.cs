using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Web.Maintenance
{
    public static class ScheduledTask
    {
        private static List<Tuple<DateTime, System.Timers.Timer>> _startList = new List<Tuple<DateTime, System.Timers.Timer>>();

        static ScheduledTask()
        {
            System.Timers.Timer timer = new System.Timers.Timer(30000);
            timer.Elapsed += (sender, e) =>
            {
                var list = _startList.ToList();
                foreach (var item in list)
                {
                    if (item.Item1 < DateTime.Now)
                    {
                        item.Item2.Start();
                        _startList.Remove(item);
                    }
                }
            };
            timer.Start();
        }

        public static void Register(Action action, DateTime startTime, TimeSpan period)
        {
            System.Timers.Timer timer = new System.Timers.Timer(period.TotalMilliseconds);
            timer.Elapsed += (sender, e) => action();
            _startList.Add(Tuple.Create(startTime, timer));
        }
    }
}
