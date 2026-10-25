using N1che.Domain.Interfaces;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Transactions;

public sealed class PostgresTransactionScope : ITransactionScope
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly PostgresTransactionContext _context;

    public PostgresTransactionScope(NpgsqlDataSource dataSource, PostgresTransactionContext context)
    {
        _dataSource = dataSource;
        _context = context;
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
    {
        await ExecuteAsync(async token =>
        {
            await operation(token);
            return true;
        }, cancellationToken);
    }

    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        if (_context.Transaction is not null)
        {
            return await operation(cancellationToken);
        }

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        _context.Connection = connection;
        _context.Transaction = transaction;
        try
        {
            var result = await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            _context.Connection = null;
            _context.Transaction = null;
        }
    }
}
