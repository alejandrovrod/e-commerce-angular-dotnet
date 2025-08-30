using ECommerce.File.Domain.Entities;
using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Application.Interfaces;

public interface IFileRepository
{
    Task<ECommerce.File.Domain.Entities.File?> GetByIdAsync(Guid id);
    Task<ECommerce.File.Domain.Entities.File?> GetByFileNameAsync(string fileName);
    Task<List<ECommerce.File.Domain.Entities.File>> GetByUserIdAsync(Guid userId);
    Task<List<ECommerce.File.Domain.Entities.File>> GetByOrderIdAsync(Guid orderId);
    Task<List<ECommerce.File.Domain.Entities.File>> GetByProductIdAsync(Guid productId);
    Task<List<ECommerce.File.Domain.Entities.File>> GetByTypeAsync(FileType type);
    Task<List<ECommerce.File.Domain.Entities.File>> GetByStatusAsync(FileStatus status);
    Task<List<ECommerce.File.Domain.Entities.File>> GetPublicFilesAsync();
    Task<List<ECommerce.File.Domain.Entities.File>> GetAllAsync();
    Task<ECommerce.File.Domain.Entities.File> AddAsync(ECommerce.File.Domain.Entities.File file);
    Task<ECommerce.File.Domain.Entities.File> UpdateAsync(ECommerce.File.Domain.Entities.File file);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetCountAsync();
    Task<List<ECommerce.File.Domain.Entities.File>> GetExpiredFilesAsync();
}
