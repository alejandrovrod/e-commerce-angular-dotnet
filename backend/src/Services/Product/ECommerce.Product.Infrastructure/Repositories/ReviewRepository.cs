using Microsoft.EntityFrameworkCore;
using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Domain.Enums;
using ECommerce.Product.Infrastructure.Data;
using System.Linq.Expressions;

namespace ECommerce.Product.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly ProductDbContext _context;

    public ReviewRepository(ProductDbContext context)
    {
        _context = context;
    }

    // IReviewRepository specific methods
    public async Task<Review?> GetByIdAsync(Guid id)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Review>> GetAllAsync()
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByProductIdAsync(Guid productId)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Where(r => r.ProductId == productId)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Where(r => r.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByStatusAsync(ReviewStatus status)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Where(r => r.Status == status)
            .ToListAsync();
    }

    public async Task<List<Review>> GetByRatingAsync(int minRating)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Where(r => r.Rating >= minRating)
            .ToListAsync();
    }

    public async Task<decimal> GetAverageRatingAsync(Guid productId)
    {
        var averageRating = await _context.Reviews
            .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
            .AverageAsync(r => (decimal)r.Rating);
        
        return Math.Round(averageRating, 2);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Reviews.AnyAsync(r => r.Id == id);
    }

    public async Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId)
    {
        return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId);
    }

    // IRepository implementation with CancellationToken
    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Review>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Review>> FindAsync(Expression<Func<Review, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Review?> SingleOrDefaultAsync(Expression<Func<Review, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Reviews
            .Include(r => r.Product)
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Review> AddAsync(Review entity, CancellationToken cancellationToken)
    {
        var result = await _context.Reviews.AddAsync(entity, cancellationToken);
        return result.Entity;
    }

    public async Task<IEnumerable<Review>> AddRangeAsync(IEnumerable<Review> entities, CancellationToken cancellationToken)
    {
        await _context.Reviews.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(Review entity, CancellationToken cancellationToken)
    {
        _context.Reviews.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Review entity, CancellationToken cancellationToken)
    {
        _context.Reviews.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var review = await GetByIdAsync(id, cancellationToken);
        if (review != null)
        {
            _context.Reviews.Remove(review);
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<Review, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Reviews.AnyAsync(predicate, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Reviews.AnyAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return await _context.Reviews.CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Review, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _context.Reviews.CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<Review> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<Review, bool>>? predicate = null,
        Expression<Func<Review, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Reviews.AsQueryable();
        
        if (predicate != null)
            query = query.Where(predicate);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
        }

        var items = await query
            .Include(r => r.Product)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
