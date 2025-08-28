using Microsoft.EntityFrameworkCore;
using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Infrastructure.Data;
using System.Linq.Expressions;

namespace ECommerce.Product.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly ProductDbContext _context;

    public BrandRepository(ProductDbContext context)
    {
        _context = context;
    }

    // IBrandRepository specific methods
    public async Task<Brand?> GetByIdAsync(Guid id)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .Include(b => b.Products)
            .ToListAsync();
    }

    public async Task<List<Brand>> GetActiveAsync()
    {
        return await _context.Brands
            .Include(b => b.Products)
            .Where(b => b.IsActive)
            .ToListAsync();
    }

    public async Task<List<Brand>> SearchAsync(string searchTerm)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .Where(b => b.Name.Contains(searchTerm) || b.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Brands.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await _context.Brands.AnyAsync(b => b.Name == name && b.Id != excludeId.Value);
        }
        return await _context.Brands.AnyAsync(b => b.Name == name);
    }

    // IRepository implementation with CancellationToken
    public async Task<Brand?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Brand>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Brand>> FindAsync(Expression<Func<Brand, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Brand?> SingleOrDefaultAsync(Expression<Func<Brand, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Brands
            .Include(b => b.Products)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Brand> AddAsync(Brand entity, CancellationToken cancellationToken)
    {
        var result = await _context.Brands.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IEnumerable<Brand>> AddRangeAsync(IEnumerable<Brand> entities, CancellationToken cancellationToken)
    {
        await _context.Brands.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(Brand entity, CancellationToken cancellationToken)
    {
        _context.Brands.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Brand entity, CancellationToken cancellationToken)
    {
        _context.Brands.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var brand = await GetByIdAsync(id, cancellationToken);
        if (brand != null)
        {
            _context.Brands.Remove(brand);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<Brand, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Brands.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _context.Brands.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Brand, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Brands.CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<Brand> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<Brand, bool>>? predicate = null,
        Expression<Func<Brand, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Brands.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        var items = await query
            .Include(b => b.Products)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    // Legacy methods without CancellationToken (for backward compatibility)
    public async Task<Brand> AddAsync(Brand entity)
    {
        var result = await _context.Brands.AddAsync(entity);
        return result.Entity;
    }

    public async Task UpdateAsync(Brand entity)
    {
        _context.Brands.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Brand entity)
    {
        _context.Brands.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

	public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}

