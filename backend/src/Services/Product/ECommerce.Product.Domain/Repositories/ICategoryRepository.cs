using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetActiveAsync();
    Task<List<Category>> GetByParentIdAsync(Guid? parentId);
    Task<List<Category>> GetSubcategoriesAsync(Guid parentId);
    Task<List<Category>> SearchAsync(string searchTerm);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
}
