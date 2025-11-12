using FluentAssertions;
using Health.Domain.Entities;
using Xunit;

namespace Health.Domain.Tests.Entities;

internal sealed class TestEntity : BaseEntity;

public sealed class BaseEntityTests
{
    [Fact(DisplayName = "Deve atribuir a data de modificação ao chamar o método UpdateEntity")]
    public void UpdateEntity_WhenCalled_ShouldSetModificationDate()
    {
        // Arrange
        var result = new TestEntity();

        // Act
        result.UpdateEntity();

        // Assert
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact(DisplayName = "Deve atribuir a data de remoção ao chamar o método DeleteEntity")]
    public void DeleteEntity_WhenCalled_ShouldSetRemovalDate()
    {
        // Arrange
        var result = new TestEntity();

        // Act
        result.DeleteEntity();

        // Assert
        result.DeletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}