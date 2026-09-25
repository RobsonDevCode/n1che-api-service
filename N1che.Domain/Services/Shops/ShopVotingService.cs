using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Persistence.Writers.Shops;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Models.Users;

namespace N1che.Domain.Services.Shops;

public sealed class ShopVotingService : IShopVotingService
{
    private readonly IShopVotesWriter _shopVotesWriter;
    private readonly IShopsWriter _shopsWriter;
    private readonly IShopsReader _shopsReader;
    private readonly ITransactionScope _transactionScope;

    public ShopVotingService(
        IShopVotesWriter shopVotesWriter,
        IShopsWriter shopsWriter,
        IShopsReader shopsReader,
        ITransactionScope transactionScope)
    {
        _shopVotesWriter = shopVotesWriter;
        _shopsWriter = shopsWriter;
        _shopsReader = shopsReader;
        _transactionScope = transactionScope;
    }

    public async Task AddAsync(Guid shopId, UserModel user, CancellationToken cancellationToken)
    {
        await _transactionScope.ExecuteAsync(async token =>
        {
            await _shopVotesWriter.Create(shopId, user.Id, token);
            await _shopsWriter.IncrementVoteCount(shopId, token);
        }, cancellationToken);
    }

    public async Task RemoveAsync(Guid shopId, UserModel user, CancellationToken cancellationToken)
    {
        await _transactionScope.ExecuteAsync(async token =>
        {
            var removed = await _shopVotesWriter.Delete(shopId, user.Id, token);
            if (removed)
            {
                await _shopsWriter.DecrementVoteCount(shopId, token);
                return;
            }

            // A delete that removed nothing means the shop is gone or the vote never existed; the
            // missing shop is the more informative of the two, so it wins.
            var shopExists = await _shopsReader.Exists(shopId, token);

            throw shopExists
                ? new NotFoundException(EntityTypes.Vote, shopId)
                : new NotFoundException(EntityTypes.Shop, shopId);
        }, cancellationToken);
    }
}
