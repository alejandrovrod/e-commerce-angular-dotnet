using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Category;

public class GetSubcategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>
{
    public Guid ParentCategoryId { get; set; }
    public bool? IsActive { get; set; }
}
