using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CIIPP
{
    public static class ExportExcel
    {
        public static void ExportExcelT(string filepath)
        {
            SpreadsheetDocument document = SpreadsheetDocument.Create(filepath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = document.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            WorksheetPart worksheetPart1 = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart1.Worksheet = new Worksheet(new SheetData());

            WorksheetPart worksheetPart2 = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart2.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet0 = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart2), SheetId = 1, Name = "Home" };
            Sheet sheet1 = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 2, Name = "City" };
            Sheet sheet2 = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart1), SheetId = 3, Name = "Budget" };
            sheets.Append(sheet0);
            sheets.Append(sheet1);
            sheets.Append(sheet2);

            #region 全局设定
            TitleStyle(worksheetPart2.Worksheet, worksheetPart2, "全局设定", 1);
            ExcelTableRegion etrg = new ExcelTableRegion();
            AddRow1(etrg, "Country", 0, -2);
            AddRow1(etrg, "A", 0, -1);
            AddRow1(etrg, "所在国家", 0, 0);
            AddRowString(etrg, DocumentManager.CurrentDocument.City.Country, 0);
            AddRow1(etrg, "Intro", 1, -2);
            AddRow1(etrg, "B", 1, -1);
            AddRow1(etrg, "介绍", 1, 0);
            AddRowString(etrg, DocumentManager.CurrentDocument.City.Intro, 1);
            AddRow1(etrg, "Currency", 2, -2);
            AddRow1(etrg, "C", 2, -1);
            AddRow1(etrg, "货币", 2, 0);
            AddRowString(etrg, CityStatistics.CurrencyEnum[DocumentManager.CurrentDocument.City.Currency], 2);
            AddRow1(etrg, "Multiple", 3, -2);
            AddRow1(etrg, "D", 3, -1);
            AddRow1(etrg, "倍数", 3, 0);
            AddRowString(etrg, CityStatistics.MultipleEnum[DocumentManager.CurrentDocument.City.Multiple], 3);
            ExcelTableHelper.InsertTableRegion(worksheetPart2, etrg, 2, 3);
            #endregion

            int years = DocumentManager.CurrentDocument.City.C02;

            #region 当地政府关键数据

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "1.0当地政府关键数据", 1);
            ExcelTableRegion etr4 = new ExcelTableRegion();
            AddRow1(etr4, "C01", 0, -2);
            AddRow1(etr4, "A", 0, -1);
            AddRow1(etr4, "Name of city", 0, 0);
            AddRowString(etr4, DocumentManager.CurrentDocument.City.C01, 0);
            AddRow1(etr4, "C02", 1, -2);
            AddRow1(etr4, "B", 1, -1);
            AddRow1(etr4, "Latest year", 1, 0);
            AddRowString(etr4, DocumentManager.CurrentDocument.City.C02.ToString(), 1);
            AddRow1(etr4, "C03", 2, -2);
            AddRow1(etr4, "C", 2, -1);
            AddRow1(etr4, "Populaton size", 2, 0);
            AddRowString(etr4, DocumentManager.CurrentDocument.City.C03.ToString(), 2);
            AddRow1(etr4, "C04", 3, -2);
            AddRow1(etr4, "D", 3, -1);
            AddRow1(etr4, "Annual population growth", 3, 0);
            AddRowString(etr4, DocumentManager.CurrentDocument.City.C04.ToString(), 3);
            AddRow1(etr4, "C05", 4, -2);
            AddRow1(etr4, "E", 4, -1);
            AddRow1(etr4, "Date", 4, 0);
            AddRowString(etr4, DocumentManager.CurrentDocument.City.C05.ToString(), 4);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr4, 2, 3);

            #endregion

            #region 当地政府收入

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "1.1当地政府收入", 8);
            InsertCell(worksheetPart, "A", 9, "");
            InsertCell(worksheetPart, "B", 9, "");
            InsertCell(worksheetPart, "C", 9, "");
            InsertCell(worksheetPart, "D", 9, (years - 3).ToString());
            InsertCell(worksheetPart, "E", 9, (years - 2).ToString());
            InsertCell(worksheetPart, "F", 9, (years - 1).ToString());
            InsertCell(worksheetPart, "G", 9, years.ToString());
            ExcelTableRegion etr = new ExcelTableRegion();
            AddRow1(etr, "C1A", 0, -2);
            AddRow1(etr, "A", 0, -1);
            AddRow1(etr, "经常性收入（市政税基）：", 0, 0);
            AddRow(etr, DocumentManager.CurrentDocument.City.C1A, 0, -3, 4);
            AddRow1(etr, "C1B", 1, -2);
            AddRow1(etr, "B", 1, -1);
            AddRow1(etr, "经常性收入（用户费用及罚款）：", 1, 0);
            AddRow(etr, DocumentManager.CurrentDocument.City.C1B, 1, -3, 4);
            AddRow1(etr, "C1C", 2, -2);
            AddRow1(etr, "C", 2, -1);
            AddRow1(etr, "资本收入（共享收入）：", 2, 0);
            AddRow(etr, DocumentManager.CurrentDocument.City.C1C, 2, -3, 4);
            AddRow1(etr, "C1D", 3, -2);
            AddRow1(etr, "D", 3, -1);
            AddRow1(etr, "资本收入（专项拨款）：", 3, 0);
            AddRow(etr, DocumentManager.CurrentDocument.City.C1D, 3, -3, 4);
            AddRow1(etr, "C1E", 4, -2);
            AddRow1(etr, "E", 4, -1);
            AddRow1(etr, "资本收入（土地/市政资产出售所得）：", 4, 0);
            AddRow(etr, DocumentManager.CurrentDocument.City.C1E, 4, -3, 4);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr, 10, 3);

            #endregion

            #region 当地政府支出

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "1.2当地政府支出", 16);
            InsertCell(worksheetPart, "A", 17, "");
            InsertCell(worksheetPart, "B", 17, "");
            InsertCell(worksheetPart, "C", 17, "");
            InsertCell(worksheetPart, "D", 17, (years - 3).ToString());
            InsertCell(worksheetPart, "E", 17, (years - 2).ToString());
            InsertCell(worksheetPart, "F", 17, (years - 1).ToString());
            InsertCell(worksheetPart, "G", 17, years.ToString());
            ExcelTableRegion etr1 = new ExcelTableRegion();
            AddRow1(etr1, "C2A", 0, -2);
            AddRow1(etr1, "A", 0, -1);
            AddRow1(etr1, "资本支出（投资）：", 0, 0);
            AddRow(etr1, DocumentManager.CurrentDocument.City.C2A, 0, -3, 4);
            AddRow1(etr1, "C2B", 1, -2);
            AddRow1(etr1, "B", 1, -1);
            AddRow1(etr1, "经常性支出（运营/维护）：", 1, 0);
            AddRow(etr1, DocumentManager.CurrentDocument.City.C2B, 1, -3, 4);
            AddRow1(etr1, "C2C", 2, -2);
            AddRow1(etr1, "C", 2, -1);
            AddRow1(etr1, "经常性支出（还本付息）：", 2, 0);
            AddRow(etr1, DocumentManager.CurrentDocument.City.C2C, 2, -3, 4);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr1, 18, 3);

            #endregion

            #region 当地政府资产
            TitleStyle(worksheetPart.Worksheet, worksheetPart, "1.3当地政府资产", 23);
            InsertCell(worksheetPart, "A", 24, "");
            InsertCell(worksheetPart, "B", 24, "");
            InsertCell(worksheetPart, "C", 24, "");
            InsertCell(worksheetPart, "D", 24, (years - 3).ToString());
            InsertCell(worksheetPart, "E", 24, (years - 2).ToString());
            InsertCell(worksheetPart, "F", 24, (years - 1).ToString());
            InsertCell(worksheetPart, "G", 24, years.ToString());
            ExcelTableRegion etr2 = new ExcelTableRegion();
            AddRow1(etr2, "C3A", 0, -2);
            AddRow1(etr2, "A", 0, -1);
            AddRow1(etr2, "现金", 0, 0);
            AddRow(etr2, DocumentManager.CurrentDocument.City.C3A, 0, -3, 4);
            AddRow1(etr2, "C3B", 1, -2);
            AddRow1(etr2, "B", 1, -1);
            AddRow1(etr2, "证券", 1, 0);
            AddRow(etr2, DocumentManager.CurrentDocument.City.C3B, 1, -3, 4);
            AddRow1(etr2, "C3C", 2, -2);
            AddRow1(etr2, "C", 2, -1);
            AddRow1(etr2, "长期债券", 2, 0);
            AddRow(etr2, DocumentManager.CurrentDocument.City.C3C, 2, -3, 4);
            AddRow1(etr2, "C3D", 3, -2);
            AddRow1(etr2, "D", 3, -1);
            AddRow1(etr2, "其他有形资产", 3, 0);
            AddRow(etr2, DocumentManager.CurrentDocument.City.C3D, 3, -3, 4);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr2, 25, 3);
            #endregion

            #region 当地政府债务
            TitleStyle(worksheetPart.Worksheet, worksheetPart, "1.4当地政府债务", 30);
            InsertCell(worksheetPart, "A", 31, "");
            InsertCell(worksheetPart, "B", 31, "");
            InsertCell(worksheetPart, "C", 31, "");
            InsertCell(worksheetPart, "D", 31, (years - 3).ToString());
            InsertCell(worksheetPart, "E", 31, (years - 2).ToString());
            InsertCell(worksheetPart, "F", 31, (years - 1).ToString());
            InsertCell(worksheetPart, "G", 31, (years - 0).ToString());
            InsertCell(worksheetPart, "H", 31, (years + 1).ToString());
            InsertCell(worksheetPart, "I", 31, (years + 2).ToString());
            InsertCell(worksheetPart, "J", 31, (years + 3).ToString());
            InsertCell(worksheetPart, "K", 31, (years + 4).ToString());
            InsertCell(worksheetPart, "L", 31, (years + 5).ToString());
            InsertCell(worksheetPart, "M", 31, (years + 6).ToString());
            InsertCell(worksheetPart, "N", 31, (years + 7).ToString());
            InsertCell(worksheetPart, "O", 31, (years + 8).ToString());
            InsertCell(worksheetPart, "P", 31, (years + 9).ToString());
            InsertCell(worksheetPart, "Q", 31, (years + 10).ToString());
            ExcelTableRegion etr3 = new ExcelTableRegion();
            AddRow1(etr3, "C4A", 0, -2);
            AddRow1(etr3, "A", 0, -1);
            AddRow1(etr3, "未偿还贷款额", 0, 0);
            AddRow(etr3, DocumentManager.CurrentDocument.City.C4A, 0, -3, 14);
            AddRow1(etr3, "C4B", 1, -2);
            AddRow1(etr3, "B", 1, -1);
            AddRow1(etr3, "年度利息付款", 1, 0);
            AddRow(etr3, DocumentManager.CurrentDocument.City.C4B, 1, -3, 14);
            AddRow1(etr3, "C4C", 2, -2);
            AddRow1(etr3, "C", 2, -1);
            AddRow1(etr3, "年度本金付款", 2, 0);
            AddRow(etr3, DocumentManager.CurrentDocument.City.C4C, 2, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr3, 32, 3);
            #endregion

            #region 城市问卷
            TitleStyle(worksheetPart.Worksheet, worksheetPart, "城市问卷", 37);
            ExcelTableRegion etrc = new ExcelTableRegion();
            AddRow1(etrc, "CQ01", 0, -2);
            AddRow1(etrc, "A", 0, -1);
            AddRow1(etrc, "信用评级", 0, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ01"].ToString(), 0);
            AddRow1(etrc, "CQ02", 1, -2);
            AddRow1(etrc, "B", 1, -1);
            AddRow1(etrc, "资本预算", 1, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ02"].ToString(), 1);
            AddRow1(etrc, "CQ03", 2, -2);
            AddRow1(etrc, "C", 2, -1);
            AddRow1(etrc, "什么是征管效率：=所征缴收入/应征缴收入总额？", 2, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ03"].ToString(), 2);
            AddRow1(etrc, "CQ04", 3, -2);
            AddRow1(etrc, "D", 3, -1);
            AddRow1(etrc, "如无确切数据：当地政府收入征管能力的发展状况如何？", 3, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ04"].ToString(), 3);
            AddRow1(etrc, "CQ05", 4, -2);
            AddRow1(etrc, "E", 4, -1);
            AddRow1(etrc, "法律和行政框架质量", 4, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ05"].ToString(), 4);
            AddRow1(etrc, "CQ06", 5, -2);
            AddRow1(etrc, "F", 5, -1);
            AddRow1(etrc, "信息技术能力", 5, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ06"].ToString(), 5);
            AddRow1(etrc, "CQ07", 6, -2);
            AddRow1(etrc, "G", 6, -1);
            AddRow1(etrc, "工作人员的能力", 6, 0);
            AddRowString(etrc, DocumentManager.CurrentDocument.City.Questionnaire["CQ07"].ToString(), 6);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etrc, 38, 3);
            #endregion

            #region 当地政府收入
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.6当地政府收入", 1);
            InsertCell(worksheetPart1, "A", 2, "");
            InsertCell(worksheetPart1, "B", 2, "");
            InsertCell(worksheetPart1, "C", 2, "");
            InsertCell(worksheetPart1, "D", 2, (years - 3).ToString());
            InsertCell(worksheetPart1, "E", 2, (years - 2).ToString());
            InsertCell(worksheetPart1, "F", 2, (years - 1).ToString());
            InsertCell(worksheetPart1, "G", 2, (years - 0).ToString());
            InsertCell(worksheetPart1, "H", 2, (years + 1).ToString());
            InsertCell(worksheetPart1, "I", 2, (years + 2).ToString());
            InsertCell(worksheetPart1, "J", 2, (years + 3).ToString());
            InsertCell(worksheetPart1, "K", 2, (years + 4).ToString());
            InsertCell(worksheetPart1, "L", 2, (years + 5).ToString());
            InsertCell(worksheetPart1, "M", 2, (years + 6).ToString());
            InsertCell(worksheetPart1, "N", 2, (years + 7).ToString());
            InsertCell(worksheetPart1, "O", 2, (years + 8).ToString());
            InsertCell(worksheetPart1, "P", 2, (years + 9).ToString());
            InsertCell(worksheetPart1, "Q", 2, (years + 10).ToString());
            ExcelTableRegion betr = new ExcelTableRegion();
            AddRow1(betr, "C6A", 0, -2);
            AddRow1(betr, "A", 0, -1);
            AddRow1(betr, "经常性收入：一般收入", 0, 0);
            AddRowD(betr, DocumentManager.CurrentDocument.City.C6A, 0, -3, 14);
            AddRow1(betr, "C6B", 1, -2);
            AddRow1(betr, "B", 1, -1);
            AddRow1(betr, "经常性收入：用户费用及罚款", 1, 0);
            AddRowD(betr, DocumentManager.CurrentDocument.City.C6B, 1, -3, 14);
            AddRow1(betr, "C6C", 2, -2);
            AddRow1(betr, "C", 2, -1);
            AddRow1(betr, "资本收入：共享收入/税收和一般性拨款", 2, 0);
            AddRowD(betr, DocumentManager.CurrentDocument.City.C6C, 2, -3, 14);
            AddRow1(betr, "C6D", 3, -2);
            AddRow1(betr, "D", 3, -1);
            AddRow1(betr, "资本收入：专项拨款", 3, 0);
            AddRowD(betr, DocumentManager.CurrentDocument.City.C6D, 3, -3, 14);
            AddRow1(betr, "C6E", 4, -2);
            AddRow1(betr, "E", 4, -1);
            AddRow1(betr, "资本收入：市政资产出售所得", 4, 0);
            AddRowD(betr, DocumentManager.CurrentDocument.City.C6E, 4, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr, 3, 3);
            #endregion

            #region 当地政府支出
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.7当地政府支出", 10);
            InsertCell(worksheetPart1, "A", 11, "");
            InsertCell(worksheetPart1, "B", 11, "");
            InsertCell(worksheetPart1, "C", 11, "");
            InsertCell(worksheetPart1, "D", 11, (years - 3).ToString());
            InsertCell(worksheetPart1, "E", 11, (years - 2).ToString());
            InsertCell(worksheetPart1, "F", 11, (years - 1).ToString());
            InsertCell(worksheetPart1, "G", 11, (years - 0).ToString());
            InsertCell(worksheetPart1, "H", 11, (years + 1).ToString());
            InsertCell(worksheetPart1, "I", 11, (years + 2).ToString());
            InsertCell(worksheetPart1, "J", 11, (years + 3).ToString());
            InsertCell(worksheetPart1, "K", 11, (years + 4).ToString());
            InsertCell(worksheetPart1, "L", 11, (years + 5).ToString());
            InsertCell(worksheetPart1, "M", 11, (years + 6).ToString());
            InsertCell(worksheetPart1, "N", 11, (years + 7).ToString());
            InsertCell(worksheetPart1, "O", 11, (years + 8).ToString());
            InsertCell(worksheetPart1, "P", 11, (years + 9).ToString());
            InsertCell(worksheetPart1, "Q", 11, (years + 10).ToString());
            ExcelTableRegion betr1 = new ExcelTableRegion();
            AddRow1(betr1, "C7A", 0, -2);
            AddRow1(betr1, "A", 0, -1);
            AddRow1(betr1, "资本支出 - 投资", 0, 0);
            AddRowD(betr1, DocumentManager.CurrentDocument.City.C7A, 0, -3, 14);
            AddRow1(betr1, "C7B", 1, -2);
            AddRow1(betr1, "B", 1, -1);
            AddRow1(betr1, "经常性支出 - 运营/维护", 1, 0);
            AddRowD(betr1, DocumentManager.CurrentDocument.City.C7B, 1, -3, 14);
            AddRow1(betr1, "C7C", 2, -2);
            AddRow1(betr1, "C", 2, -1);
            AddRow1(betr1, "经常性支出 - 还本付息", 2, 0);
            AddRowD(betr1, DocumentManager.CurrentDocument.City.C7C, 2, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr1, 12, 3);
            #endregion

            #region 总投资能力
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.8总投资能力", 16);
            InsertCell(worksheetPart1, "A", 17, "");
            InsertCell(worksheetPart1, "B", 17, "");
            InsertCell(worksheetPart1, "C", 17, "");
            InsertCell(worksheetPart1, "D", 17, (years - 3).ToString());
            InsertCell(worksheetPart1, "E", 17, (years - 2).ToString());
            InsertCell(worksheetPart1, "F", 17, (years - 1).ToString());
            InsertCell(worksheetPart1, "G", 17, (years - 0).ToString());
            InsertCell(worksheetPart1, "H", 17, (years + 1).ToString());
            InsertCell(worksheetPart1, "I", 17, (years + 2).ToString());
            InsertCell(worksheetPart1, "J", 17, (years + 3).ToString());
            InsertCell(worksheetPart1, "K", 17, (years + 4).ToString());
            InsertCell(worksheetPart1, "L", 17, (years + 5).ToString());
            InsertCell(worksheetPart1, "M", 17, (years + 6).ToString());
            InsertCell(worksheetPart1, "N", 17, (years + 7).ToString());
            InsertCell(worksheetPart1, "O", 17, (years + 8).ToString());
            InsertCell(worksheetPart1, "P", 17, (years + 9).ToString());
            InsertCell(worksheetPart1, "Q", 17, (years + 10).ToString());
            ExcelTableRegion betr2 = new ExcelTableRegion();
            AddRow1(betr2, "C8A", 0, -2);
            AddRow1(betr2, "A", 0, -1);
            AddRow1(betr2, "经营性收入（自有+共享）", 0, 0);
            AddRowD(betr2, DocumentManager.CurrentDocument.City.C8A, 0, -3, 14);
            AddRow1(betr2, "C8B", 1, -2);
            AddRow1(betr2, "B", 1, -1);
            AddRow1(betr2, "经营性支出", 1, 0);
            AddRowD(betr2, DocumentManager.CurrentDocument.City.C8B, 1, -3, 14);
            AddRow1(betr2, "C8C", 2, -2);
            AddRow1(betr2, "C", 2, -1);
            AddRow1(betr2, "净运营盈余/亏损", 2, 0);
            AddRowD(betr2, DocumentManager.CurrentDocument.City.C8C, 2, -3, 14);
            AddRow1(betr2, "C8D", 3, -2);
            AddRow1(betr2, "D", 3, -1);
            AddRow1(betr2, "投资预算", 3, 0);
            AddRowD(betr2, DocumentManager.CurrentDocument.City.C8D, 3, -3, 14);
            AddRow1(betr2, "C8E", 4, -2);
            AddRow1(betr2, "E", 4, -1);
            AddRow1(betr2, "Est.debt servicing capacity", 4, 0);
            AddRowD(betr2, DocumentManager.CurrentDocument.City.C8E, 4, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr2, 18, 3);
            #endregion

            #region 宏观经济数据假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.9宏观经济数据假设", 25);
            InsertCell(worksheetPart1, "A", 26, "");
            InsertCell(worksheetPart1, "B", 26, "");
            InsertCell(worksheetPart1, "C", 26, "");
            InsertCell(worksheetPart1, "D", 26, (years - 3).ToString());
            InsertCell(worksheetPart1, "E", 26, (years - 2).ToString());
            InsertCell(worksheetPart1, "F", 26, (years - 1).ToString());
            InsertCell(worksheetPart1, "G", 26, (years - 0).ToString());
            InsertCell(worksheetPart1, "H", 26, (years + 1).ToString());
            InsertCell(worksheetPart1, "I", 26, (years + 2).ToString());
            InsertCell(worksheetPart1, "J", 26, (years + 3).ToString());
            InsertCell(worksheetPart1, "K", 26, (years + 4).ToString());
            InsertCell(worksheetPart1, "L", 26, (years + 5).ToString());
            InsertCell(worksheetPart1, "M", 26, (years + 6).ToString());
            InsertCell(worksheetPart1, "N", 26, (years + 7).ToString());
            InsertCell(worksheetPart1, "O", 26, (years + 8).ToString());
            InsertCell(worksheetPart1, "P", 26, (years + 9).ToString());
            InsertCell(worksheetPart1, "Q", 26, (years + 10).ToString());
            ExcelTableRegion betr3 = new ExcelTableRegion();
            AddRow1(betr3, "C9A", 0, -2);
            AddRow1(betr3, "A", 0, -1);
            AddRow1(betr3, "通货膨胀率", 0, 0);
            AddRow(betr3, DocumentManager.CurrentDocument.City.C9A, 0, -3, 14);
            AddRow1(betr3, "C9B", 1, -2);
            AddRow1(betr3, "B", 1, -1);
            AddRow1(betr3, "国内生产总值增长率", 1, 0);
            AddRow(betr3, DocumentManager.CurrentDocument.City.C9B, 1, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr3, 27, 3);
            #endregion

            #region 商业贷款贷款条件假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.10商业贷款贷款条件假设", 31);
            ExcelTableRegion betr4 = new ExcelTableRegion();
            AddRow1(betr4, "C10A", 0, -2);
            AddRow1(betr4, "A", 0, -1);
            AddRow1(betr4, "利率", 0, 0);
            AddRowString(betr4, DocumentManager.CurrentDocument.City.C10A.ToString(), 0);
            AddRow1(betr4, "C10B", 1, -2);
            AddRow1(betr4, "B", 1, -1);
            AddRow1(betr4, "偿还期（年）", 1, 0);
            AddRowString(betr4, DocumentManager.CurrentDocument.City.C10B.ToString(), 1);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr4, 32, 3);
            #endregion

            #region 优惠贷款贷款条件假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.11优惠贷款贷款条件假设", 36);
            ExcelTableRegion betr5 = new ExcelTableRegion();
            AddRow1(betr5, "C11A", 0, -2);
            AddRow1(betr5, "A", 0, -1);
            AddRow1(betr5, "利率", 0, 0);
            AddRowString(betr5, DocumentManager.CurrentDocument.City.C11A.ToString(), 0);
            AddRow1(betr5, "C11B", 1, -2);
            AddRow1(betr5, "B", 1, -1);
            AddRow1(betr5, "偿还期（年）", 1, 0);
            AddRowString(betr5, DocumentManager.CurrentDocument.City.C11B.ToString(), 1);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr5, 37, 3);
            #endregion

            #region 当地税收征管假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.12当地税收征管假设", 41);
            ExcelTableRegion betr6 = new ExcelTableRegion();
            AddRow1(betr6, "C12A", 0, -2);
            AddRow1(betr6, "A", 0, -1);
            AddRow1(betr6, "当地征税率", 0, 0);
            AddRowString(betr6, DocumentManager.CurrentDocument.City.C12A.ToString(), 0);
            AddRow1(betr6, "C12B", 1, -2);
            AddRow1(betr6, "B", 1, -1);
            AddRow1(betr6, "项目延迟对当地一般收入的影响（年）", 1, 0);
            AddRowString(betr6, DocumentManager.CurrentDocument.City.C12B.ToString(), 1);
            AddRow1(betr6, "C12C", 2, -2);
            AddRow1(betr6, "C", 2, -1);
            AddRow1(betr6, "项目对当地税基的影响（百分比）", 2, 0);
            AddRowString(betr6, DocumentManager.CurrentDocument.City.C12C.ToString(), 2);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr6, 42, 3);
            #endregion

            #region 汇率假设（兑美元）
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.13汇率假设（兑美元）", 47);
            ExcelTableRegion betr7 = new ExcelTableRegion();
            AddRow1(betr7, "C13A", 0, -2);
            AddRow1(betr7, "A", 0, -1);
            AddRow1(betr7, "利率", 0, 0);
            AddRowString(betr7, DocumentManager.CurrentDocument.City.C13A.ToString(), 0);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr7, 48, 3);
            #endregion

            #region 投资预算和债务清偿假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.14投资预算和债务清偿假设", 51);
            ExcelTableRegion betr8 = new ExcelTableRegion();
            AddRow1(betr8, "C14A", 0, -2);
            AddRow1(betr8, "A", 0, -1);
            AddRow1(betr8, "运营盈余用于战略投资项目的比例", 0, 0);
            AddRowString(betr8, DocumentManager.CurrentDocument.City.C14A.ToString(), 0);
            AddRow1(betr8, "C14B", 1, -2);
            AddRow1(betr8, "B", 1, -1);
            AddRow1(betr8, "还本付息额占运运营盈余的比例", 1, 0);
            AddRowString(betr8, DocumentManager.CurrentDocument.City.C14B.ToString(), 1);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr8, 52, 3);
            #endregion

            #region 收入预测假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.15收入预测假设", 56);
            ExcelTableRegion betr9 = new ExcelTableRegion();
            AddRow1(betr9, "C15A_1", 0, -2);
            AddRow1(betr9, "A", 0, -1);
            AddRow1(betr9, "经常性收入：一般收入（通货膨胀率）", 0, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15A_1.ToString(), 0);
            AddRow1(betr9, "C15A_2", 1, -2);
            AddRow1(betr9, "A", 1, -1);
            AddRow1(betr9, "经常性收入：一般收入（GDP）", 1, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15A_2.ToString(), 1);
            AddRow1(betr9, "C15B_1", 2, -2);
            AddRow1(betr9, "B", 2, -1);
            AddRow1(betr9, "用户费用及罚款（通货膨胀率）", 2, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15B_1.ToString(), 2);
            AddRow1(betr9, "C15B_2", 3, -2);
            AddRow1(betr9, "B", 3, -1);
            AddRow1(betr9, "用户费用及罚款（GDP）", 3, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15B_2.ToString(), 3);
            AddRow1(betr9, "C15C_1", 4, -2);
            AddRow1(betr9, "C", 4, -1);
            AddRow1(betr9, "资本收入：共享收入/税收和一般性拨款（通货膨胀率）", 4, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15C_1.ToString(), 4);
            AddRow1(betr9, "C15C_2", 5, -2);
            AddRow1(betr9, "C", 5, -1);
            AddRow1(betr9, "资本收入：共享收入/税收和一般性拨款（GDP）", 5, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15C_2.ToString(), 5);
            AddRow1(betr9, "C15D_1", 6, -2);
            AddRow1(betr9, "D", 6, -1);
            AddRow1(betr9, "资本收入：专项性拨款（通货膨胀率）", 6, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15D_1.ToString(), 6);
            AddRow1(betr9, "C15D_2", 7, -2);
            AddRow1(betr9, "D", 7, -1);
            AddRow1(betr9, "资本收入：专项性拨款（GDP）", 7, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15D_2.ToString(), 7);
            AddRow1(betr9, "C15E_1", 8, -2);
            AddRow1(betr9, "E", 8, -1);
            AddRow1(betr9, "资本收入：市政资产出售所得（通货膨胀率）", 8, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15E_1.ToString(), 8);
            AddRow1(betr9, "C15E_2", 9, -2);
            AddRow1(betr9, "E", 9, -1);
            AddRow1(betr9, "资本收入：市政资产出售所得（GDP）", 9, 0);
            AddRowString(betr9, DocumentManager.CurrentDocument.City.C15E_2.ToString(), 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr9, 57, 3);
            #endregion

            #region 支出预测假设
            TitleStyle(worksheetPart1.Worksheet, worksheetPart1, "1.16支出预测假设", 69);
            ExcelTableRegion betr10 = new ExcelTableRegion();
            AddRow1(betr10, "C16A_1", 0, -2);
            AddRow1(betr10, "A", 0, -1);
            AddRow1(betr10, "资本支出：投资用款（通货膨胀率）", 0, 0);
            AddRowString(betr10, DocumentManager.CurrentDocument.City.C16A_1.ToString(), 0);
            AddRow1(betr10, "C16A_2", 1, -2);
            AddRow1(betr10, "A", 1, -1);
            AddRow1(betr10, "资本支出：投资用款（GDP）", 1, 0);
            AddRowString(betr10, DocumentManager.CurrentDocument.City.C16A_2.ToString(), 1);
            AddRow1(betr10, "C16B_1", 2, -2);
            AddRow1(betr10, "B", 2, -1);
            AddRow1(betr10, "经常性支出：运营/维护（通货膨胀率）", 2, 0);
            AddRowString(betr10, DocumentManager.CurrentDocument.City.C16B_1.ToString(), 2);
            AddRow1(betr10, "C16B_2", 3, -2);
            AddRow1(betr10, "B", 3, -1);
            AddRow1(betr10, "经常性支出：运营/维护（GDP）", 3, 0);
            AddRowString(betr10, DocumentManager.CurrentDocument.City.C16B_2.ToString(), 3);
            AddRow1(betr10, "C16C_1", 4, -2);
            AddRow1(betr10, "C", 4, -1);
            AddRow1(betr10, "经常性支出：年度还本付息额（通货膨胀率）", 4, 0);
            AddRowString(betr10, DocumentManager.CurrentDocument.City.C16C_1.ToString(), 4);
            AddRow1(betr10, "C16C_2", 5, -2);
            AddRow1(betr10, "C", 5, -1);
            AddRow1(betr10, "经常性支出：年度还本付息额（GDP）", 5, 0);
            AddRowString(betr10, DocumentManager.CurrentDocument.City.C16C_2.ToString(), 5);
            ExcelTableHelper.InsertTableRegion(worksheetPart1, betr10, 70, 3);
            #endregion

            for (int i = 0; i < DocumentManager.CurrentDocument.Projects.Count; i++)
            {
                WorksheetPart worksheetPart0 = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart0.Worksheet = new Worksheet(new SheetData());
                Sheet sheet = new Sheet() { Id = document.WorkbookPart.GetIdOfPart(worksheetPart0), SheetId = (uint)i + 4, Name = DocumentManager.CurrentDocument.Projects[i].P1A };
                sheets.Append(sheet);

                ProjSheet(worksheetPart0, sheet, worksheetPart0.Worksheet, DocumentManager.CurrentDocument.Projects[i]);
            }

            WorkbookStylesPart workbookStylesPart1 = workbookpart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart1.Stylesheet = GenerateStyleSheet();

            workbookpart.Workbook.Save();
            document.Close();
        }

        private static void AddRowString(ExcelTableRegion table, string st, int row)
        {
            Cell cell = new Cell();
            cell.StyleIndex = 7;
            cell.DataType = CellValues.InlineString;
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = st;
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);
            table.Cells.Add(cell, new CellPosition(row, 1));
        }

        private static void AddRowStringD(ExcelTableRegion table, string st, int row)
        {
            Cell cell = new Cell();
            cell.StyleIndex = 4;
            cell.DataType = CellValues.InlineString;
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = st;
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);
            table.Cells.Add(cell, new CellPosition(row, 1));
        }

        private static void AddRow1(ExcelTableRegion table, string st, int row, int col)
        {
            Cell cell = new Cell();
            cell.StyleIndex = 3;
            cell.DataType = CellValues.InlineString;
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = st;
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);
            table.Cells.Add(cell, new CellPosition(row, col));
        }

        private static void AddRowS(ExcelTableRegion table, string st, int row, int col)
        {
            Cell cell = new Cell();
            cell.StyleIndex = 3;

            cell.DataType = CellValues.InlineString;
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = st;
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);
            table.Cells.Add(cell, new CellPosition(row, col));
        }

        private static void AddRow(ExcelTableRegion table, YearwiseExpression rowData, int row, int range1, int range2)
        {
            int col = 1;
            Enumerable.Range(range1, range2).Select(x => rowData.YearR(x)).ToList().ForEach(x =>
            {
                Cell cell = new Cell();
                cell.StyleIndex = 7;
                cell.DataType = CellValues.InlineString;
                InlineString inlineString = new InlineString();
                Text t = new Text();
                t.Text = x.ToString("F2");
                inlineString.AppendChild(t);
                cell.AppendChild(inlineString);
                table.Cells.Add(cell, new CellPosition(row, col));
                col++;
            });
        }

        private static void AddRowD2(ExcelTableRegion table, YearwiseExpression rowData, int row, int range1, int range2)
        {
            int col = 6;
            Enumerable.Range(range1, range2).Select(x => rowData.YearR(x)).ToList().ForEach(x =>
            {
                Cell cell = new Cell();
                cell.StyleIndex = 4;
                cell.DataType = CellValues.InlineString;
                InlineString inlineString = new InlineString();
                Text t = new Text();
                t.Text = x.ToString("F2");
                inlineString.AppendChild(t);
                cell.AppendChild(inlineString);
                table.Cells.Add(cell, new CellPosition(row, col));
                col++;
            });
        }

        private static void AddRowD(ExcelTableRegion table, YearwiseExpression rowData, int row, int range1, int range2)
        {
            int col = 1;
            Enumerable.Range(range1, range2).Select(x => rowData.YearR(x)).ToList().ForEach(x =>
            {
                Cell cell = new Cell();
                cell.StyleIndex = 4;
                cell.DataType = CellValues.InlineString;
                InlineString inlineString = new InlineString();
                Text t = new Text();
                t.Text = x.ToString("F2");
                inlineString.AppendChild(t);
                cell.AppendChild(inlineString);
                table.Cells.Add(cell, new CellPosition(row, col));
                col++;
            });
        }

        private static void AddRowList(ExcelTableRegion table, List<int> list, int row)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Cell cell = new Cell();
                cell.StyleIndex = 7;
                cell.DataType = CellValues.InlineString;
                InlineString inlineString = new InlineString();
                Text t = new Text();
                t.Text = list[i].ToString();
                inlineString.AppendChild(t);
                cell.AppendChild(inlineString);
                table.Cells.Add(cell, new CellPosition(row, i + 1));
            }
        }

        private static Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "FFFF0000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 2 - The Italic font.
                        new Italic(),
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Calibri" }),
                    new Font(                                                               // Index 3 - The Times Roman font. with 16 size
                        new FontSize() { Val = 16 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "Times New Roman" }),
                    new Font(                                                               // Index 4 - The bold font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "64FFFFFF" } },
                        new FontName() { Val = "Calibri" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(                                                           // Index 1 - The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "78016454" } }
                        ) { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 2 - The yellow fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "23656457" } }
                        ) { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 3 
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFFFFF00" } }
                        ) { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 4 
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FF99CC00" } }
                        ) { PatternType = PatternValues.Solid }),
                    new Fill(                                                           // Index 5 
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FFA3A1A1" } }
                        ) { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Auto = true }
                        ) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true },                          // Index 0 - 
                    new CellFormat() { FontId = 1, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 1 - Bold 
                    new CellFormat() { FontId = 2, FillId = 0, BorderId = 0, ApplyFont = true },       // Index 2 - Italic
                    new CellFormat() { FontId = 4, FillId = 5, BorderId = 1, ApplyFill = true },       // Index 3 - Times Roman
                    new CellFormat() { FontId = 4, FillId = 2, BorderId = 1, ApplyFill = true },       // Index 4 - 
                    new CellFormat(                                                                   // Index 5 - Alignment
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                    ) { FontId = 1, FillId = 0, BorderId = 0, ApplyAlignment = true },
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true },      // Index 6 - Border
                    new CellFormat() { FontId = 0, FillId = 4, BorderId = 1, ApplyFill = true }
                )
            );
        }

        private static void TitleStyle(Worksheet worksheet1, WorksheetPart worksheetPart, string headname, uint rowIndex)
        {
            SheetData data = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            Row row = new Row() { RowIndex = rowIndex };
            data.Append(row);
            Cell newCell = new Cell() { StyleIndex = 5 };
            newCell.DataType = new EnumValue<CellValues>(CellValues.String);
            row.InsertAt<Cell>(newCell, 0);
            newCell.CellValue = new CellValue(headname);
            newCell.CellValue.Text = headname;

            MergeCells mergeCells;
            if (worksheet1.Elements<MergeCells>().Count() > 0)
            {
                mergeCells = worksheet1.Elements<MergeCells>().First();
            }
            else
            {
                mergeCells = new MergeCells();
                // Insert a MergeCells object into the specified position.
                if (worksheet1.Elements<CustomSheetView>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<CustomSheetView>().First());
                }
                else if (worksheet1.Elements<DataConsolidate>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<DataConsolidate>().First());
                }
                else if (worksheet1.Elements<SortState>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<SortState>().First());
                }
                else if (worksheet1.Elements<AutoFilter>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<AutoFilter>().First());
                }
                else if (worksheet1.Elements<Scenarios>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<Scenarios>().First());
                }
                else if (worksheet1.Elements<ProtectedRanges>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<ProtectedRanges>().First());
                }
                else if (worksheet1.Elements<SheetProtection>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<SheetProtection>().First());
                }
                else if (worksheet1.Elements<SheetCalculationProperties>().Count() > 0)
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<SheetCalculationProperties>().First());
                }
                else
                {
                    worksheet1.InsertAfter(mergeCells, worksheet1.Elements<SheetData>().First());
                }
            }
            MergeCell mergeCell = new MergeCell() { Reference = new StringValue("A" + rowIndex + ":" + "F" + rowIndex) };
            mergeCells.Append(mergeCell);
            worksheetPart.Worksheet.Save();
        }

        private static void InsertCell(WorksheetPart worksheetPart, string columnName, uint rowIndex, string name)
        {
            SheetData data = worksheetPart.Worksheet.GetFirstChild<SheetData>();
            //Row row = new Row() { RowIndex = rowIndex };
            Row row;
            if (data.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = data.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                data.AppendChild(row);
            }
            Cell c = new Cell();
            c.StyleIndex = 3;
            c.CellReference = columnName + rowIndex;
            c.DataType = CellValues.InlineString;
            InlineString inlineString = new InlineString();
            Text t = new Text();
            t.Text = name;
            inlineString.AppendChild(t);
            c.AppendChild(inlineString);
            row.AppendChild(c);
        }

        public static void ProjSheet(WorksheetPart worksheetPart, Sheet sheet, Worksheet worksheet, ProjectStatistics proj)
        {
            int year = proj.P1F_1;

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.1项目描述", 1);
            ExcelTableRegion etr = new ExcelTableRegion();
            AddRow1(etr, "P1A", 0, -2);
            AddRow1(etr, "A", 0, -1);
            AddRow1(etr, "项目名称", 0, 0);
            AddRowString(etr, proj.P1A, 0);
            AddRow1(etr, "P1B", 1, -2);
            AddRow1(etr, "B", 1, -1);
            AddRow1(etr, "项目位置", 1, 0);
            AddRowString(etr, proj.P1B, 1);
            AddRow1(etr, "P1C", 2, -2);
            AddRow1(etr, "C", 2, -1);
            AddRow1(etr, "项目隶属部门", 2, 0);
            AddRowList(etr, proj.P1C, 2);
            AddRow1(etr, "P1D", 3, -2);
            AddRow1(etr, "D", 3, -1);
            AddRow1(etr, "项目主要方面", 3, 0);
            AddRowString(etr, proj.P1D.ToString(), 3);
            AddRow1(etr, "P1E", 4, -2);
            AddRow1(etr, "E", 4, -1);
            AddRow1(etr, "项目现状", 4, 0);
            AddRowString(etr, proj.P1E.ToString(), 4);
            AddRow1(etr, "P1F_1", 5, -2);
            AddRow1(etr, "F", 5, -1);
            AddRow1(etr, "预计开始年份", 5, 0);
            AddRowString(etr, proj.P1F_1.ToString(), 5);
            AddRow1(etr, "P1F_2", 6, -2);
            AddRow1(etr, "G", 6, -1);
            AddRow1(etr, "预计完成年份", 6, 0);
            AddRowString(etr, proj.P1F_2.ToString(), 6);
            AddRow1(etr, "PIP", 7, -2);
            AddRow1(etr, "H", 7, -1);
            AddRow1(etr, "包括在PIP中", 7, 0);
            AddRowString(etr, proj.PIP.ToString(), 7);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr, 2, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.2资本成本", 10);
            InsertCell(worksheetPart, "A", 11, "");
            InsertCell(worksheetPart, "B", 11, "");
            InsertCell(worksheetPart, "C", 11, "");
            InsertCell(worksheetPart, "D", 11, year.ToString());
            InsertCell(worksheetPart, "E", 11, (year + 1).ToString());
            InsertCell(worksheetPart, "F", 11, (year + 2).ToString());
            InsertCell(worksheetPart, "G", 11, (year + 3).ToString());
            InsertCell(worksheetPart, "H", 11, (year + 4).ToString());
            ExcelTableRegion etr1 = new ExcelTableRegion();
            AddRow1(etr1, "P2A", 0, -2);
            AddRow1(etr1, "A", 0, -1);
            AddRow1(etr1, "规划、筹备和采购", 0, 0);
            AddRow(etr1, proj.P2A, 0, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr1, "P2B", 1, -2);
            AddRow1(etr1, "B", 1, -1);
            AddRow1(etr1, "土地收购", 1, 0);
            AddRow(etr1, proj.P2B, 1, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr1, "P2C", 2, -2);
            AddRow1(etr1, "C", 2, -1);
            AddRow1(etr1, "施工建设", 2, 0);
            AddRow(etr1, proj.P2C, 2, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr1, "P2D", 3, -2);
            AddRow1(etr1, "D", 3, -1);
            AddRow1(etr1, "设备和设施", 3, 0);
            AddRow(etr1, proj.P2D, 3, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr1, "P2E", 4, -2);
            AddRow1(etr1, "E", 4, -1);
            AddRow1(etr1, "其他成本", 4, 0);
            AddRow(etr1, proj.P2E, 4, proj.P1F_1 - proj.P02, 5);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr1, 12, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.3资本投资的预期资金来源", 19);
            InsertCell(worksheetPart, "A", 20, "");
            InsertCell(worksheetPart, "B", 20, "");
            InsertCell(worksheetPart, "C", 20, "");
            InsertCell(worksheetPart, "D", 20, year.ToString());
            InsertCell(worksheetPart, "E", 20, (year + 1).ToString());
            InsertCell(worksheetPart, "F", 20, (year + 2).ToString());
            InsertCell(worksheetPart, "G", 20, (year + 3).ToString());
            InsertCell(worksheetPart, "H", 20, (year + 4).ToString());
            ExcelTableRegion etr2 = new ExcelTableRegion();
            AddRow1(etr2, "P3A", 0, -2);
            AddRow1(etr2, "A", 0, -1);
            AddRow1(etr2, "自有资金（当地政府预算）", 0, 0);
            AddRow(etr2, proj.P3A, 0, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr2, "P3B", 1, -2);
            AddRow1(etr2, "B", 1, -1);
            AddRow1(etr2, "国家或地区基金/拨款/转移", 1, 0);
            AddRow(etr2, proj.P3B, 1, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr2, "P3C", 2, -2);
            AddRow1(etr2, "C", 2, -1);
            AddRow1(etr2, "私营部门 /公众参与", 2, 0);
            AddRow(etr2, proj.P3C, 2, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr2, "P3D", 3, -2);
            AddRow1(etr2, "D", 3, -1);
            AddRow1(etr2, "商业贷款", 3, 0);
            AddRow(etr2, proj.P3D, 3, proj.P1F_1 - proj.P02, 5);
            AddRow1(etr2, "P3E", 4, -2);
            AddRow1(etr2, "E", 4, -1);
            AddRow1(etr2, "优惠贷款（捐助机构）", 4, 0);
            AddRow(etr2, proj.P3E, 4, proj.P1F_1 - proj.P02, 5);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr2, 21, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.4运营与维护成本", 28);
            ExcelTableRegion etr3 = new ExcelTableRegion();
            AddRow1(etr3, "P4A", 0, -2);
            AddRow1(etr3, "A", 0, -1);
            AddRow1(etr3, "年均预估额", 0, 0);
            AddRowString(etr3, proj.P4A.ToString(), 0);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr3, 29, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.13 Additional Project Data", 32);
            InsertCell(worksheetPart, "A", 33, "A");
            InsertCell(worksheetPart, "B", 33, "目的");
            InsertCell(worksheetPart, "C", 33, "");
            ExcelTableRegion etr4 = new ExcelTableRegion();
            AddRow1(etr4, "P13A1", 0, -1);
            AddRow1(etr4, "简短项目说明", 0, 0);
            AddRowString(etr4, proj.P13A1, 0);
            AddRow1(etr4, "P13A2", 1, -1);
            AddRow1(etr4, "直接受益者", 1, 0);
            AddRowString(etr4, proj.P13A2, 1);
            AddRow1(etr4, "P13A3", 2, -1);
            AddRow1(etr4, "间接受益者", 2, 0);
            AddRowString(etr4, proj.P13A3, 2);
            AddRow1(etr4, "B", 3, -1);
            AddRow1(etr4, "投资理由", 3, 0);
            AddRow1(etr4, "", 3, 1);
            AddRow1(etr4, "P13B", 4, -1);
            AddRow1(etr4, "为什么说该项目最有效利用了纳税人的钱", 4, 0);
            AddRowString(etr4, proj.P13B, 4);
            AddRow1(etr4, "C", 5, -1);
            AddRow1(etr4, "计划提出", 5, 0);
            AddRow1(etr4, "", 5, 1);
            AddRow1(etr4, "P13C", 6, -1);
            AddRow1(etr4, "谁参与了本项目的启动进程", 6, 0);
            AddRowString(etr4, proj.P13C, 6);
            AddRow1(etr4, "D", 7, -1);
            AddRow1(etr4, "项目实施者", 7, 0);
            AddRow1(etr4, "", 7, 1);
            AddRow1(etr4, "P13D1", 8, -1);
            AddRow1(etr4, "项目设计", 8, 0);
            AddRowString(etr4, proj.P13D1, 8);
            AddRow1(etr4, "P13D2", 9, -1);
            AddRow1(etr4, "项目实施", 9, 0);
            AddRowString(etr4, proj.P13D2, 9);
            AddRow1(etr4, "P13D3", 10, -1);
            AddRow1(etr4, "项目运营", 10, 0);
            AddRowString(etr4, proj.P13D3, 10);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr4, 34, 2);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.14新增商业贷款预测", 46);
            InsertCell(worksheetPart, "A", 47, "");
            InsertCell(worksheetPart, "B", 47, "");
            InsertCell(worksheetPart, "C", 47, "");
            InsertCell(worksheetPart, "D", 47, (year - 4).ToString());
            InsertCell(worksheetPart, "E", 47, (year - 3).ToString());
            InsertCell(worksheetPart, "F", 47, (year - 2).ToString());
            InsertCell(worksheetPart, "G", 47, (year - 1).ToString());
            InsertCell(worksheetPart, "H", 47, year.ToString());
            InsertCell(worksheetPart, "I", 47, (year + 1).ToString());
            InsertCell(worksheetPart, "J", 47, (year + 2).ToString());
            InsertCell(worksheetPart, "K", 47, (year + 3).ToString());
            InsertCell(worksheetPart, "L", 47, (year + 4).ToString());
            InsertCell(worksheetPart, "M", 47, (year + 5).ToString());
            InsertCell(worksheetPart, "N", 47, (year + 6).ToString());
            InsertCell(worksheetPart, "O", 47, (year + 7).ToString());
            InsertCell(worksheetPart, "P", 47, (year + 8).ToString());
            InsertCell(worksheetPart, "Q", 47, (year + 9).ToString());
            ExcelTableRegion etr5 = new ExcelTableRegion();
            AddRow1(etr5, "P14A", 0, -2);
            AddRow1(etr5, "A", 0, -1);
            AddRow1(etr5, "初始贷款额", 0, 0);
            AddRow(etr5, proj.P14A, 0, -3, 5);
            AddRowD2(etr5, proj.P14A, 0, 2, 9);
            AddRow1(etr5, "P14B", 1, -2);
            AddRow1(etr5, "B", 1, -1);
            AddRow1(etr5, "贷款净额", 1, 0);
            AddRow(etr5, proj.P14B, 1, -3, 5);
            AddRowD2(etr5, proj.P14B, 1, 2, 9);
            AddRow1(etr5, "P14C", 2, -2);
            AddRow1(etr5, "C", 2, -1);
            AddRow1(etr5, "利息付款", 2, 0);
            AddRow(etr5, proj.P14C, 2, -3, 5);
            AddRowD2(etr5, proj.P14C, 2, 2, 9);
            AddRow1(etr5, "P14D", 3, -2);
            AddRow1(etr5, "D", 3, -1);
            AddRow1(etr5, "本金付款", 3, 0);
            AddRow(etr5, proj.P14D, 3, -3, 5);
            AddRowD2(etr5, proj.P14D, 3, 2, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr5, 48, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.15新增优惠贷款预测", 54);
            InsertCell(worksheetPart, "A", 55, "");
            InsertCell(worksheetPart, "B", 55, "");
            InsertCell(worksheetPart, "C", 55, "");
            InsertCell(worksheetPart, "D", 55, (year - 4).ToString());
            InsertCell(worksheetPart, "E", 55, (year - 3).ToString());
            InsertCell(worksheetPart, "F", 55, (year - 2).ToString());
            InsertCell(worksheetPart, "G", 55, (year - 1).ToString());
            InsertCell(worksheetPart, "H", 55, year.ToString());
            InsertCell(worksheetPart, "I", 55, (year + 1).ToString());
            InsertCell(worksheetPart, "J", 55, (year + 2).ToString());
            InsertCell(worksheetPart, "K", 55, (year + 3).ToString());
            InsertCell(worksheetPart, "L", 55, (year + 4).ToString());
            InsertCell(worksheetPart, "M", 55, (year + 5).ToString());
            InsertCell(worksheetPart, "N", 55, (year + 6).ToString());
            InsertCell(worksheetPart, "O", 55, (year + 7).ToString());
            InsertCell(worksheetPart, "P", 55, (year + 8).ToString());
            InsertCell(worksheetPart, "Q", 55, (year + 9).ToString());
            ExcelTableRegion etr6 = new ExcelTableRegion();
            AddRow1(etr6, "P15A", 0, -2);
            AddRow1(etr6, "A", 0, -1);
            AddRow1(etr6, "初始贷款额", 0, 0);
            AddRow(etr6, proj.P15A, 0, -3, 5);
            AddRowD2(etr6, proj.P15A, 0, 2, 9);
            AddRow1(etr6, "P15B", 1, -2);
            AddRow1(etr6, "B", 1, -1);
            AddRow1(etr6, "贷款净额", 1, 0);
            AddRow(etr6, proj.P15B, 1, -3, 5);
            AddRowD2(etr6, proj.P15B, 1, 2, 9);
            AddRow1(etr6, "P15C", 2, -2);
            AddRow1(etr6, "C", 2, -1);
            AddRow1(etr6, "利息付款", 2, 0);
            AddRow(etr6, proj.P15C, 2, -3, 5);
            AddRowD2(etr6, proj.P15C, 2, 2, 9);
            AddRow1(etr6, "P15D", 3, -2);
            AddRow1(etr6, "D", 3, -1);
            AddRow1(etr6, "本金付款", 3, 0);
            AddRow(etr6, proj.P15D, 3, -3, 5);
            AddRowD2(etr6, proj.P15D, 3, 2, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr6, 56, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.16额外收入预估值", 62);
            InsertCell(worksheetPart, "A", 63, "");
            InsertCell(worksheetPart, "B", 63, "");
            InsertCell(worksheetPart, "C", 63, "");
            InsertCell(worksheetPart, "D", 63, (year - 4).ToString());
            InsertCell(worksheetPart, "E", 63, (year - 3).ToString());
            InsertCell(worksheetPart, "F", 63, (year - 2).ToString());
            InsertCell(worksheetPart, "G", 63, (year - 1).ToString());
            InsertCell(worksheetPart, "H", 63, year.ToString());
            InsertCell(worksheetPart, "I", 63, (year + 1).ToString());
            InsertCell(worksheetPart, "J", 63, (year + 2).ToString());
            InsertCell(worksheetPart, "K", 63, (year + 3).ToString());
            InsertCell(worksheetPart, "L", 63, (year + 4).ToString());
            InsertCell(worksheetPart, "M", 63, (year + 5).ToString());
            InsertCell(worksheetPart, "N", 63, (year + 6).ToString());
            InsertCell(worksheetPart, "O", 63, (year + 7).ToString());
            InsertCell(worksheetPart, "P", 63, (year + 8).ToString());
            InsertCell(worksheetPart, "Q", 63, (year + 9).ToString());
            ExcelTableRegion etr7 = new ExcelTableRegion();
            AddRow1(etr7, "P16A", 0, -2);
            AddRow1(etr7, "A", 0, -1);
            AddRow1(etr7, "一般收入", 0, 0);
            AddRow(etr7, proj.P16A, 0, -3, 5);
            AddRowD2(etr7, proj.P16A, 0, 2, 9);
            AddRow1(etr7, "P16B", 1, -2);
            AddRow1(etr7, "B", 1, -1);
            AddRow1(etr7, "用户费用", 1, 0);
            AddRow(etr7, proj.P16B, 1, -3, 5);
            AddRowD2(etr7, proj.P16B, 1, 2, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr7, 64, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.17额外支出预估值", 68);
            InsertCell(worksheetPart, "A", 69, "");
            InsertCell(worksheetPart, "B", 69, "");
            InsertCell(worksheetPart, "C", 69, "");
            InsertCell(worksheetPart, "D", 69, (year - 4).ToString());
            InsertCell(worksheetPart, "E", 69, (year - 3).ToString());
            InsertCell(worksheetPart, "F", 69, (year - 2).ToString());
            InsertCell(worksheetPart, "G", 69, (year - 1).ToString());
            InsertCell(worksheetPart, "H", 69, year.ToString());
            InsertCell(worksheetPart, "I", 69, (year + 1).ToString());
            InsertCell(worksheetPart, "J", 69, (year + 2).ToString());
            InsertCell(worksheetPart, "K", 69, (year + 3).ToString());
            InsertCell(worksheetPart, "L", 69, (year + 4).ToString());
            InsertCell(worksheetPart, "M", 69, (year + 5).ToString());
            InsertCell(worksheetPart, "N", 69, (year + 6).ToString());
            InsertCell(worksheetPart, "O", 69, (year + 7).ToString());
            InsertCell(worksheetPart, "P", 69, (year + 8).ToString());
            InsertCell(worksheetPart, "Q", 69, (year + 9).ToString());
            ExcelTableRegion etr8 = new ExcelTableRegion();
            AddRow1(etr8, "P17A", 0, -2);
            AddRow1(etr8, "A", 0, -1);
            AddRow1(etr8, "运营和维护", 0, 0);
            AddRow(etr8, proj.P17A, 0, -3, 5);
            AddRowD2(etr8, proj.P17A, 0, 2, 9);
            AddRow1(etr8, "P17B", 1, -2);
            AddRow1(etr8, "B", 1, -1);
            AddRow1(etr8, "还本付息额", 1, 0);
            AddRow(etr8, proj.P17B, 1, -3, 5);
            AddRowD2(etr8, proj.P17B, 1, 2, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr8, 70, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.18 ORIGINAL BUDGET FORECAST (FROM FORECAST SHEET)", 74);
            InsertCell(worksheetPart, "A", 75, "");
            InsertCell(worksheetPart, "B", 75, "");
            InsertCell(worksheetPart, "C", 75, "");
            InsertCell(worksheetPart, "D", 75, (year - 4).ToString());
            InsertCell(worksheetPart, "E", 75, (year - 3).ToString());
            InsertCell(worksheetPart, "F", 75, (year - 2).ToString());
            InsertCell(worksheetPart, "G", 75, (year - 1).ToString());
            InsertCell(worksheetPart, "H", 75, year.ToString());
            InsertCell(worksheetPart, "I", 75, (year + 1).ToString());
            InsertCell(worksheetPart, "J", 75, (year + 2).ToString());
            InsertCell(worksheetPart, "K", 75, (year + 3).ToString());
            InsertCell(worksheetPart, "L", 75, (year + 4).ToString());
            InsertCell(worksheetPart, "M", 75, (year + 5).ToString());
            InsertCell(worksheetPart, "N", 75, (year + 6).ToString());
            InsertCell(worksheetPart, "O", 75, (year + 7).ToString());
            InsertCell(worksheetPart, "P", 75, (year + 8).ToString());
            InsertCell(worksheetPart, "Q", 75, (year + 9).ToString());
            ExcelTableRegion etr9 = new ExcelTableRegion();
            AddRow1(etr9, "P18A", 0, -2);
            AddRow1(etr9, "A", 0, -1);
            AddRow1(etr9, "预计收入", 0, 0);
            AddRowD(etr9, proj.P18A, 0, -3, 14);
            AddRow1(etr9, "P18B", 1, -2);
            AddRow1(etr9, "B", 1, -1);
            AddRow1(etr9, "预计运营支出", 1, 0);
            AddRowD(etr9, proj.P18B, 1, -3, 14);
            AddRow1(etr9, "P18C", 2, -2);
            AddRow1(etr9, "C", 2, -1);
            AddRow1(etr9, "预计运营盈余/亏损", 2, 0);
            AddRowD(etr9, proj.P18C, 2, -3, 14);
            AddRow1(etr9, "P18D", 3, -2);
            AddRow1(etr9, "D", 3, -1);
            AddRow1(etr9, "预计投资运算", 3, 0);
            AddRowD(etr9, proj.P18D, 3, -3, 14);
            AddRow1(etr9, "P18E", 4, -2);
            AddRow1(etr9, "E", 4, -1);
            AddRow1(etr9, "预计偿债能力", 4, 0);
            AddRowD(etr9, proj.P18E, 4, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr9, 76, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.19 对预算预测的影响", 83);
            InsertCell(worksheetPart, "A", 84, "");
            InsertCell(worksheetPart, "B", 84, "");
            InsertCell(worksheetPart, "C", 84, "");
            InsertCell(worksheetPart, "D", 84, (year - 4).ToString());
            InsertCell(worksheetPart, "E", 84, (year - 3).ToString());
            InsertCell(worksheetPart, "F", 84, (year - 2).ToString());
            InsertCell(worksheetPart, "G", 84, (year - 1).ToString());
            InsertCell(worksheetPart, "H", 84, year.ToString());
            InsertCell(worksheetPart, "I", 84, (year + 1).ToString());
            InsertCell(worksheetPart, "J", 84, (year + 2).ToString());
            InsertCell(worksheetPart, "K", 84, (year + 3).ToString());
            InsertCell(worksheetPart, "L", 84, (year + 4).ToString());
            InsertCell(worksheetPart, "M", 84, (year + 5).ToString());
            InsertCell(worksheetPart, "N", 84, (year + 6).ToString());
            InsertCell(worksheetPart, "O", 84, (year + 7).ToString());
            InsertCell(worksheetPart, "P", 84, (year + 8).ToString());
            InsertCell(worksheetPart, "Q", 84, (year + 9).ToString());
            ExcelTableRegion etr10 = new ExcelTableRegion();
            AddRow1(etr10, "P19A", 0, -2);
            AddRow1(etr10, "A", 0, -1);
            AddRow1(etr10, "预计收入", 0, 0);
            AddRowD(etr10, proj.P19A, 0, -3, 14);
            AddRow1(etr10, "P19B", 1, -2);
            AddRow1(etr10, "B", 1, -1);
            AddRow1(etr10, "预计运营支出", 1, 0);
            AddRowD(etr10, proj.P19B, 1, -3, 14);
            AddRow1(etr10, "P19C", 2, -2);
            AddRow1(etr10, "C", 2, -1);
            AddRow1(etr10, "预计运营盈余/亏损", 2, 0);
            AddRowD(etr10, proj.P19C, 2, -3, 14);
            AddRow1(etr10, "P19D", 3, -2);
            AddRow1(etr10, "D", 3, -1);
            AddRow1(etr10, "预计投资运算", 3, 0);
            AddRowD(etr10, proj.P19D, 3, -3, 14);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr10, 85, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.20资本投资资金来源", 90);
            InsertCell(worksheetPart, "A", 91, "");
            InsertCell(worksheetPart, "B", 91, "");
            InsertCell(worksheetPart, "C", 91, "");
            InsertCell(worksheetPart, "D", 91, year.ToString());
            InsertCell(worksheetPart, "E", 91, (year + 1).ToString());
            InsertCell(worksheetPart, "F", 91, (year + 2).ToString());
            InsertCell(worksheetPart, "G", 91, (year + 3).ToString());
            InsertCell(worksheetPart, "H", 91, (year + 4).ToString());
            InsertCell(worksheetPart, "I", 91, (year + 5).ToString());
            InsertCell(worksheetPart, "J", 91, (year + 6).ToString());
            InsertCell(worksheetPart, "K", 91, (year + 7).ToString());
            InsertCell(worksheetPart, "L", 91, (year + 8).ToString());
            ExcelTableRegion etr11 = new ExcelTableRegion();
            AddRow1(etr11, "P20A", 0, -2);
            AddRow1(etr11, "A", 0, -1);
            AddRow1(etr11, "自有资金（当地政府预算）", 0, 0);
            AddRowD(etr11, proj.P20A, 0, 1, 9);
            AddRow1(etr11, "P20B", 1, -2);
            AddRow1(etr11, "B", 1, -1);
            AddRow1(etr11, "国家或地区 基金/拨款/转移", 1, 0);
            AddRowD(etr11, proj.P20B, 1, 1, 9);
            AddRow1(etr11, "P20C", 2, -2);
            AddRow1(etr11, "C", 2, -1);
            AddRow1(etr11, "私营部门 /公众参与", 2, 0);
            AddRowD(etr11, proj.P20C, 2, 1, 9);
            AddRow1(etr11, "P20D", 3, -2);
            AddRow1(etr11, "D", 3, -1);
            AddRow1(etr11, "商业贷款", 3, 0);
            AddRowD(etr11, proj.P20D, 3, 1, 9);
            AddRow1(etr11, "P20E", 4, -2);
            AddRow1(etr11, "E", 4, -1);
            AddRow1(etr11, "优惠贷款（捐助机构）", 4, 0);
            AddRowD(etr11, proj.P20E, 4, 1, 9);
            AddRow1(etr11, "P20F", 5, -2);
            AddRow1(etr11, "F", 5, -1);
            AddRow1(etr11, "融资缺口", 5, 0);
            AddRowD(etr11, proj.P20F, 5, 1, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr11, 92, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.21资本支出", 101);
            InsertCell(worksheetPart, "A", 102, "");
            InsertCell(worksheetPart, "B", 102, "");
            InsertCell(worksheetPart, "C", 102, "");
            InsertCell(worksheetPart, "D", 102, year.ToString());
            InsertCell(worksheetPart, "E", 102, (year + 1).ToString());
            InsertCell(worksheetPart, "F", 102, (year + 2).ToString());
            InsertCell(worksheetPart, "G", 102, (year + 3).ToString());
            InsertCell(worksheetPart, "H", 102, (year + 4).ToString());
            InsertCell(worksheetPart, "I", 102, (year + 5).ToString());
            InsertCell(worksheetPart, "J", 102, (year + 6).ToString());
            InsertCell(worksheetPart, "K", 102, (year + 7).ToString());
            InsertCell(worksheetPart, "L", 102, (year + 8).ToString());
            ExcelTableRegion etr12 = new ExcelTableRegion();
            AddRow1(etr12, "P21A", 0, -2);
            AddRow1(etr12, "A", 0, -1);
            AddRow1(etr12, "资本投资", 0, 0);
            AddRowD(etr12, proj.P21A, 0, 1, 9);
            AddRow1(etr12, "P21B", 1, -2);
            AddRow1(etr12, "B", 1, -1);
            AddRow1(etr12, "现有支出", 1, 0);
            AddRowD(etr12, proj.P21B, 1, 1, 9);
            AddRow1(etr12, "P21C", 2, -2);
            AddRow1(etr12, "C", 2, -1);
            AddRow1(etr12, "支出限额", 2, 0);
            AddRowD(etr12, proj.P21C, 2, 1, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr12, 103, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.22资本支出", 108);
            InsertCell(worksheetPart, "A", 109, "");
            InsertCell(worksheetPart, "B", 109, "");
            InsertCell(worksheetPart, "C", 109, "");
            InsertCell(worksheetPart, "D", 109, year.ToString());
            InsertCell(worksheetPart, "E", 109, (year + 1).ToString());
            InsertCell(worksheetPart, "F", 109, (year + 2).ToString());
            InsertCell(worksheetPart, "G", 109, (year + 3).ToString());
            InsertCell(worksheetPart, "H", 109, (year + 4).ToString());
            InsertCell(worksheetPart, "I", 109, (year + 5).ToString());
            InsertCell(worksheetPart, "J", 109, (year + 6).ToString());
            InsertCell(worksheetPart, "K", 109, (year + 7).ToString());
            InsertCell(worksheetPart, "L", 109, (year + 8).ToString());
            ExcelTableRegion etr13 = new ExcelTableRegion();
            AddRow1(etr13, "P22A", 0, -2);
            AddRow1(etr13, "A", 0, -1);
            AddRow1(etr13, "新增贷款", 0, 0);
            AddRowD(etr13, proj.P22A, 0, 1, 9);
            AddRow1(etr13, "P22B", 1, -2);
            AddRow1(etr13, "B", 1, -1);
            AddRow1(etr13, "当前还本付息额", 1, 0);
            AddRowD(etr13, proj.P22B, 1, 1, 9);
            AddRow1(etr13, "P22C", 2, -2);
            AddRow1(etr13, "C", 2, -1);
            AddRow1(etr13, "预计最大偿债能力", 2, 0);
            AddRowD(etr13, proj.P22C, 2, 1, 9);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr13, 110, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.5新增商业贷款", 115);
            ExcelTableRegion etr14 = new ExcelTableRegion();
            AddRow1(etr14, "P5A", 0, -2);
            AddRow1(etr14, "A", 0, -1);
            AddRow1(etr14, "贷款额", 0, 0);
            AddRowStringD(etr14, proj.P5A.ToString(), 0);
            AddRow1(etr14, "P5B", 1, -2);
            AddRow1(etr14, "B", 1, -1);
            AddRow1(etr14, "发放年份", 1, 0);
            AddRowString(etr14, proj.P5B.ToString(), 1);
            AddRow1(etr14, "P5C", 2, -2);
            AddRow1(etr14, "C", 2, -1);
            AddRow1(etr14, "偿还期(年）", 2, 0);
            AddRowStringD(etr14, proj.P5C.ToString(), 2);
            AddRow1(etr14, "P5D", 3, -2);
            AddRow1(etr14, "D", 3, -1);
            AddRow1(etr14, "利率", 3, 0);
            AddRowStringD(etr14, proj.P5D.ToString(), 3);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr14, 116, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.6新增优惠贷款", 122);
            ExcelTableRegion etr15 = new ExcelTableRegion();
            AddRow1(etr15, "P6A", 0, -2);
            AddRow1(etr15, "A", 0, -1);
            AddRow1(etr15, "贷款额", 0, 0);
            AddRowStringD(etr15, proj.P6A.ToString(), 0);
            AddRow1(etr15, "P6B", 1, -2);
            AddRow1(etr15, "B", 1, -1);
            AddRow1(etr15, "发放年份", 1, 0);
            AddRowString(etr15, proj.P6B.ToString(), 1);
            AddRow1(etr15, "P6C", 2, -2);
            AddRow1(etr15, "C", 2, -1);
            AddRow1(etr15, "偿还期(年）", 2, 0);
            AddRowStringD(etr15, proj.P6C.ToString(), 2);
            AddRow1(etr15, "P6D", 3, -2);
            AddRow1(etr15, "D", 3, -1);
            AddRow1(etr15, "利率", 3, 0);
            AddRowStringD(etr15, proj.P6D.ToString(), 3);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr15, 123, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "2.7运营与维护成本的资金来源", 129);
            ExcelTableRegion etr16 = new ExcelTableRegion();
            AddRow1(etr16, "P7A", 0, -2);
            AddRow1(etr16, "A", 0, -1);
            AddRow1(etr16, "自有资金", 0, 0);
            AddRowString(etr16, proj.P7A.ToString(), 0);
            AddRow1(etr16, "P7B", 1, -2);
            AddRow1(etr16, "B", 1, -1);
            AddRow1(etr16, "国家/地区基金/拨款/支付", 1, 0);
            AddRowString(etr16, proj.P7B.ToString(), 1);
            AddRow1(etr16, "P7C", 2, -2);
            AddRow1(etr16, "C", 2, -1);
            AddRow1(etr16, "费用收入", 2, 0);
            AddRowString(etr16, proj.P7C.ToString(), 2);
            AddRow1(etr16, "P7D", 3, -2);
            AddRow1(etr16, "D", 3, -1);
            AddRow1(etr16, "其他", 3, 0);
            AddRowString(etr16, proj.P7D.ToString(), 3);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etr16, 130, 3);

            TitleStyle(worksheetPart.Worksheet, worksheetPart, "项目问卷", 136);
            ExcelTableRegion etrq = new ExcelTableRegion();
            AddRow1(etrq, "", 0, -2);
            AddRow1(etrq, "项目目的", 0, -1);
            AddRow1(etrq, "", 0, 0);
            AddRow1(etrq, "PQ01", 1, -2);
            AddRow1(etrq, "A", 1, -1);
            AddRow1(etrq, "处理这个问题的现有服务处于什么状况", 1, 0);
            AddRowString(etrq, proj.Questionnaire["PQ01"].ToString(), 1);
            AddRow1(etrq, "PQ02", 2, -2);
            AddRow1(etrq, "B", 2, -1);
            AddRow1(etrq, "新设施服务人数占所在地区人口总数的百分比约为多少", 2, 0);
            AddRowString(etrq, proj.Questionnaire["PQ02"].ToString(), 2);
            AddRow1(etrq, "PQ03", 3, -2);
            AddRow1(etrq, "C", 3, -1);
            AddRow1(etrq, "与城市发展计划下提议的其他项目相比，该项目处于什么优先级别", 3, 0);
            AddRowString(etrq, proj.Questionnaire["PQ03"].ToString(), 3);
            AddRow1(etrq, "PQ04", 4, -2);
            AddRow1(etrq, "D", 4, -1);
            AddRow1(etrq, "项目对实现地区发展目标有什么贡献", 4, 0);
            AddRowString(etrq, proj.Questionnaire["PQ04"].ToString(), 4);
            AddRow1(etrq, "PQ05", 5, -2);
            AddRow1(etrq, "E", 5, -1);
            AddRow1(etrq, "推迟项目将在市民健康、财产、安全、繁荣等方面产生什么影响", 5, 0);
            AddRowString(etrq, proj.Questionnaire["PQ05"].ToString(), 5);
            AddRow1(etrq, "PQ06", 6, -2);
            AddRow1(etrq, "F", 6, -1);
            AddRow1(etrq, "项目在更广泛的服务提供系统中是否填补空白", 6, 0);
            AddRowString(etrq, proj.Questionnaire["PQ06"].ToString(), 6);
            AddRow1(etrq, "", 7, -2);
            AddRow1(etrq, "公众反应", 7, -1);
            AddRow1(etrq, "", 7, 0);
            AddRow1(etrq, "PQ07", 8, -2);
            AddRow1(etrq, "A", 8, -1);
            AddRow1(etrq, "项目在当地是否有“拥护者”（拥护该项目的人）", 8, 0);
            AddRowString(etrq, proj.Questionnaire["PQ07"].ToString(), 8);
            AddRow1(etrq, "PQ08", 9, -2);
            AddRow1(etrq, "B", 9, -1);
            AddRow1(etrq, "项目是否获得当地或更广泛的政治支持", 9, 0);
            AddRowString(etrq, proj.Questionnaire["PQ08"].ToString(), 9);
            AddRow1(etrq, "PQ09", 10, -2);
            AddRow1(etrq, "C", 10, -1);
            AddRow1(etrq, "项目是否获得城市管理部门的支持", 10, 0);
            AddRowString(etrq, proj.Questionnaire["PQ09"].ToString(), 10);
            AddRow1(etrq, "PQ10", 11, -2);
            AddRow1(etrq, "D", 11, -1);
            AddRow1(etrq, "是否有非政府组织、社会团体、网络组织、媒体或商业机构支持或反对该项目", 11, 0);
            AddRowString(etrq, proj.Questionnaire["PQ10"].ToString(), 11);
            AddRow1(etrq, "PQ11", 12, -2);
            AddRow1(etrq, "E", 12, -1);
            AddRow1(etrq, "是否有来自新设施周边地区居民的支持或反对意见", 12, 0);
            AddRowString(etrq, proj.Questionnaire["PQ11"].ToString(), 12);
            AddRow1(etrq, "PQ12", 13, -2);
            AddRow1(etrq, "F", 13, -1);
            AddRow1(etrq, "是否就该项目与市民代表进行过任何形式的公众 /社区咨询", 13, 0);
            AddRowString(etrq, proj.Questionnaire["PQ12"].ToString(), 13);
            AddRow1(etrq, "PQ13", 14, -2);
            AddRow1(etrq, "G", 14, -1);
            AddRow1(etrq, "项目是否涉及重新安置社区住户和/或企业", 14, 0);
            AddRowString(etrq, proj.Questionnaire["PQ13"].ToString(), 14);
            AddRow1(etrq, "PQ14", 15, -2);
            AddRow1(etrq, "H", 15, -1);
            AddRow1(etrq, "公众或私营企业是否愿意贡献自身资源（资金或劳动力）", 15, 0);
            AddRowString(etrq, proj.Questionnaire["PQ14"].ToString(), 15);
            AddRow1(etrq, "", 16, -2);
            AddRow1(etrq, "环境影响", 16, -1);
            AddRow1(etrq, "", 16, 0);
            AddRow1(etrq, "PQ15", 17, -2);
            AddRow1(etrq, "A", 17, -1);
            AddRow1(etrq, "是否会直接影响当地环境质量，例如空气或水污染物、废弃物等", 17, 0);
            AddRowString(etrq, proj.Questionnaire["PQ15"].ToString(), 17);
            AddRow1(etrq, "PQ16", 18, -2);
            AddRow1(etrq, "B", 18, -1);
            AddRow1(etrq, "该项目是否有利于长期可持续发展，例如可再生能源、洁净水供应、回收利用等", 18, 0);
            AddRowString(etrq, proj.Questionnaire["PQ16"].ToString(), 18);
            AddRow1(etrq, "PQ17", 19, -2);
            AddRow1(etrq, "C", 19, -1);
            AddRow1(etrq, "是否有具体的健康效益（尤其对当地而言）", 19, 0);
            AddRowString(etrq, proj.Questionnaire["PQ17"].ToString(), 19);
            AddRow1(etrq, "PQ18", 20, -2);
            AddRow1(etrq, "D", 20, -1);
            AddRow1(etrq, "是否能够减轻环境危害或减少人类脆弱性", 20, 0);
            AddRowString(etrq, proj.Questionnaire["PQ18"].ToString(), 20);
            AddRow1(etrq, "PQ19", 21, -2);
            AddRow1(etrq, "E", 21, -1);
            AddRow1(etrq, "项目是否有助于减轻气候变化（减少环境有害物质的产生或者减少温室气体的排放）", 21, 0);
            AddRowString(etrq, proj.Questionnaire["PQ19"].ToString(), 21);
            AddRow1(etrq, "PQ20", 22, -2);
            AddRow1(etrq, "F", 22, -1);
            AddRow1(etrq, "是否有益于城市自然空间质量，例如公园、绿色植物、露天场所等", 22, 0);
            AddRowString(etrq, proj.Questionnaire["PQ20"].ToString(), 22);
            AddRow1(etrq, "", 23, -2);
            AddRow1(etrq, "社会经济影响", 23, -1);
            AddRow1(etrq, "", 23, 0);
            AddRow1(etrq, "PQ21", 24, -2);
            AddRow1(etrq, "A", 24, -1);
            AddRow1(etrq, "预计项目对当地经济发展产生什么直接影响", 24, 0);
            AddRowString(etrq, proj.Questionnaire["PQ21"].ToString(), 24);
            AddRow1(etrq, "PQ22", 25, -2);
            AddRow1(etrq, "B", 25, -1);
            AddRow1(etrq, "是否有间接的长期经济效益，例如提升土地/财产价格、减少市民支出等", 25, 0);
            AddRowString(etrq, proj.Questionnaire["PQ22"].ToString(), 25);
            AddRow1(etrq, "PQ23", 26, -2);
            AddRow1(etrq, "C", 26, -1);
            AddRow1(etrq, "项目是否利用当地可用资源", 26, 0);
            AddRowString(etrq, proj.Questionnaire["PQ23"].ToString(), 26);
            AddRow1(etrq, "PQ24", 27, -2);
            AddRow1(etrq, "D", 27, -1);
            AddRow1(etrq, "项目是否能为经济贫困地区带来改善", 27, 0);
            AddRowString(etrq, proj.Questionnaire["PQ24"].ToString(), 27);
            AddRow1(etrq, "PQ25", 28, -2);
            AddRow1(etrq, "E", 28, -1);
            AddRow1(etrq, "项目是否以低收入群体为对象", 28, 0);
            AddRowString(etrq, proj.Questionnaire["PQ25"].ToString(), 28);
            AddRow1(etrq, "PQ26", 29, -2);
            AddRow1(etrq, "F", 29, -1);
            AddRow1(etrq, "需要缴费的人员是否能够负担提议的费用", 29, 0);
            AddRowString(etrq, proj.Questionnaire["PQ26"].ToString(), 29);
            AddRow1(etrq, "PQ27", 30, -2);
            AddRow1(etrq, "G", 30, -1);
            AddRow1(etrq, "项目是否有助于构建和谐社会", 30, 0);
            AddRowString(etrq, proj.Questionnaire["PQ27"].ToString(), 30);
            AddRow1(etrq, "PQ28", 31, -2);
            AddRow1(etrq, "H", 31, -1);
            AddRow1(etrq, "项目是否有助于建设更具活力的城市中心", 31, 0);
            AddRowString(etrq, proj.Questionnaire["PQ28"].ToString(), 31);
            AddRow1(etrq, "PQ29", 32, -2);
            AddRow1(etrq, "I", 32, -1);
            AddRow1(etrq, "项目是否有助于提升市民的城市自豪感", 32, 0);
            AddRowString(etrq, proj.Questionnaire["PQ29"].ToString(), 32);
            AddRow1(etrq, "", 33, -2);
            AddRow1(etrq, "项目实施的可行性", 33, -1);
            AddRow1(etrq, "", 33, 0);
            AddRow1(etrq, "PQ30", 34, -2);
            AddRow1(etrq, "A", 34, -1);
            AddRow1(etrq, "当地政府的项目预算是否已落实资金保障", 34, 0);
            AddRowString(etrq, proj.Questionnaire["PQ30"].ToString(), 34);
            AddRow1(etrq, "PQ31", 35, -2);
            AddRow1(etrq, "B", 35, -1);
            AddRow1(etrq, "资金是否通过外部融资渠道得到保障，以及/或者是否有外部融资潜力", 35, 0);
            AddRowString(etrq, proj.Questionnaire["PQ31"].ToString(), 35);
            AddRow1(etrq, "PQ32", 36, -2);
            AddRow1(etrq, "C", 36, -1);
            AddRow1(etrq, "项目能否带来直接收入", 36, 0);
            AddRowString(etrq, proj.Questionnaire["PQ32"].ToString(), 36);
            AddRow1(etrq, "PQ33", 37, -2);
            AddRow1(etrq, "D", 37, -1);
            AddRow1(etrq, "项目能否带来间接收入", 37, 0);
            AddRowString(etrq, proj.Questionnaire["PQ33"].ToString(), 37);
            AddRow1(etrq, "PQ34", 38, -2);
            AddRow1(etrq, "E", 38, -1);
            AddRow1(etrq, "项目是否降低目前的预算成本", 38, 0);
            AddRowString(etrq, proj.Questionnaire["PQ34"].ToString(), 38);
            AddRow1(etrq, "PQ35", 39, -2);
            AddRow1(etrq, "F", 39, -1);
            AddRow1(etrq, "提议费用征收系统的合理程度", 39, 0);
            AddRowString(etrq, proj.Questionnaire["PQ35"].ToString(), 39);
            AddRow1(etrq, "PQ36", 40, -2);
            AddRow1(etrq, "G", 40, -1);
            AddRow1(etrq, "确保项目实施和运营的有力制度是否到位，或者是否需要外部支持", 40, 0);
            AddRowString(etrq, proj.Questionnaire["PQ36"].ToString(), 40);
            AddRow1(etrq, "PQ37", 41, -2);
            AddRow1(etrq, "H", 41, -1);
            AddRow1(etrq, "金融/经济因素是否会对项目的完成/可持续性构成风险", 41, 0);
            AddRowString(etrq, proj.Questionnaire["PQ37"].ToString(), 41);
            AddRow1(etrq, "PQ38", 42, -2);
            AddRow1(etrq, "I", 42, -1);
            AddRow1(etrq, "政治因素是否会对项目的完成构成风险", 42, 0);
            AddRowString(etrq, proj.Questionnaire["PQ38"].ToString(), 42);
            AddRow1(etrq, "PQ39", 43, -2);
            AddRow1(etrq, "J", 43, -1);
            AddRow1(etrq, "如果存在风险，项目设计是否包括风险缓解策略", 43, 0);
            AddRowString(etrq, proj.Questionnaire["PQ39"].ToString(), 43);
            ExcelTableHelper.InsertTableRegion(worksheetPart, etrq, 137, 3);
        }
    }
}
