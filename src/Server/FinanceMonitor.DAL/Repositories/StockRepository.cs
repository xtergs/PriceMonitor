using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FinanceMonitor.DAL.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly StockOptions _options;

        public StockRepository(IOptions<StockOptions> options)
        {
            _options = options.Value;
        }
        

        public async Task<Stock> GetStock(string symbol)
        {
            await using var db = new SqlConnection(_options.ConnectionString);

            var stock = await db.QueryFirstOrDefaultAsync<Stock>(@"select * from Stock
            where Symbol = @Symbol", new
            {
                Symbol = symbol
            });

            return stock;
        }

        public async Task<Stock> CreateStock(Stock stock)
        {
            await using var db = new SqlConnection(_options.ConnectionString);

            var inserted = await db.QueryFirstAsync<Stock>(
                @"Insert into Stock (Symbol, Market, Timezone, Time, ShortName, LongName, Currency, FinancialCurrency, 
                   Language, QuoteType )
Output  Inserted.* 
values (@Symbol, @Market, @Timezone, @Time, @ShortName, @LongName, @Currency, @FinancialCurrency, 
        @Language, @QuoteType)", stock);
            return inserted;
        }

        public async Task<UserPrice> AddUserPrice(UserPrice price)
        {
            await using var db = GetConnection();

            var inserted = await db.QueryFirstAsync<UserPrice>(
                @"Insert into UserPrice(UserId, StockId, Price, Count, DateTime)
 OUTPUT Inserted.*
 values (@UserId, @StockId, @Price, @Count, @DateTime)", price);

            return inserted;
        }

        public async Task<ICollection<UserPrice>> GetUserStockPrices(Guid userId, Guid stockId)
        {
            await using var db = GetConnection();

            var collection = await db.QueryAsync<UserPrice>(@"select * from UserPrice
where UserId = @UserId and StockId = @StockId", new
            {
                UserId = userId,
                StockId = stockId
            });

            return collection.ToArray();
        }

        public async Task<ICollection<UserSock>> GetUserStocks(Guid userId)
        {
            await using var db = GetConnection();

            var collection = await db.QueryAsync<UserSock>(
                @"select s.Id, s.Symbol, Count(UP.Id) as Count from Stock as s
inner join UserPrice UP on s.Id = UP.StockId
where UP.UserId = @UserId
group by s.Id, s.Symbol", new
                {
                    UserId = userId
                });

            return collection.ToArray();
        }

        public async Task<ICollection<Stock>> GetStocksWithoutHistory()
        {
            var db = GetConnection();

            var stocks = await db.QueryAsync<Stock>(@"select S.* from Stock as S
left join PriceHistory PH on S.Id = PH.StockId
where PH.Id is null");

            return stocks.ToArray();
        }

        public async Task InsertHistory(ICollection<PriceHistory> history)
        {
            var db = GetConnection();

            var result =  await db.ExecuteAsync(@"Insert into PriceHistory (StockId, Volume, Opened, Closed, High, Low, DateTime) 
values (@StockId, @Volume, @Opened, @Closed, @High, @Low, @DateTime)",
                history);

            var inserted = result;
        }

        public async Task AddDailyPrice(PriceDaily price)
        {
            var db = GetConnection();

            var result = await db.ExecuteAsync(
                @"Insert into PriceDaily (StockId, Ask, Bid, AskSize, BidSize, Time,
                        Price, Volume) 
values (@StockId, @Ask, @Bid, @AskSize, @BidSize, @Time, @Price, @Volume)", price);
        }

        public async Task<ICollection<ShortStockInfo>> GetStocks()
        {
            var db = GetConnection();

            var result = await db.QueryAsync<ShortStockInfo>(
                @"select S.Id, S.Symbol from Stock as S");

            return result.ToArray();
        }
        
        public async Task<ICollection<Stock>> GetSavedStocks()
        {
            var db = GetConnection();

            var result = await db.QueryAsync<Stock>(
                @"select S.* from Stock as S");

            return result.ToArray();
        }

        public async Task<ICollection<PriceHistory>> GetStockHistory(string stockId)
        {
            var db = GetConnection();

            var result = await db.QueryAsync<PriceHistory>(
                @"exec dbo.GetStock @StockId , @Time", new
                {
                    StockId = stockId,
                    Time = DateTime.UtcNow.AddDays(-90)
                });

            return result.ToArray();
        }

        public async Task<ICollection<PriceDaily>> GetStockDaily(string symbol, DateTime date)
        {
            var db = GetConnection();

            var start = date.ToUniversalTime().Date;
            var end = start.AddDays(1);
            
            var result = await db.QueryAsync<PriceDaily>(
                @"exec dbo.GetStockDaily @Symbol, @Start, @End", new
                {
                    Symbol = symbol,
                    Start = start,
                    End = end
                });

            return result.ToArray();
        }


        private SqlConnection GetConnection() => new(_options.ConnectionString);
    }
}