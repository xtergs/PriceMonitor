using System;
using System.Collections.Generic;
using FinanceMonitor.DAL.Models;
#pragma warning disable 8618

namespace FinanceMonitor.DAL.Stocks.Queries.GetSavedStocks
{
    public sealed record StockListItemDto
    {
        public string Symbol { get; init; }
        public string? LongName { get; init; }
        public string ShortName { get; init; }
        
        public string QuoteType { get; init; }
        public double CurrentPrice { get; init; }
        public double CurrentVolume { get; init; }
        public DateTime CurrentTime { get; init; }
        public string Status { get; init; }
        public string Currency { get; init; }
        public string? FinancialCurrency { get; init; }
        public string Market { get; init; }
        public string Timezone { get; init; }

        public IReadOnlyCollection<PriceHistory> FullHistory { get; init; }
    }
}