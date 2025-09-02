using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Domain.Repositories;

public interface IInventoryMovementRepository : IRepository<InventoryMovement>
{
    Task<List<InventoryMovement>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetByMovementTypeAsync(string movementType, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetFilteredAsync(
        Guid? productId = null,
        string? movementType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        Guid? userId = null,
        int page = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
