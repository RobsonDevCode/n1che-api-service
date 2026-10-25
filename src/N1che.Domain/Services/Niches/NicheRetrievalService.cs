using Microsoft.Extensions.Caching.Memory;
using N1che.Domain.Interfaces.Persistence.Readers.Niches;
using N1che.Domain.Interfaces.Services.Niches;
using N1che.Domain.Models.Niches;

namespace N1che.Domain.Services.Niches;

public sealed class NicheRetrievalService : INicheRetrievalService
{
    private readonly INichesReader _nichesReader;
    private readonly IMemoryCache _cache;

    private const string AllNichesCacheKey = "niches:all";
    public NicheRetrievalService(INichesReader nichesReader, IMemoryCache cache)
    {
        _nichesReader = nichesReader;
        _cache = cache;
    }

    public async ValueTask<IReadOnlyCollection<NicheModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _cache.GetOrCreateAsync(AllNichesCacheKey, async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromHours(1));
            return await _nichesReader.GetAll(cancellationToken);
        }) ?? [];
    }
}
