using System;

namespace FinanceMonitor.DAL.Models
{
    public class PriceHistory
    {
        public Guid Id { get; set; }
        public Guid StockId { get; set; }
        public double Volume { get; set; }
        public double Opened { get; set; }
        public double Closed { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public DateTime DateTime { get; set; }
    }
}