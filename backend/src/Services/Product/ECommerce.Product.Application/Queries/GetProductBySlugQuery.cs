using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries;

public class GetProductBySlugQuery : IRequest<ProductDto?>
{
    public string Slug { get; set; } = string.Empty;
}






