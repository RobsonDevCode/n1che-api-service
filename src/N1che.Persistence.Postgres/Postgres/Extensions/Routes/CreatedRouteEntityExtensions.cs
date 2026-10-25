using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

public static class CreatedRouteEntityExtensions
{
    public static CreatedRouteModel ToDomainModel(this CreatedRouteEntity entity) => new()
    {
        Id = entity.Id,
        CreatedAt = entity.CreatedAt,
    };
}
