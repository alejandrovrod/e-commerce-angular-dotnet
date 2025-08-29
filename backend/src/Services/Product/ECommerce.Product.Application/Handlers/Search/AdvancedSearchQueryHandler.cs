using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Search;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Search;

public class AdvancedSearchQueryHandler : IRequestHandler<AdvancedSearchQuery, ApiResponse<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdvancedSearchQueryHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<ProductDto>>> Handle(AdvancedSearchQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllAsync();

            // Apply filters
            if (!string.IsNullOrEmpty(request.Name))
                products = products.Where(p => p.Name.Contains(request.Name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrEmpty(request.Description))
                products = products.Where(p => p.Description.Contains(request.Description, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.MinPrice.HasValue)
                products = products.Where(p => p.Price.Amount >= request.MinPrice.Value).ToList();

            if (request.MaxPrice.HasValue)
                products = products.Where(p => p.Price.Amount <= request.MaxPrice.Value).ToList();

            if (!string.IsNullOrEmpty(request.Category))
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryEntity = categories.FirstOrDefault(c => c.Name.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
                if (categoryEntity != null)
                    products = products.Where(p => p.CategoryId == categoryEntity.Id).ToList();
            }

            if (!string.IsNullOrEmpty(request.Brand))
                products = products.Where(p => p.Brand.Equals(request.Brand, StringComparison.OrdinalIgnoreCase)).ToList();

            if (request.IsFeatured.HasValue)
                products = products.Where(p => p.IsFeatured == request.IsFeatured.Value).ToList();

            if (request.IsDigital.HasValue)
                products = products.Where(p => p.IsDigital == request.IsDigital.Value).ToList();

            if (request.Status.HasValue)
                products = products.Where(p => p.Status == request.Status.Value).ToList();

            // Apply pagination
            var paginatedProducts = products
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var productDtos = paginatedProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                SKU = p.SKU,
                Price = p.Price.Amount,
                Brand = p.Brand,
                CategoryId = p.CategoryId,
                Status = p.Status,
                IsFeatured = p.IsFeatured,
                IsDigital = p.IsDigital,
                RequiresShipping = p.RequiresShipping,
                IsTaxable = p.IsTaxable,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();

            return ApiResponse<List<ProductDto>>.SuccessResult(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductDto>>.ErrorResult($"Advanced search error: {ex.Message}");
        }
    }
}
