using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Brand;

public class GetBrandByIdQuery : IRequest<ApiResponse<BrandDto>>
{
    public Guid Id { get; set; }
}
