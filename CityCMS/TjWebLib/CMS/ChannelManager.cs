using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TongJi.Web.DAL;

namespace TongJi.Web.CMS
{
    public static class ChannelManager
    {
        public static int NewChannel(string name, int father)
        {
            cms_channel channel = new cms_channel { name = name, fk_father_id = father, enabled = true, has_main = false, brother_index = 0, display_mode = ChannelDisplayMode.ContentList };
            LinqHelper.CMS.cms_channel.InsertOnSubmit(channel);
            LinqHelper.CMS.SubmitChanges();
            return LinqHelper.CMS.cms_channel.Max(x => x.id);
        }

        public static cms_channel GetDbRecord(int id)
        {
            return LinqHelper.CMS.cms_channel.Single(x => x.id == id);
        }

        public static void SaveChanges()
        {
            LinqHelper.CMS.SubmitChanges();
        }

        public static IEnumerable<cms_channel> GetAll()
        {
            return LinqHelper.CMS.cms_channel.Where(x => x.enabled);
        }

        public static IEnumerable<cms_channel> GetAllRoots()
        {
            return GetChildren(0);
        }

        public static IEnumerable<cms_channel> GetChildren(int id)
        {
            return LinqHelper.CMS.cms_channel.Where(x => x.fk_father_id == id && x.enabled).OrderBy(x => x.brother_index);
        }

        public static int GetLevel(int id)
        {
            return GetRoutToRoot(id).Count;
        }

        public static List<int> GetRoutToRoot(int id)
        {
            var record = GetDbRecord(id);
            List<int> rout = new List<int> { id };
            while (record.fk_father_id != 0)
            {
                record = GetDbRecord(record.fk_father_id);
                rout.Add(record.id);
            }
            return rout;
        }

        public static IEnumerable<cms_content> GetContentList(int id)
        {
            return LinqHelper.CMS.cms_content.Where(x => x.fk_channel_id == id && x.enabled).OrderByDescending(x => x.id);
        }

        public static IEnumerable<cms_content> GetContentListIncludingSubChannels(int id)
        {
            return LinqHelper.CMS.cms_content.ToList().Where(x => GetRoutToRoot(x.fk_channel_id).Contains(id) && x.enabled).OrderByDescending(x => x.id);
        }
    }

    public static class ChannelDisplayMode
    {
        public const int Unknown = 0;
        public const int ContentList = 1;
        public const int MainContent = 2;
        public const int SubChannelList = 3;
        public const int SubChannelContentLists = 4;
        public const int SubChannelContentListsGrouped = 5;
        public const int Hyperlink = 6;
        public const int LoadPage = 7;
        public const int PreviewList = 8;
    }
}
