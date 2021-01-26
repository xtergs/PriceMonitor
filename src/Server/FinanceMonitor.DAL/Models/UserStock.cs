using System;

namespace FinanceMonitor.DAL.Models
{
    public class UserStock
    {
        public string Symbol { get; set; }
        public int Shares { get; set; }
        public double Total { get; set; }
        public double TotalProfit { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        
    }
}