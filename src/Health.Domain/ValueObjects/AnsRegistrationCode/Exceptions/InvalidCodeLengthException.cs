using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.AnsRegistrationCode.Exceptions;

public sealed class InvalidCodeLengthException(string message) 
    : DomainException(message);