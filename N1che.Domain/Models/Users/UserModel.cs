namespace N1che.Domain.Models.Users;

/// <summary>The user making the request, as carried by their access token.</summary>
public record UserModel
{
    /// <summary>Cognito subject — the user key, as there is no users table.</summary>
    public required string Id { get; init; }

    public required string Username { get; init; }
}
