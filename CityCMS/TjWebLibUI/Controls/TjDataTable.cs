using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace TongJi.Web.Controls
{
    public static class DataCellGenerators
    {
        public static Func<T, string> Hyperlink<T>(string text, Func<T, string> hrefGenerator)
        {
            return x => string.Format("<a href=\"{0}\">{1}</a>", hrefGenerator(x), text);
        }

        public static Func<T, string> Hyperlinks<T>(List<Tuple<string, Func<T, string>>> links)
        {
            return x =>
            {
                string result = string.Empty;
                links.ForEach(l => result += string.Format("<a href=\"{0}\">{1}</a> ", l.Item2(x), l.Item1));
                return result;
            };
        }

        public static Func<T, Control> LinkButton<T>(string text, Func<T, string> clientAction, Func<T, Action> serverAction)
        {
            return x =>
            {
                LinkButton btn = new LinkButton { Text = text, OnClientClick = clientAction(x) };
                btn.Click += (sender, e) => serverAction(x)();
                return btn;
            };
        }
    }

    public class TjDataTable<T> : WebControl
    {
        public Dictionary<string, Func<T, object>> Columns { get; private set; }
        public IEnumerable<T> Rows { get; set; }

        protected List<CheckBox> _checkBoxes = new List<CheckBox>();

        protected List<double> _stars = new List<double>();
        public List<double> Stars { get { return _stars; } }

        public bool ShowHeader { get; set; }
        public bool ShowCheckBox { get; set; }

        public TjDataTable()
        {
            Columns = new Dictionary<string, Func<T, object>>();
            Rows = new List<T>();

            ShowHeader = true;
            ShowCheckBox = false;
        }

        public void SetStars(params double[] stars)
        {
            // todo: implement this.
            _stars.Clear();
            _stars.AddRange(stars);
        }

        public string GetHtml()
        {
            string html = GetTable().ToHtml();
            double sumStar = _stars.Sum();
            HtmlColGroup colgroup = new HtmlColGroup(_stars.Select(x => (x / sumStar).ToString("0.#%")).ToArray());
            string colgroupHtml = colgroup.ToHtml();
            int insertPos = html.IndexOf("<tr>");
            html = html.Insert(insertPos, colgroupHtml);
            return html;
        }

        /// <summary>
        /// 不支持定义列宽。请使用GetHtml()方法。
        /// </summary>
        /// <param name="showHeader"></param>
        /// <param name="showCheckBox"></param>
        /// <returns></returns>
        public HtmlTable GetTable()
        {
            HtmlTable result = new HtmlTable();
            result.Attributes.Add("class", "normal");
            result.Attributes.Add("id", this.ID);
            //result.Attributes.Add("style", TableStyle);
            _checkBoxes.Clear();

            // header
            if (ShowHeader)
            {
                HtmlTableRow header = new HtmlTableRow();
                if (ShowCheckBox)
                {
                    HtmlTableCell tableCell = new HtmlTableCell("th");
                    header.Cells.Add(tableCell);
                }
                foreach (var col in Columns)
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
                if (ShowCheckBox)
                {
                    HtmlTableCell tableCell = new HtmlTableCell();
                    CheckBox cb = new CheckBox();
                    tableCell.Controls.Add(cb);
                    _checkBoxes.Add(cb);
                    tableRow.Cells.Add(tableCell);
                }
                foreach (var col in Columns)
                {
                    HtmlTableCell tableCell = new HtmlTableCell();
                    if (col.Value(row) is Control)
                    {
                        tableCell.Controls.Add(col.Value(row) as Control);
                    }
                    else
                    {
                        tableCell.InnerHtml = col.Value(row).ToString();
                    }
                    tableRow.Cells.Add(tableCell);
                }
                result.Rows.Add(tableRow);
            }

            return result;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            //GetTable().RenderControl(writer);
            writer.Write(GetHtml());
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

        public void CheckRow(T row)
        {
            int i = Rows.ToList().IndexOf(row);
            if (i > -1)
            {
                _checkBoxes[i].Checked = true;
            }
        }

        public void UncheckRow(T row)
        {
            int i = Rows.ToList().IndexOf(row);
            if (i > -1)
            {
                _checkBoxes[i].Checked = false;
            }
        }

        public bool IsRowChecked(T row)
        {
            int i = Rows.ToList().IndexOf(row);
            if (i > -1)
            {
                return _checkBoxes[i].Checked;
            }
            return false;
        }
    }
}
