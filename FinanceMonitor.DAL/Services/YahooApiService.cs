using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Services.Interfaces;
using YahooFinanceApi;

namespace FinanceMonitor.DAL.Services
{
    public class YahooApiService : IYahooApiService
    {
        public async Task<ApiStock> GetStock(string symbol)
        {
            symbol = symbol?.ToUpper();
            var result = await Yahoo.Symbols(symbol)
                .Fields(Field.Symbol, Field.RegularMarketPrice, Field.FiftyTwoWeekHigh,
                    Field.Currency,
                    Field.FinancialCurrency,
                    Field.LongName,
                    Field.ShortName,
                    Field.Language)
                .QueryAsync();

            if (!result.ContainsKey(symbol)) return null;

            var data = result[symbol];
            return new ApiStock
            {
                Symbol = data.Symbol,
                Market = data.Market,
                Time = DateTimeOffset.FromUnixTimeSeconds(data.RegularMarketTime).UtcDateTime,
                Timezone = data.ExchangeTimezoneName,
                LongName = data.LongName,
                ShortName = data.ShortName,
                Currency = data.Currency,
                Language = data.Language,
                FinancialCurrency = data.FinancialCurrency
            };
        }

        public async Task<ICollection<ApiHistory>> GetFullHistory(string symbol)
        {
            symbol = symbol.ToUpper();
            var result = await Yahoo.GetHistoricalAsync(symbol, new DateTime(2000, 1, 1),
                DateTime.UtcNow);

            var models = result.Select(x => new ApiHistory
            {
                Volume = x.Volume,
                Low = (double) x.Low,
                High = (double) x.High,
                Open = (double) x.Open,
                Close = (double) x.Close,
                DateTime = x.DateTime
            }).ToArray();

            return models;
        }


        public async Task<ApiDailyStock> GetDailyStock(string symbol)
        {
            symbol = symbol?.ToUpper();
            var result = await Yahoo.Symbols(symbol)
                .Fields(Field.Symbol,
                    Field.Ask,
                    Field.Bid,
                    Field.AskSize,
                    Field.BidSize,
                    Field.MarketState,
                    Field.PostMarketTime,
                    Field.PostMarketPrice)
                .QueryAsync();

            if (!result.ContainsKey(symbol)) return null;

            var data = result[symbol];
            return new ApiDailyStock
            {
                Symbol = data.Symbol,
                Time = DateTimeOffset.FromUnixTimeSeconds(data.PostMarketTime).UtcDateTime,
                Ask = data.Ask,
                Bid = data.Bid,
                AskSize = data.AskSize,
                BidSize = data.BidSize,
                MarketState = data.MarketState
            };
        }
    }
}