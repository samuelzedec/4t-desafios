using Health.Api.Extensions;
using Health.Api.Requests;
using Health.Application.Common;
using Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;
using Health.Application.Features.HealthPlans.Commands.DeleteHealthPlanCommand;
using Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmptyResult = Health.Application.Common.EmptyResult;

namespace Health.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/health-plans")]
[Produces("application/json")]
public sealed class HealthPlanController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<CreateHealthPlanCommandResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateHealthPlanAsync(
        [FromBody] CreateHealthPlanCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResponse($"api/health-plans/{result.Value?.Id}");
    }
    
    [HttpPost("{id:guid}")]
    [ProducesResponseType(typeof(Result<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteHealthPlanAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteHealthPlanCommand(id), cancellationToken);
        return result.ToHttpResponse();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<UpdateHealthPlanCommand>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateHealthPlanAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateHealthPlanRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result.ToHttpResponse();
    }
}