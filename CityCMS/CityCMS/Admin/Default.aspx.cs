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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!System.Web.Security.Roles.IsUserInRole("Administrators"))
            {
                Response.Redirect("~/Account");
            }

            TjDataTable<cms_channel> table = new TjDataTable<cms_channel>();
            table.Rows = ChannelManager.GetAllRoots();
            table.Columns.Add("ID", x => x.id.ToString());
            table.Columns.Add("栏目名称", x => x.name);
            table.Columns.Add("父栏目", x => "<根栏目>");
            table.Columns.Add("详情", DataCellGenerators.Hyperlink<cms_channel>("详情", x => string.Format("Channel.aspx?id={0}", x.id)));
            table.Columns.Add("编辑", DataCellGenerators.Hyperlink<cms_channel>("编辑", x => string.Format("ChannelEdit.aspx?id={0}", x.id)));
            table.Columns.Add("文章", DataCellGenerators.Hyperlink<cms_channel>("文章", x => string.Format("Content.aspx?channel={0}", x.id)));
            root.Controls.Add(table.GetTable());

            LinkButton btnNew = new LinkButton();
            btnNew.Text = "添加栏目";
            btnNew.Click += new EventHandler(btnNew_Click);
            root.Controls.Add(btnNew);
        }

        void btnNew_Click(object sender, EventArgs e)
        {
            ChannelManager.NewChannel("新栏目", 0);
            Response.Redirect(Request.RawUrl);
        }
    }
}