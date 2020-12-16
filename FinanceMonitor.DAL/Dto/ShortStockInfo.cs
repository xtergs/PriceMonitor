using System;

namespace FinanceMonitor.DAL.Dto
{
    public class ShortStockInfo
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public DateTime? LastDailyRecord { get; set; }
    }
}