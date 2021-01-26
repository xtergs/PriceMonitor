using System;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;
using FinanceMonitor.DAL.Stocks.Commands.UpdateStockStatus;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class PullDailyInfoJob : IJob
    {
        private readonly IYahooApiService _apiService;
        private readonly IMediator _mediator;
        private readonly ILogger<PullDailyInfoJob> _logger;
        private readonly IStockRepository _repository;

        public PullDailyInfoJob(ILogger<PullDailyInfoJob> logger,
            IStockRepository repository,
            IYahooApiService apiService,
            IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var stocks = await _repository.GetStocks();

            if (stocks.Count == 0) return;

            foreach (var stock in stocks)
            {
                var apiInfo = await _apiService.GetDailyStock(stock.Symbol);

                _mediator.Send(new UpdateStockStatusCommand(apiInfo.Symbol, apiInfo.MarketState));

                if (apiInfo.MarketState == "POST") //closed
                    continue;
                
                if (apiInfo.MarketState == "CLOSED")
                    continue;

                await _repository.AddDailyPrice(new PriceDaily
                {
                    Ask = apiInfo.Ask,
                    Bid = apiInfo.Bid,
                    AskSize = apiInfo.AskSize,
                    BidSize = apiInfo.BidSize,
                    StockSymbol = stock.Symbol,
                    Time = apiInfo.Time,
                    Price = apiInfo.Price,
                    Volume = apiInfo.Volume
                });
            }
        }
    }
}