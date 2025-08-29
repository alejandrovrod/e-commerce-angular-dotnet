using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Category;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Category;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing category
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                return ApiResponse<bool>.ErrorResult("Category not found");
            }

            // Check if category has subcategories
            var subcategories = await _categoryRepository.GetSubcategoriesAsync(request.Id);
            if (subcategories.Any())
            {
                return ApiResponse<bool>.ErrorResult("Cannot delete category with subcategories");
            }

            // Check if category has products
            if (category.Products.Any())
            {
                return ApiResponse<bool>.ErrorResult("Cannot delete category with products");
            }

            // Delete category
            await _categoryRepository.DeleteAsync(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting category: {ex.Message}");
        }
    }
}
