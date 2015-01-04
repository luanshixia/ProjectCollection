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
    public static class LoadExcel
    {
        public static void LoadExcelTable(string docName)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(docName, false))
            {
                DocumentManager.CurrentDocument.City.C01 = ReadExcel(document.WorkbookPart, 2, "C01", 3);
                DocumentManager.CurrentDocument.City.C02 = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "C02", 3));
                DocumentManager.CurrentDocument.City.C03 = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "C03", 3));
                DocumentManager.CurrentDocument.City.C04 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C04", 3));
                DocumentManager.CurrentDocument.City.C05 = Convert.ToDateTime(ReadExcel(document.WorkbookPart, 2, "C05", 3));
                DocumentManager.CurrentDocument.City.Country = ReadExcel(document.WorkbookPart, 1, "Country", 3);
                DocumentManager.CurrentDocument.City.Intro = ReadExcel(document.WorkbookPart, 1, "Intro", 3);
                DocumentManager.CurrentDocument.City.Currency = CityStatistics.CurrencyEnum.ToList().IndexOf(ReadExcel(document.WorkbookPart, 1, "Currency", 3));
                DocumentManager.CurrentDocument.City.Multiple = CityStatistics.MultipleEnum.ToList().IndexOf(ReadExcel(document.WorkbookPart, 1, "Multiple", 3));

                int year = DocumentManager.CurrentDocument.City.C02;
                DocumentManager.CurrentDocument.City.C1A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1A", 3));
                DocumentManager.CurrentDocument.City.C1A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1A", 4));
                DocumentManager.CurrentDocument.City.C1A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1A", 5));
                DocumentManager.CurrentDocument.City.C1A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1A", 6));
                DocumentManager.CurrentDocument.City.C1B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1B", 3));
                DocumentManager.CurrentDocument.City.C1B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1B", 4));
                DocumentManager.CurrentDocument.City.C1B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1B", 5));
                DocumentManager.CurrentDocument.City.C1B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1B", 6));
                DocumentManager.CurrentDocument.City.C1C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1C", 3));
                DocumentManager.CurrentDocument.City.C1C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1C", 4));
                DocumentManager.CurrentDocument.City.C1C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1C", 5));
                DocumentManager.CurrentDocument.City.C1C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1C", 6));
                DocumentManager.CurrentDocument.City.C1D[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1D", 3));
                DocumentManager.CurrentDocument.City.C1D[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1D", 4));
                DocumentManager.CurrentDocument.City.C1D[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1D", 5));
                DocumentManager.CurrentDocument.City.C1D[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1D", 6));
                DocumentManager.CurrentDocument.City.C1E[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1E", 3));
                DocumentManager.CurrentDocument.City.C1E[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1E", 4));
                DocumentManager.CurrentDocument.City.C1E[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1E", 5));
                DocumentManager.CurrentDocument.City.C1E[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C1E", 6));

                DocumentManager.CurrentDocument.City.C2A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2A", 3));
                DocumentManager.CurrentDocument.City.C2A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2A", 4));
                DocumentManager.CurrentDocument.City.C2A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2A", 5));
                DocumentManager.CurrentDocument.City.C2A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2A", 6));
                DocumentManager.CurrentDocument.City.C2B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2B", 3));
                DocumentManager.CurrentDocument.City.C2B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2B", 4));
                DocumentManager.CurrentDocument.City.C2B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2B", 5));
                DocumentManager.CurrentDocument.City.C2B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2B", 6));
                DocumentManager.CurrentDocument.City.C2C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2C", 3));
                DocumentManager.CurrentDocument.City.C2C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2C", 4));
                DocumentManager.CurrentDocument.City.C2C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2C", 5));
                DocumentManager.CurrentDocument.City.C2C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C2C", 6));

                DocumentManager.CurrentDocument.City.C3A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3A", 3));
                DocumentManager.CurrentDocument.City.C3A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3A", 4));
                DocumentManager.CurrentDocument.City.C3A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3A", 5));
                DocumentManager.CurrentDocument.City.C3A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3A", 6));
                DocumentManager.CurrentDocument.City.C3B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3B", 3));
                DocumentManager.CurrentDocument.City.C3B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3B", 4));
                DocumentManager.CurrentDocument.City.C3B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3B", 5));
                DocumentManager.CurrentDocument.City.C3B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3B", 6));
                DocumentManager.CurrentDocument.City.C3C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3C", 3));
                DocumentManager.CurrentDocument.City.C3C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3C", 4));
                DocumentManager.CurrentDocument.City.C3C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3C", 5));
                DocumentManager.CurrentDocument.City.C3C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3C", 6));
                DocumentManager.CurrentDocument.City.C3D[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3D", 3));
                DocumentManager.CurrentDocument.City.C3D[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3D", 4));
                DocumentManager.CurrentDocument.City.C3D[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3D", 5));
                DocumentManager.CurrentDocument.City.C3D[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C3D", 6));

                DocumentManager.CurrentDocument.City.C4A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 3));
                DocumentManager.CurrentDocument.City.C4A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 4));
                DocumentManager.CurrentDocument.City.C4A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 5));
                DocumentManager.CurrentDocument.City.C4A[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 6));
                DocumentManager.CurrentDocument.City.C4A[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 7));
                DocumentManager.CurrentDocument.City.C4A[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 8));
                DocumentManager.CurrentDocument.City.C4A[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 9));
                DocumentManager.CurrentDocument.City.C4A[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 10));
                DocumentManager.CurrentDocument.City.C4A[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 11));
                DocumentManager.CurrentDocument.City.C4A[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 12));
                DocumentManager.CurrentDocument.City.C4A[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 13));
                DocumentManager.CurrentDocument.City.C4A[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 14));
                DocumentManager.CurrentDocument.City.C4A[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 15));
                DocumentManager.CurrentDocument.City.C4A[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4A", 16));
                DocumentManager.CurrentDocument.City.C4B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 3));
                DocumentManager.CurrentDocument.City.C4B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 4));
                DocumentManager.CurrentDocument.City.C4B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 5));
                DocumentManager.CurrentDocument.City.C4B[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 6));
                DocumentManager.CurrentDocument.City.C4B[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 7));
                DocumentManager.CurrentDocument.City.C4B[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 8));
                DocumentManager.CurrentDocument.City.C4B[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 9));
                DocumentManager.CurrentDocument.City.C4B[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 10));
                DocumentManager.CurrentDocument.City.C4B[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 11));
                DocumentManager.CurrentDocument.City.C4B[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 12));
                DocumentManager.CurrentDocument.City.C4B[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 13));
                DocumentManager.CurrentDocument.City.C4B[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 14));
                DocumentManager.CurrentDocument.City.C4B[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 15));
                DocumentManager.CurrentDocument.City.C4B[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4B", 16));
                DocumentManager.CurrentDocument.City.C4C[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 3));
                DocumentManager.CurrentDocument.City.C4C[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 4));
                DocumentManager.CurrentDocument.City.C4C[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 5));
                DocumentManager.CurrentDocument.City.C4C[year] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 6));
                DocumentManager.CurrentDocument.City.C4C[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 7));
                DocumentManager.CurrentDocument.City.C4C[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 8));
                DocumentManager.CurrentDocument.City.C4C[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 9));
                DocumentManager.CurrentDocument.City.C4C[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 10));
                DocumentManager.CurrentDocument.City.C4C[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 11));
                DocumentManager.CurrentDocument.City.C4C[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 12));
                DocumentManager.CurrentDocument.City.C4C[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 13));
                DocumentManager.CurrentDocument.City.C4C[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 14));
                DocumentManager.CurrentDocument.City.C4C[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 15));
                DocumentManager.CurrentDocument.City.C4C[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 2, "C4C", 16));

                DocumentManager.CurrentDocument.City.Questionnaire["CQ01"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ01", 3));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ02"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ02", 3));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ03"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ03", 3));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ04"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ04", 3));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ05"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ05", 3));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ06"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ06", 3));
                DocumentManager.CurrentDocument.City.Questionnaire["CQ07"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, 2, "CQ07", 3));

                DocumentManager.CurrentDocument.City.C9A[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 3));
                DocumentManager.CurrentDocument.City.C9A[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 4));
                DocumentManager.CurrentDocument.City.C9A[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 5));
                DocumentManager.CurrentDocument.City.C9A[year + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 6));
                DocumentManager.CurrentDocument.City.C9A[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 7));
                DocumentManager.CurrentDocument.City.C9A[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 8));
                DocumentManager.CurrentDocument.City.C9A[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 9));
                DocumentManager.CurrentDocument.City.C9A[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 10));
                DocumentManager.CurrentDocument.City.C9A[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 11));
                DocumentManager.CurrentDocument.City.C9A[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 12));
                DocumentManager.CurrentDocument.City.C9A[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 13));
                DocumentManager.CurrentDocument.City.C9A[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 14));
                DocumentManager.CurrentDocument.City.C9A[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 15));
                DocumentManager.CurrentDocument.City.C9A[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9A", 16));
                DocumentManager.CurrentDocument.City.C9B[year - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 3));
                DocumentManager.CurrentDocument.City.C9B[year - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 4));
                DocumentManager.CurrentDocument.City.C9B[year - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 5));
                DocumentManager.CurrentDocument.City.C9B[year + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 6));
                DocumentManager.CurrentDocument.City.C9B[year + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 7));
                DocumentManager.CurrentDocument.City.C9B[year + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 8));
                DocumentManager.CurrentDocument.City.C9B[year + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 9));
                DocumentManager.CurrentDocument.City.C9B[year + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 10));
                DocumentManager.CurrentDocument.City.C9B[year + 5] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 11));
                DocumentManager.CurrentDocument.City.C9B[year + 6] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 12));
                DocumentManager.CurrentDocument.City.C9B[year + 7] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 13));
                DocumentManager.CurrentDocument.City.C9B[year + 8] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 14));
                DocumentManager.CurrentDocument.City.C9B[year + 9] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 15));
                DocumentManager.CurrentDocument.City.C9B[year + 10] = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C9B", 16));

                DocumentManager.CurrentDocument.City.C10A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C10A", 3));
                DocumentManager.CurrentDocument.City.C10B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C10B", 3));
                                                                                       
                DocumentManager.CurrentDocument.City.C11A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C11A", 3));
                DocumentManager.CurrentDocument.City.C11B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C11B", 3));
                                                                                       
                DocumentManager.CurrentDocument.City.C12A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C12A", 3));
                DocumentManager.CurrentDocument.City.C12B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C12B", 3));
                DocumentManager.CurrentDocument.City.C12C = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C12C", 3));
                                                                                       
                DocumentManager.CurrentDocument.City.C13A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C13A", 3));
                                                                                       
                DocumentManager.CurrentDocument.City.C14A = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C14A", 3));
                DocumentManager.CurrentDocument.City.C14B = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C14B", 3));

                DocumentManager.CurrentDocument.City.C15A_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15A_1", 3));
                DocumentManager.CurrentDocument.City.C15A_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15A_2", 3));
                DocumentManager.CurrentDocument.City.C15B_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15B_1", 3));
                DocumentManager.CurrentDocument.City.C15B_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15B_2", 3));
                DocumentManager.CurrentDocument.City.C15C_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15C_1", 3));
                DocumentManager.CurrentDocument.City.C15C_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15C_2", 3));
                DocumentManager.CurrentDocument.City.C15D_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15D_1", 3));
                DocumentManager.CurrentDocument.City.C15D_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15D_2", 3));
                DocumentManager.CurrentDocument.City.C15E_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15E_1", 3));
                DocumentManager.CurrentDocument.City.C15E_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C15E_2", 3));

                DocumentManager.CurrentDocument.City.C16A_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C16A_1", 3));
                DocumentManager.CurrentDocument.City.C16A_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C16A_2", 3));
                DocumentManager.CurrentDocument.City.C16B_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C16B_1", 3));
                DocumentManager.CurrentDocument.City.C16B_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C16B_2", 3));
                DocumentManager.CurrentDocument.City.C16C_1 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C16C_1", 3));
                DocumentManager.CurrentDocument.City.C16C_2 = Convert.ToDouble(ReadExcel(document.WorkbookPart, 3, "C16C_2", 3));
                                                                                       
                for (int i = 4; i <= GetExcel.GetSheetsNum(docName); i++)
                {
                    ProjectStatistics projStcs = new ProjectStatistics(ReadExcel(document.WorkbookPart, (uint)i, "P1A", 3), ReadExcel(document.WorkbookPart, (uint)i, "P1B", 3));
                    projStcs.P1C[0] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "P1C", 3));
                    projStcs.P1D = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "P1D", 3));
                    projStcs.P1E = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "P1E", 3));
                    projStcs.P1F_1 = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "P1F_1", 3));
                    projStcs.P1F_2 = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "P1F_2", 3));
                    projStcs.PIP = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PIP", 3));

                    int year1 = projStcs.P1F_1;
                    projStcs.P2A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2A", 3));
                    projStcs.P2A[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2A", 4));
                    projStcs.P2A[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2A", 5));
                    projStcs.P2A[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2A", 6));
                    projStcs.P2A[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2A", 7));
                    projStcs.P2B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2B", 3));
                    projStcs.P2B[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2B", 4));
                    projStcs.P2B[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2B", 5));
                    projStcs.P2B[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2B", 6));
                    projStcs.P2B[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2B", 7));
                    projStcs.P2C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2C", 3));
                    projStcs.P2C[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2C", 4));
                    projStcs.P2C[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2C", 5));
                    projStcs.P2C[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2C", 6));
                    projStcs.P2C[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2C", 7));
                    projStcs.P2D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2D", 3));
                    projStcs.P2D[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2D", 4));
                    projStcs.P2D[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2D", 5));
                    projStcs.P2D[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2D", 6));
                    projStcs.P2D[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2D", 7));
                    projStcs.P2E[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2E", 3));
                    projStcs.P2E[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2E", 4));
                    projStcs.P2E[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2E", 5));
                    projStcs.P2E[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2E", 6));
                    projStcs.P2E[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P2E", 7));

                    projStcs.P3A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3A", 3));
                    projStcs.P3A[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3A", 4));
                    projStcs.P3A[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3A", 5));
                    projStcs.P3A[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3A", 6));
                    projStcs.P3A[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3A", 7));
                    projStcs.P3B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3B", 3));
                    projStcs.P3B[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3B", 4));
                    projStcs.P3B[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3B", 5));
                    projStcs.P3B[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3B", 6));
                    projStcs.P3B[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3B", 7));
                    projStcs.P3C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3C", 3));
                    projStcs.P3C[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3C", 4));
                    projStcs.P3C[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3C", 5));
                    projStcs.P3C[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3C", 6));
                    projStcs.P3C[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3C", 7));
                    projStcs.P3D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3D", 3));
                    projStcs.P3D[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3D", 4));
                    projStcs.P3D[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3D", 5));
                    projStcs.P3D[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3D", 6));
                    projStcs.P3D[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3D", 7));
                    projStcs.P3E[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3E", 3));
                    projStcs.P3E[year1 + 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3E", 4));
                    projStcs.P3E[year1 + 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3E", 5));
                    projStcs.P3E[year1 + 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3E", 6));
                    projStcs.P3E[year1 + 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P3E", 7));

                    projStcs.P4A = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P4A", 3));
                                                             
                    projStcs.P5B = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P5B", 3));
                    projStcs.P6B = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P6B", 3));
                                                             
                    projStcs.P7A = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P7A", 3));
                    projStcs.P7B = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P7B", 3));
                    projStcs.P7C = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P7C", 3));
                    projStcs.P7D = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P7D", 3));

                    projStcs.P13A1 = ReadExcel(document.WorkbookPart, (uint)i, "P13A1", 2);
                    projStcs.P13A2 = ReadExcel(document.WorkbookPart, (uint)i, "P13A2", 2);
                    projStcs.P13A3 = ReadExcel(document.WorkbookPart, (uint)i, "P13A3", 2);
                    projStcs.P13B = ReadExcel(document.WorkbookPart, (uint)i, "P13B", 2);
                    projStcs.P13C = ReadExcel(document.WorkbookPart, (uint)i, "P13C", 2);
                    projStcs.P13D1 = ReadExcel(document.WorkbookPart, (uint)i, "P13D1", 2);
                    projStcs.P13D2 = ReadExcel(document.WorkbookPart, (uint)i, "P13D2", 2);
                    projStcs.P13D3 = ReadExcel(document.WorkbookPart, (uint)i, "P13D3", 2);

                    projStcs.P14A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14A", 3));
                    projStcs.P14A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14A", 4));
                    projStcs.P14A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14A", 5));
                    projStcs.P14A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14A", 6));
                    projStcs.P14A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14A", 7));
                    projStcs.P14B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14B", 3));
                    projStcs.P14B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14B", 4));
                    projStcs.P14B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14B", 5));
                    projStcs.P14B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14B", 6));
                    projStcs.P14B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14B", 7));
                    projStcs.P14C[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14C", 3));
                    projStcs.P14C[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14C", 4));
                    projStcs.P14C[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14C", 5));
                    projStcs.P14C[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14C", 6));
                    projStcs.P14C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14C", 7));
                    projStcs.P14D[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14D", 3));
                    projStcs.P14D[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14D", 4));
                    projStcs.P14D[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14D", 5));
                    projStcs.P14D[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14D", 6));
                    projStcs.P14D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P14D", 7));
                                                                                   
                    projStcs.P15A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15A", 3));
                    projStcs.P15A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15A", 4));
                    projStcs.P15A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15A", 5));
                    projStcs.P15A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15A", 6));
                    projStcs.P15A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15A", 7));
                    projStcs.P15B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15B", 3));
                    projStcs.P15B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15B", 4));
                    projStcs.P15B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15B", 5));
                    projStcs.P15B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15B", 6));
                    projStcs.P15B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15B", 7));
                    projStcs.P15C[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15C", 3));
                    projStcs.P15C[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15C", 4));
                    projStcs.P15C[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15C", 5));
                    projStcs.P15C[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15C", 6));
                    projStcs.P15C[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15C", 7));
                    projStcs.P15D[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15D", 3));
                    projStcs.P15D[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15D", 4));
                    projStcs.P15D[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15D", 5));
                    projStcs.P15D[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15D", 6));
                    projStcs.P15D[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P15D", 7));

                    projStcs.P16A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16A", 3));
                    projStcs.P16A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16A", 4));
                    projStcs.P16A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16A", 5));
                    projStcs.P16A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16A", 6));
                    projStcs.P16A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16A", 7));
                    projStcs.P16B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16B", 3));
                    projStcs.P16B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16B", 4));
                    projStcs.P16B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16B", 5));
                    projStcs.P16B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16B", 6));
                    projStcs.P16B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P16B", 7));

                    projStcs.P17A[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17A", 3));
                    projStcs.P17A[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17A", 4));
                    projStcs.P17A[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17A", 5));
                    projStcs.P17A[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17A", 6));
                    projStcs.P17A[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17A", 7));
                    projStcs.P17B[year1 - 4] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17B", 3));
                    projStcs.P17B[year1 - 3] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17B", 4));
                    projStcs.P17B[year1 - 2] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17B", 5));
                    projStcs.P17B[year1 - 1] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17B", 6));
                    projStcs.P17B[year1 + 0] = Convert.ToDouble(ReadExcel(document.WorkbookPart, (uint)i, "P17B", 7));

                    projStcs.Questionnaire["PQ01"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ01", 3));
                    projStcs.Questionnaire["PQ02"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ02", 3));
                    projStcs.Questionnaire["PQ03"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ03", 3));
                    projStcs.Questionnaire["PQ04"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ04", 3));
                    projStcs.Questionnaire["PQ05"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ05", 3));
                    projStcs.Questionnaire["PQ06"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ06", 3));
                    projStcs.Questionnaire["PQ07"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ07", 3));
                    projStcs.Questionnaire["PQ08"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ08", 3));
                    projStcs.Questionnaire["PQ09"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ09", 3));
                    projStcs.Questionnaire["PQ10"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ10", 3));
                    projStcs.Questionnaire["PQ11"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ11", 3));
                    projStcs.Questionnaire["PQ12"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ12", 3));
                    projStcs.Questionnaire["PQ13"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ13", 3));
                    projStcs.Questionnaire["PQ14"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ14", 3));
                    projStcs.Questionnaire["PQ15"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ15", 3));
                    projStcs.Questionnaire["PQ16"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ16", 3));
                    projStcs.Questionnaire["PQ17"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ17", 3));
                    projStcs.Questionnaire["PQ18"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ18", 3));
                    projStcs.Questionnaire["PQ19"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ19", 3));
                    projStcs.Questionnaire["PQ20"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ20", 3));
                    projStcs.Questionnaire["PQ21"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ21", 3));
                    projStcs.Questionnaire["PQ22"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ22", 3));
                    projStcs.Questionnaire["PQ23"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ23", 3));
                    projStcs.Questionnaire["PQ24"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ24", 3));
                    projStcs.Questionnaire["PQ25"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ25", 3));
                    projStcs.Questionnaire["PQ26"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ26", 3));
                    projStcs.Questionnaire["PQ27"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ27", 3));
                    projStcs.Questionnaire["PQ28"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ28", 3));
                    projStcs.Questionnaire["PQ29"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ29", 3));
                    projStcs.Questionnaire["PQ30"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ30", 3));
                    projStcs.Questionnaire["PQ31"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ31", 3));
                    projStcs.Questionnaire["PQ32"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ32", 3));
                    projStcs.Questionnaire["PQ33"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ33", 3));
                    projStcs.Questionnaire["PQ34"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ34", 3));
                    projStcs.Questionnaire["PQ35"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ35", 3));
                    projStcs.Questionnaire["PQ36"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ36", 3));
                    projStcs.Questionnaire["PQ37"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ37", 3));
                    projStcs.Questionnaire["PQ38"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ38", 3));
                    projStcs.Questionnaire["PQ39"] = Convert.ToInt32(ReadExcel(document.WorkbookPart, (uint)i, "PQ39", 3));
                    DocumentManager.CurrentDocument.Projects.Add(projStcs);    
                }                                                              
            }                                                                  
        }

        public static string ReadExcel(WorkbookPart work, uint SheetId, string cellText, int col)
        {
            string cLo = GetExcel.MatchExcelById(cellText, work, SheetId);
            uint r = GetExcel.GetRowIndex(cLo);
            string c = GetExcel.GetColumnName(cLo);
            return GetExcel.GetExcelValueById(r, GetCol(c, col), work, SheetId);
        }

        public static string GetCol(string st, int i)
        {
            string col = string.Empty;
            byte[] array = new byte[1];
            array = System.Text.Encoding.ASCII.GetBytes(st);
            int asciicode = (int)(array[0]);
            byte[] byteArray = new byte[] { (byte)(asciicode + i) };
            col = System.Text.Encoding.ASCII.GetString(byteArray);
            return col;
        }
    }
}
