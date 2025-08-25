using ECommerce.User.Domain.Enums;

namespace ECommerce.User.Application.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender,
    string? Avatar,
    UserRole Role,
    UserStatus Status,
    bool EmailVerified,
    bool PhoneVerified,
    bool TwoFactorEnabled,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    List<AddressDto> Addresses,
    UserPreferencesDto? Preferences
);

public record AddressDto(
    Guid Id,
    AddressType Type,
    string FirstName,
    string LastName,
    string? Company,
    string Address1,
    string? Address2,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? Phone,
    bool IsDefault
);

public record UserPreferencesDto(
    string Theme,
    string Language,
    string Currency,
    EmailNotificationDto EmailNotifications,
    PushNotificationDto PushNotifications
);

public record EmailNotificationDto(
    bool OrderUpdates,
    bool Promotions,
    bool Newsletter,
    bool ProductRecommendations
);

public record PushNotificationDto(
    bool OrderUpdates,
    bool Promotions,
    bool Recommendations
);

public record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string TokenType
);

public record LoginResponseDto(
    UserDto User,
    AuthTokensDto Tokens
);

public record RegisterRequestDto(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    bool AcceptTerms
);

public record LoginRequestDto(
    string Email,
    string Password,
    bool RememberMe
);

public record UpdateProfileRequestDto(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender
);

public record ChangePasswordRequestDto(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);

public record CreateAddressRequestDto(
    AddressType Type,
    string FirstName,
    string LastName,
    string? Company,
    string Address1,
    string? Address2,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? Phone,
    bool IsDefault
);

public record UpdateAddressRequestDto(
    string FirstName,
    string LastName,
    string? Company,
    string Address1,
    string? Address2,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? Phone,
    bool IsDefault
);

public record ForgotPasswordRequestDto(
    string Email
);

public record ResetPasswordRequestDto(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword
);

public record RefreshTokenRequestDto(
    string RefreshToken
);





