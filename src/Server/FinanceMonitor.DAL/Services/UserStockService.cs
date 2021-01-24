using System;
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
        private readonly IStockRepository _stockRepository;
        private readonly IUserRepository _userRepository;

        public UserStockService(IStockRepository repository,
            IYahooApiService apiService,
            IUserRepository userRepository)
        {
            _stockRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserPrice> AddUserPrice(AddUserPriceDto price)
        {
            var apiResult = await _apiService.GetStock(price.Symbol);
            if (apiResult == null) throw new NotFoundException("Symbol is not found");

            var existingStock = await _stockRepository.GetStock(price.Symbol);
            if (existingStock == null)
                existingStock = await _stockRepository.CreateStock(new Stock
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

            var addedPricing = await _userRepository.AddUserPrice(new UserPrice
            {
                StockSymbol = existingStock.Symbol,
                UserId = price.UserId,
                Price = price.Price,
                Count = price.Count,
                DateTime = price.DateTime
            });

            return addedPricing;
        }
    }
}