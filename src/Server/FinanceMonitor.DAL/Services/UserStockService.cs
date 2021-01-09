using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Exceptions;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;

namespace FinanceMonitor.DAL.Services
{
    public class UserStockService : IUserStockService
    {
        private readonly IYahooApiService _apiService;
        private readonly IStockRepository _repository;

        public UserStockService(IStockRepository repository,
            IYahooApiService apiService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task<UserPrice> AddUserPrice(AddUserPriceDto price)
        {
            var apiResult = await _apiService.GetStock(price.Symbol);
            if (apiResult == null) throw new NotFoundException("Symbol is not found");

            var existingStock = await _repository.GetStock(price.Symbol);
            if (existingStock == null)
                existingStock = await _repository.CreateStock(new Stock
                {
                    Symbol = price.Symbol,
                    Market = apiResult.Market,
                    Time = apiResult.Time,
                    Timezone = apiResult.Timezone,
                    LongName = apiResult.LongName,
                    ShortName = apiResult.ShortName,
                    Currency = apiResult.Currency,
                    FinancialCurrency = apiResult.FinancialCurrency,
                    Language = apiResult.Language,
                    QuoteType = apiResult.QuoteType
                });

            var addedPricing = await _repository.AddUserPrice(new UserPrice
            {
                StockId = existingStock.Id,
                UserId = price.UserId,
                Price = price.Price,
                Count = price.Count,
                DateTime = price.DateTime
            });

            return addedPricing;
        }

        public async Task<IReadOnlyCollection<UserPrice>> GetUserStockPrices(string userId, string symbol)
        {
            return (await _repository.GetUserStockPrices(userId, symbol)).ToArray();
        }

        public async Task<IReadOnlyCollection<UserStock>> GetUserStocks(string userId)
        {
            return (await _repository.GetUserStocks(userId)).ToArray();
        }
    }
}