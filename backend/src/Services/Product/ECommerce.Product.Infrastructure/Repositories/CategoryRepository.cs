using Microsoft.EntityFrameworkCore;
using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Infrastructure.Data;
using System.Linq.Expressions;

namespace ECommerce.Product.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ProductDbContext _context;

    public CategoryRepository(ProductDbContext context)
    {
        _context = context;
    }

    // ICategoryRepository specific methods
    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .ToListAsync();
    }

    public async Task<List<Category>> GetActiveAsync()
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .ToListAsync();
    }

    public async Task<List<Category>> GetByParentIdAsync(Guid? parentId)
    {
        if (parentId.HasValue)
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Subcategories)
                .Include(c => c.Products)
                .Where(c => c.ParentCategoryId == parentId.Value)
                .ToListAsync();
        }
        else
        {
            return await _context.Categories
                .Include(c => c.ParentCategory)
                .Include(c => c.Subcategories)
                .Include(c => c.Products)
                .Where(c => c.ParentCategoryId == null)
                .ToListAsync();
        }
    }

    public async Task<List<Category>> GetSubcategoriesAsync(Guid parentId)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .Where(c => c.ParentCategoryId == parentId)
            .ToListAsync();
    }

    public async Task<List<Category>> SearchAsync(string searchTerm)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
    {
        // Log para debugging
        var allCategories = await _context.Categories.ToListAsync();
        Console.WriteLine($"CategoryRepository: ExistsByNameAsync - Consultando categoría '{name}'. Total en contexto: {allCategories.Count}");
        foreach (var cat in allCategories)
        {
            Console.WriteLine($"CategoryRepository: Categoría encontrada - ID: {cat.Id}, Name: '{cat.Name}', IsActive: {cat.IsActive}");
        }
        
        if (excludeId.HasValue)
        {
            return await _context.Categories.AnyAsync(c => c.Name == name && c.Id != excludeId.Value);
        }
        return await _context.Categories.AnyAsync(c => c.Name == name);
    }

    // IRepository implementation with CancellationToken
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Category>> FindAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> SingleOrDefaultAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Category> AddAsync(Category entity, CancellationToken cancellationToken)
    {
        var result = await _context.Categories.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IEnumerable<Category>> AddRangeAsync(IEnumerable<Category> entities, CancellationToken cancellationToken)
    {
        await _context.Categories.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken)
    {
        _context.Categories.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Category entity, CancellationToken cancellationToken)
    {
        _context.Categories.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Category, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Categories.CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<Category> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<Category, bool>>? predicate = null,
        Expression<Func<Category, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        var items = await query
            .Include(c => c.ParentCategory)
            .Include(c => c.Subcategories)
            .Include(c => c.Products)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}

