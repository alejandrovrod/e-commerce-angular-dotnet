using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Domain.Repositories;

public interface IBrandRepository : IRepository<Brand>
{
    Task<Brand?> GetByIdAsync(Guid id);
    Task<List<Brand>> GetAllAsync();
    Task<List<Brand>> GetActiveAsync();
    Task<List<Brand>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
}

