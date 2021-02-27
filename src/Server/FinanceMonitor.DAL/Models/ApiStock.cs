using System;
#pragma warning disable 8618

namespace FinanceMonitor.DAL.Models
{
    public sealed record ApiStock
    {
        public string Symbol { get; init; }
        public string Market { get; init; }
        public string Timezone { get; init; }
        public DateTime Time { get; init; }
        public string ShortName { get; init; }
        public string LongName { get; init; }
        public string Language { get; init; }
        public string Currency { get; init; }
        public string FinancialCurrency { get; set; }
        public string QuoteType { get; init; }
    }
}