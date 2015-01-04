using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.CMS;
using TongJi.Web.Controls;
using TongJi.Web.DAL;

namespace CityCMS.Admin
{
    public partial class Content : System.Web.UI.Page
    {
        int channel;

        protected void Page_Load(object sender, EventArgs e)
        {
            channel = Convert.ToInt32(Request.QueryString["channel"]);
            lblId.Text = ChannelManager.GetDbRecord(channel).name;

            TjDataTable<cms_content> table = new TjDataTable<cms_content>();
            table.Rows = ChannelManager.GetContentList(channel);
            table.Columns.Add("ID", x => x.id.ToString());
            table.Columns.Add("标题", x => x.title);
            table.Columns.Add("作者", x => x.author);
            table.Columns.Add("所属栏目", x => lblId.Text);
            table.Columns.Add("发表时间", x => x.post_time.ToString());
            table.Columns.Add("编辑", DataCellGenerators.Hyperlink<cms_content>("编辑", x => string.Format("ContentEdit.aspx?id={0}", x.id)));
            root.Controls.Add(table.GetTable());

            LinkButton btnNew = new LinkButton();
            btnNew.Text = "添加文章";
            btnNew.Click += new EventHandler(btnNew_Click);
            root.Controls.Add(btnNew);
            root.Controls.Add(new HtmlBreak());

            LinkButton btnBack = new LinkButton();
            btnBack.Text = "返回上级";
            btnBack.Click += new EventHandler(btnBack_Click);
            root.Controls.Add(btnBack);
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            var record = ChannelManager.GetDbRecord(channel);
            if (record.fk_father_id == 0)
            {
                Response.Redirect("Default.aspx");
            }
            else
            {
                Response.Redirect(string.Format("Channel.aspx?id={0}", record.fk_father_id));
            }
        }

        void btnNew_Click(object sender, EventArgs e)
        {
            ContentManager.NewContent("新文章", "未知作者", string.Empty, channel);
            Response.Redirect(Request.RawUrl);
        }
    }
}