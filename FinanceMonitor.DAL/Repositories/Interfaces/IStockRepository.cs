using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task AddStock();
        Task<Stock> GetStock(string symbol);
        Task<Stock> CreateStock(Stock stock);
        Task<UserPrice> AddUserPrice(UserPrice price);
        Task<ICollection<UserPrice>> GetUserStockPrices(Guid userId, Guid stockId);
        Task<ICollection<UserSock>> GetUserStocks(Guid userId);
        Task<ICollection<Stock>> GetStocksWithoutHistory();
        Task InsertHistory(ICollection<PriceHistory> history);
        Task AddDailyPrice(PriceDaily price);
        Task<ICollection<ShortStockInfo>> GetStocks();
    }
}