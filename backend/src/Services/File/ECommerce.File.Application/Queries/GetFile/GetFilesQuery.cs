using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Application.Queries.GetFile;

public class GetFilesQuery : IRequest<ApiResponse<List<FileDto>>>
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
