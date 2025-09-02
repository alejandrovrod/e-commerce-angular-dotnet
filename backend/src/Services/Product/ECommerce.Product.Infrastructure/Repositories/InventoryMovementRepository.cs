using Microsoft.EntityFrameworkCore;
using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Infrastructure.Data;
using System.Linq.Expressions;

namespace ECommerce.Product.Infrastructure.Repositories;

public class InventoryMovementRepository : IInventoryMovementRepository
{
    private readonly ProductDbContext _context;

    public InventoryMovementRepository(ProductDbContext context)
    {
        _context = context;
    }

    // IInventoryMovementRepository specific methods
    public async Task<List<InventoryMovement>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .Where(m => m.ProductId == productId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryMovement>> GetByMovementTypeAsync(string movementType, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .Where(m => m.MovementType == movementType)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryMovement>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .Where(m => m.CreatedAt >= fromDate && m.CreatedAt <= toDate)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryMovement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryMovement>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .Where(m => m.OrderId == orderId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<InventoryMovement>> GetFilteredAsync(
        Guid? productId = null,
        string? movementType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? userId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryMovements
            .Include(m => m.Product)
            .AsQueryable();

        if (productId.HasValue)
            query = query.Where(m => m.ProductId == productId.Value);

        if (!string.IsNullOrEmpty(movementType))
            query = query.Where(m => m.MovementType == movementType);

        if (fromDate.HasValue)
            query = query.Where(m => m.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(m => m.CreatedAt <= toDate.Value);

        if (userId.HasValue)
            query = query.Where(m => m.UserId == userId.Value);

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    // IRepository implementation
    public async Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InventoryMovement>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InventoryMovement>> FindAsync(Expression<Func<InventoryMovement, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .Where(predicate)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryMovement?> SingleOrDefaultAsync(Expression<Func<InventoryMovement, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements
            .Include(m => m.Product)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<InventoryMovement> AddAsync(InventoryMovement entity, CancellationToken cancellationToken)
    {
        var result = await _context.InventoryMovements.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IEnumerable<InventoryMovement>> AddRangeAsync(IEnumerable<InventoryMovement> entities, CancellationToken cancellationToken)
    {
        await _context.InventoryMovements.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(InventoryMovement entity, CancellationToken cancellationToken)
    {
        _context.InventoryMovements.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(InventoryMovement entity, CancellationToken cancellationToken)
    {
        _context.InventoryMovements.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var movement = await GetByIdAsync(id, cancellationToken);
        if (movement != null)
        {
            _context.InventoryMovements.Remove(movement);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<InventoryMovement, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements.AnyAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<InventoryMovement, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.InventoryMovements.CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<InventoryMovement> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<InventoryMovement, bool>>? predicate = null,
        Expression<Func<InventoryMovement, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.InventoryMovements.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }
        else
        {
            query = query.OrderByDescending(m => m.CreatedAt);
        }

        var items = await query
            .Include(m => m.Product)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
