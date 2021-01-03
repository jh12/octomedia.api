using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OctoMedia.Api.Common.Options;

namespace OctoMedia.Api.DataAccess.Mssql.Factories
{
    public class MssqlConnectionFactory
    {
        private readonly MssqlOptions _configuration;

        public MssqlConnectionFactory(IOptions<MssqlOptions> configuration)
        {
            _configuration = configuration.Value;
        }

        public Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            SqlConnection sqlConnection = new SqlConnection(_configuration.ConnectionString);

            return sqlConnection.OpenAsync(cancellationToken)
                .ContinueWith(t => (IDbConnection) sqlConnection, cancellationToken);
        }
    }
}