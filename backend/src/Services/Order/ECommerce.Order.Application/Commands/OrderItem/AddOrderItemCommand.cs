using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;

namespace ECommerce.Order.Application.Commands.OrderItem;

public class AddOrderItemCommand : IRequest<ApiResponse<OrderDto>>
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal? Weight { get; set; }
}




