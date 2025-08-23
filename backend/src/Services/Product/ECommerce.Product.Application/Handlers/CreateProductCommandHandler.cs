using MediatR;
using ECommerce.Product.Application.Commands.CreateProduct;
using ECommerce.Product.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<ProductDto>>
{
    public async Task<ApiResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implementar lógica real con repositorio
        var product = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            SKU = request.SKU,
            Price = request.Price.Amount,
            CompareAtPrice = request.CompareAtPrice?.Amount,
            CostPrice = null, // No está en el comando
            Brand = request.Brand,
            CategoryId = request.CategoryId,
            CategoryName = "Categoría Demo", // Valor por defecto
            Images = request.Images ?? new List<ProductImageDto>(),
            Variants = request.Variants ?? new List<ProductVariantDto>(),
            Specifications = request.Specifications ?? new List<ProductSpecificationDto>(),
            Tags = request.Tags ?? new List<string>(),
            Weight = request.Weight,
            Dimensions = request.Dimensions,
            Inventory = request.Inventory,
            Status = Domain.Enums.ProductStatus.Active,
            IsFeatured = request.IsFeatured,
            IsDigital = request.IsDigital,
            RequiresShipping = request.RequiresShipping,
            IsTaxable = request.IsTaxable,
            SEO = request.SEO,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return ApiResponse<ProductDto>.SuccessResult(product, "Product created successfully");
    }
}

