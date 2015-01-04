using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using System.Data.Entity;

using TongJi.Web.Notifications;
using TongJi.Web.Communication;
using TongJi.Web.Models;

namespace UPlanWeb.Controllers
{
    [Authorize]
    public class SiteMailController : Controller
    {
        [HttpPost]
        public ActionResult SendMail(string sendto, string title, string content)
        {
            SiteMailManager.SendMail(WebSecurity.CurrentUserName, sendto, title, content);
            Notification.EnqueueMessage(WebSecurity.CurrentUserName, string.Format("已向 {0} 发出一封站内信。", sendto));
            Notification.EnqueueMessage(sendto, string.Format("{0} 发来一封站内信。", WebSecurity.CurrentUserName));
            return RedirectToAction("SendBox");
        }

        public ActionResult DialogMode(string with)
        {
            ViewBag.with = with;
            var mails = SiteMailManager.GetDialog(WebSecurity.CurrentUserName, with);
            return View(mails);
        }

        public ActionResult Detail(int id, int type)
        {
            ViewBag.type = type; // type: 1 - 收件； 2 - 已发送
            var mail = SiteMailManager.FindMail(id);
            if (WebSecurity.CurrentUserName == mail.Receiver)
            {
                if (SiteMailManager.MarkRead(id))
                {
                    Notification.EnqueueMessage(mail.Sender, string.Format("{0} 已读取您的站内信。", mail.Receiver));
                }
            }
            return View(mail);
        }

        public ActionResult New(string to)
        {
            ViewBag.to = to;
            using (var context = new BasicWebContext())
            {
                var users = context.UserProfiles.Select(x => x.UserName).ToList();
                return View(users);
            }
        }

        public ActionResult ReceiveBox()
        {
            var mails = SiteMailManager.GetReceiveBox(WebSecurity.CurrentUserName);
            return View(mails);
        }

        public ActionResult SendBox()
        {
            var mails = SiteMailManager.GetSendBox(WebSecurity.CurrentUserName);
            return View(mails);
        }

        public ActionResult Tip()
        {
            ViewBag.UnreadCount = SiteMailManager.GetReceiveBox(WebSecurity.CurrentUserName).Where(x => !x.IsRead).Count();
            return PartialView();
        }

    }
}