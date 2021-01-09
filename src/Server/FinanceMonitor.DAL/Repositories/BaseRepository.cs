using Microsoft.Data.SqlClient;

namespace FinanceMonitor.DAL.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected SqlConnection GetConnection()
        {
            return new(_connectionString);
        }
    }
}