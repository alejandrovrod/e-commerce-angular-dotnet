using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Brand;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Brand;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, ApiResponse<List<BrandDto>>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetBrandsQueryHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<BrandDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var brands = await _brandRepository.GetAllAsync();
            
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
