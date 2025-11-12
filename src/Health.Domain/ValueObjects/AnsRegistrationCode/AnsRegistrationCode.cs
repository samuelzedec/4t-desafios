using Health.Domain.ValueObjects.AnsRegistrationCode.Exceptions;

namespace Health.Domain.ValueObjects.AnsRegistrationCode;

public sealed record AnsRegistrationCode : ValueObject
{
    #region Constants

    public const int CodeLength = 6;
    
    #endregion

    #region Properties

    public string Value { get; init; }

    #endregion

    #region Constructors

    private AnsRegistrationCode(string value)
        => Value = value;

    #endregion

    #region Factory Methods

    public static AnsRegistrationCode Create(string value)
    {
        if (!value.All(char.IsDigit))
            throw new InvalidCodeFormatException("O código ANS deve conter somente números.");

        if (value.StartsWith('0'))
            throw new InvalidCodeFormatException("O código ANS não pode começar com zero.");

        return value.Length is not CodeLength
            ? throw new InvalidCodeLengthException("O código ANS deve conter exatamente 6 dígitos.")
            : new AnsRegistrationCode(value);
    }

    #endregion

    #region Operators

    public static implicit operator string(AnsRegistrationCode code)
        => code.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => $"ANS-{Value}";

    #endregion
}