using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FinanceMonitor.DAL.Dto;
using FinanceMonitor.DAL.Enums;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Stocks.Queries.GetSavedStocks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FinanceMonitor.DAL.Repositories
{
    public class StockRepository : BaseRepository, IStockRepository
    {
        private readonly StockOptions _options;

        public StockRepository(IOptions<StockOptions> options)
            : base(options.Value.ConnectionString)
        {
            _options = options.Value;
        }


        public async Task<Stock?> GetStock(string symbol)
        {
            await using var db = new SqlConnection(_options.ConnectionString);

            var stock = await db.QueryFirstOrDefaultAsync<Stock>(@"exec dbo.GetStock @Symbol", new
            {
                Symbol = symbol
            });

            return stock;
        }

        public async Task<Stock> CreateStock(Stock stock)
        {
            await using var db = new SqlConnection(_options.ConnectionString);

            var inserted = await db.QueryFirstAsync<Stock>(
                @"exec dbo.AddStock @Symbol, @Market, @Timezone, @Time, @ShortName, @LongName, @Currency, 
 @FinancialCurrency, @Language, @QuoteType", stock);
            return inserted;
        }


        public async Task<ICollection<Stock>> GetStocksWithoutHistory()
        {
            var db = GetConnection();

            var stocks = await db.QueryAsync<Stock>(@"exec dbo.GetStocksWithoutHistory");

            return stocks.ToArray();
        }

        public async Task InsertHistory(ICollection<PriceHistory> history)
        {
            var db = GetConnection();

            var result = await db.ExecuteAsync(@"exec dbo.InsertHistory
 @StockSymbol, @Volume, @Opened, @Closed, @High, @Low, @DateTime",
                history);

            var inserted = result;
        }

        public async Task AddDailyPrice(PriceDaily price)
        {
            var db = GetConnection();

            var result = await db.ExecuteAsync(
                @"exec dbo.AddDailyPrice
 @StockSymbol, @Ask, @Bid, @AskSize, @BidSize, @Time, @Price, @Volume", price);
        }

        public async Task<ICollection<ShortStockInfo>> GetStocks()
        {
            var db = GetConnection();

            var result = await db.QueryAsync<ShortStockInfo>(
                @"exec dbo.GetStocks");

            return result.ToArray();
        }

        public async Task<ICollection<StockListItemDto>> GetSavedStocks()
        {
            var db = GetConnection();

            var result = await db.QueryMultipleAsync(
                @"exec dbo.GetSavedStocks");


            var stocks = result.Read<StockListItemDto>();
            var history = result.Read<PriceHistory>();

            var grouped = stocks.GroupJoin(history, dto => dto.Symbol, priceHistory => priceHistory.StockSymbol,
                (dto, histories) =>
                {
                    dto = dto with {FullHistory = histories.ToArray()};
                    return dto;
                });

            return grouped.ToArray();
        }

        public async Task<ICollection<PriceHistory>> GetStockHistory(string symbol,
            HistoryType type,
            DateTime start = default, DateTime end = default)
        {
            var db = GetConnection();

            var result = await db.QueryAsync<PriceHistory>(
                @"exec dbo.GetStockHistory @Symbol , @Type, @Start, @End", new
                {
                    Symbol = symbol,
                    Type = type,
                    Start = start,
                    End = end
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

        public Task UpdateStockStatus(string symbol, string status)
        {
            var db = GetConnection();
            return db.ExecuteAsync("exec dbo.UpdateStockStatus @Symbol, @Status", new
            {
                Symbol = symbol,
                Status = status
            });
        }
    }
}