using MediatR;
using Mapster;
using Microsoft.Extensions.Logging;
using ECommerce.User.Application.DTOs;
using ECommerce.User.Application.Interfaces;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.BuildingBlocks.Common.Exceptions;
using ECommerce.BuildingBlocks.EventBus.Events;
using MassTransit;

namespace ECommerce.User.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, ApiResponse<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IPublishEndpoint publishEndpoint,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing user registration for email: {Email}", request.Email);

            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null)
            {
                throw new DuplicateException("User", "email", request.Email);
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create user
            var user = Domain.Entities.User.Create(
                request.Email,
                passwordHash,
                request.FirstName,
                request.LastName,
                request.PhoneNumber);

            // Save user
            await _userRepository.AddAsync(user, cancellationToken);

            // Generate JWT tokens
            var tokens = await _jwtTokenService.GenerateTokensAsync(user);

            // Update user with refresh token
            user.SetRefreshToken(tokens.RefreshToken, DateTime.UtcNow.AddDays(7));
            await _userRepository.UpdateAsync(user, cancellationToken);

            // Publish integration event
            await _publishEndpoint.Publish(new UserRegisteredEvent(
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.CreatedAt
            ), cancellationToken);

            _logger.LogInformation("User {UserId} registered successfully", user.Id);

            // Map to DTOs
            var userDto = user.Adapt<UserDto>();
            var tokensDto = tokens.Adapt<AuthTokensDto>();

            var response = new LoginResponseDto(userDto, tokensDto);

            return ApiResponse<LoginResponseDto>.SuccessResult(response, "User registered successfully");
        }
        catch (Exception ex) when (ex is not BusinessException)
        {
            _logger.LogError(ex, "Error registering user with email: {Email}", request.Email);
            throw new BusinessException("An error occurred during registration", ex);
        }
    }
}


