using Health.Application.Abstractions.Common;
using MediatR;

namespace Health.Application.Abstractions.Queries;

/// <summary>
/// Representa um manipulador para processar consultas dentro da aplicação.
/// Esta interface define o contrato para implementar a lógica de tratamento de consultas,
/// aproveitando o MediatR para padrões de requisição-resposta e encapsulando o resultado
/// em um tipo de resposta estruturado.
/// </summary>
/// <typeparam name="TQuery">
/// O tipo da consulta sendo tratada, que deve implementar <see cref="IQuery&lt;TResponse&gt;"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// O tipo da resposta retornada após a execução bem-sucedida da consulta,
/// que deve implementar <see cref="IQueryResponse"/>.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TResponse : class, IQueryResponse
    where TQuery : IQuery<TResponse>;