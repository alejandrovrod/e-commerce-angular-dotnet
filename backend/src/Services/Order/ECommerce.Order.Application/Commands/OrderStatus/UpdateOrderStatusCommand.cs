using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Application.Commands.OrderStatus;

public class UpdateOrderStatusCommand : IRequest<ApiResponse<OrderDto>>
{
    public Guid Id { get; set; }
    public ECommerce.Order.Domain.Enums.OrderStatus NewStatus { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
}
