using FluentAssertions;
using Health.Domain.Exceptions;
using Health.Domain.ValueObjects.Name;
using Health.Domain.ValueObjects.Name.Exceptions;
using Xunit;

namespace Health.Domain.Tests.ValueObjects;

public sealed class NameTests : BaseTest
{
    [Fact(DisplayName = "Deve criar a instância com o nome válido")]
    public void Create_WhenNameIsValid_ShouldCreateNameValueObject()
    {
        // Arrange
        var name = _faker.Person.FullName;

        // Act
        var result = Name.Create(name);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(name);
    }

    [Theory(DisplayName = "Deve lançar EmptyNameException quando o nome for vazio")]
    [InlineData("     ")]
    [InlineData("")]
    [InlineData("\n")]
    public void Create_WhenNameIsEmpty_ShouldThrowEmptyNameException(string value)
    {
        // Arrange & Act
        var result = () => Name.Create(value);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<EmptyNameException>()
            .WithMessage("O nome não pode estar vazio.");
    }

    [Fact(DisplayName = "Deve lançar InvalidNameLengthException quando o nome ultrapassar o limite de  caracteres")]
    public void Create_WhenNameIsLongerThan255Characters_ShouldThrowInvalidNameLengthException()
    {
        // Arrange
        var name = _faker.Random.AlphaNumeric(256);

        // Act
        var result = () => Name.Create(name);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidNameLengthException>()
            .WithMessage($"O nome deve conter entre {Name.MinLength} e {Name.MaxLength} caracteres.");
    }

    [Fact(DisplayName = "Deve lançar InvalidNameLengthException quando o nome tiver abaixo do limite de caracteres")]
    public void Create_WhenNameLengthIsBelowMinimum_ShouldThrowInvalidNameLengthException()
    {
        // Arrange
        var name = _faker.Random.AlphaNumeric(4);

        // Act
        var result = () => Name.Create(name);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidNameLengthException>()
            .WithMessage($"O nome deve conter entre {Name.MinLength} e {Name.MaxLength} caracteres.");
    }

    [Theory(DisplayName = "Deve lançar InvalidNameException qunado o nome tiver caracteres inválidos")]
    [InlineData("N0me com número")]
    [InlineData("Nome_com_sublinhado")]
    [InlineData("Nome@com#caracteres$especiais!")]
    public void Create_WhenNameContainsInvalidCharacters_ShouldThrowInvalidNameException(string value)
    {
        // Arrange & Act
        var result = () => Name.Create(value);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidNameException>()
            .WithMessage("O nome contém caracteres inválidos.");
    }

    [Fact(DisplayName = "Deve retornar o nome quando usado o implicit operator")]
    public void ImplicitOperator_ShouldReturnName()
    {
        // Arrange
        var name = _faker.Person.FullName;

        // Act
        string nameValueObject = Name.Create(name);

        // Assert
        nameValueObject.Should().Be(name);
    }
}