using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ECommerce.User.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;
using System;
using System.Collections.Generic;

namespace ECommerce.User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IMediator mediator, ILogger<AdminController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all users with pagination and filtering
    /// </summary>
    /// <param name="page">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="searchTerm">Search term</param>
    /// <param name="role">Filter by role</param>
    /// <param name="status">Filter by status</param>
    /// <param name="sortBy">Sort by field</param>
    /// <param name="sortOrder">Sort order (asc/desc)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet("users")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? role = null,
        [FromQuery] string? status = null,
        [FromQuery] string? sortBy = "CreatedAt",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersAdminQuery(page, pageSize, searchTerm, role, status, sortBy, sortOrder);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get user details by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User details</returns>
    [HttpGet("users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserDetails(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserDetailsAdminQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Create new user (Admin only)
    /// </summary>
    /// <param name="request">User creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user</returns>
    [HttpPost("users")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserAdminRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateUserAdminCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Role,
            request.Status);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetUserDetails), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">User update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated user</returns>
    [HttpPut("users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserAdminRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserAdminCommand(
            id,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.Role,
            request.Status);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserAdminCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Bulk update user roles
    /// </summary>
    /// <param name="request">Bulk update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("users/bulk-role")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> BulkUpdateUserRoles([FromBody] BulkUpdateUserRolesRequestDto request, CancellationToken cancellationToken)
    {
        var command = new BulkUpdateUserRolesCommand(request.UserIds, request.Role);
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get user statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User statistics</returns>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<UserStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserStatistics(CancellationToken cancellationToken)
    {
        var query = new GetUserStatisticsAdminQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Export users to CSV
    /// </summary>
    /// <param name="role">Filter by role</param>
    /// <param name="status">Filter by status</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CSV file</returns>
    [HttpGet("users/export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportUsers(
        [FromQuery] string? role = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportUsersQuery(role, status);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success && result.Data != null)
        {
            var fileName = $"users_export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(result.Data, "text/csv", fileName);
        }

        return BadRequest(result);
    }
}

// Commands and Queries (to be implemented)
public record GetUsersAdminQuery(
    int Page,
    int PageSize,
    string? SearchTerm,
    string? Role,
    string? Status,
    string SortBy,
    string SortOrder
) : IRequest<ApiResponse<PaginatedResponse<UserDto>>>;

public record GetUserDetailsAdminQuery(Guid UserId) : IRequest<ApiResponse<UserDto>>;

public record CreateUserAdminCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    string Status
) : IRequest<ApiResponse<UserDto>>;

public record UpdateUserAdminCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    string Status
) : IRequest<ApiResponse<UserDto>>;

public record DeleteUserAdminCommand(Guid UserId) : IRequest<ApiResponse<object>>;

public record BulkUpdateUserRolesCommand(
    List<Guid> UserIds,
    string Role
) : IRequest<ApiResponse<object>>;

public record GetUserStatisticsAdminQuery() : IRequest<ApiResponse<UserStatisticsDto>>;

public record ExportUsersQuery(string? Role, string? Status) : IRequest<ApiResponse<byte[]>>;

public record CreateUserAdminRequestDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    string Status
);

public record UpdateUserAdminRequestDto(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    string Status
);

public record BulkUpdateUserRolesRequestDto(
    List<Guid> UserIds,
    string Role
);
