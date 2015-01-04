//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using CIIPP;

//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;

//namespace DesktopClient
//{
//    public class YearTable : Table
//    {
//        //public Dictionary<string, IYearwiseValue> Rows { get; private set; }
//        public List<string> Rows { get; private set; }
//        public List<YearDefinition> Years { get; private set; }
//        public Grid LayoutRoot { get; set; }

//        public YearTable()
//            : base()
//        {
//            //Rows = new Dictionary<string, IYearwiseValue>();
//            Rows = new List<string>();
//            Years = new List<YearDefinition>();

//            LayoutRoot = new Grid();
//            this.Content = LayoutRoot;
//        }

//        private bool IsRowEditable(string row)
//        {
//            return !_lockedRows.Contains(row);
//        }

//        public void Render()
//        {
//            _cells.Clear();
//            LayoutRoot.Children.Clear();
//            //LayoutBorder.BorderBrush = Brush_TableBorder;
//            LayoutRoot.RowDefinitions.Clear();
//            LayoutRoot.ColumnDefinitions.Clear();

//            Enumerable.Range(0, Rows.Count() + 2).ToList().ForEach(i => LayoutRoot.RowDefinitions.Add(new RowDefinition()));
//            Enumerable.Range(0, Years.Count() + 2).ToList().ForEach(i => LayoutRoot.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(GetColWidth(i)) }));

//            // Row 1
//            _cells.Add(Table.BuildTextBlock(SectionNumber, Brush_TitleCell, Brush_TitleCell, Brush_Title, true), new CellPosition(0, 0));
//            _cells.Add(Table.BuildTextBlock(Title, Brush_TitleCell, Brush_TitleCell, Brush_Title), new CellPosition(0, 1, 1, Years.Count + 1));

//            // Row 2
//            _cells.Add(Table.BuildEmptyCell(Brush_Cell, Brush_CellBorder), new CellPosition(1, 0));
//            _cells.Add(Table.BuildEmptyCell(Brush_Cell, Brush_CellBorder), new CellPosition(1, 1));
//            Enumerable.Range(0, Years.Count).ToList().ForEach(i => _cells.Add(Table.BuildTextBlock(GetYearHeader(Years[i].Year), Brush_Cell, Brush_CellBorder, Brushes.White, true), new CellPosition(1, i + 2)));

//            // Rows
//            int r = 2;
//            foreach (var row in Rows)
//            {
//                _cells.Add(Table.BuildTextBlock(row.Last().ToString(), Brush_Cell, Brush_CellBorder, Brushes.White, true), new CellPosition(r, 0));
//                _cells.Add(Table.BuildTextBlock(Table.GetRowDescription(row), Brush_Cell, Brush_CellBorder, Brushes.White), new CellPosition(r, 1));

//                var prop = row;
//                //var expr = row.Value;

//                int c = 2;
//                foreach (var year in Years)
//                {
//                    int y = DocumentManager.CurrentDocument.City.C02 + year.Year;
//                    if (year.IsEditable && IsRowEditable(prop))
//                    {
//                        var valueString = FormatValue(prop, expr[y]);
//                        TextBox tb = Table.BuildTextBox(valueString, Brush_EditCell) as TextBox;
//                        tb.GotFocus += (sender, e) =>
//                        {
//                            (sender as TextBox).BorderBrush = Brushes.Black;
//                            (sender as TextBox).BorderThickness = new Thickness(2);
//                        };
//                        tb.LostFocus += (sender, e) =>
//                        {
//                            (sender as TextBox).BorderBrush = Brushes.WhiteSmoke;
//                            (sender as TextBox).BorderThickness = new Thickness(1);

