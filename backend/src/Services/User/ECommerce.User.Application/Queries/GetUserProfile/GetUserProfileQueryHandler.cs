using MediatR;
using Microsoft.Extensions.Logging;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Application.DTOs;
using ECommerce.User.Application.Interfaces;
using Mapster;
using System;
using System.Collections.Generic;

namespace ECommerce.User.Application.Queries.GetUserProfile;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, ApiResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileQueryHandler> _logger;

    public GetUserProfileQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserProfileQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<UserDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting user profile for user ID: {UserId}", request.UserId);

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                    if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
            return ApiResponse<UserDto>.ErrorResult("User not found");
        }

                    var userDto = user.Adapt<UserDto>();

        _logger.LogInformation("User profile retrieved successfully for user ID: {UserId}", request.UserId);

        return ApiResponse<UserDto>.SuccessResult(userDto, "User profile retrieved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile for user ID: {UserId}", request.UserId);
            return ApiResponse<UserDto>.ErrorResult("Error retrieving user profile");
        }
    }
}
