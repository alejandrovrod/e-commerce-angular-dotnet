using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Inventory;

public class GetInventoryByProductIdQuery : IRequest<ApiResponse<InventoryDto>>
{
    public Guid ProductId { get; set; }
}
