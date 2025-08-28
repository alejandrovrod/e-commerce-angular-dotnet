using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;
using ECommerce.Product.Application.Events;
using ECommerce.Product.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Mapster;

namespace ECommerce.Product.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICacheService _cacheService;
    private readonly IEventService _eventService;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository,
        IInventoryRepository inventoryRepository,
        ICacheService cacheService,
        IEventService eventService,
        ILogger<CreateProductCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _inventoryRepository = inventoryRepository;
        _cacheService = cacheService;
        _eventService = eventService;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating product: {ProductName}", request.Name);

            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
            {
                _logger.LogWarning("Category not found: {CategoryId}", request.CategoryId);
                return ApiResponse<ProductDto>.ErrorResult("Category not found");
            }

            // Check if SKU already exists
            if (await _productRepository.ExistsBySkuAsync(request.SKU))
            {
                _logger.LogWarning("Product with SKU already exists: {SKU}", request.SKU);
                return ApiResponse<ProductDto>.ErrorResult("Product with this SKU already exists");
            }

            // Check if slug already exists
            var slug = GenerateSlug(request.Name);
            if (await _productRepository.ExistsBySlugAsync(slug))
            {
                _logger.LogWarning("Product with slug already exists: {Slug}", slug);
                return ApiResponse<ProductDto>.ErrorResult("Product with this slug already exists");
            }

            // Create inventory for the product first
            var inventory = new Domain.Entities.Inventory(
                productId: Guid.Empty, // Will be set after product creation
                quantity: 0,
                location: "Main Warehouse"
            );

            // Create product entity with inventory
            var product = Domain.Entities.Product.Create(
                request.Name,
                request.Description,
                request.SKU,
                new Money(request.Price, "USD"), // Assuming USD currency
                request.Brand,
                request.CategoryId,
                inventory
            );

            // Set additional properties
            product.SetStatus(request.IsFeatured ? Domain.Enums.ProductStatus.Active : Domain.Enums.ProductStatus.Draft);
            product.SetFeatured(request.IsFeatured);
            product.SetDigitalAndShipping(request.IsDigital, request.RequiresShipping);
            product.SetTaxable(request.IsTaxable);

            // Save product (this will also save the inventory due to the relationship)
            var createdProduct = await _productRepository.AddAsync(product);
            
            // Save changes to database using Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Clear cache
            await _cacheService.RemoveByPatternAsync("products:*");
            await _cacheService.RemoveByPatternAsync("categories:*");

            // Publish event
            var productCreatedEvent = new ProductCreatedEvent(
                createdProduct.Id,
                createdProduct.Name,
                createdProduct.SKU,
                createdProduct.Brand,
                createdProduct.CategoryId,
                request.Price,
                createdProduct.IsDigital,
                createdProduct.RequiresShipping,
                createdProduct.CreatedAt
            );

            await _eventService.PublishAsync(productCreatedEvent);

            // Map to DTO manually to avoid Mapster issues with immutable types
            var productDto = new ProductDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Description = createdProduct.Description,
                SKU = createdProduct.SKU,
                Price = createdProduct.Price.Amount,
                Brand = createdProduct.Brand,
                CategoryId = createdProduct.CategoryId,
                Status = createdProduct.Status,
                IsDigital = createdProduct.IsDigital,
                RequiresShipping = createdProduct.RequiresShipping,
                IsTaxable = createdProduct.IsTaxable,
                IsFeatured = createdProduct.IsFeatured,
                CreatedAt = createdProduct.CreatedAt,
                UpdatedAt = createdProduct.UpdatedAt
            };

            _logger.LogInformation("Product created successfully: {ProductId}", createdProduct.Id);

            return ApiResponse<ProductDto>.SuccessResult(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
            return ApiResponse<ProductDto>.ErrorResult("An error occurred while creating the product");
        }
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("|", "-")
            .Replace(":", "-")
            .Replace(";", "-")
            .Replace(",", "-")
            .Replace(".", "-")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("@", "")
            .Replace("#", "")
            .Replace("$", "")
            .Replace("%", "")
            .Replace("^", "")
            .Replace("*", "")
            .Replace("+", "")
            .Replace("=", "")
            .Replace("_", "-")
            .Replace("~", "")
            .Replace("`", "");
    }
}
