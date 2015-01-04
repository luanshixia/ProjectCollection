using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public class CommentPresenter
    {
        public IEnumerable<cms_comment> _comments;

        public CommentPresenter(int contentId)
        {
            _comments = ContentManager.GetComments(contentId);
        }

        public string GetHtml()
        {
            XElement ul = new XElement("ul", new XAttribute("class", "cms_comment_presenter"));
            int i = 1;
            foreach (var comment in _comments)
            {
                XElement li = new XElement("li", new XAttribute("class", "cms_comment"));
                XElement pInfo = new XElement("p", new XAttribute("class", "cms_comment_info"));
                XElement pText = new XElement("p", new XAttribute("class", "cms_comment_text"));
                li.Add(pInfo, pText);
                ul.Add(li);

                pInfo.Add(new XElement("span", string.Format("{0}楼", i), new XAttribute("class", "cms_comment_number")));
                pInfo.Add(new XElement("span", string.Format("{0}", comment.author), new XAttribute("class", "cms_comment_author")));
                pInfo.Add(new XElement("span", string.Format("{0}", comment.title), new XAttribute("class", "cms_comment_title")));
                pInfo.Add(new XElement("br"));
                pInfo.Add(new XElement("span", string.Format("{0}", comment.post_time)));
                pInfo.Add(new XElement("span", string.Format("{0}", comment.ip)));

                pText.Add(comment.text);

                i++;
            }
            return ul.ToString();
        }
    }
}
