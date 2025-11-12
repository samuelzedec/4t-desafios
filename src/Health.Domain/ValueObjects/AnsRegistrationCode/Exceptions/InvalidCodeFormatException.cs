using Health.Domain.Exceptions;

namespace Health.Domain.ValueObjects.AnsRegistrationCode.Exceptions;

public sealed class InvalidCodeFormatException(string message) 
    : DomainException(message);
