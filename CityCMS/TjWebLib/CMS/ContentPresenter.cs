using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public class ContentPresenter
    {
        private cms_content _content;

        public ContentPresenter(int contentId)
        {
            _content = ContentManager.GetDbRecord(contentId);
        }

        public string GetHtml()
        {
            XElement div = new XElement("div", new XAttribute("class", "cms_content"));
            XElement h = new XElement("h2", _content.title);
            XElement pInfo = new XElement("p", string.Format("作者：{0}&nbsp;&nbsp;&nbsp;&nbsp;发表时间：{1}", _content.author, _content.post_time));
            XElement pBlankLine = new XElement("p", "&nbsp;");
            XElement divContent = new XElement("div", _content.text.Replace("&lt;", "%lt%").Replace("&gt;", "%gt%").Replace("&amp;", "%amp%"));
            div.Add(h, pInfo, pBlankLine, divContent);
            return div.ToString().Replace("&lt;", "<").Replace("&gt;", ">").Replace("%lt%", "&lt;").Replace("%gt%", "&gt;").Replace("&amp;", "&").Replace("%amp%", "&amp;");//.Replace("\r\n", "<br />");
        }
    }
}
