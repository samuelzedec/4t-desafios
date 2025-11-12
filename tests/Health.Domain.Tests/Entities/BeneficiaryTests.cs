using Bogus.Extensions.Brazil;
using FluentAssertions;
using Health.Domain.Entities;
using Health.Domain.Enums;
using Xunit;

namespace Health.Domain.Tests.Entities;

public sealed class BeneficiaryTests : BaseTest
{
    [Fact(DisplayName = "Deve criar a entidade do beneficiário com os dados corretos")]
    public void Create_WhenDataIsValid_ShouldCreateBeneficiaryEntity()
    {
        // Arrange & Act
        var result = CreateFaker();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeNull();
        result.DeletedAt.Should().BeNull();
        result.FullName.Should().NotBeNull();
        result.Cpf.Should().NotBeNull();
        result.BirthDate.Should().NotBeNull();
        result.Status.Should().Be(Status.Active);
        result.HealthPlanId.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Deve atualizar o nome completo do beneficiário quando o dado estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateBeneficiaryEntityFullName()
    {
        // Arrange
        var result = CreateFaker();
        const string newName = "Nome de teste";

        // Act
        result.UpdateFullName(newName);

        // Assert
        result.FullName.Value.Should().Be(newName);
    }

    [Fact(DisplayName = "Deve atualizar o CPF do beneficiário quando o dado estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateBeneficiaryEntityCpf()
    {
        // Arrange
        var result = CreateFaker();
        string newCpf = _faker.Person.Cpf(false);

        // Act
        result.UpdateCpf(newCpf);

        // Assert
        result.Cpf.Value.Should().Be(newCpf);
    }

    [Fact(DisplayName = "Deve atualizar a data de nascimento do beneficiário quando o dado estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateBeneficiaryEntityBirthDate()
    {
        // Arrange
        var result = CreateFaker();
        DateOnly newBirthDate = DateOnly.FromDateTime(_faker.Person.DateOfBirth);

        // Act
        result.UpdateBirthDate(newBirthDate);

        // Assert
        result.BirthDate.Value.Should().Be(newBirthDate);
    }

    [Fact(DisplayName = "Deve atualizar o status do beneficiário quando o dado estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateBeneficiaryEntityStatus()
    {
        // Arrange
        var result = CreateFaker();
        const Status newStatus = Status.Inactive;

        // Act
        result.UpdateStatus(newStatus);

        // Assert
        result.Status.Should().Be(newStatus);
    }

    [Fact(DisplayName = "Deve atualizar o ID do plano de saúde do beneficiário quando o dado estiver correto")]
    public void Update_WhenDataIsValid_ShouldUpdateBeneficiaryEntityHealthPlanId()
    {
        // Arrange
        var result = CreateFaker();
        Guid newHealthPlanId = Guid.NewGuid();

        // Act
        result.UpdateFlatId(newHealthPlanId);

        // Assert
        result.HealthPlanId.Should().Be(newHealthPlanId);
    }

    private static Beneficiary CreateFaker()
        => CreateFaker<Beneficiary>()
            .CustomInstantiator(f => Beneficiary.Create(
                f.Person.FullName,
                f.Person.Cpf(),
                DateOnly.FromDateTime(f.Person.DateOfBirth),
                Guid.NewGuid()
            )).Generate();
}