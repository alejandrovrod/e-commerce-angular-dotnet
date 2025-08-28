using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Category;

namespace ECommerce.Product.Application.Handlers.Category;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, ApiResponse<bool>>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
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
            // Note: SaveChangesAsync is handled by the Unit of Work pattern or called from the controller

            return ApiResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResult($"Error deleting category: {ex.Message}");
        }
    }
}
