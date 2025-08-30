using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.File.Application.Commands.DeleteFile;

public class DeleteFileCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
