using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Niches;
using N1che.Domain.Models.Niches;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Niches;
using N1che.Persistence.Postgres.Postgres.Extensions.Niches;

namespace N1che.Persistence.Postgres.Postgres.Readers.Niches;

public sealed class NicheReader : INichesReader
{
    private const string SelectAllSql = "SELECT id, label, sub_label, description FROM niches ORDER BY label";

    private readonly PostgresConnectionFactory _connectionFactory;

    public NicheReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<NicheModel>> GetAll(CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(SelectAllSql, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<NicheEntity>(command);

        return entities.Select(entity => entity.ToDomainModel()).ToArray();
    }
}
