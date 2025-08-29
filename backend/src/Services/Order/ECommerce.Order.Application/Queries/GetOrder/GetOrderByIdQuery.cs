using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;

namespace ECommerce.Order.Application.Queries.GetOrder;

public class GetOrderByIdQuery : IRequest<ApiResponse<OrderDto>>
{
    public Guid Id { get; set; }
}

