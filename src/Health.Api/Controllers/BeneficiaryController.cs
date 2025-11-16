using Health.Api.Extensions;
using Health.Api.Requests;
using Health.Application.Common;
using Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;
using Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<UpdateBeneficiaryCommandResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBeneficiaryAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateBeneficiaryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request.ToCommand(id), cancellationToken);
        return result.ToHttpResponse();
    }
}