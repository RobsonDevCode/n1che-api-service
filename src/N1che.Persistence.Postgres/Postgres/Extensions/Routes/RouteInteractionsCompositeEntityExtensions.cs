using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

public static class RouteInteractionsCompositeEntityExtensions
{
    public static RouteInteractionsModel ToDomainModel(this RouteInteractionsCompositeEntity entity) => new()
    {
        VoteCount = entity.VoteCount,
        Voted = entity.Voted,
    };
}
