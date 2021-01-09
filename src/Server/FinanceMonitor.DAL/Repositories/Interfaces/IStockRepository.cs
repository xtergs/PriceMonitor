using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Enums;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Repositories.Interfaces
{
    public interface IStockRepository
    {
        Task<Stock> GetStock(string symbol);
        Task<Stock> CreateStock(Stock stock);
        Task<UserPrice> AddUserPrice(UserPrice price);
        Task<ICollection<UserPrice>> GetUserStockPrices(string userId, string symbol);
        Task<ICollection<UserStock>> GetUserStocks(string userId);
        Task<ICollection<Stock>> GetStocksWithoutHistory();
        Task InsertHistory(ICollection<PriceHistory> history);
        Task AddDailyPrice(PriceDaily price);
        Task<ICollection<ShortStockInfo>> GetStocks();
        Task<ICollection<StockListItemDto>> GetSavedStocks();
        Task<ICollection<PriceHistory>> GetStockHistory(string symbol, HistoryType type, DateTime start, DateTime end);
        Task<ICollection<PriceDaily>> GetStockDaily(string symbol, DateTime date);
    }
}