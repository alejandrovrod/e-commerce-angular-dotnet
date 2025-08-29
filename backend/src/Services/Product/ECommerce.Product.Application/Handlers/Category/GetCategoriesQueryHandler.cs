using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Category;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Category;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryRepository.GetAllAsync();
            
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
