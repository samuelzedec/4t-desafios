using FluentAssertions;
using Health.Domain.Entities;
using Xunit;

namespace Health.Domain.Tests.Entities;

public sealed class HealthPlanTests : BaseTest
{
    [Fact(DisplayName = "Deve criar a entidade do plano de saúde com os dados corretos")]
    public void Create_WhenDataIsValid_ShouldCreateHealthPlanEntity()
    {
        // Arrange & Act
        var result = CreateFaker();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeNull();
        result.DeletedAt.Should().BeNull();
        result.AnsRegistrationCode.Should().NotBeNull();
        result.Name.Should().NotBeNull();
    }

    [Fact(DisplayName = "Deve atualizar o nome do plano de saúde quando o dad estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateHealthPlanEntityName()
    {
        // Arrange
        var result = CreateFaker();
        const string newName = "Nome de teste";

        // Act
        result.UpdateName(newName);

        // Assert
        result.Name.Value.Should().Be(newName);
    }

    [Fact(DisplayName = "Deve atualizar o código da ANS do plano de saúde quando o dad estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateHealthPlanEntityAnsCode()
    {
        // Arrange
        var result = CreateFaker();
        string newCode = _faker.Random.Number(100_000, 999_999).ToString();

        // Act
        result.UpdateAnsCode(newCode);

        // Assert
        result.AnsRegistrationCode.Value.Should().Be(newCode);
    }

    private static HealthPlan CreateFaker()
        => CreateFaker<HealthPlan>()
            .CustomInstantiator(f => HealthPlan.Create(
                f.Person.FullName,
                f.Random.Number(100_000, 999_999).ToString()
            )).Generate();
}