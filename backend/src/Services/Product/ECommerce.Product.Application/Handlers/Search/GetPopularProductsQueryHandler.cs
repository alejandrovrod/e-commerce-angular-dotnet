using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Search;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Search;

public class GetPopularProductsQueryHandler : IRequestHandler<GetPopularProductsQuery, ApiResponse<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetPopularProductsQueryHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<ProductDto>>> Handle(GetPopularProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var popularProducts = products.Where(p => p.IsFeatured).Take(request.Limit).ToList();

            var productDtos = popularProducts.Select(p => new ProductDto
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
            return ApiResponse<List<ProductDto>>.ErrorResult($"Popular products error: {ex.Message}");
        }
    }
}
