using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using TongJi.Web.DAL;
using TongJi.Web.Controls;

namespace TongJi.Web.CMS
{
    public class CommentPoster
    {
        private int _contentId;
        TextBox txtTilte;
        TextBox txtAuthor;
        TextBox txtText;
        Label lblErrorMessage;
        public event EventHandler Post;

        public CommentPoster(int contentId)
        {
            _contentId = contentId;
        }

        private bool ValidateComment(string text)
        {
            if (text == null)
            {
                return false;
            }
            else
            {
                return text.Length > 5;
            }
        }

        public Control GetControl()
        {
            HtmlDivision div = new HtmlDivision();
            txtTilte = new TextBox { Text = "主题评论", Width = 500 };
            txtAuthor = new TextBox { Text = "匿名用户", Width = 500 };
            txtText = new TextBox { TextMode = TextBoxMode.MultiLine, Width = 500, Height = 150 };
            lblErrorMessage = new Label { Text = "请认真填写评论", ForeColor = System.Drawing.Color.Red, Visible = false };
            //CustomValidator cv = new CustomValidator { ForeColor = System.Drawing.Color.Red, ControlToValidate = "txtText"};
            
            div.Controls.Add(new HtmlSpan { InnerText = "标题" });
            div.Controls.Add(txtTilte);
            div.Controls.Add(new HtmlBreak());
            div.Controls.Add(new HtmlSpan { InnerText = "署名" });
            div.Controls.Add(txtAuthor);
            div.Controls.Add(new HtmlBreak());
            div.Controls.Add(new HtmlSpan { InnerText = "内容" });
            div.Controls.Add(txtText);
            div.Controls.Add(lblErrorMessage);
            div.Controls.Add(new HtmlBreak());

            Button btnPost = new Button { Text = "发表评论" };
            btnPost.Attributes.Add("id", "btnPost");
            //btnPost.Attributes.Add("onclick", @"if($('textarea').val().length < 6){ event.preventDefault(); $('#btnPost').parent().append('<span style=""color:red"">请认真填写评论。</span>'); }");
            //btnPost.OnClientClick = "if($('textarea').val().length < 6){ event.preventDefault(); $('#btnPost').parent().append('<span style=\"color:red\">请认真填写评论。</span>'); }";
            btnPost.Click += new EventHandler(btnPost_Click);
            div.Controls.Add(btnPost);

            return div;
        }

        void btnPost_Click(object sender, EventArgs e)
        {
            if (ValidateComment(txtText.Text))
            {
                ContentManager.AddComment(txtTilte.Text, txtText.Text, _contentId, txtAuthor.Text);
                if (Post != null)
                {
                    Post(sender, e);
                }
            }
            else
            {
                lblErrorMessage.Visible = true;
            }
        }
    }
}
