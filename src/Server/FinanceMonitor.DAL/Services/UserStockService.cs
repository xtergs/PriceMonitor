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
        private readonly IStockRepository _stockRepository;
        private readonly IUserRepository _userRepository;

        public UserStockService(IStockRepository repository,
            IUserRepository userRepository)
        {
            _stockRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<UserPrice> AddUserPrice(AddUserPriceDto price)
        {
            var existingStock = await _stockRepository.GetStock(price.Symbol);
            if (existingStock == null)
                throw new NotFoundException("Symbol is not found");


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