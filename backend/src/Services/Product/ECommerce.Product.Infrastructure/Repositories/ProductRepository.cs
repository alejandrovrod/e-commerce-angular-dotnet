using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using ECommerce.Product.Infrastructure.Data;

namespace ECommerce.Product.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ProductDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Domain.Entities.Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.Product>> FindAsync(Expression<Func<Domain.Entities.Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.Product?> SingleOrDefaultAsync(Expression<Func<Domain.Entities.Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Domain.Entities.Product> AddAsync(Domain.Entities.Product entity, CancellationToken cancellationToken = default)
    {
        var result = await _context.Products.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<IEnumerable<Domain.Entities.Product>> AddRangeAsync(IEnumerable<Domain.Entities.Product> entities, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(Domain.Entities.Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Domain.Entities.Product entity, CancellationToken cancellationToken = default)
    {
        _context.Products.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Domain.Entities.Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Domain.Entities.Product, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Products.CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<Domain.Entities.Product> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<Domain.Entities.Product, bool>>? predicate = null,
        Expression<Func<Domain.Entities.Product, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await _context.Products.CountAsync();
    }

    // Product-specific methods
    public async Task<Domain.Entities.Product?> GetBySkuAsync(string sku)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task<Domain.Entities.Product?> GetBySlugAsync(string slug)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<Domain.Entities.Product?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Domain.Entities.Product>> GetByCategoryAsync(Guid categoryId, int page, int pageSize)
    {
        return await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.Product>> GetByBrandAsync(string brand, int page, int pageSize)
    {
        return await _context.Products
            .Where(p => p.Brand == brand)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.Product>> GetFeaturedAsync(int count)
    {
        return await _context.Products
            .Where(p => p.IsFeatured)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.Product>> GetRelatedAsync(Guid productId, int count)
    {
        var product = await GetByIdAsync(productId);
        if (product == null) return new List<Domain.Entities.Product>();

        return await _context.Products
            .Where(p => p.CategoryId == product.CategoryId && p.Id != productId)
            .Take(count)
            .ToListAsync();
    }

    public async Task<List<Domain.Entities.Product>> SearchAsync(string searchTerm, int page, int pageSize)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountByCategoryAsync(Guid categoryId)
    {
        return await _context.Products
            .CountAsync(p => p.CategoryId == categoryId);
    }

    public async Task<bool> ExistsBySkuAsync(string sku)
    {
        return await _context.Products.AnyAsync(p => p.SKU == sku);
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Products.AnyAsync(p => p.Slug == slug);
    }

    public async Task<int> CountProductsByBrandAsync(string brand)
    {
        var count = await _context.Products.CountAsync(p => p.Brand == brand);
        Console.WriteLine($"🔍 CountProductsByBrandAsync: Brand='{brand}', Count={count}");
        
        // Debug: mostrar algunos productos para verificar
        var sampleProducts = await _context.Products.Take(5).Select(p => new { p.Name, p.Brand }).ToListAsync();
        Console.WriteLine($"🔍 Sample products: {string.Join(", ", sampleProducts.Select(p => $"{p.Name}:{p.Brand}"))}");
        
        return count;
    }
}
