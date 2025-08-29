using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands;
using ECommerce.Product.Application.Interfaces;
using ECommerce.Product.Domain.ValueObjects;

namespace ECommerce.Product.Application.Handlers.Product;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResult("Product not found");
            }

            // Validate category exists
            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                {
                    return ApiResponse<ProductDto>.ErrorResult("Category not found");
                }
            }

            // Update product using available methods
            product.UpdateBasicInfo(request.Name, request.Description, request.ShortDescription);
            product.UpdatePricing(new Money(request.Price), request.CompareAtPrice.HasValue ? new Money(request.CompareAtPrice.Value) : null, request.CostPrice.HasValue ? new Money(request.CostPrice.Value) : null);
            product.UpdateSKU(request.SKU);
            product.UpdateBrand(request.Brand);
            if (request.CategoryId.HasValue)
            {
                product.UpdateCategory(request.CategoryId.Value);
            }
            product.SetFeatured(request.IsFeatured);
            product.SetDigitalAndShipping(request.IsDigital, request.RequiresShipping);
            product.SetTaxable(request.IsTaxable);
            product.SetStatus(request.Status);

            // Save changes
            await _productRepository.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Map to DTO
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                Price = product.Price.Amount,
                Brand = product.Brand,
                CategoryId = product.CategoryId,
                Status = product.Status,
                IsFeatured = product.IsFeatured,
                IsDigital = product.IsDigital,
                RequiresShipping = product.RequiresShipping,
                IsTaxable = product.IsTaxable,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return ApiResponse<ProductDto>.SuccessResult(productDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResult($"Error updating product: {ex.Message}");
        }
    }
}
