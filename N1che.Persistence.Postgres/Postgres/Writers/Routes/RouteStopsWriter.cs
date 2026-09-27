using Dapper;
using N1che.Domain.Interfaces.Persistence.Writers.Routes;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Writers.Routes;

public sealed class RouteStopsWriter : IRouteStopsWriter
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public RouteStopsWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Create(
        Guid routeId, IReadOnlyDictionary<Guid, int> stopPositions, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO route_stops (route_id, shop_id, position)
            VALUES (@RouteId, @ShopId, @Position)
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var entities = stopPositions.Select(stop => new RouteStopEntity
        {
            RouteId = routeId,
            ShopId = stop.Key,
            Position = stop.Value,
        }).ToArray();

        var command = new CommandDefinition(sql, entities, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
