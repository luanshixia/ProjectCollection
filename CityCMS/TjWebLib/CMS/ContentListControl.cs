using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public class ContentListControl
    {
        public string ContentPageName { get; set; }
        public string ChannelPageName { get; set; }

        public ContentListControl()
        {
            ContentPageName = "Content";
            ChannelPageName = "Channel";
        }

        public string GetHtml(int channelId, int loadSize = -1, bool showDate = false, int lineLength = -1)
        {
            return Render(BuildItems(channelId, loadSize, showDate), lineLength);
        }

        public string GetHtml(IEnumerable<cms_content> contents, int loadSize = -1, bool showDate = false, int lineLength = -1)
        {
            return Render(BuildItems(contents, loadSize, showDate), lineLength);
        }

        private List<Tuple<string, string, string>> BuildItems(int channelId, int loadSize, bool showDate)
        {
            return BuildItems(ChannelManager.GetContentList(channelId), loadSize, showDate);
        }

        private List<Tuple<string, string, string>> BuildItems(IEnumerable<cms_content> contents, int loadSize, bool showDate)
        {
            var list1 = contents.Where(x => x.fix_to_top).Select(x => Tuple.Create("[置顶] " + x.title, string.Format("{0}?id={1}", ContentPageName, x.id), BuildAddtionalInfo(x, showDate))).ToList();
            var list2 = contents.Where(x => !x.fix_to_top).Select(x => Tuple.Create(x.title, string.Format("{0}?id={1}", ContentPageName, x.id), BuildAddtionalInfo(x, showDate))).ToList();
            list1.AddRange(list2);
            if (loadSize >= 0)
            {
                list1.RemoveAll(x => list1.IndexOf(x) >= loadSize);
            }
            return list1;
        }

        private string BuildAddtionalInfo(cms_content content, bool showDate)
        {
            return showDate ? string.Format("{0}-{1}", content.post_time.Month, content.post_time.Day) : string.Empty;
        }

        private string Render(List<Tuple<string, string, string>> items, int lineLength)
        {
            XElement ul = new XElement("ul", new XAttribute("class", "cms_list"));
            foreach (var item in items)
            {
                string text = item.Item1;
                int length = lineLength - 2;
                if (length > 0 && text.Length > lineLength)
                {
                    text = text.Remove(length) + "...";
                }
                XElement li = new XElement("li", new XAttribute("class", "cms_list_item"));
                XElement a = new XElement("a", text, new XAttribute("href", item.Item2), new XAttribute("class", "cms_list_item_a"), new XAttribute("title", item.Item1));
                XElement span = new XElement("span", string.Format("{0}", item.Item3, new XAttribute("class", "cms_list_item_date"))); // 去掉四个nbsp
                li.Add(a);
                li.Add(span);
                ul.Add(li);
            }
            return ul.ToString().Replace("&amp;", "&");
        }
    }
}
