using System.Threading.Tasks;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Services.Interfaces
{
    public interface IUserStockService
    {
        Task<UserPrice> AddUserPrice(AddUserPriceDto price);
    }
}