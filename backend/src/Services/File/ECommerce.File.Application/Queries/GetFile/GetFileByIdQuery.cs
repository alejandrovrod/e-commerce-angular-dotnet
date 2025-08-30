using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;

namespace ECommerce.File.Application.Queries.GetFile;

public class GetFileByIdQuery : IRequest<ApiResponse<FileDto>>
{
    public Guid Id { get; set; }
}
