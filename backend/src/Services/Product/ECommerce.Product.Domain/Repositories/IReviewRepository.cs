using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Domain.Repositories;

public interface IReviewRepository : IRepository<Review>
{
    Task<Review?> GetByIdAsync(Guid id);
    Task<List<Review>> GetAllAsync();
    Task<List<Review>> GetByProductIdAsync(Guid productId);
    Task<List<Review>> GetByUserIdAsync(Guid userId);
    Task<List<Review>> GetByStatusAsync(ReviewStatus status);
    Task<List<Review>> GetByRatingAsync(int minRating);
    Task<decimal> GetAverageRatingAsync(Guid productId);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId);
}

