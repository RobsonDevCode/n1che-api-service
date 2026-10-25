using N1che.Domain.Models.Reviews;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Reviews;

public static class ReviewEntityExtensions
{
    public static ReviewModel ToDomainModel(this ReviewEntity entity) => new()
    {
        Id = entity.Id,
        ShopId = entity.ShopId,
        Niche = entity.Niche,
        UserId = entity.UserId,
        Username = entity.Username,
        Text = entity.Body,
        CreatedAt = entity.CreatedAt,
    };
}
