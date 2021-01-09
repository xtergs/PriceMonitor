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
                    Field.Language,
                    Field.QuoteType)
                .QueryAsync();

            if (!result.ContainsKey(symbol)) return null;

            var data = result[symbol];
            var model = new ApiStock
            {
                Symbol = data.Symbol,
                Market = data.Market,
                Time = DateTimeOffset.FromUnixTimeSeconds(data.RegularMarketTime).UtcDateTime,
                Timezone = data.ExchangeTimezoneName,
                ShortName = data.ShortName,
                Currency = data.Currency,
                Language = data.Language,
                QuoteType = data.QuoteType
            };

            if (data.Fields.ContainsKey("LongName")) model.LongName = data.LongName;

            if (data.Fields.ContainsKey("FinancialCurrency")) model.FinancialCurrency = data.FinancialCurrency;

            return model;
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
                    Field.PostMarketPrice,
                    Field.RegularMarketTime,
                    Field.RegularMarketPrice,
                    Field.RegularMarketVolume)
                .QueryAsync();

            if (!result.ContainsKey(symbol)) return null;

            var data = result[symbol];
            var time = DateTime.UtcNow;

            var model = new ApiDailyStock
            {
                Symbol = data.Symbol,
                MarketState = data.MarketState
            };

            if (data.MarketState == "REGULAR")
            {
                model.Time = DateTimeOffset.FromUnixTimeSeconds(data.RegularMarketTime).UtcDateTime;
                model.Price = data.RegularMarketPrice;
                model.Volume = data.RegularMarketVolume;
            }
            else if (data.MarketState == "POST")
            {
                model.Time = DateTimeOffset.FromUnixTimeSeconds(data.PostMarketTime).UtcDateTime;
                model.Price = data.PostMarketPrice;
                model.Volume = data.RegularMarketVolume;
            }

            if (data.Fields.ContainsKey("Ask")) model.Ask = data.Ask;

            if (data.Fields.ContainsKey("AskSize")) model.AskSize = data.AskSize;

            if (data.Fields.ContainsKey("Bid")) model.Bid = data.Bid;

            if (data.Fields.ContainsKey("BidSize")) model.BidSize = data.BidSize;

            return model;
        }
    }
}