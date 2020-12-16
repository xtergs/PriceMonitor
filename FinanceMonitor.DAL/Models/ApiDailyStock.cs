using System;

namespace FinanceMonitor.DAL.Models
{
    public class ApiDailyStock
    {
        public string Symbol { get; set; }
        public double Ask { get; set; }
        public double Bid { get; set; }
        public double AskSize { get; set; }
        public double BidSize { get; set; }
        public string MarketState { get; set; }
        public DateTime Time { get; set; }
    }
}