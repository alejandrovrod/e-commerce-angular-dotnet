using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Inventory;

public class CreateInventoryCommand : IRequest<ApiResponse<InventoryDto>>
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
}
