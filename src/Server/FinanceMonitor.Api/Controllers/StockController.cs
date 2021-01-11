using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Enums;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Stocks.Queries.GetSavedStocks;
using FinanceMonitor.DAL.Stocks.Queries.GetStockDaily;
using FinanceMonitor.DAL.Stocks.Queries.GetStockDetails;
using FinanceMonitor.DAL.Stocks.Queries.GetStockHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FinanceMonitor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("list")]
        public Task<ICollection<StockListItemDto>> GetStocks()
        {
            return _mediator.Send(new GetSavedStocksQuery());
        }

        [HttpGet("{symbol}")]
        public Task<Stock> GetStockDetails(string symbol)
        {
            return _mediator.Send(new GetStockDetailsQuery(symbol));
        }

        [HttpGet("{symbol}/history")]
        public Task<ICollection<PriceHistory>> GetStockHistory(string symbol, HistoryType type,
            DateTime start = default, DateTime end = default)
        {
            return _mediator.Send(new GetStockHistoryQuery(symbol, type, start, end));
        }

        [HttpGet("{symbol}/daily")]
        public Task<ICollection<PriceDaily>> GetStockDaily(string symbol, DateTime? date)
        {
            return _mediator.Send(new GetStockDailyQuery(symbol, date ?? DateTime.UtcNow));
        }
    }
}