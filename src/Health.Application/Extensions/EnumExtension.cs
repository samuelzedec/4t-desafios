using System.ComponentModel;
using System.Reflection;

namespace Health.Application.Extensions;

public static class EnumExtension
{
    /// <summary>
    /// Recupera o atributo de descrição associado ao valor enum especificado.
    /// Se nenhum atributo de descrição for encontrado, o nome do valor enum é retornado como uma string.
    /// </summary>
    /// <param name="value">O valor enum cuja descrição deve ser recuperada.</param>
    /// <returns>A string de descrição associada ao valor enum, ou o nome do valor enum se nenhum atributo de descrição estiver presente.</returns>
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? value.ToString();
    }
}