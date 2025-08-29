using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Category;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Category;

public class GetSubcategoriesQueryHandler : IRequestHandler<GetSubcategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetSubcategoriesQueryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetSubcategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            var subcategories = categories.Where(c => c.ParentCategoryId == request.ParentCategoryId).ToList();

            var categoryDtos = subcategories.Select(c => new CategoryDto
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
            return ApiResponse<List<CategoryDto>>.ErrorResult($"Error retrieving subcategories: {ex.Message}");
        }
    }
}
