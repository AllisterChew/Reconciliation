using Microsoft.VisualBasic.FileIO;
using Reconciliation.Enum;
using Reconciliation.Model;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace Reconciliation.Helpers
{
    public class BankReportHelper
    {
        #region common readonly
        private static readonly int SHEETS = 1;
        public static readonly Utilities util = new Utilities();
        #endregion

        #region maybank bank format
        private static readonly int MAYBANK__HEADER_CHECK = 0;

        private static readonly int MAYBANK_FORMAT_CREDIT_CARD_COLUMN = 0;
        private static readonly int MAYBANK_FORMAT_REPORT_DATE_COLUMN = 1;
        private static readonly int MAYBANK_FORMAT_AMOUNT_COLUMN = 2;
        private static readonly int MAYBANK_FORMAT_TRANSACTION_DATE_COLUMN = 4;
        private static readonly int MAYBANK_FORMAT_AUTHCODE_COLUMN = 6;
        private static readonly int MAYBANK_FORMAT_TRANSACTION_ID_COLUMN = 8;
        private static readonly int MAYBANK_FORMAT_TERMINAL_NUMBER_COLUMN = 12;
        private static readonly int MAYBANK_FORMAT_REPORT_COUNT = 19;
        private static readonly int MAYBANK_FORMAT_COUNT = 22;

        private static readonly string MAYBANK_HEADER_CHECK_VALUE = "Card Number";
        private static readonly string MAYBANK_HEADER_CHECK_VALUE_2 = "TOTAL CREDIT AMOUNT";
        private static readonly string MAYBANK_HEADER_CHECK_VALUE_3 = "TOTAL DEBIT AMOUNT";
        private static readonly string MAYBANK_HEADER_CHECK_VALUE_4 = "Total Amount";
        private static readonly string MAYBANK_HEADER_CHECK_VALUE_5 = "Report Date  :";
        #endregion

        #region hsbc bank format readonly
        public static readonly int HSBC_CREDIT_CARD_COLUMN = 1;
        public static readonly int HSBC_HEADER_COLUMN_CHECK = 1;
        public static readonly int HSBC_TRANSACTION_DATE_COLUMN = 2;
        public static readonly int HSBC_AMOUNT_COLUMN = 4;
        public static readonly int HSBC_AUTH_CODE_COLUMN = 20;

        public static readonly string HSBC_HEADER_CHECK = "Card Number";
        public static readonly string HSBC_HEADER_CHECK2 = "Processing Day";
        public static readonly string HSBC_FOOTER_CHECK = "calculations";
        #endregion

        public void ReadCSV(string path, FileType fileType, List<BankReport> bankReport)
        {
            if (fileType.Equals(FileType.Amex) || fileType.Equals(FileType.MBB) || fileType.Equals(FileType.ALIPAY))
                MaybankFileFormat(path, fileType, bankReport);
        }

        public void ReadExcel(string path, FileType fileType, List<BankReport> bankReport)
        {
            if (fileType.Equals(FileType.HSBC))
                HSBCFileFormat(path, fileType, bankReport);
        }

        public void MaybankFileFormat(string path, FileType fileType, List<BankReport> bankReport)
        {
            using (TextFieldParser csvParser = new TextFieldParser(path))
            {
                string reportDate = string.Empty;
                csvParser.TextFieldType = FieldType.Delimited;
                csvParser.SetDelimiters(",");

                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();

                    if(fields[0].Contains(MAYBANK_HEADER_CHECK_VALUE_5))
                    {
                        reportDate = fields[MAYBANK_FORMAT_REPORT_DATE_COLUMN];
                    }

                    if (fields[0].Contains("******"))
                    {
                        var count = 0;
                        BankReport br = new BankReport();

                        foreach (var field in fields)
                        {
                            #region credit card
                            if (count.Equals(MAYBANK_FORMAT_CREDIT_CARD_COLUMN) && !string.IsNullOrEmpty(field))
                            {
                                br.CreditCard = field.Trim();
                                br.CreditCardLastFourDigit = util.GetLastFour(br.CreditCard);
                            }
                            #endregion

                            #region amount
                            if (count.Equals(MAYBANK_FORMAT_AMOUNT_COLUMN) && !string.IsNullOrEmpty(field))
                            {
                                decimal amount;
                                if (decimal.TryParse(field, out amount))
                                    br.Amount = amount;
                                else
                                    continue;
                            }
                            #endregion

                            #region transaction id
                            if (count.Equals(MAYBANK_FORMAT_TRANSACTION_ID_COLUMN) && !string.IsNullOrEmpty(field))
                            {
                                if (field.Equals("41"))
                                {
                                    if (br.Amount > 0)
                                        br.Amount = br.Amount * (-1);
                                }
                            }
                            #endregion

                            #region authcode
                            //if (count.Equals(MAYBANK_FORMAT_AUTHCODE_COLUMN) && !string.IsNullOrEmpty(field))
                            //{
                            //    if (field[0].Equals('B'))
                            //    {
                            //        if (br.Amount > 0)
                            //            br.Amount = br.Amount * (-1);
                            //    }
                            //}
                            #endregion

                            #region transaction date
                            if (count.Equals(MAYBANK_FORMAT_TRANSACTION_DATE_COLUMN) && !string.IsNullOrEmpty(field))
                            {
                                br.TransactionDate = field;
                            }
                            #endregion

                            #region terminal number
                            if (count.Equals(MAYBANK_FORMAT_TERMINAL_NUMBER_COLUMN) && !string.IsNullOrEmpty(field))
                            {
                                br.TerminalNumber = field;
                            }
                            #endregion

                            #region report date
                            br.ReportDate = reportDate;
                            #endregion

                            #region bank
                            br.Bank = util.GetBankName(fileType);
                            #endregion

                            count++;
                        }
                        bankReport.Add(br);
                    }
                }
            }
        }

        public void HSBCFileFormat(string path, FileType type, List<BankReport> bankReport)
        {
            #region Initialization of excel interop
            Excel.Application excelApplication = new Excel.Application();
            excelApplication.Visible = false;
            excelApplication.ScreenUpdating = false;
            excelApplication.DisplayAlerts = false;

            Excel.Workbook excelWorkbook = excelApplication.Workbooks.Open(path);
            Excel.Worksheet excelWorksheet = excelWorkbook.Sheets[SHEETS];
            Excel.Range excelRange = excelWorksheet.UsedRange;
            #endregion

            string reportDate = string.Empty;
            bool keywordDetect = false;
            int rowCount = excelRange.Rows.Count;
            int colCount = excelRange.Columns.Count;

            for (int i = 1; i <= rowCount; i++)
            {
                var headerCheck = excelRange.Cells[i, HSBC_HEADER_COLUMN_CHECK].Value2;

                if (string.IsNullOrEmpty(headerCheck))
                    continue;

                if (headerCheck.Equals(HSBC_HEADER_CHECK2))
                {
                    #region report date
                    var rawReportDate = excelRange.Cells[i, HSBC_TRANSACTION_DATE_COLUMN].Value2;
                    if (rawReportDate > 0)
                    {
                        var convertedDateTime = DateTime.FromOADate(rawReportDate);
                        reportDate = convertedDateTime.ToShortDateString();
                    }
                    #endregion

                    continue;
                }

                if (headerCheck.Equals(HSBC_HEADER_CHECK))
                {
                    keywordDetect = true;
                    continue;
                }

                if (!keywordDetect)
                    continue;

                BankReport br = new BankReport();

                #region credit card
                var creditCard = excelRange.Cells[i, HSBC_CREDIT_CARD_COLUMN].Value2;
                if (!string.IsNullOrEmpty(creditCard))
                {
                    br.CreditCard = creditCard.Trim();
                    br.CreditCardLastFourDigit = util.GetLastFour(br.CreditCard);
                }
                #endregion

                #region transaction date
                var transactionDate = excelRange.Cells[i, HSBC_TRANSACTION_DATE_COLUMN].Value2;
                if (transactionDate > 0)
                {
                    DateTime datetime = DateTime.FromOADate(transactionDate);
                    br.TransactionDate = datetime.ToShortDateString();
                }
                #endregion

                #region amount
                var amount = excelRange.Cells[i, HSBC_AMOUNT_COLUMN].Value2;
                if(amount != null)
                    br.Amount = (decimal) amount;
                #endregion

                #region bank
                br.Bank = util.GetBankName(type);
                #endregion

                #region report date
                br.ReportDate = reportDate;
                #endregion 

                if (!string.IsNullOrEmpty(br.CreditCard) && !string.IsNullOrEmpty(br.TransactionDate))
                    bankReport.Add(br);
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
    }
}
