using Health.Application.Abstractions.Queries;

namespace Health.Application.Features.Beneficiaries.Queries.GetBeneficiaryByIdQuery;

public sealed record GetBeneficiaryByIdQuery(Guid Id) 
    : IQuery<GetBeneficiaryByIdQueryResponse>;