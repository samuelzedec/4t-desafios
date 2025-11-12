namespace Health.Domain.ValueObjects;

/// <summary>
/// Representa uma classe base abstrata para um objeto de valor na camada de domínio.
/// Um objeto de valor é imutável e definido apenas pelos valores de suas propriedades,
/// ao invés de uma identidade. Esta classe fornece a base para implementar conceitos 
/// de negócio que requerem igualdade baseada em valores.
/// </summary>
/// <remarks>
/// A classe ValueObject é usada para encapsular e impor a lógica de igualdade
/// para objetos de valor. Classes derivadas devem garantir que as comparações 
/// de igualdade definidas sejam consistentes com as propriedades que constituem 
/// o objeto de valor.
/// </remarks>
public abstract record ValueObject;