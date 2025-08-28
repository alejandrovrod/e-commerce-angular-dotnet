using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Inventory;

public class UpdateInventoryCommand : IRequest<ApiResponse<InventoryDto>>
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Location { get; set; } = string.Empty;
}
