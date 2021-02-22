using System;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class CalculateFullHistoryGraphicJob : IJob
    {
        private readonly ILogger<CalculateFullHistoryGraphicJob> _logger;
        private readonly IManagementRepository _repository;

        public CalculateFullHistoryGraphicJob(ILogger<CalculateFullHistoryGraphicJob> logger,
            IManagementRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _repository.CalculateFullHistoryGraphic();
        }
    }
}