using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
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

        public async Task AddUser(Models.UserProfile profile)
        {
            await using var con = GetConnection();
            await con.ExecuteAsync("EXEC dbo.AddUser @Id", profile);
        }

        public async Task<ICollection<UserStock>> GetUserStocks(string userId)
        {
            await using var db = GetConnection();

            var collection = await db.QueryAsync<UserStock>(
                @"exec dbo.GetUserStocks @UserId", new
                {
                    UserId = userId
                });

            return collection.ToArray();
        }

        public async Task<ICollection<UserPrice>> GetUserStockShares(string userId, string symbol)
        {
            await using var db = GetConnection();

            var collection = await db.QueryAsync<UserPrice>(@"exec dbo.GetUserStockPrices  @UserId, @Symbol", new
            {
                UserId = userId,
                Symbol = symbol
            });

            return collection.ToArray();
        }

        public async Task<UserPrice> AddUserPrice(UserPrice price)
        {
            await using var db = GetConnection();

            var inserted = await db.QueryFirstAsync<UserPrice>(
                @"exec dbo.AddUserPrice @UserId, @StockId, @Price, @Count, @DateTime", price);

            return inserted;
        }
    }
}