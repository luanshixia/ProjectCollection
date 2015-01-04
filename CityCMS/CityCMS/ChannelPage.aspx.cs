using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.CMS;

namespace CityCMS
{
    public partial class ChannelPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int channel = Convert.ToInt32(Request.QueryString["id"]);
            NavigationControl nav = new NavigationControl();
            sidebar.InnerHtml = nav.GetHtml(channel);

            var record = ChannelManager.GetDbRecord(channel);
            int displayMode = record.display_mode;
            if (displayMode == ChannelDisplayMode.MainContent)
            {
                int mainContent = ChannelManager.GetContentList(channel).First().id;
                ContentPresenter cp = new ContentPresenter(mainContent);
                content.InnerHtml = cp.GetHtml();
            }
            else if (displayMode == ChannelDisplayMode.ContentList)
            {
                ContentListControl list = new ContentListControl();
                content.InnerHtml = list.GetHtml(channel, -1, true);
            }
            else if (displayMode == ChannelDisplayMode.LoadPage)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "loadPage", string.Format("<script>loadPage('{0}');</script>", record.link));
            }
            else if (displayMode == ChannelDisplayMode.Hyperlink)
            {
                Response.Redirect(record.link);
            }
        }
    }
}