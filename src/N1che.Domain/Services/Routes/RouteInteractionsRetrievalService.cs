using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Routes;
using N1che.Domain.Interfaces.Services.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Domain.Services.Routes;

public sealed class RouteInteractionsRetrievalService : IRouteInteractionsRetrievalService
{
    private readonly IRouteInteractionsReader _routeInteractionsReader;

    public RouteInteractionsRetrievalService(IRouteInteractionsReader routeInteractionsReader)
    {
        _routeInteractionsReader = routeInteractionsReader;
    }

    public async Task<RouteInteractionsModel> GetByRouteIdAsync(Guid routeId, string userId, CancellationToken cancellationToken)
    {
        return await _routeInteractionsReader.GetById(routeId, userId, cancellationToken)
               ?? throw new NotFoundException(EntityTypes.Route, routeId);
    }
}
