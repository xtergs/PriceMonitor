using System;
using System.Threading.Tasks;

namespace FinanceMonitor.DAL.Repositories.Interfaces
{
    public interface IManagementRepository
    {
        Task ProcessDailyData(DateTime dateTime);
    }
}