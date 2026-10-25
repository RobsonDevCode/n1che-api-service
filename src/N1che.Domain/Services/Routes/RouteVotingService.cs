using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces;
using N1che.Domain.Interfaces.Persistence.Readers.Routes;
using N1che.Domain.Interfaces.Persistence.Writers.Routes;
using N1che.Domain.Interfaces.Services.Routes;
using N1che.Domain.Models.Users;

namespace N1che.Domain.Services.Routes;

public sealed class RouteVotingService : IRouteVotingService
{
    private readonly IRouteVotesWriter _routeVotesWriter;
    private readonly IRoutesWriter _routesWriter;
    private readonly IRoutesReader _routesReader;
    private readonly ITransactionScope _transactionScope;

    public RouteVotingService(
        IRouteVotesWriter routeVotesWriter,
        IRoutesWriter routesWriter,
        IRoutesReader routesReader,
        ITransactionScope transactionScope)
    {
        _routeVotesWriter = routeVotesWriter;
        _routesWriter = routesWriter;
        _routesReader = routesReader;
        _transactionScope = transactionScope;
    }

    public async Task AddAsync(Guid routeId, UserModel user, CancellationToken cancellationToken)
    {
        await _transactionScope.ExecuteAsync(async token =>
        {
            await _routeVotesWriter.Create(routeId, user.Id, token);
            await _routesWriter.IncrementVoteCount(routeId, token);
        }, cancellationToken);
    }

    public async Task RemoveAsync(Guid routeId, UserModel user, CancellationToken cancellationToken)
    {
        await _transactionScope.ExecuteAsync(async token =>
        {
            if (!await _routesReader.Exists(routeId, token))
            {
                throw new NotFoundException(EntityTypes.Route, routeId);
            }

            await _routeVotesWriter.Delete(routeId, user.Id, token);
            await _routesWriter.DecrementVoteCount(routeId, token);
        }, cancellationToken);
    }
}
