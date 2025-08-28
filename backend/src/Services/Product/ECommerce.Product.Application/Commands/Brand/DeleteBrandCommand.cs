using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands.Brand;

public class DeleteBrandCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
