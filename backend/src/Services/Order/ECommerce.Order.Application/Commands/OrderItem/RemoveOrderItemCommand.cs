using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;

namespace ECommerce.Order.Application.Commands.OrderItem;

public class RemoveOrderItemCommand : IRequest<ApiResponse<OrderDto>>
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
}




