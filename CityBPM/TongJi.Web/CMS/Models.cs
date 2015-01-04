using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TongJi.Web.CMS
{
    public class Article
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime PostTime { get; set; }
        public string PostUser { get; set; }
        public bool AllowComment { get; set; }
        public int ViewCount { get; set; }
        public int Order { get; set; } // 负数表示显式置顶
        public int ChannelID { get; set; }
    }

    public class Channel
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public int Order { get; set; }
        public int ParentID { get; set; }
    }

    public class Comment
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostTime { get; set; }
        public string PostUser { get; set; }
        public int ArticleID { get; set; }
    }

    public class Attachment
    {
        public Guid ID { get; set; }
        public int ArticleID { get; set; }
        public Guid FileID { get; set; }
    }

    public class File
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public string Tags { get; set; }
        public DateTime PostTime { get; set; }
        public string PostUser { get; set; }
        public int Size { get; set; }
    }
}
