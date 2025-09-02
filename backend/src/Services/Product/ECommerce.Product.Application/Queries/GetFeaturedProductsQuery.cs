using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries;

public class GetFeaturedProductsQuery : IRequest<List<ProductDto>>
{
    public int Limit { get; set; } = 10;
}










