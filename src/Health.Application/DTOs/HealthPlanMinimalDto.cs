namespace Health.Application.DTOs;

public sealed record HealthPlanMinimalDto(
    string Name,
    string AnsRegistrationCode
);