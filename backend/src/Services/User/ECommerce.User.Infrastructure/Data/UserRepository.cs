using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.User.Application.Interfaces;
using ECommerce.User.Domain.Entities;
using ECommerce.User.Domain.Enums;

namespace ECommerce.User.Infrastructure.Data;

public class UserRepository : IUserRepository
{
    private readonly UserDbContext _context;

    public UserRepository(UserDbContext context)
    {
        _context = context;
    }

    // IRepository<User> implementation
    public async Task<Domain.Entities.User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.User>> FindAsync(Expression<Func<Domain.Entities.User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.User?> SingleOrDefaultAsync(Expression<Func<Domain.Entities.User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<Domain.Entities.User> AddAsync(Domain.Entities.User entity, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<Domain.Entities.User>> AddRangeAsync(IEnumerable<Domain.Entities.User> entities, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(Domain.Entities.User entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Domain.Entities.User entity, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<Domain.Entities.User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(Expression<Func<Domain.Entities.User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .CountAsync(predicate, cancellationToken);
    }

    public async Task<(IEnumerable<Domain.Entities.User> Items, int TotalCount)> GetPagedAsync(
        int page, 
        int pageSize, 
        Expression<Func<Domain.Entities.User, bool>>? predicate = null,
        Expression<Func<Domain.Entities.User, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

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

    // IUserRepository specific methods
    public async Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<Domain.Entities.User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);
    }

    public async Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return false;

        return await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<IEnumerable<Domain.Entities.User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Role == role)
            .ToListAsync(cancellationToken);
    }

    public async Task<Domain.Entities.User?> GetUserWithAddressesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<Domain.Entities.User?> GetUserWithPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Include(u => u.Preferences)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }
}
