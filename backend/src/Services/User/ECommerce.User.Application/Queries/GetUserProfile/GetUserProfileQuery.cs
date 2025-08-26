using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Application.DTOs;

namespace ECommerce.User.Application.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid UserId) : IRequest<ApiResponse<UserDto>>;
