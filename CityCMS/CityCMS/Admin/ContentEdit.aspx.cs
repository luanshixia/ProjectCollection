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
    public partial class ContentEdit : System.Web.UI.Page
    {
        int id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id = Convert.ToInt32(Request.QueryString["id"]);
            if (!IsPostBack)
            {
                var record = ContentManager.GetDbRecord(id);
                txtTitle.Text = record.title;
                txtAuthor.Text = record.author;
                txtContent.Text = record.text;
                cbFixToTop.Checked = record.fix_to_top;
                cbAllowComment.Checked = record.allow_comment;
                ChannelManager.GetAll().ForEach(x => selectChannel.Items.Add(x.name));
                selectChannel.SelectedIndex = ChannelManager.GetAll().Select(x => x.id).ToList().IndexOf(record.fk_channel_id);
            }
            else
            {
                if (hidden1.Value == "del")
                {
                    ContentManager.GetDbRecord(id).enabled = false;
                    ContentManager.SaveChanges();
                    Back();
                }
                else if (hidden1.Value.StartsWith("attDel"))
                {
                    int attId = Convert.ToInt32(hidden1.Value.Split('=')[1]);
                    var record = ContentManager.GetAttachmentDbRecord(attId);
                    System.IO.File.Delete(record.filename);
                    ContentManager.DelAttachment(attId);
                    Response.Redirect(Request.RawUrl);
                }
            }

            root.Controls.Add(new HtmlParagraph { InnerText = "本文章中的附件：" });
            TjDataTable<cms_attachment> table = new TjDataTable<cms_attachment>();
            table.Rows = ContentManager.GetAttachments(id);
            table.Columns.Add("ID", x => x.id.ToString());
            table.Columns.Add("标题", x => x.name);
            table.Columns.Add("上传时间", x => x.post_time.ToString());
            table.Columns.Add("下载地址", x => string.Format("Admin/Download.ashx?fileid={0}", x.id));
            table.Columns.Add("删除", DataCellGenerators.LinkButton<cms_attachment>("删除", x => string.Format("if(confirm('确实要删除这个附件吗？')){{document.getElementById('MainContent_hidden1').value = 'attDel={0}';}}", x.id), x => () => { }));
            table.Columns.Add("查看", DataCellGenerators.Hyperlink<cms_attachment>("查看", x => string.Format("Download.ashx?fileid={0}", x.id)));
            root.Controls.Add(table.GetTable());

            //HyperLink link = new HyperLink { Text = "添加附件", NavigateUrl = string.Format("~/Admin/Upload.aspx?id={0}", id) };
            //root.Controls.Add(link);
        }

        protected void btnSaveAndBack_Click(object sender, EventArgs e)
        {
            Save();
            Back();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Back();
        }

        private void Save()
        {
            var record = ContentManager.GetDbRecord(id);
            record.title = txtTitle.Text;
            record.author = txtAuthor.Text;
            record.text = txtContent.Text;
            record.fix_to_top = cbFixToTop.Checked;
            record.allow_comment = cbAllowComment.Checked;
            record.fk_channel_id = ChannelManager.GetAll().ElementAt(selectChannel.SelectedIndex).id;
            ContentManager.SaveChanges();
        }

        private void Back()
        {
            Response.Redirect(string.Format("Content.aspx?channel={0}", ContentManager.GetDbRecord(id).fk_channel_id));
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            //ClientScript.RegisterStartupScript(this.GetType(), "del", "if(confirm('确实要删除这篇文章吗？')){document.getElementById('MainContent_hidden1').value = 'del';}");
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            Save();
            if (FileUpload1.HasFile)
            {
                string fileName = ContentManager.NewAttachmentFileName();
                FileUpload1.SaveAs(ContentManager.AttachmentPath + fileName);
                ContentManager.AddAttachment(FileUpload1.FileName.Substring(FileUpload1.FileName.LastIndexOf('\\') + 1), fileName, id);
                Response.Redirect(Request.RawUrl);
            }
        }
    }
}