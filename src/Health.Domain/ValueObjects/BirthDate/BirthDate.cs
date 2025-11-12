using Health.Domain.ValueObjects.BirthDate.Exceptions;

namespace Health.Domain.ValueObjects.BirthDate;

public sealed record BirthDate : ValueObject
{
    #region Constants

    public const int MaximumAge = 150;

    #endregion

    #region Properties

    public DateOnly Value { get; init; }

    #endregion

    #region Constructors

    private BirthDate(DateOnly value)
        => Value = value;

    #endregion

    #region Factories Methods

    public static BirthDate Create(DateOnly value)
    {
        if (value > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new InvalidBirthDateException("A data de nascimento não pode ser futura.");

        return CalculateAge(value) > MaximumAge
            ? throw new InvalidAgeException($"A idade máxima permitida é {MaximumAge} anos")
            : new BirthDate(value);
    }

    #endregion

    #region Methods

    public static int CalculateAge(DateOnly birthDate)
        => DateTime.UtcNow.Year - birthDate.Year;

    #endregion

    #region Operators

    public static implicit operator DateOnly(BirthDate birthDate)
        => birthDate.Value;

    public static implicit operator int(BirthDate birthDate)
        => CalculateAge(birthDate);
    
    public static implicit operator string(BirthDate birthDate)
        => birthDate.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value.ToString("dd/MM/yyyy");

    #endregion
}