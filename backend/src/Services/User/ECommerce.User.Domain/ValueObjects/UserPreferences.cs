using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.User.Domain.ValueObjects;

public class UserPreferences : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Theme { get; private set; } = "system"; // light, dark, system
    public string Language { get; private set; } = "en";
    public string Currency { get; private set; } = "USD";
    // Temporarily commented out to fix migration issues
    // public EmailNotificationSettings EmailNotifications { get; private set; } = new();
    // public PushNotificationSettings PushNotifications { get; private set; } = new();

    // Navigation properties
    public virtual Entities.User User { get; private set; } = default!;

    private UserPreferences() { } // For EF Core

    public static UserPreferences CreateDefault(Guid userId)
    {
        return new UserPreferences
        {
            UserId = userId,
            Theme = "system",
            Language = "en",
            Currency = "USD"
            // Temporarily commented out to fix migration issues
            // EmailNotifications = new EmailNotificationSettings(),
            // PushNotifications = new PushNotificationSettings()
        };
    }

    public void UpdateTheme(string theme)
    {
        Theme = theme;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLanguage(string language)
    {
        Language = language;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCurrency(string currency)
    {
        Currency = currency;
        UpdatedAt = DateTime.UtcNow;
    }

    // Temporarily commented out to fix migration issues
    /*
    public void UpdateEmailNotifications(EmailNotificationSettings settings)
    {
        EmailNotifications = settings;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePushNotifications(PushNotificationSettings settings)
    {
        PushNotifications = settings;
        UpdatedAt = DateTime.UtcNow;
    }
    */
}

public class EmailNotificationSettings
{
    public bool OrderUpdates { get; set; } = true;
    public bool Promotions { get; set; } = true;
    public bool Newsletter { get; set; } = false;
    public bool ProductRecommendations { get; set; } = false;
}

public class PushNotificationSettings
{
    public bool OrderUpdates { get; set; } = true;
    public bool Promotions { get; set; } = false;
    public bool Recommendations { get; set; } = false;
}





