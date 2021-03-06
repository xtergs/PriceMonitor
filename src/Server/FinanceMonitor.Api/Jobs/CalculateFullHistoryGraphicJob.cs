using System;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using FinanceMonitor.DAL.Stocks.Commands.CalculateFullHistoryGraphic;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class CalculateFullHistoryGraphicJob : IJob
    {
        private readonly ILogger<CalculateFullHistoryGraphicJob> _logger;
        private readonly IManagementRepository _repository;
        private readonly IMediator _mediator;

        public CalculateFullHistoryGraphicJob(ILogger<CalculateFullHistoryGraphicJob> logger,
            IManagementRepository repository, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _mediator.Send(new CalculateFullHistoryGraphicCommand());
        }
    }
}