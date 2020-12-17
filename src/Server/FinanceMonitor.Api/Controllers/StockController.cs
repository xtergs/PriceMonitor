using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<ICollection<Stock>> GetStocks()
        {
            var stocks = await _repository.GetSavedStocks();
            return stocks;
        }

        [HttpGet("{stockId:Guid}/history")]
        public async Task<ICollection<PriceHistory>> GetStockHistory(Guid stockId)
        {
            var history = await _repository.GetStockHistory(stockId);

            return history;
        }

        [HttpGet("{stockId:Guid}/daily")]
        public async Task<ICollection<PriceDaily>> GetStockDaily(Guid stockId)
        {
            var daily = await _repository.GetStockDaily(stockId);

            return daily;
        }
        
    }
}