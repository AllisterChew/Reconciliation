using Reconciliation.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reconciliation.Helpers
{
    public class Utilities
    {
        public string GetLastFour(string value)
        {
            return value.Substring(Math.Max(0, value.Length - 4));
        }

        public string CsvEscape(string value)
        {
            if (!string.IsNullOrEmpty(value) && value.Contains(","))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            return value;
        }

        public string GetBankName(FileType fileType)
        {
            var bankName = string.Empty;

            switch (fileType)
            {
                case FileType.Amex:
                    bankName = "AMEX";
                    break;
                case FileType.MBB:
                    bankName = "MBB";
                    break;
                case FileType.ALIPAY:
                    bankName = "ALIPAY";
                    break;
                case FileType.HSBC:
                    bankName = "HSBC";
                    break;
            }

            return bankName;
        }

        public string AccountNumberToBank(string value)
        {
            var bankName = string.Empty;
            switch (value)
            {
                case "2001.173651":
                    bankName = "AMEX";
                    break;
                case "2001.173653":
                    bankName = "MBB";
                    break;
                case "2001.176535":
                    bankName = "ALIPAY";
                    break;
                case "2001.173654":
                    bankName = "HSBC";
                    break;
                default:
                    bankName = null;
                    break;
            }

            return bankName;
        }

        public string StringToDecimalString(string value)
        {
            decimal convertedValue;
            if (!decimal.TryParse(value, out convertedValue))
                return value;
            else
                return convertedValue.ToString("0.##");
        }
    }
}
