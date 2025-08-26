using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Application.DTOs;
using ECommerce.User.Domain.Enums;

namespace ECommerce.User.Application.Commands.UpdateUserProfile;

public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    Gender? Gender
) : IRequest<ApiResponse<UserDto>>;
