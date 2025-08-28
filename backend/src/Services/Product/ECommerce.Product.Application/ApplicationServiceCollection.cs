using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Mapster;
using MapsterMapper;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application;

public static class ApplicationServiceCollection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Configure Mapster
        var config = TypeAdapterConfig.GlobalSettings;
        
        // Configure mapping for Product entity to ProductDto
        config.NewConfig<Domain.Entities.Product, ProductDto>()
            .Map(dest => dest.Price, src => src.Price.Amount)
            .Map(dest => dest.Brand, src => src.Brand)
            .Map(dest => dest.CategoryId, src => src.CategoryId)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.IsFeatured, src => src.IsFeatured)
            .Map(dest => dest.IsDigital, src => src.IsDigital)
            .Map(dest => dest.RequiresShipping, src => src.RequiresShipping)
            .Map(dest => dest.IsTaxable, src => src.IsTaxable)
            .Map(dest => dest.CreatedAt, src => src.CreatedAt)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAt);

        // Register Mapster
        services.AddSingleton(config);
        services.AddScoped<IMapper, Mapper>();

        return services;
    }
}

