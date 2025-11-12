using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.Cpf.Exceptions;

public sealed class InvalidCpfException(string message)
    : DomainException(message);