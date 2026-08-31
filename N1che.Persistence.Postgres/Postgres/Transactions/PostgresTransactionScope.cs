using N1che.Domain.Interfaces;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Transactions;

public sealed class PostgresTransactionScope(NpgsqlDataSource dataSource, PostgresTransactionContext context)
    : ITransactionScope
{
    public async Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken)
    {
        if (context.Transaction is not null)
        {
            return await operation(cancellationToken);
        }

        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        context.Connection = connection;
        context.Transaction = transaction;
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
            context.Connection = null;
            context.Transaction = null;
        }
    }
}
