using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Persistence.Readers.Routes;

public interface IRoutesReader
{
    Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearby(RoutesFilterModel filterModel, CancellationToken cancellationToken);

    Task<RouteDetailModel?> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>Gets whether a route has that id, without reading the row.</summary>
    Task<bool> Exists(Guid id, CancellationToken cancellationToken);
}
