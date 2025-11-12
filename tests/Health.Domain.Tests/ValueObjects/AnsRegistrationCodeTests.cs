using FluentAssertions;
using Health.Domain.Exceptions;
using Health.Domain.ValueObjects.AnsRegistrationCode;
using Health.Domain.ValueObjects.AnsRegistrationCode.Exceptions;
using Xunit;

namespace Health.Domain.Tests.ValueObjects;

public sealed class AnsRegistrationCodeTests : BaseTest
{
    [Fact(DisplayName = "Deve criar o código de registro ANS quando o valor é válido")]
    public void Create_WhenValueIsValid_ShouldCreateAnsRegistrationCode()
    {
        // Arrange
        var code = _faker.Random.Number(100_000, 999_999).ToString();

        // Act
        var result = AnsRegistrationCode.Create(code);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(code);
    }

    [Fact(DisplayName = "Deve lançar InvalidCodeFormatException quando o código de registro ANS possui letras")]
    public void Create_WhenCodeContainsLetters_ShouldThrowInvalidCodeFormatException()
    {
        // Arrange
        var code = _faker.Random.AlphaNumeric(6);

        // Act
        var act = () => AnsRegistrationCode.Create(code);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCodeFormatException>()
            .WithMessage("O código ANS deve conter somente números.");
    }

    [Fact(DisplayName =
        "Deve lançar InvalidCodeFormatException quando o código de registro ANS possui começa com zero")]
    public void Create_WhenCodeStartsWithZero_ShouldThrowInvalidCodeFormatException()
    {
        // Arrange
        var code = $"0{_faker.Random.Number(10_000, 99_999)}";

        // Act
        var act = () => AnsRegistrationCode.Create(code);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCodeFormatException>()
            .WithMessage("O código ANS não pode começar com zero.");
    }

    [Fact(DisplayName = "Deve lançar InvalidCodeLengthException quando o código de registro ANS não possui 6 dígitos")]
    public void Create_WhenCodeIsNot6Digits_ShouldThrowInvalidCodeLengthException()
    {
        // Arrange
        var code = _faker.Random.Number(10_000, 99_999).ToString();

        // Act
        var act = () => AnsRegistrationCode.Create(code);

        // Assert
        act.Should().Throw<DomainException>();
        act.Should().ThrowExactly<InvalidCodeLengthException>()
            .WithMessage("O código ANS deve conter exatamente 6 dígitos.");
    }

    [Fact(DisplayName = "Deve retornar o código de registro ANS com a sigla ANS ao chamar ToString")]
    public void ToString_ShouldReturnAnsRegistrationCodeWithAnsPrefix()
    {
        // Arrange
        var code = _faker.Random.Number(100_000, 999_999).ToString();
        var ansRegistrationCode = AnsRegistrationCode.Create(code);
        
        // Act
        var result = ansRegistrationCode.ToString();
        
        // Assert
        result.Should().Be($"ANS-{code}");
    }

    [Fact(DisplayName = "Deve retornar código de registro ANS com a sigla ANS ao chamar implicit operator")]
    public void ImplicitOperator_ShouldReturnAnsRegistrationCodeWithAnsPrefix()
    {
        // Arrange
        var code = _faker.Random.Number(100_000, 999_999).ToString();
        var ansRegistrationCode = AnsRegistrationCode.Create(code);
        
        // Act
        string result = ansRegistrationCode;
        
        // Assert
        result.Should().Be($"ANS-{code}");
    }
}