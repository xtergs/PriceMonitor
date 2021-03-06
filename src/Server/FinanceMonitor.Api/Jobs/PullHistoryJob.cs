﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Models;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Services.Interfaces;
using FinanceMonitor.DAL.Stocks.Commands.CalculateFullHistoryGraphic;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class PullHistoryJob : IJob
    {
        private readonly IYahooApiService _apiService;
        private readonly IMediator _mediator;
        private readonly ILogger<PullHistoryJob> _logger;
        private readonly IStockRepository _repository;

        public PullHistoryJob(ILogger<PullHistoryJob> logger,
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
                    StockSymbol = stock.Symbol
                }).ToArray();

                await _repository.InsertHistory(mapped);
            }

            await _mediator.Send(new CalculateFullHistoryGraphicCommand());
        }
    }
}