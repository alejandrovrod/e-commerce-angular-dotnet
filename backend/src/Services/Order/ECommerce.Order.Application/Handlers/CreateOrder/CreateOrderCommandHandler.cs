using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Application.Commands.CreateOrder;
using ECommerce.Order.Application.Interfaces;
using ECommerce.Order.Domain.Entities;
using ECommerce.Order.Domain.ValueObjects;

namespace ECommerce.Order.Application.Handlers.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResponse<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Convert DTOs to domain objects
            var orderItems = request.Items.Select(item => 
                ECommerce.Order.Domain.ValueObjects.OrderItem.Create(
                    item.ProductId, 
                    item.ProductName, 
                    item.UnitPrice, 
                    item.Quantity, 
                    item.ProductSku, 
                    item.Weight
                )).ToList();

            var shippingAddress = new Address(
                request.ShippingAddress.Street,
                request.ShippingAddress.City,
                request.ShippingAddress.State,
                request.ShippingAddress.PostalCode,
                request.ShippingAddress.Country,
                request.ShippingAddress.AddressLine2
            );

            Address? billingAddress = null;
            if (request.BillingAddress != null)
            {
                billingAddress = new Address(
                    request.BillingAddress.Street,
                    request.BillingAddress.City,
                    request.BillingAddress.State,
                    request.BillingAddress.PostalCode,
                    request.BillingAddress.Country,
                    request.BillingAddress.AddressLine2
                );
            }

            // Create order
            var order = ECommerce.Order.Domain.Entities.Order.Create(request.UserId, orderItems, shippingAddress, billingAddress);

            // Apply coupon if provided
            if (!string.IsNullOrEmpty(request.CouponCode))
            {
                // Note: In a real application, you would validate the coupon and get the discount amount
                order.ApplyCoupon(request.CouponCode, 0); // Placeholder for discount amount
            }

            // Set shipping method if provided
            if (request.ShippingMethodId.HasValue)
            {
                // Note: In a real application, you would get the shipping cost from the shipping method
                order.SetShippingMethod(request.ShippingMethodId.Value, 0); // Placeholder for shipping cost
            }

            // Add notes if provided
            if (!string.IsNullOrEmpty(request.Notes))
            {
                order.AddNotes(request.Notes);
            }

            // Save order
            var createdOrder = await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var orderDto = MapToDto(createdOrder);

            return ApiResponse<OrderDto>.SuccessResult(orderDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<OrderDto>.ErrorResult($"Error creating order: {ex.Message}");
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
