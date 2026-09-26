namespace N1che.Domain.Exceptions;

/// <summary>The exception thrown when a call to Google Routes fails.</summary>
public sealed class GoogleRoutesException() : Exception(DefaultMessage)
{
    private const string DefaultMessage = "Google Routes is currently unavailable.";
}
