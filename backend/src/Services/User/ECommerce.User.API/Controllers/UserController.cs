using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Application.DTOs;
using ECommerce.User.Application.Queries;
using ECommerce.User.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ECommerce.User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;

    public UserController(IMediator mediator, ILogger<UserController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user profile</returns>
    [HttpGet("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new GetUserProfileQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    /// <param name="request">Profile update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user profile</returns>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateUserProfileCommand(
            userId,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.DateOfBirth,
            request.Gender);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="request">Password change data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("password")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new ChangePasswordCommand(
            userId,
            request.CurrentPassword,
            request.NewPassword);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User data</returns>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="role">Filter by role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? role = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersQuery(page, pageSize, searchTerm, role);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Update user role (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Role update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user data</returns>
    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserRoleCommand(id, request.Role);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Deactivate user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Activate user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ActivateUser(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get user statistics (Admin only)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User statistics</returns>
    [HttpGet("statistics")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<UserStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserStatistics(CancellationToken cancellationToken)
    {
        var query = new GetUserStatisticsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("Invalid user token");
    }
}

// Commands and Queries (to be implemented)
public record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
	Gender? Gender
) : IRequest<ApiResponse<UserDto>>;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<ApiResponse<object>>;

public record GetUserByIdQuery(Guid UserId) : IRequest<ApiResponse<UserDto>>;

public record GetUsersQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string? Role
) : IRequest<ApiResponse<PaginatedResponse<UserDto>>>;

public record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<ApiResponse<UserDto>>;

public record DeactivateUserCommand(Guid UserId) : IRequest<ApiResponse<object>>;

public record ActivateUserCommand(Guid UserId) : IRequest<ApiResponse<object>>;

public record GetUserStatisticsQuery() : IRequest<ApiResponse<UserStatisticsDto>>;

public record UpdateUserRoleRequestDto(string Role);

public record UserStatisticsDto(
    int TotalUsers,
    int ActiveUsers,
    int PendingUsers,
    int SuspendedUsers,
    Dictionary<string, int> UsersByRole,
    int NewUsersThisMonth,
    int NewUsersThisWeek
);
