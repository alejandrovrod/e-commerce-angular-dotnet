using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Category;

public class UpdateCategoryCommand : IRequest<ApiResponse<CategoryDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
}
