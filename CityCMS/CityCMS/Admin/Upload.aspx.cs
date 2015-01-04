using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.CMS;

namespace CityCMS.Admin
{
    public partial class Upload : System.Web.UI.Page
    {
        int contentId;

        protected void Page_Load(object sender, EventArgs e)
        {
            contentId = Convert.ToInt32(Request.QueryString["id"]);
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                string fileName = ContentManager.NewAttachmentFileName();
                FileUpload1.SaveAs(fileName);
                ContentManager.AddAttachment(FileUpload1.FileName.Substring(FileUpload1.FileName.LastIndexOf('\\') + 1), fileName, contentId);
                Response.Redirect(string.Format("~/Admin/ContentEdit.aspx?id={0}", contentId));
            }
        }
    }
}