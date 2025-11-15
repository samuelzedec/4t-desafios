using System.Net;
using FluentAssertions;
using Health.Application.Features.HealthPlans.Commands.DeleteHealthPlanCommand;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.HealthPlans.Commands;

public sealed class DeleteHealthPlanCommandHandlerTests : BaseTest
{
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteHealthPlanCommandHandler _handlerTests;
    private readonly CancellationTokenSource _cancellationToken;

    public DeleteHealthPlanCommandHandlerTests()
    {
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);
        _handlerTests = new DeleteHealthPlanCommandHandler(_unitOfWorkMock.Object);

        _cancellationToken = new CancellationTokenSource();
        _cancellationToken.Cancel();
    }

    [Fact(DisplayName = "Deve marcar o plano de saúde como removido do banco")]
    public async Task DeleteHealthPlan_WhenCalled_ShouldMarkAsRemovedInDatabase()
    {
        // Arrange
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .Setup(h => h.Delete(It.IsAny<HealthPlan>()))
            .Callback<HealthPlan>(plan => plan.DeleteEntity());

        _unitOfWorkMock
            .Setup(h => h.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handlerTests.Handle(new DeleteHealthPlanCommand(healthPlan.Id), _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        healthPlan.DeletedAt.Should().NotBeNull();
        healthPlan.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Delete(It.IsAny<HealthPlan>()), Times.Once);

        _unitOfWorkMock
            .Verify(h => h.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha quando o plano de saúde não for encontrado")]
    public async Task GetHealthPlan_WhenNotFound_ShouldReturnFailure()
    {
        // Arrange
        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as HealthPlan);

        // Act
        var result = await _handlerTests.Handle(new DeleteHealthPlanCommand(Guid.CreateVersion7()),
            _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("Plano de saúde não encontrado.");

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Delete(It.IsAny<HealthPlan>()), Times.Never);

        _unitOfWorkMock
            .Verify(h => h.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o token for cancelado na verificação de existência")]
    public async Task GetHealthPlan_WhenCancellationTokenIsCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () =>
            await _handlerTests.Handle(new DeleteHealthPlanCommand(Guid.NewGuid()), _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Delete(It.IsAny<HealthPlan>()), Times.Never);

        _unitOfWorkMock
            .Verify(h => h.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o token for cancelado ao salvar as mudanças")]
    public async Task DeleteHealthPlan_WhenCancellationTokenIsCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .Setup(h => h.Delete(It.IsAny<HealthPlan>()))
            .Callback<HealthPlan>(plan => plan.DeleteEntity());

        _unitOfWorkMock
            .Setup(h => h.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () =>
            await _handlerTests.Handle(new DeleteHealthPlanCommand(Guid.NewGuid()), _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();
        healthPlan.DeletedAt.Should().NotBeNull();
        healthPlan.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Delete(It.IsAny<HealthPlan>()), Times.Once);

        _unitOfWorkMock
            .Verify(h => h.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static HealthPlan CreateFaker()
        => CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();
}