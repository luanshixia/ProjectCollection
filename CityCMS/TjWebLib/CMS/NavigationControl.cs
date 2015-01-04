using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace TongJi.Web.CMS
{
    public class NavigationControl
    {
        public enum NavigationLevel
        {
            Level1,
            Level2,
            Level3,
        }

        public string ContentPageName { get; set; }
        public string ChannelPageName { get; set; }

        public NavigationControl()
        {
            ContentPageName = "Content";
            ChannelPageName = "Channel";
        }

        public string GetHtml(int focusedChannelId = 0)
        {
            return Render(BuildItems(focusedChannelId));
        }

        private List<Tuple<string, NavigationLevel, string, bool>> BuildItems(int focusedChannelId)
        {
            var list1 = ChannelManager.GetAllRoots().Select(x => Tuple.Create(x.name, NavigationLevel.Level1, string.Format("{0}?id={1}", ChannelPageName, x.id), x.id == focusedChannelId)).ToList();
            if (focusedChannelId > 0)
            {
                var rout = ChannelManager.GetRoutToRoot(focusedChannelId);
                rout.Reverse();
                var list2 = ChannelManager.GetChildren(rout[0]).Select(x => Tuple.Create(x.name, NavigationLevel.Level2, string.Format("{0}?id={1}", ChannelPageName, x.id), x.id == focusedChannelId)).ToList();
                int insPoint1 = ChannelManager.GetAllRoots().Select(x => x.id).ToList().IndexOf(rout[0]) + 1;
                if (rout.Count > 1)
                {
                    var list3 = ChannelManager.GetChildren(rout[1]).Select(x => Tuple.Create(x.name, NavigationLevel.Level3, string.Format("{0}?id={1}", ChannelPageName, x.id), x.id == focusedChannelId)).ToList();
                    int insPoint2 = ChannelManager.GetChildren(rout[0]).Select(x => x.id).ToList().IndexOf(rout[1]) + 1;
                    list2.InsertRange(insPoint2, list3);
                }
                list1.InsertRange(insPoint1, list2);
            }
            return list1;
        }

        private string Render(List<Tuple<string, NavigationLevel, string, bool>> items)
        {
            XElement ul = new XElement("ul", new XAttribute("class", "cms_nav"));
            foreach (var item in items)
            {
                XElement li = new XElement("li", new XAttribute("class", "cms_" + item.Item2.ToString().ToLower()));
                string aClass = item.Item4 ? " cms_a_focus" : string.Empty;
                XElement a = new XElement("a", item.Item1, new XAttribute("href", item.Item3), new XAttribute("class", "cms_a_" + item.Item2.ToString().ToLower() + aClass));
                li.Add(a);
                ul.Add(li);
            }
            return ul.ToString();
        }
    }
}
