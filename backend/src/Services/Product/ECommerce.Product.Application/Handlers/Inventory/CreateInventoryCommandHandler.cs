using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Inventory;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Inventory;

public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, ApiResponse<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryCommandHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<InventoryDto>> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if inventory already exists for this product
            if (await _inventoryRepository.ExistsByProductIdAsync(request.ProductId))
            {
                return ApiResponse<InventoryDto>.ErrorResult("Inventory already exists for this product");
            }

            // Create new inventory
            var inventory = new ECommerce.Product.Domain.Entities.Inventory(request.ProductId, request.Quantity, request.Location);
            
            // Save to repository
            await _inventoryRepository.AddAsync(inventory);
            
            // Save changes to database using Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var inventoryDto = new InventoryDto
            {
                Id = inventory.Id,
                ProductId = inventory.ProductId,
                Quantity = inventory.Quantity,
                ReservedQuantity = inventory.ReservedQuantity,
                AvailableQuantity = inventory.AvailableQuantity,
                Location = inventory.Location,
                LastUpdated = inventory.LastUpdated,
                CreatedAt = inventory.CreatedAt
            };

            return ApiResponse<InventoryDto>.SuccessResult(inventoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<InventoryDto>.ErrorResult($"Error creating inventory: {ex.Message}");
        }
    }
}
