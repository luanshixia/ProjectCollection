using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.Controls
{
    public class TableRowDataHolder
    {
        private Dictionary<string, string> _cells = new Dictionary<string, string>();
        public Dictionary<string, string> Cells { get { return _cells; } }
    }

    public static class CellGenerators
    {
        public static Func<TableRowDataHolder, string> Value(string key)
        {
            return x => x.Cells[key];
        }

        public static Func<TableRowDataHolder, string> LinkButton(string text, string href)
        {
            return x => string.Format("<a href=\"{0}\">{1}</a>", href, text);
        }

        public static Func<TableRowDataHolder, string> LinkButton(string text, Func<TableRowDataHolder, string> hrefGenerator)
        {
            return x => string.Format("<a href=\"{0}\">{1}</a>", hrefGenerator(x), text);
        }

        public static Func<TableRowDataHolder, string> LinkButtons(List<Tuple<string, string>> links)
        {
            return x =>
            {
                string result = string.Empty;
                links.ForEach(l => result += string.Format("<a href=\"{0}\">{1}</a> ", l.Item2, l.Item1));
                return result;
            };
        }

        public static Func<TableRowDataHolder, string> LinkButtons(List<Tuple<string, Func<TableRowDataHolder, string>>> links)
        {
            return x =>
            {
                string result = string.Empty;
                links.ForEach(l => result += string.Format("<a href=\"{0}\">{1}</a> ", l.Item2(x), l.Item1));
                return result;
            };
        }
    }

    public class TjTable : WebControl
    {
        protected Dictionary<string, Func<TableRowDataHolder, string>> _colomns = new Dictionary<string, Func<TableRowDataHolder, string>>();
        public Dictionary<string, Func<TableRowDataHolder, string>> Colomns { get { return _colomns; } }

        protected List<TableRowDataHolder> _rows = new List<TableRowDataHolder>();
        public List<TableRowDataHolder> Rows { get { return _rows; } }

        protected List<CheckBox> _checkBoxes = new List<CheckBox>();

        protected List<double> _stars = new List<double>();
        public List<double> Stars { get { return _stars; } }

        public void SetStars(params double[] stars)
        {
            // todo: implement this.
            _stars.Clear();
            _stars.AddRange(stars);
        }

        public HtmlTable GetTable(bool showHeader = true, bool showCheckBox = false)
        {
            HtmlTable result = new HtmlTable();
            result.Attributes.Add("class", "normal");
            //result.Attributes.Add("style", TableStyle);
            _checkBoxes.Clear();

            // header
            if (showHeader)
            {
                HtmlTableRow header = new HtmlTableRow();
                if (showCheckBox)
                {
                    HtmlTableCell tableCell = new HtmlTableCell("th");
                    header.Cells.Add(tableCell);
                }
                foreach (var col in Colomns)
                {
                    HtmlTableCell tableCell = new HtmlTableCell("th");
                    tableCell.InnerText = col.Key;
                    header.Cells.Add(tableCell);
                }
                result.Rows.Add(header);
            }

            // data
            foreach (var row in Rows)
            {
                HtmlTableRow tableRow = new HtmlTableRow();
                if (showCheckBox)
                {
                    HtmlTableCell tableCell = new HtmlTableCell();
                    CheckBox cb = new CheckBox();
                    tableCell.Controls.Add(cb);
                    _checkBoxes.Add(cb);
                    tableRow.Cells.Add(tableCell);
                }
                foreach (var col in Colomns)
                {
                    HtmlTableCell tableCell = new HtmlTableCell();
                    tableCell.InnerHtml = col.Value(row);
                    tableRow.Cells.Add(tableCell);
                }
                result.Rows.Add(tableRow);
            }

            return result;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            GetTable().RenderControl(writer);
        }

        public List<int> GetCheckedRowIndices()
        {
            List<int> result = new List<int>();
            _checkBoxes.ForEach(x => { if (x.Checked)result.Add(_checkBoxes.IndexOf(x)); });
            return result;
        }

        public void SetCheckedRowIndices(List<int> rows)
        {
            rows.ForEach(i => { if (i >= 0 && i <= _checkBoxes.Count - 1)_checkBoxes[i].Checked = true; });
        }

        public void CheckRow(TableRowDataHolder row)
        {
            int i = Rows.IndexOf(row);
            if (i > -1)
            {
                _checkBoxes[i].Checked = true;
            }
        }

        public void UncheckRow(TableRowDataHolder row)
        {
            int i = Rows.IndexOf(row);
            if (i > -1)
            {
                _checkBoxes[i].Checked = false;
            }
        }

        public bool IsRowChecked(TableRowDataHolder row)
        {
            int i = Rows.IndexOf(row);
            if (i > -1)
            {
                return _checkBoxes[i].Checked;
            }
            return false;
        }
    }
}
