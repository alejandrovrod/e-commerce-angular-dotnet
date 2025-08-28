using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Category;

namespace ECommerce.Product.Application.Handlers.Category;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing category
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            if (category == null)
            {
                return ApiResponse<CategoryDto>.ErrorResult("Category not found");
            }

            // Check if new name already exists (excluding current category)
            if (await _categoryRepository.ExistsByNameAsync(request.Name, request.Id))
            {
                return ApiResponse<CategoryDto>.ErrorResult("Category name already exists");
            }

            // Update category
            category.Update(request.Name, request.Description);
            category.SetActive(request.IsActive);
            category.SetParentCategory(request.ParentCategoryId);

            // Save changes
            await _categoryRepository.UpdateAsync(category);
            // Note: SaveChangesAsync is handled by the Unit of Work pattern or called from the controller

            // Map to DTO
            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };

            return ApiResponse<CategoryDto>.SuccessResult(categoryDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.ErrorResult($"Error updating category: {ex.Message}");
        }
    }
}
