using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Inventory;

public class AdjustStockCommand : IRequest<ApiResponse<InventoryDto>>
{
    public Guid Id { get; set; }
    public int QuantityChange { get; set; }
    public string Reason { get; set; } = string.Empty;
}
