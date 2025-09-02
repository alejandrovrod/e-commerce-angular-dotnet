using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.Commands.Inventory;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Application.Handlers.Inventory;

public class AdjustStockCommandHandler : IRequestHandler<ECommerce.Product.Application.Commands.Inventory.AdjustStockCommand, ApiResponse<bool>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockCommandHandler(
        IInventoryRepository inventoryRepository, 
        IInventoryMovementRepository inventoryMovementRepository,
        IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(ECommerce.Product.Application.Commands.Inventory.AdjustStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar el inventario por ProductId
            var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
            
            if (inventory == null)
            {
                return ApiResponse<bool>.ErrorResult($"No se encontró inventario para el producto con ID: {request.ProductId}");
            }

            // Obtener la cantidad anterior para el mensaje
            var previousQuantity = inventory.Quantity;
            var newQuantity = previousQuantity + request.Quantity;

            // Validar que la cantidad no sea negativa
            if (newQuantity < 0)
            {
                return ApiResponse<bool>.ErrorResult("La cantidad resultante no puede ser negativa");
            }

            // Usar el método de la entidad para ajustar el stock
            inventory.AdjustStock(request.Quantity, request.Reason);

            // Crear el registro de movimiento en el historial
            var movement = InventoryMovement.CreateAdjustment(
                request.ProductId,
                request.Quantity,
                previousQuantity,
                newQuantity,
                request.Reason,
                request.Notes,
                null, // TODO: Obtener del contexto de usuario cuando esté disponible
                "System" // TODO: Obtener del contexto de usuario cuando esté disponible
            );

            // Guardar los cambios
            await _inventoryRepository.UpdateAsync(inventory);
            await _inventoryMovementRepository.AddAsync(movement);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, $"Stock ajustado exitosamente. Cantidad anterior: {previousQuantity}, Nueva cantidad: {newQuantity}");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error al ajustar el stock: {ex.Message}");
        }
    }
}
