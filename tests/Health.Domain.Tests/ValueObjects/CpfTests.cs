using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Domain.Exceptions;
using Health.Domain.ValueObjects.Cpf;
using Health.Domain.ValueObjects.Cpf.Exceptions;
using Xunit;

namespace Health.Domain.Tests.ValueObjects;

public sealed class CpfTests : BaseTest
{
    [Fact(DisplayName = "Deve criar a instância quando o CPF for válido")]
    public void Create_WhenCpfIsValid_ShouldCreateInstance()
    {
        // Arrange
        var cpf = _faker.Person.Cpf(false);

        // Act
        var result = Cpf.Create(cpf);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(cpf);
    }

    [Theory(DisplayName = "Deve lançar EmptyCpfException quando o CPF for nulo ou vazio")]
    [InlineData("")]
    [InlineData("\n\n")]
    [InlineData("     ")]
    public void Create_WhenCpfIsNullOrEmpty_ShouldThrowEmptyCpfException(string value)
    {
        // Arrange & Act
        var result = () => Cpf.Create(value);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<EmptyCpfException>().WithMessage("O CPF não pode estar vazio.");
    }

    [Theory(DisplayName = "Deve lançar InvalidCpfLengthException quando CPF não o tamanho correto")]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("12345678")]
    [InlineData("1234567890232132")]
    public void Create_WhenCpfLengthIsInvalid_ShouldThrowInvalidCpfLengthException(string value)
    {
        // Arrange & Act
        var result = () => Cpf.Create(value);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidCpfLengthException>().WithMessage("O CPF deve conter 11 caracteres.");
    }

    [Theory(DisplayName = "Deve lançar InvalidCpfException quando o CPF for somente números iguais")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    public void Create_WhenCpfIsOnlyNumbers_ShouldThrowInvalidCpfException(string value)
    {
        // Arrange & Act
        var result = () => Cpf.Create(value);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidCpfException>()
            .WithMessage("CPF não pode estar somente com números iguais.");
    }

    [Theory(DisplayName = "Deve lançar InvalidCpfException quando o CPF for inválido")]
    [InlineData("12345678900")]
    [InlineData("987.664.321-00")]
    public void Create_WhenCpfIsInvalid_ShouldThrowInvalidCpfException(string value)
    {
        // Arrange & Act
        var result = () => Cpf.Create(value);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidCpfException>()
            .WithMessage("CPF está inválido.");
    }

    [Fact(DisplayName = "Deve sanitizar o CPF removendo caracteres especiais")]
    public void Sanitize_ShouldRemoveSpecialCharacters()
    {
        // Arrange
        var cpf = _faker.Person.Cpf();

        // Act
        var result = Cpf.Sanitize(cpf);

        // Assert
        result.Should().BeEquivalentTo(cpf.Replace(".", "").Replace("-", ""));
    }

    [Fact(DisplayName = "Deve formatar o CPF com o padrão brasileiro")]
    public void Format_WhenCalled_ShouldReturnCpfInBrazilianPattern()
    {
        // Arrange
        const string cpf = "12345678901";
        const string cpfExpected = "123.456.789-01";

        // Act
        var result = Cpf.Format(cpf);

        // Assert
        result.Should().Be(cpfExpected);
    }

    [Fact(DisplayName = "Deve formatar o CPF para o padrão brasileiro usando o implicit operator")]
    public void ImplicitOperator_WhenUsed_ShouldFormatCpfInBrazilianPattern()
    {
        // Arrange
        const string cpf = "12345678909";
        const string cpfExpected = "123.456.789-09";

        // Act
        string result = Cpf.Create(cpf);

        // Assert
        result.Should().Be(cpfExpected);
    }
}