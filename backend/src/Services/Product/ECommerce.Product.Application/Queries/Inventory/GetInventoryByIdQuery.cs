using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Inventory;

public class GetInventoryByIdQuery : IRequest<ApiResponse<InventoryDto>>
{
    public Guid Id { get; set; }
}
