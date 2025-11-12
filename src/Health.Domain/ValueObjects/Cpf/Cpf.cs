using Health.Domain.ValueObjects.Cpf.Exceptions;

namespace Health.Domain.ValueObjects.Cpf;

public sealed record Cpf : ValueObject
{
    #region Constants

    public const int CpfLength = 11;

    #endregion

    #region Properties

    public string Value { get; init; }

    #endregion

    #region Constructors

    private Cpf(string value)
        => Value = value;

    #endregion

    #region Factory Methods

    public static Cpf Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new EmptyCpfException("O CPF não pode estar vazio.");

        var sanitizedValue = Sanitize(value);

        IsValid(sanitizedValue);
        return new Cpf(sanitizedValue);
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Determina se o valor do CPF fornecido é válido ou não. <br/>
    /// Fonte: <see href="https://www.macoratti.net/alg_cpf.htm"/>.
    /// </summary>
    /// <param name="cpf">O valor do CPF a ser validado, representado como uma string.</param>
    /// <exception cref="InvalidCpfException">
    /// Lançada quando o valor do CPF é inválido devido a várias condições:
    /// - CPF contém apenas dígitos idênticos.
    /// - CPF falha na validação da soma de verificação dos dígitos verificadores.
    /// </exception>
    public static void IsValid(string cpf)
    {
        if (cpf.Length != CpfLength)
            throw new InvalidCpfLengthException("O CPF deve conter 11 caracteres.");

        if (cpf.Distinct().Count() == 1)
            throw new InvalidCpfException("CPF não pode estar somente com números iguais.");

        int[] digits = [.. cpf.Select(c => c - '0')];

        int sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (digits[9] != digit1)
            throw new InvalidCpfException("CPF está inválido.");

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        if (digits[10] != digit2)
            throw new InvalidCpfException("CPF está inválido.");
    }

    public static string Sanitize(string document)
        => new([..document.Where(char.IsDigit)]);

    public static string Format(string document)
        => $"{document[..3]}.{document[3..6]}.{document[6..9]}-{document[9..11]}";

    #endregion

    #region Operators

    public static implicit operator string(Cpf cpf)
        => cpf.ToString();

    #endregion
    
    #region Overrides

    public override string ToString()
        => Format(Value);

    #endregion
}