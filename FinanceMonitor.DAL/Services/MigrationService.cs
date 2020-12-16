using System.IO;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FinanceMonitor.DAL.Services
{
    public class MigrationService
    {
        private readonly MigrationOptions _options;

        public MigrationService(IOptions<MigrationOptions> options)
        {
            _options = options.Value;
        }

        public async Task Migrate()
        {
            var migrationScript = await File.ReadAllTextAsync(_options.FilePath);

            await using var db = new SqlConnection(_options.ConnectionString);
            await db.OpenAsync();
            await using var transaction = db.BeginTransaction();

            await db.ExecuteAsync(migrationScript, transaction: transaction);

            await transaction.CommitAsync();
        }
    }
}