using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;

namespace FinanceMonitor.DAL.Repositories
{
    public interface IUserRepository
    {
        Task AddUser(UserProfile profile);
    }
}