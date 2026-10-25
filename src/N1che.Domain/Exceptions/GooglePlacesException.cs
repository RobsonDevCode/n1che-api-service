namespace N1che.Domain.Exceptions;

/// <summary>
/// Google Places could not answer. What it actually said is logged at the call site, so the response
/// says nothing about it.
/// </summary>
public sealed class GooglePlacesException() : Exception(DefaultMessage)
{
    private const string DefaultMessage = "Google Places is currently unavailable.";
}
