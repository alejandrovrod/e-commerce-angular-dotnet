using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands.Inventory;

public class ReserveStockCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string ReservationId { get; set; } = string.Empty;
}
