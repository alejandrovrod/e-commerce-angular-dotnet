using ECommerce.Order.Domain.Entities;
using ECommerce.Order.Domain.Enums;
using ECommerce.Order.Application.Interfaces;

namespace ECommerce.Order.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    // Temporary in-memory storage until Entity Framework is properly configured
    private static readonly List<ECommerce.Order.Domain.Entities.Order> _orders = new();

    public OrderRepository()
    {
        // No dependency injection needed for now
    }

    public async Task<ECommerce.Order.Domain.Entities.Order?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetAllAsync()
    {
        return await Task.FromResult(_orders.OrderByDescending(o => o.CreatedAt).ToList());
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetByUserIdAsync(Guid userId)
    {
        return await Task.FromResult(_orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt).ToList());
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetByStatusAsync(OrderStatus status)
    {
        return await Task.FromResult(_orders.Where(o => o.Status == status).OrderByDescending(o => o.CreatedAt).ToList());
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await Task.FromResult(_orders.Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate).OrderByDescending(o => o.CreatedAt).ToList());
    }

    public async Task<ECommerce.Order.Domain.Entities.Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await Task.FromResult(_orders.FirstOrDefault(o => o.OrderNumber == orderNumber));
    }

    public async Task<ECommerce.Order.Domain.Entities.Order> AddAsync(ECommerce.Order.Domain.Entities.Order order)
    {
        _orders.Add(order);
        return await Task.FromResult(order);
    }

    public async Task<ECommerce.Order.Domain.Entities.Order> UpdateAsync(ECommerce.Order.Domain.Entities.Order order)
    {
        var existingOrder = _orders.FirstOrDefault(o => o.Id == order.Id);
        if (existingOrder != null)
        {
            var index = _orders.IndexOf(existingOrder);
            _orders[index] = order;
        }
        return await Task.FromResult(order);
    }

    public async Task DeleteAsync(Guid id)
    {
        var order = await GetByIdAsync(id);
        if (order != null)
        {
            _orders.Remove(order);
        }
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Task.FromResult(_orders.Any(o => o.Id == id));
    }

    public async Task<int> GetCountAsync()
    {
        return await Task.FromResult(_orders.Count);
    }

    public async Task<List<ECommerce.Order.Domain.Entities.Order>> GetPaginatedAsync(int page, int pageSize)
    {
        return await Task.FromResult(_orders
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList());
    }

    public async Task SaveChangesAsync()
    {
        // No-op for in-memory implementation
        await Task.CompletedTask;
    }
}
