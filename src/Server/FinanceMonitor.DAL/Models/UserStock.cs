#pragma warning disable 8618


namespace FinanceMonitor.DAL.Models
{
    public sealed record UserStock
    {
        public string Symbol { get; init; }
        public int Shares { get; init; }
        public double Total { get; init; }
        public double TotalProfit { get; init; }
        public string ShortName { get; init; }
        public string LongName { get; init; }
        
    }
}