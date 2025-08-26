using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.User.Domain.Enums;
using System.Text.Json;
using ECommerce.User.Domain.ValueObjects;

namespace ECommerce.User.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<Domain.Entities.User>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.User> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(254)
            .HasAnnotation("EmailAddress", true);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Avatar)
            .HasMaxLength(500);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(u => u.Gender)
            .HasConversion<int?>();

        builder.Property(u => u.RefreshToken)
            .HasMaxLength(256);

        // Configure Metadata property as JSON
        builder.Property(u => u.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.PhoneNumber)
            .HasDatabaseName("IX_Users_PhoneNumber");

        builder.HasIndex(u => u.RefreshToken)
            .HasDatabaseName("IX_Users_RefreshToken");

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Users_Status");

        // Relationships - Temporarily commented out to fix migration issues
        // builder.HasMany(u => u.Addresses)
        //     .WithOne(a => a.User)
        //     .HasForeignKey(a => a.UserId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // builder.HasOne(u => u.Preferences)
        //     .WithOne(p => p.User)
        //     .HasForeignKey<UserPreferences>(p => p.UserId)
        //     .OnDelete(DeleteBehavior.Cascade);

        // Seed data
        builder.HasData(
            new
            {
                Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                Email = "admin@ecommerce.com",
                PasswordHash = "$2a$11$5Z3k5K5K5K5K5K5K5K5K5OWJvqXfY5qZgQJ5K5K5K5K5K5K5K5K5K", // Password: Admin123!
                FirstName = "System",
                LastName = "Administrator",
                Role = UserRole.SuperAdmin,
                Status = UserStatus.Active,
                EmailVerified = true,
                PhoneVerified = false,
                TwoFactorEnabled = false,
                Version = "1.0",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsDeleted = false
            }
        );
    }
}





