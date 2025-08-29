using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Brand;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Brand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, ApiResponse<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBrandCommandHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<BrandDto>> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing brand
            var brand = await _brandRepository.GetByIdAsync(request.Id);
            if (brand == null)
            {
                return ApiResponse<BrandDto>.ErrorResult("Brand not found");
            }

            // Check if new name already exists (excluding current brand)
            if (await _brandRepository.ExistsByNameAsync(request.Name, request.Id))
            {
                return ApiResponse<BrandDto>.ErrorResult("Brand name already exists");
            }

            // Update brand
            brand.Update(request.Name, request.Description, request.LogoUrl, request.Website);
            brand.SetActive(request.IsActive);

            // Save changes
            await _brandRepository.UpdateAsync(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            return ApiResponse<BrandDto>.ErrorResult($"Error updating brand: {ex.Message}");
        }
    }
}
