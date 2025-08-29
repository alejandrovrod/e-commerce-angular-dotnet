using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands;

public class UpdateInventoryCommand : IRequest<ProductDto?>
{
    public Guid ProductId { get; set; }
    public int Stock { get; set; }
    public int LowStockThreshold { get; set; } = 5;
    public bool TrackQuantity { get; set; } = true;
    public bool AllowBackorder { get; set; } = false;
}







