using System;

namespace FinanceMonitor.DAL.Dto
{
    public class StockHistory
    {
        public Guid Id { get; set; }
        public Guid StockId { get; set; }

        public double RegularPrice { get; set; }
        public double YearAverage { get; set; }
    }
}