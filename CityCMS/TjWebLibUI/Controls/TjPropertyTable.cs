using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.Controls
{
    public class TjPropertyTable : WebControl
    {
        public Dictionary<string, object> Properties { get; private set; }

        public TjPropertyTable()
        {
            Properties = new Dictionary<string, object>();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            GetTable().RenderControl(writer);
        }

        public HtmlTable GetTable()
        {
            HtmlTable result = new HtmlTable();
            foreach (var prop in Properties)
            {
                HtmlTableRow row = new HtmlTableRow();
                row.Cells.Add(new HtmlTableCell("th") { InnerText = prop.Key });
                row.Cells.Add(new HtmlTableCell("td") { InnerText = prop.Value.ToString() });
                result.Rows.Add(row);
            }
            return result;
        }
    }
}
