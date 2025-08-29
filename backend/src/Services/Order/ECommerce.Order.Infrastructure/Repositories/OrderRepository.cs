using Microsoft.EntityFrameworkCore;
using ECommerce.Order.Domain.Entities;
using ECommerce.Order.Domain.Enums;
using ECommerce.Order.Application.Interfaces;
using ECommerce.Order.Infrastructure.Data;

namespace ECommerce.Order.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _context;

    public OrderRepository(OrderDbContext context)
    {
        _context = context;
    }

    public async Task<ECommerce.Order.Domain.Entities.Order?> GetByIdAsync(Guid id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<ECommerce.Order.Domain.Entities.Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<ECommerce.Order.Domain.Entities.Order> AddAsync(ECommerce.Order.Domain.Entities.Order order)
    {
        var entry = await _context.Orders.AddAsync(order);
        return entry.Entity;
    }

    public async Task<ECommerce.Order.Domain.Entities.Order> UpdateAsync(ECommerce.Order.Domain.Entities.Order order)
    {
        var entry = _context.Orders.Update(order);
        return entry.Entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await GetByIdAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Orders.AnyAsync(o => o.Id == id);
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Orders.CountAsync();
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetPaginatedAsync(int page, int pageSize)
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
