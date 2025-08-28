using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Domain.Repositories;

public interface IInventoryRepository : IRepository<Inventory>
{
    Task<Inventory?> GetByIdAsync(Guid id);
    Task<Inventory?> GetByProductIdAsync(Guid productId);
    Task<List<Inventory>> GetAllAsync();
    Task<List<Inventory>> GetByLocationAsync(string location);
    Task<List<Inventory>> GetLowStockAsync(int threshold = 10);
    Task<List<Inventory>> GetOutOfStockAsync();
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByProductIdAsync(Guid productId);
}

