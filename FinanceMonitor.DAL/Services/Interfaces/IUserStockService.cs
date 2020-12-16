using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Services.Interfaces
{
    public interface IUserStockService
    {
        Task<UserPrice> AddUserPrice(AddUserPriceDto price);
        Task<IReadOnlyCollection<UserPrice>> GetUserStockPrices(Guid userId, Guid stockId);
        Task<IReadOnlyCollection<UserSock>> GetUserStocks(Guid userId);
    }
}