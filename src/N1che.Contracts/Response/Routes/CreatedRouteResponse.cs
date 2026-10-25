namespace N1che.Contracts.Response.Routes;

/// <summary>A route that has just been saved.</summary>
public record CreatedRouteResponse
{
    public required Guid Id { get; init; }

    public required DateTime CreatedAt { get; init; }
}
