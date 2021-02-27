using System;
#pragma warning disable 8618

namespace FinanceMonitor.DAL.Models
{
    public record UserPrice
    {
        public string UserId { get; init; }
        public string  StockSymbol { get; init; }
        public double Price { get; init; }
        public int Count { get; init; }
        public DateTime DateTime { get; init; }
    }
}