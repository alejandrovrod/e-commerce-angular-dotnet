using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Queries.Category;

namespace ECommerce.Product.Application.Handlers.Category;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<ECommerce.Product.Domain.Entities.Category> categories;

            if (request.ParentCategoryId.HasValue)
            {
                categories = await _categoryRepository.GetByParentIdAsync(request.ParentCategoryId.Value);
            }
            else if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                categories = await _categoryRepository.SearchAsync(request.SearchTerm);
            }
            else if (request.IsActive.HasValue)
            {
                categories = request.IsActive.Value 
                    ? await _categoryRepository.GetActiveAsync()
                    : await _categoryRepository.GetAllAsync();
            }
            else
            {
                categories = await _categoryRepository.GetAllAsync();
            }

            // Filter by active status if specified
            if (request.IsActive.HasValue)
            {
                categories = categories.Where(c => c.IsActive == request.IsActive.Value).ToList();
            }

            // Map to DTOs
            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                ParentCategoryId = c.ParentCategoryId,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            return ApiResponse<List<CategoryDto>>.SuccessResult(categoryDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CategoryDto>>.ErrorResult($"Error retrieving categories: {ex.Message}");
        }
    }
}
