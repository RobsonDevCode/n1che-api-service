using N1che.Persistence.Postgres.Postgres.Transactions;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Connections;

public sealed class PostgresConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly PostgresTransactionContext _context;

    public PostgresConnectionFactory(NpgsqlDataSource dataSource, PostgresTransactionContext context)
    {
        _dataSource = dataSource;
        _context = context;
    }

    public async Task<PostgresConnection> ConnectAsync(CancellationToken cancellationToken)
    {
        if (_context.Connection is not null)
        {
            return new PostgresConnection(_context.Connection, ownsConnection: false);
        }

        var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return new PostgresConnection(connection, ownsConnection: true);
    }
}
