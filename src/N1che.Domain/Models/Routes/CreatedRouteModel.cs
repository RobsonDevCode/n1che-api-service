namespace N1che.Domain.Models.Routes;

/// <summary>A route as the store returned it once saved.</summary>
public record CreatedRouteModel
{
    public required Guid Id { get; init; }

    public required DateTime CreatedAt { get; init; }
}
