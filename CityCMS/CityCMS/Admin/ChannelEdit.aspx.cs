using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.CMS;

namespace CityCMS.Admin
{
    public partial class ChannelEdit : System.Web.UI.Page
    {
        int id;

        protected void Page_Load(object sender, EventArgs e)
        {
            id = Convert.ToInt32(Request.QueryString["id"]);
            if (!IsPostBack)
            {
                var record = ChannelManager.GetDbRecord(id);
                txtChannelName.Text = record.name;
                selectFather.Items.Add("<根栏目>");
                ChannelManager.GetAll().ForEach(x => selectFather.Items.Add(x.name));
                if (record.fk_father_id == 0)
                {
                    selectFather.SelectedIndex = 0;
                }
                else
                {
                    selectFather.SelectedIndex = ChannelManager.GetAll().Select(x => x.id).ToList().IndexOf(record.fk_father_id) + 1;
                }
                txtBrotherIndex.Text = record.brother_index.ToString();
                //cbHasMain.Checked = record.has_main;
                selectDisplay.SelectedIndex = record.display_mode - 1;
                txtLink.Text = record.link;
            }
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            var record = ChannelManager.GetDbRecord(id);
            record.name = txtChannelName.Text;
            record.brother_index = Convert.ToInt32(txtBrotherIndex.Text);
            if (selectFather.SelectedIndex == 0)
            {
                record.fk_father_id = 0;
            }
            else
            {
                record.fk_father_id = ChannelManager.GetAll().ElementAt(selectFather.SelectedIndex - 1).id;
            }
            //record.has_main = cbHasMain.Checked;
            record.display_mode = selectDisplay.SelectedIndex + 1;
            record.link = txtLink.Text;
            ChannelManager.SaveChanges();
            GoBack();
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            if (ChannelManager.GetContentList(id).Count() == 0 && ChannelManager.GetChildren(id).Count() == 0)
            {
                ChannelManager.GetDbRecord(id).enabled = false;
                ChannelManager.SaveChanges();
                GoBack();
            }
        }

        private void GoBack()
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

        protected void btnBack_Click(object sender, EventArgs e)
        {
            GoBack();
        }
    }
}