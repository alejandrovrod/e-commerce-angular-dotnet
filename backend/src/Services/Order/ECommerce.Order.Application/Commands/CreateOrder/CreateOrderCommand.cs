using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;

namespace ECommerce.Order.Application.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<ApiResponse<OrderDto>>
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public CreateAddressDto ShippingAddress { get; set; } = new();
    public CreateAddressDto? BillingAddress { get; set; }
    public string? CouponCode { get; set; }
    public Guid? ShippingMethodId { get; set; }
    public string? Notes { get; set; }
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal? Weight { get; set; }
}

public class CreateAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
}



