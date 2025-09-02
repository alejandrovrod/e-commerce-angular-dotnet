using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;

namespace ECommerce.Order.Application.Queries.GetOrder;

public class GetOrdersByUserQuery : IRequest<ApiResponse<List<OrderDto>>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}




