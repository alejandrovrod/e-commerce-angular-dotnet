using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Order.Infrastructure.Data.Configurations;

public class PaymentInfoConfiguration : IEntityTypeConfiguration<ECommerce.Order.Domain.ValueObjects.PaymentInfo>
{
    public void Configure(EntityTypeBuilder<ECommerce.Order.Domain.ValueObjects.PaymentInfo> builder)
    {
        // This is configured as an owned entity in OrderConfiguration
        // This file is kept for potential future use or specific configurations
    }
}

