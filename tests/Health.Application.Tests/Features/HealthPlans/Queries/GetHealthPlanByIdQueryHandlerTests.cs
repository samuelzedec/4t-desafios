using System.Net;
using FluentAssertions;
using Health.Application.Features.HealthPlans.Queries.GetHealthPlanByIdQuery;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.HealthPlans.Queries;

public sealed class GetHealthPlanByIdQueryHandlerTests : BaseTest
{
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly GetHealthPlanByIdQueryHandler _handler;
    private readonly HealthPlan _healthPlan;

    public GetHealthPlanByIdQueryHandlerTests()
    {
        Mock<IUnitOfWork> unitOfWorkMock = new();
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);
        _handler = new GetHealthPlanByIdQueryHandler(unitOfWorkMock.Object);

        _healthPlan = CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();
    }

    [Fact(DisplayName = "Deve retornar o plano de saúde com o id correto")]
    public async Task GetHealthPlanById_WhenIdIsValid_ShouldReturnCorrectHealthPlan()
    {
        // Arrange
        var query = new GetHealthPlanByIdQuery(_healthPlan.Id);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdWithBeneficiariesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_healthPlan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Value.Should().NotBeNull();
        result.Value.AnsRegistrationCode.Should().Be(_healthPlan.AnsRegistrationCode);
        result.Value.Name.Should().Be(_healthPlan.Name);
        result.Value.Beneficiaries.Should().HaveCount(0);
        result.Value.CreationDate.Should().Be(_healthPlan.CreatedAt);
        result.Value.ModificationDate.Should().BeNull();

        _healthPlanRepositoryMock.Verify(
            h => h.GetByIdWithBeneficiariesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha ao não encontrar o plano de saúde")]
    public async Task GetHealthPlanById_WhenIdIsInvalid_ShouldReturnFailure()
    {
        var query = new GetHealthPlanByIdQuery(Guid.Empty);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdWithBeneficiariesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as HealthPlan);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Value.Should().BeNull();

        _healthPlanRepositoryMock.Verify(
            h => h.GetByIdWithBeneficiariesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o token está cancelado na busca do plano de saúde")]
    public async Task GetHealthPlanById_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var query = new GetHealthPlanByIdQuery(Guid.Empty);
        var cancellationToken = new CancellationTokenSource();
        await cancellationToken.CancelAsync();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdWithBeneficiariesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(query, cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _healthPlanRepositoryMock.Verify(
            h => h.GetByIdWithBeneficiariesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}