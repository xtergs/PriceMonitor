using System;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class PullDailyInfoJob : IJob
    {
        private readonly IYahooApiService _apiService;
        private readonly ILogger<PullDailyInfoJob> _logger;
        private readonly IStockRepository _repository;

        public PullDailyInfoJob(ILogger<PullDailyInfoJob> logger,
            IStockRepository repository,
            IYahooApiService apiService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }
        
        public async Task Execute(IJobExecutionContext context)
        {
            var stocks = await _repository.GetStocks();

            if (stocks.Count == 0)
            {
                return;
            }

            foreach (var stock in stocks)
            {
                var apiInfo = await _apiService.GetDailyStock(stock.Symbol);

                if (apiInfo.MarketState == "POST") //closed
                {
                    continue;
                }
                
                await _repository.AddDailyPrice(new PriceDaily()
                {
                    Ask = apiInfo.Ask,
                    Bid = apiInfo.Bid,
                    AskSize = apiInfo.AskSize,
                    BidSize = apiInfo.BidSize,
                    StockId = stock.Id,
                    Time = apiInfo.Time,
                    Price = apiInfo.Price,
                    Volume =  apiInfo.Volume,
                });
            }
        }
    }
}