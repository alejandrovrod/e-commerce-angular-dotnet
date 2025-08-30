using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;

namespace ECommerce.File.Application.Queries.GetFile;

public class GetFilesByUserQuery : IRequest<ApiResponse<List<FileDto>>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
