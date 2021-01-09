using System;

namespace FinanceMonitor.DAL.Models
{
    public class PriceDaily
    {
        public Guid Id { get; set; }
        public Guid StockId { get; set; }
        public double? Ask { get; set; }
        public double? Bid { get; set; }
        public double AskSize { get; set; }
        public double BidSize { get; set; }
        public double Volume { get; set; }
        public double Price { get; set; }
        public DateTime Time { get; set; }
    }
}