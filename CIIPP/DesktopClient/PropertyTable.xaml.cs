using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopClient
{
    /// <summary>
    /// PropertyTable.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyTable : Table, IUpdateTable
    {
        /// <summary>
        /// 行定义。指定属性名、对象、默认值。
        /// </summary>
        public Dictionary<string, Tuple<object, object>> Rows { get; private set; }

        public PropertyTable()
            : base()
        {
            InitializeComponent();
            Rows = new Dictionary<string, Tuple<object, object>>();
        }

        public void Render()
        {
            _cells.Clear();
            LayoutRoot.Children.Clear();
            LayoutBorder.BorderBrush = Brush_TableBorder;
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            Enumerable.Range(0, Rows.Count() + 1).ToList().ForEach(i => LayoutRoot.RowDefinitions.Add(new RowDefinition()));
            Enumerable.Range(0, 3).ToList().ForEach(i => LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(GetColWidth(i)) }));
            LayoutRoot.RowDefinitions[0].Height = new GridLength(25);

            // Row 1
            _cells.Add(Table.BuildTextBlock(SectionNumber, Brush_TitleCell, Brush_TitleCell, Brush_Title, true), new CellPosition(0, 0));
            _cells.Add(Table.BuildTextBlock(Title, Brush_TitleCell, Brush_TitleCell, Brush_Title), new CellPosition(0, 1, 1, 2));

            // Rows
            int r = 1;
            foreach (var row in Rows)
            {
                _cells.Add(Table.BuildTextBlock(GetDisplayId(row.Key), Brush_Cell, Brush_CellBorder, Brushes.White, true), new CellPosition(r, 0));
                _cells.Add(Table.BuildTextBlock(Table.GetRowDescription(row.Key), Brush_LabelCell, Brush_CellBorder, Brushes.White), new CellPosition(r, 1));

                var prop = row.Key;
                var obj = row.Value.Item1;
                var defaultValue = row.Value.Item2;
                var property = obj.GetType().GetProperty(prop);
                var value = property.GetValue(obj, null);
                if (value is CIIPP.DoubleExpression)
                {
                    double num = (value as CIIPP.DoubleExpression).GetValue();
                    var valueString = double.IsNaN(num) ? "na" : num.ToString("0.##");
                    if (_percentageRows.Contains(row.Key))
                    {
                        valueString = string.Format("{0:0.##%}", num);
                    }
                    var tb = Table.BuildTextBlock(valueString, Brush_ResultCell, Brush_CellBorder, Brushes.White);
                    _cells.Add(tb, new CellPosition(r, 2));
                }
                else if (!IsRowEditable(row.Key))
                {
                    var tb = Table.BuildTextBlock(string.Format("{0:0.##}", value), Brush_ResultCell, Brush_CellBorder, Brushes.White);
                    _cells.Add(tb, new CellPosition(r, 2));
                }
                else
                {
                    var valueString = FormatValue(prop, value);
                    TextBox tb = Table.BuildTextBox(valueString, Brush_EditCell) as TextBox;
                    //var callback = row.Value.Item2;
                    tb.GotFocus += (sender, e) =>
                    {
                        (sender as TextBox).BorderBrush = Brushes.Black;
                        (sender as TextBox).BorderThickness = new Thickness(2);
                    };
                    tb.LostFocus += (sender, e) =>
                    {
                        (sender as TextBox).BorderBrush = Brushes.WhiteSmoke;
                        (sender as TextBox).BorderThickness = new Thickness(0);
                        //callback(sender, null);
                        UpdateValue(obj, prop, (sender as TextBox).Text, defaultValue);
                        var newValue = property.GetValue(obj, null);
                        (sender as TextBox).Text = FormatValue(prop, newValue); // newly 20120214
                        base.OnUserInput(e);
                    };
                    _cells.Add(tb, new CellPosition(r, 2));
                }

                r++;
            }

            foreach (var cell in _cells)
            {
                Grid.SetRow(cell.Key, cell.Value.Row);
                Grid.SetRowSpan(cell.Key, cell.Value.RowSpan);
                Grid.SetColumn(cell.Key, cell.Value.Col);
                Grid.SetColumnSpan(cell.Key, cell.Value.ColSpan);
                LayoutRoot.Children.Add(cell.Key);
            }
        }

        private string FormatValue(string prop, object value)
        {
            var valueString = value is DateTime ? ((DateTime)value).ToShortDateString() : string.Format("{0:0.##}", value);
            if (_percentageRows.Contains(prop))
            {
                valueString = string.Format("{0:0.##%}", value);
            }
            return valueString;
        }

        public void UpdateData()
        {
            // Rows
            int r = 1;
            foreach (var row in Rows)
            {
                var prop = row.Key;
                var obj = row.Value.Item1;
                var defaultValue = row.Value.Item2;
                var property = obj.GetType().GetProperty(prop);
                var value = property.GetValue(obj, null);
                string valueString = string.Empty;
                if (value is CIIPP.DoubleExpression)
                {
                    double num = (value as CIIPP.DoubleExpression).GetValue();
                    valueString = double.IsNaN(num) ? "na" : num.ToString("0.##");
                    if (_percentageRows.Contains(row.Key))
                    {
                        valueString = string.Format("{0:0.##%}", num);
                    }
                    var border = _cells.First(x => x.Value.Row == r && x.Value.Col == 2).Key as Border;
                    (border.Child as TextBlock).Text = valueString;
                }
                else if (!IsRowEditable(row.Key))
                {
                    valueString = string.Format("{0:0.##}", value);
                    var border = _cells.First(x => x.Value.Row == r && x.Value.Col == 2).Key as Border;
                    (border.Child as TextBlock).Text = valueString;
                }
                else
                {
                }
                r++;
            }
        }

        private static void UpdateValue(object obj, string prop, string value, object defaultValue)
        {
            var property = obj.GetType().GetProperty(prop);
            if (property.PropertyType == typeof(int))
            {
                int num = Convert.ToInt32(defaultValue);
                int.TryParse(value, out num);
                property.SetValue(obj, num, null);
            }
            else if (property.PropertyType == typeof(double))
            {
                double num = Convert.ToDouble(defaultValue);
                double.TryParse(value, out num);
                if (value.Contains('%'))
                {
                    num = Convert.ToDouble(value.Trim('%')) / 100.0;
                }
                property.SetValue(obj, num, null);
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                DateTime num = Convert.ToDateTime(defaultValue);
                DateTime.TryParse(value, out num);
                property.SetValue(obj, num, null);
            }
            else if (property.PropertyType == typeof(string))
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = defaultValue.ToString();
                }
                property.SetValue(obj, value, null);
            }
        }

        private static double GetColWidth(int col)
        {
            double[] colWidths = { 30, 300, 300, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80, 80 };
            return colWidths[col];
        }

        private static string GetDisplayId(string key)
        {
            if (key.All(x => char.IsLetter(x)))
            {
                return key.Remove(3);
            }
            else if (key.Count(x => char.IsLetter(x)) > 1)
            {
                return key.Where(x => char.IsLetter(x)).ElementAt(1).ToString();
            }
            else
            {
                return key.Last().ToString();
            }
        }

        public void SetPercentageTable()
        {
            _percentageRows = Rows.Keys.ToArray();
        }
    }
}
