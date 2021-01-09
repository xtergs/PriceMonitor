using System.Threading.Tasks;
using Dapper;
using FinanceMonitor.DAL.Models;
using Microsoft.Extensions.Options;

namespace FinanceMonitor.DAL.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly StockOptions _options;

        public UserRepository(IOptions<StockOptions> options)
            : base(options.Value.ConnectionString)
        {
            _options = options.Value;
        }

        public async Task AddUser(UserProfile profile)
        {
            await using var con = GetConnection();
            await con.ExecuteAsync("EXEC dbo.AddUser @Id", profile);
        }
    }
}