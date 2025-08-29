using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Inventory;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Inventory;

public class GetInventoryByIdQueryHandler : IRequestHandler<GetInventoryByIdQuery, ApiResponse<InventoryDto>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetInventoryByIdQueryHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<InventoryDto>> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var inventory = await _inventoryRepository.GetByIdAsync(request.Id);
            
            if (inventory == null)
            {
                return ApiResponse<InventoryDto>.ErrorResult("Inventory not found");
            }

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
            return ApiResponse<InventoryDto>.ErrorResult($"Error retrieving inventory: {ex.Message}");
        }
    }
}
