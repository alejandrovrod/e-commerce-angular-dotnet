using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Inventory;
using ECommerce.Product.Domain.Repositories;

namespace ECommerce.Product.Application.Handlers.Inventory;

public class GetInventoryHistoryQueryHandler : IRequestHandler<GetInventoryHistoryQuery, ApiResponse<List<InventoryMovementDto>>>
{
    private readonly IInventoryMovementRepository _inventoryMovementRepository;

    public GetInventoryHistoryQueryHandler(IInventoryMovementRepository inventoryMovementRepository)
    {
        _inventoryMovementRepository = inventoryMovementRepository;
    }

    public async Task<ApiResponse<List<InventoryMovementDto>>> Handle(GetInventoryHistoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener movimientos reales de la base de datos
            var movements = await _inventoryMovementRepository.GetFilteredAsync(
                productId: request.ProductId,
                movementType: request.MovementType,
                fromDate: request.DateFrom,
                toDate: request.DateTo,
                userId: request.UserId,
                page: request.Page,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken
            );

            // Mapear a DTOs
            var movementDtos = movements.Select(m => new InventoryMovementDto
            {
                Id = m.Id,
                ProductId = m.ProductId,
                ProductName = m.Product?.Name,
                MovementType = m.MovementType,
                Quantity = m.Quantity,
                PreviousQuantity = m.PreviousQuantity,
                NewQuantity = m.NewQuantity,
                Reason = m.Reason,
                Notes = m.Notes,
                UserId = m.UserId,
                UserName = m.UserName,
                OrderId = m.OrderId,
                CreatedAt = m.CreatedAt
            }).ToList();

            return ApiResponse<List<InventoryMovementDto>>.SuccessResult(movementDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<InventoryMovementDto>>.ErrorResult($"Error al obtener el historial de inventario: {ex.Message}");
        }
    }
}
