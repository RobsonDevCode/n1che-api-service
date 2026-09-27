using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Persistence.Writers.Routes;

/// <summary>Writes routes to the store.</summary>
public interface IRoutesWriter
{
    /// <summary>Creates the route anchored at that point, and returns the identity the store gave it.</summary>
    Task<CreatedRouteModel> Create(CreateRouteModel route, CoordinateModel anchor, CancellationToken cancellationToken);
}
