using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Inventory;

public class AdjustStockCommand : IRequest<ApiResponse<bool>>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
