using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Application.Commands.CreateFile;

public class CreateFileCommand : IRequest<ApiResponse<FileDto>>
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
