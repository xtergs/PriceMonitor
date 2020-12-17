using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Services.Interfaces
{
    public interface IYahooApiService
    {
        Task<ApiStock> GetStock(string symbol);
        Task<ICollection<ApiHistory>> GetFullHistory(string symbol);
        Task<ApiDailyStock> GetDailyStock(string symbol);
    }
}