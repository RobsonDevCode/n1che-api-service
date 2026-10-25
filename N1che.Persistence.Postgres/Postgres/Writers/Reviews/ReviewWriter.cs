using Dapper;
using N1che.Domain.Interfaces.Persistence.Writers.Reviews;
using N1che.Domain.Models.Reviews;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;
using N1che.Persistence.Postgres.Postgres.Extensions.Reviews;

namespace N1che.Persistence.Postgres.Postgres.Writers.Reviews;

public sealed class ReviewWriter : IReviewsWriter
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ReviewWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReviewModel> Create(CreateReviewRequestModel review, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO reviews (shop_id, niche, user_id, username, body)
            VALUES (@ShopId, @Niche, @UserId, @Username, @Body)
            RETURNING id,
                      shop_id,
                      niche,
                      user_id,
                      username,
                      body,
                      update_count,
                      created_at,
                      updated_at
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            review.ShopId,
            review.Niche,
            review.UserId,
            review.Username,
            Body = review.Text,
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var entity = await connection.QuerySingleAsync<ReviewEntity>(command);

        return entity.ToDomainModel();
    }
}
