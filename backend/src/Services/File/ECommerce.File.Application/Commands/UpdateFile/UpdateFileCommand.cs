using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;

namespace ECommerce.File.Application.Commands.UpdateFile;

public class UpdateFileCommand : IRequest<ApiResponse<FileDto>>
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
