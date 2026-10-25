using Microsoft.Extensions.Caching.Memory;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Services.Shops;

public sealed class ShopRetrievalService : IShopRetrievalService
{
    private readonly IShopsReader _shopsReader;
    private readonly IMemoryCache _cache;

    public ShopRetrievalService(IShopsReader shopsReader, IMemoryCache cache)
    {
        _shopsReader = shopsReader;
        _cache = cache;
    }

    public Task<IReadOnlyCollection<ShopModel>> GetNearbyAsync(NearbyShopsFilter filter, CancellationToken cancellationToken)
    {
        return _shopsReader.GetNearby(filter, cancellationToken);
    }

    public Task<PaginationModel<ShopModel>> GetPageAsync(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken)
    {
        return _shopsReader.GetPage(filterModel, pagination, cancellationToken);
    }

    public async ValueTask<ShopModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Throwing inside the factory leaves the entry uncommitted, so unknown ids are never cached.
        var shop = await _cache.GetOrCreateAsync($"shop:{id}", async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            return await _shopsReader.GetById(id, cancellationToken)
                   ?? throw new NotFoundException(EntityTypes.Shop, id);
        });

        return shop!;
    }
}
