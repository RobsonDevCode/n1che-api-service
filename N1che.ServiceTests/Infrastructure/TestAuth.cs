using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace N1che.ServiceTests.Infrastructure;

// Mints Cognito-access-token-shaped JWTs signed with a symmetric key the test host trusts, so the
// real JwtBearer pipeline runs end-to-end without reaching Cognito's JWKS endpoint.
internal static class TestAuth
{
    public const string Region = "eu-west-2";
    public const string UserPoolId = "eu-west-2_testpool";
    public const string ClientId = "n1che-mobile";
    public const string Issuer = $"https://cognito-idp.{Region}.amazonaws.com/{UserPoolId}";
    public const string Sub = "11111111-1111-1111-1111-111111111111";
    public const string Username = "test-user";

    private const string SigningSecret = "n1che-service-tests-symmetric-signing-key-000000";

    public static SymmetricSecurityKey SigningKey => new(Encoding.UTF8.GetBytes(SigningSecret));

    public static string GenerateToken(string sub = Sub, string username = Username)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                ["sub"] = sub,
                ["username"] = username,
                ["client_id"] = ClientId,
                ["token_use"] = "access"
            }
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
