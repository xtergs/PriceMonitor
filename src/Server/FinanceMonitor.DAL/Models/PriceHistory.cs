﻿using System;
#pragma warning disable 8618


namespace FinanceMonitor.DAL.Models
{
    public sealed record PriceHistory
    {
        public string StockSymbol { get; init; }
        public double Volume { get; init; }
        public double Opened { get; init; }
        public double Closed { get; init; }
        public double High { get; init; }
        public double Low { get; init; }
        public DateTime DateTime { get; init; }
    }
}