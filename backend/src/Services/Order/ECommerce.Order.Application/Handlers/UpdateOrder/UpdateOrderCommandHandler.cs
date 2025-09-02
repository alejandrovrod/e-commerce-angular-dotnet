using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Application.Commands.UpdateOrder;
using ECommerce.Order.Application.Interfaces;
using ECommerce.Order.Domain.ValueObjects;

namespace ECommerce.Order.Application.Handlers.UpdateOrder;

public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, ApiResponse<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<OrderDto>> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing order
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null)
            {
                return ApiResponse<OrderDto>.ErrorResult("Order not found");
            }

            // Update notes if provided
            if (!string.IsNullOrEmpty(request.Notes))
            {
                order.AddNotes(request.Notes);
            }

            // Update tracking number if provided
            if (!string.IsNullOrEmpty(request.TrackingNumber))
            {
                // Note: In a real application, you would have a method to update tracking number
                // For now, we'll just update the notes
                order.AddNotes($"Tracking number updated: {request.TrackingNumber}");
            }

            // Update addresses if provided
            if (request.ShippingAddress != null)
            {
                // Note: In a real application, you would have methods to update addresses
                // For now, we'll just update the notes
                order.AddNotes($"Shipping address updated: {request.ShippingAddress.Street}, {request.ShippingAddress.City}");
            }

            if (request.BillingAddress != null)
            {
                // Note: In a real application, you would have methods to update addresses
                // For now, we'll just update the notes
                order.AddNotes($"Billing address updated: {request.BillingAddress.Street}, {request.BillingAddress.City}");
            }

            // Save changes
            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var orderDto = MapToDto(order);

            return ApiResponse<OrderDto>.SuccessResult(orderDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<OrderDto>.ErrorResult($"Error updating order: {ex.Message}");
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




