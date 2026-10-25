using Dapper;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Writers.Reviews;
using N1che.Domain.Models.Reviews;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;
using N1che.Persistence.Postgres.Postgres.Extensions.Reviews;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Writers.Reviews;

public sealed class ReviewWriter : IReviewsWriter
{
    private const string ShopForeignKey = "reviews_shop_id_fkey";

    private readonly PostgresConnectionFactory _connectionFactory;

    public ReviewWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ReviewModel> Create(CreateReviewRequestModel review, CancellationToken cancellationToken)
    {
        const string sql =
            """
            WITH shop AS (
                SELECT @Niche = ANY(niches) AS niche_listed
                FROM shops
                WHERE id = @ShopId
            ),
            inserted AS (
                INSERT INTO reviews (shop_id, niche, user_id, username, body)
                SELECT @ShopId, @Niche, @UserId, @Username, @Body
                FROM shop
                WHERE shop.niche_listed
                RETURNING id,
                          shop_id,
                          niche,
                          user_id,
                          username,
                          body,
                          update_count,
                          created_at,
                          updated_at
            )
            SELECT shop.niche_listed,
                   inserted.id,
                   inserted.shop_id,
                   inserted.niche,
                   inserted.user_id,
                   inserted.username,
                   inserted.body,
                   inserted.update_count,
                   inserted.created_at,
                   inserted.updated_at
            FROM shop
            LEFT JOIN inserted ON true
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

        try
        {
            var rows = await connection.QueryAsync<CreatedReviewCompositeEntity, ReviewEntity?, CreatedReviewCompositeEntity>(
                command, (created, inserted) => created with { Review = inserted }, splitOn: "id");

            var entity = rows.SingleOrDefault() ?? throw new NotFoundException(EntityTypes.Shop, review.ShopId);

            if (entity is not { NicheListed: true, Review: { } inserted })
            {
                throw new InvalidRequestException(EntityTypes.Review, review.ShopId, review.Niche);
            }

            return inserted.ToDomainModel();
        }
        // The shop was deleted between the niche check and the insert's foreign key check.
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.ForeignKeyViolation
                  && exception.ConstraintName == ShopForeignKey)
        {
            throw new NotFoundException(EntityTypes.Shop, review.ShopId);
        }
    }
}
