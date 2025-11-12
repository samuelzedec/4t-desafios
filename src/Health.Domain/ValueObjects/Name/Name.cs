using System.Text.RegularExpressions;
using Health.Domain.ValueObjects.Name.Exceptions;

namespace Health.Domain.ValueObjects.Name;

public sealed partial record Name : ValueObject
{
    #region Contants

    public const int MaxLength = 255;
    public const int MinLength = 5;
    public const string RegexPatten = "^[A-Za-zÀ-ÖØ-öø-ÿ]+([ '-][A-Za-zÀ-ÖØ-öø-ÿ]+)*$";

    #endregion

    #region Properties

    public string Value { get; init; }

    #endregion

    #region Constructors

    private Name(string value)
        => Value = value;

    #endregion

    #region Factory Methods

    public static Name Create(string value)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
            throw new EmptyNameException("O nome não pode estar vazio.");

        if (value.Length is >= MaxLength or <= MinLength)
            throw new InvalidNameLengthException($"O nome deve conter entre {MinLength} e {MaxLength} caracteres.");

        return ValidateName().IsMatch(value)
            ? new Name(value)
            : throw new InvalidNameException("O nome contém caracteres inválidos.");
    }

    #endregion

    #region Source Generator

    [GeneratedRegex(RegexPatten)]
    private static partial Regex ValidateName();

    #endregion

    #region Operators

    public static implicit operator string(Name name)
        => name.ToString();

    #endregion

    #region Overrides

    public override string ToString()
        => Value;

    #endregion
}