//                            double num = 0;
//                            double.TryParse((sender as TextBox).Text, out num);
//                            if ((sender as TextBox).Text.Contains('%'))
//                            {
//                                num = Convert.ToDouble((sender as TextBox).Text.Trim('%')) / 100.0;
//                            }
//                            expr[y] = num;
//                            (sender as TextBox).Text = FormatValue(prop, num);
//                            base.OnUserInput(e);
//                        };
//                        _cells.Add(tb, new CellPosition(r, c));
//                    }
//                    else
//                    {
//                        double value = 0;
//                        if (year.Year == YearDefinition.CodeForAverage)
//                        {
//                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => row.Value[x.Year]).Where(x => !double.IsNaN(x));
//                            value = temp.Count() > 0 ? temp.Average() : 0;
//                            //value = row.Value.KnownYearsAverage;
//                        }
//                        else if (year.Year == YearDefinition.CodeForSum)
//                        {
//                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => row.Value[x.Year]).Where(x => !double.IsNaN(x));
//                            value = temp.Count() > 0 ? temp.Sum() : 0;
//                            //value = row.Value.KnownYearsSum;
//                        }
//                        else
//                        {
//                            value = row.Value.Year(y);
//                        }
//                        _cells.Add(Table.BuildTextBlock(FormatValue(prop, value), Brush_Cell, Brush_CellBorder, Brushes.White), new CellPosition(r, c));
//                    }
//                    c++;
//                }

//                r++;
//            }

//            foreach (var cell in _cells)
//            {
//                Grid.SetRow(cell.Key, cell.Value.Row);
//                Grid.SetRowSpan(cell.Key, cell.Value.RowSpan);
//                Grid.SetColumn(cell.Key, cell.Value.Col);
//                Grid.SetColumnSpan(cell.Key, cell.Value.ColSpan);
//                LayoutRoot.Children.Add(cell.Key);
//            }
//        }

//        private string FormatValue(string prop, double value)
//        {
//            var valueString = value.ToString("0").ThreeDigits();
//            if (_percentageRows.Contains(prop))
//            {
//                valueString = value.ToString("0.#%");
//            }
//            return valueString;
//        }

//        public void UpdateData()
//        {
//            // Rows
//            int r = 2;
//            foreach (var row in Rows)
//            {
//                var prop = row.Key;
//                var expr = row.Value;

//                int c = 2;
//                foreach (var year in Years)
//                {
//                    int y = DocumentManager.CurrentDocument.City.C02 + year.Year;
//                    if (year.IsEditable && IsRowEditable(row.Key))
//                    {
//                    }
//                    else
//                    {
//                        double value = 0;
//                        if (year.Year == YearDefinition.CodeForAverage)
//                        {
//                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => expr[x.Year]).Where(x => !double.IsNaN(x));
//                            value = temp.Count() > 0 ? temp.Average() : 0;
//                        }
//                        else if (year.Year == YearDefinition.CodeForSum)
//                        {
//                            var temp = Years.Where(x => YearDefinition.IsNormalYear(x.Year)).Select(x => expr[x.Year]).Where(x => !double.IsNaN(x));
//                            value = temp.Count() > 0 ? temp.Sum() : 0;
//                        }
//                        else
//                        {
//                            value = expr.Year(y);
//                        }
//                        var border = _cells.First(x => x.Value.Row == r && x.Value.Col == c).Key as Border;
//                        (border.Child as TextBlock).Text = double.IsNaN(value) ? "na" : FormatValue(prop, value);
//                    }
//                    c++;
//                }
//                r++;
//            }
//        }

//        private string GetYearHeader(int year)
//        {
//            if (year == YearDefinition.CodeForAverage)
//            {
//                return "AVERAGE";
//            }
//            else if (year == YearDefinition.CodeForSum)
//            {
//                return "TOTAL";
//            }
//            else
//            {
//                return (DocumentManager.CurrentDocument.City.C02 + year).ToString();
//            }
//        }

//        private double GetColWidth(int col)
//        {
//            if (col == 1)
//            {
//                if (Years.Count < 8)
//                {
//                    return 300;
//                }
//            }
//            double[] colWidths = { 30, 100, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59, 59 };
//            return colWidths[col];
//        }
//    }
//}
