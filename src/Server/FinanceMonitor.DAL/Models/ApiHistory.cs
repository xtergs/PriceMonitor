using System;
#pragma warning disable 8618

namespace FinanceMonitor.DAL.Models
{
    public record ApiHistory
    {
        public double Volume { get; init; }
        public double Low { get; init; }
        public double High { get; init; }
        public double Open { get; init; }
        public double Close { get; init; }
        public DateTime DateTime { get; init; }
    }
}