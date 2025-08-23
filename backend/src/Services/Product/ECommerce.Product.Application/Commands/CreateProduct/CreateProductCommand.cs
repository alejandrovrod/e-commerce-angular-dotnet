using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string? ShortDescription,
    string SKU,
    MoneyDto Price,
    MoneyDto? CompareAtPrice,
    string Brand,
    Guid CategoryId,
    List<ProductImageDto>? Images,
    List<ProductVariantDto>? Variants,
    List<ProductSpecificationDto>? Specifications,
    List<string>? Tags,
    WeightDto? Weight,
    DimensionsDto? Dimensions,
    InventoryDto Inventory,
    bool IsFeatured = false,
    bool IsDigital = false,
    bool RequiresShipping = true,
    bool IsTaxable = true,
    SEODto? SEO = null
) : IRequest<ApiResponse<ProductDto>>;



