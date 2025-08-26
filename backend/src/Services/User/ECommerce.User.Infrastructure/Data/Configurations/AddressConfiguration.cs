using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.User.Domain.Entities;

namespace ECommerce.User.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // Temporarily commented out to fix migration issues
        /*
        builder.ToTable("Addresses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Address1)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Address2)
            .HasMaxLength(200);

        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Company)
            .HasMaxLength(100);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.IsDefault)
            .IsRequired();

        // Relationship with User - configured in UserConfiguration

        // Indexes
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.IsDefault });
        */
    }
}
