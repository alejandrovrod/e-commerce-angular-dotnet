using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Domain.ValueObjects;
using ECommerce.User.Domain.Enums;

namespace ECommerce.User.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string? PhoneNumber { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public string? Avatar { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Customer;
    public UserStatus Status { get; private set; } = UserStatus.PendingVerification;
    public bool EmailVerified { get; private set; } = false;
    public bool PhoneVerified { get; private set; } = false;
    public bool TwoFactorEnabled { get; private set; } = false;
    public DateTime? LastLoginAt { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    // Navigation properties
    public virtual ICollection<Address> Addresses { get; private set; } = new List<Address>();
    public virtual UserPreferences? Preferences { get; private set; }

    private User() { } // For EF Core

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        string? phoneNumber = null)
    {
        var user = new User
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Role = UserRole.Customer,
            Status = UserStatus.PendingVerification
        };

        return user;
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber = null, DateTime? dateOfBirth = null, Gender? gender = null)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyEmail()
    {
        EmailVerified = true;
        if (Status == UserStatus.PendingVerification)
        {
            Status = UserStatus.Active;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void VerifyPhone()
    {
        PhoneVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole role)
    {
        Role = role;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAvatar(string avatarUrl)
    {
        Avatar = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableTwoFactor()
    {
        TwoFactorEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DisableTwoFactor()
    {
        TwoFactorEnabled = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAddress(Address address)
    {
        Addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = Addresses.FirstOrDefault(a => a.Id == addressId);
        if (address != null)
        {
            Addresses.Remove(address);
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public string GetFullName() => $"{FirstName} {LastName}";
    
    public bool IsActive() => Status == UserStatus.Active;
    
    public bool CanLogin() => Status == UserStatus.Active && EmailVerified;
}








