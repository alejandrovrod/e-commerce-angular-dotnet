using ECommerce.Order.Domain.Entities;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Application.Interfaces;

public interface IOrderRepository
{
    Task<ECommerce.Order.Domain.Entities.Order?> GetByIdAsync(Guid id);
    Task<List<ECommerce.Order.Domain.Entities.Order>> GetAllAsync();
    Task<List<ECommerce.Order.Domain.Entities.Order>> GetByUserIdAsync(Guid userId);
    Task<List<ECommerce.Order.Domain.Entities.Order>> GetByStatusAsync(ECommerce.Order.Domain.Enums.OrderStatus status);
    Task<List<ECommerce.Order.Domain.Entities.Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<ECommerce.Order.Domain.Entities.Order?> GetByOrderNumberAsync(string orderNumber);
    Task<ECommerce.Order.Domain.Entities.Order> AddAsync(ECommerce.Order.Domain.Entities.Order order);
    Task<ECommerce.Order.Domain.Entities.Order> UpdateAsync(ECommerce.Order.Domain.Entities.Order order);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetCountAsync();
}
