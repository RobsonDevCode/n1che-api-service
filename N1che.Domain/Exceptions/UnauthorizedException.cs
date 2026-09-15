namespace N1che.Domain.Exceptions;

public sealed class UnauthorizedException() : Exception(DefaultMessage)
{
    private const string DefaultMessage = "Unauthorized";
}
