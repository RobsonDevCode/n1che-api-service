namespace N1che.Domain.Exceptions;

public sealed class BadRequestException(string message) : Exception(message);
