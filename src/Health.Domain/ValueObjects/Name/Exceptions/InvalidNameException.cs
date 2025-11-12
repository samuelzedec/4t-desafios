using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.Name.Exceptions;

public sealed class InvalidNameException(string message)
    : DomainException(message);