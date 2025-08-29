using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Brand;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Brand;

public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, ApiResponse<bool>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandCommandHandler(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing brand
            var brand = await _brandRepository.GetByIdAsync(request.Id);
            if (brand == null)
            {
                return ApiResponse<bool>.ErrorResult("Brand not found");
            }

            // Check if brand has products
            if (brand.Products.Any())
            {
                return ApiResponse<bool>.ErrorResult("Cannot delete brand with products");
            }

            // Delete brand
            await _brandRepository.DeleteAsync(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting brand: {ex.Message}");
        }
    }
}
