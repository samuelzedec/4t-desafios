using Health.Domain.Entities;
using Health.Domain.ValueObjects.Cpf;
using Health.Domain.ValueObjects.Name;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Health.Infrastructure.Persistence.Mappings;

public sealed class BeneficiaryMap
    : BaseEntityTypeConfiguration<Beneficiary>
{
    protected override string GetTableName()
        => "beneficiaries";

    protected override void ConfigureEntity(EntityTypeBuilder<Beneficiary> builder)
    {
        builder.OwnsOne(b => b.FullName, navigationBuilder => navigationBuilder
            .Property(f => f.Value)
            .HasColumnName("full_name")
            .HasColumnType("text")
            .HasMaxLength(Name.MaxLength)
            .UseCollation("case_insensitive")
            .IsRequired()
        );

        builder.OwnsOne(b => b.Cpf, navigationBuilder =>
        {
            navigationBuilder
                .Property(c => c.Value)
                .HasColumnName("cpf")
                .HasColumnType("text")
                .HasMaxLength(Cpf.CpfLength)
                .IsRequired();

            navigationBuilder
                .HasIndex(c => c.Value, "ix_cpf")
                .IsUnique();
        });

        builder.OwnsOne(b => b.BirthDate, navigationBuilder => navigationBuilder
            .Property(b => b.Value)
            .HasColumnName("birth_date")
            .HasColumnType("date")
            .IsRequired()
        );

        builder
            .Property(b => b.Status)
            .HasColumnName("status")
            .HasColumnType("text")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(b => b.HealthPlanId)
            .HasColumnName("health_plan_id")
            .HasColumnType("uuid")
            .IsRequired();

        builder
            .HasOne(b => b.HealthPlan)
            .WithMany(hp => hp.Beneficiaries)
            .HasForeignKey(b => b.HealthPlanId)
            .HasConstraintName("fk_beneficiaries_health_plans")
            .OnDelete(DeleteBehavior.Restrict);
    }
}