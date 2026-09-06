using N1che.Contracts.Filters.Shops;
using N1che.Domain.Models.Shops;

namespace N1che.Api.Extensions.Shops;

public static class ShopsFilterExtensions
{
    public static ShopsFilterModel ToDomainFilter(this ShopsFilter filter) => new()
    {
        Niche = filter.Niche,
    };
}
