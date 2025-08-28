using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Inventory;

public class GetInventoryQuery : IRequest<ApiResponse<List<InventoryDto>>>
{
    public Guid? ProductId { get; set; }
    public string? Location { get; set; }
    public bool? InStock { get; set; }
}
