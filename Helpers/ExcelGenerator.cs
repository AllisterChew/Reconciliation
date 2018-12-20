using Reconciliation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reconciliation.Helpers
{
    public class ExcelGenerator
    {
        public void GenerateReport(string path, List<HospitalReport> hospitalReports, List<BankReport> bankReports, List<HospitalReconcileReport> hospitalReportReconcile, List<BankReconcileReport> bankReportReconcile, List<HospitalReport> hospitalMisMatchReport)
        {
            #region Initialization of excel interop
            Excel.Application excelApplication = new Excel.Application();
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;

            object misValue = System.Reflection.Missing.Value;
            xlWorkBook = excelApplication.Workbooks.Add(misValue);

            excelApplication.Visible = false;
            excelApplication.ScreenUpdating = false;
            excelApplication.DisplayAlerts = false;

            xlWorkBook.Sheets.Add();
            #endregion

            #region hospital reports
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

            var excelRow = 1;
            foreach(var report in hospitalReports)
            {
                xlWorkSheet.Cells[excelRow, 1] = report.Bank;
                xlWorkSheet.Cells[excelRow, 2] = report.TransactionDate;
                xlWorkSheet.Cells[excelRow, 3] = report.Cashier;
                xlWorkSheet.Cells[excelRow, 4] = report.ReceivedFrom;
                xlWorkSheet.Cells[excelRow, 5] = report.CreditCardLastFourDigit;
                xlWorkSheet.Cells[excelRow, 6] = report.Amount;
                xlWorkSheet.Cells[excelRow, 7] = report.ReceiptId;
                xlWorkSheet.Cells[excelRow, 8] = report.CreditCard;
                excelRow++;
            }
            xlWorkSheet.Name = "Hospital";
            #endregion

            #region bank reports
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

            excelRow = 1;
            foreach (var report in bankReports)
            {
                xlWorkSheet.Cells[excelRow, 1] = report.Bank;
                xlWorkSheet.Cells[excelRow, 2] = report.TransactionDate;
                xlWorkSheet.Cells[excelRow, 3] = report.TerminalNumber;
                xlWorkSheet.Cells[excelRow, 4] = report.ReportDate;
                xlWorkSheet.Cells[excelRow, 5] = report.CreditCardLastFourDigit;
                xlWorkSheet.Cells[excelRow, 6] = report.Amount;
                xlWorkSheet.Cells[excelRow, 7] = string.Empty;
                xlWorkSheet.Cells[excelRow, 8] = report.CreditCard;
                excelRow++;
            }
            xlWorkSheet.Name = "Bank";
            #endregion

            #region hospital reconcile report
            if(xlWorkBook.Sheets.Count < 3)
                xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);

            excelRow = 1;
            foreach (var report in hospitalReportReconcile)
            {
                xlWorkSheet.Cells[excelRow, 1] = report.Bank;
                xlWorkSheet.Cells[excelRow, 2] = report.TransactionDate;
                xlWorkSheet.Cells[excelRow, 3] = report.Cashier;
                xlWorkSheet.Cells[excelRow, 4] = report.From;
                xlWorkSheet.Cells[excelRow, 5] = report.CreditCardLastFourDigit;
                xlWorkSheet.Cells[excelRow, 6] = report.Amount;
                xlWorkSheet.Cells[excelRow, 7] = report.ReceiptId;
                xlWorkSheet.Cells[excelRow, 8] = report.CreditCard;
                xlWorkSheet.Cells[excelRow, 9] = report.ReconcileResult;
                excelRow++;
            }
            xlWorkSheet.Name = "Hospital Reconcile";
            #endregion

            #region bank reconcile report
            if (xlWorkBook.Sheets.Count < 4)
                xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);

            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(4);

            excelRow = 1;
            foreach (var report in bankReportReconcile)
            {
                xlWorkSheet.Cells[excelRow, 1] = report.Bank;
                xlWorkSheet.Cells[excelRow, 2] = report.TransactionDate;
                xlWorkSheet.Cells[excelRow, 3] = report.TerminalNumber;
                xlWorkSheet.Cells[excelRow, 4] = report.ReportDate;
                xlWorkSheet.Cells[excelRow, 5] = report.CreditCardLastFourDigit;
                xlWorkSheet.Cells[excelRow, 6] = report.Amount;
                xlWorkSheet.Cells[excelRow, 7] = string.Empty;
                xlWorkSheet.Cells[excelRow, 8] = report.CreditCard;
                xlWorkSheet.Cells[excelRow, 9] = report.ReconcileResult;
                excelRow++;
            }
            xlWorkSheet.Name = "Bank Reconcile";
            #endregion

            #region mismatch report
            if(hospitalMisMatchReport.Count > 0)
            {
                if (xlWorkBook.Sheets.Count < 5)
                    xlWorkBook.Sheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(5);

                excelRow = 1;
                foreach (var report in hospitalMisMatchReport)
                {
                    xlWorkSheet.Cells[excelRow, 1] = report.Bank;
                    xlWorkSheet.Cells[excelRow, 2] = report.TransactionDate;
                    xlWorkSheet.Cells[excelRow, 3] = report.Cashier;
                    xlWorkSheet.Cells[excelRow, 4] = report.ReceivedFrom;
                    xlWorkSheet.Cells[excelRow, 5] = report.CreditCardLastFourDigit;
                    xlWorkSheet.Cells[excelRow, 6] = report.Amount;
                    xlWorkSheet.Cells[excelRow, 7] = report.ReceiptId;
                    xlWorkSheet.Cells[excelRow, 8] = report.CreditCard;
                    excelRow++;
                }
                xlWorkSheet.Name = "Mismatch Hospital Report";
            }
            #endregion

            string filePath = Path.Combine(path, String.Format("Reconciliation_{0}.xls", DateTime.Now.ToString("yyyyMMdd")));
            xlWorkBook.SaveAs(filePath, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
            xlWorkBook.Close(true, misValue, misValue);
            excelApplication.Quit();

            #region Dispose of excel interop
            Marshal.ReleaseComObject(xlWorkSheet);
            Marshal.ReleaseComObject(xlWorkBook);
            Marshal.ReleaseComObject(excelApplication);
            #endregion
        }
    }
}
