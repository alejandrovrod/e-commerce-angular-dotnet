using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Domain.Entities;

public class File : BaseAuditableEntity
{
    public string FileName { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string FileExtension { get; private set; } = string.Empty;
    public FileType Type { get; private set; }
    public FileStatus Status { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? OrderId { get; private set; }
    public Guid? ProductId { get; private set; }
    public string? Description { get; private set; }
    public Dictionary<string, object>? Metadata { get; private set; }
    public string? ThumbnailPath { get; private set; }
    public bool IsPublic { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int DownloadCount { get; private set; }
    public string? Checksum { get; private set; }

    // Private constructor for EF Core
    private File() { }

    public static File Create(
        string fileName,
        string originalFileName,
        string filePath,
        string contentType,
        long fileSize,
        string fileExtension,
        FileType type,
        Guid? userId = null,
        Guid? orderId = null,
        Guid? productId = null,
        string? description = null,
        bool isPublic = false,
        DateTime? expiresAt = null)
    {
        return new File
        {
            FileName = fileName,
            OriginalFileName = originalFileName,
            FilePath = filePath,
            ContentType = contentType,
            FileSize = fileSize,
            FileExtension = fileExtension,
            Type = type,
            Status = FileStatus.Uploaded,
            UserId = userId,
            OrderId = orderId,
            ProductId = productId,
            Description = description,
            IsPublic = isPublic,
            ExpiresAt = expiresAt,
            DownloadCount = 0,
            Checksum = string.Empty,
            Metadata = new Dictionary<string, object>()
        };
    }

    public void UpdateStatus(FileStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementDownloadCount()
    {
        DownloadCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateThumbnail(string thumbnailPath)
    {
        ThumbnailPath = thumbnailPath;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetExpiration(DateTime? expiresAt)
    {
        ExpiresAt = expiresAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
    public bool CanBeDownloaded => Status == FileStatus.Processed && !IsExpired;
}
