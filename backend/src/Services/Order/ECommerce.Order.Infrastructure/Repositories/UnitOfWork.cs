using ECommerce.Order.Application.Interfaces;

namespace ECommerce.Order.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    // Temporary in-memory implementation until Entity Framework is properly configured
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // No-op for in-memory implementation
        return await Task.FromResult(1);
    }

    public async Task<object> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // Return a dummy transaction object
        return await Task.FromResult(new object());
    }

    public async Task CommitTransactionAsync(object transaction, CancellationToken cancellationToken = default)
    {
        // No-op for in-memory implementation
        await Task.CompletedTask;
    }

    public async Task RollbackTransactionAsync(object transaction, CancellationToken cancellationToken = default)
    {
        // No-op for in-memory implementation
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        // Nothing to dispose in in-memory implementation
    }
}

