using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.Cpf.Exceptions;

public sealed class InvalidCpfLengthException(string message)
    : DomainException(message);