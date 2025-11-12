using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.Name.Exceptions;

public sealed class InvalidNameLengthException(string message)
    : DomainException(message);