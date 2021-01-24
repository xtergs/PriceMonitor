using System;

namespace FinanceMonitor.DAL.Models
{
    public class UserPrice
    {
        public string UserId { get; set; }
        public string  StockSymbol { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
    }
}