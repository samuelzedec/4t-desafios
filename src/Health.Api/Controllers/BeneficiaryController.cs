using Health.Api.Extensions;
using Health.Api.Requests;
using Health.Application.Common;
using Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;
using Health.Application.Features.Beneficiaries.Commands.DeleteBeneficiaryCommand;
using Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;
using Health.Application.Features.Beneficiaries.Queries.GetBeneficiaryByIdQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmptyResult = Health.Application.Common.EmptyResult;

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

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result<EmptyResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBeneficiaryAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteBeneficiaryCommand(id), cancellationToken);
        return result.ToHttpResponse();
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<GetBeneficiaryByIdQueryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBeneficiaryByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBeneficiaryByIdQuery(id), cancellationToken);
        return result.ToHttpResponse();
    }
}