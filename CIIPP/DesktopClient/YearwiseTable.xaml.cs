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

using CIIPP;

namespace DesktopClient
{
    public interface IUpdateTable
    {
        void Render();
        void UpdateData();
    }

    /// <summary>
    /// 表格基类
    /// </summary>
    public class Table : UserControl
    {
        public string SectionNumber { get; set; }
        public string Title { get; set; }

        protected Dictionary<UIElement, CellPosition> _cells = new Dictionary<UIElement, CellPosition>();
        protected string[] _lockedRows = new string[0];
        protected string[] _percentageRows = new string[0];

        public Brush Brush_TitleCell { get; set; }
        public Brush Brush_Title { get; set; }
        public Brush Brush_Cell { get; set; }
        public Brush Brush_CellBorder { get; set; }
        public Brush Brush_TableBorder { get; set; }
        public Brush Brush_EditCell { get; set; }
        public Brush Brush_ResultCell { get; set; }
        public Brush Brush_LabelCell { get; set; }

        public event EventHandler UserInput;

        public Table()
        {
            SectionNumber = string.Empty;
            Title = string.Empty;

            Brush_TitleCell = new LinearGradientBrush(Colors.DimGray, Color.FromRgb(40, 40, 40), 90);
            Brush_Title = Brushes.White;
            Brush_Cell = Brushes.Gray;
            Brush_CellBorder = Brushes.LightGray;
            Brush_TableBorder = Brushes.LightGray;
            Brush_EditCell = new LinearGradientBrush(Color.FromRgb(198, 239, 123), Colors.YellowGreen, 90);
            Brush_ResultCell = Brushes.Gray;
            Brush_LabelCell = new SolidColorBrush(Color.FromRgb(148, 148, 148));

            HorizontalAlignment = HorizontalAlignment.Left;
            Margin = new Thickness(0, 0, 0, 20);
        }

        public void LockRows(params string[] rows)
        {
            _lockedRows = rows;
        }

        public static string GetRowDescription(string variable)
        {
            return VarDescriptionManager.Descriptions[variable];
        }

        public static UIElement BuildTextBlock(string text, Brush background, Brush border, Brush foreground, bool centered = false)
        {
            if (text == "非数字")
            {
                text = "na";
            }
            TextBlock t = new TextBlock { Text = text, Foreground = foreground, Margin = new Thickness(3), TextWrapping = TextWrapping.Wrap, FontSize = 11 };
            double num = 0;
            if (double.TryParse(text, out num) || text == "na" || double.TryParse(text.Trim('%'), out num))
            {
                t.HorizontalAlignment = HorizontalAlignment.Right;
            }
            if (centered)
            {
                t.HorizontalAlignment = HorizontalAlignment.Center;
            }
            t.VerticalAlignment = VerticalAlignment.Center;
            Border b = new Border { Background = background, BorderBrush = border, BorderThickness = new Thickness(0.25), Child = t };
            return b;
        }

        public static UIElement BuildEmptyCell(Brush background, Brush border)
        {
            Border b = new Border { Background = background, BorderBrush = border, BorderThickness = new Thickness(0.25) };
            return b;
        }

        public static UIElement BuildTextBox(string text, Brush background)
        {
            TextBox t = new TextBox { Text = text, Background = background, BorderThickness = new Thickness(0), TextAlignment = TextAlignment.Right, TextWrapping = TextWrapping.Wrap, FontSize = 11 };
            return t;
        }

        /// <summary>
        /// 引发 UserInput 事件
        /// </summary>
        /// <param name="e">事件参数</param>
        public void OnUserInput(EventArgs e)
        {
            if (UserInput != null)
            {
                UserInput(this, e);
            }
        }

        public void SetPercentageRows(params string[] rows)
        {
            _percentageRows = rows;
        }

        protected bool IsRowEditable(string row)
        {
            return !_lockedRows.Contains(row);
        }
    }

