using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Brand;

public class GetBrandsQuery : IRequest<ApiResponse<List<BrandDto>>>
{
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
}
