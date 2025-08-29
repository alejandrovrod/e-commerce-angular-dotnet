using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands;

public class DeleteProductCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}






