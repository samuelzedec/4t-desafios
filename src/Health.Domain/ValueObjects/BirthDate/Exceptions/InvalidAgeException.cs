using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.BirthDate.Exceptions;

public sealed class InvalidAgeException(string message)
    : DomainException(message);