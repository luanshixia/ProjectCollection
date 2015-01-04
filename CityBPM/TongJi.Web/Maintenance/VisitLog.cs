using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

using System.Net;

namespace TongJi.Web.Maintenance
{
    public static class VisitLog
    {
        public const string LogFileName = "TongJiWebVisitLog.txt";

        public static string LogFileFullName
        {
            get
            {
                return HttpContext.Current.Request.PhysicalApplicationPath + "\\" + LogFileName;
            }
        }

        public static void AddLog()
        {
            string clientIP = HttpContext.Current.Request.UserHostAddress;
            string dateTime = DateTime.Now.ToString();
            VisitLog.WriteLog(clientIP, dateTime);
        }

        public static string GetClientIPv4()
        {
            string ipv4 = "127.0.0.1";
            foreach (IPAddress IPA in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            return ipv4;
        }

        private static void WriteLog(params object[] values)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(LogFileFullName, true))
            {
                sw.WriteLine(string.Join(",", values));
            }
        }

        public static int GetVisitCount()
        {
            return System.IO.File.ReadAllLines(LogFileFullName).Length;
        }

    }
}
