using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CIIPP
{
    public class ExcelTableRegion
    {
        //public int Columns { get; set; }
        //public List<Cell> Cells { get; private set; }

        public Dictionary<Cell, CellPosition> Cells { get; private set; }

        public ExcelTableRegion()
        {
            Cells = new Dictionary<Cell, CellPosition>();
        }
    }

    public static class ExcelTableHelper
    {
        public static void InsertTableRegion(WorksheetPart worksheetPart, ExcelTableRegion region, int upperRow, int leftColumn)
        {
            SheetData data = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            Dictionary<int, Row> rows = new Dictionary<int, Row>();
            foreach (var cell in region.Cells)
            {
                int r = cell.Value.Row + upperRow;
                int c = cell.Value.Col + leftColumn;
                cell.Key.CellReference = getColumnName(c) + r;
                if (rows.ContainsKey(r))
                {
                    //rows[r] = data.Elements<Row>().Where(x => x.RowIndex == (uint)r).First();
                }
                else
                {
                    rows[r] = new Row() { RowIndex = (uint)r, };
                }
                rows[r].AppendChild(cell.Key);
            }
            rows.ToList().ForEach(x => data.AppendChild(x.Value));
        }

        private static string getColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName =
                    Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }
    }

    public class CellPosition
    {
        public int Row{get;set;}
        public int Col{get;set;}
        public CellPosition(int row,int col)
        {
            Row = row;
            Col = col;
        }
    }
}
