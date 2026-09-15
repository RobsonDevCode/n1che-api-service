using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Shops;

public static class ShopInteractionsCompositeEntityExtensions
{
    public static ShopInteractionsModel ToDomainModel(this ShopInteractionsCompositeEntity entity) => new()
    {
        VoteCount = entity.VoteCount,
        Voted = entity.Voted,
        Saved = entity.Saved,
    };
}
