using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

using TongJi.Web.Models;

namespace TongJi.Web.Communication
{
    public static class SiteMailManager
    {
        public static List<SiteMail> GetReceiveBox(string username)
        {
            using (var context = new BasicWebContext())
            {
                return context.SiteMails.Where(x => x.Receiver == username).AsEnumerable().Reverse().ToList();
            }
        }

        public static List<SiteMail> GetSendBox(string username)
        {
            using (var context = new BasicWebContext())
            {
                return context.SiteMails.Where(x => x.Sender == username).AsEnumerable().Reverse().ToList();
            }
        }

        public static void SendMail(string sender, string receiver, string title, string content)
        {
            using (var context = new BasicWebContext())
            {
                SiteMail mail = new SiteMail { Sender = sender, Receiver = receiver, SendTime = DateTime.Now, Title = title, Content = content };
                context.SiteMails.Add(mail);
                context.SaveChanges();
            }
        }

        public static List<SiteMail> GetDialog(string hostUser, string guestUser)
        {
            using (var context = new BasicWebContext())
            {
                return context.SiteMails.Where(x => (x.Sender == hostUser && x.Receiver == guestUser)
                    || (x.Sender == guestUser && x.Receiver == hostUser)).ToList();
            }
        }

        public static SiteMail FindMail(int id)
        {
            using (var context = new BasicWebContext())
            {
                return context.SiteMails.Find(id);
            }
        }

        public static bool MarkRead(int id)
        {
            using (var context = new BasicWebContext())
            {
                bool origin = context.SiteMails.Find(id).IsRead;
                context.SiteMails.Find(id).IsRead = true;
                context.SaveChanges();
                return !origin;
            }
        }

        public static string GetContentSummary(string content, int length)
        {
            if (content == null)
            {
                return string.Empty;
            }
            content = System.Web.HttpContext.Current.Server.UrlDecode(content);
            if (content.Length > length)
            {
                return content.Remove(length) + "...";
            }
            else
            {
                return content;
            }
        }
    }
}
