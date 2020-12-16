using System;

namespace FinanceMonitor.DAL.Models
{
    public class ApiStock
    {
        public string Symbol { get; set; }
        public string Market { get; set; }
        public string Timezone { get; set; }
        public DateTime Time { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public string FinancialCurrency { get; set; }
    }

    public class ApiHistory
    {
        public double Volume { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public DateTime DateTime { get; set; }
    }
}