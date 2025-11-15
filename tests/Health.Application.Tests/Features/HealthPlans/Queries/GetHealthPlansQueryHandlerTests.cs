using FluentAssertions;
using Health.Application.Features.HealthPlans.Queries.GetHealthPlansQuery;
using Health.Domain.Abstractions;
using Health.Domain.Entities;
using Health.Domain.Repositories;
using Health.Domain.Tests;
using Moq;
using Xunit;

namespace Health.Application.Tests.Features.HealthPlans.Queries;

public sealed class GetHealthPlansQueryHandlerTests : BaseTest
{
    private readonly Mock<IHealthPlanRepository> _healthPlanRepositoryMock;
    private readonly GetHealthPlansQueryHandler _handler;

    public GetHealthPlansQueryHandlerTests()
    {
        _healthPlanRepositoryMock = new Mock<IHealthPlanRepository>();
        Mock<IUnitOfWork> unitOfWorkMock = new();
        unitOfWorkMock.Setup(u => u.HealthPlans).Returns(_healthPlanRepositoryMock.Object);
        _handler = new GetHealthPlansQueryHandler(unitOfWorkMock.Object);
    }

    [Fact(DisplayName = "Deve retornar lista paginada de planos de saúde com sucesso")]
    public async Task Handle_WhenValidQuery_ShouldReturnPagedHealthPlans()
    {
        // Arrange
        var filter = new GetHealthPlansQueryFilter();
        var query = new GetHealthPlansQuery(filter, PageSize: 10, AfterKey: null);

        var healthPlans = CreateHealthPlansList(5);

        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(5);
        result.Value.PageSize.Should().Be(10);
        result.Value.HasPreviousPage.Should().BeFalse();

        _healthPlanRepositoryMock
            .Verify(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                10,
                null,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar lista vazia quando não houver planos de saúde")]
    public async Task Handle_WhenNoHealthPlans_ShouldReturnEmptyList()
    {
        // Arrange
        var filter = new GetHealthPlansQueryFilter();
        var query = new GetHealthPlansQuery(filter, PageSize: 10, AfterKey: null);

        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
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
        var filter = new GetHealthPlansQueryFilter();
        var query = new GetHealthPlansQuery(filter, PageSize: 10, AfterKey: afterKey);

        var healthPlans = CreateHealthPlansList(3);

        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.HasPreviousPage.Should().BeTrue();

        _healthPlanRepositoryMock
            .Verify(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                10,
                afterKey,
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Deve retornar HasNextPage como true quando há mais itens disponíveis")]
    public async Task Handle_WhenMoreItemsAvailable_ShouldSetHasNextPageTrue()
    {
        // Arrange
        var filter = new GetHealthPlansQueryFilter();
        var query = new GetHealthPlansQuery(filter, PageSize: 5, AfterKey: null);
        var healthPlans = CreateHealthPlansList(6);

        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.HasNextPage.Should().BeTrue();
        result.Value.NextKey.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(5);
    }

    [Fact(DisplayName = "Deve aplicar filtro por nome corretamente")]
    public async Task Handle_WhenFilterByName_ShouldPassFilterToRepository()
    {
        // Arrange
        var filter = new GetHealthPlansQueryFilter { Name = "saude" };
        var query = new GetHealthPlansQuery(filter, PageSize: 10, AfterKey: null);

        var healthPlans = CreateHealthPlansList(2);

        IFilter<HealthPlan>? capturedFilter = null;
        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<IFilter<HealthPlan>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<HealthPlan>, int, Guid?, CancellationToken>((f, _, _, _) => capturedFilter = f)
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();

        var concreteFilter = (GetHealthPlansQueryFilter)capturedFilter!;
        concreteFilter.Name.Should().Be("saude");
    }

    [Fact(DisplayName = "Deve aplicar filtro por código ANS corretamente")]
    public async Task Handle_WhenFilterByAnsCode_ShouldPassFilterToRepository()
    {
        // Arrange
        var expectedAnsCode = "123456";
        var filter = new GetHealthPlansQueryFilter { AnsCode = expectedAnsCode };
        var query = new GetHealthPlansQuery(filter, PageSize: 10, AfterKey: null);

        var healthPlans = CreateHealthPlansList(2);

        IFilter<HealthPlan>? capturedFilter = null;
        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<IFilter<HealthPlan>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .Callback<IFilter<HealthPlan>, int, Guid?, CancellationToken>((f, _, _, _) =>
            {
                capturedFilter = f;
            })
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFilter.Should().NotBeNull();
        capturedFilter.Should().BeOfType<GetHealthPlansQueryFilter>();

        var concreteFilter = (GetHealthPlansQueryFilter)capturedFilter!;
        concreteFilter.AnsCode.Should().Be(expectedAnsCode);
    }

    [Fact(DisplayName = "Deve mapear corretamente HealthPlan para GetHealthPlansQueryResponse")]
    public async Task Handle_WhenHealthPlansReturned_ShouldMapToResponseCorrectly()
    {
        // Arrange
        var filter = new GetHealthPlansQueryFilter();
        var query = new GetHealthPlansQuery(filter, PageSize: 10, AfterKey: null);

        var healthPlan = HealthPlan.Create(
            "Plano Teste",
            "123456"
        );
        var healthPlans = new List<HealthPlan> { healthPlan };

        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<IFilter<HealthPlan>>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);

        var response = result.Value.Items[0];
        response.Id.Should().Be(healthPlan.Id);
        response.Name.Should().Be("Plano Teste");
        response.AnsRegistrationCode.Should().Be("ANS-123456");
    }

    [Fact(DisplayName = "Deve usar pageSize customizado quando fornecido")]
    public async Task Handle_WhenCustomPageSize_ShouldUseProvidedPageSize()
    {
        // Arrange
        var filter = new GetHealthPlansQueryFilter();
        var query = new GetHealthPlansQuery(filter, PageSize: 20, AfterKey: null);

        var healthPlans = CreateHealthPlansList(5);

        _healthPlanRepositoryMock
            .Setup(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                It.IsAny<int>(),
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthPlans);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.PageSize.Should().Be(20);

        _healthPlanRepositoryMock
            .Verify(h => h.GetPagedAsync(
                It.IsAny<GetHealthPlansQueryFilter>(),
                20,
                It.IsAny<Guid?>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    private static List<HealthPlan> CreateHealthPlansList(int? count)
        => CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            ))
            .Generate(count ?? 1);
}