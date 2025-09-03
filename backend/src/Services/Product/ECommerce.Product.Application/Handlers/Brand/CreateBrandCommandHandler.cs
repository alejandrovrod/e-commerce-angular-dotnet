using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Brand;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Brand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, ApiResponse<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBrandCommandHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
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
            var brand = new ECommerce.Product.Domain.Entities.Brand(request.Name, request.Description, request.LogoUrl, request.Website, request.Country, request.FoundedYear);
            
            // Save to repository
            await _brandRepository.AddAsync(brand);
            
            // Save changes to database using Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var brandDto = new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                LogoUrl = brand.LogoUrl,
                Website = brand.Website,
                Country = brand.Country,
                FoundedYear = brand.FoundedYear,
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
