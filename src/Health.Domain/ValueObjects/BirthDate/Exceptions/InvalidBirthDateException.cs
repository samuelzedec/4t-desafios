using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.BirthDate.Exceptions;

public sealed class InvalidBirthDateException(string message)
    : DomainException(message);