using Microsoft.VisualBasic.FileIO;
using Reconciliation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reconciliation.Helpers
{
    public class HospitalReportHelper
    {
        #region common readonly
        private static readonly int SHEETS = 1;
        public static readonly Utilities util = new Utilities();
        #endregion

        #region account number readonly
        private static readonly string ACCOUNT_NUMBER_AMEX = "2001.173651";
        private static readonly string ACCOUNT_NUMBER_MAYBANK = "2001.173653";
        private static readonly string ACCOUNT_NUMBER_HSBC = "2001.173654";
        private static readonly string ACCOUNT_NUMBER_ALIPAY = "2001.176535";
        #endregion

        #region hospital format readonly
        private static readonly int RECEIPTS_RECEIPT_ID = 13;
        private static readonly int RECEIPTS_RECEIPTS_NUMBER = 14;
        private static readonly int RECEIPTS_STARTING_ROW = 18;
        private static readonly int RECEIPTS_AMOUNT = 20;
        private static readonly int RECEIPTS_AMOUNT2 = 21;
        private static readonly int RECEIPTS_ACCOUNT_NUMBER = 25;
        private static readonly int RECEIPTS_CREDITCARD_NUMBER = 26;
        #endregion

        #region list of receipts format
        private static readonly int LIST_OF_RECEIPTS_REPORT_RECEIPT_NUMBER_COLUMN = 4;
        private static readonly int LIST_OF_RECEIPTS_RECEIVED_FROM_NUMBER_COLUMN = 7;
        private static readonly int LIST_OF_RECEIPTS_CASHIER_NUMBER_COLUMN = 8;
        #endregion

        public void ReadReceiptsExcel(string receiptsPath, List<HospitalReport> hospitalReports)
        {
            #region Initialization of excel interop
            Excel.Application excelApplication = new Excel.Application();
            excelApplication.Visible = false;
            excelApplication.ScreenUpdating = false;
            excelApplication.DisplayAlerts = false;

            Excel.Workbook excelWorkbook = excelApplication.Workbooks.Open(receiptsPath);
            Excel.Worksheet excelWorksheet = excelWorkbook.Sheets[SHEETS];
            Excel.Range excelRange = excelWorksheet.UsedRange;
            #endregion

            int rowCount = excelRange.Rows.Count;
            int colCount = excelRange.Columns.Count;

            for (int i = RECEIPTS_STARTING_ROW; i <= rowCount; i++)
            {
                var accountNumber = excelRange.Cells[i, RECEIPTS_ACCOUNT_NUMBER].Value2;

                if (string.IsNullOrEmpty(accountNumber))
                    continue;

                if (!accountNumber.Equals(ACCOUNT_NUMBER_AMEX) && !accountNumber.Equals(ACCOUNT_NUMBER_MAYBANK) && !accountNumber.Equals(ACCOUNT_NUMBER_HSBC) && !accountNumber.Equals(ACCOUNT_NUMBER_ALIPAY))
                    continue;

                HospitalReport hospitalReport = new HospitalReport();

                #region bank data
                var bank = util.AccountNumberToBank(accountNumber);

                if (string.IsNullOrEmpty(bank))
                {
                    hospitalReport.Bank = string.Empty;
                }else
                {
                    hospitalReport.Bank = bank;
                }
                #endregion

                #region receipt date data
                var transactionDate = excelRange.Cells[i, RECEIPTS_RECEIPTS_NUMBER].Value2;
                if (transactionDate <= 0)
                {
                    hospitalReport.TransactionDate = string.Empty;
                }
                else
                {
                    var hospitalTransactionDate = DateTime.FromOADate(transactionDate);
                    hospitalReport.TransactionDate = hospitalTransactionDate.ToShortDateString();
                }
                #endregion

                #region credit card data
                var creditCard = excelRange.Cells[i, RECEIPTS_CREDITCARD_NUMBER].Value2;

                if (string.IsNullOrEmpty(creditCard))
                {
                    hospitalReport.CreditCard = string.Empty;
                    hospitalReport.CreditCardLastFourDigit = string.Empty;
                }
                else
                {
                    hospitalReport.CreditCard = creditCard.Trim();
                    hospitalReport.CreditCardLastFourDigit = util.GetLastFour(hospitalReport.CreditCard);
                }
                #endregion

                #region amount
                var amount = excelRange.Cells[i, RECEIPTS_AMOUNT].Value2;
                var amount2 = excelRange.Cells[i, RECEIPTS_AMOUNT2].Value2;

                if (amount == null || amount2 == null)
                {
                    hospitalReport.Amount = 0;
                }
                else
                {
                    hospitalReport.Amount = (decimal)((amount + amount2) * (-1));
                }
                #endregion

                #region receipt id
                var receiptId = excelRange.Cells[i, RECEIPTS_RECEIPT_ID].Value2;
                if (string.IsNullOrEmpty(receiptId))
                {
                    hospitalReport.ReceiptId = string.Empty;
                }
                else
                {
                    hospitalReport.ReceiptId = receiptId;
                }
                #endregion

                hospitalReports.Add(hospitalReport);
            }

            #region Garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            #endregion

            #region Dispose of excel interop
            Marshal.ReleaseComObject(excelRange);
            Marshal.ReleaseComObject(excelWorksheet);

            excelWorkbook.Close();
            Marshal.ReleaseComObject(excelWorkbook);

            excelApplication.Visible = true;
            excelApplication.ScreenUpdating = true;
            excelApplication.DisplayAlerts = true;

            excelApplication.Quit();
            Marshal.ReleaseComObject(excelApplication);
            #endregion
        }

        public void ReadListOfReceiptReport(string path, List<HospitalReport> hospitalReports)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                csvParser.TextFieldType = FieldType.Delimited;
                csvParser.SetDelimiters(",");

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    var receiptNumber = fields[LIST_OF_RECEIPTS_REPORT_RECEIPT_NUMBER_COLUMN];

                    if (!string.IsNullOrEmpty(receiptNumber))
                    {
                        var matchedDatas = hospitalReports.Where(item => item.ReceiptId.Equals(receiptNumber));
                        if (matchedDatas.Any())
                        {
                            var count = 0;

                            foreach (var field in fields)
                            {
                                #region received from
                                if (count.Equals(LIST_OF_RECEIPTS_RECEIVED_FROM_NUMBER_COLUMN) && !string.IsNullOrEmpty(field))
                                {
                                    foreach (var matchData in matchedDatas)
                                    {
                                        matchData.ReceivedFrom = field;
                                    }
                                }
                                #endregion

                                #region cashier
                                if (count.Equals(LIST_OF_RECEIPTS_CASHIER_NUMBER_COLUMN) && !string.IsNullOrEmpty(field))
                                {
                                    foreach (var matchData in matchedDatas)
                                    {
                                        matchData.Cashier = field;
                                    }
                                }
                                #endregion

                                count++;
                            }
                        }
                    }
                }
            }
        }

        public void RevertHospitalReports(List<HospitalReport> hospitalReports, List<HospitalReport> hospitalReportsForMismatch)
        {
            foreach(var hr in hospitalReports)
            {
                HospitalReport hospitalReport = new HospitalReport();

                hospitalReport.Bank = hr.Bank;
                hospitalReport.TransactionDate = hr.TransactionDate;
                hospitalReport.CreditCard = hr.CreditCard;
                hospitalReport.CreditCardLastFourDigit = hr.CreditCardLastFourDigit;
                hospitalReport.Amount = (hr.Amount * -1);
                hospitalReport.ReceiptId = hr.ReceiptId;
                hospitalReport.ReceivedFrom = hr.ReceivedFrom;
                hospitalReport.Cashier = hr.Cashier;
                hospitalReportsForMismatch.Add(hospitalReport);
            }
        }
    }
}
