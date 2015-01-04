using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.CMS;

namespace CityCMS
{
    public partial class ContentPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(Request.QueryString["id"]);
            int channel = ContentManager.GetDbRecord(id).fk_channel_id;
            NavigationControl nav = new NavigationControl();
            sidebar.InnerHtml = nav.GetHtml(channel);

            ContentPresenter cp = new ContentPresenter(id);
            content.InnerHtml = cp.GetHtml();
            content.InnerHtml += "<p>&nbsp;</p>";

            // 附件
            if (ContentManager.GetAttachments(id).Count() > 0)
            {
                AttachmentPresenter ap = new AttachmentPresenter(id);
                TongJi.Web.Controls.HtmlDivision div = new TongJi.Web.Controls.HtmlDivision();
                div.InnerHtml = ap.GetHtml();
                content.Controls.Add(div);
                content.Controls.Add(new TongJi.Web.Controls.HtmlParagraph());
            }

            // 评论
            if (ContentManager.GetDbRecord(id).allow_comment)
            {
                if (ContentManager.GetComments(id).Count() > 0)
                {
                    CommentPresenter presenter = new CommentPresenter(id);
                    TongJi.Web.Controls.HtmlDivision div = new TongJi.Web.Controls.HtmlDivision();
                    div.InnerHtml = presenter.GetHtml();
                    content.Controls.Add(div);
                }
                CommentPoster poster = new CommentPoster(id);
                content.Controls.Add(poster.GetControl());
                poster.Post += new EventHandler(poster_Post);
            }
        }

        void poster_Post(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }
    }
}