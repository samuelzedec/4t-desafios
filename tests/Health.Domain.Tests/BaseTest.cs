using Bogus;

namespace Health.Domain.Tests;

/// <summary>
/// Fornece uma classe base para testes, incluindo métodos utilitários e propriedades para geração de dados de teste.
/// </summary>
public abstract class BaseTest
{
    /// <summary>
    /// Uma instância protegida da classe Faker da biblioteca Bogus, usada para gerar dados de teste
    /// aleatórios nas classes de teste derivadas.
    /// </summary>
    protected readonly Faker _faker = new();

    /// <summary>
    /// Cria e retorna uma instância da classe Faker para o tipo especificado, permitindo a geração de dados de teste aleatórios.
    /// </summary>
    /// <typeparam name="T">O tipo para o qual a instância do Faker irá gerar dados de teste. Deve ser um tipo de referência.</typeparam>
    /// <returns>Uma instância do Faker configurada para gerar dados para o tipo especificado.</returns>
    protected static Faker<T> CreateFaker<T>() where T : class
        => new();
}