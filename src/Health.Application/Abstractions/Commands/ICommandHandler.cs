using Health.Application.Common;
using MediatR;

namespace Health.Application.Abstractions.Commands;

/// <summary>
/// Define um mecanismo para manipular comandos de um tipo específico.
/// O manipulador de comando processa um comando e retorna um resultado que inclui o resultado da operação.
/// </summary>
/// <typeparam name="TCommand">
/// O tipo de comando que este manipulador processa. Deve implementar <see cref="ICommand"/>.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result<EmptyResult>>
    where TCommand : ICommand;

/// <summary>
/// Define um mecanismo para manipular comandos de um tipo específico e retornar um resultado personalizado.
/// Este manipulador processa comandos e gera um resultado que indica o sucesso ou o fracasso da operação.
/// </summary>
/// <typeparam name="TCommand">
/// O tipo de comando que este manipulador processa. Deve implementar <see cref="ICommand&lt;TResponse&gt;"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// O tipo de resposta retornado após o processamento do comando. Deve implementar <see cref="ICommandResponse"/>.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TResponse : class, ICommandResponse
    where TCommand : ICommand<TResponse>;