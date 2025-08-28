using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Category;

public class GetCategoryByIdQuery : IRequest<ApiResponse<CategoryDto>>
{
    public Guid Id { get; set; }
}
