using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Domain.Repositories;

public interface IProductRepository : IRepository<Entities.Product>
{
    Task<Entities.Product?> GetBySkuAsync(string sku);
    Task<Entities.Product?> GetBySlugAsync(string slug);
    Task<Entities.Product?> GetByIdWithDetailsAsync(Guid id);
    Task<List<Entities.Product>> GetByCategoryAsync(Guid categoryId, int page, int pageSize);
    Task<List<Entities.Product>> GetByBrandAsync(string brand, int page, int pageSize);
    Task<List<Entities.Product>> GetFeaturedAsync(int count);
    Task<List<Entities.Product>> GetRelatedAsync(Guid productId, int count);
    Task<List<Entities.Product>> SearchAsync(string searchTerm, int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<int> GetCountByCategoryAsync(Guid categoryId);
    Task<int> CountProductsByBrandAsync(string brand);
    Task<bool> ExistsBySkuAsync(string sku);
    Task<bool> ExistsBySlugAsync(string slug);
}
