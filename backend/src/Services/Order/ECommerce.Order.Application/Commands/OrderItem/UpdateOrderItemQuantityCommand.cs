using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;

namespace ECommerce.Order.Application.Commands.OrderItem;

public class UpdateOrderItemQuantityCommand : IRequest<ApiResponse<OrderDto>>
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}




