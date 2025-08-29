using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands;

public class PublishProductCommand : IRequest<ProductDto?>
{
    public Guid Id { get; set; }
}







