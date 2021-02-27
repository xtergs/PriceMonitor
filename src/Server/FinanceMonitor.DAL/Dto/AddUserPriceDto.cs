using System;
#pragma warning disable 8618


namespace FinanceMonitor.DAL.Dto
{
    public sealed record AddUserPriceDto
    {
        public string UserId { get; init; }
        public string Symbol { get; init; }
        public double Price { get; init; }
        public int Count { get; init; }
        public DateTime DateTime { get; init; }
    }
}