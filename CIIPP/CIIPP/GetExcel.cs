using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CIIPP
{
    public static class GetExcel
    {
        public static int GetSheetsNum(string strFileName)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(strFileName, false))
            {
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();
                return sheets.Count();
            }
        }

        public static string GetExcelValueById(uint rowIndex, string columnName, WorkbookPart workbookPart, uint i)//由坐标取值
        {
            IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.SheetId == i);
            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheets.First().Id);
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData data = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex == rowIndex);
            List<string> cellText = new List<string>();
            foreach (Row row in rows)
            {
                foreach (Cell cell in row)
                {
                    if (cell != null && cell.CellReference != null)
                    {
                        if (cell.CellReference.Value == columnName + rowIndex)
                        {
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                SharedStringTablePart shareStringPart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();
                                SharedStringItem[] items = shareStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();
                                cellText.Add(items[int.Parse(cell.CellValue.Text)].InnerText);
                            }
                            else
                            {
                                cellText.Add(cell.InnerText);
                            }
                        }
                    }
                }
            }
            if (cellText == null || cellText.Count == 0)
            {
                return null;
            }
            else
            {
                return cellText.First();
            }
        }

        public static string MatchExcelById(string cellText, WorkbookPart workbookPart, uint i) //由值取坐标
        {
            IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.SheetId == i);
            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheets.First().Id);
            IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();
            foreach (Row row in rows)
            {
                foreach (Cell cell in row)
                {
                    if (cell.CellValue != null)
                    {
                        if (cell.CellValue.Text == cellText)
                        {
                            return cell.CellReference;
                        }
                    }
                    if (cell.InlineString != null)
                    {
                        if (cell.InlineString.Text.ToString() == cellText)
                        {
                            return cell.CellReference;
                        }
                    }
                    if (cell.InnerText != null)
                    {
                        if (cell.InnerText == cellText)
                        {
                            return cell.CellReference;
                        }
                    }                    
                }
            }
            return "A1";
        }

        public static string GetExcelValueByName(uint rowIndex, string columnName, string docName, string sheetName)//由坐标取值
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, false))
            {
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName.Trim());
                if (sheets.Count() == 0)
                {
                    return null;
                }
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);
                Worksheet worksheet = worksheetPart.Worksheet;
                SheetData data = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>().Where(r => r.RowIndex == rowIndex);
                List<string> cellText = new List<string>();
                foreach (Row row in rows)
                {
                    foreach (Cell cell in row)
                    {
                        if (cell != null && cell.CellReference != null)
                        {
                            if (cell.CellReference.Value == columnName + rowIndex)
                            {
                                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                                {
                                    SharedStringTablePart shareStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                                    SharedStringItem[] items = shareStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();
                                    cellText.Add(items[int.Parse(cell.CellValue.Text)].InnerText);
                                }
                                else
                                {
                                    cellText.Add(cell.InlineString.Text.ToString());
                                }
                            }
                        }
                    }
                }
                if (cellText == null || cellText.Count == 0)
                {
                    return null;
                }
                else
                {
                    return cellText.First();
                }
            }
        }

        public static string MatchExcelByName(string cellText, string docName, string sheetName) //由值取坐标
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, false))
            {
                IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName.Trim());
                if (sheets.Count() == 0)
                {
                    return "A1";
                }
                WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);

                IEnumerable<Row> rows = worksheetPart.Worksheet.Descendants<Row>();

                foreach (Row row in rows)
                {
                    foreach (Cell cell in row)
                    {
                        if (cell.CellValue != null)
                        {
                            if (cell.CellValue.Text == cellText)
                            {
                                return cell.CellReference;
                            }
                        }
                        if (cell.InlineString != null)
                        {
                            if (cell.InlineString.Text.ToString() == cellText)
                            {
                                return cell.CellReference;
                            }
                        }
                        if (cell.InnerText != null)
                        {
                            if (cell.InnerText == cellText)
                            {
                                return cell.CellReference;
                            }
                        }
                    }
                }
                return "A1";
            }
        }

        //public static string GetColumnHeading(string docName, string worksheetName, string cellName)
        //{
        //    // Open the document as read-only.
        //    using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, false))
        //    {
        //        IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == worksheetName);
        //        if (sheets.Count() == 0)
        //        {
        //            // The specified worksheet does not exist.
        //            return "0";
        //        }

        //        WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);

        //        // Get the column name for the specified cell.
        //        string columnName = GetColumnName(cellName);

        //        // Get the cells in the specified column and order them by row.
        //        IEnumerable<Cell> cells = worksheetPart.Worksheet.Descendants<Cell>().Where(c => string.Compare(GetColumnName(c.CellReference.Value), columnName, true) == 0)
        //                                                                                                                    .OrderBy(r => GetRowIndex(r.CellReference));

        //        if (cells.Count() == 0)
        //        {
        //            // The specified column does not exist.
        //            return "1";
        //        }

        //        // Get the first cell in the column.
        //        Cell headCell = cells.First();

        //        // If the content of the first cell is stored as a shared string, get the text of the first cell
        //        // from the SharedStringTablePart and return it. Otherwise, return the string value of the cell.
        //        if (headCell.DataType != null && headCell.DataType.Value == CellValues.SharedString)
        //        {
        //            SharedStringTablePart shareStringPart = document.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
        //            SharedStringItem[] items = shareStringPart.SharedStringTable.Elements<SharedStringItem>().ToArray();
        //            return items[int.Parse(headCell.CellValue.Text)].InnerText;
        //        }
        //        else
        //        {
        //            return headCell.CellValue.Text;
        //        }
        //    }
        //}

        public static string GetColumnName(string cellName)//列号
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }

        public static uint GetRowIndex(string cellName)//行号
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellName);

            return uint.Parse(match.Value);
        }
    }
}
