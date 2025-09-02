using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.Commands.UpdateProduct;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Interfaces;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Domain.ValueObjects;

namespace ECommerce.Product.Application.Handlers.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Buscar el producto existente con detalles
            var product = await _productRepository.GetByIdWithDetailsAsync(request.Id);
            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResult($"Product with ID {request.Id} not found");
            }

            // Actualizar la información básica del producto
            product.UpdateBasicInfo(request.Name, request.Description);
            product.UpdateSKU(request.SKU);
            product.UpdateBrand(request.Brand);
            product.UpdateCategory(request.CategoryId);
            var compareAtPrice = request.OriginalPrice.HasValue && request.OriginalPrice.Value > 0 
                ? new Money(request.OriginalPrice.Value, "USD") 
                : null;
            product.UpdatePricing(new Money(request.Price, "USD"), compareAtPrice);
            product.SetFeatured(request.IsFeatured);
            product.SetDigitalAndShipping(request.IsDigital, request.RequiresShipping);
            product.SetTaxable(request.IsTaxable);
            
            // Actualizar el stock
            product.UpdateInventory(request.Stock);

            // Actualizar imágenes
            if (request.Images != null && request.Images.Any())
            {
                var images = request.Images
                    .Where(img => !string.IsNullOrWhiteSpace(img))
                    .Select((img, index) => (img, $"Image {index + 1} for {request.Name}"))
                    .ToArray();
                product.SetImages(images);
            }
            else
            {
                product.ClearImages();
            }

            // Actualizar tags
            if (request.Tags != null && request.Tags.Any())
            {
                product.SetTags(request.Tags.ToArray());
            }
            else
            {
                product.ClearTags();
            }

            // Guardar los cambios
            await _unitOfWork.SaveChangesAsync();

            // Mapear a DTO
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price?.Amount ?? 0,
                OriginalPrice = product.CompareAtPrice?.Amount,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                Status = product.Status,
                IsFeatured = product.IsFeatured,
                IsDigital = product.IsDigital,
                RequiresShipping = product.RequiresShipping,
                IsTaxable = product.IsTaxable,
                Images = product.Images?.Select(img => img.Url).ToList() ?? new List<string>(),
                Tags = product.Tags?.ToList() ?? new List<string>(),
                Stock = product.Inventory?.Quantity ?? 0,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return ApiResponse<ProductDto>.SuccessResult(productDto, "Product updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResult($"Error updating product: {ex.Message}");
        }
    }
}
