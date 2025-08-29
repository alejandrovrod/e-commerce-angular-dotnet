using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Application.Queries.GetOrder;
using ECommerce.Order.Application.Interfaces;

namespace ECommerce.Order.Application.Handlers.GetOrder;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResponse<List<OrderDto>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetOrdersQueryHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderRepository.GetAllAsync();

            // Apply filters
            if (request.UserId.HasValue)
            {
                orders = orders.Where(o => o.UserId == request.UserId.Value).ToList();
            }

            if (request.Status.HasValue)
            {
                orders = orders.Where(o => o.Status == request.Status.Value).ToList();
            }

            if (request.FromDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt >= request.FromDate.Value).ToList();
            }

            if (request.ToDate.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt <= request.ToDate.Value).ToList();
            }

            // Apply pagination
            var paginatedOrders = orders
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var orderDtos = paginatedOrders.Select(MapToDto).ToList();

            return ApiResponse<List<OrderDto>>.SuccessResult(orderDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<OrderDto>>.ErrorResult($"Error retrieving orders: {ex.Message}");
        }
    }

    private OrderDto MapToDto(ECommerce.Order.Domain.Entities.Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId,
            Status = order.Status,
            Items = order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductSku = item.ProductSku,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                Weight = item.Weight
            }).ToList(),
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingAddress.Street,
                City = order.ShippingAddress.City,
                State = order.ShippingAddress.State,
                PostalCode = order.ShippingAddress.PostalCode,
                Country = order.ShippingAddress.Country,
                AddressLine2 = order.ShippingAddress.AddressLine2,
                FullAddress = order.ShippingAddress.FullAddress
            },
            BillingAddress = order.BillingAddress != null ? new AddressDto
            {
                Street = order.BillingAddress.Street,
                City = order.BillingAddress.City,
                State = order.BillingAddress.State,
                PostalCode = order.BillingAddress.PostalCode,
                Country = order.BillingAddress.Country,
                AddressLine2 = order.BillingAddress.AddressLine2,
                FullAddress = order.BillingAddress.FullAddress
            } : null,
            Pricing = new OrderPricingDto
            {
                Subtotal = order.Pricing.Subtotal,
                Tax = order.Pricing.Tax,
                ShippingCost = order.Pricing.ShippingCost,
                DiscountAmount = order.Pricing.DiscountAmount,
                Total = order.Pricing.Total
            },
            CouponCode = order.CouponCode,
            ShippingMethodId = order.ShippingMethodId,
            PaymentInfo = order.PaymentInfo != null ? new PaymentInfoDto
            {
                PaymentMethod = order.PaymentInfo.PaymentMethod,
                PaymentId = order.PaymentInfo.PaymentId,
                TransactionId = order.PaymentInfo.TransactionId
            } : null,
            StatusHistory = order.StatusHistory.Select(history => new OrderStatusHistoryDto
            {
                Status = history.Status,
                Reason = history.Reason,
                Timestamp = history.Timestamp
            }).ToList(),
            Notes = order.Notes,
            ShippedAt = order.ShippedAt,
            DeliveredAt = order.DeliveredAt,
            TrackingNumber = order.TrackingNumber,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }
}
