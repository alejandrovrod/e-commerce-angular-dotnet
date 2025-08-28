using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Category;

public class CreateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; }
}
