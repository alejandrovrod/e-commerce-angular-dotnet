using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Application.Commands.OrderStatus;
using ECommerce.Order.Application.Interfaces;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Application.Handlers.OrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, ApiResponse<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<OrderDto>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);

            if (order == null)
            {
                return ApiResponse<OrderDto>.ErrorResult("Order not found");
            }

            // Update order status using the appropriate method based on the new status
            switch (request.NewStatus)
            {
                case ECommerce.Order.Domain.Enums.OrderStatus.Confirmed:
                    order.Confirm();
                    break;
                case ECommerce.Order.Domain.Enums.OrderStatus.Processing:
                    order.Process();
                    break;
                case ECommerce.Order.Domain.Enums.OrderStatus.Shipped:
                    order.Ship(request.TrackingNumber);
                    break;
                case ECommerce.Order.Domain.Enums.OrderStatus.Delivered:
                    order.Deliver();
                    break;
                case ECommerce.Order.Domain.Enums.OrderStatus.Cancelled:
                    order.Cancel(request.Reason);
                    break;
                case ECommerce.Order.Domain.Enums.OrderStatus.Refunded:
                    order.Refund(request.Reason);
                    break;
                default:
                    // For other statuses, we'll need to add a method to handle them
                    // For now, we'll use reflection to set the status directly
                    var statusProperty = typeof(ECommerce.Order.Domain.Entities.Order).GetProperty("Status");
                    if (statusProperty != null)
                    {
                        statusProperty.SetValue(order, request.NewStatus);
                        // Add status history
                        var addStatusHistoryMethod = typeof(ECommerce.Order.Domain.Entities.Order).GetMethod("AddStatusHistory", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        addStatusHistoryMethod?.Invoke(order, new object[] { request.NewStatus, request.Reason });
                    }
                    break;
            }

            // Save changes
            var updatedOrder = await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var orderDto = MapToDto(updatedOrder);

            return ApiResponse<OrderDto>.SuccessResult(orderDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<OrderDto>.ErrorResult($"Error updating order status: {ex.Message}");
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
