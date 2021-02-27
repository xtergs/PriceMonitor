using System;
#pragma warning disable 8618


namespace FinanceMonitor.DAL.Models
{
    public sealed record ApiDailyStock
    {
        public string Symbol { get; init; }
        public double? Ask { get; init; }
        public double? Bid { get; init; }
        public double AskSize { get; init; }
        public double BidSize { get; init; }
        public string MarketState { get; init; }
        public double Volume { get; init; }
        public double Price { get; init; }
        public DateTime Time { get; init; }
    }
}