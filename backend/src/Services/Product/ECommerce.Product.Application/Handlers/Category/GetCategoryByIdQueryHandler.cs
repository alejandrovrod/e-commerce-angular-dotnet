using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Category;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Category;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(request.Id);
            
            if (category == null)
            {
                return ApiResponse<CategoryDto>.ErrorResult("Category not found");
            }

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
            return ApiResponse<CategoryDto>.ErrorResult($"Error retrieving category: {ex.Message}");
        }
    }
}
