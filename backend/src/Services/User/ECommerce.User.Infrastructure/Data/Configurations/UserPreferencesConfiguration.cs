using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.User.Domain.ValueObjects;
using System.Text.Json;

namespace ECommerce.User.Infrastructure.Data.Configurations;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        builder.ToTable("UserPreferences");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Theme)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(e => e.Currency)
            .IsRequired()
            .HasMaxLength(10);

        // Store EmailNotifications as JSON
        builder.Property(e => e.EmailNotifications)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<EmailNotificationSettings>(v, (JsonSerializerOptions?)null) ?? new EmailNotificationSettings()
            )
            .HasColumnType("nvarchar(max)");

        // Store PushNotifications as JSON
        builder.Property(e => e.PushNotifications)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<PushNotificationSettings>(v, (JsonSerializerOptions?)null) ?? new PushNotificationSettings()
            )
            .HasColumnType("nvarchar(max)");

        // Relationship with User
        builder.HasOne<Domain.Entities.User>()
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreferences>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.UserId).IsUnique();
    }
}
