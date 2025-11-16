using System.Linq.Expressions;
using System.Net;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Application.Features.Beneficiaries.Commands.UpdateBeneficiaryCommand;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.Beneficiaries.Commands;

public sealed class UpdateBeneficiaryCommandHandlerTests : BaseTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IBeneficiaryRepository> _beneficiaryRepositoryMock;
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly UpdateBeneficiaryCommandHandler _handler;

    public UpdateBeneficiaryCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _beneficiaryRepositoryMock = new Mock<IBeneficiaryRepository>();
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        _unitOfWorkMock.Setup(u => u.Beneficiaries).Returns(_beneficiaryRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);

        _handler = new UpdateBeneficiaryCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact(DisplayName = "Deve atualizar todos os campos do beneficiário quando os dados são válidos")]
    public async Task Update_WhenAllDataIsValid_ShouldUpdateBeneficiaryEntity()
    {
        // Arrange
        var command = CreateCommand();
        var beneficiary = CreateBeneficiaryFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _beneficiaryRepositoryMock
            .Setup(b => b.Update(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FullName.Should().Be(command.FullName);
        result.Value.Cpf.Should().Be(command.Cpf);
        result.StatusCode.Should().Be(HttpStatusCode.OK);

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact(DisplayName = "Deve atualizar somente o nome do beneficiário")]
    public async Task Update_WhenOnlyNameIsProvided_ShouldUpdateNameOnly()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();
        var command = new UpdateBeneficiaryCommand(
            beneficiary.Id,
            _faker.Person.FullName,
            string.Empty,
            null,
            null,
            null
        );

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.Update(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.FullName.Should().Be(command.FullName);

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Never);

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact(DisplayName = "Deve atualizar somente o CPF do beneficiário")]
    public async Task Update_WhenOnlyCpfIsProvided_ShouldUpdateCpfOnly()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();
        var command = new UpdateBeneficiaryCommand(
            beneficiary.Id,
            string.Empty,
            _faker.Person.Cpf(false),
            null,
            null,
            null
        );

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _beneficiaryRepositoryMock
            .Setup(b => b.Update(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Cpf.Should().Be(command.Cpf);

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact(DisplayName = "Deve atualizar somente a data de nascimento do beneficiário")]
    public async Task Update_WhenOnlyBirthDateIsProvided_ShouldUpdateBirthDateOnly()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();
        var command = new UpdateBeneficiaryCommand(
            beneficiary.Id,
            string.Empty,
            string.Empty,
            DateOnly.FromDateTime(_faker.Person.DateOfBirth.Date),
            null,
            null
        );

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.Update(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Once);
    }

    [Fact(DisplayName = "Deve atualizar somente o plano de saúde do beneficiário")]
    public async Task Update_WhenOnlyHealthPlanIsProvided_ShouldUpdateHealthPlanOnly()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();
        var command = new UpdateBeneficiaryCommand(
            beneficiary.Id,
            string.Empty,
            string.Empty,
            null,
            Guid.NewGuid(),
            null
        );

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _beneficiaryRepositoryMock
            .Setup(b => b.Update(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact(DisplayName = "Deve atualizar somente o status do beneficiário")]
    public async Task Update_WhenOnlyStatusIsProvided_ShouldUpdateStatusOnly()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();
        var command = new UpdateBeneficiaryCommand(
            beneficiary.Id,
            string.Empty,
            string.Empty,
            null,
            null,
            Status.Inactive
        );

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.Update(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha quando o beneficiário não for encontrado")]
    public async Task Update_WhenBeneficiaryNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var command = CreateCommand();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Beneficiary);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("Beneficiário não encontrado.");

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Deve retornar falha quando o novo CPF já estiver em uso por outro beneficiário")]
    public async Task Update_WhenCpfExists_ShouldReturnConflict()
    {
        // Arrange
        var command = CreateCommand();
        var beneficiary = CreateBeneficiaryFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.Conflict);
        result.Error.Message.Should().Be("O CPF do beneficiário já está em uso.");

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _beneficiaryRepositoryMock
            .Verify(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha quando o plano de saúde informado não existir")]
    public async Task Update_WhenHealthPlanNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var command = CreateCommand();
        var beneficiary = CreateBeneficiaryFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.ExistsAsync(It.IsAny<Expression<Func<Beneficiary, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _healthPlanRepositoryMock
            .Setup(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("O plano de saúde informado não existe.");

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Update(It.IsAny<Beneficiary>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

        _healthPlanRepositoryMock
            .Verify(h => h.ExistsAsync(It.IsAny<Expression<Func<HealthPlan, bool>>>(), It.IsAny<CancellationToken>()),
                Times.Once);
    }

    private static Beneficiary CreateBeneficiaryFaker()
        => CreateFaker<Beneficiary>()
            .CustomInstantiator(f => Beneficiary.Create(
                f.Person.FullName,
                f.Person.Cpf(false),
                DateOnly.FromDateTime(f.Person.DateOfBirth.Date),
                Guid.NewGuid()
            ))
            .Generate();

    private static UpdateBeneficiaryCommand CreateCommand()
        => CreateFaker<UpdateBeneficiaryCommand>()
            .CustomInstantiator(f => new UpdateBeneficiaryCommand(
                Guid.NewGuid(),
                f.Person.FullName,
                f.Person.Cpf(false),
                DateOnly.FromDateTime(f.Person.DateOfBirth.Date),
                Guid.NewGuid(),
                f.PickRandom<Status>()
            ))
            .Generate();
}