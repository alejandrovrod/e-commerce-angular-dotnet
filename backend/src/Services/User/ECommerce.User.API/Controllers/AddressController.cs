using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using ECommerce.User.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.User.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ECommerce.User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AddressController> _logger;

    public AddressController(IMediator mediator, ILogger<AddressController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all addresses for current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of user addresses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<AddressDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAddresses(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new GetUserAddressesQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get address by ID
    /// </summary>
    /// <param name="id">Address ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Address data</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAddress(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new GetAddressByIdQuery(userId, id);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Create new address
    /// </summary>
    /// <param name="request">Address data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created address</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAddress([FromBody] CreateAddressRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new CreateAddressCommand(
            userId,
            request.Type.ToString(),
            request.FirstName,
            request.LastName,
            request.Company,
            request.Address1,
            request.Address2,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.Phone,
            request.IsDefault);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return CreatedAtAction(nameof(GetAddress), new { id = result.Data?.Id }, result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Update address
    /// </summary>
    /// <param name="id">Address ID</param>
    /// <param name="request">Updated address data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated address</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAddress(Guid id, [FromBody] UpdateAddressRequestDto request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new UpdateAddressCommand(
            userId,
            id,
            request.FirstName,
            request.LastName,
            request.Company,
            request.Address1,
            request.Address2,
            request.City,
            request.State,
            request.ZipCode,
            request.Country,
            request.Phone,
            request.IsDefault);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    /// <summary>
    /// Delete address
    /// </summary>
    /// <param name="id">Address ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new DeleteAddressCommand(userId, id);
        var result = await _mediator.Send(command, cancellationToken);

        if (result.Success)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    /// <summary>
    /// Set address as default
    /// </summary>
    /// <param name="id">Address ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response</returns>
    [HttpPut("{id}/default")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetDefaultAddress(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new SetDefaultAddressCommand(userId, id);
        var result = await _mediator.Send(command, cancellationToken);

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
public record GetUserAddressesQuery(Guid UserId) : IRequest<ApiResponse<List<AddressDto>>>;

public record GetAddressByIdQuery(Guid UserId, Guid AddressId) : IRequest<ApiResponse<AddressDto>>;

public record CreateAddressCommand(
    Guid UserId,
    string Type,
    string FirstName,
    string LastName,
    string? Company,
    string Address1,
    string? Address2,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? Phone,
    bool IsDefault
) : IRequest<ApiResponse<AddressDto>>;

public record UpdateAddressCommand(
    Guid UserId,
    Guid AddressId,
    string FirstName,
    string LastName,
    string? Company,
    string Address1,
    string? Address2,
    string City,
    string State,
    string ZipCode,
    string Country,
    string? Phone,
    bool IsDefault
) : IRequest<ApiResponse<AddressDto>>;

public record DeleteAddressCommand(Guid UserId, Guid AddressId) : IRequest<ApiResponse<object>>;

public record SetDefaultAddressCommand(Guid UserId, Guid AddressId) : IRequest<ApiResponse<object>>;
