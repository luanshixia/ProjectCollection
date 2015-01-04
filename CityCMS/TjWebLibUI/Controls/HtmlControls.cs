using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.Controls
{
    public class HtmlDivision : HtmlContainerControl
    {
        public HtmlDivision()
            : base("div")
        {
        }
    }

    public class HtmlSpan : HtmlContainerControl
    {
        public HtmlSpan()
            : base("span")
        {
        }
    }

    public class HtmlParagraph : HtmlContainerControl
    {
        public HtmlParagraph()
            : base("p")
        {
        }
    }

    public class HtmlBreak : HtmlControl
    {
        public HtmlBreak()
            : base("br")
        {
        }
    }

    public class HtmlScript : HtmlContainerControl
    {
        public HtmlScript()
            : base("script")
        {
            this.Attributes.Add("type", "text/javascript");
        }
    }

    public class HtmlColGroup : HtmlContainerControl
    {
        public HtmlColGroup(params string[] colWidths)
            : base("colgroup")
        {
            foreach (var colWidth in colWidths)
            {
                this.InnerHtml += string.Format("<col style=\"width:{0}; min-width:{0};\" />", colWidth);
            }
        }
    }

    public static class HtmlHelper
    {
        public static string AnchorLine(string text, string href)
        {
            return string.Format("<a href=\"{0}\">{1}</a><br />", href, text);
        }

        public static string Anchor(string text, string href)
        {
            return string.Format("<a href=\"{0}\">{1}</a>", href, text);
        }

        public static string Line(string text)
        {
            return "<br />" + text;
        }

        public static string Paragraph(string text)
        {
            return string.Format("<p>{0}</p>", text);
        }

        public static string ToHtml(this HtmlControl control)
        {
            System.IO.StringWriter sw = new System.IO.StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            control.RenderControl(htw);
            return sw.ToString();
        }
    }
}
