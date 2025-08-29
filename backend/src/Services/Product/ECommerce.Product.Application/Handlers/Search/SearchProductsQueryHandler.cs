using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Search;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Search;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, ApiResponse<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SearchProductsQueryHandler(
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

    public async Task<ApiResponse<List<ProductDto>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.SearchAsync(request.Query, request.Page, request.PageSize);
            
            // Filter by category if specified
            if (!string.IsNullOrEmpty(request.Category))
            {
                var categories = await _categoryRepository.GetAllAsync();
                var categoryEntity = categories.FirstOrDefault(c => c.Name.Equals(request.Category, StringComparison.OrdinalIgnoreCase));
                if (categoryEntity != null)
                {
                    products = products.Where(p => p.CategoryId == categoryEntity.Id).ToList();
                }
            }

            // Filter by brand if specified
            if (!string.IsNullOrEmpty(request.Brand))
            {
                products = products.Where(p => p.Brand.Equals(request.Brand, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var productDtos = products.Select(p => new ProductDto
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
            return ApiResponse<List<ProductDto>>.ErrorResult($"Search error: {ex.Message}");
        }
    }
}
