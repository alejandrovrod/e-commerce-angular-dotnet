using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Category;

public class GetCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>
{
    public bool? IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? SearchTerm { get; set; }
}
