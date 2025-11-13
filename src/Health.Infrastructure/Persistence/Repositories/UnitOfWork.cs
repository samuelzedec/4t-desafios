using Health.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Health.Infrastructure.Persistence.Repositories;

public sealed class UnitOfWork(AppDbContext context)
    : IUnitOfWork
{
    #region Fields

    private IDbContextTransaction? _transaction;
    private IBeneficiaryRepository? _beneficiaryRepository;
    private IHealthPlanRepository? _healthPlanRepository;

    #endregion

    #region Properties

    public IBeneficiaryRepository Beneficiaries
        => _beneficiaryRepository ??= new BeneficiaryRepository(context);

    public IHealthPlanRepository HealthPlans
        => _healthPlanRepository ??= new HealthPlanRepository(context);

    #endregion

    #region Methods

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        => _transaction ??= await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.CommitAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
            return;

        await _transaction.RollbackAsync(cancellationToken);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    #endregion
}