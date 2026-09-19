using Dapper;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Connections;

public sealed class PostgresConnection(NpgsqlConnection connection, bool ownsConnection) : IAsyncDisposable
{
    public Task<IEnumerable<T>> QueryAsync<T>(CommandDefinition command) => connection.QueryAsync<T>(command);

    public Task<T?> QuerySingleOrDefaultAsync<T>(CommandDefinition command) => connection.QuerySingleOrDefaultAsync<T>(command);

    // Dapper annotates the result as nullable although a missing row throws instead.
    public Task<T> QuerySingleAsync<T>(CommandDefinition command) => connection.QuerySingleAsync<T>(command)!;

    public Task<int> ExecuteAsync(CommandDefinition command) => connection.ExecuteAsync(command);

    public Task<T> ExecuteScalarAsync<T>(CommandDefinition command) => connection.ExecuteScalarAsync<T>(command);

    public ValueTask DisposeAsync() => ownsConnection ? connection.DisposeAsync() : ValueTask.CompletedTask;
}
