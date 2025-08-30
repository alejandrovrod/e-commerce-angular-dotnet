using Microsoft.EntityFrameworkCore;
using ECommerce.File.Application.Interfaces;
using ECommerce.File.Domain.Entities;
using ECommerce.File.Domain.Enums;
using ECommerce.File.Infrastructure.Data;

namespace ECommerce.File.Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    private readonly FileDbContext _context;

    public FileRepository(FileDbContext context)
    {
        _context = context;
    }

    public async Task<ECommerce.File.Domain.Entities.File?> GetByIdAsync(Guid id)
    {
        return await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<ECommerce.File.Domain.Entities.File?> GetByFileNameAsync(string fileName)
    {
        return await _context.Files.FirstOrDefaultAsync(f => f.FileName == fileName);
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Files
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetByOrderIdAsync(Guid orderId)
    {
        return await _context.Files
            .Where(f => f.OrderId == orderId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetByProductIdAsync(Guid productId)
    {
        return await _context.Files
            .Where(f => f.ProductId == productId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetByTypeAsync(FileType type)
    {
        return await _context.Files
            .Where(f => f.Type == type)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetByStatusAsync(FileStatus status)
    {
        return await _context.Files
            .Where(f => f.Status == status)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetPublicFilesAsync()
    {
        return await _context.Files
            .Where(f => f.IsPublic)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetAllAsync()
    {
        return await _context.Files
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }

    public async Task<ECommerce.File.Domain.Entities.File> AddAsync(ECommerce.File.Domain.Entities.File file)
    {
        var entry = await _context.Files.AddAsync(file);
        return entry.Entity;
    }

    public async Task<ECommerce.File.Domain.Entities.File> UpdateAsync(ECommerce.File.Domain.Entities.File file)
    {
        var entry = _context.Files.Update(file);
        return entry.Entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var file = await GetByIdAsync(id);
        if (file != null)
        {
            _context.Files.Remove(file);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Files.AnyAsync(f => f.Id == id);
    }

    public async Task<int> GetCountAsync()
    {
        return await _context.Files.CountAsync();
    }

    public async Task<List<ECommerce.File.Domain.Entities.File>> GetExpiredFilesAsync()
    {
        return await _context.Files
            .Where(f => f.ExpiresAt.HasValue && f.ExpiresAt.Value < DateTime.UtcNow)
            .ToListAsync();
    }
}
