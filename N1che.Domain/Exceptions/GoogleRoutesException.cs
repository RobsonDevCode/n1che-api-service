namespace N1che.Domain.Exceptions;

/// <summary>
/// Google Routes could not answer. What it actually said is logged at the call site, so the response
/// says nothing about it.
/// </summary>
public sealed class GoogleRoutesException() : Exception(DefaultMessage)
{
    private const string DefaultMessage = "Google Routes is currently unavailable.";
}
