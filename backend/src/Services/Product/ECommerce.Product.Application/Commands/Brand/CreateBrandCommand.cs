using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Brand;

public class CreateBrandCommand : IRequest<ApiResponse<BrandDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
