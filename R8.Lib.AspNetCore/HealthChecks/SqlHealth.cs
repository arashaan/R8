using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using System.Threading;
using System.Threading.Tasks;

namespace R8.Lib.AspNetCore.HealthChecks
{
    public class SqlHealth : IHealthCheck
    {
        private readonly string _connectionString;

        public SqlHealth(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            if (string.IsNullOrEmpty(_connectionString))
                return HealthCheckResult.Unhealthy("ConnectionString shouldn't be null");

            await using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);
                }
                catch (SqlException)
                {
                    return HealthCheckResult.Unhealthy();
                }
            }
            return HealthCheckResult.Healthy();
        }
    }
}