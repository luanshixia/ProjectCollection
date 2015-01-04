using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public static class ContentManager
    {
        public static int NewContent(string title, string author, string text, int channel)
        {
            cms_content content = new cms_content { title = title, author = author, text = text, enabled = true, fix_to_top = false, fk_channel_id = channel, post_time = DateTime.Now, allow_comment = true };
            LinqHelper.CMS.cms_content.InsertOnSubmit(content);
            LinqHelper.CMS.SubmitChanges();
            return LinqHelper.CMS.cms_content.Max(x => x.id);
        }

        public static cms_content GetDbRecord(int id)
        {
            return LinqHelper.CMS.cms_content.Single(x => x.id == id);
        }

        public static void SaveChanges()
        {
            LinqHelper.CMS.SubmitChanges();
        }

        public static int AddAttachment(string name, string filename, int belonging)
        {
            cms_attachment attachment = new cms_attachment { name = name, filename = filename, fk_content_id = belonging, post_time = DateTime.Now };
            LinqHelper.CMS.cms_attachment.InsertOnSubmit(attachment);
            LinqHelper.CMS.SubmitChanges();
            return LinqHelper.CMS.cms_attachment.Max(x => x.id);
        }

        public static int AddComment(string title, string text, int belonging, string author)
        {
            cms_comment comment = new cms_comment { title = title, text = text, fk_content_id = belonging, post_time = DateTime.Now, author = author, ip = GetClientIPv4() };
            LinqHelper.CMS.cms_comment.InsertOnSubmit(comment);
            LinqHelper.CMS.SubmitChanges();
            return LinqHelper.CMS.cms_comment.Max(x => x.id);
        }

        public static IEnumerable<cms_attachment> GetAttachments(int id)
        {
            return LinqHelper.CMS.cms_attachment.Where(x => x.fk_content_id == id);
        }

        public static IEnumerable<cms_comment> GetComments(int id)
        {
            return LinqHelper.CMS.cms_comment.Where(x => x.fk_content_id == id);
        }

        public static string NewAttachmentFileName()
        {            
            string guidName = Guid.NewGuid().ToString() + ".file";
            return guidName;
        }

        public static string AttachmentPath
        {
            get
            {
                string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "\\upload\\";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static cms_attachment GetAttachmentDbRecord(int attId)
        {
            return LinqHelper.CMS.cms_attachment.Single(x => x.id == attId);
        }

        public static void DelAttachment(int attId)
        {
            LinqHelper.CMS.cms_attachment.DeleteOnSubmit(GetAttachmentDbRecord(attId));
            LinqHelper.CMS.SubmitChanges();
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
    }
}
