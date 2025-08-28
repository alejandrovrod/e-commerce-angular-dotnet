using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Queries.Brand;

namespace ECommerce.Product.Application.Handlers.Brand;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, ApiResponse<List<BrandDto>>>
{
    private readonly IBrandRepository _brandRepository;

    public GetBrandsQueryHandler(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<ApiResponse<List<BrandDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<ECommerce.Product.Domain.Entities.Brand> brands;

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                brands = await _brandRepository.SearchAsync(request.SearchTerm);
            }
            else if (request.IsActive.HasValue)
            {
                brands = request.IsActive.Value 
                    ? await _brandRepository.GetActiveAsync()
                    : await _brandRepository.GetAllAsync();
            }
            else
            {
                brands = await _brandRepository.GetAllAsync();
            }

            // Filter by active status if specified
            if (request.IsActive.HasValue)
            {
                brands = brands.Where(b => b.IsActive == request.IsActive.Value).ToList();
            }

            // Map to DTOs
            var brandDtos = brands.Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoUrl = b.LogoUrl,
                Website = b.Website,
                IsActive = b.IsActive,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            }).ToList();

            return ApiResponse<List<BrandDto>>.SuccessResult(brandDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BrandDto>>.ErrorResult($"Error retrieving brands: {ex.Message}");
        }
    }
}
