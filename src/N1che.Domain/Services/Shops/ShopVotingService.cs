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
            if (!await _shopsReader.Exists(shopId, token))
            {
                throw new NotFoundException(EntityTypes.Shop, shopId);
            }

            await _shopVotesWriter.Delete(shopId, user.Id, token);
            await _shopsWriter.DecrementVoteCount(shopId, token);
        }, cancellationToken);
    }
}
