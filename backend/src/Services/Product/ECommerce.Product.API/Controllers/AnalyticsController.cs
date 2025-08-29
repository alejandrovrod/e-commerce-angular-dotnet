using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public AnalyticsController(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IReviewRepository reviewRepository,
        IInventoryRepository inventoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _reviewRepository = reviewRepository;
        _inventoryRepository = inventoryRepository;
    }

    [HttpGet("products")]
    public async Task<ActionResult<ApiResponse<object>>> GetProductAnalytics()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var brands = await _brandRepository.GetAllAsync();

            var analytics = new
            {
                TotalProducts = products.Count(),
                ActiveProducts = products.Count(p => p.Status == Domain.Enums.ProductStatus.Active),
                DraftProducts = products.Count(p => p.Status == Domain.Enums.ProductStatus.Draft),
                FeaturedProducts = products.Count(p => p.IsFeatured),
                DigitalProducts = products.Count(p => p.IsDigital),
                ProductsByCategory = categories.Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = products.Count(p => p.CategoryId == c.Id)
                }).ToList(),
                ProductsByBrand = brands.Select(b => new
                {
                    BrandName = b.Name,
                    ProductCount = products.Count(p => p.Brand == b.Name)
                }).ToList(),
                AveragePrice = products.Any() ? products.Average(p => p.Price.Amount) : 0,
                PriceRange = new
                {
                    Min = products.Any() ? products.Min(p => p.Price.Amount) : 0,
                    Max = products.Any() ? products.Max(p => p.Price.Amount) : 0
                }
            };

            return Ok(ApiResponse<object>.SuccessResult(analytics));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult($"Product analytics error: {ex.Message}"));
        }
    }

    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<object>>> GetCategoryAnalytics()
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var products = await _productRepository.GetAllAsync();

            var analytics = new
            {
                TotalCategories = categories.Count,
                ActiveCategories = categories.Count(c => c.IsActive),
                CategoriesWithProducts = categories.Count(c => products.Any(p => p.CategoryId == c.Id)),
                TopCategories = categories
                    .Select(c => new
                    {
                        CategoryName = c.Name,
                        ProductCount = products.Count(p => p.CategoryId == c.Id),
                        IsActive = c.IsActive
                    })
                    .OrderByDescending(x => x.ProductCount)
                    .Take(10)
                    .ToList()
            };

            return Ok(ApiResponse<object>.SuccessResult(analytics));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult($"Category analytics error: {ex.Message}"));
        }
    }

    [HttpGet("sales")]
    public async Task<ActionResult<ApiResponse<object>>> GetSalesAnalytics()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var inventory = await _inventoryRepository.GetAllAsync();

            var analytics = new
            {
                TotalProducts = products.Count(),
                ProductsInStock = inventory.Count(i => i.Quantity > 0),
                ProductsOutOfStock = inventory.Count(i => i.Quantity == 0),
                LowStockProducts = inventory.Count(i => i.Quantity > 0 && i.Quantity <= 10),
                TotalStockValue = inventory.Sum(i => 
                {
                    var product = products.FirstOrDefault(p => p.Id == i.ProductId);
                    return product?.Price.Amount * i.Quantity ?? 0;
                }),
                StockByLocation = inventory
                    .GroupBy(i => i.Location)
                    .Select(g => new
                    {
                        Location = g.Key,
                        ProductCount = g.Count(),
                        TotalQuantity = g.Sum(i => i.Quantity)
                    })
                    .ToList()
            };

            return Ok(ApiResponse<object>.SuccessResult(analytics));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult($"Sales analytics error: {ex.Message}"));
        }
    }

    [HttpGet("trending")]
    public async Task<ActionResult<ApiResponse<object>>> GetTrendingAnalytics()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var reviews = await _reviewRepository.GetAllAsync();

            var trendingProducts = products
                .Where(p => p.IsFeatured)
                .Select(p => new
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    Brand = p.Brand,
                    Price = p.Price.Amount,
                    IsFeatured = p.IsFeatured,
                    ReviewCount = reviews.Count(r => r.ProductId == p.Id),
                    AverageRating = reviews.Any(r => r.ProductId == p.Id) 
                        ? reviews.Where(r => r.ProductId == p.Id).Average(r => r.Rating) 
                        : 0
                })
                .OrderByDescending(x => x.AverageRating)
                .ThenByDescending(x => x.ReviewCount)
                .Take(10)
                .ToList();

            var analytics = new
            {
                TrendingProducts = trendingProducts,
                TopRatedProducts = trendingProducts.OrderByDescending(x => x.AverageRating).Take(5).ToList(),
                MostReviewedProducts = trendingProducts.OrderByDescending(x => x.ReviewCount).Take(5).ToList()
            };

            return Ok(ApiResponse<object>.SuccessResult(analytics));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult($"Trending analytics error: {ex.Message}"));
        }
    }

    [HttpGet("performance")]
    public async Task<ActionResult<ApiResponse<object>>> GetPerformanceAnalytics()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var brands = await _brandRepository.GetAllAsync();
            var reviews = await _reviewRepository.GetAllAsync();
            var inventory = await _inventoryRepository.GetAllAsync();

            var performance = new
            {
                SystemHealth = new
                {
                    TotalEntities = products.Count() + categories.Count() + brands.Count + reviews.Count + inventory.Count,
                    ProductsWithInventory = products.Count(p => inventory.Any(i => i.ProductId == p.Id)),
                    ProductsWithReviews = products.Count(p => reviews.Any(r => r.ProductId == p.Id)),
                    CategoriesWithProducts = categories.Count(c => products.Any(p => p.CategoryId == c.Id))
                },
                DataQuality = new
                {
                    ProductsWithCompleteInfo = products.Count(p => 
                        !string.IsNullOrEmpty(p.Name) && 
                        !string.IsNullOrEmpty(p.Description) && 
                        !string.IsNullOrEmpty(p.SKU)),
                    ProductsWithImages = products.Count(p => p.Images.Any()),
                    ProductsWithSpecifications = products.Count(p => p.Specifications.Any())
                },
                BusinessMetrics = new
                {
                    ActiveProductPercentage = products.Any() ? (double)products.Count(p => p.Status == Domain.Enums.ProductStatus.Active) / products.Count() * 100 : 0,
                    FeaturedProductPercentage = products.Any() ? (double)products.Count(p => p.IsFeatured) / products.Count() * 100 : 0,
                    DigitalProductPercentage = products.Any() ? (double)products.Count(p => p.IsDigital) / products.Count() * 100 : 0
                }
            };

            return Ok(ApiResponse<object>.SuccessResult(performance));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<object>.ErrorResult($"Performance analytics error: {ex.Message}"));
        }
    }
}
