using MediatR;
using ECommerce.Product.Application.Queries;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implementar lógica real con repositorio
        if (request.Id == Guid.Empty)
            return null;

        return new ProductDto
        {
            Id = request.Id,
            Name = "Producto Demo",
            Description = "Descripción del producto demo",
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
        };
    }
}

