using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using OctoMedia.Api.DataAccess.Mssql.Configuration;

namespace OctoMedia.Api.DataAccess.Mssql.Factories
{
    public class MssqlConnectionFactory
    {
        private readonly MssqlConfiguration _configuration;

        public MssqlConnectionFactory(MssqlConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<IDbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
        {
            SqlConnection sqlConnection = new SqlConnection(_configuration.ConnectionString);

            return sqlConnection.OpenAsync(cancellationToken)
                .ContinueWith(t => (IDbConnection) sqlConnection, cancellationToken);
        }
    }
}