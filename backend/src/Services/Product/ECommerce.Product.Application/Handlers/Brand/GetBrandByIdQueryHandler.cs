using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Queries.Brand;

namespace ECommerce.Product.Application.Handlers.Brand;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, ApiResponse<BrandDto>>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandByIdQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<ApiResponse<BrandDto>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var brand = await _brandRepository.GetByIdAsync(request.Id);
            if (brand == null)
            {
                return ApiResponse<BrandDto>.ErrorResult("Brand not found");
            }

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
            return ApiResponse<BrandDto>.ErrorResult($"Error retrieving brand: {ex.Message}");
        }
    }
}
