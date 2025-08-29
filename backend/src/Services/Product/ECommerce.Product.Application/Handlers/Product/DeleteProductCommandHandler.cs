using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Product;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse<bool>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                return ApiResponse<bool>.ErrorResult("Product not found");
            }

            // Check if product has inventory
            if (product.Inventory != null && product.Inventory.Quantity > 0)
            {
                return ApiResponse<bool>.ErrorResult("Cannot delete product with inventory");
            }

            // Check if product has reviews
            if (product.Reviews.Any())
            {
                return ApiResponse<bool>.ErrorResult("Cannot delete product with reviews");
            }

            // Delete product
            await _productRepository.DeleteAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting product: {ex.Message}");
        }
    }
}
