using System;

namespace FinanceMonitor.DAL.Models
{
    public class Stock
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public string Market { get; set; }
        public string Timezone { get; set; }
        public DateTime Time { get; set; }
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public string Language { get; set; }
        public string Currency { get; set; }
        public string FinancialCurrency { get; set; }
    }
}