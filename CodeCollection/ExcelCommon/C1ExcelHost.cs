using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using C1.C1Excel;

namespace C1.Web.Api.Excel
{
    public class C1ExcelHost : IExcelHost, IExcelHostOperations
    {
        Workbook IExcelHost.Read(Stream file)
        {
            var host = new C1XLBook();
            host.Load(file);
            return host.ToWorkbook();
        }

        void IExcelHost.Write(Workbook workbook, Stream file)
        {
            var host = workbook.ToC1XLBook();
            host.Save(file);
        }
    }

    public class C1ExcelHostOperations
    {
        private C1XLBook _host;
        private Workbook _workbook;

        public C1ExcelHostOperations(C1XLBook host, Workbook workbook)
        {
            _host = host;
            _workbook = workbook;
        }
    }

    internal static class C1ExcelHostExtensions
    {
        internal const int DefaultColumnWidth = 100;
        internal const int DefaultRowHeight = 20;

        public static C1XLBook ToC1XLBook(this Workbook workbook)
        {
            var host = new C1XLBook();
            workbook.Sheets.Select((sheet, index) => host.Sheets.Add(sheet.ToXLSheet(host, index))).ToList();
            return host;
        }

        public static Workbook ToWorkbook(this C1XLBook host)
        {
            var workbook = new Workbook();
            workbook.Sheets = host.Sheets.Cast<XLSheet>().Select(sheet => sheet.ToWorksheet()).ToList();
            return workbook;
        }

        public static XLSheet ToXLSheet(this Worksheet worksheet, C1XLBook host, int sheetIndex)
        {
            int nRows = worksheet.Data.Count;
            int nCols = worksheet.Data.Max(x => x.Count);

            // Set sheet-wide properties
            var c1Sheet = sheetIndex < host.Sheets.Count ? host.Sheets[sheetIndex] : host.Sheets.Add();
            c1Sheet.Name = worksheet.Name;
            c1Sheet.DefaultColumnWidth = worksheet.DefaultColumnWidth ?? DefaultColumnWidth;
            c1Sheet.DefaultRowHeight = worksheet.DefaultRowHeight ?? DefaultRowHeight;

            // Rows and columns
            c1Sheet.Rows.Clear();
            foreach (var row in worksheet.Rows)
            {
                c1Sheet.Rows.Add(new XLRow
                {
                    Height = row.Height ?? c1Sheet.DefaultRowHeight,
                    Style = row.Style.ToXLStyle(host)
                });
            }
            c1Sheet.Columns.Clear();
            foreach (var col in worksheet.Colunms)
            {
                c1Sheet.Columns.Add(new XLColumn
                {
                    Width = col.Width ?? c1Sheet.DefaultColumnWidth,
                    Style = col.Style.ToXLStyle(host)
                });
            }

            // Fill the table
            for (int i = 0; i < nRows; i++)
            {
                for (int j = 0; j < nCols; j++)
                {
                    if (j < worksheet.Data[i].Count)
                    {
                        c1Sheet[i, j].Value = worksheet.Data[i][j];
                    }
                }
            }

            // Global style
            var tableRange = new XLCellRange(c1Sheet, 0, nRows - 1, 0, nCols - 1);
            tableRange.Style = worksheet.GlobalStyle.ToXLStyle(host);

            // Range-based styles
            foreach (var rs in worksheet.RangeStyles)
            {
                var styleRange = new XLCellRange(c1Sheet, rs.Range.From.Row, rs.Range.To.Row, rs.Range.From.Col, rs.Range.To.Col);
                styleRange.Style = rs.Style.ToXLStyle(host);

                // Merged cells
                if (rs.Style.MergedCells == true)
                {
                    c1Sheet.MergedCells.Add(styleRange);
                }
            }

            return c1Sheet;
        }

        public static Worksheet ToWorksheet(this XLSheet sheet)
        {
            throw new NotImplementedException();
        }

        public static XLStyle ToXLStyle(this Style style, C1XLBook host)
        {
            var c1Style = new XLStyle(host);
            if (style.NumberFormat.HasValue)
            {
                c1Style.Format = style.NumberFormat.Value.ToString();
            }
            if (style.HAlign.HasValue)
            {
                c1Style.AlignHorz = style.HAlign.Value.ToXLAlignH();
            }
            if (style.VAlign.HasValue)
            {
                c1Style.AlignVert = style.VAlign.Value.ToXLAlignV();
            }
            // TODO: other styles
            return c1Style;
        }

        public static XLAlignHorzEnum ToXLAlignH(this HAlign align)
        {
            XLAlignHorzEnum result;
            if (Enum.TryParse(align.ToString(), out result))
            {
                return result;
            }
            else
            {
                return XLAlignHorzEnum.Undefined;
            }
        }

        public static XLAlignVertEnum ToXLAlignV(this VAlign align)
        {
            XLAlignVertEnum result;
            if (Enum.TryParse(align.ToString(), out result))
            {
                return result;
            }
            else
            {
                return XLAlignVertEnum.Undefined;
            }
        }
    }
}
