using System.Linq.Expressions;
using System.Net;
using FluentAssertions;
using Health.Application.Features.HealthPlans.Commands.UpdateHealthPlanCommand;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.HealthPlans.Commands;

public sealed class UpdateHealthPlanCommandHandlerTests : BaseTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly UpdateHealthPlanCommandHandler _handler;
    private readonly CancellationTokenSource _cancellationToken;

    public UpdateHealthPlanCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        _unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);

        _handler = new UpdateHealthPlanCommandHandler(_unitOfWorkMock.Object);

        _cancellationToken = new CancellationTokenSource();
        _cancellationToken.Cancel();
    }

    [Fact(DisplayName = "Deve atualizar os dois campos do plano de saúde quando os dados são válidos")]
    public async Task Update_WhenDataIsValid_ShouldUpdateHealthPlanEntity()
    {
        // Arrange
        var command = CreateCommand();
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .SetupSequence(h =>
                h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.Update(It.IsAny<HealthPlan>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(command.NewName);
        result.Value.AnsCode.Should().Contain(command.NewAnsRegistrationCode);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Update(It.IsAny<HealthPlan>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock.Verify(
            h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact(DisplayName = "Deve atualizar somente o campo nome do plano de saúde")]
    public async Task Update_WhenDataIsValid_ShouldUpdateHealthPlanNameOnly()
    {
        // Arrange
        var command = CreateCommand();
        command = command with { NewAnsRegistrationCode = string.Empty };
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .Setup(h =>
                h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.Update(It.IsAny<HealthPlan>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(command.NewName);
        result.Value.AnsCode.Should().Be(healthPlan.AnsRegistrationCode);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Update(It.IsAny<HealthPlan>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock.Verify(
            h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Deve atualizar somente o código de registro ANS do plano de saúde")]
    public async Task Update_WhenDataIsValid_ShouldUpdateHealthPlanAnsCodeOnly()
    {
        // Arrange
        var command = CreateCommand();
        command = command with { NewName = string.Empty };
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .SetupSequence(h =>
                h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.Update(It.IsAny<HealthPlan>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Name.Should().Be(healthPlan.Name);
        result.Value.AnsCode.Should().Contain(command.NewAnsRegistrationCode);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Update(It.IsAny<HealthPlan>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock.Verify(
            h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha quando o plano de saúde não for encontrado")]
    public async Task Update_WhenHealthPlanNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var command = CreateCommand();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as HealthPlan);

        // Act
        var result = await _handler.Handle(command, _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("Plano de saúde não encontrado.");

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Update(It.IsAny<HealthPlan>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact(DisplayName = "Deve retornar falha quando o novo nome já estiver em uso por outro plano de saúde")]
    public async Task Update_WhenNameExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .Setup(h =>
                h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _healthPlanRepositoryMock
            .Setup(h => h.Update(It.IsAny<HealthPlan>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("O nome do plano de saúde já está em uso.");

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Update(It.IsAny<HealthPlan>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _healthPlanRepositoryMock.Verify(
            h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha quando o código de registro já estiver em uso por outro plano de saúde")]
    public async Task Update_WhenAnsCodeExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();
        var healthPlan = CreateFaker();

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _healthPlanRepositoryMock
            .SetupSequence(h =>
                h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false)
            .ReturnsAsync(true);

        _healthPlanRepositoryMock
            .Setup(h => h.Update(It.IsAny<HealthPlan>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("O código de registro ANS do plano de saúde já está em uso.");

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.Update(It.IsAny<HealthPlan>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _healthPlanRepositoryMock.Verify(
            h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    private static HealthPlan CreateFaker()
        => CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();

    private static UpdateHealthPlanCommand CreateCommand()
        => CreateFaker<UpdateHealthPlanCommand>()
            .CustomInstantiator(f => new UpdateHealthPlanCommand(
                Guid.CreateVersion7(),
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();
}