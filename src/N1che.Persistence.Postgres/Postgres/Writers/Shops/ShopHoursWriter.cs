using Dapper;
using N1che.Domain.Interfaces.Persistence.Writers.Shops;
using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Connections;

namespace N1che.Persistence.Postgres.Postgres.Writers.Shops;

public sealed class ShopHoursWriter : IShopHoursWriter
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ShopHoursWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Create(Guid shopId, IReadOnlyCollection<ShopHoursModel> hours, CancellationToken cancellationToken)
    {
        if (hours.Count == 0)
        {
            return;
        }

        const string sql =
            """
            INSERT INTO shop_hours (shop_id, day_of_week, open_time, close_time)
            VALUES (@ShopId, @DayOfWeek, @OpenTime, @CloseTime)
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = hours.Select(entry => new
        {
            ShopId = shopId,
            entry.DayOfWeek,
            entry.OpenTime,
            entry.CloseTime,
        });

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
