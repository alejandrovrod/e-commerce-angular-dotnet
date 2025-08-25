using ECommerce.BuildingBlocks.Common.Interfaces;
using ECommerce.User.Domain.Entities;

namespace ECommerce.User.Application.Interfaces;

public interface IUserRepository : IRepository<Domain.Entities.User>
{
    Task<Domain.Entities.User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Domain.Entities.User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> PhoneExistsAsync(string phoneNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Domain.Entities.User>> GetUsersByRoleAsync(Domain.Enums.UserRole role, CancellationToken cancellationToken = default);
    Task<Domain.Entities.User?> GetUserWithAddressesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Domain.Entities.User?> GetUserWithPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IJwtTokenService
{
    Task<TokenResult> GenerateTokensAsync(Domain.Entities.User user);
    Task<TokenResult> RefreshTokensAsync(string refreshToken);
    Task<bool> ValidateTokenAsync(string token);
    Task RevokeTokenAsync(string refreshToken);
}

public record TokenResult(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string TokenType = "Bearer"
);





