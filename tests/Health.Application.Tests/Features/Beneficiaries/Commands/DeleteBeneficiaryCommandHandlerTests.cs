using System.Net;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Application.Features.Beneficiaries.Commands.DeleteBeneficiaryCommand;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.Beneficiaries.Commands;

public sealed class DeleteBeneficiaryCommandHandlerTests : BaseTest
{
    private readonly Mock<IBeneficiaryRepository> _beneficiaryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly DeleteBeneficiaryCommandHandler _handlerTests;
    private readonly CancellationTokenSource _cancellationToken;

    public DeleteBeneficiaryCommandHandlerTests()
    {
        _beneficiaryRepositoryMock = new Mock<IBeneficiaryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkMock.Setup(u => u.Beneficiaries).Returns(_beneficiaryRepositoryMock.Object);
        _handlerTests = new DeleteBeneficiaryCommandHandler(_unitOfWorkMock.Object);

        _cancellationToken = new CancellationTokenSource();
        _cancellationToken.Cancel();
    }

    [Fact(DisplayName = "Deve marcar o beneficiário como removido do banco")]
    public async Task DeleteBeneficiary_WhenCalled_ShouldMarkAsRemovedInDatabase()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.Delete(It.IsAny<Beneficiary>()))
            .Callback<Beneficiary>(b => b.DeleteEntity());

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()));

        // Act
        var result = await _handlerTests.Handle(new DeleteBeneficiaryCommand(beneficiary.Id), _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        beneficiary.DeletedAt.Should().NotBeNull();
        beneficiary.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Delete(It.IsAny<Beneficiary>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar falha quando o beneficiário não for encontrado")]
    public async Task GetBeneficiary_WhenNotFound_ShouldReturnFailure()
    {
        // Arrange
        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as Beneficiary);

        // Act
        var result = await _handlerTests.Handle(new DeleteBeneficiaryCommand(Guid.NewGuid()),
            _cancellationToken.Token);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        result.Error.Message.Should().Be("Beneficiário não encontrado.");

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Delete(It.IsAny<Beneficiary>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado ao buscar beneficiário")]
    public async Task GetBeneficiary_WhenTokenIsCancelled_ShouldThrowException()
    {
        // Arrange
        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(new DeleteBeneficiaryCommand(Guid.NewGuid()),
            _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Delete(It.IsAny<Beneficiary>()), Times.Never);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Lança exceção quando o token está cancelado ao salvar no banco")]
    public async Task SaveChanges_WhenTokenIsCancelled_ShouldThrowException()
    {
        // Arrange
        var beneficiary = CreateBeneficiaryFaker();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiary);

        _beneficiaryRepositoryMock
            .Setup(b => b.Delete(It.IsAny<Beneficiary>()));

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handlerTests.Handle(new DeleteBeneficiaryCommand(beneficiary.Id),
            _cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        _beneficiaryRepositoryMock
            .Verify(b => b.Delete(It.IsAny<Beneficiary>()), Times.Once);

        _unitOfWorkMock
            .Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
}