    public static class LocalExtensions
    {
        /// <summary>
        /// 给数字进行三位分节
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns>输出</returns>
        public static string ThreeDigits(this string input)
        {
            string result = string.Empty;
            if (input.Contains('.'))
            {
                result = System.Text.RegularExpressions.Regex.Replace(input, @"\d+?(?=(?:\d{3})+\.)", "$0,");                 // 整数部分
                //result = System.Text.RegularExpressions.Regex.Replace(result, @"(?<=\.(?:\d{3})+)\d+?", ",$0").Trim(',');   // 小数部分
            }
            else
            {
                result = System.Text.RegularExpressions.Regex.Replace(input, @"\d+?(?=(?:\d{3})+$)", "$0,");
            }
            return result;
        }
    }

    /// <summary>
    /// YearwiseTable.xaml 的交互逻辑
    /// </summary>
    public partial class YearwiseTable : Table, IUpdateTable
    {
        public Dictionary<string, IYearwiseValue> Rows { get; private set; }
        public List<YearDefinition> Years { get; private set; }

        public YearwiseTable()
            : base()
        {
            InitializeComponent();
            Rows = new Dictionary<string, IYearwiseValue>();
            Years = new List<YearDefinition>();
        }

        public void Render()
        {
            _cells.Clear();
            LayoutRoot.Children.Clear();
            LayoutBorder.BorderBrush = Brush_TableBorder;
            LayoutRoot.RowDefinitions.Clear();
            LayoutRoot.ColumnDefinitions.Clear();

            Enumerable.Range(0, Rows.Count() + 2).ToList().ForEach(i => LayoutRoot.RowDefinitions.Add(new RowDefinition()));
            Enumerable.Range(0, Years.Count() + 2).ToList().ForEach(i => LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(GetColWidth(i)) }));
            LayoutRoot.RowDefinitions[0].Height = new GridLength(25);

            // Row 1
            _cells.Add(Table.BuildTextBlock(SectionNumber, Brush_TitleCell, Brush_TitleCell, Brush_Title, true), new CellPosition(0, 0));
            _cells.Add(Table.BuildTextBlock(Title, Brush_TitleCell, Brush_TitleCell, Brush_Title), new CellPosition(0, 1, 1, Years.Count + 1));

            // Row 2
            _cells.Add(Table.BuildEmptyCell(Brush_Cell, Brush_CellBorder), new CellPosition(1, 0));
            _cells.Add(Table.BuildEmptyCell(Brush_Cell, Brush_CellBorder), new CellPosition(1, 1));
            Enumerable.Range(0, Years.Count).ToList().ForEach(i => _cells.Add(Table.BuildTextBlock(GetYearHeader(Years[i].Year), Brush_Cell, Brush_CellBorder, Brushes.White, true), new CellPosition(1, i + 2)));

            // Rows
            int r = 2;
            foreach (var row in Rows)
            {
                _cells.Add(Table.BuildTextBlock(row.Key.Last().ToString(), Brush_Cell, Brush_CellBorder, Brushes.White, true), new CellPosition(r, 0));
                _cells.Add(Table.BuildTextBlock(Table.GetRowDescription(row.Key), Brush_LabelCell, Brush_CellBorder, Brushes.White), new CellPosition(r, 1));

                var prop = row.Key;
                var expr = row.Value;

                int c = 2;
                foreach (var year in Years)
                {
                    int y = DocumentManager.CurrentDocument.City.C02 + year.Year;
                    if (year.IsEditable && IsRowEditable(row.Key))
                    {
                        var valueString = FormatValue(prop, expr[y]);
                        TextBox tb = Table.BuildTextBox(valueString, Brush_EditCell) as TextBox;
                        tb.GotFocus += (sender, e) =>
                        {
                            (sender as TextBox).BorderBrush = Brushes.Black;
                            (sender as TextBox).BorderThickness = new Thickness(2);
                        };
                        tb.LostFocus += (sender, e) =>
                        {
                            (sender as TextBox).BorderBrush = Brushes.WhiteSmoke;
                            (sender as TextBox).BorderThickness = new Thickness(0);

                            double num = 0;
                            double.TryParse((sender as TextBox).Text, out num);
                            if ((sender as TextBox).Text.Contains('%'))
                            {
                                if (double.TryParse((sender as TextBox).Text.Trim('%'), out num))
                                {
                                    num = Convert.ToDouble((sender as TextBox).Text.Trim('%')) / 100.0;
                                }
                            }
                            expr[y] = num;
                            (sender as TextBox).Text = FormatValue(prop, num);
                            base.OnUserInput(e);
                        };
                        _cells.Add(tb, new CellPosition(r, c));
                    }
                    else
                    {
                        double value = 0;
                        if (year.Year == YearDefinition.CodeForAverage)
                        {
                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => row.Value[x.Year]).Where(x => !double.IsNaN(x));
                            value = temp.Count() > 0 ? temp.Average() : 0;
                            //value = row.Value.KnownYearsAverage;
                        }
                        else if (year.Year == YearDefinition.CodeForSum)
                        {
                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => row.Value[x.Year]).Where(x => !double.IsNaN(x));
                            value = temp.Count() > 0 ? temp.Sum() : 0;
                            //value = row.Value.KnownYearsSum;
                        }
                        else
                        {
                            value = row.Value.Year(y);
                        }
                        _cells.Add(Table.BuildTextBlock(FormatValue(prop, value), Brush_Cell, Brush_CellBorder, Brushes.White), new CellPosition(r, c));
                    }
                    c++;
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

