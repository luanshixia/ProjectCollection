using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public class AttachmentPresenter
    {
        public IEnumerable<cms_attachment> _attachments;
        public string DownloadPage { get; set; }

        public AttachmentPresenter(int contentId)
        {
            _attachments = ContentManager.GetAttachments(contentId);
            DownloadPage = "Admin/Download.ashx";
        }

        public string GetHtml()
        {
            XElement ul = new XElement("ul", new XAttribute("class", "cms_attachment_presenter"));
            ul.Add(new XElement("span", "附件", new XAttribute("style", "font-weight:bold")));
            foreach (var att in _attachments)
            {
                XElement li = new XElement("li", new XAttribute("class", "cms_attachment"));
                li.Add(TongJi.Web.Controls.HtmlHelper.Anchor(att.name, string.Format("{0}?fileid={1}", DownloadPage, att.id)));
                ul.Add(li);
            }
            return ul.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
        }
    }
}
