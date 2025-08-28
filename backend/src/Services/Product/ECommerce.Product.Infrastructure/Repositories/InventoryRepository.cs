using Microsoft.EntityFrameworkCore;
using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Infrastructure.Data;
using System.Linq.Expressions;

namespace ECommerce.Product.Infrastructure.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly ProductDbContext _context;

    public InventoryRepository(ProductDbContext context)
    {
        _context = context;
    }

    // IInventoryRepository specific methods
    public async Task<Inventory?> GetByIdAsync(Guid id)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Inventory?> GetByProductIdAsync(Guid productId)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);
    }

    public async Task<List<Inventory>> GetAllAsync()
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .ToListAsync();
    }

    public async Task<List<Inventory>> GetByLocationAsync(string location)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.Location == location)
            .ToListAsync();
    }

    public async Task<List<Inventory>> GetLowStockAsync(int threshold = 10)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.AvailableQuantity <= threshold)
            .ToListAsync();
    }

    public async Task<List<Inventory>> GetOutOfStockAsync()
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.AvailableQuantity <= 0)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Inventories.AnyAsync(i => i.Id == id);
    }

    public async Task<bool> ExistsByProductIdAsync(Guid productId)
    {
        return await _context.Inventories.AnyAsync(i => i.ProductId == productId);
    }

    // IRepository implementation with CancellationToken
    public async Task<Inventory?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> FindAsync(Expression<Func<Inventory, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Inventory?> SingleOrDefaultAsync(Expression<Func<Inventory, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Inventory> AddAsync(Inventory entity, CancellationToken cancellationToken)
    {
        var result = await _context.Inventories.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IEnumerable<Inventory>> AddRangeAsync(IEnumerable<Inventory> entities, CancellationToken cancellationToken)
    {
        await _context.Inventories.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(Inventory entity, CancellationToken cancellationToken)
    {
        _context.Inventories.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Inventory entity, CancellationToken cancellationToken)
    {
        _context.Inventories.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var inventory = await GetByIdAsync(id, cancellationToken);
        if (inventory != null)
        {
            _context.Inventories.Remove(inventory);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<Inventory, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Inventories.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Inventories.AnyAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _context.Inventories.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Inventory, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Inventories.CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<Inventory> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<Inventory, bool>>? predicate = null,
        Expression<Func<Inventory, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Inventories.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        var items = await query
            .Include(i => i.Product)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
