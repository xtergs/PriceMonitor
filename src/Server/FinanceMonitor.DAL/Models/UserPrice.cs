using System;

namespace FinanceMonitor.DAL.Models
{
    public class UserPrice
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid StockId { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
    }
}