namespace N1che.Api.Authentication;

public sealed record CognitoOptions
{
    public required string Region { get; init; }
    public required string UserPoolId { get; init; }
    public required string ClientId { get; init; }

    public string Authority => $"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}";
}
