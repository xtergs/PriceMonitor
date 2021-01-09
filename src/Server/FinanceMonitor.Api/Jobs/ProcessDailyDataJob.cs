using System;
using System.Threading.Tasks;
using FinanceMonitor.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Quartz;

namespace FinanceMonitor.Api.Jobs
{
    [DisallowConcurrentExecution]
    public class ProcessDailyDataJob : IJob
    {
        private readonly ILogger<ProcessDailyDataJob> _logger;
        private readonly IManagementRepository _repository;

        public ProcessDailyDataJob(ILogger<ProcessDailyDataJob> logger,
            IManagementRepository repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var lastDate = DateTime.UtcNow.Date.AddDays(-1);
            await _repository.ProcessDailyData(lastDate);
        }
    }
}