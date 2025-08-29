using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerce.Order.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<object> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(object transaction, CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(object transaction, CancellationToken cancellationToken = default);
}
