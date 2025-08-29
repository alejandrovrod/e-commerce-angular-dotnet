using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Inventory;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Inventory;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, ApiResponse<List<InventoryDto>>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetInventoryQueryHandler(IInventoryRepository inventoryRepository, IUnitOfWork unitOfWork)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<InventoryDto>>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var inventory = await _inventoryRepository.GetAllAsync();
            
            var inventoryDtos = inventory.Select(i => new InventoryDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ReservedQuantity = i.ReservedQuantity,
                AvailableQuantity = i.AvailableQuantity,
                Location = i.Location,
                LastUpdated = i.LastUpdated,
                CreatedAt = i.CreatedAt
            }).ToList();

            return ApiResponse<List<InventoryDto>>.SuccessResult(inventoryDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<InventoryDto>>.ErrorResult($"Error retrieving inventory: {ex.Message}");
        }
    }
}
