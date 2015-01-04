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
    public partial class Channel : System.Web.UI.Page
    {
        int id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id = Convert.ToInt32(Request.QueryString["id"]);
            lblId.Text = ChannelManager.GetDbRecord(id).name;

            TjDataTable<cms_channel> table = new TjDataTable<cms_channel>();
            table.Rows = ChannelManager.GetChildren(id);
            table.Columns.Add("ID", x => x.id.ToString());
            table.Columns.Add("栏目名称", x => x.name);
            table.Columns.Add("父栏目", x => lblId.Text);
            table.Columns.Add("详情", DataCellGenerators.Hyperlink<cms_channel>("详情", x => string.Format("Channel.aspx?id={0}", x.id)));
            table.Columns.Add("编辑", DataCellGenerators.Hyperlink<cms_channel>("编辑", x => string.Format("ChannelEdit.aspx?id={0}", x.id)));
            table.Columns.Add("文章", DataCellGenerators.Hyperlink<cms_channel>("文章", x => string.Format("Content.aspx?channel={0}", x.id)));
            root.Controls.Add(table.GetTable());

            LinkButton btnNew = new LinkButton();
            btnNew.Text = "添加栏目";
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
            var record = ChannelManager.GetDbRecord(id);
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
            ChannelManager.NewChannel("新栏目", id);
            Response.Redirect(Request.RawUrl);
        }
    }
}