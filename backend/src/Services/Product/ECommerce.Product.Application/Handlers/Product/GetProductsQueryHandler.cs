using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.GetProducts;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Product;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResponse<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetProductsQueryHandler(
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

    public async Task<ApiResponse<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"🔍 GetProductsQueryHandler: Iniciando consulta de productos");
            Console.WriteLine($"🔍 GetProductsQueryHandler: Cultura actual: {System.Globalization.CultureInfo.CurrentCulture.Name}");
            Console.WriteLine($"🔍 GetProductsQueryHandler: Cultura UI actual: {System.Globalization.CultureInfo.CurrentUICulture.Name}");
            
            var products = await _productRepository.GetAllAsync();
            Console.WriteLine($"🔍 GetProductsQueryHandler: Productos obtenidos: {products.Count()}");
            
            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                products = products.Where(p => 
                    p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (request.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == request.CategoryId.Value).ToList();
            }

            if (!string.IsNullOrEmpty(request.Brand))
            {
                products = products.Where(p => p.Brand.Equals(request.Brand, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (request.IsFeatured.HasValue)
            {
                products = products.Where(p => p.IsFeatured == request.IsFeatured.Value).ToList();
            }

            if (request.Status.HasValue)
            {
                products = products.Where(p => p.Status == request.Status.Value).ToList();
            }

            // Apply pagination
            var paginatedProducts = products
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            Console.WriteLine($"🔍 GetProductsQueryHandler: Iniciando mapeo a DTOs");
            var productDtos = paginatedProducts.Select(p => 
            {
                Console.WriteLine($"🔍 GetProductsQueryHandler: Mapeando producto {p.Name}, Price: {p.Price?.Amount}");
                return new ProductDto
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
                };
            }).ToList();

            return ApiResponse<List<ProductDto>>.SuccessResult(productDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductDto>>.ErrorResult($"Error retrieving products: {ex.Message}");
        }
    }
}
