using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CIIPP
{
    public static class LoadExcelOpenXml
    {
        public static void LoadExcelTable(string docName)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, false))
            {
                DocumentManager.CurrentDocument.City.C01 = ReadExcel(document.WorkbookPart, 2, "D", 2);
                DocumentManager.CurrentDocument.City.C02 = Convert.ToInt32(ReadExcel(document.WorkbookPart, 3, "D", 2));
                DocumentManager.CurrentDocument.City.C03 = Convert.ToInt32(ReadExcel(document.WorkbookPart, 4, "D", 2));
                DocumentManager.CurrentDocument.City.C04 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 5, "D", 2));
                DocumentManager.CurrentDocument.City.C05 = Convert.ToDateTime(ReadExcel(document.WorkbookPart, 6, "D", 2));
                DocumentManager.CurrentDocument.City.Country = ReadExcel(document.WorkbookPart, 2, "D", 1);
                DocumentManager.CurrentDocument.City.Intro = ReadExcel(document.WorkbookPart, 3, "D", 1);
                DocumentManager.CurrentDocument.City.Currency = CityStatistics.CurrencyEnum.ToList().IndexOf(ReadExcel(document.WorkbookPart, 4, "D", 1));
                DocumentManager.CurrentDocument.City.Multiple = CityStatistics.MultipleEnum.ToList().IndexOf(ReadExcel(document.WorkbookPart, 5, "D", 1));

                int year = DocumentManager.CurrentDocument.City.C02;
                DocumentManager.CurrentDocument.City.C1A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 10, "D", 2));
                DocumentManager.CurrentDocument.City.C1A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 10, "E", 2));
                DocumentManager.CurrentDocument.City.C1A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 10, "F", 2));
                DocumentManager.CurrentDocument.City.C1A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 10, "G", 2));
                DocumentManager.CurrentDocument.City.C1B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 11, "D", 2));
                DocumentManager.CurrentDocument.City.C1B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 11, "E", 2));
                DocumentManager.CurrentDocument.City.C1B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 11, "F", 2));
                DocumentManager.CurrentDocument.City.C1B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 11, "G", 2));
                DocumentManager.CurrentDocument.City.C1C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "D", 2));
                DocumentManager.CurrentDocument.City.C1C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "E", 2));
                DocumentManager.CurrentDocument.City.C1C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "F", 2));
                DocumentManager.CurrentDocument.City.C1C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "G", 2));
                DocumentManager.CurrentDocument.City.C1D[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "D", 2));
                DocumentManager.CurrentDocument.City.C1D[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "E", 2));
                DocumentManager.CurrentDocument.City.C1D[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "F", 2));
                DocumentManager.CurrentDocument.City.C1D[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "G", 2));
                DocumentManager.CurrentDocument.City.C1E[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "D", 2));
                DocumentManager.CurrentDocument.City.C1E[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "E", 2));
                DocumentManager.CurrentDocument.City.C1E[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "F", 2));
                DocumentManager.CurrentDocument.City.C1E[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "G", 2));

                DocumentManager.CurrentDocument.City.C2A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 18, "D", 2));
                DocumentManager.CurrentDocument.City.C2A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 18, "E", 2));
                DocumentManager.CurrentDocument.City.C2A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 18, "F", 2));
                DocumentManager.CurrentDocument.City.C2A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 18, "G", 2));
                DocumentManager.CurrentDocument.City.C2B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 19, "D", 2));
                DocumentManager.CurrentDocument.City.C2B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 19, "E", 2));
                DocumentManager.CurrentDocument.City.C2B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 19, "F", 2));
                DocumentManager.CurrentDocument.City.C2B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 19, "G", 2));
                DocumentManager.CurrentDocument.City.C2C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 20, "D", 2));
                DocumentManager.CurrentDocument.City.C2C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 20, "E", 2));
                DocumentManager.CurrentDocument.City.C2C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 20, "F", 2));
                DocumentManager.CurrentDocument.City.C2C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 20, "G", 2));

                DocumentManager.CurrentDocument.City.C3A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "D", 2));
                DocumentManager.CurrentDocument.City.C3A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "E", 2));
                DocumentManager.CurrentDocument.City.C3A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "F", 2));
                DocumentManager.CurrentDocument.City.C3A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "G", 2));
                DocumentManager.CurrentDocument.City.C3B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 26, "D", 2));
                DocumentManager.CurrentDocument.City.C3B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 26, "E", 2));
                DocumentManager.CurrentDocument.City.C3B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 26, "F", 2));
                DocumentManager.CurrentDocument.City.C3B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 26, "G", 2));
                DocumentManager.CurrentDocument.City.C3C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "D", 2));
                DocumentManager.CurrentDocument.City.C3C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "E", 2));
                DocumentManager.CurrentDocument.City.C3C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "F", 2));
                DocumentManager.CurrentDocument.City.C3C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "G", 2));
                DocumentManager.CurrentDocument.City.C3D[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "D", 2));
                DocumentManager.CurrentDocument.City.C3D[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "E", 2));
                DocumentManager.CurrentDocument.City.C3D[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "F", 2));
                DocumentManager.CurrentDocument.City.C3D[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "G", 2));

                DocumentManager.CurrentDocument.City.C4A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "D", 2));
                DocumentManager.CurrentDocument.City.C4A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "E", 2));
                DocumentManager.CurrentDocument.City.C4A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "F", 2));
                DocumentManager.CurrentDocument.City.C4A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "G", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "H", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "I", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "J", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "K", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "L", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "M", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "N", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "O", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "P", 2));
                DocumentManager.CurrentDocument.City.C4A[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "Q", 2));
                DocumentManager.CurrentDocument.City.C4B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "D", 2));
                DocumentManager.CurrentDocument.City.C4B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "E", 2));
                DocumentManager.CurrentDocument.City.C4B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "F", 2));
                DocumentManager.CurrentDocument.City.C4B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "G", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "H", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "I", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "J", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "K", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "L", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "M", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "N", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "O", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "P", 2));
                DocumentManager.CurrentDocument.City.C4B[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "Q", 2));
                DocumentManager.CurrentDocument.City.C4C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "D", 2));
                DocumentManager.CurrentDocument.City.C4C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "E", 2));
                DocumentManager.CurrentDocument.City.C4C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "F", 2));
                DocumentManager.CurrentDocument.City.C4C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "G", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "H", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "I", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "J", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "K", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "L", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "M", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "N", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "O", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "P", 2));
                DocumentManager.CurrentDocument.City.C4C[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 34, "Q", 2));

                DocumentManager.CurrentDocument.City.Questionnaire["CQ01"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 38, "D", 2));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ02"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 39, "D", 2));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ03"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 40, "D", 2));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ04"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 41, "D", 2));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ05"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 42, "D", 2));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ06"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 43, "D", 2));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ07"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 44, "D", 2));

                DocumentManager.CurrentDocument.City.C9A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "D", 3));
                DocumentManager.CurrentDocument.City.C9A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "E", 3));
                DocumentManager.CurrentDocument.City.C9A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "F", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "G", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "H", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "I", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "J", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "K", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "L", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "M", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "N", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "O", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "P", 3));
                DocumentManager.CurrentDocument.City.C9A[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 27, "Q", 3));
                DocumentManager.CurrentDocument.City.C9B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "D", 3));
                DocumentManager.CurrentDocument.City.C9B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "E", 3));
                DocumentManager.CurrentDocument.City.C9B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "F", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "G", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "H", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "I", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "J", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "K", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "L", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "M", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "N", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "O", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "P", 3));
                DocumentManager.CurrentDocument.City.C9B[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 28, "Q", 3));

                DocumentManager.CurrentDocument.City.C10A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 32, "D", 3));
                DocumentManager.CurrentDocument.City.C10B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 33, "D", 3));

                DocumentManager.CurrentDocument.City.C11A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 37, "D", 3));
                DocumentManager.CurrentDocument.City.C11B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 38, "D", 3));

                DocumentManager.CurrentDocument.City.C12A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 42, "D", 3));
                DocumentManager.CurrentDocument.City.C12B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 43, "D", 3));
                DocumentManager.CurrentDocument.City.C12C = Convert.ToDouble(ReadExcel(document.WorkbookPart, 44, "D", 3));

                DocumentManager.CurrentDocument.City.C13A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 48, "D", 3));

                DocumentManager.CurrentDocument.City.C14A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 52, "D", 3));
                DocumentManager.CurrentDocument.City.C14B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 53, "D", 3));

                DocumentManager.CurrentDocument.City.C15A_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 57, "D", 3));
                DocumentManager.CurrentDocument.City.C15A_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 58, "D", 3));
                DocumentManager.CurrentDocument.City.C15B_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 59, "D", 3));
                DocumentManager.CurrentDocument.City.C15B_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 60, "D", 3));
                DocumentManager.CurrentDocument.City.C15C_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 61, "D", 3));
                DocumentManager.CurrentDocument.City.C15C_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 62, "D", 3));
                DocumentManager.CurrentDocument.City.C15D_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 63, "D", 3));
                DocumentManager.CurrentDocument.City.C15D_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 64, "D", 3));
                DocumentManager.CurrentDocument.City.C15E_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 65, "D", 3));
                DocumentManager.CurrentDocument.City.C15E_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 66, "D", 3));

                DocumentManager.CurrentDocument.City.C16A_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 70, "D", 3));
                DocumentManager.CurrentDocument.City.C16A_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 71, "D", 3));
                DocumentManager.CurrentDocument.City.C16B_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 72, "D", 3));
                DocumentManager.CurrentDocument.City.C16B_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 73, "D", 3));
                DocumentManager.CurrentDocument.City.C16C_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 74, "D", 3));
                DocumentManager.CurrentDocument.City.C16C_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 75, "D", 3));

                for (int i = 4; i <= GetExcel.GetSheetsNum(docName); i++)
                {
                    ProjectStatistics projStcs = new ProjectStatistics(ReadExcel(document.WorkbookPart, 2, "D", (uint)i), ReadExcel(document.WorkbookPart, 3, "D", (uint)i));
                    projStcs.P1C[0] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 4, "D", (uint)i));
                    projStcs.P1D = Convert.ToInt32(ReadExcel(document.WorkbookPart, 5, "D", (uint)i));
                    projStcs.P1E = Convert.ToInt32(ReadExcel(document.WorkbookPart, 6, "D", (uint)i));
                    projStcs.P1F_1 = Convert.ToInt32(ReadExcel(document.WorkbookPart, 7, "D", (uint)i));
                    projStcs.P1F_2 = Convert.ToInt32(ReadExcel(document.WorkbookPart, 8, "D", (uint)i));
                    projStcs.PIP = Convert.ToInt32(ReadExcel(document.WorkbookPart, 9, "D", (uint)i));
                    int year1 = projStcs.P1F_1;
                    projStcs.P2A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "D", (uint)i));
                    projStcs.P2A[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "E", (uint)i));
                    projStcs.P2A[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "F", (uint)i));
                    projStcs.P2A[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "G", (uint)i));
                    projStcs.P2A[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 12, "H", (uint)i));
                    projStcs.P2B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "D", (uint)i));
                    projStcs.P2B[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "E", (uint)i));
                    projStcs.P2B[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "F", (uint)i));
                    projStcs.P2B[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "G", (uint)i));
                    projStcs.P2B[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 13, "H", (uint)i));
                    projStcs.P2C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "D", (uint)i));
                    projStcs.P2C[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "E", (uint)i));
                    projStcs.P2C[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "F", (uint)i));
                    projStcs.P2C[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "G", (uint)i));
                    projStcs.P2C[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 14, "H", (uint)i));
                    projStcs.P2D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 15, "D", (uint)i));
                    projStcs.P2D[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 15, "E", (uint)i));
                    projStcs.P2D[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 15, "F", (uint)i));
                    projStcs.P2D[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 15, "G", (uint)i));
                    projStcs.P2D[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 15, "H", (uint)i));
                    projStcs.P2E[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 16, "D", (uint)i));
                    projStcs.P2E[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 16, "E", (uint)i));
                    projStcs.P2E[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 16, "F", (uint)i));
                    projStcs.P2E[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 16, "G", (uint)i));
                    projStcs.P2E[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 16, "H", (uint)i));

                    projStcs.P3A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 21, "D", (uint)i));
                    projStcs.P3A[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 21, "E", (uint)i));
                    projStcs.P3A[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 21, "F", (uint)i));
                    projStcs.P3A[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 21, "G", (uint)i));
                    projStcs.P3A[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 21, "H", (uint)i));
                    projStcs.P3B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 22, "D", (uint)i));
                    projStcs.P3B[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 22, "E", (uint)i));
                    projStcs.P3B[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 22, "F", (uint)i));
                    projStcs.P3B[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 22, "G", (uint)i));
                    projStcs.P3B[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 22, "H", (uint)i));
                    projStcs.P3C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 23, "D", (uint)i));
                    projStcs.P3C[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 23, "E", (uint)i));
                    projStcs.P3C[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 23, "F", (uint)i));
                    projStcs.P3C[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 23, "G", (uint)i));
                    projStcs.P3C[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 23, "H", (uint)i));
                    projStcs.P3D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 24, "D", (uint)i));
                    projStcs.P3D[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 24, "E", (uint)i));
                    projStcs.P3D[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 24, "F", (uint)i));
                    projStcs.P3D[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 24, "G", (uint)i));
                    projStcs.P3D[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 24, "H", (uint)i));
                    projStcs.P3E[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "D", (uint)i));
                    projStcs.P3E[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "E", (uint)i));
                    projStcs.P3E[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "F", (uint)i));
                    projStcs.P3E[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "G", (uint)i));
                    projStcs.P3E[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 25, "H", (uint)i));

                    projStcs.P4A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 29, "D", (uint)i));

                    projStcs.P5B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 117, "D", (uint)i));
                    projStcs.P6B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 124, "D", (uint)i));

                    projStcs.P7A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 130, "D", (uint)i));
                    projStcs.P7B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 131, "D", (uint)i));
                    projStcs.P7C = Convert.ToDouble(ReadExcel(document.WorkbookPart, 132, "D", (uint)i));
                    projStcs.P7D = Convert.ToDouble(ReadExcel(document.WorkbookPart, 133, "D", (uint)i));

                    projStcs.P13A1 = ReadExcel(document.WorkbookPart, 34, "C", (uint)i);
                    projStcs.P13A2 = ReadExcel(document.WorkbookPart, 35, "C", (uint)i);
                    projStcs.P13A3 = ReadExcel(document.WorkbookPart, 36, "C", (uint)i);
                    projStcs.P13B = ReadExcel(document.WorkbookPart, 38, "C", (uint)i);
                    projStcs.P13C = ReadExcel(document.WorkbookPart, 40, "C", (uint)i);
                    projStcs.P13D1 = ReadExcel(document.WorkbookPart, 42, "C", (uint)i);
                    projStcs.P13D2 = ReadExcel(document.WorkbookPart, 43, "C", (uint)i);
                    projStcs.P13D3 = ReadExcel(document.WorkbookPart, 44, "C", (uint)i);

                    projStcs.P14A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 48, "D", (uint)i));
                    projStcs.P14A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 48, "E", (uint)i));
                    projStcs.P14A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 48, "F", (uint)i));
                    projStcs.P14A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 48, "G", (uint)i));
                    projStcs.P14A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 48, "H", (uint)i));
                    projStcs.P14B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 49, "D", (uint)i));
                    projStcs.P14B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 49, "E", (uint)i));
                    projStcs.P14B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 49, "F", (uint)i));
                    projStcs.P14B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 49, "G", (uint)i));
                    projStcs.P14B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 49, "H", (uint)i));
                    projStcs.P14C[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 50, "D", (uint)i));
                    projStcs.P14C[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 50, "E", (uint)i));
                    projStcs.P14C[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 50, "F", (uint)i));
                    projStcs.P14C[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 50, "G", (uint)i));
                    projStcs.P14C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 50, "H", (uint)i));
                    projStcs.P14D[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 51, "D", (uint)i));
                    projStcs.P14D[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 51, "E", (uint)i));
                    projStcs.P14D[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 51, "F", (uint)i));
                    projStcs.P14D[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 51, "G", (uint)i));
                    projStcs.P14D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 51, "H", (uint)i));

                    projStcs.P15A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 56, "D", (uint)i));
                    projStcs.P15A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 56, "E", (uint)i));
                    projStcs.P15A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 56, "F", (uint)i));
                    projStcs.P15A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 56, "G", (uint)i));
                    projStcs.P15A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 56, "H", (uint)i));
                    projStcs.P15B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 57, "D", (uint)i));
                    projStcs.P15B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 57, "E", (uint)i));
                    projStcs.P15B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 57, "F", (uint)i));
                    projStcs.P15B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 57, "G", (uint)i));
                    projStcs.P15B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 57, "H", (uint)i));
                    projStcs.P15C[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 58, "D", (uint)i));
                    projStcs.P15C[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 58, "E", (uint)i));
                    projStcs.P15C[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 58, "F", (uint)i));
                    projStcs.P15C[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 58, "G", (uint)i));
                    projStcs.P15C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 58, "H", (uint)i));
                    projStcs.P15D[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 59, "D", (uint)i));
                    projStcs.P15D[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 59, "E", (uint)i));
                    projStcs.P15D[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 59, "F", (uint)i));
                    projStcs.P15D[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 59, "G", (uint)i));
                    projStcs.P15D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 59, "H", (uint)i));

                    projStcs.P16A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 64, "D", (uint)i));
                    projStcs.P16A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 64, "E", (uint)i));
                    projStcs.P16A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 64, "F", (uint)i));
                    projStcs.P16A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 64, "G", (uint)i));
                    projStcs.P16A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 64, "H", (uint)i));
                    projStcs.P16B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 65, "D", (uint)i));
                    projStcs.P16B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 65, "E", (uint)i));
                    projStcs.P16B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 65, "F", (uint)i));
                    projStcs.P16B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 65, "G", (uint)i));
                    projStcs.P16B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 65, "H", (uint)i));

                    projStcs.P17A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 70, "D", (uint)i));
                    projStcs.P17A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 70, "E", (uint)i));
                    projStcs.P17A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 70, "F", (uint)i));
                    projStcs.P17A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 70, "G", (uint)i));
                    projStcs.P17A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 70, "H", (uint)i));
                    projStcs.P17B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 71, "D", (uint)i));
                    projStcs.P17B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 71, "E", (uint)i));
                    projStcs.P17B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 71, "F", (uint)i));
                    projStcs.P17B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 71, "G", (uint)i));
                    projStcs.P17B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 71, "H", (uint)i));

                    projStcs.Questionnaire["PQ01"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 138, "D", (uint)i));
                    projStcs.Questionnaire["PQ02"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 139, "D", (uint)i));
                    projStcs.Questionnaire["PQ03"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 140, "D", (uint)i));
                    projStcs.Questionnaire["PQ04"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 141, "D", (uint)i));
                    projStcs.Questionnaire["PQ05"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 142, "D", (uint)i));
                    projStcs.Questionnaire["PQ06"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 143, "D", (uint)i));
                    projStcs.Questionnaire["PQ07"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 145, "D", (uint)i));
                    projStcs.Questionnaire["PQ08"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 146, "D", (uint)i));
                    projStcs.Questionnaire["PQ09"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 147, "D", (uint)i));
                    projStcs.Questionnaire["PQ10"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 148, "D", (uint)i));
                    projStcs.Questionnaire["PQ11"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 149, "D", (uint)i));
                    projStcs.Questionnaire["PQ12"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 150, "D", (uint)i));
                    projStcs.Questionnaire["PQ13"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 151, "D", (uint)i));
                    projStcs.Questionnaire["PQ14"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 152, "D", (uint)i));
                    projStcs.Questionnaire["PQ15"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 154, "D", (uint)i));
                    projStcs.Questionnaire["PQ16"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 155, "D", (uint)i));
                    projStcs.Questionnaire["PQ17"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 156, "D", (uint)i));
                    projStcs.Questionnaire["PQ18"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 157, "D", (uint)i));
                    projStcs.Questionnaire["PQ19"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 158, "D", (uint)i));
                    projStcs.Questionnaire["PQ20"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 159, "D", (uint)i));
                    projStcs.Questionnaire["PQ21"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 161, "D", (uint)i));
                    projStcs.Questionnaire["PQ22"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 162, "D", (uint)i));
                    projStcs.Questionnaire["PQ23"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 163, "D", (uint)i));
                    projStcs.Questionnaire["PQ24"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 164, "D", (uint)i));
                    projStcs.Questionnaire["PQ25"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 165, "D", (uint)i));
                    projStcs.Questionnaire["PQ26"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 166, "D", (uint)i));
                    projStcs.Questionnaire["PQ27"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 167, "D", (uint)i));
                    projStcs.Questionnaire["PQ28"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 168, "D", (uint)i));
                    projStcs.Questionnaire["PQ29"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 169, "D", (uint)i));
                    projStcs.Questionnaire["PQ30"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 171, "D", (uint)i));
                    projStcs.Questionnaire["PQ31"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 172, "D", (uint)i));
                    projStcs.Questionnaire["PQ32"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 173, "D", (uint)i));
                    projStcs.Questionnaire["PQ33"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 174, "D", (uint)i));
                    projStcs.Questionnaire["PQ34"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 175, "D", (uint)i));
                    projStcs.Questionnaire["PQ35"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 176, "D", (uint)i));
                    projStcs.Questionnaire["PQ36"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 177, "D", (uint)i));
                    projStcs.Questionnaire["PQ37"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 178, "D", (uint)i));
                    projStcs.Questionnaire["PQ38"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 179, "D", (uint)i));
                    projStcs.Questionnaire["PQ39"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 180, "D", (uint)i));
                    DocumentManager.CurrentDocument.Projects.Add(projStcs);
                }
            }
        }

        public static string ReadExcel(WorkbookPart workbookPart, uint rowIndex, string columnName, uint sheetId)
        {
            IEnumerable<Sheet> sheets = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.SheetId == sheetId);
            WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheets.First().Id);
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
    }
}
