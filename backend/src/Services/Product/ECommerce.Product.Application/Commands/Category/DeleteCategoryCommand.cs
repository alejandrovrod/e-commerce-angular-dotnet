using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands.Category;

public class DeleteCategoryCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
