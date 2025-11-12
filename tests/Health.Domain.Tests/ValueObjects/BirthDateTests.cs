using FluentAssertions;
using Health.Domain.Exceptions;
using Health.Domain.ValueObjects.BirthDate;
using Health.Domain.ValueObjects.BirthDate.Exceptions;
using Xunit;

namespace Health.Domain.Tests.ValueObjects;

public sealed class BirthDateTests : BaseTest
{
    [Fact(DisplayName = "Deve validar a data de nascimento com os dados corretos")]
    public void ValidateBirthDate_WhenDataIsValid_ShouldSucceed()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        // Act
        var result = BirthDate.Create(birthDateValue);

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().Be(birthDateValue);
    }

    [Fact(DisplayName = "Deve lançar InvalidBirthDateException quando a data for futura")]
    public void Create_WhenBirthDateIsInTheFuture_ShouldThrowInvalidBirthDateException()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(30));

        // Act
        var result = () => BirthDate.Create(birthDateValue);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidBirthDateException>()
            .WithMessage("A data de nascimento não pode ser futura.");
    }

    [Fact(DisplayName = "Deve lançar InvalidAgeException quando a idade for que a idade limite")]
    public void Create_WhenAgeIsGreaterThanLimit_ShouldThrowInvalidAgeException()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-151));

        // Act
        var result = () => BirthDate.Create(birthDateValue);

        // Assert
        result.Should().Throw<DomainException>();
        result.Should().ThrowExactly<InvalidAgeException>()
            .WithMessage($"A idade máxima permitida é {BirthDate.MaximumAge} anos");
    }

    [Fact(DisplayName = "Deve converter para DateOnly quando usado o implicit operator")]
    public void ImplicitOperator_WhenUsed_ShouldConvertToDateOnly()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        // Act
        DateOnly result = BirthDate.Create(birthDateValue);

        // Assert
        result.Should().Be(birthDateValue);
    }

    [Fact(DisplayName = "Deve converter para a idade quando usado o implicit operator")]
    public void ImplicitOperator_WhenUsed_ShouldConvertToAge()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        // Act
        int result = BirthDate.Create(birthDateValue);

        // Assert
        result.Should().Be(30);
    }

    [Fact(DisplayName = "Deve converter a data para o formato brasileiro")]
    public void Format_WhenCalled_ShouldConvertDateToBrazilianFormat()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        // Act
        string result = BirthDate.Create(birthDateValue);

        // Assert
        result.Should().Be(birthDateValue.ToString("dd/MM/yyyy"));
    }

    [Fact(DisplayName = "Deve retornar a idade ao chamar o método CalculateAge")]
    public void CalculateAge_WhenCalled_ShouldReturnAge()
    {
        // Arrange
        var birthDateValue = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-30));

        // Act
        int result = BirthDate.CalculateAge(birthDateValue);

        // Assert
        result.Should().Be(30);
    }
}