using System;

namespace FinanceMonitor.DAL.Dto
{
    public class AddUserPriceDto
    {
        public Guid UserId { get; set; }
        public string Symbol { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public DateTime DateTime { get; set; }
    }
}