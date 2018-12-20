using Reconciliation.Enum;
using Reconciliation.Helpers;
using Reconciliation.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Reconciliation
{
    public partial class Reconciliation : Form
    {
        #region readonly
        public static readonly ReconciliationHelper reconciliationHelper = new ReconciliationHelper();

        private static readonly BankReportHelper bankReportHelper = new BankReportHelper();
        private static readonly HospitalReportHelper hospitalReportHelper = new HospitalReportHelper();
        private static readonly ExcelGenerator excelGenerator = new ExcelGenerator();
        #endregion

        #region label readonly
        public static readonly string TECHNICAL_ISSUE_ERROR_LABEL = "Processing error occurred at {0}";
        public static readonly string SELECTED_PATH_ERROR_LABEL = "Please select a folder path";
        public static readonly string RECONCILE_STANDARD_LABEL = "Reconcile";
        public static readonly string RECONCILE_PROCESSING_LABEL = "Please wait..";
        public static readonly string PROCESSING_MESSAGE_LABEL = "Processing {0}..";
        public static readonly string PROCESSING_RECONCILE_MESSAGE_LABEL = "Reconciling..";
        public static readonly string PROCESSING_GENERATE_REPORT_MESSAGE_LABEL = "Generating Report..";
        public static readonly string PROCESSING_COMPLETED_MESSAGE_LABEL = "Reconciliation completed..";
        #endregion

        #region file name
        public static readonly string RECEIPT_FILENAME = "Receipts.xlsx";
        public static readonly string LIST_OF_RECEIPTS_REPORT_FILENAME = "ListOfReceiptsReport.csv";
        public static readonly string AMEX = "AMEX";
        public static readonly string MBB = "MBB";
        public static readonly string ALIPAY = "ALIPAY";
        public static readonly string HSBC = "HSBC";
        #endregion

        public Reconciliation()
        {
            InitializeComponent();
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            selectedPathLabel.Text = string.Empty;
            selectedPathLabel.Visible = false;
            selectFolderButton.Visible = true;

            processingLabel(string.Empty, false);
            ErrorLabel(string.Empty, false);
        }

        private void reconcileButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedPathLabel.Text) && Directory.Exists(selectedPathLabel.Text))
            {
                reconcileButton.Text = RECONCILE_PROCESSING_LABEL;
                reconcileButton.Enabled = false;
                resetButton.Enabled = false;

                #region processing hospital report
                List<HospitalReport> hospitalReports = new List<HospitalReport>();

                var receiptsPath = string.Format(@"{0}\{1}", selectedPathLabel.Text, RECEIPT_FILENAME);
                if (!string.IsNullOrEmpty(receiptsPath) && File.Exists(receiptsPath))
                {
                    processingLabel(string.Format(PROCESSING_MESSAGE_LABEL, RECEIPT_FILENAME), true);
                    try
                    {
                        hospitalReportHelper.ReadReceiptsExcel(receiptsPath, hospitalReports);
                    }
                    catch
                    {
                        ErrorLabel(string.Format(TECHNICAL_ISSUE_ERROR_LABEL, RECEIPT_FILENAME), true);
                        processingLabel(string.Empty, false);
                    }
                }
                else
                {
                    ErrorLabel(string.Format("{0} is not found in the selected folder", RECEIPT_FILENAME), true);
                }

                var listOfReceiptsPath = string.Format(@"{0}\{1}", selectedPathLabel.Text, LIST_OF_RECEIPTS_REPORT_FILENAME);
                if (!string.IsNullOrEmpty(listOfReceiptsPath) && File.Exists(listOfReceiptsPath))
                {
                    processingLabel(string.Format(PROCESSING_MESSAGE_LABEL, LIST_OF_RECEIPTS_REPORT_FILENAME), true);

                    try
                    {
                        hospitalReportHelper.ReadListOfReceiptReport(listOfReceiptsPath, hospitalReports);
                    }
                    catch
                    {
                        ErrorLabel(string.Format(TECHNICAL_ISSUE_ERROR_LABEL, LIST_OF_RECEIPTS_REPORT_FILENAME), true);
                        processingLabel(string.Empty, false);
                    }
                }
                #endregion

                #region processing bank report
                List<BankReport> bankReports = new List<BankReport>();

                string[] fileEntries = Directory.GetFiles(selectedPathLabel.Text);
                foreach (string filePath in fileEntries)
                {
                    if (!filePath.Contains(RECEIPT_FILENAME) && !filePath.Contains(LIST_OF_RECEIPTS_REPORT_FILENAME) && !filePath.Contains("~"))
                    {
                        if (filePath.ToUpper().Contains(AMEX))
                        {
                            processingLabel(string.Format(PROCESSING_MESSAGE_LABEL, AMEX), true);
                            bankReportHelper.ReadCSV(filePath, FileType.Amex, bankReports);
                        }

                        if (filePath.ToUpper().Contains(MBB))
                        {
                            processingLabel(string.Format(PROCESSING_MESSAGE_LABEL, MBB), true);
                            bankReportHelper.ReadCSV(filePath, FileType.MBB, bankReports);
                        }

                        if (filePath.ToUpper().Contains(ALIPAY))
                        {
                            processingLabel(string.Format(PROCESSING_MESSAGE_LABEL, ALIPAY), true);
                            bankReportHelper.ReadCSV(filePath, FileType.ALIPAY, bankReports);
                        }

                        if (filePath.ToUpper().Contains(HSBC))
                        {
                            processingLabel(string.Format(PROCESSING_MESSAGE_LABEL, Path.GetFileName(filePath)), true);
                            bankReportHelper.ReadExcel(filePath, FileType.HSBC, bankReports);
                        }
                    }
                }
                #endregion

                #region reconciliation
                List<ReconcileReport> mergedData = new List<ReconcileReport>();
                List<HospitalReconcileReport> hospitalReportReconcile = new List<HospitalReconcileReport>();
                List<BankReconcileReport> bankReportReconcile = new List<BankReconcileReport>();

                processingLabel(PROCESSING_RECONCILE_MESSAGE_LABEL, true);
                reconciliationHelper.MergingData(hospitalReports, bankReports, mergedData);
                reconciliationHelper.Reconcile(mergedData, hospitalReportReconcile, bankReportReconcile);
                #endregion

                #region check mismatch report
                List<HospitalReport> hospitalReportsForMismatch = new List<HospitalReport>();
                List<HospitalReport> hospitalMisMatchReport = new List<HospitalReport>();

                hospitalReportHelper.RevertHospitalReports(hospitalReports, hospitalReportsForMismatch);
                reconciliationHelper.CheckForMismatch(hospitalReportsForMismatch, bankReports, hospitalMisMatchReport);
                #endregion

                #region generating report
                processingLabel(PROCESSING_GENERATE_REPORT_MESSAGE_LABEL, true);
                excelGenerator.GenerateReport(selectedPathLabel.Text, hospitalReports, bankReports, hospitalReportReconcile, bankReportReconcile, hospitalMisMatchReport);
                #endregion

                processingLabel(PROCESSING_COMPLETED_MESSAGE_LABEL, true);
                reconcileButton.Text = RECONCILE_STANDARD_LABEL;
                reconcileButton.Enabled = true;
                resetButton.Enabled = true;
            }
            else
            {
                ErrorLabel(SELECTED_PATH_ERROR_LABEL, true);
            }
        }

        private void selectFolderButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result.Equals(DialogResult.OK) && !string.IsNullOrEmpty(fbd.SelectedPath))
                {
                    selectFolderButton.Visible = false;
                    selectedPathLabel.Text = fbd.SelectedPath;
                    selectedPathLabel.Visible = true;

                    processingLabel(string.Empty, false);
                    ErrorLabel(string.Empty, false);
                }
            }
        }

        public void processingLabel(string label, bool visibility)
        {
            processingMessageLabel.Text = label;
            processingMessageLabel.Visible = visibility;
        }

        public void ErrorLabel(string label, bool visibility)
        {
            errorLabel.Text = label;
            errorLabel.Visible = visibility;
        }
    }
}
