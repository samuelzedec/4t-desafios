using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.Name.Exceptions;

public sealed class EmptyNameException(string message)
    : DomainException(message);