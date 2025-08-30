using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerce.File.Application.Commands.CreateFile;
using ECommerce.File.Application.Commands.UpdateFile;
using ECommerce.File.Application.Commands.DeleteFile;
using ECommerce.File.Application.Commands.UpdateFileStatus;
using ECommerce.File.Application.Queries.GetFile;
using ECommerce.File.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.File.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IMediator _mediator;

    public FileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new file record
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateFile([FromBody] CreateFileDto createFileDto)
    {
        var command = new CreateFileCommand
        {
            OriginalFileName = createFileDto.OriginalFileName,
            ContentType = createFileDto.ContentType,
            FileSize = createFileDto.FileSize,
            FileExtension = createFileDto.FileExtension,
            Type = createFileDto.Type,
            UserId = createFileDto.UserId,
            OrderId = createFileDto.OrderId,
            ProductId = createFileDto.ProductId,
            Description = createFileDto.Description,
            IsPublic = createFileDto.IsPublic,
            ExpiresAt = createFileDto.ExpiresAt
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get file by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(Guid id)
    {
        var query = new GetFileByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all files with optional filters
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetFiles(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? orderId,
        [FromQuery] Guid? productId,
        [FromQuery] string? type,
        [FromQuery] string? status,
        [FromQuery] bool? isPublic,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFilesQuery
        {
            UserId = userId,
            OrderId = orderId,
            ProductId = productId,
            Type = !string.IsNullOrEmpty(type) ? Enum.Parse<Domain.Enums.FileType>(type) : null,
            Status = !string.IsNullOrEmpty(status) ? Enum.Parse<Domain.Enums.FileStatus>(status) : null,
            IsPublic = isPublic,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get files by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetFilesByUser(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFilesByUserQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update file information
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFile(
        Guid id,
        [FromBody] UpdateFileDto updateFileDto)
    {
        var command = new UpdateFileCommand
        {
            Id = id,
            Description = updateFileDto.Description,
            IsPublic = updateFileDto.IsPublic,
            ExpiresAt = updateFileDto.ExpiresAt,
            Metadata = updateFileDto.Metadata
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update file status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateFileStatus(
        Guid id,
        [FromBody] UpdateFileStatusDto updateStatusDto)
    {
        var command = new UpdateFileStatusCommand
        {
            Id = id,
            Status = updateStatusDto.Status,
            Reason = updateStatusDto.Reason
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Delete file
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(Guid id)
    {
        var command = new DeleteFileCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get files by type
    /// </summary>
    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetFilesByType(
        string type,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (!Enum.TryParse<Domain.Enums.FileType>(type, out var fileType))
        {
            return BadRequest("Invalid file type");
        }

        var query = new GetFilesQuery
        {
            Type = fileType,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get files by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetFilesByStatus(
        string status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (!Enum.TryParse<Domain.Enums.FileStatus>(status, out var fileStatus))
        {
            return BadRequest("Invalid file status");
        }

        var query = new GetFilesQuery
        {
            Status = fileStatus,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get public files
    /// </summary>
    [HttpGet("public")]
    public async Task<IActionResult> GetPublicFiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetFilesQuery
        {
            IsPublic = true,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
