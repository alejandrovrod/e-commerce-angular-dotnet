using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Application.DTOs;

public class FileDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileExtension { get; set; } = string.Empty;
    public FileType Type { get; set; }
    public FileStatus Status { get; set; }
    public Guid? UserId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public string? Description { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public string? ThumbnailPath { get; set; }
    public bool IsPublic { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public int DownloadCount { get; set; }
    public string? Checksum { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsExpired { get; set; }
    public bool CanBeDownloaded { get; set; }
}

public class CreateFileDto
{
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileExtension { get; set; } = string.Empty;
    public FileType Type { get; set; }
    public Guid? UserId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UpdateFileDto
{
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class UpdateFileStatusDto
{
    public FileStatus Status { get; set; }
    public string? Reason { get; set; }
}

public class FileFilterDto
{
    public Guid? UserId { get; set; }
    public Guid? OrderId { get; set; }
    public Guid? ProductId { get; set; }
    public FileType? Type { get; set; }
    public FileStatus? Status { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class FileUploadResponseDto
{
    public Guid FileId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string UploadUrl { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
