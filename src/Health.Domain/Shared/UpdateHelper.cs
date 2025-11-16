namespace Health.Domain.Shared;

public static class UpdateHelper
{
    /// <summary>
    /// Determina se uma atualização deve ser ignorada com base na comparação entre o valor atual e o novo valor genérico.
    /// </summary>
    /// <typeparam name="T">O tipo do valor a ser comparado.</typeparam>
    /// <param name="currentValue">O valor atual da propriedade.</param>
    /// <param name="newValue">O novo valor proposto para atualização.</param>
    /// <returns>
    /// Retorna true se o novo valor for igual ao valor atual; caso contrário, retorna false.
    /// </returns>
    public static bool ShouldSkipUpdate<T>(T currentValue, T? newValue)
    {
        if (typeof(T) != typeof(string))
            return newValue is null || EqualityComparer<T>.Default.Equals(currentValue, newValue);

        var currentValueString = currentValue as string;
        var newValueString = newValue as string;

        return string.IsNullOrWhiteSpace(newValueString)
               || string.Equals(currentValueString, newValueString, StringComparison.OrdinalIgnoreCase);
    }
}