using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public Guid Id { get; set; }
}







