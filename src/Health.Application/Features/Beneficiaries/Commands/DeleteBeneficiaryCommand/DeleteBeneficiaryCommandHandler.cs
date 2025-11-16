using System.Net;
using Health.Application.Abstractions.Commands;
using Health.Application.Common;
using Health.Domain.Repositories;

namespace Health.Application.Features.Beneficiaries.Commands.DeleteBeneficiaryCommand;

internal sealed class DeleteBeneficiaryCommandHandler(
    IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteBeneficiaryCommand>
{
    public async Task<Result<EmptyResult>> Handle(
        DeleteBeneficiaryCommand request,
        CancellationToken cancellationToken)
    {
        var beneficiary = await unitOfWork.Beneficiaries
            .GetByIdAsync(request.Id, cancellationToken);

        if (beneficiary is null)
            return Result.Failure("Beneficiário não encontrado.", HttpStatusCode.NotFound);

        unitOfWork.Beneficiaries.Delete(beneficiary);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}