        private string FormatValue(string prop, double value)
        {
            var valueString = value.ToString("0").ThreeDigits();
            if (_percentageRows.Contains(prop))
            {
                valueString = value.ToString("0.#%");
            }
            return valueString;
        }

        public void UpdateData()
        {
            // Rows
            int r = 2;
            foreach (var row in Rows)
            {
                var prop = row.Key;
                var expr = row.Value;

                int c = 2;
                foreach (var year in Years)
                {
                    int y = DocumentManager.CurrentDocument.City.C02 + year.Year;
                    if (year.IsEditable && IsRowEditable(row.Key))
                    {
                    }
                    else
                    {
                        double value = 0;
                        if (year.Year == YearDefinition.CodeForAverage)
                        {
                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => expr[x.Year]).Where(x => !double.IsNaN(x));
                            value = temp.Count() > 0 ? temp.Average() : 0;
                        }
                        else if (year.Year == YearDefinition.CodeForSum)
                        {
                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => expr[x.Year]).Where(x => !double.IsNaN(x));
                            value = temp.Count() > 0 ? temp.Sum() : 0;
                        }
                        else
                        {
                            value = expr.Year(y);
                        }
                        var border = _cells.First(x => x.Value.Row == r && x.Value.Col == c).Key as Border;
                        (border.Child as TextBlock).Text = double.IsNaN(value) ? "na" : FormatValue(prop, value);
                    }
                    c++;
                }
                r++;
            }
        }

        private string GetYearHeader(int year)
        {
            if (year == YearDefinition.CodeForAverage)
            {
                return "AVERAGE";
            }
            else if (year == YearDefinition.CodeForSum)
            {
                return "TOTAL";
            }
            else
            {
                return (DocumentManager.CurrentDocument.City.C02 + year).ToString();
            }
        }

        private double GetColWidth(int col)
        {
            if (col == 1)
            {
                if (Years.Count < 8)
                {
                    return 300;
                }
            }
            double[] colWidths = { 30, 100, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59 };
            return colWidths[col];
        }
    }

    public class YearDefinition
    {
        public int Year { get; set; }
        public bool IsEditable { get; set; }
        public bool IsFuture { get; set; }

        public const int CodeForSum = 1002;
        public const int CodeForAverage = 1001;

        public YearDefinition(int year, bool isEditable, bool isFuture)
        {
            Year = year;
            IsEditable = isEditable;
            IsFuture = isFuture;
        }

        public static YearDefinition Average
        {
            get
            {
                return new YearDefinition(CodeForAverage, false, true);
            }
        }

        public static YearDefinition Sum
        {
            get
            {
                return new YearDefinition(CodeForSum, false, true);
            }
        }

        public static bool IsNormalYear(int year)
        {
            return year != CodeForSum && year != CodeForAverage;
        }
    }

    public class CellPosition
    {
        public int Row;
        public int Col;
        public int RowSpan = 1;
        public int ColSpan = 1;

        public CellPosition(int row, int col, int rowSpan = 1, int colSpan = 1)
        {
            Row = row;
            Col = col;
            RowSpan = rowSpan;
            ColSpan = colSpan;
        }
    }
}
