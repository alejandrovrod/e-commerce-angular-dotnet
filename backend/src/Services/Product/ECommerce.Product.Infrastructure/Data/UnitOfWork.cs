using Microsoft.EntityFrameworkCore.Storage;
using ECommerce.Product.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Product.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ProductDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(ProductDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("UnitOfWork: Iniciando SaveChangesAsync");
            
            // Log del estado del contexto antes de SaveChanges
            var categoriesCount = _context.ChangeTracker.Entries<ECommerce.Product.Domain.Entities.Category>().Count();
            var productsCount = _context.ChangeTracker.Entries<ECommerce.Product.Domain.Entities.Product>().Count();
            _logger.LogInformation("UnitOfWork: Contexto tiene {CategoriesCount} categorías y {ProductsCount} productos en ChangeTracker", categoriesCount, productsCount);
            
            var result = await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("UnitOfWork: SaveChangesAsync completado. {Count} entidades guardadas", result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UnitOfWork: Error en SaveChangesAsync");
            throw;
        }
    }

    public async Task<object> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return _currentTransaction;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync();
        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}
