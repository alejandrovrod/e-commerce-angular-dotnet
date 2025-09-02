using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Application.Queries.GetOrder;

public class GetOrdersQuery : IRequest<ApiResponse<List<OrderDto>>>
{
    public Guid? UserId { get; set; }
    public OrderStatus? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}




