using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Order.Infrastructure.Data.Configurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<ECommerce.Order.Domain.ValueObjects.OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<ECommerce.Order.Domain.ValueObjects.OrderStatusHistory> builder)
    {
        // This is configured as an owned entity in OrderConfiguration
        // This file is kept for potential future use or specific configurations
    }
}



