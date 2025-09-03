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
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetBrandsQueryHandler(IBrandRepository brandRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<BrandDto>>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var brands = await _brandRepository.GetAllAsync();
            
            var brandDtos = new List<BrandDto>();
            
            foreach (var brand in brands)
            {
                // Contar productos que tienen esta marca
                var productCount = await _productRepository.CountProductsByBrandAsync(brand.Name);
                Console.WriteLine($"🔍 Brand: {brand.Name}, ProductCount: {productCount}");
                
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
                    UpdatedAt = brand.UpdatedAt,
                    ProductCount = productCount,
                    Products = new List<ProductDto>() // Lista vacía, solo necesitamos el conteo
                };
                
                brandDtos.Add(brandDto);
            }

            return ApiResponse<List<BrandDto>>.SuccessResult(brandDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<BrandDto>>.ErrorResult($"Error retrieving brands: {ex.Message}");
        }
    }
}
