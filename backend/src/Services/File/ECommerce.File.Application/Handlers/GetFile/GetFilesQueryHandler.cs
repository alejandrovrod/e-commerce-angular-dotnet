using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Application.Queries.GetFile;
using ECommerce.File.Application.Interfaces;

namespace ECommerce.File.Application.Handlers.GetFile;

public class GetFilesQueryHandler : IRequestHandler<GetFilesQuery, ApiResponse<List<FileDto>>>
{
    private readonly IFileRepository _fileRepository;

    public GetFilesQueryHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ApiResponse<List<FileDto>>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<ECommerce.File.Domain.Entities.File> files;

            // Apply filters based on query parameters
            if (request.UserId.HasValue)
            {
                files = await _fileRepository.GetByUserIdAsync(request.UserId.Value);
            }
            else if (request.OrderId.HasValue)
            {
                files = await _fileRepository.GetByOrderIdAsync(request.OrderId.Value);
            }
            else if (request.ProductId.HasValue)
            {
                files = await _fileRepository.GetByProductIdAsync(request.ProductId.Value);
            }
            else if (request.Type.HasValue)
            {
                files = await _fileRepository.GetByTypeAsync(request.Type.Value);
            }
            else if (request.Status.HasValue)
            {
                files = await _fileRepository.GetByStatusAsync(request.Status.Value);
            }
            else if (request.IsPublic.HasValue && request.IsPublic.Value)
            {
                files = await _fileRepository.GetPublicFilesAsync();
            }
            else
            {
                files = await _fileRepository.GetAllAsync();
            }

            // Apply date filters
            if (request.FromDate.HasValue)
            {
                files = files.Where(f => f.CreatedAt >= request.FromDate.Value).ToList();
            }

            if (request.ToDate.HasValue)
            {
                files = files.Where(f => f.CreatedAt <= request.ToDate.Value).ToList();
            }

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
            return ApiResponse<List<FileDto>>.ErrorResult($"Error retrieving files: {ex.Message}");
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
