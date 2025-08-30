using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Application.Queries.GetFile;
using ECommerce.File.Application.Interfaces;

namespace ECommerce.File.Application.Handlers.GetFile;

public class GetFilesByUserQueryHandler : IRequestHandler<GetFilesByUserQuery, ApiResponse<List<FileDto>>>
{
    private readonly IFileRepository _fileRepository;

    public GetFilesByUserQueryHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ApiResponse<List<FileDto>>> Handle(GetFilesByUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var files = await _fileRepository.GetByUserIdAsync(request.UserId);

            // Apply pagination
            var totalCount = files.Count;
            var pagedFiles = files
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var fileDtos = pagedFiles.Select(MapToDto).ToList();

            return ApiResponse<List<FileDto>>.SuccessResult(fileDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<FileDto>>.ErrorResult($"Error retrieving files by user: {ex.Message}");
        }
    }

    private FileDto MapToDto(ECommerce.File.Domain.Entities.File file)
    {
        return new FileDto
        {
            Id = file.Id,
            FileName = file.FileName,
            OriginalFileName = file.OriginalFileName,
            FilePath = file.FilePath,
            ContentType = file.ContentType,
            FileSize = file.FileSize,
            FileExtension = file.FileExtension,
            Type = file.Type,
            Status = file.Status,
            UserId = file.UserId,
            OrderId = file.OrderId,
            ProductId = file.ProductId,
            Description = file.Description,
            Metadata = file.Metadata,
            ThumbnailPath = file.ThumbnailPath,
            IsPublic = file.IsPublic,
            ExpiresAt = file.ExpiresAt,
            DownloadCount = file.DownloadCount,
            Checksum = file.Checksum,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
            IsExpired = file.IsExpired,
            CanBeDownloaded = file.CanBeDownloaded
        };
    }
}
