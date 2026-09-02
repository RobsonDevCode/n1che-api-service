using N1che.Domain.Models.Niches;
using N1che.Persistence.Postgres.Postgres.Entities.Niches;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Niches;

public static class NicheEntityExtensions
{
    public static NicheModel ToDomainModel(this NicheEntity entity) => new()
    {
        Id = entity.Id,
        Label = entity.Label,
        SubLabel = entity.SubLabel,
        Description = entity.Description,
    };
}
