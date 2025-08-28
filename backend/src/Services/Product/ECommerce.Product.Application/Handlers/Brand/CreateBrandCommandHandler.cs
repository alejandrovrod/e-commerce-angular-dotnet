using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Brand;

namespace ECommerce.Product.Application.Handlers.Brand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, ApiResponse<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;

    public CreateBrandCommandHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<ApiResponse<BrandDto>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if brand name already exists
            if (await _brandRepository.ExistsByNameAsync(request.Name))
            {
                return ApiResponse<BrandDto>.ErrorResult("Brand name already exists");
            }

            // Create new brand
            var brand = new ECommerce.Product.Domain.Entities.Brand(request.Name, request.Description, request.LogoUrl, request.Website);
            
            // Save to repository
            await _brandRepository.AddAsync(brand);
            // Note: SaveChangesAsync is handled by the Unit of Work pattern or called from the controller

            // Map to DTO
            var brandDto = new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                LogoUrl = brand.LogoUrl,
                Website = brand.Website,
                IsActive = brand.IsActive,
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            };

            return ApiResponse<BrandDto>.SuccessResult(brandDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<BrandDto>.ErrorResult($"Error creating brand: {ex.Message}");
        }
    }
}
