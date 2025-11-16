using System.Net;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Application.Features.Beneficiaries.Queries.GetBeneficiaryByIdQuery;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.Beneficiaries.Queries;

public sealed class GetBeneficiaryByIdQueryHandlerTests : BaseTest
{
    private readonly Mock<IBeneficiaryRepository> _beneficiaryRepositoryMock;
    private readonly GetBeneficiaryByIdQueryHandler _handler;
    private readonly Beneficiary _beneficiary;
    private readonly HealthPlan _healthPlan;

    public GetBeneficiaryByIdQueryHandlerTests()
    {
        Mock<IUnitOfWork> unitOfWorkMock = new();
        _beneficiaryRepositoryMock = new Mock<IBeneficiaryRepository>();
        unitOfWorkMock.Setup(u => u.Beneficiaries).Returns(_beneficiaryRepositoryMock.Object);
        _handler = new GetBeneficiaryByIdQueryHandler(unitOfWorkMock.Object);

        _healthPlan = CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();

        _beneficiary = CreateFaker<Beneficiary>()
            .CustomInstantiator(f => Beneficiary.Create(
                f.Person.FullName,
                f.Person.Cpf(false),
                DateOnly.FromDateTime(f.Person.DateOfBirth.Date),
                _healthPlan.Id
            ))
            .Generate();

        var healthPlanProperty = typeof(Beneficiary).GetProperty("HealthPlan");
        healthPlanProperty!.SetValue(_beneficiary, _healthPlan);
    }

    [Fact(DisplayName = "Deve retornar o beneficiário com o id correto")]
    public async Task GetBeneficiaryById_WhenIdIsValid_ShouldReturnCorrectBeneficiary()
    {
        // Arrange
        var query = new GetBeneficiaryByIdQuery(_beneficiary.Id);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdWithHealthPlanAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_beneficiary);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_beneficiary.Id);
        result.Value.FullName.Should().Be(_beneficiary.FullName.Value);
        result.Value.Cpf.Should().Be(_beneficiary.Cpf);
        result.Value.BirthDate.Should().Be(_beneficiary.BirthDate);
        result.Value.HealthPlan.Should().NotBeNull();
        result.Value.HealthPlan.Name.Should().Be(_healthPlan.Name);
        result.Value.HealthPlan.AnsRegistrationCode.Should().Be(_healthPlan.AnsRegistrationCode);
        result.Value.CreationDate.Should().Be(_beneficiary.CreatedAt);
        result.Value.ModificationDate.Should().Be(_beneficiary.UpdatedAt);

        _beneficiaryRepositoryMock.Verify(
            b => b.GetByIdWithHealthPlanAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha ao não encontrar o beneficiário")]
    public async Task GetBeneficiaryById_WhenIdIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var query = new GetBeneficiaryByIdQuery(Guid.Empty);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdWithHealthPlanAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Beneficiary);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("Beneficiário não encontrado.");
        result.Value.Should().BeNull();

        _beneficiaryRepositoryMock.Verify(
            b => b.GetByIdWithHealthPlanAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o token está cancelado na busca do beneficiário")]
    public async Task GetBeneficiaryById_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var query = new GetBeneficiaryByIdQuery(Guid.Empty);
        var cancellationToken = new CancellationTokenSource();
        await cancellationToken.CancelAsync();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdWithHealthPlanAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(query, cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock.Verify(
            b => b.GetByIdWithHealthPlanAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

