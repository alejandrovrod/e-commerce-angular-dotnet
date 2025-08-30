using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Domain.Enums;

namespace ECommerce.File.Application.Commands.UpdateFileStatus;

public class UpdateFileStatusCommand : IRequest<ApiResponse<FileDto>>
{
    public Guid Id { get; set; }
    public FileStatus Status { get; set; }
    public string? Reason { get; set; }
}
