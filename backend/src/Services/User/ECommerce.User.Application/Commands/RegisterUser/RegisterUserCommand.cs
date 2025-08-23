using MediatR;
using ECommerce.User.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.User.Application.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber = null
) : IRequest<ApiResponse<LoginResponseDto>>;


