using System;

namespace FinanceMonitor.DAL.Dto
{
    public class StockDto
    {
        public Guid Id { get; set; }
        public string Market { get; set; }
        public string Timezone { get; set; }
        public string Symbol { get; set; }
    }
}