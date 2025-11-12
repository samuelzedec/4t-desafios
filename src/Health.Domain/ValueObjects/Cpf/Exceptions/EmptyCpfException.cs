using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.Cpf.Exceptions;

public sealed class EmptyCpfException(string message)
    : DomainException(message);