namespace Reconciliation.Model
{
    public class HospitalReport
    {
        public string Bank { get; set; }
        public string TransactionDate { get; set; }
        public string Cashier { get; set; }
        public string ReceivedFrom { get; set; }
        public string CreditCardLastFourDigit { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptId { get; set; }
        public string CreditCard { get; set; }
    }

    public class BankReport
    {
        public string Bank { get; set; }
        public string TransactionDate { get; set; }
        public string TerminalNumber { get; set; }
        public string ReportDate { get; set; }
        public string CreditCardLastFourDigit { get; set; }
        public decimal Amount { get; set; }
        public string EmptyPlaceholder { get; set; }
        public string CreditCard { get; set; }
    }

    public class HospitalReconcileReport
    {
        public string Bank { get; set; }
        public string TransactionDate { get; set; }
        public string Cashier { get; set; }
        public string From { get; set; }
        public string CreditCardLastFourDigit { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptId { get; set; }
        public string CreditCard { get; set; }
        public string SourceReport { get; set; }
        public string ReconcileResult { get; set; }
    }

    public class BankReconcileReport
    {
        public string Bank { get; set; }
        public string TransactionDate { get; set; }
        public string ReportDate { get; set; }
        public string TerminalNumber { get; set; }
        public string CreditCardLastFourDigit { get; set; }
        public decimal Amount { get; set; }
        public string CreditCard { get; set; }
        public string SourceReport { get; set; }
        public string ReconcileResult { get; set; }
    }

    public class ReconcileReport
    {
        public string Bank { get; set; }
        public string TransactionDate { get; set; }
        public string ReportDate { get; set; }
        public string Cashier { get; set; }
        public string From { get; set; }
        public string TerminalNumber { get; set; }
        public string CreditCardLastFourDigit { get; set; }
        public decimal Amount { get; set; }
        public string ReceiptId { get; set; }
        public string CreditCard { get; set; }
        public string SourceReport { get; set; }
        public string ReconcileResult { get; set; }
    }
}
