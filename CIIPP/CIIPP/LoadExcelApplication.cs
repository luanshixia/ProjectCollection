using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Office.Interop.Excel;
using ExcelAppliction = Microsoft.Office.Interop.Excel.Application;

namespace CIIPP
{
    public static class LoadExcelApplication
    {
        public static void LoadExcelTable1(string docName)
        {
            ExcelAppliction excelApp = new ExcelAppliction();
            excelApp.Visible = false;
            excelApp.DisplayAlerts = false;
            Workbook wBook = excelApp.Workbooks.Open(docName);
            Worksheet wSheeth = wBook.Worksheets.Cast<Worksheet>().First(x => x.Index == 1) as Worksheet;
            DocumentManager.CurrentDocument.City.Country = (wSheeth.Cells[2, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.Intro = (wSheeth.Cells[3, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.Currency = CityStatistics.CurrencyEnum.ToList().IndexOf((wSheeth.Cells[4, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Multiple = CityStatistics.MultipleEnum.ToList().IndexOf((wSheeth.Cells[5, 4] as Range).Value);

            Worksheet wSheet = wBook.Worksheets.Cast<Worksheet>().First(x => x.Index == 2) as Worksheet;
            DocumentManager.CurrentDocument.City.C01 = (wSheet.Cells[2, 4] as Range).Value;
            DocumentManager.CurrentDocument.City.C02 = Convert.ToInt32((wSheet.Cells[3, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C03 = Convert.ToInt32((wSheet.Cells[4, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C04 = Convert.ToDouble((wSheet.Cells[5, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C05 = Convert.ToDateTime((wSheet.Cells[6, 4] as Range).Value);

            int year = DocumentManager.CurrentDocument.City.C02;
            DocumentManager.CurrentDocument.City.C1A[year - 3] = Convert.ToDouble((wSheet.Cells[10, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C1A[year - 2] = Convert.ToDouble((wSheet.Cells[10, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C1A[year - 1] = Convert.ToDouble((wSheet.Cells[10, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C1A[year] = Convert.ToDouble((wSheet.Cells[10, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C1B[year - 3] = Convert.ToDouble((wSheet.Cells[11, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C1B[year - 2] = Convert.ToDouble((wSheet.Cells[11, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C1B[year - 1] = Convert.ToDouble((wSheet.Cells[11, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C1B[year] = Convert.ToDouble((wSheet.Cells[11, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C1C[year - 3] = Convert.ToDouble((wSheet.Cells[12, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C1C[year - 2] = Convert.ToDouble((wSheet.Cells[12, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C1C[year - 1] = Convert.ToDouble((wSheet.Cells[12, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C1C[year] = Convert.ToDouble((wSheet.Cells[12, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C1D[year - 3] = Convert.ToDouble((wSheet.Cells[13, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C1D[year - 2] = Convert.ToDouble((wSheet.Cells[13, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C1D[year - 1] = Convert.ToDouble((wSheet.Cells[13, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C1D[year] = Convert.ToDouble((wSheet.Cells[13, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C1E[year - 3] = Convert.ToDouble((wSheet.Cells[14, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C1E[year - 2] = Convert.ToDouble((wSheet.Cells[14, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C1E[year - 1] = Convert.ToDouble((wSheet.Cells[14, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C1E[year] = Convert.ToDouble((wSheet.Cells[14, 7] as Range).Value);

            DocumentManager.CurrentDocument.City.C2A[year - 3] = Convert.ToDouble((wSheet.Cells[18, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C2A[year - 2] = Convert.ToDouble((wSheet.Cells[18, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C2A[year - 1] = Convert.ToDouble((wSheet.Cells[18, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C2A[year] = Convert.ToDouble((wSheet.Cells[18, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C2B[year - 3] = Convert.ToDouble((wSheet.Cells[19, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C2B[year - 2] = Convert.ToDouble((wSheet.Cells[19, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C2B[year - 1] = Convert.ToDouble((wSheet.Cells[19, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C2B[year] = Convert.ToDouble((wSheet.Cells[19, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C2C[year - 3] = Convert.ToDouble((wSheet.Cells[20, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C2C[year - 2] = Convert.ToDouble((wSheet.Cells[20, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C2C[year - 1] = Convert.ToDouble((wSheet.Cells[20, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C2C[year] = Convert.ToDouble((wSheet.Cells[20, 7] as Range).Value);

            DocumentManager.CurrentDocument.City.C3A[year - 3] = Convert.ToDouble((wSheet.Cells[25, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C3A[year - 2] = Convert.ToDouble((wSheet.Cells[25, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C3A[year - 1] = Convert.ToDouble((wSheet.Cells[25, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C3A[year] = Convert.ToDouble((wSheet.Cells[25, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C3B[year - 3] = Convert.ToDouble((wSheet.Cells[26, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C3B[year - 2] = Convert.ToDouble((wSheet.Cells[26, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C3B[year - 1] = Convert.ToDouble((wSheet.Cells[26, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C3B[year] = Convert.ToDouble((wSheet.Cells[26, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C3C[year - 3] = Convert.ToDouble((wSheet.Cells[27, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C3C[year - 2] = Convert.ToDouble((wSheet.Cells[27, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C3C[year - 1] = Convert.ToDouble((wSheet.Cells[27, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C3C[year] = Convert.ToDouble((wSheet.Cells[27, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C3D[year - 3] = Convert.ToDouble((wSheet.Cells[28, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C3D[year - 2] = Convert.ToDouble((wSheet.Cells[28, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C3D[year - 1] = Convert.ToDouble((wSheet.Cells[28, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C3D[year] = Convert.ToDouble((wSheet.Cells[28, 7] as Range).Value);

            DocumentManager.CurrentDocument.City.C4A[year - 3] = Convert.ToDouble((wSheet.Cells[32, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year - 2] = Convert.ToDouble((wSheet.Cells[32, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year - 1] = Convert.ToDouble((wSheet.Cells[32, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year] = Convert.ToDouble((wSheet.Cells[32, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 1] = Convert.ToDouble((wSheet.Cells[32, 8] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 2] = Convert.ToDouble((wSheet.Cells[32, 9] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 3] = Convert.ToDouble((wSheet.Cells[32, 10] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 4] = Convert.ToDouble((wSheet.Cells[32, 11] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 5] = Convert.ToDouble((wSheet.Cells[32, 12] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 6] = Convert.ToDouble((wSheet.Cells[32, 13] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 7] = Convert.ToDouble((wSheet.Cells[32, 14] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 8] = Convert.ToDouble((wSheet.Cells[32, 15] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 9] = Convert.ToDouble((wSheet.Cells[32, 16] as Range).Value);
            DocumentManager.CurrentDocument.City.C4A[year + 10] = Convert.ToDouble((wSheet.Cells[32, 17] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year - 3] = Convert.ToDouble((wSheet.Cells[33, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year - 2] = Convert.ToDouble((wSheet.Cells[33, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year - 1] = Convert.ToDouble((wSheet.Cells[33, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 0] = Convert.ToDouble((wSheet.Cells[33, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 1] = Convert.ToDouble((wSheet.Cells[33, 8] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 2] = Convert.ToDouble((wSheet.Cells[33, 9] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 3] = Convert.ToDouble((wSheet.Cells[33, 10] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 4] = Convert.ToDouble((wSheet.Cells[33, 11] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 5] = Convert.ToDouble((wSheet.Cells[33, 12] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 6] = Convert.ToDouble((wSheet.Cells[33, 13] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 7] = Convert.ToDouble((wSheet.Cells[33, 14] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 8] = Convert.ToDouble((wSheet.Cells[33, 15] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 9] = Convert.ToDouble((wSheet.Cells[33, 16] as Range).Value);
            DocumentManager.CurrentDocument.City.C4B[year + 10] = Convert.ToDouble((wSheet.Cells[33, 17] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year - 3] = Convert.ToDouble((wSheet.Cells[34, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year - 2] = Convert.ToDouble((wSheet.Cells[34, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year - 1] = Convert.ToDouble((wSheet.Cells[34, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year] = Convert.ToDouble((wSheet.Cells[34, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 1] = Convert.ToDouble((wSheet.Cells[34, 8] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 2] = Convert.ToDouble((wSheet.Cells[34, 9] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 3] = Convert.ToDouble((wSheet.Cells[34, 10] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 4] = Convert.ToDouble((wSheet.Cells[34, 11] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 5] = Convert.ToDouble((wSheet.Cells[34, 12] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 6] = Convert.ToDouble((wSheet.Cells[34, 13] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 7] = Convert.ToDouble((wSheet.Cells[34, 14] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 8] = Convert.ToDouble((wSheet.Cells[34, 15] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 9] = Convert.ToDouble((wSheet.Cells[34, 16] as Range).Value);
            DocumentManager.CurrentDocument.City.C4C[year + 10] = Convert.ToDouble((wSheet.Cells[34, 17] as Range).Value);

            DocumentManager.CurrentDocument.City.Questionnaire["CQ01"] = Convert.ToInt32((wSheet.Cells[38, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ02"] = Convert.ToInt32((wSheet.Cells[39, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ03"] = Convert.ToInt32((wSheet.Cells[40, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ04"] = Convert.ToInt32((wSheet.Cells[41, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ05"] = Convert.ToInt32((wSheet.Cells[42, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ06"] = Convert.ToInt32((wSheet.Cells[43, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.Questionnaire["CQ07"] = Convert.ToInt32((wSheet.Cells[44, 4] as Range).Value);

            Worksheet wSheet1 = wBook.Worksheets.Cast<Worksheet>().First(x => x.Index == 3) as Worksheet;
            DocumentManager.CurrentDocument.City.C9A[year - 3] = Convert.ToDouble((wSheet1.Cells[27, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year - 2] = Convert.ToDouble((wSheet1.Cells[27, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year - 1] = Convert.ToDouble((wSheet1.Cells[27, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 0] = Convert.ToDouble((wSheet1.Cells[27, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 1] = Convert.ToDouble((wSheet1.Cells[27, 8] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 2] = Convert.ToDouble((wSheet1.Cells[27, 9] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 3] = Convert.ToDouble((wSheet1.Cells[27, 10] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 4] = Convert.ToDouble((wSheet1.Cells[27, 11] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 5] = Convert.ToDouble((wSheet1.Cells[27, 12] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 6] = Convert.ToDouble((wSheet1.Cells[27, 13] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 7] = Convert.ToDouble((wSheet1.Cells[27, 14] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 8] = Convert.ToDouble((wSheet1.Cells[27, 15] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 9] = Convert.ToDouble((wSheet1.Cells[27, 16] as Range).Value);
            DocumentManager.CurrentDocument.City.C9A[year + 10] = Convert.ToDouble((wSheet1.Cells[27, 17] as Range).Value);

            DocumentManager.CurrentDocument.City.C9B[year - 3] = Convert.ToDouble((wSheet1.Cells[28, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year - 2] = Convert.ToDouble((wSheet1.Cells[28, 5] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year - 1] = Convert.ToDouble((wSheet1.Cells[28, 6] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 0] = Convert.ToDouble((wSheet1.Cells[28, 7] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 1] = Convert.ToDouble((wSheet1.Cells[28, 8] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 2] = Convert.ToDouble((wSheet1.Cells[28, 9] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 3] = Convert.ToDouble((wSheet1.Cells[28, 10] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 4] = Convert.ToDouble((wSheet1.Cells[28, 11] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 5] = Convert.ToDouble((wSheet1.Cells[28, 12] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 6] = Convert.ToDouble((wSheet1.Cells[28, 13] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 7] = Convert.ToDouble((wSheet1.Cells[28, 14] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 8] = Convert.ToDouble((wSheet1.Cells[28, 15] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 9] = Convert.ToDouble((wSheet1.Cells[28, 16] as Range).Value);
            DocumentManager.CurrentDocument.City.C9B[year + 10] = Convert.ToDouble((wSheet1.Cells[28, 17] as Range).Value);

            DocumentManager.CurrentDocument.City.C10A = Convert.ToDouble((wSheet1.Cells[32, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C10B = Convert.ToDouble((wSheet1.Cells[33, 4] as Range).Value);

            DocumentManager.CurrentDocument.City.C11A = Convert.ToDouble((wSheet1.Cells[37, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C11B = Convert.ToDouble((wSheet1.Cells[38, 4] as Range).Value);

            DocumentManager.CurrentDocument.City.C12A = Convert.ToDouble((wSheet1.Cells[42, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C12B = Convert.ToDouble((wSheet1.Cells[43, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C12C = Convert.ToDouble((wSheet1.Cells[44, 4] as Range).Value);

            DocumentManager.CurrentDocument.City.C13A = Convert.ToDouble((wSheet1.Cells[48, 4] as Range).Value);

            DocumentManager.CurrentDocument.City.C14A = Convert.ToDouble((wSheet1.Cells[52, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C14B = Convert.ToDouble((wSheet1.Cells[53, 4] as Range).Value);

            DocumentManager.CurrentDocument.City.C15A_1 = Convert.ToDouble((wSheet1.Cells[57, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15A_2 = Convert.ToDouble((wSheet1.Cells[58, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15B_1 = Convert.ToDouble((wSheet1.Cells[59, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15B_2 = Convert.ToDouble((wSheet1.Cells[60, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15C_1 = Convert.ToDouble((wSheet1.Cells[61, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15C_2 = Convert.ToDouble((wSheet1.Cells[62, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15D_1 = Convert.ToDouble((wSheet1.Cells[63, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15D_2 = Convert.ToDouble((wSheet1.Cells[64, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15E_1 = Convert.ToDouble((wSheet1.Cells[65, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C15E_2 = Convert.ToDouble((wSheet1.Cells[66, 4] as Range).Value);

            DocumentManager.CurrentDocument.City.C16A_1 = Convert.ToDouble((wSheet1.Cells[70, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C16A_2 = Convert.ToDouble((wSheet1.Cells[71, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C16B_1 = Convert.ToDouble((wSheet1.Cells[72, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C16B_2 = Convert.ToDouble((wSheet1.Cells[73, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C16C_1 = Convert.ToDouble((wSheet1.Cells[74, 4] as Range).Value);
            DocumentManager.CurrentDocument.City.C16C_2 = Convert.ToDouble((wSheet1.Cells[75, 4] as Range).Value);

            for (int i = 4; i <= wBook.Worksheets.Count; i++)
            {
                Worksheet wSheetX = wBook.Worksheets.Cast<Worksheet>().First(x => x.Index == i) as Worksheet;
                ProjectStatistics projStcs = new ProjectStatistics((wSheetX.Cells[2, 4] as Range).Value, (wSheetX.Cells[3, 4] as Range).Value);
                projStcs.P1C[0] = Convert.ToInt32((wSheetX.Cells[4, 4] as Range).Value);
                projStcs.P1D = Convert.ToInt32((wSheetX.Cells[5, 4] as Range).Value);
                projStcs.P1E = Convert.ToInt32((wSheetX.Cells[6, 4] as Range).Value);
                projStcs.P1F_1 = Convert.ToInt32((wSheetX.Cells[7, 4] as Range).Value);
                projStcs.P1F_2 = Convert.ToInt32((wSheetX.Cells[8, 4] as Range).Value);
                projStcs.PIP = Convert.ToInt32((wSheetX.Cells[9, 4] as Range).Value);

                int year1 = projStcs.P1F_1;
                projStcs.P2A[year1 + 0] = Convert.ToDouble((wSheetX.Cells[12, 4] as Range).Value);
                projStcs.P2A[year1 + 1] = Convert.ToDouble((wSheetX.Cells[12, 5] as Range).Value);
                projStcs.P2A[year1 + 2] = Convert.ToDouble((wSheetX.Cells[12, 6] as Range).Value);
                projStcs.P2A[year1 + 3] = Convert.ToDouble((wSheetX.Cells[12, 7] as Range).Value);
                projStcs.P2A[year1 + 4] = Convert.ToDouble((wSheetX.Cells[12, 8] as Range).Value);
                projStcs.P2B[year1 + 0] = Convert.ToDouble((wSheetX.Cells[13, 4] as Range).Value);
                projStcs.P2B[year1 + 1] = Convert.ToDouble((wSheetX.Cells[13, 5] as Range).Value);
                projStcs.P2B[year1 + 2] = Convert.ToDouble((wSheetX.Cells[13, 6] as Range).Value);
                projStcs.P2B[year1 + 3] = Convert.ToDouble((wSheetX.Cells[13, 7] as Range).Value);
                projStcs.P2B[year1 + 4] = Convert.ToDouble((wSheetX.Cells[13, 8] as Range).Value);
                projStcs.P2C[year1 + 0] = Convert.ToDouble((wSheetX.Cells[14, 4] as Range).Value);
                projStcs.P2C[year1 + 1] = Convert.ToDouble((wSheetX.Cells[14, 5] as Range).Value);
                projStcs.P2C[year1 + 2] = Convert.ToDouble((wSheetX.Cells[14, 6] as Range).Value);
                projStcs.P2C[year1 + 3] = Convert.ToDouble((wSheetX.Cells[14, 7] as Range).Value);
                projStcs.P2C[year1 + 4] = Convert.ToDouble((wSheetX.Cells[14, 8] as Range).Value);
                projStcs.P2D[year1 + 0] = Convert.ToDouble((wSheetX.Cells[15, 4] as Range).Value);
                projStcs.P2D[year1 + 1] = Convert.ToDouble((wSheetX.Cells[15, 5] as Range).Value);
                projStcs.P2D[year1 + 2] = Convert.ToDouble((wSheetX.Cells[15, 6] as Range).Value);
                projStcs.P2D[year1 + 3] = Convert.ToDouble((wSheetX.Cells[15, 7] as Range).Value);
                projStcs.P2D[year1 + 4] = Convert.ToDouble((wSheetX.Cells[15, 8] as Range).Value);
                projStcs.P2E[year1 + 0] = Convert.ToDouble((wSheetX.Cells[16, 4] as Range).Value);
                projStcs.P2E[year1 + 1] = Convert.ToDouble((wSheetX.Cells[16, 5] as Range).Value);
                projStcs.P2E[year1 + 2] = Convert.ToDouble((wSheetX.Cells[16, 6] as Range).Value);
                projStcs.P2E[year1 + 3] = Convert.ToDouble((wSheetX.Cells[16, 7] as Range).Value);
                projStcs.P2E[year1 + 4] = Convert.ToDouble((wSheetX.Cells[16, 8] as Range).Value);

                projStcs.P3A[year1 + 0] = Convert.ToDouble((wSheetX.Cells[21, 4] as Range).Value);
                projStcs.P3A[year1 + 1] = Convert.ToDouble((wSheetX.Cells[21, 5] as Range).Value);
                projStcs.P3A[year1 + 2] = Convert.ToDouble((wSheetX.Cells[21, 6] as Range).Value);
                projStcs.P3A[year1 + 3] = Convert.ToDouble((wSheetX.Cells[21, 7] as Range).Value);
                projStcs.P3A[year1 + 4] = Convert.ToDouble((wSheetX.Cells[21, 8] as Range).Value);
                projStcs.P3B[year1 + 0] = Convert.ToDouble((wSheetX.Cells[22, 4] as Range).Value);
                projStcs.P3B[year1 + 1] = Convert.ToDouble((wSheetX.Cells[22, 5] as Range).Value);
                projStcs.P3B[year1 + 2] = Convert.ToDouble((wSheetX.Cells[22, 6] as Range).Value);
                projStcs.P3B[year1 + 3] = Convert.ToDouble((wSheetX.Cells[22, 7] as Range).Value);
                projStcs.P3B[year1 + 4] = Convert.ToDouble((wSheetX.Cells[22, 8] as Range).Value);
                projStcs.P3C[year1 + 0] = Convert.ToDouble((wSheetX.Cells[23, 4] as Range).Value);
                projStcs.P3C[year1 + 1] = Convert.ToDouble((wSheetX.Cells[23, 5] as Range).Value);
                projStcs.P3C[year1 + 2] = Convert.ToDouble((wSheetX.Cells[23, 6] as Range).Value);
                projStcs.P3C[year1 + 3] = Convert.ToDouble((wSheetX.Cells[23, 7] as Range).Value);
                projStcs.P3C[year1 + 4] = Convert.ToDouble((wSheetX.Cells[23, 8] as Range).Value);
                projStcs.P3D[year1 + 0] = Convert.ToDouble((wSheetX.Cells[24, 4] as Range).Value);
                projStcs.P3D[year1 + 1] = Convert.ToDouble((wSheetX.Cells[24, 5] as Range).Value);
                projStcs.P3D[year1 + 2] = Convert.ToDouble((wSheetX.Cells[24, 6] as Range).Value);
                projStcs.P3D[year1 + 3] = Convert.ToDouble((wSheetX.Cells[24, 7] as Range).Value);
                projStcs.P3D[year1 + 4] = Convert.ToDouble((wSheetX.Cells[24, 8] as Range).Value);
                projStcs.P3E[year1 + 0] = Convert.ToDouble((wSheetX.Cells[25, 4] as Range).Value);
                projStcs.P3E[year1 + 1] = Convert.ToDouble((wSheetX.Cells[25, 5] as Range).Value);
                projStcs.P3E[year1 + 2] = Convert.ToDouble((wSheetX.Cells[25, 6] as Range).Value);
                projStcs.P3E[year1 + 3] = Convert.ToDouble((wSheetX.Cells[25, 7] as Range).Value);
                projStcs.P3E[year1 + 4] = Convert.ToDouble((wSheetX.Cells[25, 8] as Range).Value);

                projStcs.P4A = Convert.ToDouble((wSheetX.Cells[29, 4] as Range).Value);

                projStcs.P5B = Convert.ToDouble((wSheetX.Cells[117, 4] as Range).Value);
                projStcs.P6B = Convert.ToDouble((wSheetX.Cells[124, 4] as Range).Value);

                projStcs.P7A = Convert.ToDouble((wSheetX.Cells[130, 4] as Range).Value);
                projStcs.P7B = Convert.ToDouble((wSheetX.Cells[131, 4] as Range).Value);
                projStcs.P7C = Convert.ToDouble((wSheetX.Cells[132, 4] as Range).Value);
                projStcs.P7D = Convert.ToDouble((wSheetX.Cells[133, 4] as Range).Value);

                projStcs.P13A1 = (wSheetX.Cells[34, 3] as Range).Value;
                projStcs.P13A2 = (wSheetX.Cells[35, 3] as Range).Value;
                projStcs.P13A3 = (wSheetX.Cells[36, 3] as Range).Value;
                projStcs.P13B = (wSheetX.Cells[38, 3] as Range).Value;
                projStcs.P13C = (wSheetX.Cells[40, 3] as Range).Value;
                projStcs.P13D1 = (wSheetX.Cells[42, 3] as Range).Value;
                projStcs.P13D2 = (wSheetX.Cells[43, 3] as Range).Value;
                projStcs.P13D3 = (wSheetX.Cells[44, 3] as Range).Value;

                projStcs.P14A[year1 - 4] = Convert.ToDouble((wSheetX.Cells[48, 4] as Range).Value);
                projStcs.P14A[year1 - 3] = Convert.ToDouble((wSheetX.Cells[48, 5] as Range).Value);
                projStcs.P14A[year1 - 2] = Convert.ToDouble((wSheetX.Cells[48, 6] as Range).Value);
                projStcs.P14A[year1 - 1] = Convert.ToDouble((wSheetX.Cells[48, 7] as Range).Value);
                projStcs.P14A[year1 + 0] = Convert.ToDouble((wSheetX.Cells[48, 8] as Range).Value);
                projStcs.P14B[year1 - 4] = Convert.ToDouble((wSheetX.Cells[49, 4] as Range).Value);
                projStcs.P14B[year1 - 3] = Convert.ToDouble((wSheetX.Cells[49, 5] as Range).Value);
                projStcs.P14B[year1 - 2] = Convert.ToDouble((wSheetX.Cells[49, 6] as Range).Value);
                projStcs.P14B[year1 - 1] = Convert.ToDouble((wSheetX.Cells[49, 7] as Range).Value);
                projStcs.P14B[year1 + 0] = Convert.ToDouble((wSheetX.Cells[49, 8] as Range).Value);
                projStcs.P14C[year1 - 4] = Convert.ToDouble((wSheetX.Cells[50, 4] as Range).Value);
                projStcs.P14C[year1 - 3] = Convert.ToDouble((wSheetX.Cells[50, 5] as Range).Value);
                projStcs.P14C[year1 - 2] = Convert.ToDouble((wSheetX.Cells[50, 6] as Range).Value);
                projStcs.P14C[year1 - 1] = Convert.ToDouble((wSheetX.Cells[50, 7] as Range).Value);
                projStcs.P14C[year1 + 0] = Convert.ToDouble((wSheetX.Cells[50, 8] as Range).Value);
                projStcs.P14D[year1 - 4] = Convert.ToDouble((wSheetX.Cells[51, 4] as Range).Value);
                projStcs.P14D[year1 - 3] = Convert.ToDouble((wSheetX.Cells[51, 5] as Range).Value);
                projStcs.P14D[year1 - 2] = Convert.ToDouble((wSheetX.Cells[51, 6] as Range).Value);
                projStcs.P14D[year1 - 1] = Convert.ToDouble((wSheetX.Cells[51, 7] as Range).Value);
                projStcs.P14D[year1 + 0] = Convert.ToDouble((wSheetX.Cells[51, 8] as Range).Value);

                projStcs.P15A[year1 - 4] = Convert.ToDouble((wSheetX.Cells[56, 4] as Range).Value);
                projStcs.P15A[year1 - 3] = Convert.ToDouble((wSheetX.Cells[56, 5] as Range).Value);
                projStcs.P15A[year1 - 2] = Convert.ToDouble((wSheetX.Cells[56, 6] as Range).Value);
                projStcs.P15A[year1 - 1] = Convert.ToDouble((wSheetX.Cells[56, 7] as Range).Value);
                projStcs.P15A[year1 + 0] = Convert.ToDouble((wSheetX.Cells[56, 8] as Range).Value);
                projStcs.P15B[year1 - 4] = Convert.ToDouble((wSheetX.Cells[57, 4] as Range).Value);
                projStcs.P15B[year1 - 3] = Convert.ToDouble((wSheetX.Cells[57, 5] as Range).Value);
                projStcs.P15B[year1 - 2] = Convert.ToDouble((wSheetX.Cells[57, 6] as Range).Value);
                projStcs.P15B[year1 - 1] = Convert.ToDouble((wSheetX.Cells[57, 7] as Range).Value);
                projStcs.P15B[year1 + 0] = Convert.ToDouble((wSheetX.Cells[57, 8] as Range).Value);
                projStcs.P15C[year1 - 4] = Convert.ToDouble((wSheetX.Cells[58, 4] as Range).Value);
                projStcs.P15C[year1 - 3] = Convert.ToDouble((wSheetX.Cells[58, 5] as Range).Value);
                projStcs.P15C[year1 - 2] = Convert.ToDouble((wSheetX.Cells[58, 6] as Range).Value);
                projStcs.P15C[year1 - 1] = Convert.ToDouble((wSheetX.Cells[58, 7] as Range).Value);
                projStcs.P15C[year1 + 0] = Convert.ToDouble((wSheetX.Cells[58, 8] as Range).Value);
                projStcs.P15D[year1 - 4] = Convert.ToDouble((wSheetX.Cells[59, 4] as Range).Value);
                projStcs.P15D[year1 - 3] = Convert.ToDouble((wSheetX.Cells[59, 5] as Range).Value);
                projStcs.P15D[year1 - 2] = Convert.ToDouble((wSheetX.Cells[59, 6] as Range).Value);
                projStcs.P15D[year1 - 1] = Convert.ToDouble((wSheetX.Cells[59, 7] as Range).Value);
                projStcs.P15D[year1 + 0] = Convert.ToDouble((wSheetX.Cells[59, 8] as Range).Value);

                projStcs.P16A[year1 - 4] = Convert.ToDouble((wSheetX.Cells[64, 4] as Range).Value);
                projStcs.P16A[year1 - 3] = Convert.ToDouble((wSheetX.Cells[64, 5] as Range).Value);
                projStcs.P16A[year1 - 2] = Convert.ToDouble((wSheetX.Cells[64, 6] as Range).Value);
                projStcs.P16A[year1 - 1] = Convert.ToDouble((wSheetX.Cells[64, 7] as Range).Value);
                projStcs.P16A[year1 + 0] = Convert.ToDouble((wSheetX.Cells[64, 8] as Range).Value);
                projStcs.P16B[year1 - 4] = Convert.ToDouble((wSheetX.Cells[65, 4] as Range).Value);
                projStcs.P16B[year1 - 3] = Convert.ToDouble((wSheetX.Cells[65, 5] as Range).Value);
                projStcs.P16B[year1 - 2] = Convert.ToDouble((wSheetX.Cells[65, 6] as Range).Value);
                projStcs.P16B[year1 - 1] = Convert.ToDouble((wSheetX.Cells[65, 7] as Range).Value);
                projStcs.P16B[year1 + 0] = Convert.ToDouble((wSheetX.Cells[65, 8] as Range).Value);

                projStcs.P17A[year1 - 4] = Convert.ToDouble((wSheetX.Cells[70, 4] as Range).Value);
                projStcs.P17A[year1 - 3] = Convert.ToDouble((wSheetX.Cells[70, 5] as Range).Value);
                projStcs.P17A[year1 - 2] = Convert.ToDouble((wSheetX.Cells[70, 6] as Range).Value);
                projStcs.P17A[year1 - 1] = Convert.ToDouble((wSheetX.Cells[70, 7] as Range).Value);
                projStcs.P17A[year1 + 0] = Convert.ToDouble((wSheetX.Cells[70, 8] as Range).Value);
                projStcs.P17B[year1 - 4] = Convert.ToDouble((wSheetX.Cells[71, 4] as Range).Value);
                projStcs.P17B[year1 - 3] = Convert.ToDouble((wSheetX.Cells[71, 5] as Range).Value);
                projStcs.P17B[year1 - 2] = Convert.ToDouble((wSheetX.Cells[71, 6] as Range).Value);
                projStcs.P17B[year1 - 1] = Convert.ToDouble((wSheetX.Cells[71, 7] as Range).Value);
                projStcs.P17B[year1 + 0] = Convert.ToDouble((wSheetX.Cells[71, 8] as Range).Value);

                projStcs.Questionnaire["PQ01"] = Convert.ToInt32((wSheetX.Cells[138, 4] as Range).Value);
                projStcs.Questionnaire["PQ02"] = Convert.ToInt32((wSheetX.Cells[139, 4] as Range).Value);
                projStcs.Questionnaire["PQ03"] = Convert.ToInt32((wSheetX.Cells[140, 4] as Range).Value);
                projStcs.Questionnaire["PQ04"] = Convert.ToInt32((wSheetX.Cells[141, 4] as Range).Value);
                projStcs.Questionnaire["PQ05"] = Convert.ToInt32((wSheetX.Cells[142, 4] as Range).Value);
                projStcs.Questionnaire["PQ06"] = Convert.ToInt32((wSheetX.Cells[143, 4] as Range).Value);
                projStcs.Questionnaire["PQ07"] = Convert.ToInt32((wSheetX.Cells[145, 4] as Range).Value);
                projStcs.Questionnaire["PQ08"] = Convert.ToInt32((wSheetX.Cells[146, 4] as Range).Value);
                projStcs.Questionnaire["PQ09"] = Convert.ToInt32((wSheetX.Cells[147, 4] as Range).Value);
                projStcs.Questionnaire["PQ10"] = Convert.ToInt32((wSheetX.Cells[148, 4] as Range).Value);
                projStcs.Questionnaire["PQ11"] = Convert.ToInt32((wSheetX.Cells[149, 4] as Range).Value);
                projStcs.Questionnaire["PQ12"] = Convert.ToInt32((wSheetX.Cells[150, 4] as Range).Value);
                projStcs.Questionnaire["PQ13"] = Convert.ToInt32((wSheetX.Cells[151, 4] as Range).Value);
                projStcs.Questionnaire["PQ14"] = Convert.ToInt32((wSheetX.Cells[152, 4] as Range).Value);
                projStcs.Questionnaire["PQ15"] = Convert.ToInt32((wSheetX.Cells[154, 4] as Range).Value);
                projStcs.Questionnaire["PQ16"] = Convert.ToInt32((wSheetX.Cells[155, 4] as Range).Value);
                projStcs.Questionnaire["PQ17"] = Convert.ToInt32((wSheetX.Cells[156, 4] as Range).Value);
                projStcs.Questionnaire["PQ18"] = Convert.ToInt32((wSheetX.Cells[157, 4] as Range).Value);
                projStcs.Questionnaire["PQ19"] = Convert.ToInt32((wSheetX.Cells[158, 4] as Range).Value);
                projStcs.Questionnaire["PQ20"] = Convert.ToInt32((wSheetX.Cells[159, 4] as Range).Value);
                projStcs.Questionnaire["PQ21"] = Convert.ToInt32((wSheetX.Cells[161, 4] as Range).Value);
                projStcs.Questionnaire["PQ22"] = Convert.ToInt32((wSheetX.Cells[162, 4] as Range).Value);
                projStcs.Questionnaire["PQ23"] = Convert.ToInt32((wSheetX.Cells[163, 4] as Range).Value);
                projStcs.Questionnaire["PQ24"] = Convert.ToInt32((wSheetX.Cells[164, 4] as Range).Value);
                projStcs.Questionnaire["PQ25"] = Convert.ToInt32((wSheetX.Cells[165, 4] as Range).Value);
                projStcs.Questionnaire["PQ26"] = Convert.ToInt32((wSheetX.Cells[166, 4] as Range).Value);
                projStcs.Questionnaire["PQ27"] = Convert.ToInt32((wSheetX.Cells[167, 4] as Range).Value);
                projStcs.Questionnaire["PQ28"] = Convert.ToInt32((wSheetX.Cells[168, 4] as Range).Value);
                projStcs.Questionnaire["PQ29"] = Convert.ToInt32((wSheetX.Cells[169, 4] as Range).Value);
                projStcs.Questionnaire["PQ30"] = Convert.ToInt32((wSheetX.Cells[171, 4] as Range).Value);
                projStcs.Questionnaire["PQ31"] = Convert.ToInt32((wSheetX.Cells[172, 4] as Range).Value);
                projStcs.Questionnaire["PQ32"] = Convert.ToInt32((wSheetX.Cells[173, 4] as Range).Value);
                projStcs.Questionnaire["PQ33"] = Convert.ToInt32((wSheetX.Cells[174, 4] as Range).Value);
                projStcs.Questionnaire["PQ34"] = Convert.ToInt32((wSheetX.Cells[175, 4] as Range).Value);
                projStcs.Questionnaire["PQ35"] = Convert.ToInt32((wSheetX.Cells[176, 4] as Range).Value);
                projStcs.Questionnaire["PQ36"] = Convert.ToInt32((wSheetX.Cells[177, 4] as Range).Value);
                projStcs.Questionnaire["PQ37"] = Convert.ToInt32((wSheetX.Cells[178, 4] as Range).Value);
                projStcs.Questionnaire["PQ38"] = Convert.ToInt32((wSheetX.Cells[179, 4] as Range).Value);
                projStcs.Questionnaire["PQ39"] = Convert.ToInt32((wSheetX.Cells[180, 4] as Range).Value);
                DocumentManager.CurrentDocument.Projects.Add(projStcs);
            }
            excelApp.Quit();
        }
    }
}
