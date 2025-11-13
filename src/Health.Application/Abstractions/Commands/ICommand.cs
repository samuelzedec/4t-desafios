using Health.Application.Abstractions.Common;
using MediatR;

namespace Health.Application.Abstractions.Commands;

/// <summary>
/// Defines a command interface extending the MediatR IRequest interface for processing
/// commands in an application. Commands represent actions or operations to be executed
/// and typically modify application state.
/// </summary>
/// <remarks>
/// ICommand interfaces can optionally specify a response type. If no response type is
/// needed, the result will default to <see cref="Result&lt;EmptyResult&gt;"/>. If a response
/// is required, the generic ICommand interface should be used with the desired response
/// type as the type parameter.
/// The Result wrapper is used to encapsulate the outcome of a command, including success
/// or failure status, errors, and any relevant data.
/// </remarks>
public interface ICommand : IRequest<Result<EmptyResult>>;

/// <summary>
/// Representa uma interface de comando projetada para processar operações dentro da aplicação,
/// tipicamente modificando o estado da aplicação. Estende a interface IRequest do MediatR e encapsula o resultado
/// em um <see cref="Result&lt;TResponse&gt;"/> padronizado com o tipo de resposta especificado.
/// </summary>
/// <remarks>
/// Esta interface fornece um mecanismo para estruturar ações da aplicação de maneira consistente,
/// permitindo que comandos sejam tratados de forma desacoplada e orquestrada. Para comandos que não retornam dados,
/// utilize a variante não genérica desta interface que retorna <see cref="Result&lt;EmptyResult&gt;"/>.
/// </remarks>
/// <typeparam name="TResponse">O tipo de resposta que o comando retornará encapsulado no Result.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> where TResponse : class;