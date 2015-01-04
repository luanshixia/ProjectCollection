using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using ExcelAppliction = Microsoft.Office.Interop.Excel.Application;
using System.Text.RegularExpressions;

namespace CIIPP
{
    public static class GetCiippExcel
    {
        public static ExcelAppliction excelApp;
        public static void GetCIIPPExcelTabel(string docName)
        {
            excelApp = new ExcelAppliction();
            excelApp.Visible = false;
            excelApp.DisplayAlerts = false;
            Workbook wBook = excelApp.Workbooks.Open(docName);
            Worksheet wSheet = wBook.Worksheets.Cast<Worksheet>().First(x => x.Name.Trim() == "City") as Worksheet;

            DocumentManager.CurrentDocument.City.C01 = (wSheet.Cells[3, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C02 = Convert.ToInt32((wSheet.Cells[4, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C03 = Convert.ToInt32(GetNumber((wSheet.Cells[5, 5] as Range).Value));
            DocumentManager.CurrentDocument.City.C04 = (wSheet.Cells[6, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C05 = (wSheet.Cells[7, 5] as Range).Value;
            int year = DocumentManager.CurrentDocument.City.C02;
            DocumentManager.CurrentDocument.City.C1A[year - 3] = (wSheet.Cells[12, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C1A[year - 2] = (wSheet.Cells[12, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C1A[year - 1] = (wSheet.Cells[12, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C1A[year] = (wSheet.Cells[12, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C1B[year - 3] = (wSheet.Cells[13, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C1B[year - 2] = (wSheet.Cells[13, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C1B[year - 1] = (wSheet.Cells[13, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C1B[year] = (wSheet.Cells[13, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C1C[year - 3] = (wSheet.Cells[14, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C1C[year - 2] = (wSheet.Cells[14, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C1C[year - 1] = (wSheet.Cells[14, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C1C[year] = (wSheet.Cells[14, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C1D[year - 3] = (wSheet.Cells[15, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C1D[year - 2] = (wSheet.Cells[15, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C1D[year - 1] = (wSheet.Cells[15, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C1D[year] = (wSheet.Cells[15, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C1E[year - 3] = (wSheet.Cells[16, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C1E[year - 2] = (wSheet.Cells[16, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C1E[year - 1] = (wSheet.Cells[16, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C1E[year] = (wSheet.Cells[16, 7] as Range).Value;

            DocumentManager.CurrentDocument.City.C2A[year - 3] = (wSheet.Cells[26, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C2A[year - 2] = (wSheet.Cells[26, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C2A[year - 1] = (wSheet.Cells[26, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C2A[year] = (wSheet.Cells[26, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C2B[year - 3] = (wSheet.Cells[27, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C2B[year - 2] = (wSheet.Cells[27, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C2B[year - 1] = (wSheet.Cells[27, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C2B[year] = (wSheet.Cells[27, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C2C[year - 3] = (wSheet.Cells[28, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C2C[year - 2] = (wSheet.Cells[28, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C2C[year - 1] = (wSheet.Cells[28, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C2C[year] = (wSheet.Cells[28, 7] as Range).Value;

            DocumentManager.CurrentDocument.City.C3A[year - 3] = (wSheet.Cells[37, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C3A[year - 2] = (wSheet.Cells[37, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C3A[year - 1] = (wSheet.Cells[37, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C3A[year] = (wSheet.Cells[37, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C3B[year - 3] = (wSheet.Cells[38, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C3B[year - 2] = (wSheet.Cells[38, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C3B[year - 1] = (wSheet.Cells[38, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C3B[year] = (wSheet.Cells[38, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C3C[year - 3] = (wSheet.Cells[39, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C3C[year - 2] = (wSheet.Cells[39, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C3C[year - 1] = (wSheet.Cells[39, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C3C[year] = (wSheet.Cells[39, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C3D[year - 3] = (wSheet.Cells[40, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C3D[year - 2] = (wSheet.Cells[40, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C3D[year - 1] = (wSheet.Cells[40, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C3D[year] = (wSheet.Cells[40, 7] as Range).Value;

            DocumentManager.CurrentDocument.City.C4A[year - 3] = (wSheet.Cells[46, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year - 2] = (wSheet.Cells[46, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year - 1] = (wSheet.Cells[46, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 0] = (wSheet.Cells[46, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 1] = (wSheet.Cells[46, 8] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 2] = (wSheet.Cells[46, 9] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 3] = (wSheet.Cells[46, 10] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 4] = (wSheet.Cells[46, 11] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 5] = (wSheet.Cells[46, 12] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 6] = (wSheet.Cells[46, 13] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 7] = (wSheet.Cells[46, 14] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 8] = (wSheet.Cells[46, 15] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 9] = (wSheet.Cells[46, 16] as Range).Value;
            DocumentManager.CurrentDocument.City.C4A[year + 10] = (wSheet.Cells[46, 17] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year - 3] = (wSheet.Cells[48, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year - 2] = (wSheet.Cells[48, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year - 1] = (wSheet.Cells[48, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 0] = (wSheet.Cells[48, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 1] = (wSheet.Cells[48, 8] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 2] = (wSheet.Cells[48, 9] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 3] = (wSheet.Cells[48, 10] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 4] = (wSheet.Cells[48, 11] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 5] = (wSheet.Cells[48, 12] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 6] = (wSheet.Cells[48, 13] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 7] = (wSheet.Cells[48, 14] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 8] = (wSheet.Cells[48, 15] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 9] = (wSheet.Cells[48, 16] as Range).Value;
            DocumentManager.CurrentDocument.City.C4B[year + 10] = (wSheet.Cells[48, 17] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year - 3] = (wSheet.Cells[49, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year - 2] = (wSheet.Cells[49, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year - 1] = (wSheet.Cells[49, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 0] = (wSheet.Cells[49, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 1] = (wSheet.Cells[49, 8] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 2] = (wSheet.Cells[49, 9] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 3] = (wSheet.Cells[49, 10] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 4] = (wSheet.Cells[49, 11] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 5] = (wSheet.Cells[49, 12] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 6] = (wSheet.Cells[49, 13] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 7] = (wSheet.Cells[49, 14] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 8] = (wSheet.Cells[49, 15] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 9] = (wSheet.Cells[49, 16] as Range).Value;
            DocumentManager.CurrentDocument.City.C4C[year + 10] = (wSheet.Cells[49, 17] as Range).Value;

            DocumentManager.CurrentDocument.City.Questionnaire["CQ01"] = C1(wSheet.Cells[56, 5] as Range);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ02"] = C2(wSheet.Cells[59, 5] as Range);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ03"] = C3(wSheet.Cells[62, 5] as Range);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ04"] = C4(wSheet.Cells[63, 5] as Range);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ05"] = C5(wSheet.Cells[66, 5] as Range);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ06"] = C6(wSheet.Cells[69, 5] as Range);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ07"] = C7(wSheet.Cells[72, 5] as Range);

            Worksheet wSheet1 = wBook.Worksheets.Cast<Worksheet>().First(x => x.Name.Trim() == "Budget Forecast") as Worksheet;

            DocumentManager.CurrentDocument.City.C9A[year - 3] = (wSheet1.Cells[32, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year - 2] = (wSheet1.Cells[32, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year - 1] = (wSheet1.Cells[32, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 0] = (wSheet1.Cells[32, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 1] = (wSheet1.Cells[32, 8] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 2] = (wSheet1.Cells[32, 9] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 3] = (wSheet1.Cells[32, 10] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 4] = (wSheet1.Cells[32, 11] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 5] = (wSheet1.Cells[32, 12] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 6] = (wSheet1.Cells[32, 13] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 7] = (wSheet1.Cells[32, 14] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 8] = (wSheet1.Cells[32, 15] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 9] = (wSheet1.Cells[32, 16] as Range).Value;
            DocumentManager.CurrentDocument.City.C9A[year + 10] = (wSheet1.Cells[32, 17] as Range).Value;

            DocumentManager.CurrentDocument.City.C9B[year - 3] = (wSheet1.Cells[33, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year - 2] = (wSheet1.Cells[33, 5] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year - 1] = (wSheet1.Cells[33, 6] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 0] = (wSheet1.Cells[33, 7] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 1] = (wSheet1.Cells[33, 8] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 2] = (wSheet1.Cells[33, 9] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 3] = (wSheet1.Cells[33, 10] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 4] = (wSheet1.Cells[33, 11] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 5] = (wSheet1.Cells[33, 12] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 6] = (wSheet1.Cells[33, 13] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 7] = (wSheet1.Cells[33, 14] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 8] = (wSheet1.Cells[33, 15] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 9] = (wSheet1.Cells[33, 16] as Range).Value;
            DocumentManager.CurrentDocument.City.C9B[year + 10] = (wSheet1.Cells[33, 17] as Range).Value;

            DocumentManager.CurrentDocument.City.C10A = Ver(wSheet1.Cells[37, 4] as Range);
            DocumentManager.CurrentDocument.City.C10B = Ver(wSheet1.Cells[38, 4] as Range);

            DocumentManager.CurrentDocument.City.C11A = Ver(wSheet1.Cells[40, 4] as Range);
            DocumentManager.CurrentDocument.City.C11B = Ver(wSheet1.Cells[41, 4] as Range);

            DocumentManager.CurrentDocument.City.C12A = Ver(wSheet1.Cells[45, 4] as Range);
            DocumentManager.CurrentDocument.City.C12B = Ver(wSheet1.Cells[46, 4] as Range);
            DocumentManager.CurrentDocument.City.C12C = Ver(wSheet1.Cells[47, 4] as Range);

            DocumentManager.CurrentDocument.City.C13A = Ver(wSheet1.Cells[51, 4] as Range);

            DocumentManager.CurrentDocument.City.C14A = Ver(wSheet1.Cells[55, 4] as Range);
            DocumentManager.CurrentDocument.City.C14B = Ver(wSheet1.Cells[56, 4] as Range);

            DocumentManager.CurrentDocument.City.C15A_1 = Ver(wSheet1.Cells[60, 4] as Range);
            DocumentManager.CurrentDocument.City.C15A_2 = Ver(wSheet1.Cells[60, 5] as Range);
            DocumentManager.CurrentDocument.City.C15B_1 = Ver(wSheet1.Cells[61, 4] as Range);
            DocumentManager.CurrentDocument.City.C15B_2 = Ver(wSheet1.Cells[61, 5] as Range);
            DocumentManager.CurrentDocument.City.C15C_1 = Ver(wSheet1.Cells[62, 4] as Range);
            DocumentManager.CurrentDocument.City.C15C_2 = Ver(wSheet1.Cells[62, 5] as Range);
            DocumentManager.CurrentDocument.City.C15D_1 = Ver(wSheet1.Cells[63, 4] as Range);
            DocumentManager.CurrentDocument.City.C15D_2 = Ver(wSheet1.Cells[63, 5] as Range);
            DocumentManager.CurrentDocument.City.C15E_1 = Ver(wSheet1.Cells[64, 4] as Range);
            DocumentManager.CurrentDocument.City.C15E_2 = Ver(wSheet1.Cells[64, 5] as Range);

            DocumentManager.CurrentDocument.City.C16A_1 = Ver(wSheet1.Cells[68, 4] as Range);
            DocumentManager.CurrentDocument.City.C16A_2 = Ver(wSheet1.Cells[68, 5] as Range);
            DocumentManager.CurrentDocument.City.C16B_1 = Ver(wSheet1.Cells[69, 4] as Range);
            DocumentManager.CurrentDocument.City.C16B_2 = Ver(wSheet1.Cells[69, 5] as Range);
            DocumentManager.CurrentDocument.City.C16C_1 = Ver(wSheet1.Cells[70, 4] as Range);
            DocumentManager.CurrentDocument.City.C16C_2 = Ver(wSheet1.Cells[70, 5] as Range);

            string[] Name = new string[] { "Budget Forecast", "Home", "CDIA", "Configuration", "ProjectTemplate", "CityScenario", "Summary", "City", "Committed", "Prepared", "Ideas", "PIP" };
            IEnumerable<Worksheet> workSheet = wBook.Worksheets.Cast<Worksheet>().Where(x => Array.IndexOf(Name, x.Name.Trim()) < 0);

            for (int i = 1; i < workSheet.Count() - 1; i++)
            {
                ProjectStatistics projStcs = new ProjectStatistics((workSheet.ElementAt(i).Cells[3, 6] as Range).Value, (workSheet.ElementAt(i).Cells[4, 6] as Range).Value);
                projStcs.P1C[0] = P1C(workSheet.ElementAt(i).Cells[6, 6] as Range);
                projStcs.P1D = P1D(workSheet.ElementAt(i).Cells[8, 6] as Range);
                projStcs.P1E = P1E(workSheet.ElementAt(i).Cells[10, 7] as Range);
                projStcs.P1F_1 = Convert.ToInt32((workSheet.ElementAt(i).Cells[12, 7] as Range).Value);
                projStcs.P1F_2 = Convert.ToInt32((workSheet.ElementAt(i).Cells[13, 7] as Range).Value);
                projStcs.PIP = PIP(workSheet.ElementAt(i).Cells[1, 11] as Range);
                int year1 = projStcs.P1F_1;
                projStcs.P2A[year1 + 0] = (workSheet.ElementAt(i).Cells[18, 4] as Range).Value;
                projStcs.P2A[year1 + 1] = (workSheet.ElementAt(i).Cells[18, 5] as Range).Value;
                projStcs.P2A[year1 + 2] = (workSheet.ElementAt(i).Cells[18, 6] as Range).Value;
                projStcs.P2A[year1 + 3] = (workSheet.ElementAt(i).Cells[18, 7] as Range).Value;
                projStcs.P2A[year1 + 4] = (workSheet.ElementAt(i).Cells[18, 8] as Range).Value;
                projStcs.P2B[year1 + 0] = (workSheet.ElementAt(i).Cells[19, 4] as Range).Value;
                projStcs.P2B[year1 + 1] = (workSheet.ElementAt(i).Cells[19, 5] as Range).Value;
                projStcs.P2B[year1 + 2] = (workSheet.ElementAt(i).Cells[19, 6] as Range).Value;
                projStcs.P2B[year1 + 3] = (workSheet.ElementAt(i).Cells[19, 7] as Range).Value;
                projStcs.P2B[year1 + 4] = (workSheet.ElementAt(i).Cells[19, 8] as Range).Value;
                projStcs.P2C[year1 + 0] = (workSheet.ElementAt(i).Cells[20, 4] as Range).Value;
                projStcs.P2C[year1 + 1] = (workSheet.ElementAt(i).Cells[20, 5] as Range).Value;
                projStcs.P2C[year1 + 2] = (workSheet.ElementAt(i).Cells[20, 6] as Range).Value;
                projStcs.P2C[year1 + 3] = (workSheet.ElementAt(i).Cells[20, 7] as Range).Value;
                projStcs.P2C[year1 + 4] = (workSheet.ElementAt(i).Cells[20, 8] as Range).Value;
                projStcs.P2D[year1 + 0] = (workSheet.ElementAt(i).Cells[21, 4] as Range).Value;
                projStcs.P2D[year1 + 1] = (workSheet.ElementAt(i).Cells[21, 5] as Range).Value;
                projStcs.P2D[year1 + 2] = (workSheet.ElementAt(i).Cells[21, 6] as Range).Value;
                projStcs.P2D[year1 + 3] = (workSheet.ElementAt(i).Cells[21, 7] as Range).Value;
                projStcs.P2D[year1 + 4] = (workSheet.ElementAt(i).Cells[21, 8] as Range).Value;
                projStcs.P2E[year1 + 0] = (workSheet.ElementAt(i).Cells[22, 4] as Range).Value;
                projStcs.P2E[year1 + 1] = (workSheet.ElementAt(i).Cells[22, 5] as Range).Value;
                projStcs.P2E[year1 + 2] = (workSheet.ElementAt(i).Cells[22, 6] as Range).Value;
                projStcs.P2E[year1 + 3] = (workSheet.ElementAt(i).Cells[22, 7] as Range).Value;
                projStcs.P2E[year1 + 4] = (workSheet.ElementAt(i).Cells[22, 8] as Range).Value;

                projStcs.P3A[year1 + 0] = (workSheet.ElementAt(i).Cells[27, 4] as Range).Value;
                projStcs.P3A[year1 + 1] = (workSheet.ElementAt(i).Cells[27, 5] as Range).Value;
                projStcs.P3A[year1 + 2] = (workSheet.ElementAt(i).Cells[27, 6] as Range).Value;
                projStcs.P3A[year1 + 3] = (workSheet.ElementAt(i).Cells[27, 7] as Range).Value;
                projStcs.P3A[year1 + 4] = (workSheet.ElementAt(i).Cells[27, 8] as Range).Value;
                projStcs.P3B[year1 + 0] = (workSheet.ElementAt(i).Cells[28, 4] as Range).Value;
                projStcs.P3B[year1 + 1] = (workSheet.ElementAt(i).Cells[28, 5] as Range).Value;
                projStcs.P3B[year1 + 2] = (workSheet.ElementAt(i).Cells[28, 6] as Range).Value;
                projStcs.P3B[year1 + 3] = (workSheet.ElementAt(i).Cells[28, 7] as Range).Value;
                projStcs.P3B[year1 + 4] = (workSheet.ElementAt(i).Cells[28, 8] as Range).Value;
                projStcs.P3C[year1 + 0] = (workSheet.ElementAt(i).Cells[29, 4] as Range).Value;
                projStcs.P3C[year1 + 1] = (workSheet.ElementAt(i).Cells[29, 5] as Range).Value;
                projStcs.P3C[year1 + 2] = (workSheet.ElementAt(i).Cells[29, 6] as Range).Value;
                projStcs.P3C[year1 + 3] = (workSheet.ElementAt(i).Cells[29, 7] as Range).Value;
                projStcs.P3C[year1 + 4] = (workSheet.ElementAt(i).Cells[29, 8] as Range).Value;
                projStcs.P3D[year1 + 0] = (workSheet.ElementAt(i).Cells[30, 4] as Range).Value;
                projStcs.P3D[year1 + 1] = (workSheet.ElementAt(i).Cells[30, 5] as Range).Value;
                projStcs.P3D[year1 + 2] = (workSheet.ElementAt(i).Cells[30, 6] as Range).Value;
                projStcs.P3D[year1 + 3] = (workSheet.ElementAt(i).Cells[30, 7] as Range).Value;
                projStcs.P3D[year1 + 4] = (workSheet.ElementAt(i).Cells[30, 8] as Range).Value;
                projStcs.P3E[year1 + 0] = (workSheet.ElementAt(i).Cells[31, 4] as Range).Value;
                projStcs.P3E[year1 + 1] = (workSheet.ElementAt(i).Cells[31, 5] as Range).Value;
                projStcs.P3E[year1 + 2] = (workSheet.ElementAt(i).Cells[31, 6] as Range).Value;
                projStcs.P3E[year1 + 3] = (workSheet.ElementAt(i).Cells[31, 7] as Range).Value;
                projStcs.P3E[year1 + 4] = (workSheet.ElementAt(i).Cells[31, 8] as Range).Value;

                projStcs.P4A = Ver(workSheet.ElementAt(i).Cells[36, 4] as Range);

                projStcs.P5B = Ver(workSheet.ElementAt(i).Cells[37, 10] as Range);
                projStcs.P6B = Ver(workSheet.ElementAt(i).Cells[43, 10] as Range);

                projStcs.P7A = Ver(workSheet.ElementAt(i).Cells[39, 4] as Range);
                projStcs.P7B = Ver(workSheet.ElementAt(i).Cells[40, 4] as Range);
                projStcs.P7C = Ver(workSheet.ElementAt(i).Cells[41, 4] as Range);
                projStcs.P7D = Ver(workSheet.ElementAt(i).Cells[42, 4] as Range);

                projStcs.P13A1 = (workSheet.ElementAt(i).Cells[112, 5] as Range).Value;
                projStcs.P13A2 = (workSheet.ElementAt(i).Cells[113, 5] as Range).Value;
                projStcs.P13A3 = (workSheet.ElementAt(i).Cells[114, 5] as Range).Value;
                projStcs.P13B = (workSheet.ElementAt(i).Cells[116, 5] as Range).Value;
                projStcs.P13C = (workSheet.ElementAt(i).Cells[118, 5] as Range).Value;
                projStcs.P13D1 = (workSheet.ElementAt(i).Cells[120, 5] as Range).Value;
                projStcs.P13D2 = (workSheet.ElementAt(i).Cells[121, 5] as Range).Value;
                projStcs.P13D3 = (workSheet.ElementAt(i).Cells[122, 5] as Range).Value;

                projStcs.P14A[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[128, 4] as Range);
                projStcs.P14A[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[128, 5] as Range);
                projStcs.P14A[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[128, 6] as Range);
                projStcs.P14A[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[128, 7] as Range);
                projStcs.P14A[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[128, 8] as Range);
                projStcs.P14B[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[129, 4] as Range);
                projStcs.P14B[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[129, 5] as Range);
                projStcs.P14B[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[129, 6] as Range);
                projStcs.P14B[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[129, 7] as Range);
                projStcs.P14B[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[129, 8] as Range);
                projStcs.P14C[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[130, 4] as Range);
                projStcs.P14C[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[130, 5] as Range);
                projStcs.P14C[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[130, 6] as Range);
                projStcs.P14C[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[130, 7] as Range);
                projStcs.P14C[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[130, 8] as Range);
                projStcs.P14D[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[131, 4] as Range);
                projStcs.P14D[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[131, 5] as Range);
                projStcs.P14D[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[131, 6] as Range);
                projStcs.P14D[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[131, 7] as Range);
                projStcs.P14D[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[131, 8] as Range);

                projStcs.P15A[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[135, 4] as Range);
                projStcs.P15A[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[135, 5] as Range);
                projStcs.P15A[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[135, 6] as Range);
                projStcs.P15A[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[135, 7] as Range);
                projStcs.P15A[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[135, 8] as Range);
                projStcs.P15B[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[136, 4] as Range);
                projStcs.P15B[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[136, 5] as Range);
                projStcs.P15B[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[136, 6] as Range);
                projStcs.P15B[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[136, 7] as Range);
                projStcs.P15B[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[136, 8] as Range);
                projStcs.P15C[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[137, 4] as Range);
                projStcs.P15C[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[137, 5] as Range);
                projStcs.P15C[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[137, 6] as Range);
                projStcs.P15C[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[137, 7] as Range);
                projStcs.P15C[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[137, 8] as Range);
                projStcs.P15D[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[138, 4] as Range);
                projStcs.P15D[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[138, 5] as Range);
                projStcs.P15D[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[138, 6] as Range);
                projStcs.P15D[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[138, 7] as Range);
                projStcs.P15D[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[138, 8] as Range);

                projStcs.P16A[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[142, 4] as Range);
                projStcs.P16A[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[142, 5] as Range);
                projStcs.P16A[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[142, 6] as Range);
                projStcs.P16A[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[142, 7] as Range);
                projStcs.P16A[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[142, 8] as Range);
                projStcs.P16B[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[143, 4] as Range);
                projStcs.P16B[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[143, 5] as Range);
                projStcs.P16B[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[143, 6] as Range);
                projStcs.P16B[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[143, 7] as Range);
                projStcs.P16B[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[143, 8] as Range);

                projStcs.P17A[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[147, 4] as Range);
                projStcs.P17A[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[147, 5] as Range);
                projStcs.P17A[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[147, 6] as Range);
                projStcs.P17A[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[147, 7] as Range);
                projStcs.P17A[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[147, 8] as Range);
                projStcs.P17B[year1 - 4] = Ver(workSheet.ElementAt(i).Cells[148, 4] as Range);
                projStcs.P17B[year1 - 3] = Ver(workSheet.ElementAt(i).Cells[148, 5] as Range);
                projStcs.P17B[year1 - 2] = Ver(workSheet.ElementAt(i).Cells[148, 6] as Range);
                projStcs.P17B[year1 - 1] = Ver(workSheet.ElementAt(i).Cells[148, 7] as Range);
                projStcs.P17B[year1 + 0] = Ver(workSheet.ElementAt(i).Cells[148, 8] as Range);

                projStcs.Questionnaire["PQ01"] = P1(workSheet.ElementAt(i).Cells[48, 7] as Range);
                projStcs.Questionnaire["PQ02"] = P2(workSheet.ElementAt(i).Cells[49, 7] as Range);
                projStcs.Questionnaire["PQ03"] = P3(workSheet.ElementAt(i).Cells[50, 7] as Range);
                projStcs.Questionnaire["PQ04"] = P4(workSheet.ElementAt(i).Cells[51, 7] as Range);
                projStcs.Questionnaire["PQ05"] = P5(workSheet.ElementAt(i).Cells[52, 7] as Range);
                projStcs.Questionnaire["PQ06"] = P6(workSheet.ElementAt(i).Cells[53, 7] as Range);
                projStcs.Questionnaire["PQ07"] = P7(workSheet.ElementAt(i).Cells[57, 7] as Range);
                projStcs.Questionnaire["PQ08"] = P8(workSheet.ElementAt(i).Cells[58, 7] as Range);
                projStcs.Questionnaire["PQ09"] = P8(workSheet.ElementAt(i).Cells[59, 7] as Range);
                projStcs.Questionnaire["PQ10"] = P10(workSheet.ElementAt(i).Cells[60, 7] as Range);
                projStcs.Questionnaire["PQ11"] = P11(workSheet.ElementAt(i).Cells[61, 7] as Range);
                projStcs.Questionnaire["PQ12"] = P12(workSheet.ElementAt(i).Cells[62, 7] as Range);
                projStcs.Questionnaire["PQ13"] = P13(workSheet.ElementAt(i).Cells[63, 7] as Range);
                projStcs.Questionnaire["PQ14"] = P14(workSheet.ElementAt(i).Cells[64, 7] as Range);
                projStcs.Questionnaire["PQ15"] = P15(workSheet.ElementAt(i).Cells[68, 7] as Range);
                projStcs.Questionnaire["PQ16"] = P16(workSheet.ElementAt(i).Cells[69, 7] as Range);
                projStcs.Questionnaire["PQ17"] = P17(workSheet.ElementAt(i).Cells[70, 7] as Range);
                projStcs.Questionnaire["PQ18"] = P18(workSheet.ElementAt(i).Cells[71, 7] as Range);
                projStcs.Questionnaire["PQ19"] = P19(workSheet.ElementAt(i).Cells[72, 7] as Range);
                projStcs.Questionnaire["PQ20"] = P20(workSheet.ElementAt(i).Cells[73, 7] as Range);
                projStcs.Questionnaire["PQ21"] = P21(workSheet.ElementAt(i).Cells[77, 7] as Range);
                projStcs.Questionnaire["PQ22"] = P22(workSheet.ElementAt(i).Cells[78, 7] as Range);
                projStcs.Questionnaire["PQ23"] = P23(workSheet.ElementAt(i).Cells[79, 7] as Range);
                projStcs.Questionnaire["PQ24"] = P24(workSheet.ElementAt(i).Cells[80, 7] as Range);
                projStcs.Questionnaire["PQ25"] = P25(workSheet.ElementAt(i).Cells[81, 7] as Range);
                projStcs.Questionnaire["PQ26"] = P26(workSheet.ElementAt(i).Cells[82, 7] as Range);
                projStcs.Questionnaire["PQ27"] = P27(workSheet.ElementAt(i).Cells[83, 7] as Range);
                projStcs.Questionnaire["PQ28"] = P28(workSheet.ElementAt(i).Cells[84, 7] as Range);
                projStcs.Questionnaire["PQ29"] = P29(workSheet.ElementAt(i).Cells[85, 7] as Range);
                projStcs.Questionnaire["PQ30"] = P30(workSheet.ElementAt(i).Cells[89, 7] as Range);
                projStcs.Questionnaire["PQ31"] = P31(workSheet.ElementAt(i).Cells[90, 7] as Range);
                projStcs.Questionnaire["PQ32"] = P32(workSheet.ElementAt(i).Cells[91, 7] as Range);
                projStcs.Questionnaire["PQ33"] = P33(workSheet.ElementAt(i).Cells[92, 7] as Range);
                projStcs.Questionnaire["PQ34"] = P34(workSheet.ElementAt(i).Cells[93, 7] as Range);
                projStcs.Questionnaire["PQ35"] = P35(workSheet.ElementAt(i).Cells[94, 7] as Range);
                projStcs.Questionnaire["PQ36"] = P36(workSheet.ElementAt(i).Cells[95, 7] as Range);
                projStcs.Questionnaire["PQ37"] = P37(workSheet.ElementAt(i).Cells[96, 7] as Range);
                projStcs.Questionnaire["PQ38"] = P37(workSheet.ElementAt(i).Cells[97, 7] as Range);
                projStcs.Questionnaire["PQ39"] = P39(workSheet.ElementAt(i).Cells[98, 7] as Range);
                DocumentManager.CurrentDocument.Projects.Add(projStcs);
            }
            excelApp.Quit();
        }

        private static dynamic Ver(Range x)
        {
            if ((x).Value == null)
            { return 0; }
            else
            {
                return (x).Value;
            }
        }

        private static int PIP(Range x)
        {
            if ((x).Value == "Yes")
            { return 0; }
            else { return 1; }
        }

        private static int P1C(Range x)
        {
            if ((x).Value == "固体废物")
            {
                return 0;
            }
            else if ((x).Value == "水和污水 ")
            {
                return 1;
            }
            else if ((x).Value == "公路、铁路、桥梁、机场（港口）")
            {
                return 2;
            }
            else if ((x).Value == "供电、电力")
            {
                return 3;
            }
            else if ((x).Value == "商业 /工业 /技术设施")
            {
                return 4;
            }
            else if ((x).Value == "卫生")
            {
                return 5;
            }
            else if ((x).Value == "教育")
            {
                return 6;
            }
            else if ((x).Value == "其他")
            {
                return 8;
            }
            else { return 7; }
        }
        private static int P1D(Range x)
        {
            if ((x).Value == "ENV =环境")
            {
                return 0;
            }
            else if ((x).Value == "SOC =社会")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int P1E(Range x)
        {
            if ((x).Value == "COMM =项目/承诺资金")
            {
                return 0;
            }
            else if ((x).Value == "PREP =项目筹备")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int C1(Range x)
        {
            if ((x).Value == "The local government has no credit rating or the local government has risk rating (BB, B or C) rating")
            {
                return 0;
            }
            else if ((x).Value == "The local government has a moderate (BBB) rating")
            {
                return 1;
            }
            else if ((x).Value == "The local government has a safe  (AAA, AA, A) rating")
            {
                return 2;
            }
            else
            {
                return 3;
            }
        }

        private static int C2(Range x)
        {
            if ((x).Value == "Annual bugeting")
            {
                return 0;
            }
            else if ((x).Value == "Partial multi-year budgeting")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private static int C3(Range x)
        {
            if ((x).Value == "<75%")
            {
                return 0;
            }
            else if ((x).Value == "75-90%")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int C4(Range x)
        {
            if ((x).Value == "Capacity to collect local revenues needs significant improvement")
            {
                return 0;
            }
            else if ((x).Value == "Capacity to collect local revenues could be improved")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int C5(Range x)
        {
            if ((x).Value == "Basic financial procedures and control mechanism are in place")
            {
                return 0;
            }
            else if ((x).Value == "The system works with reasonable efficiency")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int C6(Range x)
        {
            if ((x).Value == "IT skills of staff and availability of computers are limited")
            {
                return 0;
            }
            else if ((x).Value == "Computers are available and staff have basic level of IT skills")
            {
                return 1;
            }
            else if ((x).Value == "Computers are widely available and IT skills of staff are well developed")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int C7(Range x)
        {
            if ((x).Value == "Too few in number and limited skill set")
            {
                return 0;
            }
            else if ((x).Value == "Adequate in number but skills need to be improved")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int P1(Range x)
        {
            if ((x).Value == "现有设施负担过重。需要额外设施。")
            {
                return 0;
            }
            else if ((x).Value == "设施可用，但服务不属最优之列（有待改善或扩展）")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int P2(Range x)
        {
            if ((x).Value == "<25%")
            {
                return 0;
            }
            else if ((x).Value == "25%-50%")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int P3(Range x)
        {
            if ((x).Value == "低优先级")
            {
                return 0;
            }
            else if ((x).Value == "优先项目")
            {
                return 1;
            }
            else if ((x).Value == "高优先级")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P4(Range x)
        {
            if ((x).Value == "没有贡献")
            {
                return 0;
            }
            else if ((x).Value == "间接贡献")
            {
                return 1;
            }
            else if ((x).Value == "直接贡献")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P5(Range x)
        {
            if ((x).Value == "无不良影响")
            {
                return 0;
            }
            else if ((x).Value == "次要的不良影响")
            {
                return 1;
            }
            else if ((x).Value == "重大的未来不良影响")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P6(Range x)
        {
            if ((x).Value == "与其他设施无关")
            {
                return 0;
            }
            else if ((x).Value == "其他设施将长期受益于该项目")
            {
                return 1;
            }
            else if ((x).Value == "其他设施将立即受益于该项目")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P7(Range x)
        {
            if ((x).Value == "社区发起者")
            {
                return 0;
            }
            else if ((x).Value == "城市管理部门内部发起者")
            {
                return 1;
            }
            else if ((x).Value == "市领导发起者")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P8(Range x)
        {
            if ((x).Value == "零星支持者")
            {
                return 0;
            }
            else if ((x).Value == " 少数支持")
            {
                return 1;
            }
            else if ((x).Value == "多数支持")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P10(Range x)
        {
            if ((x).Value == "零星支持者")
            {
                return 0;
            }
            else if ((x).Value == "少数支持")
            {
                return 1;
            }
            else if ((x).Value == "多数支持")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P11(Range x)
        {
            if ((x).Value == " 零星支持者")
            {
                return 0;
            }
            else if ((x).Value == "少数支持")
            {
                return 1;
            }
            else if ((x).Value == "多数支持")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P12(Range x)
        {
            if ((x).Value == "尚未进行，项目理念仍在完善中")
            {
                return 0;
            }
            else if ((x).Value == "未进行正式咨询，但公众已知情")
            {
                return 1;
            }
            else if ((x).Value == "计划已提交社区代表，以获得相关信息")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P13(Range x)
        {
            if ((x).Value == "是的，大规模（>100户须搬迁）")
            {
                return 0;
            }
            else if ((x).Value == "是的，小规模（50 -100户）住户须搬迁")
            {
                return 1;
            }
            else if ((x).Value == "涉及少于10户居民的搬迁和/或物业边界的重新协商")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P14(Range x)
        {
            if ((x).Value == "未准备做出贡献")
            {
                return 0;
            }
            else if ((x).Value == "需要额外努力以调动资源")
            {
                return 1;
            }
            else if ((x).Value == "有意向时准备做出贡献")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P15(Range x)
        {
            if ((x).Value == "对当地环境质量有显著的直接负面影响")
            {
                return 0;
            }
            else if ((x).Value == "对当地环境质量有直接负面影响")
            {
                return 1;
            }
            else if ((x).Value == "没有影响")
            {
                return 2;
            }
            else if ((x).Value == "对当地环境质量有一定的直接正面影响")
            { return 3; }
            else { return 4; }
        }
        private static int P16(Range x)
        {
            if ((x).Value == "不利于长期可持续发展")
            {
                return 0;
            }
            else if ((x).Value == "对长期可持续发展没有影响")
            {
                return 1;
            }
            else if ((x).Value == "对长期可持续发展有一定贡献")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P17(Range x)
        {
            if ((x).Value == "对总体健康状况有显著的负面影响")
            {
                return 0;
            }
            else if ((x).Value == "对总体健康状况有负面影响")
            {
                return 1;
            }
            else if ((x).Value == "没有影响")
            {
                return 2;
            }
            else if ((x).Value == "对总体健康状况有一定的积极影响")
            { return 3; }
            else { return 4; }
        }
        private static int P18(Range x)
        {
            if ((x).Value == "项目位于环境承受力脆弱的地点，但未纳入缓解措施")
            {
                return 0;
            }
            else if ((x).Value == "不相关")
            {
                return 1;
            }
            else if ((x).Value == "项目纳入缓解措施")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P19(Range x)
        {
            if ((x).Value == "新设施会增加温室气体排放")
            {
                return 0;
            }
            else if ((x).Value == "不相关")
            {
                return 1;
            }
            else if ((x).Value == "新设施在一定程度上会减少温室气体排放")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P20(Range x)
        {
            if ((x).Value == "对自然空间的质量有负面影响")
            {
                return 0;
            }
            else if ((x).Value == "没有影响")
            {
                return 1;
            }
            else if ((x).Value == "对自然空间的质量有一定益处")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P21(Range x)
        {
            if ((x).Value == "当地商业 /工业实体减少")
            {
                return 0;
            }
            else if ((x).Value == "没有（重大）影响")
            {
                return 1;
            }
            else if ((x).Value == "小企业数量小幅增加、规模略有扩大")
            {
                return 2;
            }
            else if ((x).Value == "小企业数量大幅增加、规模大幅扩大")
            { return 3; }
            else { return 4; }
        }
        private static int P22(Range x)
        {
            if ((x).Value == "对当地经济有负面影响")
            {
                return 0;
            }
            else if ((x).Value == "对长期经济发展有极少好处或没有好处")
            {
                return 1;
            }
            else if ((x).Value == "产生下游业务，可能会给市民带来经济效益和价值转移")
            {
                return 2;
            }
            else if ((x).Value == "该地区获得额外投资，增加市民财富")
            { return 3; }
            else { return 4; }
        }
        private static int P23(Range x)
        {
            if ((x).Value == "多数材料和劳动力将需从外部引进")
            {
                return 0;
            }
            else if ((x).Value == "所有劳动力均可在当地招聘，但部分材料必须从外部引进")
            {
                return 1;
            }
            else if ((x).Value == "所有材料均可在本地采购，但部分劳动力必须从外部引进")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P24(Range x)
        {
            if ((x).Value == "没有效果/没有显著效果")
            {
                return 0;
            }
            else if ((x).Value == "没有具体目标，但对生活质量有积极影响")
            {
                return 1;
            }
            else if ((x).Value == "没有具体目标，但对投资有积极影响")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P25(Range x)
        {
            if ((x).Value == "新设施不惠及低收入群体")
            {
                return 0;
            }
            else if ((x).Value == "该项目专门针对低收入阶层")
            {
                return 3;
            }
            else if ((x).Value == "没有具体益贫重点，但会增加低收入群体的就业机会")
            {
                return 2;
            }
            else { return 1; }
        }
        private static int P26(Range x)
        {
            if ((x).Value == "中等收入群体能够负担提议的费用")
            {
                return 0;
            }
            else if ((x).Value == "中低收入群体能够负担提议的费用")
            {
                return 1;
            }
            else if ((x).Value == "贫困人口能够负担提议的费用")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P27(Range x)
        {
            if ((x).Value == "不，该项目将会把某些群体排除在外")
            {
                return 0;
            }
            else if ((x).Value == "没有影响")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int P28(Range x)
        {
            if ((x).Value == "没有效果/没有显著效果")
            {
                return 0;
            }
            else if ((x).Value == " 没有具体重点，但对市中心居民生活质量有积极影响")
            {
                return 1;
            }
            else if ((x).Value == "没有具体重点，但会为市中心带来一些投资")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P29(Range x)
        {
            if ((x).Value == "没有影响")
            {
                return 0;
            }
            else if ((x).Value == "是的")
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        private static int P30(Range x)
        {
            if ((x).Value == "当地政府预算未对该项目分配资金")
            {
                return 0;
            }
            else if ((x).Value == "不需要，所有费用均由国家 /地区预算承担")
            {
                return 1;
            }
            else if ((x).Value == "是的，当地政府预算将承担项目部分费用")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P31(Range x)
        {
            if ((x).Value == "无需外部融资来源")
            {
                return 0;
            }
            else if ((x).Value == "有可能获得国家/地区拨款")
            {
                return 1;
            }
            else if ((x).Value == "有可能获得优惠贷款")
            {
                return 2;
            }
            else if ((x).Value == "有可能获得私营部门投资")
            { return 4; }
            else if ((x).Value == "已获得国家/地区拨款")
            { return 5; }
            else if ((x).Value == "已获得优惠贷款")
            { return 6; }
            else { return 7; }
        }
        private static int P32(Range x)
        {
            if ((x).Value == "没有直接收入")
            {
                return 0;
            }
            else if ((x).Value == "直接收入可忽略不计")
            {
                return 1;
            }
            else if ((x).Value == "收入能够承担运营和维护成本")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P33(Range x)
        {
            if ((x).Value == "无间接收入")
            {
                return 0;
            }
            else if ((x).Value == "较少（当地税基至多增加4％）")
            {
                return 1;
            }
            else if ((x).Value == "中度（当地税基增加5-7％）")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P34(Range x)
        {
            if ((x).Value == " 增加现有成本")
            {
                return 0;
            }
            else if ((x).Value == "无成本节约")
            {
                return 1;
            }
            else if ((x).Value == "长期节约成本")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P35(Range x)
        {
            if ((x).Value == "将征收<60％的费用")
            {
                return 0;
            }
            else if ((x).Value == "将征收60-90％的费用")
            {
                return 1;
            }
            else if ((x).Value == "将征收>90％的费用")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P36(Range x)
        {
            if ((x).Value == "建设施工和运营维护需要外部专业知识")
            {
                return 0;
            }
            else if ((x).Value == "仅建设施工阶段需要外部专业知识")
            {
                return 1;
            }
            else if ((x).Value == "筹备阶段（即可行性研究）需要外部专业知识")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P37(Range x)
        {
            if ((x).Value == "高风险")
            {
                return 0;
            }
            else if ((x).Value == "中等风险")
            {
                return 1;
            }
            else if ((x).Value == "低风险")
            {
                return 2;
            }
            else { return 3; }
        }
        private static int P39(Range x)
        {
            if ((x).Value == "尚未制定策略")
            {
                return 0;
            }
            else if ((x).Value == "正在制定策略")
            {
                return 1;
            }
            else if ((x).Value == "是的，已制定策略")
            {
                return 2;
            }
            else { return 3; }
        }

        public static double GetNumber(string str)
        {
            // Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex("^[0-9]+(\\.[0-9]+)");
            Match match = regex.Match(str);

            return double.Parse(match.Value);
        }
    }
}
