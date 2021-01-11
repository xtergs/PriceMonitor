using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task AddUser(Models.UserProfile profile);
        Task<ICollection<UserStock>> GetUserStocks(string userId);
        Task<ICollection<UserPrice>> GetUserStockShares(string userId, string symbol);
        Task<UserPrice> AddUserPrice(UserPrice price);
    }
}