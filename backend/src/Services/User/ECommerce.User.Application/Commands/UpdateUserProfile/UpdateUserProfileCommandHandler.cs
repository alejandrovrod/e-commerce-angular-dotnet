using MediatR;
using Microsoft.Extensions.Logging;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Application.DTOs;
using ECommerce.User.Application.Interfaces;
using ECommerce.User.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;

namespace ECommerce.User.Application.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, ApiResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<UserDto>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating user profile for user ID: {UserId}", request.UserId);

            // Get existing user
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                    if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
            return ApiResponse<UserDto>.ErrorResult("User not found");
        }

            // Update user properties
            user.UpdateProfile(
                request.FirstName,
                request.LastName,
                request.PhoneNumber,
                request.DateOfBirth,
                request.Gender);

            // Save changes
            await _userRepository.UpdateAsync(user, cancellationToken);

                    // Map to DTO
        var userDto = user.Adapt<UserDto>();

        _logger.LogInformation("User profile updated successfully for user ID: {UserId}", request.UserId);

        return ApiResponse<UserDto>.SuccessResult(userDto, "User profile updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for user ID: {UserId}", request.UserId);
            return ApiResponse<UserDto>.ErrorResult("Error updating user profile");
        }
    }
}
