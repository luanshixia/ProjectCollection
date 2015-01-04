using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.CMS;

namespace CityCMS
{
    public partial class HomeMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var roots = ChannelManager.GetAllRoots();
            foreach (var root in roots)
            {
                MenuItem item = new MenuItem
                {
                    Text = root.name,
                    Value = root.name,
                    NavigateUrl = "ChannelPage.aspx?id=" + root.id,
                };
                NavigationMenu.Items.Add(item);
            }
        }
    }
}
