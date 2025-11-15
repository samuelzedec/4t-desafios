using Health.Api.Extensions;
using Health.Application.Common;
using Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;
using Health.Application.Features.HealthPlans.Commands.DeleteHealthPlanCommand;
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
    [ProducesResponseType(typeof(CreateHealthPlanCommandResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateHealthPlanAsync(
        [FromBody] CreateHealthPlanCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResponse($"api/health-plans/{result.Value?.Id}");
    }
    
    [HttpPost("{healthPlanId:guid}")]
    [ProducesResponseType(typeof(Result<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteHealthPlanAsync(
        [FromRoute] Guid healthPlanId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteHealthPlanCommand(healthPlanId), cancellationToken);
        return result.ToHttpResponse();
    }
}