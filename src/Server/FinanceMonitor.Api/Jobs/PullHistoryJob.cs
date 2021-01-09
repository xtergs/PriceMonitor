using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class PullHistoryJob : IJob
    {
        private readonly IYahooApiService _apiService;
        private readonly ILogger<PullHistoryJob> _logger;
        private readonly IStockRepository _repository;

        public PullHistoryJob(ILogger<PullHistoryJob> logger,
            IStockRepository repository,
            IYahooApiService apiService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var stocks = await _repository.GetStocksWithoutHistory();

            if (stocks.Count == 0) return;

            foreach (var stock in stocks)
            {
                var history = await _apiService.GetFullHistory(stock.Symbol);

                var mapped = history.Select(x => new PriceHistory
                {
                    Volume = x.Volume,
                    Opened = x.Open,
                    Closed = x.Close,
                    High = x.High,
                    Low = x.Low,
                    DateTime = x.DateTime,
                    StockId = stock.Id
                }).ToArray();

                await _repository.InsertHistory(mapped);
            }
        }
    }
}