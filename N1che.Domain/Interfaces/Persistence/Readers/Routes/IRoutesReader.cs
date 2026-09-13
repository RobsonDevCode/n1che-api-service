using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Persistence.Readers.Routes;

public interface IRoutesReader
{
    Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearby(RoutesFilterModel filterModel, CancellationToken cancellationToken);
}
