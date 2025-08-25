using MediatR;

namespace ECommerce.Product.Application.Commands;

public class DeleteProductCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}




