using System;
using System.Threading.Tasks;
using Dapper;
using FinanceMonitor.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace FinanceMonitor.DAL.Repositories
{
    public class ManagementRepository : BaseRepository, IManagementRepository
    {
        public ManagementRepository(IOptions<StockOptions> options)
            : base(options.Value.ConnectionString)
        {
        }

        public async Task ProcessDailyData(DateTime dateTime)
        {
            var connection = GetConnection();

            await connection.ExecuteAsync("exec dbo.ProcessDailyDataIntoHistory @Start", new
            {
                Start = dateTime
            });
        }

        public async Task CalculateFullHistoryGraphic()
        {
            var connection = GetConnection();

            await connection.ExecuteAsync("exec dbo.CalculateHistoryGraphic");
        }
    }
}