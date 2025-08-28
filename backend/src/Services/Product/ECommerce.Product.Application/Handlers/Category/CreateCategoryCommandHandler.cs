using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Category;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Category;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if category name already exists
            if (await _categoryRepository.ExistsByNameAsync(request.Name))
            {
                return ApiResponse<CategoryDto>.ErrorResult("Category name already exists");
            }

            // Create new category
            var category = new ECommerce.Product.Domain.Entities.Category(request.Name, request.Description, request.ParentCategoryId);
            
            // Save to repository
            await _categoryRepository.AddAsync(category);
            
            // Save changes to database using Unit of Work
            await _unitOfWork.SaveChangesAsync(cancellationToken);

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
            return ApiResponse<CategoryDto>.ErrorResult($"Error creating category: {ex.Message}");
        }
    }
}
