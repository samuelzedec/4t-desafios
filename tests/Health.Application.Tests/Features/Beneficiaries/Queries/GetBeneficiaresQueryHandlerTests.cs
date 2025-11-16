using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Application.Features.Beneficiaries.Queries.GetBeneficiaresQuery;
using Health.Domain.Abstractions;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.Beneficiaries.Queries;

public sealed class GetBeneficiaresQueryHandlerTests : BaseTest
{
    private readonly Mock<IBeneficiaryRepository> _beneficiaryRepositoryMock;
    private readonly GetBeneficiaresQueryHandler _handler;

    public GetBeneficiaresQueryHandlerTests()
    {
        _beneficiaryRepositoryMock = new Mock<IBeneficiaryRepository>();
        Mock<IUnitOfWork> unitOfWorkMock = new();
        unitOfWorkMock.Setup(u => u.Beneficiaries).Returns(_beneficiaryRepositoryMock.Object);
        _handler = new GetBeneficiaresQueryHandler(unitOfWorkMock.Object);
    }

    [Fact(DisplayName = "Deve retornar lista paginada de beneficiários com sucesso")]
    public async Task Handle_WhenValidQuery_ShouldReturnPagedBeneficiaries()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter();
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(5);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(5);
        result.Value.PageSize.Should().Be(10);
        result.Value.HasPreviousPage.Should().BeFalse();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                10,
                null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar lista vazia quando não houver beneficiários")]
    public async Task Handle_WhenNoBeneficiaries_ShouldReturnEmptyList()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter();
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().BeEmpty();
        result.Value.Count.Should().Be(0);
        result.Value.HasNextPage.Should().BeFalse();
        result.Value.HasPreviousPage.Should().BeFalse();
    }

    [Fact(DisplayName = "Deve indicar HasPreviousPage como true quando AfterKey é fornecido")]
    public async Task Handle_WhenAfterKeyProvided_ShouldSetHasPreviousPageTrue()
    {
        // Arrange
        var afterKey = Guid.NewGuid();
        var filter = new GetBeneficiaresQueryFilter();
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: afterKey);

        var beneficiaries = CreateBeneficiariesList(3);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.HasPreviousPage.Should().BeTrue();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                10,
                afterKey,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar HasNextPage como true quando há mais itens disponíveis")]
    public async Task Handle_WhenMoreItemsAvailable_ShouldSetHasNextPageTrue()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter();
        var query = new GetBeneficiaresQuery(filter, PageSize: 5, AfterKey: null);
        var beneficiaries = CreateBeneficiariesList(6);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.HasNextPage.Should().BeTrue();
        result.Value.NextKey.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(5);
    }

    [Fact(DisplayName = "Deve aplicar filtro por nome completo corretamente")]
    public async Task Handle_WhenFilterByFullName_ShouldPassFilterToRepository()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter { FullName = "João" };
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(2);

        IFilter<Beneficiary>? capturedFilter = null;
        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<IFilter<Beneficiary>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<Beneficiary>, int, Guid?, CancellationToken>((f, _, _, _) => capturedFilter = f)
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();

        var concreteFilter = (GetBeneficiaresQueryFilter)capturedFilter!;
        concreteFilter.FullName.Should().Be("João");
    }

    [Fact(DisplayName = "Deve aplicar filtro por CPF corretamente")]
    public async Task Handle_WhenFilterByCpf_ShouldPassFilterToRepository()
    {
        // Arrange
        var expectedCpf = "12345678900";
        var filter = new GetBeneficiaresQueryFilter { Cpf = expectedCpf };
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(1);

        IFilter<Beneficiary>? capturedFilter = null;
        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<IFilter<Beneficiary>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<Beneficiary>, int, Guid?, CancellationToken>((f, _, _, _) => capturedFilter = f)
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();
        capturedFilter.Should().BeOfType<GetBeneficiaresQueryFilter>();

        var concreteFilter = (GetBeneficiaresQueryFilter)capturedFilter!;
        concreteFilter.Cpf.Should().Be(expectedCpf);
    }

    [Fact(DisplayName = "Deve aplicar filtro por status corretamente")]
    public async Task Handle_WhenFilterByStatus_ShouldPassFilterToRepository()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter { Status = Status.Active };
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(3);

        IFilter<Beneficiary>? capturedFilter = null;
        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<IFilter<Beneficiary>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<Beneficiary>, int, Guid?, CancellationToken>((f, _, _, _) => capturedFilter = f)
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();

        var concreteFilter = (GetBeneficiaresQueryFilter)capturedFilter!;
        concreteFilter.Status.Should().Be(Status.Active);
    }

    [Fact(DisplayName = "Deve aplicar filtro por plano de saúde corretamente")]
    public async Task Handle_WhenFilterByHealthPlanId_ShouldPassFilterToRepository()
    {
        // Arrange
        var healthPlanId = Guid.NewGuid();
        var filter = new GetBeneficiaresQueryFilter { HealthPlanId = healthPlanId };
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(2);

        IFilter<Beneficiary>? capturedFilter = null;
        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<IFilter<Beneficiary>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<Beneficiary>, int, Guid?, CancellationToken>((f, _, _, _) => capturedFilter = f)
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();

        var concreteFilter = (GetBeneficiaresQueryFilter)capturedFilter!;
        concreteFilter.HealthPlanId.Should().Be(healthPlanId);
    }

    [Fact(DisplayName = "Deve aplicar filtro por data de nascimento corretamente")]
    public async Task Handle_WhenFilterByBirthDate_ShouldPassFilterToRepository()
    {
        // Arrange
        var birthDate = DateOnly.FromDateTime(DateTime.Now.AddYears(-30));
        var filter = new GetBeneficiaresQueryFilter { BirthDate = birthDate };
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(1);

        IFilter<Beneficiary>? capturedFilter = null;
        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<IFilter<Beneficiary>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<Beneficiary>, int, Guid?, CancellationToken>((f, _, _, _) => capturedFilter = f)
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();

        var concreteFilter = (GetBeneficiaresQueryFilter)capturedFilter!;
        concreteFilter.BirthDate.Should().Be(birthDate);
    }

    [Fact(DisplayName = "Deve usar pageSize customizado quando fornecido")]
    public async Task Handle_WhenCustomPageSize_ShouldUseProvidedPageSize()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter();
        var query = new GetBeneficiaresQuery(filter, PageSize: 20, AfterKey: null);

        var beneficiaries = CreateBeneficiariesList(5);

        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(beneficiaries);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageSize.Should().Be(20);

        _beneficiaryRepositoryMock
            .Verify(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                20,
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve lançar exceção quando o token está cancelado")]
    public async Task Handle_WhenTokenIsCanceled_ShouldThrowException()
    {
        // Arrange
        var filter = new GetBeneficiaresQueryFilter();
        var query = new GetBeneficiaresQuery(filter, PageSize: 10, AfterKey: null);
        var cancellationToken = new CancellationTokenSource();
        await cancellationToken.CancelAsync();

        _beneficiaryRepositoryMock
            .Setup(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act
        var result = async () => await _handler.Handle(query, cancellationToken.Token);

        // Assert
        await result.Should().ThrowAsync<OperationCanceledException>();

        _beneficiaryRepositoryMock
            .Verify(b => b.GetPagedAsync(
                It.IsAny<GetBeneficiaresQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    private List<Beneficiary> CreateBeneficiariesList(int? count)
    {
        var healthPlan = HealthPlan.Create(_faker.Person.FullName, _faker.Random.Number(100_000, 999_999).ToString());
        var beneficiaries = CreateFaker<Beneficiary>()
            .CustomInstantiator(f => Beneficiary.Create(
                f.Person.FullName,
                f.Person.Cpf(false),
                DateOnly.FromDateTime(f.Person.DateOfBirth.Date),
                healthPlan.Id
            ))
            .Generate(count ?? 1);

        return beneficiaries;
    }
}