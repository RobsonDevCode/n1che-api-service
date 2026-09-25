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
        var removed = await _bookmarksWriter.Delete(shopId, user.Id, cancellationToken);
        if (removed)
        {
            return;
        }

        // A delete that removed nothing means the shop is gone or the bookmark never existed; the
        // missing shop is the more informative of the two, so it wins.
        var shopExists = await _shopsReader.Exists(shopId, cancellationToken);

        throw shopExists
            ? new NotFoundException(EntityTypes.Bookmark, shopId)
            : new NotFoundException(EntityTypes.Shop, shopId);
    }
}
