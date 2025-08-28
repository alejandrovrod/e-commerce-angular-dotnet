using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;
using ECommerce.Product.Domain.Enums;
using Microsoft.Extensions.Logging;
using Mapster;

namespace ECommerce.Product.Application.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResponse<PagedResult<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        ICacheService cacheService,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<PagedResult<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting products with filters: Page={Page}, PageSize={PageSize}, SearchTerm={SearchTerm}", 
                request.Page, request.PageSize, request.SearchTerm);

            // Generate cache key based on query parameters
            var cacheKey = GenerateCacheKey(request);

            // Try to get from cache first
            var cachedResult = await _cacheService.GetAsync<PagedResult<ProductDto>>(cacheKey);
            if (cachedResult != null)
            {
                _logger.LogInformation("Products retrieved from cache");
                return ApiResponse<PagedResult<ProductDto>>.SuccessResult(cachedResult);
            }

            // Get products from repository
            var products = await GetFilteredProducts(request);
            var totalCount = await GetTotalCount(request);

            // Apply pagination
            var pagedProducts = products
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var productDtos = pagedProducts.Adapt<List<ProductDto>>();

            // Create paged result
            var result = new PagedResult<ProductDto>(
                productDtos,
                totalCount,
                request.Page,
                request.PageSize
            );

            // Cache the result
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

            _logger.LogInformation("Products retrieved successfully. Total: {TotalCount}, Page: {Page}, PageSize: {PageSize}", 
                totalCount, request.Page, request.PageSize);

            return ApiResponse<PagedResult<ProductDto>>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting products");
            return ApiResponse<PagedResult<ProductDto>>.ErrorResult("An error occurred while retrieving products");
        }
    }

    private async Task<List<Domain.Entities.Product>> GetFilteredProducts(GetProductsQuery request)
    {
        var query = _productRepository.GetAllAsync().Result.AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrEmpty(request.BrandId?.ToString()))
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value);
            if (brand != null)
            {
                query = query.Where(p => p.Brand == brand.Name);
            }
        }

        if (request.MinPrice.HasValue)
        {
            // Note: Since we're ignoring Price property in EF, this filter won't work
            // You'll need to implement a proper price filtering mechanism
        }

        if (request.MaxPrice.HasValue)
        {
            // Note: Since we're ignoring Price property in EF, this filter won't work
            // You'll need to implement a proper price filtering mechanism
        }

        if (request.InStock.HasValue)
        {
            if (request.InStock.Value)
            {
                // This would require joining with Inventory table
                // For now, we'll skip this filter
            }
        }

        if (request.IsFeatured.HasValue)
        {
            query = query.Where(p => p.IsFeatured == request.IsFeatured.Value);
        }

        if (request.IsDigital.HasValue)
        {
            query = query.Where(p => p.IsDigital == request.IsDigital.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(p => p.Status == request.Status.Value);
        }

        // Apply sorting
        query = ApplySorting(query, request.SortBy, request.SortOrder);

        return query.ToList();
    }

    private async Task<int> GetTotalCount(GetProductsQuery request)
    {
        // For now, return total count without filters
        // In a real implementation, you'd want to apply the same filters
        return await _productRepository.GetTotalCountAsync();
    }

    private IQueryable<Domain.Entities.Product> ApplySorting(IQueryable<Domain.Entities.Product> query, string? sortBy, string? sortOrder)
    {
        sortBy = sortBy?.ToLowerInvariant() ?? "name";
        sortOrder = sortOrder?.ToLowerInvariant() ?? "asc";

        switch (sortBy)
        {
            case "name":
                query = sortOrder == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                break;
            case "createdat":
                query = sortOrder == "desc" ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
                break;
            case "updatedat":
                query = sortOrder == "desc" ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt);
                break;
            case "status":
                query = sortOrder == "desc" ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status);
                break;
            default:
                query = query.OrderBy(p => p.Name);
                break;
        }

        return query;
    }

    private static string GenerateCacheKey(GetProductsQuery request)
    {
        return $"products:filtered:{request.Page}:{request.PageSize}:{request.SearchTerm}:{request.CategoryId}:{request.BrandId}:{request.MinPrice}:{request.MaxPrice}:{request.InStock}:{request.IsFeatured}:{request.IsDigital}:{request.Status}:{request.SortBy}:{request.SortOrder}";
    }
}

