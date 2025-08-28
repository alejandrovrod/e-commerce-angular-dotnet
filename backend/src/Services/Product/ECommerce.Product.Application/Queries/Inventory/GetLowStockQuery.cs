using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Inventory;

public class GetLowStockQuery : IRequest<ApiResponse<List<InventoryDto>>>
{
    public int Threshold { get; set; } = 10;
    public string? Location { get; set; }
}
