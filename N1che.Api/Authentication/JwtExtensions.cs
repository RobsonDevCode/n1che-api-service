using System.Security.Claims;
using N1che.Domain.Exceptions;
using N1che.Domain.Models.Users;

namespace N1che.Api.Authentication;

public static class JwtExtensions
{
    private const string SubjectClaim = "sub";
    private const string UsernameClaim = "username";

    public static UserModel ToUserModel(this ClaimsPrincipal principal) => new()
    {
        Id = principal.FindFirstValue(SubjectClaim) ?? throw new UnauthorizedException(),
        Username = principal.FindFirstValue(UsernameClaim) ?? throw new UnauthorizedException(),
    };
}
