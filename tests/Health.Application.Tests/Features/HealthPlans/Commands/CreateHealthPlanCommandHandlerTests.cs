using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using Health.Application.Features.HealthPlans.Commands.CreateHealthPlanCommand;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.HealthPlans.Commands;

public sealed class CreateHealthPlanCommandHandlerTests : BaseTest
{
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateHealthPlanCommandHandler _handlerTests;
    private readonly CancellationTokenSource _cancellationToken;

    public CreateHealthPlanCommandHandlerTests()
    {
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);
        _handlerTests = new CreateHealthPlanCommandHandler(_unitOfWorkMock.Object);

        _cancellationToken = new CancellationTokenSource();
        _cancellationToken.Cancel();
    }

    [Fact(DisplayName = "Deve criar o plano de saúde com sucesso")]
    public async Task CreateHealthPlan_WhenValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handlerTests.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.Name.Should().Be(command.Name);
        result.Value.AnsCode.Should().Contain(command.AnsRegistrationCode);
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        _healthPlanRepositoryMock
            .Verify(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar conflito ao tentar criar um plano de saúde com nome já existente")]
    public async Task CreateHealthPlan_WhenNameExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handlerTests.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("O nome do plano de saúde já está em uso.");

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve retornar conflito ao tentar criar um plano de saúde com código ANS já existente")]
    public async Task CreateHealthPlan_WhenAnsCodeExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .SetupSequence(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        // Act
        var result = await _handlerTests.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("O código ANS já está em uso.");

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        _healthPlanRepositoryMock
            .Verify(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado na verificação de existência")]
    public async Task CheckExistence_WhenTokenIsCancelled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado na criação do plano de saúde")]
    public async Task CreateHealthPlan_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        _healthPlanRepositoryMock
            .Verify(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado ao salvar plano de saúde no banco")]
    public async Task SaveHealthPlan_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()), Times.Exactly(2));

        _healthPlanRepositoryMock
            .Verify(h => h.CreateAsync(It.IsAny<HealthPlan>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static HealthPlan CreateFaker()
        => CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();

    private static CreateHealthPlanCommand CreateCommand()
        => CreateFaker<CreateHealthPlanCommand>()
            .CustomInstantiator(f => new CreateHealthPlanCommand(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();
}