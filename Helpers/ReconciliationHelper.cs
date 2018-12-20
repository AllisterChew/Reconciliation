using Reconciliation.Model;
using System.Collections.Generic;
using System.Linq;

namespace Reconciliation.Helpers
{
    public class ReconciliationHelper
    {
        #region readonly
        public static readonly Utilities util = new Utilities();

        private static readonly string hospital = "Hospital";
        private static readonly string bank = "Bank";

        private static readonly string match = "Match";
        private static readonly string unmatch = "Unmatch";
        private static readonly string missingCreditCard = "Missing Credit Card";
        #endregion

        public void MergingData(List<HospitalReport> hospitalReports, List<BankReport> bankReports, List<ReconcileReport> MergedData)
        {
            foreach(var hospitalReport in hospitalReports)
            {
                ReconcileReport data = new ReconcileReport();
                data.Bank = hospitalReport.Bank;
                data.TransactionDate = hospitalReport.TransactionDate;
                data.Cashier = hospitalReport.Cashier;
                data.From = hospitalReport.ReceivedFrom;
                data.CreditCardLastFourDigit = hospitalReport.CreditCardLastFourDigit;
                data.Amount = hospitalReport.Amount;
                data.ReceiptId = hospitalReport.ReceiptId;
                data.CreditCard = hospitalReport.CreditCard;
                data.SourceReport = hospital;

                MergedData.Add(data);
            }

            foreach (var bankReport in bankReports)
            {
                ReconcileReport data = new ReconcileReport();
                data.Bank = bankReport.Bank;
                data.TransactionDate = bankReport.TransactionDate;
                data.ReportDate = bankReport.ReportDate;
                data.TerminalNumber = bankReport.TerminalNumber;
                data.CreditCardLastFourDigit = bankReport.CreditCardLastFourDigit;
                data.Amount = bankReport.Amount;
                data.CreditCard = bankReport.CreditCard;
                data.SourceReport = bank;

                MergedData.Add(data);
            }
        }

        public void Reconcile(List<ReconcileReport> MergedData, List<HospitalReconcileReport> hospitalReportReconcile, List<BankReconcileReport> bankReportReconcile)
        {
            #region reconciliation
            var groupByCreditCardLastFourDigit = MergedData.Where(value => !string.IsNullOrEmpty(value.CreditCardLastFourDigit)).GroupBy(g => g.CreditCardLastFourDigit).Select(s => s.ToList());

            foreach (var groupDatas in groupByCreditCardLastFourDigit)
            {
                decimal totalAmount = 0;

                foreach (var data in groupDatas)
                {
                    totalAmount = totalAmount + data.Amount;
                }

                var reconcileResult = (totalAmount >= -0.5M && totalAmount <= 0.5M) ? match : unmatch;

                foreach (var data in groupDatas)
                {
                    if (data.SourceReport.Equals(hospital))
                    {
                        HospitalReconcileReport reconcileHospitalReport = new HospitalReconcileReport();
                        reconcileHospitalReport.Bank = data.Bank;
                        reconcileHospitalReport.TransactionDate = data.TransactionDate;
                        reconcileHospitalReport.Cashier = data.Cashier;
                        reconcileHospitalReport.From = data.From;
                        reconcileHospitalReport.CreditCardLastFourDigit = data.CreditCardLastFourDigit;
                        reconcileHospitalReport.Amount = data.Amount;
                        reconcileHospitalReport.ReceiptId = data.ReceiptId;
                        reconcileHospitalReport.CreditCard = data.CreditCard;
                        reconcileHospitalReport.SourceReport = hospital;
                        reconcileHospitalReport.ReconcileResult = reconcileResult;

                        hospitalReportReconcile.Add(reconcileHospitalReport);
                    }

                    if (data.SourceReport.Equals(bank))
                    {
                        BankReconcileReport reconcileBankReport = new BankReconcileReport();
                        reconcileBankReport.Bank = data.Bank;
                        reconcileBankReport.TransactionDate = data.TransactionDate;
                        reconcileBankReport.ReportDate = data.ReportDate;
                        reconcileBankReport.TerminalNumber = data.TerminalNumber;
                        reconcileBankReport.CreditCardLastFourDigit = data.CreditCardLastFourDigit;
                        reconcileBankReport.Amount = data.Amount;
                        reconcileBankReport.CreditCard = data.CreditCard;
                        reconcileBankReport.SourceReport = bank;
                        reconcileBankReport.ReconcileResult = reconcileResult;

                        bankReportReconcile.Add(reconcileBankReport);
                    }
                }
            }
            #endregion

            #region missing credit card
            var checkMissingInformation = MergedData.Where(value => string.IsNullOrEmpty(value.CreditCardLastFourDigit)).GroupBy(g => g.CreditCardLastFourDigit).Select(s => s.ToList());

            foreach (var groupDatas in checkMissingInformation)
            {
                foreach (var data in groupDatas)
                {
                    if (data.SourceReport.Equals(hospital))
                    {
                        HospitalReconcileReport reconcileHospitalReport = new HospitalReconcileReport();
                        reconcileHospitalReport.Bank = data.Bank;
                        reconcileHospitalReport.TransactionDate = data.TransactionDate;
                        reconcileHospitalReport.Cashier = data.Cashier;
                        reconcileHospitalReport.From = data.From;
                        reconcileHospitalReport.CreditCardLastFourDigit = data.CreditCardLastFourDigit;
                        reconcileHospitalReport.Amount = data.Amount;
                        reconcileHospitalReport.ReceiptId = data.ReceiptId;
                        reconcileHospitalReport.CreditCard = data.CreditCard;
                        reconcileHospitalReport.SourceReport = hospital;
                        reconcileHospitalReport.ReconcileResult = missingCreditCard;

                        hospitalReportReconcile.Add(reconcileHospitalReport);
                    }

                    if (data.SourceReport.Equals(bank))
                    {
                        BankReconcileReport reconcileBankReport = new BankReconcileReport();
                        reconcileBankReport.Bank = data.Bank;
                        reconcileBankReport.TransactionDate = data.TransactionDate;
                        reconcileBankReport.ReportDate = data.ReportDate;
                        reconcileBankReport.TerminalNumber = data.TerminalNumber;
                        reconcileBankReport.CreditCardLastFourDigit = data.CreditCardLastFourDigit;
                        reconcileBankReport.Amount = data.Amount;
                        reconcileBankReport.CreditCard = data.CreditCard;
                        reconcileBankReport.SourceReport = bank;
                        reconcileBankReport.ReconcileResult = missingCreditCard;

                        bankReportReconcile.Add(reconcileBankReport);
                    }
                }
            }
            #endregion
        }

        public void CheckForMismatch(List<HospitalReport> hospitalReportsForMismatch, List<BankReport> bankReports, List<HospitalReport> hospitalMisMatchReport)
        {
            foreach(var hr in hospitalReportsForMismatch)
            {
                foreach(var br in bankReports)
                {
                    if(hr.CreditCardLastFourDigit.Equals(br.CreditCardLastFourDigit))
                    {
                        var difference = (hr.Amount - br.Amount);
                        if(difference >= -0.5M && difference <= 0.5M)
                        {
                            if(hr.Bank != br.Bank)
                            {
                                hospitalMisMatchReport.Add(hr);
                            }
                        }
                    }
                }
            }
        }
    }
}
