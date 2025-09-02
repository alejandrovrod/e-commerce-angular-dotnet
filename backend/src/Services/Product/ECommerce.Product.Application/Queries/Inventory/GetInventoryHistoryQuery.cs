using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Inventory;

public class GetInventoryHistoryQuery : IRequest<ApiResponse<List<InventoryMovementDto>>>
{
    public Guid? ProductId { get; set; }
    public string? MovementType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}
