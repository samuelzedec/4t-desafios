using Health.Api.Extensions;
using Health.Application.Common;
using Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Health.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/beneficiaries")]
[Produces("application/json")]
public sealed class BeneficiaryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<CreateBeneficiaryCommandResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBeneficiaryAsync(
        [FromBody] CreateBeneficiaryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResponse($"api/beneficiaries/{result.Value?.Id}");
    }
}