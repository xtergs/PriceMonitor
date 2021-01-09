using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Enums;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinanceMonitor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _repository;

        public StockController(IStockRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet("list")]
        public async Task<ICollection<StockListItemDto>> GetStocks()
        {
            var stocks = await _repository.GetSavedStocks();
            return stocks;
        }

        [HttpGet("{symbol}")]
        public async Task<Stock> GetStockDetails(string symbol)
        {
            var stock = await _repository.GetStock(symbol);
            return stock;
        }

        [HttpGet("{symbol}/history")]
        public async Task<ICollection<PriceHistory>> GetStockHistory(string symbol, HistoryType type,
            DateTime start = default, DateTime end = default)
        {
            var history = await _repository.GetStockHistory(symbol, type,
                start, end);

            return history;
        }

        [HttpGet("{symbol}/daily")]
        public async Task<ICollection<PriceDaily>> GetStockDaily(string symbol)
        {
            var daily = await _repository.GetStockDaily(symbol, DateTime.UtcNow);

            return daily;
        }
    }
}