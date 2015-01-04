using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.Controls
{
    public class TjBlock : WebControl
    {
        public string Header { get; private set; }
        public string Content { get; private set; }
        public bool ShowHeader { get; set; }

        public TjBlock(string header)
        {
            Header = header;
            Content = string.Empty;
            ShowHeader = true;
        }

        public HtmlTable GetBlock()
        {
            HtmlTable result = new HtmlTable();
            result.Attributes.Add("class", "block");

            HtmlTableRow header = new HtmlTableRow();
            HtmlTableCell tableCell = new HtmlTableCell("th");
            tableCell.Attributes.Add("class", "block");
            tableCell.InnerText = Header;
            header.Cells.Add(tableCell);
            result.Rows.Add(header);

            HtmlTableRow content = new HtmlTableRow();
            HtmlTableCell contentCell = new HtmlTableCell();
            contentCell.Attributes.Add("class", "block");
            contentCell.InnerHtml = Content;
            content.Cells.Add(contentCell);
            result.Rows.Add(content);

            return result;
        }

        public string GetBlockHtml()
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            GetBlock().RenderControl(htw);
            return sw.ToString();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            GetBlock().RenderControl(writer);
        }

        public void WriteLine(string line)
        {
            if (Content == string.Empty)
            {
                Content += line;
            }
            else
            {
                Content += "<br />" + line;
            }
        }

        public void WriteAnchor(string text, string href)
        {
            Content += string.Format("<a href=\"{0}\">{1}</a>", href, text);
        }

        public void Write(string text)
        {
            Content += text;
        }
    }
}
