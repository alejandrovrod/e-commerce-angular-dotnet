using MediatR;
using ECommerce.Product.Application.Queries;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Handlers;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedResult<ProductDto>>
{
    public async Task<PaginatedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implementar lógica real con repositorio
        var mockProducts = new List<ProductDto>
        {
            new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Producto Demo 1",
                Description = "Descripción del producto demo 1",
                SKU = "DEMO-001",
                Price = 99.99m,
                Brand = "Demo Brand",
                CategoryId = Guid.NewGuid(),
                CategoryName = "Demo Category",
                Images = new List<ProductImageDto>(),
                Variants = new List<ProductVariantDto>(),
                Specifications = new List<ProductSpecificationDto>(),
                Tags = new List<string>(),
                Inventory = new InventoryDto
                {
                    Stock = 10,
                    LowStockThreshold = 5,
                    TrackQuantity = true,
                    AllowBackorder = false
                },
                Status = Domain.Enums.ProductStatus.Active,
                IsFeatured = false,
                IsDigital = false,
                RequiresShipping = true,
                IsTaxable = true,
                Rating = new ProductRatingDto(),
                Analytics = new ProductAnalyticsDto(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new ProductDto
            {
                Id = Guid.NewGuid(),
                Name = "Producto Demo 2",
                Description = "Descripción del producto demo 2",
                SKU = "DEMO-002",
                Price = 149.99m,
                Brand = "Demo Brand",
                CategoryId = Guid.NewGuid(),
                CategoryName = "Demo Category",
                Images = new List<ProductImageDto>(),
                Variants = new List<ProductVariantDto>(),
                Specifications = new List<ProductSpecificationDto>(),
                Tags = new List<string>(),
                Inventory = new InventoryDto
                {
                    Stock = 5,
                    LowStockThreshold = 3,
                    TrackQuantity = true,
                    AllowBackorder = false
                },
                Status = Domain.Enums.ProductStatus.Active,
                IsFeatured = false,
                IsDigital = false,
                RequiresShipping = true,
                IsTaxable = true,
                Rating = new ProductRatingDto(),
                Analytics = new ProductAnalyticsDto(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        return new PaginatedResult<ProductDto>
        {
            Items = mockProducts,
            TotalCount = mockProducts.Count,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}

