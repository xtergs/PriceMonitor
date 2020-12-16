using System;

namespace FinanceMonitor.DAL.Models
{
    public class UserSock
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public int Count { get; set; }
    }
}