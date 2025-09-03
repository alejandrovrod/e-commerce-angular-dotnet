using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.Brand;

public class UpdateBrandCommand : IRequest<ApiResponse<BrandDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string? Country { get; set; }
    public int? FoundedYear { get; set; }
    public bool IsActive { get; set; }
}
