using System.Linq.Expressions;
using System.Net;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Application.Features.Beneficiaries.Commands.CreateBeneficiaryCommand;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.Beneficiaries.Commands;

public sealed class CreateBeneficiaryCommandHandlerTests : BaseTest
{
    private readonly Mock<IBeneficiaryRepository> _beneficiaryRepositoryMock;
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateBeneficiaryCommandHandler _handlerTests;
    private readonly CancellationTokenSource _cancellationToken;

    public CreateBeneficiaryCommandHandlerTests()
    {
        _beneficiaryRepositoryMock = new Mock<IBeneficiaryRepository>();
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Beneficiaries).Returns(_beneficiaryRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);
        _handlerTests = new CreateBeneficiaryCommandHandler(_unitOfWorkMock.Object);

        _cancellationToken = new CancellationTokenSource();
        _cancellationToken.Cancel();
    }

    [Fact(DisplayName = "Deve criar o beneficiário com sucesso")]
    public async Task CreateBeneficiary_WhenValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var command = CreateCommand();
        var healthPlan = CreateHealthPlanFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _beneficiaryRepositoryMock
            .Setup(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handlerTests.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().NotBeEmpty();
        result.Value.FullName.Should().Be(command.FullName);
        result.Value.Cpf.Should().Be(command.Cpf);
        result.Value.HealthPlanName.Should().Be(healthPlan.Name);
        result.StatusCode.Should().Be(HttpStatusCode.Created);

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar conflito ao tentar criar um beneficiário com CPF já existente")]
    public async Task CreateBeneficiary_WhenCpfExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handlerTests.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("O CPF do beneficiário já está em uso.");

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve retornar conflito ao tentar criar um beneficiário com plano de saúde inexistente")]
    public async Task CreateBeneficiary_WhenHealthPlanNotExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HealthPlan?)null);

        // Act
        var result = await _handlerTests.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("Plano de saúde não existente.");

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado na verificação de existência do CPF")]
    public async Task CheckCpfExistence_WhenTokenIsCancelled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado na busca do plano de saúde")]
    public async Task GetHealthPlan_WhenTokenIsCancelled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado na criação do beneficiário")]
    public async Task CreateBeneficiary_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();
        var healthPlan = CreateHealthPlanFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _beneficiaryRepositoryMock
            .Setup(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado ao salvar beneficiário no banco")]
    public async Task SaveBeneficiary_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var command = CreateCommand();
        var healthPlan = CreateHealthPlanFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlan);

        _beneficiaryRepositoryMock
            .Setup(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(command, _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.CreateAsync(It.IsAny<Beneficiary>(), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static HealthPlan CreateHealthPlanFaker()
        => CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate();

    private static CreateBeneficiaryCommand CreateCommand()
        => CreateFaker<CreateBeneficiaryCommand>()
            .CustomInstantiator(f => new CreateBeneficiaryCommand(
                f.Person.FullName,
                f.Person.Cpf(false),
                DateOnly.FromDateTime(f.Person.DateOfBirth.Date),
                f.Random.Guid()
            ))
            .Generate();
}