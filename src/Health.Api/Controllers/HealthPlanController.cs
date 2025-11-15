using Health.Api.Extensions;
using Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}