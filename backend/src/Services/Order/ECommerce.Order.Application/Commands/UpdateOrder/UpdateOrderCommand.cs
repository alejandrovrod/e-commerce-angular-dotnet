using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Application.DTOs;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Application.Commands.UpdateOrder;

public class UpdateOrderCommand : IRequest<ApiResponse<OrderDto>>
{
    public Guid Id { get; set; }
    public string? Notes { get; set; }
    public string? TrackingNumber { get; set; }
    public UpdateAddressDto? ShippingAddress { get; set; }
    public UpdateAddressDto? BillingAddress { get; set; }
}

public class UpdateAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
}



