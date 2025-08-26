using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Reflection;

namespace ECommerce.User.Application;

public static class ApplicationServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Add AutoMapper/Mapster
        // services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
