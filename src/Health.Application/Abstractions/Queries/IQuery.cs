using Health.Application.Common;
using MediatR;

namespace Health.Application.Abstractions.Queries;

/// <summary>
/// Representa uma interface de consulta que define um padrão de requisição-resposta
/// resultando em uma resposta encapsulada do tipo <c>Result&lt;TQuery&gt;</c>.
/// </summary>
/// <typeparam name="TQuery">
/// O tipo dos dados de resposta retornados pela execução da consulta, encapsulado em um <c>Result</c>.
/// </typeparam>
public interface IQuery<TQuery> : IRequest<Result<TQuery>>;