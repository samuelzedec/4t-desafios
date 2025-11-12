namespace Health.Domain.Exceptions;

/// <summary>
/// Representa o tipo base de exceção para erros específicos do domínio.
/// Esta exceção foi projetada para ser a base de outras exceções do domínio,
/// encapsulando erros que ocorrem dentro da camada de domínio da aplicação.
/// </summary>
public abstract class DomainException(string message)
    : Exception(message);