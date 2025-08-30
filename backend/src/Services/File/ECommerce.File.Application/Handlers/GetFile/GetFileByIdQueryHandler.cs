using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.File.Application.DTOs;
using ECommerce.File.Application.Queries.GetFile;
using ECommerce.File.Application.Interfaces;

namespace ECommerce.File.Application.Handlers.GetFile;

public class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, ApiResponse<FileDto>>
{
    private readonly IFileRepository _fileRepository;

    public GetFileByIdQueryHandler(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public async Task<ApiResponse<FileDto>> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _fileRepository.GetByIdAsync(request.Id);
            if (file == null)
            {
                return ApiResponse<FileDto>.ErrorResult("File not found");
            }

            var fileDto = MapToDto(file);
            return ApiResponse<FileDto>.SuccessResult(fileDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<FileDto>.ErrorResult($"Error retrieving file: {ex.Message}");
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
