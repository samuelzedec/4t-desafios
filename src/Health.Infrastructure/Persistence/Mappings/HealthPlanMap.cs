using Health.Domain.Entities;
using Health.Domain.ValueObjects.AnsRegistrationCode;
using Health.Domain.ValueObjects.Name;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Health.Infrastructure.Persistence.Mappings;

public sealed class HealthPlanMap
    : BaseEntityTypeConfiguration<HealthPlan>
{
    protected override string GetTableName()
        => "health_plans";

    protected override void ConfigureEntity(EntityTypeBuilder<HealthPlan> builder)
    {
        builder.OwnsOne(h => h.Name, navigationBuilder => navigationBuilder
            .Property(n => n.Value)
            .HasColumnName("name")
            .HasColumnType("text")
            .HasMaxLength(Name.MaxLength)
            .UseCollation("case_insensitive")
            .IsRequired()
        );

        builder.OwnsOne(h => h.AnsRegistrationCode, navigationBuilder =>
        {
            navigationBuilder
                .Property(ans => ans.Value)
                .HasColumnName("ans_registration_code")
                .HasColumnType("text")
                .HasMaxLength(AnsRegistrationCode.CodeLength)
                .IsRequired();

            navigationBuilder
                .HasIndex(ans => ans.Value, "ix_ans_registration_code");
        });
    }
}