using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Services.Routes;

/// <summary>Saves the routes users build.</summary>
public interface IRouteCreationService
{
    /// <summary>Saves the route and returns the identity the store gave it.</summary>
    /// <exception cref="Exceptions.InvalidRequestException">The niche is not one of the seeded niches.</exception>
    /// <exception cref="Exceptions.NotFoundException">A stop names no shop.</exception>
    Task<CreatedRouteModel> CreateAsync(CreateRouteModel route, CancellationToken cancellationToken);
}
