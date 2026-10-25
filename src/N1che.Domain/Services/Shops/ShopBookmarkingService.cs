using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Persistence.Writers.Shops;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Models.Users;

namespace N1che.Domain.Services.Shops;

public sealed class ShopBookmarkingService : IShopBookmarkingService
{
    private readonly IBookmarksWriter _bookmarksWriter;
    private readonly IShopsReader _shopsReader;

    public ShopBookmarkingService(IBookmarksWriter bookmarksWriter, IShopsReader shopsReader)
    {
        _bookmarksWriter = bookmarksWriter;
        _shopsReader = shopsReader;
    }

    public async Task AddAsync(Guid shopId, UserModel user, CancellationToken cancellationToken)
    {
        await _bookmarksWriter.Create(shopId, user.Id, cancellationToken);
    }

    public async Task RemoveAsync(Guid shopId, UserModel user, CancellationToken cancellationToken)
    {
        if (!await _shopsReader.Exists(shopId, cancellationToken))
        {
            throw new NotFoundException(EntityTypes.Shop, shopId);
        }

        await _bookmarksWriter.Delete(shopId, user.Id, cancellationToken);
    }
}
