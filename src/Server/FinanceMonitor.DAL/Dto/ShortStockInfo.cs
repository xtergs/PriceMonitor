using System;
#pragma warning disable 8618

namespace FinanceMonitor.DAL.Dto
{
    public record ShortStockInfo
    {
        public Guid Id { get; init; }
        public string Symbol { get; init; }
        public DateTime? LastDailyRecord { get; init; }
    }
}